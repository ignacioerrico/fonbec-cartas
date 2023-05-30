using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui
{
    public partial class App
    {
        [Inject]
        public InitialState TokenProvider { get; set; } = default!;

        [Parameter]
        public InitialState InitialState { get; set; } = default!;

        protected override Task OnInitializedAsync()
        {
            TokenProvider.XsrfToken = InitialState.XsrfToken;

            return base.OnInitializedAsync();
        }
    }
}
