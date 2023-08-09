using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.Services.MessageTemplate;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Components.Coordinador;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PlanPreview : PerFilialComponentBase
    {
        private PlannedEventPreviewViewModel _viewModel = new();

        private bool _loading;
        private string? _pageTitle;

        [Parameter]
        public string PlannedEventId { get; set; } = string.Empty;

        [Inject]
        public IPlannedEventService PlannedEventService { get; set; } = default!;

        [Inject]
        public IMessageTemplateGetterService MessageTemplateGetterService { get; set; } = default!;

        [Inject]
        public IMessageTemplateParser MessageTemplateParser { get; set; } = default!;

        private readonly MessageTemplateData _messageTemplateData = new()
        {
            Documents = "la carta y el boletín",
            Padrino = PersonSelectorForPreview.Padrino,
            Becario = PersonSelectorForPreview.Ahijado,
            Revisor = new PersonData("Victoria", Gender.Female),
            FilialNombre = "AMBA",
        };

        private bool _highlight;

        private string _renderedSubject = default!;
        private string _renderedMessageBody = default!;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var authenticatedUserData = await GetAuthenticatedUserDataAsync();
            if (!authenticatedUserData.DataObtainedSuccessfully)
            {
                _loading = false;
                return;
            }

            if (int.TryParse(PlannedEventId, out var plannedEventId) && plannedEventId > 0)
            {
                var result = await PlannedEventService.GetPlannedEventForPreviewAsync(plannedEventId, authenticatedUserData.FilialId);

                if (!result.IsFound || result.Data is null)
                {
                    Snackbar.Add($"No se encontró plan con ID {PlannedEventId}.", Severity.Error);
                    NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanificaciónNew);
                    return;
                }

                _viewModel = result.Data;

                _pageTitle = $"Plan de {result.Data.StartDate.ToPlanName()}";
                _messageTemplateData.Date = result.Data.StartDate;
            }
            else
            {
                Snackbar.Add($"'{PlannedEventId}' no es un ID de planificación válido.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanificación);
            }

            UpdatePreview();

            _loading = false;
        }

        private void UpdatePreview()
        {
            _renderedSubject = MessageTemplateParser.FillPlaceholders(_viewModel.Subject, _messageTemplateData, _highlight);
            _renderedMessageBody = MessageTemplateGetterService.GetHtmlMessage(_viewModel.MessageMarkdown, _messageTemplateData, _highlight);
        }
    }
}
