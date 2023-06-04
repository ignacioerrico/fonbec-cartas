using System.Security.Claims;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.Constants;

namespace Fonbec.Cartas.Logic.ExtensionMethods
{
    public static class ClaimsPrincipalExtensionMethods
    {
        public static int? UserWithAccountId(this ClaimsPrincipal claimsPrincipal)
        {
            var userWithAccountIdString = GetClaimByType(claimsPrincipal, FonbecUserCustomClaims.UserWithAccountId);

            if (userWithAccountIdString is null)
            {
                return null;
            }

            return int.TryParse(userWithAccountIdString.Value, out var userWithAccountId)
                ? userWithAccountId
                : null;
        }

        public static Gender Gender(this ClaimsPrincipal claimsPrincipal)
        {
            var genderString = GetClaimByType(claimsPrincipal, FonbecUserCustomClaims.Gender);

            if (genderString is null)
            {
                return DataAccess.Entities.Enums.Gender.Unknown;
            }

            return Enum.TryParse(typeof(Gender), genderString.Value, out var gender)
                ? (Gender)gender
                : DataAccess.Entities.Enums.Gender.Unknown;
        }

        public static string NickName(this ClaimsPrincipal claimsPrincipal)
        {
            var nickName = GetClaimByType(claimsPrincipal, FonbecUserCustomClaims.NickName);
            
            return nickName is null
                ? "desconocido"
                : nickName.Value;
        }
        
        public static int? FilialId(this ClaimsPrincipal claimsPrincipal)
        {
            var filialIdString = GetClaimByType(claimsPrincipal, FonbecUserCustomClaims.FilialId);
            
            if (filialIdString is null)
            {
                return null;
            }

            return int.TryParse(filialIdString.Value, out var filialId)
                ? filialId
                : null;
        }

        public static string? FilialName(this ClaimsPrincipal claimsPrincipal)
        {
            var filialName = GetClaimByType(claimsPrincipal, FonbecUserCustomClaims.FilialName);

            return filialName?.Value;
        }

        private static Claim? GetClaimByType(ClaimsPrincipal claimsPrincipal, string type)
        {
            return claimsPrincipal.FindFirst(c => string.Equals(c.Type, type, StringComparison.OrdinalIgnoreCase));
        }
    }
}
