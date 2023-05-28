﻿using Fonbec.Cartas.DataAccess.Entities.Actors;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public class RevisorRepository : UserWithAccountRepositoryBase<Revisor>
    {
        public RevisorRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
            : base(appDbContextFactory)
        {
        }
    }
}
