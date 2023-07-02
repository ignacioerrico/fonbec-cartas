using Fonbec.Cartas.DataAccess.DataModels.Admin;
using Fonbec.Cartas.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Admin
{
    public interface IFilialesRepository
    {
        Task<List<FilialesListDataModel>> GetAllFilialesAsync();
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

        public async Task<List<FilialesListDataModel>> GetAllFilialesAsync()
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var filialesListDataModel = await appDbContext.Filiales
                .Include(f => f.Coordinadores)
                .Include(f => f.Mediadores)
                .Include(f => f.Revisores)
                .Include(f => f.Padrinos)
                .Include(f => f.Becarios)
                .Select(f => new FilialesListDataModel
                {
                    FilialId = f.Id,
                    FilialName = f.Name,
                    Coordinadores = f.Coordinadores
                        .OrderBy(c => c.FirstName)
                        .Select(c => c.FullName(false))
                        .ToList(),
                    QtyMediadores = f.Mediadores.Count,
                    QtyRevisores = f.Revisores.Count,
                    QtyPadrinos = f.Padrinos.Count,
                    QtyBecarios = f.Becarios.Count,
                    CreatedOnUtc = f.CreatedOnUtc,
                    LastUpdatedOnUtc = f.LastUpdatedOnUtc,
                })
                .ToListAsync();
            return filialesListDataModel;
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
