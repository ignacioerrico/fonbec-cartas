using Fonbec.Cartas.DataAccess.Entities.Planning;
using Fonbec.Cartas.DataAccess.Repositories.Coordinador;
using Fonbec.Cartas.Logic.Models.Results;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Services.Coordinador
{
    public interface IPlanService
    {
        Task<List<PlansListViewModel>> GetAllPlansAsync(int filialId);
        Task<SearchResult<PlanEditViewModel>> GetPlanAsync(int planId, int filialId);
        Task<SearchResult<PlanPreviewViewModel>> GetPlanForPreviewAsync(int planId, int filialId);
        Task<List<DateTime>> GetAllPlansStartDates(int filialId);
        Task<CrudResult> CreatePlanAsync(PlanEditViewModel planEditViewModel);
        Task<CrudResult> UpdatePlanAsync(int planId, PlanEditViewModel planEditViewModel);
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

        public async Task<SearchResult<PlanEditViewModel>> GetPlanAsync(int planId, int filialId)
        {
            var plan = await _planRepository.GetPlanAsync(planId, filialId);
            var planEditViewModel = plan?.Adapt<PlanEditViewModel>();
            return new SearchResult<PlanEditViewModel>(planEditViewModel);
        }

        public async Task<SearchResult<PlanPreviewViewModel>> GetPlanForPreviewAsync(int planId, int filialId)
        {
            var plan = await _planRepository.GetPlanAsync(planId, filialId);
            var planEditViewModel = plan?.Adapt<PlanPreviewViewModel>();
            return new SearchResult<PlanPreviewViewModel>(planEditViewModel);
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

        public async Task<CrudResult> UpdatePlanAsync(int planId, PlanEditViewModel planEditViewModel)
        {
            var plan = planEditViewModel.Adapt<Plan>();
            var rowsAffected = await _planRepository.UpdatePlanAsync(planId, plan);
            return new CrudResult(rowsAffected);
        }
    }
}
