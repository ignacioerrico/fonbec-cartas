using Fonbec.Cartas.DataAccess.Entities.Actors;

namespace Fonbec.Cartas.DataAccess.Entities.Planning
{
    public class PlannedDelivery
    {
        public int Id { get; set; }

        public int PlannedEventId { get; set; }
        public PlannedEvent PlannedEvent { get; set; } = default!;

        public int FromBecarioId { get; set; }
        public Becario FromBecario { get; set; } = default!;

        public int ToPadrinoId { get; set; }
        public Padrino ToPadrino { get; set; } = default!;

        public DateTimeOffset? SentOn { get; set; }

        public bool HasBeenSent => SentOn.HasValue;
    }
}
