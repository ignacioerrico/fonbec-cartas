using Fonbec.Cartas.DataAccess.Projections;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public interface IIdentityRepository
    {
        Task<FonbecUserCustomClaimsModel> GetClaimsForCoordinadorAsync(string aspNetUserId);
        Task<FonbecUserCustomClaimsModel> GetClaimsForMediadorAsync(string aspNetUserId);
        Task<FonbecUserCustomClaimsModel> GetClaimsForRevisorAsync(string aspNetUserId);
    }

    public class IdentityRepository : IIdentityRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public IdentityRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<FonbecUserCustomClaimsModel> GetClaimsForCoordinadorAsync(string aspNetUserId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var coordinador = await appDbContext.Coordinadores
                .Include(c => c.Filial)
                .Where(c => EF.Functions.Like(c.AspNetUserId, aspNetUserId))
                .SingleAsync();

            var fonbecUserCustomClaimsModel = new FonbecUserCustomClaimsModel
            {
                NickName = coordinador.NickName ?? coordinador.FirstName,
                FilialId = coordinador.FilialId.ToString(),
            };

            return fonbecUserCustomClaimsModel;
        }

        public async Task<FonbecUserCustomClaimsModel> GetClaimsForMediadorAsync(string aspNetUserId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var coordinador = await appDbContext.Mediadores
                .Include(c => c.Filial)
                .Where(c => EF.Functions.Like(c.AspNetUserId, aspNetUserId))
                .SingleAsync();

            var fonbecUserCustomClaimsModel = new FonbecUserCustomClaimsModel
            {
                NickName = coordinador.NickName ?? coordinador.FirstName,
                FilialId = coordinador.FilialId.ToString(),
            };

            return fonbecUserCustomClaimsModel;
        }

        public async Task<FonbecUserCustomClaimsModel> GetClaimsForRevisorAsync(string aspNetUserId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var coordinador = await appDbContext.Revisores
                .Include(c => c.Filial)
                .Where(c => EF.Functions.Like(c.AspNetUserId, aspNetUserId))
                .SingleAsync();

            var fonbecUserCustomClaimsModel = new FonbecUserCustomClaimsModel
            {
                NickName = coordinador.NickName ?? coordinador.FirstName,
                FilialId = coordinador.FilialId.ToString(),
            };

            return fonbecUserCustomClaimsModel;
        }
    }
}
