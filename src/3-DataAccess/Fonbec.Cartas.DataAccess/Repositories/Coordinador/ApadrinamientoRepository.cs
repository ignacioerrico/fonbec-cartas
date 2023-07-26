using Fonbec.Cartas.DataAccess.DataModels;
using Fonbec.Cartas.DataAccess.DataModels.Coordinador;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Coordinador
{
    public interface IApadrinamientoRepository
    {
        Task<ApadrinamientoEditDataModel> GetApadrinamientoEditDataAsync(int filialId, int becarioId);
        Task<int> CreateApadrinamientoAsync(Apadrinamiento apadrinamiento);
        Task<int> UpdateApadrinamientoAsync(Apadrinamiento apadrinamiento);
        Task<int> SetToDateToUknownAsync(int apadrinamientoId, int coordinadorId);
        Task<int> SetToDateToTodayAsync(int apadrinamientoId, int coordinadorId);
    }

    public class ApadrinamientoRepository : IApadrinamientoRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public ApadrinamientoRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<ApadrinamientoEditDataModel> GetApadrinamientoEditDataAsync(int filialId, int becarioId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            
            var becario = await appDbContext.Becarios
                .SingleOrDefaultAsync(b =>
                    b.FilialId == filialId
                    && b.Id == becarioId);

            var selectablePadrinos = await appDbContext.Padrinos
                .Where(p => p.FilialId == filialId)
                .OrderBy(p => p.FirstName)
                .Select(p => new SelectableDataModel(p.Id, p.FullName(true)))
                .ToListAsync();

            var apadrinamientosForBecario = await appDbContext.Apadrinamientos
                .Include(a => a.Padrino)
                .Include(a => a.CreatedByCoordinador)
                .Include(a => a.UpdatedByCoordinador)
                .Where(a => a.BecarioId == becarioId)
                .ToListAsync();

            var apadrinamientoEditDataModel = new ApadrinamientoEditDataModel
            {
                BecarioExists = becario is not null,
                BecarioFullName = becario?.FullName(),
                BecarioFirstName = becario?.FirstName,
                SelectablePadrinos = selectablePadrinos,
                Apadrinamientos = apadrinamientosForBecario,
            };

            return apadrinamientoEditDataModel;
        }

        public async Task<int> CreateApadrinamientoAsync(Apadrinamiento apadrinamiento)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            
            await appDbContext.Apadrinamientos.AddAsync(apadrinamiento);

            // Add apadrinamiento to corresponding planned deliveries
            var newPlansToDeliverCartas = await appDbContext.PlannedEvents
                .Where(pe =>
                    apadrinamiento.From.Date <= pe.Date.Date
                    && (!apadrinamiento.To.HasValue || pe.Date.Date < apadrinamiento.To.Value.Date))
                .Select(pe => 
                    new PlannedDelivery
                    {
                        PlannedEventId = pe.Id,
                        FromBecarioId = apadrinamiento.BecarioId,
                        ToPadrinoId = apadrinamiento.PadrinoId,
                    })
                .ToListAsync();
            await appDbContext.PlannedDeliveries.AddRangeAsync(newPlansToDeliverCartas);
            
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateApadrinamientoAsync(Apadrinamiento apadrinamiento)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var apadrinamientoDb = await appDbContext.Apadrinamientos.FindAsync(apadrinamiento.Id);
            if (apadrinamientoDb is null)
            {
                return 0;
            }

            var originalFrom = apadrinamientoDb.From;
            var originalTo = apadrinamientoDb.To;

            apadrinamientoDb.From = apadrinamiento.From;
            apadrinamientoDb.To = apadrinamiento.To;
            apadrinamientoDb.UpdatedByCoordinadorId = apadrinamiento.UpdatedByCoordinadorId;

            appDbContext.Apadrinamientos.Update(apadrinamientoDb);

            // Remove the ones outside of the new range; add the ones not included in the old range
            await AddRemovePlannedDeliveries(appDbContext, apadrinamientoDb, originalFrom, originalTo);

            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> SetToDateToUknownAsync(int apadrinamientoId, int coordinadorId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var apadrinamientoDb = await appDbContext.Apadrinamientos.FindAsync(apadrinamientoId);
            if (apadrinamientoDb is null)
            {
                return 0;
            }

            var originalTo = apadrinamientoDb.To;

            apadrinamientoDb.To = null;
            apadrinamientoDb.UpdatedByCoordinadorId = coordinadorId;

            appDbContext.Apadrinamientos.Update(apadrinamientoDb);

            // Add the ones not included in the old range
            await AddRemovePlannedDeliveries(appDbContext, apadrinamientoDb, apadrinamientoDb.From, originalTo);

            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> SetToDateToTodayAsync(int apadrinamientoId, int coordinadorId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var apadrinamientoDb = await appDbContext.Apadrinamientos.FindAsync(apadrinamientoId);
            if (apadrinamientoDb is null)
            {
                return 0;
            }

            var originalTo = apadrinamientoDb.To;

            apadrinamientoDb.To = DateTime.Today;
            apadrinamientoDb.UpdatedByCoordinadorId = coordinadorId;

            appDbContext.Apadrinamientos.Update(apadrinamientoDb);

            // Remove or add based on the new range
            await AddRemovePlannedDeliveries(appDbContext, apadrinamientoDb, apadrinamientoDb.From, originalTo);

            return await appDbContext.SaveChangesAsync();
        }

        private static async Task AddRemovePlannedDeliveries(ApplicationDbContext appDbContext, Apadrinamiento apadrinamiento, DateTime originalFrom, DateTime? originalTo)
        {
            var originalPlannedEventIds = await appDbContext.PlannedEvents
                .Where(pe =>
                    originalFrom.Date <= pe.Date.Date
                    && (!originalTo.HasValue || pe.Date.Date < originalTo.Value.Date))
                .Select(pe => pe.Id)
                .ToListAsync();

            var newFrom = apadrinamiento.From;
            var newTo = apadrinamiento.To;

            var newPlannedEventIds = await appDbContext.PlannedEvents
                .Where(pe =>
                    newFrom.Date <= pe.Date.Date
                    && (!newTo.HasValue || pe.Date.Date < newTo.Value.Date))
                .Select(pe => pe.Id)
                .ToListAsync();

            var plannedEventIdsToRemoveFrom = originalPlannedEventIds.Except(newPlannedEventIds).ToList();

            var plannedEventIdsToAddTo = newPlannedEventIds.Except(originalPlannedEventIds).ToList();

            if (plannedEventIdsToRemoveFrom.Any())
            {
                var plannedDeliveriesToRemove = await appDbContext.PlannedDeliveries
                    .Where(pd =>
                        plannedEventIdsToRemoveFrom.Contains(pd.PlannedEventId)
                        && pd.FromBecarioId == apadrinamiento.BecarioId
                        && pd.ToPadrinoId == apadrinamiento.PadrinoId)
                    .ToListAsync();

                appDbContext.PlannedDeliveries.RemoveRange(plannedDeliveriesToRemove);
            }

            if (plannedEventIdsToAddTo.Any())
            {
                var plannedDeliveriesToAdd = plannedEventIdsToAddTo
                    .Select(id =>
                        new PlannedDelivery
                        {
                            PlannedEventId = id,
                            FromBecarioId = apadrinamiento.BecarioId,
                            ToPadrinoId = apadrinamiento.PadrinoId,
                        });

                await appDbContext.PlannedDeliveries.AddRangeAsync(plannedDeliveriesToAdd);
            }
        }
    }
}
