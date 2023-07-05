using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;

namespace Fonbec.Cartas.DataAccess.Entities.Actors
{
    public class Padrino : UserManagedByCoordinador
    {
        public List<Apadrinamiento> Apadrinamientos { get; set; } = default!;

        public string Email { get; set; } = string.Empty;

        public List<SendAlsoTo>? SendAlsoTo { get; set; }
    }
}
