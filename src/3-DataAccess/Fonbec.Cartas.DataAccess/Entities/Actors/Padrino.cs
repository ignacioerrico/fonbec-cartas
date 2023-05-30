namespace Fonbec.Cartas.DataAccess.Entities.Actors
{
    public class Padrino : EntityBase, IHaveEmail
    {
        public string Email { get; set; } = string.Empty;

        public List<SendAlsoTo>? SendAlsoTo { get; set; }

        public int CreatedByCoordinadorId { get; set; }
        public Coordinador CreatedByCoordinador { get; set; } = default!;

        public int? UpdatedByCoordinadorId { get; set; }
        public Coordinador? UpdatedByCoordinador { get; set; }

        public int? DeletedByCoordinadorId { get; set; }
        public Coordinador? DeletedByCoordinador { get; set; }
    }
}
