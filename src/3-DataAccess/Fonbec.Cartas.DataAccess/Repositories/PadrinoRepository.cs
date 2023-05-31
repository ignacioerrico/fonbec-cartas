using Fonbec.Cartas.DataAccess.Entities.Actors;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public interface IPadrinoRepository
    {
        Task<List<Padrino>> GetAllPadrinosAsync(int filialId);
        Task<Padrino?> GetPadrinoAsync(int padrinoId, int filialId);
        Task<int> CreateAsync(Padrino padrino);
        Task<int> UpdateAsync(int id, Padrino padrino);
    }

    public class PadrinoRepository : IPadrinoRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public PadrinoRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<Padrino>> GetAllPadrinosAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var all = await appDbContext.Padrinos
                .Where(p => p.FilialId == filialId)
                .Include(p => p.SendAlsoTo)
                .Include(p => p.CreatedByCoordinador)
                .Include(p => p.UpdatedByCoordinador)
                .Include(p => p.DeletedByCoordinador)
                .ToListAsync();
            return all;
        }

        public async Task<Padrino?> GetPadrinoAsync(int padrinoId, int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var padrino = await appDbContext.Padrinos
                .Where(p => p.FilialId == filialId)
                .Include(p => p.SendAlsoTo)
                .SingleOrDefaultAsync(p => p.Id == padrinoId);
            return padrino;
        }

        public async Task<int> CreateAsync(Padrino padrino)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Padrinos.AddAsync(padrino);

            if (padrino.SendAlsoTo is not null && padrino.SendAlsoTo.Any())
            {
                await appDbContext.SendAlsoTo.AddRangeAsync(padrino.SendAlsoTo);
            }

            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(int id, Padrino padrino)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var padrinoDb = await appDbContext.Padrinos.FindAsync(id);
            if (padrinoDb is null)
            {
                return 0;
            }

            padrinoDb.FirstName = padrino.FirstName;
            padrinoDb.LastName = padrino.LastName;
            padrinoDb.NickName = padrino.NickName;
            padrinoDb.Gender = padrino.Gender;
            padrinoDb.Email = padrino.Email;
            padrinoDb.Phone = padrino.Phone;
            padrinoDb.UpdatedByCoordinadorId = padrino.UpdatedByCoordinadorId;

            appDbContext.Padrinos.Update(padrinoDb);

            if (padrinoDb.SendAlsoTo is not null && padrinoDb.SendAlsoTo.Any())
            {
                appDbContext.SendAlsoTo.RemoveRange(padrinoDb.SendAlsoTo);
            }

            if (padrino.SendAlsoTo is not null && padrino.SendAlsoTo.Any())
            {
                await appDbContext.SendAlsoTo.AddRangeAsync(padrino.SendAlsoTo);
            }

            return await appDbContext.SaveChangesAsync();
        }
    }
}
