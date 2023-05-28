using Fonbec.Cartas.DataAccess.Entities.Actors;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories
{
    public abstract class UserWithAccountRepositoryBase<T>
        where T : EntityBase, IAmUserWithAccount, IHaveEmail
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        protected UserWithAccountRepositoryBase(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
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
