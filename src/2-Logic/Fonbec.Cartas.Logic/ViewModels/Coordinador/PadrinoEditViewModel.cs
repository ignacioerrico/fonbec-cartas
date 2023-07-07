using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PadrinoEditViewModel
    {
        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        public string NickName { get; set; } = default!;

        public Gender Gender { get; set; }

        public string Email { get; set; } = default!;

        public List<PadrinoEditSendAlsoToViewModel> SendAlsoTo { get; set; } = new();

        public string Phone { get; set; } = default!;

        public int FilialId { get; set; }

        public int CreatedByCoordinadorId { get; set; }
        
        public int? UpdatedByCoordinadorId { get; set; }
    }

    public class PadrinoEditSendAlsoToViewModel
    {
        public string RecipientFullName { get; set; } = string.Empty;

        public string RecipientEmail { get; set; } = string.Empty;

        public bool SendAsBcc { get; set; }
    }

    public class PadrinoEditViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Padrino, PadrinoEditViewModel>()
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.NickName, src => src.NickName ?? string.Empty)
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.SendAlsoTo, src => src.SendAlsoTo,
                    srcCond => srcCond.SendAlsoTo != null)
                .Map(dest => dest.Phone, src => src.Phone ?? string.Empty)
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CreatedByCoordinadorId)
                .Map(dest => dest.UpdatedByCoordinadorId, src => src.UpdatedByCoordinadorId);

            config.NewConfig<PadrinoEditViewModel, Padrino>()
                .Map(dest => dest.FilialId, src => src.FilialId)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.NickName, src => src.NickName,
                    srcCond => !string.IsNullOrWhiteSpace(srcCond.NickName))
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.SendAlsoTo, src => src.SendAlsoTo,
                    srcCond => srcCond.SendAlsoTo.Any())
                .Map(dest => dest.Phone, src => src.Phone,
                    srcCond => !string.IsNullOrWhiteSpace(srcCond.Phone))
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CreatedByCoordinadorId)
                .Map(dest => dest.UpdatedByCoordinadorId, src => src.UpdatedByCoordinadorId);

            config.NewConfig<SendAlsoTo, PadrinoEditSendAlsoToViewModel>()
                .TwoWays()
                .Map(dest => dest.RecipientFullName, src => src.RecipientFullName)
                .Map(dest => dest.RecipientEmail, src => src.RecipientEmail)
                .Map(dest => dest.SendAsBcc, src => src.SendAsBcc);
        }
    }
}
