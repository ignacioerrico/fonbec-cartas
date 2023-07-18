using Fonbec.Cartas.DataAccess.Entities.Planning;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PlansListViewModel : AuditableWithAuthorNameViewModel
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public string PlanName { get; set; } = default!;

        public int TotalLettersToSend { get; set; }

        public int LettersSent { get; set; }

        public float Percentage { get; set; }
    }

    public class PlansListViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Plan, PlansListViewModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.StartDate, src => src.StartDate)
                .Map(dest => dest.PlanName, src => $"Carta de {src.StartDate.ToPlanName()}")
                .Map(dest => dest.TotalLettersToSend, src => 0) // TODO
                .Map(dest => dest.LettersSent, src => 0) // TODO
                .Map(dest => dest.Percentage, src => 0) // TODO
                .Map(dest => dest.CreatedOnUtc, src => src.CreatedOnUtc)
                .Map(dest => dest.CreatedBy, src => src.CreatedByCoordinador.FullName(false))
                .Map(dest => dest.LastUpdatedOnUtc, src => src.LastUpdatedOnUtc)
                .Map(dest => dest.UpdatedBy, src => src.UpdatedByCoordinador!.FullName(false), srcCond => srcCond.UpdatedByCoordinador != null)
                .Map(dest => dest.UpdatedBy, src => (string?)null, srcCond => srcCond.UpdatedByCoordinador == null);
        }
    }
}
