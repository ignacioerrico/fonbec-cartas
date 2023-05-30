namespace Fonbec.Cartas.DataAccess.Entities;

public class SendAlsoTo
{
    public int Id { get; set; }

    public string RecipientFullName { get; set; } = string.Empty;

    public string RecipientEmail { get; set; } = string.Empty;

    public bool SendAsBcc { get; set; }
}