using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;

namespace Fonbec.Cartas.Logic.Services.ServicesCoordinador
{
    public interface IBecarioService
    {
        Task<List<MediadorViewModel>> GetAllMediadoresForSelectionAsync(int filialId);
        Task<BecarioEditViewModel?> GetBecarioAsync(int becarioId, int filialId);
        Task<int> CreateAsync(BecarioEditViewModel becario);
        Task<int> UpdateAsync(int id, BecarioEditViewModel becario);
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

        public async Task<BecarioEditViewModel?> GetBecarioAsync(int becarioId, int filialId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateAsync(BecarioEditViewModel becario)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateAsync(int id, BecarioEditViewModel becario)
        {
            throw new NotImplementedException();
        }
    }
}
