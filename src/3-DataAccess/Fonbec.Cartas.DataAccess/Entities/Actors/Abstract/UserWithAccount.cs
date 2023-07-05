namespace Fonbec.Cartas.DataAccess.Entities.Actors.Abstract
{
    public abstract class UserWithAccount : EntityBase
    {
        public string AspNetUserId { get; set; } = Guid.Empty.ToString();

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
