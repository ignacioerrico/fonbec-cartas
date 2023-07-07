using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Admin
{
    public class CoordinadorRepository : UserWithAccountRepositoryBase<Entities.Actors.Coordinador>
    {
        public CoordinadorRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
            : base(appDbContextFactory)
        {
        }
    }
}
