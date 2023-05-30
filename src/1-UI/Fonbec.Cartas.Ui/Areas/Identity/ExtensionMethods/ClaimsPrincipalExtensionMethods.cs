using System.Security.Claims;

namespace Fonbec.Cartas.Ui.Areas.Identity.ExtensionMethods
{
    public static class ClaimsPrincipalExtensionMethods
    {
        public static int? UserWithAccountId(this ClaimsPrincipal claimsPrincipal)
        {
            var userWithAccountIdString = claimsPrincipal.Claims
                .FirstOrDefault(c => string.Equals(c.Type, FonbecUserCustomClaims.UserWithAccountId, StringComparison.Ordinal));

            if (userWithAccountIdString is null)
            {
                return null;
            }

            return int.TryParse(userWithAccountIdString.Value, out var userWithAccountId)
                ? userWithAccountId
                : null;
        }

        public static string NickName(this ClaimsPrincipal claimsPrincipal)
        {
            var nickName = claimsPrincipal.Claims
                .FirstOrDefault(c => string.Equals(c.Type, FonbecUserCustomClaims.NickName, StringComparison.Ordinal));
            
            return nickName is null
                ? "desconocido"
                : nickName.Value;
        }
        
        public static int? FilialId(this ClaimsPrincipal claimsPrincipal)
        {
            var filialIdString = claimsPrincipal.Claims
                .FirstOrDefault(c => string.Equals(c.Type, FonbecUserCustomClaims.FilialId, StringComparison.Ordinal));
            
            if (filialIdString is null)
            {
                return null;
            }

            return int.TryParse(filialIdString.Value, out var filialId)
                ? filialId
                : null;
        }
    }
}
