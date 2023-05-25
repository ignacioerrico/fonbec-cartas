using Fonbec.Cartas.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public interface IFilialesRepository
    {
        Task<List<Filial>> GetAllFilialesAsync();
        Task<string?> GetFilialNameAsync(int id);
        Task<int> CreateFilialAsync(string filialName);
        Task<int> UpdateFilialAsync(int id, string newName);
    }

    public class FilialesRepository : IFilialesRepository
    {
        private readonly ApplicationDbContext _appDbContext;

        public FilialesRepository(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Filial>> GetAllFilialesAsync()
        {
            return await _appDbContext.Filiales.ToListAsync();
        }

        public async Task<string?> GetFilialNameAsync(int id)
        {
            var filial = await _appDbContext.Filiales.FindAsync(id);
            return filial?.Name;
        }

        public async Task<int> CreateFilialAsync(string filialName)
        {
            var newFilial = new Filial
            {
                Name = filialName
            };

            await _appDbContext.Filiales.AddAsync(newFilial);
            return await _appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateFilialAsync(int id, string newName)
        {
            var filial = await _appDbContext.Filiales.FindAsync(id);
            if (filial is null)
            {
                return 0;
            }

            filial.Name = newName;
            return await _appDbContext.SaveChangesAsync();
        }
    }
}
