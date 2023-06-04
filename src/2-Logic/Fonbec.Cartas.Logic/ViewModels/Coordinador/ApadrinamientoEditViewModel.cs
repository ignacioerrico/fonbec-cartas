namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class ApadrinamientoEditViewModel : AuditableWithAuthorNameViewModel
    {
        public ApadrinamientoEditViewModel(DateTime from, DateTime? to)
        {
            From = from;
            To = to;

            if (From > DateTime.Today)
            {
                Status = "No comenzó";
            }
            else if (To.HasValue && To.Value < DateTime.Today)
            {
                Status = "Finalizó";
            }
            else
            {
                Status = "Activo";
            }
        }

        public int ApadrinamientoId { get; set; }

        public int PadrinoId { get; set; }

        public string PadrinoFullName { get; set; } = default!;

        public DateTime From { get; set; }

        public DateTime? To { get; set; }

        public string Status { get; set; }
    }
}
