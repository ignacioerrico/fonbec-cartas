using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Coordinador
{
    public interface IPlannedEventRepository
    {
        Task<List<PlannedEvent>> GetAllPlannedEventsAsync(int filialId);
        Task<PlannedEvent?> GetPlannedCartaForPreviewAsync(int plannedEventId, int filialId);
        Task<List<DateTime>> GetAllPlannedEventDates(int filialId, PlannedEventType? plannedEventType = null);
        Task<int> CreatePlannedCartaAsync(PlannedEvent plannedCarta);
        Task<int> UpdatePlannedCartaAsync(int plannedCartaId, PlannedEvent plannedCarta);
        Task<int> CreatePlannedCorteNotasAsync(PlannedEvent plannedCorteNotas);
        Task<int> UpdatePlannedCorteNotasAsync(PlannedEvent plannedCorteNotas);
    }

    public class PlannedEventRepository : IPlannedEventRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public PlannedEventRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<PlannedEvent>> GetAllPlannedEventsAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var all = await appDbContext.PlannedEvents
                .Include(p => p.PlannedDeliveries)
                .Include(p => p.CreatedByCoordinador)
                .Include(p => p.UpdatedByCoordinador)
                .Where(p => p.FilialId == filialId)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
            return all;
        }

        public async Task<PlannedEvent?> GetPlannedCartaForPreviewAsync(int plannedEventId, int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var plan = await appDbContext.PlannedEvents
                .Include(pe => pe.Filial)
                .Include(pe => pe.CartaObligatoria)
                .SingleOrDefaultAsync(pe =>
                    pe.Id == plannedEventId
                    && pe.FilialId == filialId
                    && pe.Type == PlannedEventType.CartaObligatoria);
            return plan;
        }

        public async Task<List<DateTime>> GetAllPlannedEventDates(int filialId, PlannedEventType? plannedEventType = null)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var plannedEvents = appDbContext.PlannedEvents
                .Where(pe =>
                    pe.FilialId == filialId
                    && pe.Date >= DateTime.Today);

            if (plannedEventType is not  null)
            {
                plannedEvents = plannedEvents.Where(pe => pe.Type == plannedEventType);
            }

            var takenStartDates =
                await plannedEvents
                        .Select(pe => pe.Date)
                        .ToListAsync();
            return takenStartDates;
        }

        public async Task<int> CreatePlannedCartaAsync(PlannedEvent plannedCarta)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();

            // Create planned "carta" event
            await appDbContext.PlannedEvents.AddAsync(plannedCarta);
            await appDbContext.SaveChangesAsync();

            // Create a "snapshot" of all the apadrinamientos that are expected to be active when the planned event starts
            await AddPlannedDeliveriesForApadrinamientosThatAreActiveWhenPlannedEventStarts(appDbContext, plannedCarta.Id, plannedCarta.Date);

            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdatePlannedCartaAsync(int plannedCartaId, PlannedEvent plannedCarta)
        {
            if (plannedCarta.CartaObligatoria is null)
            {
                return 0;
            }

            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var planDb = await appDbContext.PlannedEvents
                .Include(pe => pe.CartaObligatoria)
                .SingleOrDefaultAsync(pe =>
                    pe.Id == plannedCartaId
                    && pe.Type == PlannedEventType.CartaObligatoria);
            if (planDb is null)
            {
                return 0;
            }

            planDb.CartaObligatoria ??= new();
            planDb.CartaObligatoria.Subject = plannedCarta.CartaObligatoria.Subject;
            planDb.CartaObligatoria.MessageMarkdown = plannedCarta.CartaObligatoria.MessageMarkdown;
            planDb.UpdatedByCoordinadorId = plannedCarta.UpdatedByCoordinadorId;

            appDbContext.PlannedEvents.Update(planDb);

            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> CreatePlannedCorteNotasAsync(PlannedEvent plannedCorteNotas)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            
            // Create planned "notas" event
            await appDbContext.PlannedEvents.AddAsync(plannedCorteNotas);
            await appDbContext.SaveChangesAsync();

            // Create a "snapshot" of all the apadrinamientos that are expected to be active when the planned event starts
            await AddPlannedDeliveriesForApadrinamientosThatAreActiveWhenPlannedEventStarts(appDbContext, plannedCorteNotas.Id, plannedCorteNotas.Date);

            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdatePlannedCorteNotasAsync(PlannedEvent plannedCorteNotas)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();

            var plannedCorteNotasDb = await appDbContext.PlannedEvents
                .SingleOrDefaultAsync(pe =>
                    pe.Id == plannedCorteNotas.Id
                    && pe.Type == PlannedEventType.Notas);
            if (plannedCorteNotasDb is null)
            {
                return 0;
            }

            plannedCorteNotasDb.Date = plannedCorteNotas.Date;
            
            appDbContext.PlannedEvents.Update(plannedCorteNotasDb);

            await UpdatePlannedDeliveriesForApadrinamientosThatAreActiveWhenPlannedEventStarts(appDbContext, plannedCorteNotasDb.Id, plannedCorteNotas.Date);

            return await appDbContext.SaveChangesAsync();
        }

        private static async Task<List<PlannedDelivery>> GetPlannedDeliveriesForApadrinamientosThatAreActiveWhenPlannedEventStarts(
            ApplicationDbContext appDbContext,
            int plannedEventId,
            DateTime apadrinamientoActiveOn)
        {
            var plannedDeliveriesActiveWhenPlannedEventStarts = await appDbContext.Apadrinamientos
                .Include(a => a.Becario)
                .Include(a => a.Padrino)
                .Where(a =>
                    a.From.Date <= apadrinamientoActiveOn
                    && (a.To == null || apadrinamientoActiveOn <= a.To.Value.Date))
                .Select(a =>
                    new PlannedDelivery
                    {
                        PlannedEventId = plannedEventId,
                        FromBecarioId = a.BecarioId,
                        ToPadrinoId = a.PadrinoId,
                    })
                .ToListAsync();
            return plannedDeliveriesActiveWhenPlannedEventStarts;
        }

        private async Task AddPlannedDeliveriesForApadrinamientosThatAreActiveWhenPlannedEventStarts(ApplicationDbContext appDbContext, int plannedEventId, DateTime plannedEventDate)
        {
            var plannedDeliveries = await GetPlannedDeliveriesForApadrinamientosThatAreActiveWhenPlannedEventStarts(appDbContext, plannedEventId, plannedEventDate);

            await appDbContext.PlannedDeliveries.AddRangeAsync(plannedDeliveries);
        }

        private async Task UpdatePlannedDeliveriesForApadrinamientosThatAreActiveWhenPlannedEventStarts(ApplicationDbContext appDbContext, int currentPlannedEventId, DateTime newDate)
        {
            var currentPlannedDeliveries = await appDbContext.PlannedDeliveries
                .Where(pd => pd.PlannedEventId == currentPlannedEventId)
                .ToListAsync();

            var neededPlannedDeliveries = await GetPlannedDeliveriesForApadrinamientosThatAreActiveWhenPlannedEventStarts(appDbContext, currentPlannedEventId, newDate);

            // Planned deliveries that are in the database but are no longer valid (according to the new date)
            var plannedDeliveriesToRemove = currentPlannedDeliveries.ExceptBy(
                neededPlannedDeliveries.Select(npd => (npd.FromBecarioId, npd.ToPadrinoId)),
                pd => (pd.FromBecarioId, pd.ToPadrinoId));
            
            // Planned deliveries that are active on the new date, but are not in the database
            var plannedDeliveriesToAdd = neededPlannedDeliveries.ExceptBy(
                currentPlannedDeliveries.Select(cpd => (cpd.FromBecarioId, cpd.ToPadrinoId)),
                pd => (pd.FromBecarioId, pd.ToPadrinoId));

            appDbContext.PlannedDeliveries.RemoveRange(plannedDeliveriesToRemove);
            
            await appDbContext.PlannedDeliveries.AddRangeAsync(plannedDeliveriesToAdd);
        }
    }
}
