using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Repositories.Coordinador;
using Fonbec.Cartas.Logic.Models.Results;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Services.Coordinador
{
    public interface IPadrinoService
    {
        Task<List<PadrinosListViewModel>> GetAllPadrinosAsync(int filialId);
        Task<SearchResult<PadrinoEditViewModel>> GetPadrinoAsync(int padrinoId, int filialId);
        Task<CrudResult> CreateAsync(PadrinoEditViewModel padrinoEditViewModel);
        Task<CrudResult> UpdateAsync(int padrinoId, PadrinoEditViewModel padrinoEditViewModel);
    }

    public class PadrinoService : IPadrinoService
    {
        private readonly IPadrinoRepository _padrinoRepository;

        public PadrinoService(IPadrinoRepository padrinoRepository)
        {
            _padrinoRepository = padrinoRepository;
        }

        public async Task<List<PadrinosListViewModel>> GetAllPadrinosAsync(int filialId)
        {
            var padrinos = await _padrinoRepository.GetAllPadrinosAsync(filialId);
            return padrinos.Adapt<List<PadrinosListViewModel>>();
        }

        public async Task<SearchResult<PadrinoEditViewModel>> GetPadrinoAsync(int padrinoId, int filialId)
        {
            var padrino = await _padrinoRepository.GetPadrinoAsync(padrinoId, filialId);
            var editViewModel = padrino?.Adapt<PadrinoEditViewModel>();
            return new SearchResult<PadrinoEditViewModel>(editViewModel);
        }

        public async Task<CrudResult> CreateAsync(PadrinoEditViewModel padrinoEditViewModel)
        {
            var padrino = padrinoEditViewModel.Adapt<Padrino>();
            var rowsAffected = await _padrinoRepository.CreateAsync(padrino);
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> UpdateAsync(int padrinoId, PadrinoEditViewModel padrinoEditViewModel)
        {
            var padrino = padrinoEditViewModel.Adapt<Padrino>();
            var rowsAffected = await _padrinoRepository.UpdateAsync(padrinoId, padrino);
            return new CrudResult(rowsAffected);
        }
    }
}
