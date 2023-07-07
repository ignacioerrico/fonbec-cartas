using Fonbec.Cartas.DataAccess.DataModels;
using Fonbec.Cartas.DataAccess.DataModels.Coordinador;
using Fonbec.Cartas.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Coordinador
{
    public interface IApadrinamientoRepository
    {
        Task<ApadrinamientoEditDataModel> GetApadrinamientoEditDataAsync(int filialId, int becarioId);
        Task<ApadrinamientoAssignPadrinoToBecarioDataModel> AssignPadrinoToBecarioAsync(Apadrinamiento apadrinamiento);
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
                .Where(a => a.BecarioId == becarioId)
                .Include(a => a.Padrino)
                .Include(a => a.CreatedByCoordinador)
                .Include(a => a.UpdatedByCoordinador)
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

        public async Task<ApadrinamientoAssignPadrinoToBecarioDataModel> AssignPadrinoToBecarioAsync(Apadrinamiento apadrinamiento)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Apadrinamientos.AddAsync(apadrinamiento);
            var rowsAffected = await appDbContext.SaveChangesAsync();

            var apadrinamientoAssignPadrinoToBecarioDataModel = new ApadrinamientoAssignPadrinoToBecarioDataModel
            {
                RowsAffected = rowsAffected,
                ApadrinamientoId = apadrinamiento.Id,
            };

            return apadrinamientoAssignPadrinoToBecarioDataModel;
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
