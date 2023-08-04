using EntityFrameworkCore.Triggered;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Triggers
{
    public class ApadrinamientoAfterSaveTrigger : IAfterSaveTrigger<Apadrinamiento>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public ApadrinamientoAfterSaveTrigger(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task AfterSave(ITriggerContext<Apadrinamiento> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
            {
                await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync(cancellationToken);

                var newApadrinamiento = context.Entity;

                var plannedDeliveriesToAdd = await appDbContext.PlannedEvents
                    .Where(pe =>
                        newApadrinamiento.From.Date <= pe.Date.Date
                        && (!newApadrinamiento.To.HasValue || pe.Date.Date < newApadrinamiento.To.Value.Date))
                    .Select(pe =>
                        new PlannedDelivery
                        {
                            PlannedEventId = pe.Id,
                            ApadrinamientoId = newApadrinamiento.Id,
                        })
                    .ToListAsync(cancellationToken);

                await appDbContext.PlannedDeliveries.AddRangeAsync(plannedDeliveriesToAdd, cancellationToken);

                await appDbContext.SaveChangesAsync(cancellationToken);
            }
            else if (context.ChangeType == ChangeType.Modified)
            {
                await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync(cancellationToken);

                var updatedApadrinamiento = context.Entity;

                var oldPlannedDeliveries = await appDbContext.PlannedDeliveries
                    .Where(pd => pd.ApadrinamientoId == updatedApadrinamiento.Id)
                    .ToListAsync(cancellationToken);

                var newPlannedDeliveries = await appDbContext.PlannedEvents
                    .Where(pe =>
                        updatedApadrinamiento.From.Date <= pe.Date.Date
                        && (!updatedApadrinamiento.To.HasValue || pe.Date.Date < updatedApadrinamiento.To.Value.Date))
                    .Select(pe =>
                        new PlannedDelivery
                        {
                            PlannedEventId = pe.Id,
                            ApadrinamientoId = updatedApadrinamiento.Id,
                        })
                    .ToListAsync(cancellationToken);

                var plannedDelieveriesToRemove = oldPlannedDeliveries.Except(newPlannedDeliveries, new PlannedDeliveryComparer());

                var plannedDeliveriesToAdd = newPlannedDeliveries.Except(oldPlannedDeliveries, new PlannedDeliveryComparer());

                appDbContext.PlannedDeliveries.RemoveRange(plannedDelieveriesToRemove);

                await appDbContext.PlannedDeliveries.AddRangeAsync(plannedDeliveriesToAdd, cancellationToken);

                await appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
