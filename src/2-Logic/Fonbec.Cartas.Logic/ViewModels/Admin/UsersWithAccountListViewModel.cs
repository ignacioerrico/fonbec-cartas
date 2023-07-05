using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Admin;

public class UsersWithAccountListViewModel : AuditableViewModel
{
    public int UserWithAccountId { get; set; }
    
    public string UserWithAccountFullName { get; set; } = default!;
    
    public string FilialName { get; set; } = default!;
    
    public string Email { get; set; } = default!;
    
    public string Phone { get; set; } = default!;
    
    public Gender Gender { get; set; }
    
    public string Username { get; set; } = default!;
}

public class MapUserWithAccountToUsersWithAccountListViewModel : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserWithAccount, UsersWithAccountListViewModel>()
            .Include<DataAccess.Entities.Actors.Coordinador, UsersWithAccountListViewModel>()
            .Include<Mediador, UsersWithAccountListViewModel>()
            .Include<Revisor, UsersWithAccountListViewModel>()
            .Map(dest => dest.UserWithAccountId, src => src.Id)
            .Map(dest => dest.UserWithAccountFullName, src => src.FullName(true))
            .Map(dest => dest.FilialName, src => src.Filial.Name)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone ?? string.Empty)
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.CreatedOnUtc, src => src.CreatedOnUtc)
            .Map(dest => dest.LastUpdatedOnUtc, src => src.LastUpdatedOnUtc);
    }
}