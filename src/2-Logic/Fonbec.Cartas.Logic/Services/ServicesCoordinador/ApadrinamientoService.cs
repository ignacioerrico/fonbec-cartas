using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;

namespace Fonbec.Cartas.Logic.Services.ServicesCoordinador
{
    public interface IApadrinamientoService
    {
        Task<List<ApadrinamientoViewModel>> GetAllPadrinosForBecario(int becarioId);
        Task<int> AssignPadrinoToBecarioAsync(AssignPadrinoToBecarioViewModel viewModel);
    }

    public class ApadrinamientoService : IApadrinamientoService
    {
        private readonly IApadrinamientoRepository _apadrinamientoRepository;

        public ApadrinamientoService(IApadrinamientoRepository apadrinamientoRepository)
        {
            _apadrinamientoRepository = apadrinamientoRepository;
        }

        public async Task<List<ApadrinamientoViewModel>> GetAllPadrinosForBecario(int becarioId)
        {
            var apadrinamientosForBecario = await _apadrinamientoRepository.GetAllPadrinosForBecario(becarioId);

            return apadrinamientosForBecario.Select(a =>
                new ApadrinamientoViewModel
                {
                    PadrinoId = a.PadrinoId,
                    PadrinoFullName = a.Padrino.FullName(),
                    From = a.From,
                    To = a.To,
                }).ToList();
        }

        public async Task<int> AssignPadrinoToBecarioAsync(AssignPadrinoToBecarioViewModel viewModel)
        {
            var apadrinamiento = new Apadrinamiento
            {
                BecarioId = viewModel.BecarioId,
                PadrinoId = viewModel.PadrinoId,
                From = viewModel.From,
                To = viewModel.To,
                CreatedByCoordinadorId = viewModel.CreatedByCoordinadorId,
            };

            var rowsAffected = await _apadrinamientoRepository.AssignPadrinoToBecarioAsync(apadrinamiento);

            return rowsAffected;
        }

    }
}
