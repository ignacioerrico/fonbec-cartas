using Fonbec.Cartas.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public interface IPlanRepository
    {
        Task<List<Plan>> GetAllPlansAsync(int filialId);
        Task<int> CreatePlanAsync(Plan plan);
    }

    public class PlanRepository : IPlanRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public PlanRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<Plan>> GetAllPlansAsync(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var all = await appDbContext.Planes
                .Where(p => p.FilialId == filialId)
                .Include(p => p.CreatedByCoordinador)
                .Include(p => p.UpdatedByCoordinador)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
            return all;
        }

        public async Task<int> CreatePlanAsync(Plan plan)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Planes.AddAsync(plan);
            return await appDbContext.SaveChangesAsync();
        }
    }
}
