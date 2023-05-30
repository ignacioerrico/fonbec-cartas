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
                UserWithAccountId = coordinador.Id.ToString(),
                NickName = coordinador.NickName ?? coordinador.FirstName,
                FilialId = coordinador.FilialId.ToString(),
                FilialName = coordinador.Filial.Name,
            };

            return fonbecUserCustomClaimsModel;
        }

        public async Task<FonbecUserCustomClaimsModel> GetClaimsForMediadorAsync(string aspNetUserId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var mediador = await appDbContext.Mediadores
                .Include(c => c.Filial)
                .Where(c => EF.Functions.Like(c.AspNetUserId, aspNetUserId))
                .SingleAsync();

            var fonbecUserCustomClaimsModel = new FonbecUserCustomClaimsModel
            {
                UserWithAccountId = mediador.Id.ToString(),
                NickName = mediador.NickName ?? mediador.FirstName,
                FilialId = mediador.FilialId.ToString(),
                FilialName = mediador.Filial.Name,
            };

            return fonbecUserCustomClaimsModel;
        }

        public async Task<FonbecUserCustomClaimsModel> GetClaimsForRevisorAsync(string aspNetUserId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var revisor = await appDbContext.Revisores
                .Include(c => c.Filial)
                .Where(c => EF.Functions.Like(c.AspNetUserId, aspNetUserId))
                .SingleAsync();

            var fonbecUserCustomClaimsModel = new FonbecUserCustomClaimsModel
            {
                UserWithAccountId = revisor.Id.ToString(),
                NickName = revisor.NickName ?? revisor.FirstName,
                FilialId = revisor.FilialId.ToString(),
                FilialName = revisor.Filial.Name,
            };

            return fonbecUserCustomClaimsModel;
        }
    }
}
