using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.Logic.ViewModels.Admin;

public class CoordinadoresListViewModel : AuditableViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Filial { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public Gender Gender { get; set; }
    public string Username { get; set; }
}