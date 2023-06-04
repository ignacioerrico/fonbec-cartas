using System.Security.Claims;
using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.DataAccess.Projections;
using Fonbec.Cartas.Logic.Constants;
using Fonbec.Cartas.Logic.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Fonbec.Cartas.Ui.Identity
{
    public class FonbecUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<FonbecUser, IdentityRole>
    {
        private readonly IIdentityService _identityService;
        private readonly UserManager<FonbecUser> _userManager;

        public FonbecUserClaimsPrincipalFactory(IIdentityService identityService,
            UserManager<FonbecUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager,
                roleManager,
                options)
        {
            _identityService = identityService;
            _userManager = userManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(FonbecUser user)
        {
            var claimsIdentity = await base.GenerateClaimsAsync(user);

            Claim claim;

            var isAdmin = await _userManager.IsInRoleAsync(user, FonbecRoles.Admin);

            if (isAdmin)
            {
                claim = new Claim(FonbecUserCustomClaims.NickName, "Admin");
                claimsIdentity.AddClaim(claim);

                return claimsIdentity;
            }

            FonbecUserCustomClaimsModel customClaimsModel;

            var isCoordinador = await _userManager.IsInRoleAsync(user, FonbecRoles.Coordinador);
            var isMediador = await _userManager.IsInRoleAsync(user, FonbecRoles.Mediador);
            var isRevisor = await _userManager.IsInRoleAsync(user, FonbecRoles.Revisor);

            if (isCoordinador)
            {
                customClaimsModel = await _identityService.GetClaimsForCoordinadorAsync(user.Id);
            }
            else if (isMediador)
            {
                customClaimsModel = await _identityService.GetClaimsForMediadorAsync(user.Id);
            }
            else if (isRevisor)
            {
                customClaimsModel = await _identityService.GetClaimsForRevisorAsync(user.Id);
            }
            else
            {
                return claimsIdentity;
            }

            claim = new Claim(FonbecUserCustomClaims.UserWithAccountId, customClaimsModel.UserWithAccountId);
            claimsIdentity.AddClaim(claim);

            claim = new Claim(FonbecUserCustomClaims.Gender, customClaimsModel.Gender);
            claimsIdentity.AddClaim(claim);

            claim = new Claim(FonbecUserCustomClaims.NickName, customClaimsModel.NickName);
            claimsIdentity.AddClaim(claim);

            claim = new Claim(FonbecUserCustomClaims.FilialId, customClaimsModel.FilialId);
            claimsIdentity.AddClaim(claim);

            claim = new Claim(FonbecUserCustomClaims.FilialName, customClaimsModel.FilialName);
            claimsIdentity.AddClaim(claim);

            return claimsIdentity;
        }
    }
}
