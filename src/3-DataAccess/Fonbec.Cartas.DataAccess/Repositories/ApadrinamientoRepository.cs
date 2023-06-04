using Fonbec.Cartas.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public interface IApadrinamientoRepository
    {
        Task<List<Apadrinamiento>> GetAllPadrinosForBecario(int becarioId);
        Task<int> AssignPadrinoToBecarioAsync(Apadrinamiento apadrinamiento);
        Task<int> UpdateApadrinamientoAsync(int apadrinamientoId, DateTime from, DateTime? to, int coordinadorId);
        Task<int> SetToDateToUknownAsync(int apadrinamientoId, int coordinadorId);
        Task<int> SetToDateToTodayAsync(int apadrinamientoId, int coordinadorId);
    }

    public class ApadrinamientoRepository : IApadrinamientoRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public ApadrinamientoRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<Apadrinamiento>> GetAllPadrinosForBecario(int becarioId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var apadrinamientosForBecario = await appDbContext.Apadrinamientos
                .Where(a => a.BecarioId == becarioId)
                .Include(a => a.Padrino)
                .Include(a => a.CreatedByCoordinador)
                .Include(a => a.UpdatedByCoordinador)
                .ToListAsync();
            return apadrinamientosForBecario;
        }

        public async Task<int> AssignPadrinoToBecarioAsync(Apadrinamiento apadrinamiento)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Apadrinamientos.AddAsync(apadrinamiento);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateApadrinamientoAsync(int apadrinamientoId, DateTime from, DateTime? to, int coordinadorId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var apadrinamiento = await appDbContext.Apadrinamientos.FindAsync(apadrinamientoId);
            if (apadrinamiento is null)
            {
                return 0;
            }

            apadrinamiento.From = from;
            apadrinamiento.To = to;
            apadrinamiento.UpdatedByCoordinadorId = coordinadorId;
            appDbContext.Apadrinamientos.Update(apadrinamiento);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> SetToDateToUknownAsync(int apadrinamientoId, int coordinadorId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var apadrinamiento = await appDbContext.Apadrinamientos.FindAsync(apadrinamientoId);
            if (apadrinamiento is null)
            {
                return 0;
            }

            apadrinamiento.To = null;
            apadrinamiento.UpdatedByCoordinadorId = coordinadorId;
            appDbContext.Apadrinamientos.Update(apadrinamiento);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> SetToDateToTodayAsync(int apadrinamientoId, int coordinadorId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var apadrinamiento = await appDbContext.Apadrinamientos.FindAsync(apadrinamientoId);
            if (apadrinamiento is null)
            {
                return 0;
            }

            apadrinamiento.To = DateTime.Today;
            apadrinamiento.UpdatedByCoordinadorId = coordinadorId;
            appDbContext.Apadrinamientos.Update(apadrinamiento);
            return await appDbContext.SaveChangesAsync();
        }
    }
}
