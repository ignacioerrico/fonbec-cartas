namespace Fonbec.Cartas.DataAccess.Entities.Actors
{
    public class Mediador : EntityBase, IAmUserWithAccount, IHaveEmail
    {
        public string Email { get; set; } = string.Empty;

        public List<Becario> Becarios { get; set; } = new();

        public string AspNetUserId { get; set; } = Guid.Empty.ToString();

        public string Username { get; set; } = string.Empty;
    }
}
