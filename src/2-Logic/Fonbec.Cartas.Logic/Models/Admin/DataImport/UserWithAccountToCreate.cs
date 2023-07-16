using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Mapster;

namespace Fonbec.Cartas.Logic.Models.Admin.DataImport
{
    public class UserWithAccountToCreate
    {
        public string FirstName { get; set; } = default!;
        
        public string? LastName { get; set; } = default!;
        
        public string? NickName { get; set; } = default!;
        
        public Gender Gender { get; set; }
        
        public string Email { get; set; } = default!;
        
        public string? Phone { get; set; } = default!;
        
        public string Username { get; set; } = default!;
        
        public string Password { get; set; } = default!;
    }

    public class UserWithAccountToCreateMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserWithAccountToCreate, UserWithAccount>()
                .Include<UserWithAccountToCreate, DataAccess.Entities.Actors.Coordinador>()
                .Include<UserWithAccountToCreate, Mediador>()
                .Include<UserWithAccountToCreate, Revisor>()
                .Map(dest => dest.FilialId, src => MapContext.Current!.Parameters["filialId"],
                    _ => MapContext.Current != null)
                .Map(dest => dest.AspNetUserId, src => MapContext.Current!.Parameters["userId"],
                    _ => MapContext.Current != null)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.NickName, src => src.NickName)
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.Phone, src => src.Phone)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.Username, src => src.Username);

            config.NewConfig<UserWithAccount, UserWithAccountToCreate>()
                .Include<DataAccess.Entities.Actors.Coordinador, UserWithAccountToCreate>()
                .Include<Mediador, UserWithAccountToCreate>()
                .Include<Revisor, UserWithAccountToCreate>()
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.NickName, src => src.NickName)
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.Phone, src => src.Phone)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.Username, src => src.Username);
        }
    }
}
