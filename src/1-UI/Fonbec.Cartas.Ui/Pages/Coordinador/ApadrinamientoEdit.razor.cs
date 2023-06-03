using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.ServicesCoordinador;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Components.Dialogs;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class ApadrinamientoEdit
    {
        private bool _loading;
        private string? _pageTitle;

        private List<AssignNewPadrinoDialogModel> _padrinosAsignados = new();
        private string _becarioName = default!;

        private List<PadrinoViewModel> _padrinos = new();
        
        private int _coordinadorId;

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        [Inject]
        public IApadrinamientoService ApadrinamientoService { get; set; } = default!;

        [Inject]
        public IBecarioService BecarioService { get; set; } = default!;

        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        public int BecarioId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            if (AuthenticationState is null)
            {
                Snackbar.Add("AuthenticationState is null.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
                return;
            }

            var user = (await AuthenticationState).User;
            if (user.Identity is not { IsAuthenticated: true })
            {
                Snackbar.Add("Usuario no está autenticado.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
                return;
            }

            _coordinadorId = user.UserWithAccountId() ?? throw new NullReferenceException("No claim UserWithAccountId found");

            var filialId = user.FilialId();

            if (filialId is null)
            {
                Snackbar.Add("Filial no está en el claim.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
                return;
            }

            var becarioName = await BecarioService.GetBecarioNameAsync(filialId.Value, BecarioId);

            if (becarioName is null)
            {
                Snackbar.Add("No existe el becario en esta filial.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
                return;
            }

            _becarioName = becarioName.FirstName;

            _pageTitle = $"Padrinos de {becarioName.FullName}";

            _padrinos = await BecarioService.GetAllPadrinosForSelectionAsync(filialId.Value);

            var apadrinamientosViewModel = await ApadrinamientoService.GetAllPadrinosForBecario(BecarioId);
            _padrinosAsignados = apadrinamientosViewModel.Select(a =>
                    new AssignNewPadrinoDialogModel(new PadrinoViewModel(a.PadrinoId, a.PadrinoFullName), a.From, a.To))
                .ToList();

            _loading = false;
        }

        private async Task OpenAssignNewPadrinoDialogAsync()
        {
            var parameters = new DialogParameters
            {
                ["Padrinos"] = _padrinos,
            };
            var options = new DialogOptions
            {
                CloseButton = true,
                CloseOnEscapeKey = true
            };
            var dialog = await DialogService.ShowAsync<AssignNewPadrinoDialog>("Nueva asignación", parameters, options);
            var result = await dialog.Result;

            if (result.Canceled)
            {
                return;
            }

            if (result.Data is not AssignNewPadrinoDialogModel nuevaAsignación)
            {
                return;
            }

            // Is there any overlap with another assignment for the same Padrino?
            var overlapWithSamePadrino = _padrinosAsignados.Any(asignación =>
                string.Equals(asignación.PadrinoViewModel.Name, nuevaAsignación.PadrinoViewModel.Name, StringComparison.OrdinalIgnoreCase)
                && ((nuevaAsignación.Desde < asignación.Desde && (!nuevaAsignación.Hasta.HasValue || asignación.Desde < nuevaAsignación.Hasta.Value))
                    || (asignación.Desde < nuevaAsignación.Desde && (!asignación.Hasta.HasValue || nuevaAsignación.Desde < asignación.Hasta.Value))));
            if (overlapWithSamePadrino)
            {
                Snackbar.Add($"{nuevaAsignación.PadrinoViewModel.Name} ya apadrina a {_becarioName} en ese período.", Severity.Error);
                return;
            }

            var assignPadrinoToBecarioViewModel = new AssignPadrinoToBecarioViewModel
            {
                BecarioId = BecarioId,
                PadrinoId = nuevaAsignación.PadrinoViewModel.Id,
                From = nuevaAsignación.Desde,
                To = nuevaAsignación.Hasta,
                CreatedByCoordinadorId = _coordinadorId,
            };

            var qtyAssigned = await ApadrinamientoService.AssignPadrinoToBecarioAsync(assignPadrinoToBecarioViewModel);

            if (qtyAssigned == 0)
            {
                Snackbar.Add("No se pudo almacenar la asignación.", Severity.Error);
                return;
            }

            _padrinosAsignados.Add(nuevaAsignación);
        }
    }
}
