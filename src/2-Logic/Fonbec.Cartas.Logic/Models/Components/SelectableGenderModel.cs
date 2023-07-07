using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.Logic.Models.Components;

public sealed class SelectableGenderModel : IEquatable<SelectableGenderModel>
{
    public SelectableGenderModel(Gender gender, string displayName)
    {
        Gender = gender;
        DisplayName = displayName;
    }

    public Gender Gender { get; }

    public string DisplayName { get; }

    public bool Equals(SelectableGenderModel? other)
    {
        if (other is null)
        {
            return false;
        }

        return other.Gender == Gender
               && string.Equals(other.DisplayName, DisplayName, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        if (obj is SelectableGenderModel genderSelectorModel)
        {
            return Equals(genderSelectorModel);
        }

        return false;
    }

    public override int GetHashCode() => Gender.GetHashCode();

    public override string ToString() => DisplayName;
}