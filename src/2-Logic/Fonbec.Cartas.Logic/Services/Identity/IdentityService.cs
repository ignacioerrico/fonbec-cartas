using Fonbec.Cartas.DataAccess.DataModels;
using Fonbec.Cartas.DataAccess.Repositories;

namespace Fonbec.Cartas.Logic.Services.Identity
{
    public interface IIdentityService
    {
        Task<FonbecUserCustomClaimsModel> GetClaimsForCoordinadorAsync(string aspNetUserId);
        Task<FonbecUserCustomClaimsModel> GetClaimsForMediadorAsync(string aspNetUserId);
        Task<FonbecUserCustomClaimsModel> GetClaimsForRevisorAsync(string aspNetUserId);
    }

    public class IdentityService : IIdentityService
    {
        private readonly IIdentityRepository _identityRepository;

        public IdentityService(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
        }

        public async Task<FonbecUserCustomClaimsModel> GetClaimsForCoordinadorAsync(string aspNetUserId)
        {
            return await _identityRepository.GetClaimsForCoordinadorAsync(aspNetUserId);
        }

        public async Task<FonbecUserCustomClaimsModel> GetClaimsForMediadorAsync(string aspNetUserId)
        {
            return await _identityRepository.GetClaimsForMediadorAsync(aspNetUserId);
        }

        public async Task<FonbecUserCustomClaimsModel> GetClaimsForRevisorAsync(string aspNetUserId)
        {
            return await _identityRepository.GetClaimsForRevisorAsync(aspNetUserId);
        }
    }
}
