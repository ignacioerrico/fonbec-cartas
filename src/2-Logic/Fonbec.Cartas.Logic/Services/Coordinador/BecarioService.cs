using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Repositories.Coordinador;
using Fonbec.Cartas.Logic.Models;
using Fonbec.Cartas.Logic.Models.Results;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Services.Coordinador
{
    public interface IBecarioService
    {
        Task<List<SelectableModel>> GetAllMediadoresForSelectionAsync(int filialId);
        Task<List<BecariosListViewModel>> GetAllBecariosAsync(int filialId);
        Task<SearchResult<BecarioEditViewModel>> GetBecarioAsync(int becarioId, int filialId);
        Task<CrudResult> CreateAsync(BecarioEditViewModel becarioEditViewModel);
        Task<CrudResult> UpdateAsync(int becarioId, BecarioEditViewModel becarioEditViewModel);
    }

    public class BecarioService : IBecarioService
    {
        private readonly IBecarioRepository _becarioRepository;

        public BecarioService(IBecarioRepository becarioRepository)
        {
            _becarioRepository = becarioRepository;
        }

        public async Task<List<SelectableModel>> GetAllMediadoresForSelectionAsync(int filialId)
        {
            var mediadores = await _becarioRepository.GetAllMediadoresForSelectionAsync(filialId);
            return mediadores.Adapt<List<SelectableModel>>();
        }

        public async Task<List<BecariosListViewModel>> GetAllBecariosAsync(int filialId)
        {
            var becarios = await _becarioRepository.GetAllBecariosAsync(filialId);
            return becarios.Adapt<List<BecariosListViewModel>>();
        }

        public async Task<SearchResult<BecarioEditViewModel>> GetBecarioAsync(int becarioId, int filialId)
        {
            var becario = await _becarioRepository.GetBecarioAsync(becarioId, filialId);
            var editViewModel = becario?.Adapt<BecarioEditViewModel>();
            return new SearchResult<BecarioEditViewModel>(editViewModel);
        }

        public async Task<CrudResult> CreateAsync(BecarioEditViewModel becarioEditViewModel)
        {
            var becario = new Becario
            {
                FilialId = becarioEditViewModel.FilialId,
                MediadorId = becarioEditViewModel.MediadorId,
                NivelDeEstudio = becarioEditViewModel.NivelDeEstudio,
                FirstName = becarioEditViewModel.FirstName,
                LastName = becarioEditViewModel.LastName,
                NickName = string.IsNullOrWhiteSpace(becarioEditViewModel.NickName) ? null : becarioEditViewModel.NickName,
                Gender = becarioEditViewModel.Gender,
                Email = string.IsNullOrWhiteSpace(becarioEditViewModel.Email) ? null : becarioEditViewModel.Email,
                Phone = string.IsNullOrWhiteSpace(becarioEditViewModel.Phone) ? null : becarioEditViewModel.Phone,
                CreatedByCoordinadorId = becarioEditViewModel.CreatedByCoordinadorId,
            };

            var rowsAffected = await _becarioRepository.CreateAsync(becario);

            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> UpdateAsync(int becarioId, BecarioEditViewModel becarioEditViewModel)
        {
            var becario = new Becario
            {
                FilialId = becarioEditViewModel.FilialId,
                MediadorId = becarioEditViewModel.MediadorId,
                NivelDeEstudio = becarioEditViewModel.NivelDeEstudio,
                FirstName = becarioEditViewModel.FirstName,
                LastName = becarioEditViewModel.LastName,
                NickName = string.IsNullOrWhiteSpace(becarioEditViewModel.NickName) ? null : becarioEditViewModel.NickName,
                Gender = becarioEditViewModel.Gender,
                Email = string.IsNullOrWhiteSpace(becarioEditViewModel.Email) ? null : becarioEditViewModel.Email,
                Phone = string.IsNullOrWhiteSpace(becarioEditViewModel.Phone) ? null : becarioEditViewModel.Phone,
                UpdatedByCoordinadorId = becarioEditViewModel.UpdatedByCoordinadorId,
            };

            var rowsAffected = await _becarioRepository.UpdateAsync(becarioId, becario);

            return new CrudResult(rowsAffected);
        }
    }
}
