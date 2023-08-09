using Fonbec.Cartas.DataAccess.Entities.Planning;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PlannedEventEditViewModel
    {
        public int PlannedEventId { get; set; }

        public int FilialId { get; set; }

        public DateTime Date { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string MessageMarkdown { get; set; } = string.Empty;

        public int CreatedByCoordinadorId { get; set; }
        
        public int? UpdatedByCoordinadorId { get; set; }
    }

    public class PlannedEventEditViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PlannedEvent, PlannedEventEditViewModel>()
                .Map(dest => dest.PlannedEventId, src => src.Id)
                .Map(dest => dest.FilialId, src => src.FilialId)
                .Map(dest => dest.Date, src => src.StartsOn)
                .Map(dest => dest.Subject, src => src.Subject)
                .Map(dest => dest.MessageMarkdown, src => src.MessageMarkdown)
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CreatedByCoordinadorId)
                .Map(dest => dest.UpdatedByCoordinadorId, src => src.UpdatedByCoordinadorId);

            config.NewConfig<PlannedEventEditViewModel, PlannedEvent>()
                .Map(dest => dest.Id, src => src.PlannedEventId)
                .Map(dest => dest.FilialId, src => src.FilialId)
                .Map(dest => dest.StartsOn, src => src.Date)
                .Map(dest => dest.Subject, src => src.Subject)
                .Map(dest => dest.MessageMarkdown, src => src.MessageMarkdown)
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CreatedByCoordinadorId)
                .Map(dest => dest.UpdatedByCoordinadorId, src => src.UpdatedByCoordinadorId);
        }
    }
}
