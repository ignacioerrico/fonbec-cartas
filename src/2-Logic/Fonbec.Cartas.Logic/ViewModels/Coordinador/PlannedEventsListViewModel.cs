using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PlannedEventsListViewModel : AuditableWithAuthorNameViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public PlannedEventType Type { get; set; }

        public string EventName { get; set; } = default!;

        public string Explanation { get; set; } = default!;

        public int TotalToSend { get; set; }

        public int AlreadySent { get; set; }

        public float Percentage { get; set; }
    }

    public class PlansListViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PlannedEvent, PlannedEventsListViewModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Date, src => src.Date)
                .Map(dest => dest.Type, src => src.Type)
                .Map(dest => dest.EventName, src => $"Carta de {src.Date.ToPlanName()}",
                    srcCond => srcCond.Type == PlannedEventType.CartaObligatoria)
                .Map(dest => dest.EventName, src => $"{src.Date.ToPlanName()}: corte para recepción de notas",
                    srcCond => srcCond.Type == PlannedEventType.Notas)
                .Map(dest => dest.Explanation, src => $"La recepción de cartas comenzó el {src.Date:d/M/yy}",
                    srcCond => srcCond.Type == PlannedEventType.CartaObligatoria && srcCond.Date <= DateTime.Today)
                .Map(dest => dest.Explanation, src => $"La recepción de cartas se habilitará el {src.Date:d/M/yy}",
                    srcCond => srcCond.Type == PlannedEventType.CartaObligatoria && srcCond.Date > DateTime.Today)
                .Map(dest => dest.Explanation, src => $"Se recibieron notas hasta el {src.Date:d/M/yy}",
                    srcCond => srcCond.Type == PlannedEventType.Notas && srcCond.Date <= DateTime.Today)
                .Map(dest => dest.Explanation, src => $"Se recibirán notas hasta el {src.Date:d/M/yy}",
                    srcCond => srcCond.Type == PlannedEventType.Notas && srcCond.Date > DateTime.Today)
                .Map(dest => dest.TotalToSend, src => 0) // TODO
                .Map(dest => dest.AlreadySent, src => 0) // TODO
                .Map(dest => dest.Percentage, src => 0) // TODO
                .Map(dest => dest.CreatedOnUtc, src => src.CreatedOnUtc)
                .Map(dest => dest.CreatedBy, src => src.CreatedByCoordinador.FullName(false))
                .Map(dest => dest.LastUpdatedOnUtc, src => src.LastUpdatedOnUtc)
                .Map(dest => dest.UpdatedBy, src => src.UpdatedByCoordinador!.FullName(false), srcCond => srcCond.UpdatedByCoordinador != null)
                .Map(dest => dest.UpdatedBy, src => (string?)null, srcCond => srcCond.UpdatedByCoordinador == null);
        }
    }
}
