using Fonbec.Cartas.DataAccess.Entities.Actors;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public interface ICoordinadorRepository
    {
        Task<List<Coordinador>> GetAllCoordinadoresAsync();
        Task<Coordinador?> GetCoordinadorAsync(int coordinadorId);
        Task<int> CreateCoordinadorAsync(Coordinador coordinador);
        Task<int> UpdateCoordinadorAsync(int id, Coordinador coordinador);
    }

    public class CoordinadorRepository : ICoordinadorRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public CoordinadorRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<Coordinador>> GetAllCoordinadoresAsync()
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var coordinadores = await appDbContext.Coordinadores
                .Include(c => c.Filial)
                .ToListAsync();
            return coordinadores;
        }

        public async Task<Coordinador?> GetCoordinadorAsync(int coordinadorId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var coordinador = await appDbContext.Coordinadores
                .Include(c => c.Filial)
                .SingleOrDefaultAsync(c => c.Id == coordinadorId);
            return coordinador;
        }

        public async Task<int> CreateCoordinadorAsync(Coordinador coordinador)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Coordinadores.AddAsync(coordinador);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateCoordinadorAsync(int id, Coordinador coordinador)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var coordinadorDb = await appDbContext.Coordinadores.FindAsync(id);
            if (coordinadorDb is null)
            {
                return 0;
            }

            coordinadorDb.FirstName = coordinador.FirstName;
            coordinadorDb.LastName = coordinador.LastName;
            coordinadorDb.NickName = coordinador.NickName;
            coordinadorDb.Gender = coordinador.Gender;
            coordinadorDb.Email = coordinador.Email;
            coordinadorDb.Phone = coordinador.Phone;
            coordinadorDb.Username = coordinador.Username;

            appDbContext.Coordinadores.Update(coordinadorDb);
            return await appDbContext.SaveChangesAsync();
        }
    }
}
