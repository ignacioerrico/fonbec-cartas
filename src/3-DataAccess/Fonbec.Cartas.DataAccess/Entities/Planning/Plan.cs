using Fonbec.Cartas.DataAccess.Entities.Actors;

namespace Fonbec.Cartas.DataAccess.Entities.Planning
{
    public class Plan : Auditable
    {
        public int Id { get; set; }

        public int FilialId { get; set; }
        public Filial Filial { get; set; } = default!;

        public DateTime StartDate { get; set; }

        public string Subject { get; set; } = default!;

        public string MessageMarkdown { get; set; } = default!;

        public int CreatedByCoordinadorId { get; set; }
        public Coordinador CreatedByCoordinador { get; set; } = default!;

        public int? UpdatedByCoordinadorId { get; set; }
        public Coordinador? UpdatedByCoordinador { get; set; }

        public int? DeletedByCoordinadorId { get; set; }
        public Coordinador? DeletedByCoordinador { get; set; }
    }
}
