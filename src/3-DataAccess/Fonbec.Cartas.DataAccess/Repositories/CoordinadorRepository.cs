﻿using Fonbec.Cartas.DataAccess.Entities.Actors;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public class CoordinadorRepository : UserWithAccountRepositoryBase<Coordinador>
    {
        public CoordinadorRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
            : base(appDbContextFactory)
        {
        }
    }
}
