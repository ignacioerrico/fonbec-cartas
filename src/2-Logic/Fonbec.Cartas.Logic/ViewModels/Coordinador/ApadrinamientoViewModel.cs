namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class ApadrinamientoViewModel
    {
        public int PadrinoId { get; set; }

        public string PadrinoFullName { get; set; } = default!;

        public DateTime From { get; set; }

        public DateTime? To { get; set; }
    }
}
