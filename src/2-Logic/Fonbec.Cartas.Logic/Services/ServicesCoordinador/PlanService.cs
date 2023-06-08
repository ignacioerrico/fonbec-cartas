using System.Globalization;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;

namespace Fonbec.Cartas.Logic.Services.ServicesCoordinador
{
    public interface IPlanService
    {
        Task<List<PlansListViewModel>> GetAllPlansAsync(int filialId);
        Task<List<DateTime>> GetAllPlansStartDates(int filialId);
        Task<int> CreatePlanAsync(PlanEditViewModel planEditViewModel);
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
            return all.Select(p => new PlansListViewModel
            {
                Id = p.Id,
                StartDate = p.StartDate,
                PlanName = $"Carta de {p.StartDate.ToString(@"MMMM \d\e yyyy", new CultureInfo("es-AR"))}",
                // TODO
                CreatedOnUtc = p.CreatedOnUtc,
                LastUpdatedOnUtc = p.LastUpdatedOnUtc,
                CreatedBy = p.CreatedByCoordinador.FullName(),
                UpdatedBy = p.UpdatedByCoordinador?.FullName(),
            }).ToList();
        }

        public async Task<List<DateTime>> GetAllPlansStartDates(int filialId)
        {
            var takenStartDates = await _planRepository.GetAllPlansStartDates(filialId);
            return takenStartDates;
        }

        public async Task<int> CreatePlanAsync(PlanEditViewModel planEditViewModel)
        {
            var plan = new Plan
            {
                FilialId = planEditViewModel.FilialId,
                StartDate = planEditViewModel.StartDate!.Value,
                Subject = planEditViewModel.Subject,
                MessageMarkdown = planEditViewModel.MessageMarkdown,
                CreatedByCoordinadorId = planEditViewModel.CreatedByCoordinadorId,
            };

            var rowsAffected = await _planRepository.CreatePlanAsync(plan);

            return rowsAffected;
        }
    }
}
