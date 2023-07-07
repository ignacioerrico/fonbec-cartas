using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.Models.Components;
using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui.Components
{
    public partial class GenderSelector
    {
        private static readonly SelectableGenderModel Male = new(Gender.Male, "Masculino");
        private static readonly SelectableGenderModel Female = new(Gender.Female, "Femenino");

        private SelectableGenderModel _selectedGender = Male;

        [Parameter]
        public Gender SelectedGender { get; set; }

        [Parameter]
        public EventCallback<Gender> SelectedGenderChanged { get; set; }

        private async Task OnSelectedValuesChanged(IEnumerable<SelectableGenderModel> selectedGenders)
        {
            await SelectedGenderChanged.InvokeAsync(selectedGenders.Single().Gender);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (SelectedGender == Gender.Unknown)
            {
                // If no gender is selected at invocation time, set it to the default.
                await SelectedGenderChanged.InvokeAsync(_selectedGender.Gender);
            }
            else if (SelectedGender == Gender.Female)
            {
                _selectedGender = Female;
            }

            await base.OnParametersSetAsync();
        }
    }
}
