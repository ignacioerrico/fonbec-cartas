using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.DataAccess.DataModels.Coordinador
{
    public class PlansListDataModel
    {
        public int PlanningId { get; set; }

        public PlanningType PlanningType { get; set; }

        public DateTime Date { get; set; }

        public List<PlansListDeliveriesDataModel> Deliveries { get; set; } = new();

        public DateTimeOffset CreatedOnUtc { get; set; }

        public Entities.Actors.Coordinador CreatedBy { get; set; } = default!;

        public DateTimeOffset? LastUpdatedOnUtc { get; set; }

        public Entities.Actors.Coordinador? UpdatedBy { get; set; } = default!;
    }

    public class PlansListDeliveriesDataModel
    {
        public Apadrinamiento Apadrinamiento { get; set; } = default!;

        public bool IncludesBoletínOrLibUniv { get; set; }

        public bool HasBeenSent { get; set; }
    }
}
