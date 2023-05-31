using System.Globalization;

namespace Fonbec.Cartas.Logic.ViewModels
{
    public abstract class AuditableViewModel
    {
        public DateTimeOffset CreatedOnUtc { get; set; }
        public DateTimeOffset? LastUpdatedOnUtc { get; set; }

        public virtual IEnumerable<string> AuditDisplay()
        {
            var lines = new List<string>
            {
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
