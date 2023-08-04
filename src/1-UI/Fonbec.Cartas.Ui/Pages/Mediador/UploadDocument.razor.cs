using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Models;
using Fonbec.Cartas.Logic.Services.Mediador;
using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui.Pages.Mediador
{
    public partial class UploadDocument : PerFilialComponentBase
    {
        private bool _loading;

        private List<SelectableModel<int>> _becarios = new();
        private SelectableModel<int>? _selectedBecario;

        private List<SelectableModel<int>> _documentTypes = new();
        private SelectableModel<int>? _selectedDocumentType;

        [Inject]
        public IUploadDocumentService UploadDocumentService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var authenticatedUserData = await GetAuthenticatedUserDataAsync();
            if (!authenticatedUserData.DataObtainedSuccessfully)
            {
                _loading = false;
                return;
            }

            var mediadorId = authenticatedUserData.User.UserWithAccountId()
                             ?? throw new NullReferenceException("No claim UserWithAccountId found");

            _becarios = await UploadDocumentService.GetBecariosAssignedToMediador(mediadorId);

            _loading = false;
        }

        private async Task<IEnumerable<SelectableModel<int>>> SearchBecario(string searchString)
        {
            await Task.Delay(5);

            if (string.IsNullOrWhiteSpace(searchString))
            {
                return _becarios;
            }

            return _becarios.Where(m => m.DisplayName.ContainsIgnoringAccents(searchString));
        }

        private void OnSelectedBecarioChanged(SelectableModel<int> selectedBecario)
        {
            _selectedBecario = selectedBecario;

        }

        private void OnSelectedDocumentTypeChanged(SelectableModel<int> selectedDocumentType)
        {
            if (_selectedBecario is null)
            {
                return;
            }

            _selectedDocumentType = selectedDocumentType;
        }

    }
}
