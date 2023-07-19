namespace Fonbec.Cartas.Logic.Properties
{
    public interface IResourcesWrapper
    {
        string MessageTemplate { get; }
        string DefaultSubject { get; }
        string DefaultMessageMarkdown { get; }
    }

    public class ResourcesWrapper : IResourcesWrapper
    {
        public string MessageTemplate => Properties.Resources.MessageTemplate;
        
        public string DefaultSubject => Properties.Resources.DefaultSubject;
        
        public string DefaultMessageMarkdown => Properties.Resources.DefaultMessageMarkdown;
    }
}
