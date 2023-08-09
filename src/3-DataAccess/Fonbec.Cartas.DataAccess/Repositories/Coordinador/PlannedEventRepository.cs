using Fonbec.Cartas.DataAccess.DataModels.Coordinador;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Coordinador
{
    public interface IPlannedEventRepository
    {
        Task<List<PlansListDataModel>> GetAllPlansAsync(int filialId);
        Task<PlannedEvent?> GetPlannedEventAsync(int plannedEventId, int filialId);
        Task<List<DateTime>> GetAllDeadlinesDatesAsync(int filialId);
        Task<List<DateTime>> GetAllPlannedEventDatesAsync(int filialId);
        Task<int> CreatePlannedEventAsync(PlannedEvent plannedCarta);
        Task<int> UpdatePlannedEventAsync(PlannedEvent plannedEvent);
        Task<int> CreateDeadlineAsync(Deadline deadline);
        Task<int> UpdateDeadlineAsync(Deadline deadline);
    }

    public class PlannedEventRepository : IPlannedEventRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public PlannedEventRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<PlansListDataModel>> GetAllPlansAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            
            var plannedEvents = await appDbContext.PlannedEvents
                .Include(pe => pe.PlannedDeliveries)
                .ThenInclude(pd => pd.Apadrinamiento)
                .Include(p => p.CreatedByCoordinador)
                .Include(p => p.UpdatedByCoordinador)
                .Where(pe => pe.FilialId == filialId)
                .Select(pe =>
                    new PlansListDataModel
                    {
                        PlanningId = pe.Id,
                        PlanningType = PlanningType.PlannedEvent,
                        Date = pe.StartsOn,
                        Deliveries = pe.PlannedDeliveries
                            .Select(pd =>
                                new PlansListDeliveriesDataModel
                                {
                                    Apadrinamiento = pd.Apadrinamiento,
                                    IncludesBoletínOrLibUniv = false, // TODO: pd.DocumentsFromBecario.Any(d => d.DocumentType == DocumentType.Boletín || d.DocumentType == DocumentType.LibretaUniversitaria),
                                    HasBeenSent = pd.SentOn.HasValue,
                                })
                            .ToList(),
                        CreatedOnUtc = pe.CreatedOnUtc,
                        CreatedBy = pe.CreatedByCoordinador,
                        LastUpdatedOnUtc = pe.LastUpdatedOnUtc,
                        UpdatedBy = pe.UpdatedByCoordinador,
                    })
                .ToListAsync();

            var deadlines = await appDbContext.Deadlines
                .Include(d => d.UnplannedDeliveries)
                .ThenInclude(ud => ud.Apadrinamiento)
                .Include(p => p.CreatedByCoordinador)
                .Include(p => p.UpdatedByCoordinador)
                .Where(d => d.FilialId == filialId)
                .Select(d =>
                    new PlansListDataModel
                    {
                        PlanningId = d.Id,
                        PlanningType = PlanningType.Deadline,
                        Date = d.Date,
                        Deliveries = d.UnplannedDeliveries
                            .Select(ud =>
                                new PlansListDeliveriesDataModel
                                {
                                    Apadrinamiento = ud.Apadrinamiento,
                                    IncludesBoletínOrLibUniv = true,
                                    HasBeenSent = ud.SentOn.HasValue,
                                })
                            .ToList(),
                        CreatedOnUtc = d.CreatedOnUtc,
                        CreatedBy = d.CreatedByCoordinador,
                        LastUpdatedOnUtc = d.LastUpdatedOnUtc,
                        UpdatedBy = d.UpdatedByCoordinador,
                    })
                .ToListAsync();

            var plansListDataModels = plannedEvents.Union(deadlines)
                .OrderByDescending(m => m.Date)
                .ToList();

            return plansListDataModels;
        }

        public async Task<PlannedEvent?> GetPlannedEventAsync(int plannedEventId, int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var plan = await appDbContext.PlannedEvents
                .SingleOrDefaultAsync(pe =>
                    pe.Id == plannedEventId
                    && pe.FilialId == filialId);
            return plan;
        }

        public async Task<List<DateTime>> GetAllDeadlinesDatesAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var deadlinesDates = await appDbContext.Deadlines
                .Where(pe =>
                    pe.FilialId == filialId
                    && pe.Date >= DateTime.Today)
                .Select(pe => pe.Date)
                .ToListAsync();
            return deadlinesDates;
        }

        public async Task<List<DateTime>> GetAllPlannedEventDatesAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var plannedEventDates = await appDbContext.PlannedEvents
                .Where(pe =>
                    pe.FilialId == filialId
                    && pe.StartsOn >= DateTime.Today)
                .Select(pe => pe.StartsOn)
                .ToListAsync();
            return plannedEventDates;
        }

        public async Task<int> CreatePlannedEventAsync(PlannedEvent plannedCarta)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.PlannedEvents.AddAsync(plannedCarta);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdatePlannedEventAsync(PlannedEvent plannedEvent)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();

            var plannedEventDb = await appDbContext.PlannedEvents.FindAsync(plannedEvent.Id);
            if (plannedEventDb is null)
            {
                return 0;
            }

            plannedEventDb.Subject = plannedEvent.Subject;
            plannedEventDb.MessageMarkdown = plannedEvent.MessageMarkdown;
            plannedEventDb.UpdatedByCoordinadorId = plannedEvent.UpdatedByCoordinadorId;

            appDbContext.PlannedEvents.Update(plannedEventDb);

            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> CreateDeadlineAsync(Deadline deadline)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Deadlines.AddAsync(deadline);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateDeadlineAsync(Deadline deadline)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();

            var deadlineDb = await appDbContext.Deadlines.FindAsync(deadline.Id);
            if (deadlineDb is null)
            {
                return 0;
            }

            deadlineDb.Date = deadline.Date;
            deadlineDb.UpdatedByCoordinadorId = deadline.UpdatedByCoordinadorId;
            
            appDbContext.Deadlines.Update(deadlineDb);

            return await appDbContext.SaveChangesAsync();
        }
    }
}
