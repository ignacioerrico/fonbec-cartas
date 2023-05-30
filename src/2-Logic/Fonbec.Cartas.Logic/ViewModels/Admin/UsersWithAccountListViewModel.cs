using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.Logic.ViewModels.Admin;

public class UsersWithAccountListViewModel : AuditableViewModel
{
    public int Id { get; set; }
    
    public string Name { get; set; } = default!;
    
    public string Filial { get; set; } = default!;
    
    public string Email { get; set; } = default!;
    
    public string Phone { get; set; } = default!;
    
    public Gender Gender { get; set; }
    
    public string Username { get; set; } = default!;
}