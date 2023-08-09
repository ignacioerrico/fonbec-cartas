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
        /// <summary>
        /// Returns all planned events and deadlines for a given Filial in descending order by date.
        /// </summary>
        /// <param name="filialId"></param>
        /// <returns></returns>
        Task<List<PlannedEventsListViewModel>> GetAllPlansAsync(int filialId);
        
        Task<SearchResult<PlannedEventEditViewModel>> GetPlannedEventAsync(int plannedEventId, int filialId);
        Task<SearchResult<PlannedEventPreviewViewModel>> GetPlannedEventForPreviewAsync(int plannedEventId, int filialId);
        Task<List<DateTime>> GetAllPlannedEventDatesAsync(int filialId);
        Task<List<DateTime>> GetAllDeadlinesDatesAsync(int filialId);
        Task<CrudResult> CreatePlannedEventAsync(PlannedEventEditViewModel plannedEventEditViewModel);
        Task<CrudResult> UpdatePlannedEventAsync(PlannedEventEditViewModel plannedEventEditViewModel);
        Task<CrudResult> CreateDeadlineAsync(PlannedEventsListDeadlineModel model);
        Task<CrudResult> UpdateDeadlineAsync(PlannedEventsListDeadlineModel model);
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
            var dataModels = await _plannedEventRepository.GetAllPlansAsync(filialId);
            return dataModels.BuildAdapter()
                .AddParameters("dataModels", dataModels)
                .AdaptToType<List<PlannedEventsListViewModel>>();
        }

        public async Task<SearchResult<PlannedEventEditViewModel>> GetPlannedEventAsync(int plannedEventId, int filialId)
        {
            var plannedEvent = await _plannedEventRepository.GetPlannedEventAsync(plannedEventId, filialId);
            var planEditViewModel = plannedEvent?.Adapt<PlannedEventEditViewModel>();
            return new SearchResult<PlannedEventEditViewModel>(planEditViewModel);
        }

        public async Task<SearchResult<PlannedEventPreviewViewModel>> GetPlannedEventForPreviewAsync(int plannedEventId, int filialId)
        {
            var plan = await _plannedEventRepository.GetPlannedEventAsync(plannedEventId, filialId);
            var planEditViewModel = plan?.Adapt<PlannedEventPreviewViewModel>();
            return new SearchResult<PlannedEventPreviewViewModel>(planEditViewModel);
        }

        public async Task<List<DateTime>> GetAllPlannedEventDatesAsync(int filialId)
        {
            var takenStartDates = await _plannedEventRepository.GetAllPlannedEventDatesAsync(filialId);
            return takenStartDates;
        }

        public async Task<List<DateTime>> GetAllDeadlinesDatesAsync(int filialId)
        {
            var takenDates = await _plannedEventRepository.GetAllDeadlinesDatesAsync(filialId);
            return takenDates;
        }

        public async Task<CrudResult> CreatePlannedEventAsync(PlannedEventEditViewModel plannedEventEditViewModel)
        {
            var plannedEvent = plannedEventEditViewModel.Adapt<PlannedEvent>();
            var rowsAffected = await _plannedEventRepository.CreatePlannedEventAsync(plannedEvent);
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> UpdatePlannedEventAsync(PlannedEventEditViewModel plannedEventEditViewModel)
        {
            var plannedEvent = plannedEventEditViewModel.Adapt<PlannedEvent>();
            var rowsAffected = await _plannedEventRepository.UpdatePlannedEventAsync(plannedEvent);
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> CreateDeadlineAsync(PlannedEventsListDeadlineModel model)
        {
            var deadline = model.Adapt<Deadline>();
            var rowsAffected = await _plannedEventRepository.CreateDeadlineAsync(deadline);
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> UpdateDeadlineAsync(PlannedEventsListDeadlineModel model)
        {
            var deadline = model.Adapt<Deadline>();
            var rowsAffected = await _plannedEventRepository.UpdateDeadlineAsync(deadline);
            return new CrudResult(rowsAffected);
        }
    }
}
