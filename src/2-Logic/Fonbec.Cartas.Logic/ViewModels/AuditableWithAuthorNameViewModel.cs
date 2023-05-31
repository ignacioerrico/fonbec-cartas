using System.Globalization;

namespace Fonbec.Cartas.Logic.ViewModels
{
    public abstract class AuditableWithAuthorNameViewModel : AuditableViewModel
    {
        public string CreatedBy { get; set; } = default!;

        public string? UpdatedBy { get; set; }

        public override IEnumerable<string> AuditDisplay()
        {
            var lines = new List<string>
            {
                $"Creado: {CreatedOnUtc.ToLocalTime().ToString("f", new CultureInfo("es-AR"))} por {CreatedBy}"
            };

            if (LastUpdatedOnUtc.HasValue)
            {
                lines.Add($"Última actualización: {LastUpdatedOnUtc.Value.ToLocalTime().ToString("f", new CultureInfo("es-AR"))} por {UpdatedBy}");
            }

            return lines;
        }
    }
}
