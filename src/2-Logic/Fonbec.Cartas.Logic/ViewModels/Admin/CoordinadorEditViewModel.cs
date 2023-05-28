using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.Logic.ViewModels.Admin
{
    public class CoordinadorEditViewModel
    {
        public int FilialId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string AspNetUserId { get; set; }
    }
}
