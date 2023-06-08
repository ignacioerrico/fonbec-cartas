using System.Globalization;
using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;

namespace Fonbec.Cartas.Logic.Services.ServicesCoordinador
{
    public interface IPlanService
    {
        Task<List<PlansListViewModel>> GetAllPlansAsync(int filialId);
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
                PlanName = $"Carta de {p.StartDate.ToString(@"MMMM \d\e yyyy", new CultureInfo("es-AR"))}",
                // TODO
                CreatedOnUtc = p.CreatedOnUtc,
                LastUpdatedOnUtc = p.LastUpdatedOnUtc,
            }).ToList();
        }
    }
}
