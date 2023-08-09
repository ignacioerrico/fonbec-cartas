using Fonbec.Cartas.DataAccess.Entities.Actors;

namespace Fonbec.Cartas.DataAccess.Entities.Planning.Abstract
{
    public abstract class DeliveryBase
    {
        public int ApadrinamientoId { get; set; }
        public Apadrinamiento Apadrinamiento { get; set; } = default!;

        public int? DeliveryApprovedByRevisorId { get; set; }
        public Revisor? DeliveryApprovedByRevisor { get; set; } = default!;

        public DateTimeOffset? SentOn { get; set; }

        public bool HasBeenSent => SentOn.HasValue;
    }
}
