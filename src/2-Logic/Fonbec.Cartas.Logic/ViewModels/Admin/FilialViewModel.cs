namespace Fonbec.Cartas.Logic.ViewModels.Admin
{
    public class FilialViewModel
    {
        public FilialViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }

        public override bool Equals(object? obj)
        {
            var other = obj as FilialViewModel;
            return other?.Id == Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Name;
    }
}
