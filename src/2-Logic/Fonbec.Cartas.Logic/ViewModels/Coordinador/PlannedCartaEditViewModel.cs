using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PlannedCartaEditViewModel
    {
        public int FilialId { get; set; }

        public DateTime Date { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string MessageMarkdown { get; set; } = string.Empty;

        public int CreatedByCoordinadorId { get; set; }
        
        public int? UpdatedByCoordinadorId { get; set; }
    }

    public class PlannedCartaEditViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PlannedEvent, PlannedCartaEditViewModel>()
                .Map(dest => dest.FilialId, src => src.FilialId)
                .Map(dest => dest.Date, src => src.Date)
                .Map(dest => dest.Subject, src => src.CartaObligatoria!.Subject,
                    srcCond => srcCond.CartaObligatoria != null)
                .Map(dest => dest.MessageMarkdown, src => src.CartaObligatoria!.MessageMarkdown,
                    srcCond => srcCond.CartaObligatoria != null)
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CreatedByCoordinadorId)
                .Map(dest => dest.UpdatedByCoordinadorId, src => src.UpdatedByCoordinadorId);

            config.NewConfig<PlannedCartaEditViewModel, PlannedEvent>()
                .Map(dest => dest.FilialId, src => src.FilialId)
                .Map(dest => dest.Date, src => src.Date)
                .Map(dest => dest.Type, src => PlannedEventType.CartaObligatoria)
                .Map(dest => dest.CartaObligatoria!.Subject, src => src.Subject)
                .Map(dest => dest.CartaObligatoria!.MessageMarkdown, src => src.MessageMarkdown)
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CreatedByCoordinadorId)
                .Map(dest => dest.UpdatedByCoordinadorId, src => src.UpdatedByCoordinadorId);
        }
    }
}
