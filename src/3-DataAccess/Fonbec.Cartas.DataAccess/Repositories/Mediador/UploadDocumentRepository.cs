using Fonbec.Cartas.DataAccess.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Mediador
{
    public interface IUploadDocumentRepository
    {
        Task<List<SelectableDataModel>> GetBecariosAssignedToMediador(int mediadorId);
    }

    public class UploadDocumentRepository : IUploadDocumentRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public UploadDocumentRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<SelectableDataModel>> GetBecariosAssignedToMediador(int mediadorId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var mediador = await appDbContext.Mediadores
                .Include(m => m.Becarios)
                .SingleOrDefaultAsync(m => m.Id == mediadorId);
            return mediador?.Becarios
                       .OrderBy(b => b.FirstName)
                       .Select(b => new SelectableDataModel(b.Id, b.FullName()))
                       .ToList()
                   ?? new();
        }
    }
}
