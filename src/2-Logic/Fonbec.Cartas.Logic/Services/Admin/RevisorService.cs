using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Fonbec.Cartas.Logic.Services.Admin
{
    public class RevisorService : UserWithAccountService<Revisor>
    {
        public RevisorService(UserWithAccountRepositoryBase<Revisor> userWithAccountRepository,
            UserManager<FonbecUser> userManager,
            IUserStore<FonbecUser> userStore)
            : base(userWithAccountRepository,
                userManager,
                userStore)
        {
        }
    }
}
