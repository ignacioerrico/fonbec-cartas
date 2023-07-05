using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Repositories.Admin;
using Fonbec.Cartas.Logic.Models.Results;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Mapster;

namespace Fonbec.Cartas.Logic.Services.Admin
{
    public interface IFilialService
    {
        Task<List<FilialesListViewModel>> GetAllFilialesAsync();
        Task<SearchResult<string>> GetFilialNameAsync(int id);
        Task<CrudResult> CreateFilialAsync(string filialName);
        Task<CrudResult> UpdateFilialAsync(int id, string newName);
        Task<CrudResult> SoftDeleteAsync(int id);
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
            var filialesDataModel = await _filialesRepository.GetAllFilialesAsync();
            var filialesListViewModel = filialesDataModel.Adapt<List<FilialesListViewModel>>();
            return filialesListViewModel;
        }

        public async Task<SearchResult<string>> GetFilialNameAsync(int id)
        {
            var filialName = await _filialesRepository.GetFilialNameAsync(id);
            return new SearchResult<string>(filialName);
        }

        public async Task<CrudResult> CreateFilialAsync(string filialName)
        {
            var newFilial = new Filial
            {
                Name = filialName
            };

            var rowsAffected = await _filialesRepository.CreateFilialAsync(newFilial);
            
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> UpdateFilialAsync(int id, string newName)
        {
            var rowsAffected = await _filialesRepository.UpdateFilialAsync(id, newName);
            return new CrudResult(rowsAffected);
        }

        public async Task<CrudResult> SoftDeleteAsync(int id)
        {
            var rowsAffected = await _filialesRepository.SoftDeleteAsync(id);

            // TODO: Local accounts of Coordinadores, Mediadores, and Revisored in this Filial

            return new CrudResult(rowsAffected);
        }
    }
}
