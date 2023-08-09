using Fonbec.Cartas.DataAccess.DataModels.Coordinador;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PlannedEventsListViewModel : AuditableWithAuthorNameViewModel
    {
        /// <summary>
        /// ID of a planned event or a deadline, depending on the value of <see cref="PlanningType"/>.
        /// </summary>
        public int PlanningId { get; set; }

        public DateTime Date { get; set; }

        public PlanningType PlanningType { get; set; }

        public string EventName { get; set; } = default!;

        public string Description { get; set; } = default!;

        public int TotalToSend { get; set; }

        public int AlreadySent { get; set; }

        public double Percentage { get; set; }
    }

    public class PlansListViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PlansListDataModel, PlannedEventsListViewModel>()
                .Map(dest => dest.PlanningId, src => src.PlanningId)
                .Map(dest => dest.Date, src => src.Date)
                .Map(dest => dest.PlanningType, src => src.PlanningType)

                .Map(dest => dest.EventName, src => $"Carta de {src.Date.ToPlanName()}",
                    srcCond => srcCond.PlanningType == PlanningType.PlannedEvent)
                .Map(dest => dest.EventName, src => $"Corte para recepción de notas: {src.Date.ToPlanName()}",
                    srcCond => srcCond.PlanningType == PlanningType.Deadline)
                
                .Map(dest => dest.Description, src => $"La recepción de cartas comenzó el {src.Date:d/M/yyyy}",
                    srcCond => srcCond.PlanningType == PlanningType.PlannedEvent && srcCond.Date.Date <= DateTime.Today)
                .Map(dest => dest.Description, src => $"La recepción de cartas se habilitará el {src.Date:d/M/yyyy}",
                    srcCond => srcCond.PlanningType == PlanningType.PlannedEvent && DateTime.Today < srcCond.Date.Date)
                .Map(dest => dest.Description, src => $"Se recibieron notas hasta el {src.Date:d/M/yyyy}",
                    srcCond => srcCond.PlanningType == PlanningType.Deadline && srcCond.Date.Date <= DateTime.Today)
                .Map(dest => dest.Description, src => $"Se recibirán notas hasta el {src.Date:d/M/yyyy}",
                    srcCond => srcCond.PlanningType == PlanningType.Deadline && DateTime.Today < srcCond.Date.Date)
                
                .Map(dest => dest.TotalToSend, src => src.Deliveries.Count,
                    srcCond => srcCond.PlanningType == PlanningType.PlannedEvent)
                .Map(dest => dest.AlreadySent, src => src.Deliveries.Count(pd => pd.HasBeenSent),
                    srcCond => srcCond.PlanningType == PlanningType.PlannedEvent)
                .Map(dest => dest.Percentage, src => (double)src.Deliveries.Count(pd => pd.HasBeenSent) / src.Deliveries.Count,
                    srcCond => srcCond.PlanningType == PlanningType.PlannedEvent && srcCond.Deliveries.Any())

                .Map(dest => dest.TotalToSend, src => GetDeliveries(src.PlanningId, (List<PlansListDataModel>)MapContext.Current!.Parameters["dataModels"]).Count,
                    srcCond => srcCond.PlanningType == PlanningType.Deadline)
                .Map(dest => dest.AlreadySent, src => GetDeliveries(src.PlanningId, (List<PlansListDataModel>)MapContext.Current!.Parameters["dataModels"]).Count(pd => pd.HasBeenSent),
                    srcCond => srcCond.PlanningType == PlanningType.Deadline)
                .Map(dest => dest.Percentage,
                    src => (double)GetDeliveries(src.PlanningId,
                                   (List<PlansListDataModel>)MapContext.Current!.Parameters["dataModels"])
                               .Count(pd => pd.HasBeenSent) /
                           GetDeliveries(src.PlanningId,
                                   (List<PlansListDataModel>)MapContext.Current!.Parameters["dataModels"])
                               .Count,
                    srcCond => srcCond.PlanningType == PlanningType.Deadline
                               && GetDeliveries(srcCond.PlanningId, (List<PlansListDataModel>)MapContext.Current!.Parameters["dataModels"]).Any())

                .Map(dest => dest.CreatedOnUtc, src => src.CreatedOnUtc)
                .Map(dest => dest.CreatedBy, src => src.CreatedBy.FullName(false))
                .Map(dest => dest.LastUpdatedOnUtc, src => src.LastUpdatedOnUtc,
                    srcCond => srcCond.UpdatedBy != null)
                .Map(dest => dest.UpdatedBy, src => src.UpdatedBy!.FullName(false),
                    srcCond => srcCond.UpdatedBy != null);
        }

        private List<PlansListDeliveriesDataModel> GetDeliveries(int deadlineId, List<PlansListDataModel> dataModels)
        {
            var deadlineIndex = dataModels.FindIndex(dm => dm.PlanningId == deadlineId && dm.PlanningType == PlanningType.Deadline);
            if (deadlineIndex < 0)
            {
                return new();
            }

            // Ignore duplicate planned/unplanned deliveries from the same Becario to the same Padrino
            var deliveries = new HashSet<PlansListDeliveriesDataModel>(new PlansListDeliveriesDataModelComparer());

            deliveries.UnionWith(dataModels[deadlineIndex].Deliveries);

            ++deadlineIndex;
            while (deadlineIndex < dataModels.Count
                   && dataModels[deadlineIndex].PlanningType == PlanningType.PlannedEvent)
            {
                deliveries.UnionWith(dataModels[deadlineIndex].Deliveries.Where(d => d.IncludesBoletínOrLibUniv));
                ++deadlineIndex;
            }

            return deliveries.ToList();
        }

        private sealed class PlansListDeliveriesDataModelComparer : IEqualityComparer<PlansListDeliveriesDataModel>
        {
            public bool Equals(PlansListDeliveriesDataModel? x, PlansListDeliveriesDataModel? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Apadrinamiento.Id == y.Apadrinamiento.Id;
            }

            public int GetHashCode(PlansListDeliveriesDataModel obj)
            {
                return obj.Apadrinamiento.Id.GetHashCode();
            }
        }
    }
}
