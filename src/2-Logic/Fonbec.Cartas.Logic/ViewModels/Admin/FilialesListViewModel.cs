using System.Globalization;

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

        public DateTimeOffset CreatedOnUtc { get; set; }
        public DateTimeOffset? LastUpdatedOnUtc { get; set; }

        public string? CoordinadoresDisplay() =>
            Coordinadores.Any()
                ? string.Join(", ", Coordinadores)
                : null;

        public IEnumerable<string> AuditDisplay()
        {
            var lines = new List<string>
            {
                $"{Name} (ID: {Id})",
                $"Creado: {CreatedOnUtc.ToLocalTime().ToString("f", new CultureInfo("es-AR"))}"
            };

            if (LastUpdatedOnUtc.HasValue)
            {
                lines.Add($"Última actualización: {LastUpdatedOnUtc.Value.ToLocalTime().ToString("f", new CultureInfo("es-AR"))}");
            }

            return lines;
        }
    }
}
