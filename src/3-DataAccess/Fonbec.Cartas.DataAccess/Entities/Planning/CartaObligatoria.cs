namespace Fonbec.Cartas.DataAccess.Entities.Planning
{
    public class CartaObligatoria
    {
        public int Id { get; set; }

        public string Subject { get; set; } = default!;

        public string MessageMarkdown { get; set; } = default!;
    }
}
