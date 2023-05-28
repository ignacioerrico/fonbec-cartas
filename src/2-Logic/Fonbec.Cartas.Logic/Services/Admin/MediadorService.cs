using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Fonbec.Cartas.Logic.Services.Admin
{
    public class MediadorService : UserWithAccountService<Mediador>
    {
        public MediadorService(UserWithAccountRepositoryBase<Mediador> userWithAccountRepository,
            UserManager<FonbecUser> userManager,
            IUserStore<FonbecUser> userStore)
            : base(userWithAccountRepository,
                userManager,
                userStore)
        {
        }
    }
}
