namespace Fonbec.Cartas.DataAccess.Entities.DataImport
{
    public class PadrinoToUpdate
    {
        public int Id { get; set; }

        public List<SendAlsoTo> SendAlsoTo { get; set; } = new();
    }
}
