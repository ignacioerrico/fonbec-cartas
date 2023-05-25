namespace Fonbec.Cartas.Logic.ViewModels.Admin
{
    public class FilialesListViewModel
    {
        public FilialesListViewModel(string name)
        {
            Name = string.IsNullOrWhiteSpace(name)
                ? "(sin nombre)"
                : name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Coordinadores { get; set; } = new();
        public int QtyMediadores { get; set; }
        public int QtyBecarios { get; set; }
        public int QtyPadrinos { get; set; }
        public int QtyVoluntarios { get; set; }

        public string? CoordinadoresDisplay() =>
            Coordinadores.Any()
                ? string.Join(", ", Coordinadores)
                : null;
    }
}
