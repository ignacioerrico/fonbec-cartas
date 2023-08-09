using Fonbec.Cartas.DataAccess.Entities.Planning.Abstract;

namespace Fonbec.Cartas.DataAccess.Entities.Planning
{
    public class UnplannedDelivery : DeliveryBase
    {
        public int Id { get; set; }

        public int DeadlineId { get; set; }
        public Deadline Deadline { get; set; } = default!;
    }
}
