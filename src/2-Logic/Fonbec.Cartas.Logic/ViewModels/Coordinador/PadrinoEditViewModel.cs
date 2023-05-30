using Fonbec.Cartas.DataAccess.Entities.Enums;

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
        
        public int UpdatedByCoordinadorId { get; set; }
    }

    public class PadrinoEditSendAlsoToViewModel
    {
        public string RecipientFullName { get; set; } = string.Empty;

        public string RecipientEmail { get; set; } = string.Empty;

        public bool SendAsBcc { get; set; }
    }
}
