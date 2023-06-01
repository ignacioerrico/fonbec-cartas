using Fonbec.Cartas.DataAccess.Projections;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public interface IBecarioRepository
    {
        Task<List<MediadorForSelectionProjection>> GetAllMediadoresForSelectionAsync(int filialId);
    }

    public class BecarioRepository : IBecarioRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public BecarioRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<MediadorForSelectionProjection>> GetAllMediadoresForSelectionAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var mediadorForSelection = await appDbContext.Mediadores
                .Where(m => m.FilialId == filialId)
                .Select(m =>
                    new MediadorForSelectionProjection
                    {
                        Id = m.Id,
                        FullName = m.FullName(true)
                    }).ToListAsync();
            return mediadorForSelection;
        }
    }
}
