using Fonbec.Cartas.DataAccess.Entities.Enums;

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

        public int UpdatedByCoordinadorId { get; set; }
    }
}
