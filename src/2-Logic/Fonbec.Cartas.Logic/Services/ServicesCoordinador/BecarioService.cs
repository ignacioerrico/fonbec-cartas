using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;

namespace Fonbec.Cartas.Logic.Services.ServicesCoordinador
{
    public interface IBecarioService
    {
        Task<List<MediadorViewModel>> GetAllMediadoresForSelectionAsync(int filialId);
        Task<List<PadrinoViewModel>> GetAllPadrinosForSelectionAsync(int filialId);
        Task<BecarioNameViewModel?> GetBecarioNameAsync(int filialId, int becarioId);
        Task<List<BecariosListViewModel>> GetAllBecariosAsync(int filialId);
        Task<BecarioEditViewModel?> GetBecarioAsync(int becarioId, int filialId);
        Task<int> CreateAsync(BecarioEditViewModel becarioEditViewModel);
        Task<int> UpdateAsync(int becarioId, BecarioEditViewModel becarioEditViewModel);
    }

    public class BecarioService : IBecarioService
    {
        private readonly IBecarioRepository _becarioRepository;

        public BecarioService(IBecarioRepository becarioRepository)
        {
            _becarioRepository = becarioRepository;
        }

        public async Task<List<MediadorViewModel>> GetAllMediadoresForSelectionAsync(int filialId)
        {
            var mediadores = await _becarioRepository.GetAllMediadoresForSelectionAsync(filialId);
            return mediadores.Select(m => new MediadorViewModel(m.Id, m.FullName)).ToList();
        }

        public async Task<List<PadrinoViewModel>> GetAllPadrinosForSelectionAsync(int filialId)
        {
            var padrinos = await _becarioRepository.GetAllPadrinosForSelectionAsync(filialId);
            return padrinos.Select(p => new PadrinoViewModel(p.Id, p.FullName)).ToList();
        }

        public async Task<BecarioNameViewModel?> GetBecarioNameAsync(int filialId, int becarioId)
        {
            var becarioName = await _becarioRepository.GetBecarioNameAsync(filialId, becarioId);
            if (becarioName is null)
            {
                return null;
            }

            return new BecarioNameViewModel(becarioName.FullName, becarioName.FirstName);
        }

        public async Task<List<BecariosListViewModel>> GetAllBecariosAsync(int filialId)
        {
            var all = await _becarioRepository.GetAllBecariosAsync(filialId);

            return all.Select(b =>
                new BecariosListViewModel
                {
                    Id = b.Id,
                    Mediador = b.Mediador.FullName(),
                    NivelDeEstudio = b.NivelDeEstudio.ToString(),
                    Name = b.FullName(includeNickName: true),
                    Gender = b.Gender,
                    Email = b.Email ?? string.Empty,
                    Phone = b.Phone ?? string.Empty,
                    CreatedOnUtc = b.CreatedOnUtc,
                    LastUpdatedOnUtc = b.LastUpdatedOnUtc,
                    CreatedBy = b.CreatedByCoordinador.FullName(),
                    UpdatedBy = b.UpdatedByCoordinador?.FullName(),
                }).ToList();
        }

        public async Task<BecarioEditViewModel?> GetBecarioAsync(int becarioId, int filialId)
        {
            var becario = await _becarioRepository.GetBecarioAsync(becarioId, filialId);

            if (becario is null)
            {
                return null;
            }

            var becarioEditViewModel = new BecarioEditViewModel
            {
                FilialId = becario.FilialId,
                MediadorId = becario.MediadorId,
                NivelDeEstudio = becario.NivelDeEstudio,
                FirstName = becario.FirstName,
                LastName = becario.LastName,
                NickName = becario.NickName ?? string.Empty,
                Gender = becario.Gender,
                Email = becario.Email ?? string.Empty,
                Phone = becario.Phone ?? string.Empty,
                CreatedByCoordinadorId = becario.CreatedByCoordinadorId,
                UpdatedByCoordinadorId = becario.UpdatedByCoordinadorId,
            };

            return becarioEditViewModel;
        }

        public async Task<int> CreateAsync(BecarioEditViewModel becarioEditViewModel)
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

            return rowsAffected;
        }

        public async Task<int> UpdateAsync(int becarioId, BecarioEditViewModel becarioEditViewModel)
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

            return rowsAffected;
        }
    }
}
