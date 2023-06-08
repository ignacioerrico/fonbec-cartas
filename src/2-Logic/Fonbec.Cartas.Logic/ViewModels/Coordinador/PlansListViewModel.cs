namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PlansListViewModel : AuditableViewModel
    {
        public int Id { get; set; }

        public string PlanName { get; set; } = default!;

        public int TotalLettersToSend { get; set; }

        public int LettersSent { get; set; }

        public float Percentage { get; set; }
    }
}
