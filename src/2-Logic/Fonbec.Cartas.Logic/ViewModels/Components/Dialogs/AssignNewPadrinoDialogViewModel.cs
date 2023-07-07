using Fonbec.Cartas.Logic.Models;

namespace Fonbec.Cartas.Logic.ViewModels.Components.Dialogs;

public class AssignNewPadrinoDialogViewModel
{
    public SelectableModel? SelectedPadrino { get; set; }

    public DateTime? Desde { get; set; } = DateTime.Today;

    public DateTime? Hasta { get; set; }

    public bool KnownEndDate { get; set; }
}