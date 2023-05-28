using Fonbec.Cartas.DataAccess.Entities.Actors;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public class MediadorRepository : UserWithAccountRepositoryBase<Mediador>
    {
        public MediadorRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
            : base(appDbContextFactory)
        {
        }
    }
}
