using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.Services.MessageTemplate;
using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui.Components.Coordinador
{
    public partial class PersonSelectorForPreview
    {
        public static readonly PersonData Padrino = new("Patricio", Gender.Male);
        public static readonly PersonData Madrina = new("María", Gender.Female);

        public static readonly PersonData Ahijado = new("Benjamín", Gender.Male);
        public static readonly PersonData Ahijada = new("Beatriz", Gender.Female);

        [Parameter]
        public PersonData SelectedPadrino { get; set; } = Padrino;

        [Parameter]
        public PersonData SelectedBecario { get; set; } = Ahijado;

        [Parameter]
        public bool Highlight { get; set; }

        [Parameter]
        public EventCallback<PersonData> SelectedPadrinoChanged { get; set; }

        [Parameter]
        public EventCallback<PersonData> SelectedBecarioChanged { get; set; }

        [Parameter]
        public EventCallback<bool> HighlightChanged { get; set; }

        [Parameter]
        public EventCallback StateChanged { get; set; }

        [Inject]
        public IMessageTemplateGetterService MessageTemplateGetterService { get; set; } = default!;

        [Inject]
        public IMessageTemplateParser MessageTemplateParser { get; set; } = default!;

        private async Task OnStateChanged()
        {
            await StateChanged.InvokeAsync();
        }

        private async Task OnSelectedPadrinoChanged(PersonData padrinoData)
        {
            SelectedPadrino = padrinoData;
            await SelectedPadrinoChanged.InvokeAsync(padrinoData);
            await OnStateChanged();
        }

        private async Task OnSelectedBecarioChanged(PersonData becarioData)
        {
            SelectedBecario = becarioData;
            await SelectedBecarioChanged.InvokeAsync(becarioData);
            await OnStateChanged();
        }

        private async Task OnHighlightChanged(bool highlight)
        {
            Highlight = highlight;
            await HighlightChanged.InvokeAsync(highlight);
            await OnStateChanged();
        }
    }
}
