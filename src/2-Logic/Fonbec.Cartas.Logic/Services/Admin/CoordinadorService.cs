﻿using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.DataAccess.Repositories.Admin;
using Microsoft.AspNetCore.Identity;

namespace Fonbec.Cartas.Logic.Services.Admin
{
    public class CoordinadorService : UserWithAccountService<DataAccess.Entities.Actors.Coordinador>
    {
        public CoordinadorService(IUserWithAccountRepositoryBase<DataAccess.Entities.Actors.Coordinador> userWithAccountRepository,
            UserManager<FonbecUser> userManager,
            IUserStore<FonbecUser> userStore)
            : base(userWithAccountRepository,
                userManager,
                userStore)
        {
        }
    }
}
