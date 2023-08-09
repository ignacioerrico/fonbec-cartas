using Fonbec.Cartas.DataAccess.Entities.Planning.Abstract;

namespace Fonbec.Cartas.DataAccess.Entities.Planning
{
    public class PlannedDelivery : DeliveryBase
    {
        public int Id { get; set; }

        public int PlannedEventId { get; set; }
        public PlannedEvent PlannedEvent { get; set; } = default!;

        //public List<DocumentFromBecario> DocumentsFromBecario { get; set; } = new();
    }
}
