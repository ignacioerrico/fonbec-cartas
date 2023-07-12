using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class BecarioEditViewModel
    {
        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        public string NickName { get; set; } = default!;

        public Gender Gender { get; set; }

        public NivelDeEstudio NivelDeEstudio { get; set; }

        public string Email { get; set; } = default!;

        public string Phone { get; set; } = default!;

        public int MediadorId { get; set; }

        public int FilialId { get; set; }

        public int CreatedByCoordinadorId { get; set; }

        public int? UpdatedByCoordinadorId { get; set; }
    }

    public class BecarioEditViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Becario, BecarioEditViewModel>()
                .Map(dest => dest.FilialId, src => src.FilialId)
                .Map(dest => dest.MediadorId, src => src.MediadorId)
                .Map(dest => dest.NivelDeEstudio, src => src.NivelDeEstudio)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName ?? string.Empty)
                .Map(dest => dest.NickName, src => src.NickName ?? string.Empty)
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.Email, src => src.Email ?? string.Empty)
                .Map(dest => dest.Phone, src => src.Phone ?? string.Empty)
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CreatedByCoordinadorId)
                .Map(dest => dest.UpdatedByCoordinadorId, src => src.UpdatedByCoordinadorId);

            config.NewConfig<BecarioEditViewModel, Becario>()
                .Map(dest => dest.FilialId, src => src.FilialId)
                .Map(dest => dest.MediadorId, src => src.MediadorId)
                .Map(dest => dest.NivelDeEstudio, src => src.NivelDeEstudio)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName,
                    srcCond => !string.IsNullOrWhiteSpace(srcCond.LastName))
                .Map(dest => dest.NickName, src => src.NickName,
                    srcCond => !string.IsNullOrWhiteSpace(srcCond.NickName))
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.Email, src => src.Email,
                    srcCond => !string.IsNullOrWhiteSpace(srcCond.Email))
                .Map(dest => dest.Phone, src => src.Phone,
                    srcCond => !string.IsNullOrWhiteSpace(srcCond.Phone))
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CreatedByCoordinadorId)
                .Map(dest => dest.UpdatedByCoordinadorId, src => src.UpdatedByCoordinadorId);
        }
    }
}
