using Fonbec.Cartas.DataAccess.Entities.Actors;

namespace Fonbec.Cartas.DataAccess.Entities
{
    public class Filial : Auditable
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<Coordinador> Coordinadores { get; set; } = new();

        public List<Mediador> Mediadores { get; set; } = new();

        public List<Revisor> Revisores { get; set; } = new();

        public List<Padrino> Padrinos { get; set; } = new();

        public List<Becario> Becarios { get; set; } = new();
    }
}
