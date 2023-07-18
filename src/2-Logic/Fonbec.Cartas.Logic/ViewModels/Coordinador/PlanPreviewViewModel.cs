using Fonbec.Cartas.DataAccess.Entities.Planning;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PlanPreviewViewModel
    {
        public DateTime StartDate { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string MessageMarkdown { get; set; } = string.Empty;
    }

    public class PlanPreviewViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Plan, PlanPreviewViewModel>()
                .Map(dest => dest.StartDate, src => src.StartDate)
                .Map(dest => dest.Subject, src => src.Subject)
                .Map(dest => dest.MessageMarkdown, src => src.MessageMarkdown);
        }
    }
}
