using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.DataAccess.Entities.Actors
{
    public class Becario : UserManagedByCoordinador
    {
        public int MediadorId { get; set; }
        public Mediador Mediador { get; set; } = default!;

        public List<Apadrinamiento> Apadrinamientos { get; set; } = default!;

        public NivelDeEstudio NivelDeEstudio { get; set; }

        public string? Email { get; set; }

        public bool EsUniversitario => NivelDeEstudio == NivelDeEstudio.Universitario;
    }
}
