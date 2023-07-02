using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui.Components
{
    public partial class ActivityIndicator
    {
        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public string LoadingText { get; set; } = "Cargado datos";

        [Parameter]
        public bool If { get; set; } = false;

        [Parameter]
        public string ThenDisplay { get; set; } = string.Empty;
    }
}
