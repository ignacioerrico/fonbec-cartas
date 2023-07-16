using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.DataImport;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport
{
    public interface IDataImportRepository
    {
        Task<IEnumerable<Apadrinamiento>> GetAllApadrinamientosAsync();
        Task<IEnumerable<Becario>> GetAllBecariosAsync();
        Task<IEnumerable<Padrino>> GetAllPadrinosAsync();
        Task<IEnumerable<SendAlsoTo>> GetAllSendAlsoTosAsync();

        Task<List<Padrino>> CreatePadrinosAsync(List<Padrino> padrinos);
        Task UpdatePadrinosAsync(List<PadrinoToUpdate> padrinos, List<string> errors);
        Task<List<Becario>> CreateBecariosAsync(List<Becario> becarios);
        Task CreateApadrinamientoAsync(List<Apadrinamiento> apadrinamientos);
        
        Task<List<int>> GetExistingMediadorIdsAsync(int filialId);
        Task<(List<int> existingBecarioIds, List<int> existingPadrinoIds)> GetExistingBecarioAndPadrinoIdsAsync(int filialId);
    }

    public class DataImportRepository : IDataImportRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public DataImportRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<IEnumerable<Apadrinamiento>> GetAllApadrinamientosAsync()
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var apadrinamientos = await appDbContext.Apadrinamientos.ToListAsync();
            return apadrinamientos.AsEnumerable();
        }

        public async Task<IEnumerable<Becario>> GetAllBecariosAsync()
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var becarios = await appDbContext.Becarios.ToListAsync();
            return becarios.AsEnumerable();
        }

        public async Task<IEnumerable<Padrino>> GetAllPadrinosAsync()
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var padrinos = await appDbContext.Padrinos.ToListAsync();
            return padrinos.AsEnumerable();
        }

        public async Task<IEnumerable<SendAlsoTo>> GetAllSendAlsoTosAsync()
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var sendAlsoTos = await appDbContext.SendAlsoTo.ToListAsync();
            return sendAlsoTos.AsEnumerable();
        }

        public async Task<List<Padrino>> CreatePadrinosAsync(List<Padrino> padrinos)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Padrinos.AddRangeAsync(padrinos);
            await appDbContext.SaveChangesAsync();
            return padrinos;
        }

        public async Task UpdatePadrinosAsync(List<PadrinoToUpdate> padrinos, List<string> errors)
        {
            if (!padrinos.Any())
            {
                return;
            }

            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            
            foreach (var padrino in padrinos)
            {
                var padrinoToUpdate = await appDbContext.Padrinos.FindAsync(padrino.Id);
                if (padrinoToUpdate is null)
                {
                    var sendAlsoToList = padrino.SendAlsoTo.Select(sat => $"{sat.RecipientFullName} <{sat.RecipientEmail}>");
                    var sendAlsoToListString = string.Join(", ", sendAlsoToList);
                    errors.Add($"Padrino with ID {padrino.Id} not found in data base when trying to update SendAlsoTo with: {sendAlsoToListString}");
                    continue;
                }
                
                padrinoToUpdate.SendAlsoTo ??= new();
                padrinoToUpdate.SendAlsoTo.AddRange(padrino.SendAlsoTo);
            }

            await appDbContext.SaveChangesAsync();
        }

        public async Task<List<Becario>> CreateBecariosAsync(List<Becario> becarios)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Becarios.AddRangeAsync(becarios);
            await appDbContext.SaveChangesAsync();
            return becarios;
        }

        public async Task CreateApadrinamientoAsync(List<Apadrinamiento> apadrinamientos)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Apadrinamientos.AddRangeAsync(apadrinamientos);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<List<int>> GetExistingMediadorIdsAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var existingMediadorIds = await appDbContext.Mediadores
                .Where(m => m.FilialId == filialId)
                .Select(m => m.Id)
                .ToListAsync();
            return existingMediadorIds;
        }

        public async Task<(List<int> existingBecarioIds, List<int> existingPadrinoIds)> GetExistingBecarioAndPadrinoIdsAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var existingBecarioIds = await appDbContext.Becarios
                .Where(b => b.FilialId == filialId)
                .Select(b => b.Id)
                .ToListAsync();
            var existingPadrinoIds = await appDbContext.Padrinos
                .Where(p => p.FilialId == filialId)
                .Select(p => p.Id)
                .ToListAsync();
            return (existingBecarioIds, existingPadrinoIds);
        }
    }
}
