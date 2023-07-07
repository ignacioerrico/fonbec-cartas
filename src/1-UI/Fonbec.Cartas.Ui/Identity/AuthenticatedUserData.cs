using System.Security.Claims;

namespace Fonbec.Cartas.Ui.Identity
{
    public class AuthenticatedUserData
    {
        public bool DataObtainedSuccessfully { get; set; }

        public ClaimsPrincipal User { get; set; } = default!;

        public int FilialId { get; set; }

        public AuthenticatedUserData SetData(ClaimsPrincipal user, int filialId)
        {
            DataObtainedSuccessfully = true;
            User = user;
            FilialId = filialId;
            return this;
        }
    }
}
