using Fonbec.Cartas.DataAccess.DataModels;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Coordinador
{
    public interface IBecarioRepository
    {
        Task<List<SelectableDataModel>> GetAllMediadoresForSelectionAsync(int filialId);
        Task<List<Becario>> GetAllBecariosAsync(int filialId);
        Task<Becario?> GetBecarioAsync(int becarioId, int filialId);
        Task<int> CreateAsync(Becario becario);
        Task<int> UpdateAsync(int becarioId, Becario becario);
    }

    public class BecarioRepository : IBecarioRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public BecarioRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<SelectableDataModel>> GetAllMediadoresForSelectionAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var mediadorForSelection = await appDbContext.Mediadores
                .Where(m => m.FilialId == filialId)
                .OrderBy(m => m.FirstName)
                .Select(m => new SelectableDataModel(m.Id, m.FullName(true)))
                .ToListAsync();
            return mediadorForSelection;
        }

        public async Task<List<Becario>> GetAllBecariosAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var all = await appDbContext.Becarios
                .Include(b => b.Mediador)
                .Include(b => b.Apadrinamientos)
                .ThenInclude(a => a.Padrino)
                .Include(b => b.CreatedByCoordinador)
                .Include(b => b.UpdatedByCoordinador)
                .Where(b => b.FilialId == filialId)
                .OrderBy(b => b.FirstName)
                .ToListAsync();
            return all;
        }

        public async Task<Becario?> GetBecarioAsync(int becarioId, int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var becario = await appDbContext.Becarios
                .Where(b => b.FilialId == filialId)
                .SingleOrDefaultAsync(b => b.Id == becarioId);
            return becario;
        }

        public async Task<int> CreateAsync(Becario becario)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Becarios.AddAsync(becario);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(int becarioId, Becario becario)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var becarioDb = await appDbContext.Becarios.FindAsync(becarioId);
            if (becarioDb is null)
            {
                return 0;
            }

            becarioDb.MediadorId = becario.MediadorId;
            becarioDb.NivelDeEstudio = becario.NivelDeEstudio;
            becarioDb.FirstName = becario.FirstName;
            becarioDb.LastName = becario.LastName;
            becarioDb.NickName = becario.NickName;
            becarioDb.Gender = becario.Gender;
            becarioDb.Email = becario.Email;
            becarioDb.Phone = becario.Phone;
            becarioDb.UpdatedByCoordinadorId = becario.UpdatedByCoordinadorId;

            appDbContext.Becarios.Update(becarioDb);

            return await appDbContext.SaveChangesAsync();
        }
    }
}
