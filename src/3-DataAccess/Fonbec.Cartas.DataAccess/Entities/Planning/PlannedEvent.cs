using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.DataAccess.Entities.Planning
{
    public class PlannedEvent : Auditable
    {
        public int Id { get; set; }

        public int FilialId { get; set; }
        public Filial Filial { get; set; } = default!;

        public DateTime Date { get; set; }

        public PlannedEventType Type { get; set; }

        public int? CartaObligatoriaId { get; set; }
        public CartaObligatoria? CartaObligatoria { get; set; }

        public int CreatedByCoordinadorId { get; set; }
        public Coordinador CreatedByCoordinador { get; set; } = default!;

        public int? UpdatedByCoordinadorId { get; set; }
        public Coordinador? UpdatedByCoordinador { get; set; }

        public int? DeletedByCoordinadorId { get; set; }
        public Coordinador? DeletedByCoordinador { get; set; }
    }
}
