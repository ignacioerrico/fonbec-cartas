using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.DataAccess.Repositories.Admin;
using Microsoft.AspNetCore.Identity;

namespace Fonbec.Cartas.Logic.Services.Admin
{
    public class MediadorService : UserWithAccountService<DataAccess.Entities.Actors.Mediador>
    {
        public MediadorService(IUserWithAccountRepositoryBase<DataAccess.Entities.Actors.Mediador> userWithAccountRepository,
            UserManager<FonbecUser> userManager,
            IUserStore<FonbecUser> userStore)
            : base(userWithAccountRepository,
                userManager,
                userStore)
        {
        }
    }
}
