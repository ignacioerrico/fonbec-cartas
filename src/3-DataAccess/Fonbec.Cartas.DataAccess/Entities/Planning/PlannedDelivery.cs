using Fonbec.Cartas.DataAccess.Entities.Actors;

namespace Fonbec.Cartas.DataAccess.Entities.Planning
{
    public class PlannedDelivery
    {
        public int Id { get; set; }

        public int PlannedEventId { get; set; }
        public PlannedEvent PlannedEvent { get; set; } = default!;

        public int ApadrinamientoId { get; set; }
        public Apadrinamiento Apadrinamiento { get; set; } = default!;

        public int? DeliveryApprovedByRevisorId { get; set; }
        public Revisor? DeliveryApprovedByRevisor { get; set; } = default!;

        public DateTimeOffset? SentOn { get; set; }

        public bool HasBeenSent => SentOn.HasValue;
    }
}
