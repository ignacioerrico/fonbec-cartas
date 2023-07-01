using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;

namespace Fonbec.Cartas.Logic.Services.Coordinador
{
    public interface IApadrinamientoService
    {
        Task<List<ApadrinamientoEditViewModel>> GetAllPadrinosForBecario(int becarioId);
        Task<int> AssignPadrinoToBecarioAsync(AssignPadrinoToBecarioViewModel viewModel);
        Task<int> UpdateApadrinamientoAsync(int apadrinamientoId, DateTime from, DateTime? to, int coordinadorId);
        Task<int> SetToDateToUknownAsync(int apadrinamientoId, int coordinadorId);
        Task<int> SetToDateToTodayAsync(int apadrinamientoId, int coordinadorId);
    }

    public class ApadrinamientoService : IApadrinamientoService
    {
        private readonly IApadrinamientoRepository _apadrinamientoRepository;

        public ApadrinamientoService(IApadrinamientoRepository apadrinamientoRepository)
        {
            _apadrinamientoRepository = apadrinamientoRepository;
        }

        public async Task<List<ApadrinamientoEditViewModel>> GetAllPadrinosForBecario(int becarioId)
        {
            var apadrinamientosForBecario = await _apadrinamientoRepository.GetAllPadrinosForBecario(becarioId);

            return apadrinamientosForBecario.Select(a =>
                new ApadrinamientoEditViewModel(a.From, a.To)
                {
                    ApadrinamientoId = a.Id,
                    PadrinoId = a.PadrinoId,
                    PadrinoFullName = a.Padrino.FullName(),
                    CreatedOnUtc = a.CreatedOnUtc,
                    LastUpdatedOnUtc = a.LastUpdatedOnUtc,
                    CreatedBy = a.CreatedByCoordinador.FullName(),
                    UpdatedBy = a.UpdatedByCoordinador?.FullName(),
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

            var apadrinamientoId = await _apadrinamientoRepository.AssignPadrinoToBecarioAsync(apadrinamiento);

            return apadrinamientoId;
        }

        public async Task<int> UpdateApadrinamientoAsync(int apadrinamientoId, DateTime from, DateTime? to, int coordinadorId)
        {
            return await _apadrinamientoRepository.UpdateApadrinamientoAsync(apadrinamientoId, from, to, coordinadorId);
        }

        public async Task<int> SetToDateToUknownAsync(int apadrinamientoId, int coordinadorId)
        {
            return await _apadrinamientoRepository.SetToDateToUknownAsync(apadrinamientoId, coordinadorId);
        }

        public async Task<int> SetToDateToTodayAsync(int apadrinamientoId, int coordinadorId)
        {
            return await _apadrinamientoRepository.SetToDateToTodayAsync(apadrinamientoId, coordinadorId);
        }
    }
}
