namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class AssignPadrinoToBecarioViewModel
    {
        public int BecarioId { get; set; }

        public int PadrinoId { get; set; }

        public DateTime From { get; set; }

        public DateTime? To { get; set; }

        public int CreatedByCoordinadorId { get; set; }
    }
}
