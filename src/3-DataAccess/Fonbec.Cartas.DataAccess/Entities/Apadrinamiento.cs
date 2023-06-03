using Fonbec.Cartas.DataAccess.Entities.Actors;

namespace Fonbec.Cartas.DataAccess.Entities
{
    /// <summary>
    /// Asignación entre becario y padrino
    /// </summary>
    public class Apadrinamiento : Auditable
    {
        public int Id { get; set; }

        public int BecarioId { get; set; }
        public Becario Becario { get; set; } = default!;

        public int PadrinoId { get; set; }
        public Padrino Padrino { get; set; } = default!;

        public DateTime From { get; set; }

        public DateTime? To { get; set; }

        public int CreatedByCoordinadorId { get; set; }
        public Coordinador CreatedByCoordinador { get; set; } = default!;

        public int? UpdatedByCoordinadorId { get; set; }
        public Coordinador? UpdatedByCoordinador { get; set; }

        public int? DeletedByCoordinadorId { get; set; }
        public Coordinador? DeletedByCoordinador { get; set; }

        public bool EsAsignaciónActiva =>
            From.Date <= DateTime.Today
            && (To is null || DateTime.Today <= To.Value.Date);

        public bool EsAsignaciónFutura =>
            From.Date > DateTime.Today;
    }
}
