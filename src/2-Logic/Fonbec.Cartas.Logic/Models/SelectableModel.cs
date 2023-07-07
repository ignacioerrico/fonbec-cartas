using Fonbec.Cartas.DataAccess.DataModels;
using Mapster;

namespace Fonbec.Cartas.Logic.Models
{
    public sealed class SelectableModel : IEquatable<SelectableModel>
    {
        public SelectableModel(int id, string name)
        {
            Id = id;
            DisplayName = name;
        }

        public int Id { get; }
        
        public string DisplayName { get; }

        public bool Equals(SelectableModel? other)
        {
            if (other is null)
            {
                return false;
            }

            return other.Id == Id
                   && string.Equals(other.DisplayName, DisplayName, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            if (obj is SelectableModel selectableModel)
            {
                return Equals(selectableModel);
            }

            return false;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => DisplayName;
    }

    public class SelectableModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<SelectableDataModel, SelectableModel>()
                .ConstructUsing(dm => new SelectableModel(dm.Id, dm.DisplayName));
        }
    }
}
