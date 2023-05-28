namespace Fonbec.Cartas.DataAccess.Entities.Actors
{
    public class Coordinador : EntityBase
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string AspNetUserId { get; set; } = Guid.Empty.ToString();

        public string Username { get; set; } = string.Empty;
    }
}
