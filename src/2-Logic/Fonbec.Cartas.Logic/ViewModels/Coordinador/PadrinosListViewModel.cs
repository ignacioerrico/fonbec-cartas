using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PadrinosListViewModel : AuditableWithAuthorNameViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string Email { get; set; } = default!;

        public List<string> Cc { get; set; } = default!;

        public List<string> Bcc { get; set; } = default!;

        public string Phone { get; set; } = default!;

        public Gender Gender { get; set; }
    }
}
