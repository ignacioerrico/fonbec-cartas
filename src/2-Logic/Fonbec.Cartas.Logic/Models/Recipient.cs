namespace Fonbec.Cartas.Logic.Models;

public class Recipient
{
    public Recipient(string emailAddress, string? displayName = null)
    {
        DisplayName = displayName ?? emailAddress;
        EmailAddress = $"<{emailAddress}>";
    }

    public string DisplayName { get; }

    public string EmailAddress { get; }
}