using Fonbec.Cartas.DataAccess.Entities.Planning;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PlannedCartaPreviewViewModel
    {
        public DateTime StartDate { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string MessageMarkdown { get; set; } = string.Empty;
    }

    public class PlanPreviewViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PlannedEvent, PlannedCartaPreviewViewModel>()
                .Map(dest => dest.StartDate, src => src.Date)
                .Map(dest => dest.Subject, src => src.CartaObligatoria!.Subject,
                    srcCond => srcCond.CartaObligatoria != null)
                .Map(dest => dest.MessageMarkdown, src => src.CartaObligatoria!.MessageMarkdown,
                    srcCond => srcCond.CartaObligatoria != null);
        }
    }
}
