using Fonbec.Cartas.DataAccess.DataModels;
using Mapster;

namespace Fonbec.Cartas.Logic.Models
{
    public sealed class SelectableModel<TKey> : IEquatable<SelectableModel<TKey>>
        where TKey : struct
    {
        public SelectableModel(TKey id, string name)
        {
            Id = id;
            DisplayName = name;
        }

        public TKey Id { get; }
        
        public string DisplayName { get; }

        public bool Equals(SelectableModel<TKey>? other)
        {
            if (other is null)
            {
                return false;
            }

            return Id.Equals(other.Id)
                   && string.Equals(DisplayName, other.DisplayName, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            if (obj is SelectableModel<TKey> selectableModel)
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
            config.NewConfig<SelectableDataModel, SelectableModel<int>>()
                .ConstructUsing(dm => new SelectableModel<int>(dm.Id, dm.DisplayName));
        }
    }
}
