using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Fonbec.Cartas.DataAccess.Repositories.Coordinador;
using Fonbec.Cartas.Logic.Models.Coordinador;
using Fonbec.Cartas.Logic.Models.Results;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Services.Coordinador
{
    public interface IPlannedEventService
    {
        Task<List<PlannedEventsListViewModel>> GetAllPlansAsync(int filialId);
        Task<SearchResult<PlannedCartaEditViewModel>> GetPlannedCartaAsync(int planId, int filialId);
        Task<SearchResult<PlannedCartaPreviewViewModel>> GetPlannedCartaForPreviewAsync(int planId, int filialId);
        Task<List<DateTime>> GetAllPlannedEventDates(int filialId, PlannedEventType? plannedEventType = null);
        Task<CrudResult> CreatePlannedCartaAsync(PlannedCartaEditViewModel plannedCartaEditViewModel);
        Task<CrudResult> UpdatePlannedCartaAsync(int planId, PlannedCartaEditViewModel plannedCartaEditViewModel);
        Task<CrudResult> CreatePlannedCorteNotasAsync(PlannedEventsListPlannedCorteNotasModel model);
        Task<CrudResult> UpdatePlannedCorteNotasAsync(PlannedEventsListPlannedCorteNotasModel model);
    }

    public class PlannedEventService : IPlannedEventService
    {
        private readonly IPlannedEventRepository _plannedEventRepository;

        public PlannedEventService(IPlannedEventRepository plannedEventRepository)
        {
            _plannedEventRepository = plannedEventRepository;
        }

        public async Task<List<PlannedEventsListViewModel>> GetAllPlansAsync(int filialId)
        {
            var all = await _plannedEventRepository.GetAllPlannedEventsAsync(filialId);
            return all.Adapt<List<PlannedEventsListViewModel>>();
        }

        public async Task<SearchResult<PlannedCartaEditViewModel>> GetPlannedCartaAsync(int planId, int filialId)
        {
            var plannedCarta = await _plannedEventRepository.GetPlannedCartaForPreviewAsync(planId, filialId);
            var planEditViewModel = plannedCarta?.Adapt<PlannedCartaEditViewModel>();
            return new SearchResult<PlannedCartaEditViewModel>(planEditViewModel);
        }

        public async Task<SearchResult<PlannedCartaPreviewViewModel>> GetPlannedCartaForPreviewAsync(int planId, int filialId)
        {
            var plan = await _plannedEventRepository.GetPlannedCartaForPreviewAsync(planId, filialId);
            var planEditViewModel = plan?.Adapt<PlannedCartaPreviewViewModel>();
            return new SearchResult<PlannedCartaPreviewViewModel>(planEditViewModel);
        }

        public async Task<List<DateTime>> GetAllPlannedEventDates(int filialId, PlannedEventType? plannedEventType = null)
        {
            var takenStartDates = await _plannedEventRepository.GetAllPlannedEventDates(filialId, plannedEventType);
            return takenStartDates;
        }

        public async Task<CrudResult> CreatePlannedCartaAsync(PlannedCartaEditViewModel plannedCartaEditViewModel)
        {
            var plannedCarta = plannedCartaEditViewModel.Adapt<PlannedEvent>();
            var rowsAffected = await _plannedEventRepository.CreatePlannedCartaAsync(plannedCarta);
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> UpdatePlannedCartaAsync(int planId, PlannedCartaEditViewModel plannedCartaEditViewModel)
        {
            var plannedCarta = plannedCartaEditViewModel.Adapt<PlannedEvent>();
            var rowsAffected = await _plannedEventRepository.UpdatePlannedCartaAsync(planId, plannedCarta);
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> CreatePlannedCorteNotasAsync(PlannedEventsListPlannedCorteNotasModel model)
        {
            var plannedCorteNotas = model.Adapt<PlannedEvent>();
            var rowsAffected = await _plannedEventRepository.CreatePlannedCorteNotasAsync(plannedCorteNotas);
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> UpdatePlannedCorteNotasAsync(PlannedEventsListPlannedCorteNotasModel model)
        {
            var plannedCorteNotas = model.Adapt<PlannedEvent>();
            var rowsAffected = await _plannedEventRepository.UpdatePlannedCorteNotasAsync(plannedCorteNotas);
            return new CrudResult(rowsAffected);
        }
    }
}
