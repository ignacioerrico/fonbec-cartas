using EntityFrameworkCore.Triggered;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Triggers
{
    public class PlannedEventAfterSaveTrigger : IAfterSaveTrigger<PlannedEvent>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public PlannedEventAfterSaveTrigger(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task AfterSave(ITriggerContext<PlannedEvent> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
            {
                await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync(cancellationToken);

                var newPlannedEvent = context.Entity;

                var plannedDeliveries = await GetPlannedDeliveriesBasedOOnApadrinamientosActiveOn(
                    appDbContext,
                    newPlannedEvent.Id,
                    newPlannedEvent.StartsOn,
                    cancellationToken);

                await appDbContext.PlannedDeliveries.AddRangeAsync(plannedDeliveries, cancellationToken);

                await appDbContext.SaveChangesAsync(cancellationToken);
            }
            else if (context.ChangeType == ChangeType.Modified)
            {
                await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync(cancellationToken);

                var updatedPlannedEvent = context.Entity;

                var oldPlannedDeliveries = await appDbContext.PlannedDeliveries
                    .Where(pd => pd.PlannedEventId == updatedPlannedEvent.Id)
                    .ToListAsync(cancellationToken);

                var newPlannedDeliveries = await GetPlannedDeliveriesBasedOOnApadrinamientosActiveOn(
                    appDbContext,
                    updatedPlannedEvent.Id,
                    updatedPlannedEvent.StartsOn,
                    cancellationToken);

                var plannedDelieveriesToRemove = oldPlannedDeliveries.Except(newPlannedDeliveries, new PlannedDeliveryComparer());

                var plannedDeliveriesToAdd = newPlannedDeliveries.Except(oldPlannedDeliveries, new PlannedDeliveryComparer());

                appDbContext.PlannedDeliveries.RemoveRange(plannedDelieveriesToRemove);

                await appDbContext.PlannedDeliveries.AddRangeAsync(plannedDeliveriesToAdd, cancellationToken);

                await appDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        private static async Task<List<PlannedDelivery>> GetPlannedDeliveriesBasedOOnApadrinamientosActiveOn(
                ApplicationDbContext appDbContext,
                int plannedEventId,
                DateTime apadrinamientoActiveOn,
                CancellationToken cancellationToken)
        {
            var plannedDeliveriesActiveWhenPlannedEventStarts = await appDbContext.Apadrinamientos
                .Where(a =>
                    a.From.Date <= apadrinamientoActiveOn.Date
                    && (a.To == null || apadrinamientoActiveOn.Date <= a.To.Value.Date))
                .Select(a =>
                    new PlannedDelivery
                    {
                        PlannedEventId = plannedEventId,
                        ApadrinamientoId = a.Id,
                    })
                .ToListAsync(cancellationToken);
            return plannedDeliveriesActiveWhenPlannedEventStarts;
        }
    }

    public class PlannedDeliveryComparer : IEqualityComparer<PlannedDelivery>
    {
        public bool Equals(PlannedDelivery? x, PlannedDelivery? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }
            
            return x.PlannedEventId == y.PlannedEventId
                   && x.ApadrinamientoId == y.ApadrinamientoId;
        }

        public int GetHashCode(PlannedDelivery obj)
        {
            return HashCode.Combine(obj.PlannedEventId, obj.ApadrinamientoId);
        }
    }
}
