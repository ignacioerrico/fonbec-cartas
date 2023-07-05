using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Admin
{
    public interface IUserWithAccountRepositoryBase<T> where T : UserWithAccount
    {
        Task<List<Filial>> GetAllFilialesAsync();
        Task<List<T>> GetAllAsync();
        Task<T?> GetAsync(int id);
        Task<int> CreateAsync(T userWithAccount);
        Task<int> UpdateAsync(int id, T userWithAccount);
    }

    public abstract class UserWithAccountRepositoryBase<T> : IUserWithAccountRepositoryBase<T> where T : UserWithAccount
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        protected UserWithAccountRepositoryBase(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<List<Filial>> GetAllFilialesAsync()
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var filiales = await appDbContext.Filiales
                .OrderBy(f => f.Name)
                .ToListAsync();
            return filiales;
        }

        public async Task<List<T>> GetAllAsync()
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var allWithFilial = await appDbContext.Set<T>()
                .Include(e => e.Filial)
                .ToListAsync();
            return allWithFilial;
        }

        public async Task<T?> GetAsync(int id)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var singleWithFilial = await appDbContext.Set<T>()
                .Include(e => e.Filial)
                .SingleOrDefaultAsync(e => e.Id == id);
            return singleWithFilial;
        }

        public async Task<int> CreateAsync(T userWithAccount)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Set<T>().AddAsync(userWithAccount);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(int id, T userWithAccount)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var single = await appDbContext.Set<T>().FindAsync(id);
            if (single is null)
            {
                return 0;
            }

            single.FirstName = userWithAccount.FirstName;
            single.LastName = userWithAccount.LastName;
            single.NickName = userWithAccount.NickName;
            single.Gender = userWithAccount.Gender;
            single.Email = userWithAccount.Email;
            single.Phone = userWithAccount.Phone;
            single.Username = userWithAccount.Username;

            appDbContext.Set<T>().Update(single);
            return await appDbContext.SaveChangesAsync();
        }
    }
}
