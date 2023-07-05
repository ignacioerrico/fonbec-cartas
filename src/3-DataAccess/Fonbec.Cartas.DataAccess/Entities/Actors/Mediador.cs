using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;

namespace Fonbec.Cartas.DataAccess.Entities.Actors
{
    public class Mediador : UserWithAccount
    {
        public List<Becario> Becarios { get; set; } = new();
    }
}
