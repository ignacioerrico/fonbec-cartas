namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class BecarioNameViewModel
    {
        public BecarioNameViewModel(string fullName, string firstName)
        {
            FullName = fullName;
            FirstName = firstName;
        }

        public string FullName { get; }

        public string FirstName { get; }
    }
}
