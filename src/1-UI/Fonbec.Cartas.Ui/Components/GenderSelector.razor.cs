using Fonbec.Cartas.DataAccess.Entities.Enums;
using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui.Components
{
    public partial class GenderSelector
    {
        private static readonly GenderSelectorModel Male = new(Gender.Male, "Masculino");
        private static readonly GenderSelectorModel Female = new(Gender.Female, "Femenino");

        private GenderSelectorModel _selectedGender = Male;

        [Parameter]
        public Gender SelectedGender { get; set; }

        [Parameter]
        public EventCallback<Gender> SelectedGenderChanged { get; set; }

        private async Task SelectedValuesChanged(IEnumerable<GenderSelectorModel> selectedGenders)
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

    public class GenderSelectorModel
    {
        public GenderSelectorModel(Gender gender, string displayName)
        {
            Gender = gender;
            DisplayName = displayName;
        }

        public Gender Gender { get; }

        public string DisplayName { get; }

        public override bool Equals(object? obj)
        {
            var other = obj as GenderSelectorModel;
            return other?.Gender == Gender;
        }

        public override int GetHashCode() => Gender.GetHashCode();

        public override string ToString() => DisplayName;
    }
}
