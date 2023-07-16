using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport
{
    public interface ICreateUserWithAccountRepository<T> where T : UserWithAccount
    {
        Task<IEnumerable<UserWithAccount>> GetAllAsync();
        Task<List<T>> CreateAsync(List<T> usersWithAccount);
    }

    public class CreateUserWithAccountRepository<T> : ICreateUserWithAccountRepository<T> where T : UserWithAccount
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public CreateUserWithAccountRepository(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        public async Task<IEnumerable<UserWithAccount>> GetAllAsync()
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            var usersWithAccount = await appDbContext.Set<T>().ToListAsync();
            return usersWithAccount.AsEnumerable();
        }

        public async Task<List<T>> CreateAsync(List<T> usersWithAccount)
        {
            await using var appDbContext = await _appDbContextFactory.CreateDbContextAsync();
            await appDbContext.Set<T>().AddRangeAsync(usersWithAccount);
            await appDbContext.SaveChangesAsync();
            return usersWithAccount;
        }
    }
}
