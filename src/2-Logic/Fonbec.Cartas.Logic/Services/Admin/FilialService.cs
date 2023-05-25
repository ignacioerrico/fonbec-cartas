using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.Logic.ViewModels.Admin;

namespace Fonbec.Cartas.Logic.Services.Admin
{
    public interface IFilialService
    {
        Task<List<FilialesListViewModel>> GetAllFilialesAsync();
        Task<string?> GetFilialNameAsync(int id);
        Task<int> CreateFilialAsync(string filialName);
        Task<int> UpdateFilialAsync(int id, string newName);
    }

    public class FilialService : IFilialService
    {
        private readonly IFilialesRepository _filialesRepository;

        public FilialService(IFilialesRepository filialesRepository)
        {
            _filialesRepository = filialesRepository;
        }

        public async Task<List<FilialesListViewModel>> GetAllFilialesAsync()
        {
            var filiales = await _filialesRepository.GetAllFilialesAsync();
            return filiales.Select(f =>
                    new FilialesListViewModel(f.Name)
                    {
                        Id = f.Id,
                        // TODO
                    })
                .ToList();
        }

        public async Task<string?> GetFilialNameAsync(int id)
        {
            return await _filialesRepository.GetFilialNameAsync(id);
        }

        public async Task<int> CreateFilialAsync(string filialName)
        {
            return await _filialesRepository.CreateFilialAsync(filialName);
        }

        public async Task<int> UpdateFilialAsync(int id, string newName)
        {
            return await _filialesRepository.UpdateFilialAsync(id, newName);
        }
    }
}
