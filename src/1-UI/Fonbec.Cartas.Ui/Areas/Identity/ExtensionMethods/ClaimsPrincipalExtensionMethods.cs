using System.Security.Claims;

namespace Fonbec.Cartas.Ui.Areas.Identity.ExtensionMethods
{
    public static class ClaimsPrincipalExtensionMethods
    {
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

            if (!int.TryParse(filialIdString.Value, out var filialId))
            {
                return null;
            }

            return filialId;
        }

        public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal)
        {
            var nameIdentifier = claimsPrincipal.Claims
                .FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.Ordinal));
            
            return nameIdentifier is not null
                   && string.Equals(nameIdentifier.Value, "admin", StringComparison.Ordinal);
        }
    }
}
