using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Admin
{
    public class MediadorRepository : UserWithAccountRepositoryBase<Entities.Actors.Mediador>
    {
        public MediadorRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
            : base(appDbContextFactory)
        {
        }
    }
}
