using Fonbec.Cartas.DataAccess.Entities.Planning;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Coordinador
{
    public interface IPlanRepository
    {
        Task<List<Plan>> GetAllPlansAsync(int filialId);
        Task<Plan?> GetPlanAsync(int planId, int filialId);
        Task<List<DateTime>> GetAllPlansStartDates(int filialId);
        Task<int> CreatePlanAsync(Plan plan);
        Task<int> UpdatePlanAsync(int planId, Plan plan);
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
                .Include(p => p.CreatedByCoordinador)
                .Include(p => p.UpdatedByCoordinador)
                .Where(p => p.FilialId == filialId)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
            return all;
        }

        public async Task<Plan?> GetPlanAsync(int planId, int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var plan = await appDbContext.Planes
                .Include(p => p.Filial)
                .SingleOrDefaultAsync(p =>
                    p.Id == planId
                    && p.FilialId == filialId);
            return plan;
        }

        public async Task<List<DateTime>> GetAllPlansStartDates(int filialId)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var takenStartDates = await appDbContext.Planes
                .Where(p => p.FilialId == filialId)
                .Select(p => p.StartDate)
                .ToListAsync();
            return takenStartDates;
        }

        public async Task<int> CreatePlanAsync(Plan plan)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Planes.AddAsync(plan);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdatePlanAsync(int planId, Plan plan)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var planDb = await appDbContext.Planes
                .SingleOrDefaultAsync(p => p.Id == planId);
            if (planDb is null)
            {
                return 0;
            }

            planDb.Subject = plan.Subject;
            planDb.MessageMarkdown = plan.MessageMarkdown;
            planDb.UpdatedByCoordinadorId = plan.UpdatedByCoordinadorId;

            appDbContext.Planes.Update(planDb);

            return await appDbContext.SaveChangesAsync();
        }
    }
}
