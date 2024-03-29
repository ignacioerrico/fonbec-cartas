﻿using Fonbec.Cartas.DataAccess.Entities.Actors;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Coordinador
{
    public interface IPadrinoRepository
    {
        Task<List<Padrino>> GetAllPadrinosAsync(int filialId);
        Task<Padrino?> GetPadrinoAsync(int padrinoId, int filialId);
        Task<int> CreateAsync(Padrino padrino);
        Task<int> UpdateAsync(int id, Padrino padrino);
    }

    public class PadrinoRepository : IPadrinoRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public PadrinoRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<Padrino>> GetAllPadrinosAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var all = await appDbContext.Padrinos
                .Include(p =>
                    p.Apadrinamientos
                        .Where(a => a.From.Date <= DateTime.Today
                                    && (a.To == null || DateTime.Today <= a.To.Value.Date))
                        .OrderBy(a => a.Becario.FirstName))
                .ThenInclude(a => a.Becario)
                .Include(p => p.SendAlsoTo)
                .Include(p => p.CreatedByCoordinador)
                .Include(p => p.UpdatedByCoordinador)
                .Where(p => p.FilialId == filialId)
                .ToListAsync();
            return all;
        }

        public async Task<Padrino?> GetPadrinoAsync(int padrinoId, int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var padrino = await appDbContext.Padrinos
                .Include(p => p.SendAlsoTo)
                .SingleOrDefaultAsync(p =>
                    p.Id == padrinoId
                    && p.FilialId == filialId);
            return padrino;
        }

        public async Task<int> CreateAsync(Padrino padrino)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Padrinos.AddAsync(padrino);

            if (padrino.SendAlsoTo is not null && padrino.SendAlsoTo.Any())
            {
                await appDbContext.SendAlsoTo.AddRangeAsync(padrino.SendAlsoTo);
            }

            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(int id, Padrino padrino)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var padrinoDb = await appDbContext.Padrinos
                .Include(p => p.SendAlsoTo)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (padrinoDb is null)
            {
                return 0;
            }

            padrinoDb.FirstName = padrino.FirstName;
            padrinoDb.LastName = padrino.LastName;
            padrinoDb.NickName = padrino.NickName;
            padrinoDb.Gender = padrino.Gender;
            padrinoDb.Email = padrino.Email;
            padrinoDb.Phone = padrino.Phone;
            padrinoDb.UpdatedByCoordinadorId = padrino.UpdatedByCoordinadorId;

            if (padrinoDb.SendAlsoTo is not null && padrinoDb.SendAlsoTo.Any())
            {
                appDbContext.SendAlsoTo.RemoveRange(padrinoDb.SendAlsoTo);
            }

            padrinoDb.SendAlsoTo = padrino.SendAlsoTo;

            appDbContext.Padrinos.Update(padrinoDb);

            return await appDbContext.SaveChangesAsync();
        }
    }
}
