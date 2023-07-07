namespace Fonbec.Cartas.Logic.ViewModels.Components.Dialogs;

public class AddSendAlsoToDialogViewModel
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool SendAsBcc { get; set; }
}