using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Repositories.Coordinador;
using Fonbec.Cartas.Logic.Models.Coordinador;
using Fonbec.Cartas.Logic.Models.Results;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Services.Coordinador
{
    public interface IApadrinamientoService
    {
        Task<ApadrinamientoEditViewModel> GetApadrinamientoEditDataAsync(int filialId, int becarioId);
        Task<CrudDataResult<int>> AssignPadrinoToBecarioAsync(ApadrinamientoEditAssignPadrinoToBecarioModel model);
        Task<CrudResult> UpdateApadrinamientoAsync(int apadrinamientoId, DateTime from, DateTime? to, int coordinadorId);
        Task<CrudResult> SetToDateToUknownAsync(int apadrinamientoId, int coordinadorId);
        Task<CrudResult> SetToDateToTodayAsync(int apadrinamientoId, int coordinadorId);
    }

    public class ApadrinamientoService : IApadrinamientoService
    {
        private readonly IApadrinamientoRepository _apadrinamientoRepository;

        public ApadrinamientoService(IApadrinamientoRepository apadrinamientoRepository)
        {
            _apadrinamientoRepository = apadrinamientoRepository;
        }

        public async Task<ApadrinamientoEditViewModel> GetApadrinamientoEditDataAsync(int filialId, int becarioId)
        {
            var apadrinamientosForBecario = await _apadrinamientoRepository.GetApadrinamientoEditDataAsync(filialId, becarioId);
            return apadrinamientosForBecario.Adapt<ApadrinamientoEditViewModel>();
        }

        public async Task<CrudDataResult<int>> AssignPadrinoToBecarioAsync(ApadrinamientoEditAssignPadrinoToBecarioModel model)
        {
            var apadrinamiento = model.Adapt<Apadrinamiento>();
            var dataModel = await _apadrinamientoRepository.AssignPadrinoToBecarioAsync(apadrinamiento);
            return new CrudDataResult<int>(dataModel.ApadrinamientoId, dataModel.RowsAffected);
        }

        public async Task<CrudResult> UpdateApadrinamientoAsync(int apadrinamientoId, DateTime from, DateTime? to, int coordinadorId)
        {
            var rowsAffected = await _apadrinamientoRepository.UpdateApadrinamientoAsync(apadrinamientoId, from, to, coordinadorId);
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> SetToDateToUknownAsync(int apadrinamientoId, int coordinadorId)
        {
            var rowsAffected = await _apadrinamientoRepository.SetToDateToUknownAsync(apadrinamientoId, coordinadorId);
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> SetToDateToTodayAsync(int apadrinamientoId, int coordinadorId)
        {
            var rowsAffected = await _apadrinamientoRepository.SetToDateToTodayAsync(apadrinamientoId, coordinadorId);
            return new CrudResult(rowsAffected);
        }
    }
}
