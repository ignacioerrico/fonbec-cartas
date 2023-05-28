using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;

namespace Fonbec.Cartas.Logic.Services.Admin
{
    public interface ICoordinadorService
    {
        Task<List<CoordinadoresListViewModel>> GetAllCoordinadoresAsync();
        Task<CoordinadorEditViewModel?> GetCoordinadorAsync(int coordinadorId);
        Task<(int, IEnumerable<string>)> CreateCoordinadorAsync(CoordinadorEditViewModel coordinadorViewModel, string initialPassword);
        Task<int> UpdateCoordinadorAsync(int id, CoordinadorEditViewModel coordinadorViewModel);
    }

    public class CoordinadorService : ICoordinadorService
    {
        private readonly ICoordinadorRepository _coordinadorRepository;
        private readonly UserManager<FonbecUser> _userManager;
        private readonly IUserStore<FonbecUser> _userStore;

        public CoordinadorService(ICoordinadorRepository coordinadorRepository, UserManager<FonbecUser> userManager, IUserStore<FonbecUser> userStore)
        {
            _coordinadorRepository = coordinadorRepository;
            _userManager = userManager;
            _userStore = userStore;
        }

        public async Task<List<CoordinadoresListViewModel>> GetAllCoordinadoresAsync()
        {
            var coordinadores = await _coordinadorRepository.GetAllCoordinadoresAsync();

            return coordinadores.Select(c => new CoordinadoresListViewModel
            {
                Id = c.Id,
                Name = c.FullName(includeNickName: true),
                Gender = c.Gender,
                Filial = c.Filial.Name,
                Email = c.Email,
                Phone = c.Phone ?? string.Empty,
                Username = c.Username,
                CreatedOnUtc = c.CreatedOnUtc,
                LastUpdatedOnUtc = c.LastUpdatedOnUtc
            }).ToList();
        }

        public async Task<CoordinadorEditViewModel?> GetCoordinadorAsync(int coordinadorId)
        {
            var coordinador = await _coordinadorRepository.GetCoordinadorAsync(coordinadorId);

            if (coordinador is null)
            {
                return null;
            }

            var coordinadorEditViewModel = new CoordinadorEditViewModel
            {
                FilialId = coordinador.FilialId,
                FirstName = coordinador.FirstName,
                LastName = coordinador.LastName,
                NickName = coordinador.NickName ?? string.Empty,
                Gender = coordinador.Gender,
                Email = coordinador.Email,
                Phone = coordinador.Phone ?? string.Empty,
                Username = coordinador.Username,
                AspNetUserId = coordinador.AspNetUserId,
            };

            return coordinadorEditViewModel;
        }

        public async Task<(int, IEnumerable<string>)> CreateCoordinadorAsync(CoordinadorEditViewModel coordinadorViewModel, string initialPassword)
        {
            var user = new FonbecUser();

            await _userStore.SetUserNameAsync(user, coordinadorViewModel.Username, CancellationToken.None);
            await ((IUserEmailStore<FonbecUser>)_userStore).SetEmailAsync(user, coordinadorViewModel.Email, CancellationToken.None);
            
            var result = await _userManager.CreateAsync(user, initialPassword);

            if (!result.Succeeded)
            {
                return (0, result.Errors.Select(e => e.Description));
            }

            result = await _userManager.AddToRoleAsync(user, FonbecRoles.Coordinador);

            if (!result.Succeeded)
            {
                return (0, result.Errors.Select(e => e.Description));
            }

            var userId = await _userManager.GetUserIdAsync(user);

            var coordinador = new Coordinador
            {
                FilialId = coordinadorViewModel.FilialId,
                FirstName = coordinadorViewModel.FirstName,
                LastName = coordinadorViewModel.LastName,
                NickName = string.IsNullOrWhiteSpace(coordinadorViewModel.NickName) ? null : coordinadorViewModel.NickName,
                Gender = coordinadorViewModel.Gender,
                Email = coordinadorViewModel.Email,
                Phone = coordinadorViewModel.Phone,
                Username = coordinadorViewModel.Username,
                AspNetUserId = userId,
            };
            
            var rowsAffected = await _coordinadorRepository.CreateCoordinadorAsync(coordinador);

            return (rowsAffected, Enumerable.Empty<string>());
        }

        public async Task<int> UpdateCoordinadorAsync(int id, CoordinadorEditViewModel coordinadorViewModel)
        {
            var coordinador = new Coordinador
            {
                FilialId = coordinadorViewModel.FilialId,
                FirstName = coordinadorViewModel.FirstName,
                LastName = coordinadorViewModel.LastName,
                NickName = coordinadorViewModel.NickName,
                Gender = coordinadorViewModel.Gender,
                Email = coordinadorViewModel.Email,
                Phone = coordinadorViewModel.Phone,
                Username = coordinadorViewModel.Username,
            };
            var affectedRows = await _coordinadorRepository.UpdateCoordinadorAsync(id, coordinador);

            if (affectedRows == 0)
            {
                return 0;
            }

            var user = await _userManager.FindByIdAsync(coordinadorViewModel.AspNetUserId);

            if (user is null)
            {
                return -1;
            }

            var usernameHasChanged = !string.Equals(user.UserName, coordinadorViewModel.Username, StringComparison.OrdinalIgnoreCase);
            if (usernameHasChanged)
            {
                await _userStore.SetUserNameAsync(user, coordinadorViewModel.Username, CancellationToken.None);
            }

            var emailHasChanged = !string.Equals(user.Email, coordinadorViewModel.Email, StringComparison.OrdinalIgnoreCase);
            if (emailHasChanged)
            {
                await ((IUserEmailStore<FonbecUser>)_userStore).SetEmailAsync(user, coordinadorViewModel.Email, CancellationToken.None);
            }

            if (!usernameHasChanged && !emailHasChanged)
            {
                return affectedRows;
            }
            
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return -1;
            }

            return affectedRows;
        }
    }
}
