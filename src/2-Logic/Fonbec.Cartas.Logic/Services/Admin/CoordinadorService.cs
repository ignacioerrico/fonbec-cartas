﻿using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Fonbec.Cartas.Logic.Services.Admin
{
    public class CoordinadorService : UserWithAccountService<Coordinador>
    {
        public CoordinadorService(UserWithAccountRepositoryBase<Coordinador> userWithAccountRepository,
            UserManager<FonbecUser> userManager,
            IUserStore<FonbecUser> userStore)
            : base(userWithAccountRepository,
                userManager,
                userStore)
        {
        }
    }
}
