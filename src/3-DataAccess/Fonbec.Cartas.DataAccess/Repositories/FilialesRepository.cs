using Fonbec.Cartas.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public interface IFilialesRepository
    {
        Task<List<Filial>> GetAllFilialesAsync();
        Task<string?> GetFilialNameAsync(int id);
        Task<int> CreateFilialAsync(Filial filial);
        Task<int> UpdateFilialAsync(int id, string newName);
        Task<int> SoftDeleteAsync(int id);
    }

    public class FilialesRepository : IFilialesRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public FilialesRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<Filial>> GetAllFilialesAsync()
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            return await appDbContext.Filiales.ToListAsync();
        }

        public async Task<string?> GetFilialNameAsync(int id)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var filial = await appDbContext.Filiales.FindAsync(id);
            return filial?.Name;
        }

        public async Task<int> CreateFilialAsync(Filial filial)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Filiales.AddAsync(filial);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateFilialAsync(int id, string newName)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var filial = await appDbContext.Filiales.FindAsync(id);
            if (filial is null)
            {
                return 0;
            }

            filial.Name = newName;

            appDbContext.Filiales.Update(filial);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> SoftDeleteAsync(int id)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var filial = await appDbContext.Filiales.FindAsync(id);
            if (filial is null)
            {
                return 0;
            }

            filial.SoftDeletedOnUtc = DateTimeOffset.UtcNow;

            appDbContext.Filiales.Update(filial);
            return await appDbContext.SaveChangesAsync();
        }
    }
}
