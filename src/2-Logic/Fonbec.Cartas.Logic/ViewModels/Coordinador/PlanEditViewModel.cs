namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PlanEditViewModel
    {
        public int FilialId { get; set; }

        public DateTime? StartDate { get; set; }

        public string Subject { get; set; } = "Carta de tu {ahijado} {ahijado:nombre} de {mes-de-carta}";

        public string MessageMarkdown { get; set; } = string.Empty;

        public int CreatedByCoordinadorId { get; set; }
    }
}
