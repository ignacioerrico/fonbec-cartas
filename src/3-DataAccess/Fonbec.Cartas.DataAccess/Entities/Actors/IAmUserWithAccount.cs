namespace Fonbec.Cartas.DataAccess.Entities.Actors
{
    public interface IAmUserWithAccount
    {
        string AspNetUserId { get; set; }

        string Username { get; set; }
    }
}
