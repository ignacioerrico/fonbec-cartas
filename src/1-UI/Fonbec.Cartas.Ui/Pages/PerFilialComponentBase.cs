using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Ui.Constants;
using Fonbec.Cartas.Ui.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages
{
    public class PerFilialComponentBase : ComponentBase
    {
        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected async Task<AuthenticatedUserData> GetAuthenticatedUserDataAsync()
        {
            var authenticatedUserData = new AuthenticatedUserData();

            if (AuthenticationState is null)
            {
                Snackbar.Add("AuthenticationState is null.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.Root);
                return authenticatedUserData;
            }

            var user = (await AuthenticationState).User;

            if (user.Identity is not { IsAuthenticated: true })
            {
                Snackbar.Add("El usuario no está autenticado.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.Root);
                return authenticatedUserData;
            }

            var filialId = user.FilialId();

            if (filialId is null)
            {
                throw new InvalidOperationException("No claim FilialId found");
            }
            
            return authenticatedUserData.SetData(user, filialId.Value);
        }
    }
}
