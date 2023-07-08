namespace Fonbec.Cartas.Logic.ViewModels.Components
{
    public class ListOrMDashViewModel
    {
        public ListOrMDashViewModel(string text, string? email = null)
        {
            Text = text;
            Email = email;
        }

        public string Text { get; }
        
        public string? Email { get; }
    }
}
