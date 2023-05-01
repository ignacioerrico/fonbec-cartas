namespace Fonbec.Cartas.Logic.Models;

public class Recipient
{
    public Recipient(string emailAddress, string? displayName = null)
    {
        EmailAddress = $"<{emailAddress}>";
        DisplayName = displayName ?? emailAddress;
    }

    public string EmailAddress { get; }
    public string DisplayName { get; }
}