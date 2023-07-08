using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PadrinosListViewModel : AuditableWithAuthorNameViewModel
    {
        public int PadrinoId { get; set; }

        public List<PadrinosListBecariosActivosViewModel> BecariosActivos { get; set; } = default!;

        public string PadrinoFullName { get; set; } = default!;

        public string PadrinoEmail { get; set; } = default!;

        public List<string> Cc { get; set; } = default!;

        public List<string> Bcc { get; set; } = default!;

        public string PadrinoPhone { get; set; } = default!;

        public Gender PadrinoGender { get; set; }
    }

    public class PadrinosListBecariosActivosViewModel
    {
        public string BecarioFullName { get; set; } = default!;

        public string? BecarioEmail { get; set; }
    }

    public class PadrinosListViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Padrino, PadrinosListViewModel>()
                .Map(dest => dest.PadrinoId, src => src.Id)
                .Map(dest => dest.BecariosActivos, src => src.Apadrinamientos)
                .Map(dest => dest.PadrinoFullName, src => src.FullName(true))
                .Map(dest => dest.PadrinoGender, src => src.Gender)
                .Map(dest => dest.PadrinoEmail, src => src.Email)
                .Map(dest => dest.Cc, src => src.SendAlsoTo!.Where(sat => !sat.SendAsBcc).Select(ToPrintableString).ToList(), // [1]
                    srcCond => srcCond.SendAlsoTo != null)
                .Map(dest => dest.Bcc, src => src.SendAlsoTo!.Where(sat => sat.SendAsBcc).Select(ToPrintableString).ToList(), // [1]
                    srcCond => srcCond.SendAlsoTo != null)
                .Map(dest => dest.Cc, src => new List<string>(),
                    srcCond => srcCond.SendAlsoTo == null)
                .Map(dest => dest.Bcc, src => new List<string>(),
                    srcCond => srcCond.SendAlsoTo == null)
                .Map(dest => dest.PadrinoPhone, src => src.Phone ?? string.Empty)
                .Map(dest => dest.CreatedOnUtc, src => src.CreatedOnUtc)
                .Map(dest => dest.LastUpdatedOnUtc, src => src.LastUpdatedOnUtc)
                .Map(dest => dest.CreatedBy, src => src.CreatedByCoordinador.FullName(false))
                .Map(dest => dest.UpdatedBy, src => src.UpdatedByCoordinador!.FullName(false),
                    srcCond => srcCond.UpdatedByCoordinador != null);

            config.NewConfig<Apadrinamiento, PadrinosListBecariosActivosViewModel>()
                .Map(dest => dest.BecarioFullName, src => src.Becario.FullName(false))
                .Map(dest => dest.BecarioEmail, src => src.Becario.Email);
        }

        private string ToPrintableString(SendAlsoTo sendAlsoTo) =>
            $"{sendAlsoTo.RecipientFullName} <{sendAlsoTo.RecipientEmail}>";
    }
}

// FOOTNOTES
// [1] Interestingly, the call to ToList() is needed.
// Further reading: https://stackoverflow.com/questions/17151069/unable-to-cast-object-of-type-whereselectlistiterator-2-system-collections-gener