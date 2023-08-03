using Fonbec.Cartas.DataAccess.DataModels;
using Fonbec.Cartas.DataAccess.DataModels.Coordinador;
using Fonbec.Cartas.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Coordinador
{
    public interface IApadrinamientoRepository
    {
        Task<ApadrinamientoEditDataModel> GetApadrinamientoEditDataAsync(int filialId, int becarioId);
        Task<int> CreateApadrinamientoAsync(Apadrinamiento apadrinamiento);
        Task<int> UpdateApadrinamientoAsync(Apadrinamiento apadrinamiento);
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

        public async Task<ApadrinamientoEditDataModel> GetApadrinamientoEditDataAsync(int filialId, int becarioId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            
            var becario = await appDbContext.Becarios
                .SingleOrDefaultAsync(b =>
                    b.FilialId == filialId
                    && b.Id == becarioId);

            var selectablePadrinos = await appDbContext.Padrinos
                .Where(p => p.FilialId == filialId)
                .OrderBy(p => p.FirstName)
                .Select(p => new SelectableDataModel(p.Id, p.FullName(true)))
                .ToListAsync();

            var apadrinamientosForBecario = await appDbContext.Apadrinamientos
                .Include(a => a.Padrino)
                .Include(a => a.CreatedByCoordinador)
                .Include(a => a.UpdatedByCoordinador)
                .Where(a => a.BecarioId == becarioId)
                .ToListAsync();

            var apadrinamientoEditDataModel = new ApadrinamientoEditDataModel
            {
                BecarioExists = becario is not null,
                BecarioFullName = becario?.FullName(),
                BecarioFirstName = becario?.FirstName,
                SelectablePadrinos = selectablePadrinos,
                Apadrinamientos = apadrinamientosForBecario,
            };

            return apadrinamientoEditDataModel;
        }

        public async Task<int> CreateApadrinamientoAsync(Apadrinamiento apadrinamiento)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Apadrinamientos.AddAsync(apadrinamiento);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateApadrinamientoAsync(Apadrinamiento apadrinamiento)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var apadrinamientoDb = await appDbContext.Apadrinamientos.FindAsync(apadrinamiento.Id);
            if (apadrinamientoDb is null)
            {
                return 0;
            }

            apadrinamientoDb.From = apadrinamiento.From;
            apadrinamientoDb.To = apadrinamiento.To;
            apadrinamientoDb.UpdatedByCoordinadorId = apadrinamiento.UpdatedByCoordinadorId;

            appDbContext.Apadrinamientos.Update(apadrinamientoDb);

            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> SetToDateToUknownAsync(int apadrinamientoId, int coordinadorId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var apadrinamientoDb = await appDbContext.Apadrinamientos.FindAsync(apadrinamientoId);
            if (apadrinamientoDb is null)
            {
                return 0;
            }

            apadrinamientoDb.To = null;
            apadrinamientoDb.UpdatedByCoordinadorId = coordinadorId;

            appDbContext.Apadrinamientos.Update(apadrinamientoDb);

            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> SetToDateToTodayAsync(int apadrinamientoId, int coordinadorId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var apadrinamientoDb = await appDbContext.Apadrinamientos.FindAsync(apadrinamientoId);
            if (apadrinamientoDb is null)
            {
                return 0;
            }

            apadrinamientoDb.To = DateTime.Today;
            apadrinamientoDb.UpdatedByCoordinadorId = coordinadorId;

            appDbContext.Apadrinamientos.Update(apadrinamientoDb);

            return await appDbContext.SaveChangesAsync();
        }
    }
}
