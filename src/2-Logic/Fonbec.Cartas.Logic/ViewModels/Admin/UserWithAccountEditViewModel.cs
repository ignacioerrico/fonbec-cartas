using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Mapster;

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

public class UserWithAccountEditViewModelMappingDefinitions : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserWithAccount, UserWithAccountEditViewModel>()
            .Include<DataAccess.Entities.Actors.Coordinador, UserWithAccountEditViewModel>()
            .Include<Mediador, UserWithAccountEditViewModel>()
            .Include<Revisor, UserWithAccountEditViewModel>()
            .Map(dest => dest.FilialId, src => src.Filial.Id)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.NickName, src => src.NickName ?? string.Empty)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone ?? string.Empty)
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.AspNetUserId, src => src.AspNetUserId);

        config.NewConfig<UserWithAccountEditViewModel, UserWithAccount>()
            .Include<UserWithAccountEditViewModel, DataAccess.Entities.Actors.Coordinador>()
            .Include<UserWithAccountEditViewModel, Mediador>()
            .Include<UserWithAccountEditViewModel, Revisor>()
            .Map(dest => dest.FilialId, src => src.FilialId)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.NickName, src => string.IsNullOrWhiteSpace(src.NickName) ? null : src.NickName)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => string.IsNullOrWhiteSpace(src.Phone) ? null : src.Phone)
            .Map(dest => dest.Username, src => MapContext.Current!.Parameters["userId"], _ => MapContext.Current != null)
            .Map(dest => dest.Username, src => string.Empty, _ => MapContext.Current == null);
    }
}