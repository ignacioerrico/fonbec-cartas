namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class PlansListViewModel : AuditableWithAuthorNameViewModel
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public string PlanName { get; set; } = default!;

        public int TotalLettersToSend { get; set; }

        public int LettersSent { get; set; }

        public float Percentage { get; set; }
    }
}
