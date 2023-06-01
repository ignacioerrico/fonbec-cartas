using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.DataAccess.Entities.Actors
{
    public class Becario : EntityBase
    {
        public int MediadorId { get; set; }
        public Mediador Mediador { get; set; } = default!;

        public NivelDeEstudio NivelDeEstudio { get; set; }

        public string? Email { get; set; }

        /// <summary>
        /// Use in URLs to make it easy for benefactors (padrinos) to respond to grantees (becarios).
        /// </summary>
        public Guid BecarioGuid { get; set; }

        public int CreatedByCoordinadorId { get; set; }
        public Coordinador CreatedByCoordinador { get; set; } = default!;

        public int? UpdatedByCoordinadorId { get; set; }
        public Coordinador? UpdatedByCoordinador { get; set; }

        public int? DeletedByCoordinadorId { get; set; }
        public Coordinador? DeletedByCoordinador { get; set; }

        public bool EsUniversitario => NivelDeEstudio == NivelDeEstudio.Universitario;
    }
}
