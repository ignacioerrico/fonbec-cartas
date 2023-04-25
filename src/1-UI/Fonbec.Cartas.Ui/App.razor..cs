using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui
{
    public partial class App
    {
        [Inject]
        public InitialState TokenProvider { get; set; }

        [Parameter]
        public InitialState InitialState { get; set; }

        protected override Task OnInitializedAsync()
        {
            TokenProvider.XsrfToken = InitialState.XsrfToken;

            return base.OnInitializedAsync();
        }
    }
}
