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
        private PlanPreviewViewModel _plan = new();

        private bool _loading;
        private string? _pageTitle;

        [Parameter]
        public string PlanId { get; set; } = string.Empty;

        [Inject]
        public IPlanService PlanService { get; set; } = default!;

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

            if (int.TryParse(PlanId, out var planId) && planId > 0)
            {
                var result = await PlanService.GetPlanForPreviewAsync(planId, authenticatedUserData.FilialId);

                if (!result.IsFound || result.Data is null)
                {
                    Snackbar.Add($"No se encontró plan con ID {PlanId}.", Severity.Error);
                    NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanNew);
                    return;
                }

                _plan = result.Data;

                _pageTitle = $"Plan de {result.Data.StartDate.ToPlanName()}";
                _messageTemplateData.Date = result.Data.StartDate;
            }
            else
            {
                Snackbar.Add($"'{PlanId}' no es un ID de plan válido.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanes);
            }

            UpdatePreview();

            _loading = false;
        }

        private void UpdatePreview()
        {
            _renderedSubject = MessageTemplateParser.FillPlaceholders(_plan.Subject, _messageTemplateData, _highlight);
            _renderedMessageBody = MessageTemplateGetterService.GetHtmlMessage(_plan.MessageMarkdown, _messageTemplateData, _highlight);
        }
    }
}
