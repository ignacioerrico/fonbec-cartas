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
            return other?.Id == Id && other?.DisplayName == DisplayName;
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
}
