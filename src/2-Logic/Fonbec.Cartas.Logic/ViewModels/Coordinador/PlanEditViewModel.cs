using Fonbec.Cartas.DataAccess.Entities.Planning;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PlanEditViewModel
    {
        public int FilialId { get; set; }

        public DateTime StartDate { get; set; }

        public string Subject { get; set; } = "Carta de tu {ahijado} {ahijado:nombre} de {mes-de-carta}";

        public string MessageMarkdown { get; set; } = string.Empty;

        public int CreatedByCoordinadorId { get; set; }
        
        public int? UpdatedByCoordinadorId { get; set; }
    }

    public class PlanEditViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PlanEditViewModel, Plan>()
                .TwoWays()
                .Map(dest => dest.FilialId, src => src.FilialId)
                .Map(dest => dest.StartDate, src => src.StartDate)
                .Map(dest => dest.Subject, src => src.Subject)
                .Map(dest => dest.MessageMarkdown, src => src.MessageMarkdown)
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CreatedByCoordinadorId)
                .Map(dest => dest.UpdatedByCoordinadorId, src => src.UpdatedByCoordinadorId);
        }
    }
}
