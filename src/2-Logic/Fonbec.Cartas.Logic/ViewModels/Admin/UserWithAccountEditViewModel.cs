using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.Logic.ViewModels.Admin;

public class UserWithAccountEditViewModel
{
    public int FilialId { get; set; }
    
    public string FirstName { get; set; } = default!;
    
    public string LastName { get; set; } = default!;
    
    public string NickName { get; set; } = default!;
    
    public Gender Gender { get; set; }
    
    public string Email { get; set; } = default!;
    
    public string Phone { get; set; } = default!;
    
    public string Username { get; set; } = default!;
    
    public string AspNetUserId { get; set; } = default!;
}