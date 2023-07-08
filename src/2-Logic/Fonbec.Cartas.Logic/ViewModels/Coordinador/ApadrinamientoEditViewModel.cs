using Fonbec.Cartas.DataAccess.DataModels.Coordinador;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.Logic.Models;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class ApadrinamientoEditViewModel
    {
        public List<ApadrinamientoEditApadrinamientoViewModel> ApadrinamientosViewModel { get; set; } = new();

        public bool BecarioExists { get; set; }

        public string? BecarioFirstName { get; set; }

        public string? BecarioFullName { get; set; }
        
        public List<SelectableModel<int>> PadrinosForBecario { get; set; } = new();
    }

    public class ApadrinamientoEditApadrinamientoViewModel : AuditableWithAuthorNameViewModel
    {
        public ApadrinamientoEditApadrinamientoViewModel(DateTime from, DateTime? to)
        {
            From = from;
            To = to;

            if (From > DateTime.Today)
            {
                Status = "No comenzó";
            }
            else if (To.HasValue && To.Value < DateTime.Today)
            {
                Status = "Finalizó";
            }
            else
            {
                Status = "Activo";
            }
        }

        public int ApadrinamientoId { get; set; }

        public int PadrinoId { get; set; }

        public string PadrinoFullName { get; set; } = default!;

        public DateTime From { get; set; }

        public DateTime? To { get; set; }

        public string Status { get; set; }
    }

    public class ApadrinamientoEditViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Apadrinamiento, ApadrinamientoEditApadrinamientoViewModel>()
                .ConstructUsing(apadrinamiento => new ApadrinamientoEditApadrinamientoViewModel(apadrinamiento.From, apadrinamiento.To))
                .Map(dest => dest.ApadrinamientoId, src => src.Id)
                .Map(dest => dest.PadrinoId, src => src.PadrinoId)
                .Map(dest => dest.PadrinoFullName, src => src.Padrino.FullName(false))
                .Map(dest => dest.CreatedOnUtc, src => src.CreatedOnUtc)
                .Map(dest => dest.LastUpdatedOnUtc, src => src.LastUpdatedOnUtc)
                .Map(dest => dest.CreatedBy, src => src.CreatedByCoordinador.FullName(false))
                .Map(dest => dest.UpdatedBy, src => src.UpdatedByCoordinador!.FullName(false),
                    srcCond => srcCond.UpdatedByCoordinador != null);

            config.NewConfig<ApadrinamientoEditDataModel, ApadrinamientoEditViewModel>()
                .Map(dest => dest.ApadrinamientosViewModel, src => src.Apadrinamientos)
                .Map(dest => dest.BecarioExists, src => src.BecarioExists)
                .Map(dest => dest.BecarioFullName, src => src.BecarioFullName)
                .Map(dest => dest.BecarioFirstName, src => src.BecarioFirstName)
                .Map(dest => dest.PadrinosForBecario, src => src.SelectablePadrinos);
        }
    }
}
