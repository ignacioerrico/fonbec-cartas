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
        Task<CrudResult> CreateApadrinamientoAsync(ApadrinamientoEditNewApadrinamientoModel model);
        Task<CrudResult> UpdateApadrinamientoAsync(ApadrinamientoEditUpdateApadrinamientoModel model);
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

        public async Task<CrudResult> CreateApadrinamientoAsync(ApadrinamientoEditNewApadrinamientoModel model)
        {
            var apadrinamiento = model.Adapt<Apadrinamiento>();
            var rowsAffected = await _apadrinamientoRepository.CreateApadrinamientoAsync(apadrinamiento);
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> UpdateApadrinamientoAsync(ApadrinamientoEditUpdateApadrinamientoModel model)
        {
            var apadrinamiento = model.Adapt<Apadrinamiento>();
            var rowsAffected = await _apadrinamientoRepository.UpdateApadrinamientoAsync(apadrinamiento);
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
