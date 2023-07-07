using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Repositories.Coordinador;
using Fonbec.Cartas.Logic.Models.Results;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Services.Coordinador
{
    public interface IPlanService
    {
        Task<List<PlansListViewModel>> GetAllPlansAsync(int filialId);
        Task<List<DateTime>> GetAllPlansStartDates(int filialId);
        Task<CrudResult> CreatePlanAsync(PlanEditViewModel planEditViewModel);
    }

    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;

        public PlanService(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<List<PlansListViewModel>> GetAllPlansAsync(int filialId)
        {
            var all = await _planRepository.GetAllPlansAsync(filialId);
            return all.Adapt<List<PlansListViewModel>>();
        }

        public async Task<List<DateTime>> GetAllPlansStartDates(int filialId)
        {
            var takenStartDates = await _planRepository.GetAllPlansStartDates(filialId);
            return takenStartDates;
        }

        public async Task<CrudResult> CreatePlanAsync(PlanEditViewModel planEditViewModel)
        {
            var plan = planEditViewModel.Adapt<Plan>();
            var rowsAffected = await _planRepository.CreatePlanAsync(plan);
            return new CrudResult(rowsAffected);
        }
    }
}
