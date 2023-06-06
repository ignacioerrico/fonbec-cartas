using System.Reflection;

namespace Fonbec.Cartas.Logic.Services.MessageTemplate
{
    public interface IEmbeddedResourceFileReader
    {
        string Read(string embeddedResourceFileName);
    }

    public class EmbeddedResourceFileReader : IEmbeddedResourceFileReader
    {
        public string Read(string embeddedResourceFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var @namespace = typeof(MessageTemplateGetterService).Assembly.GetName().Name;
            var resourceName = $"{@namespace}.Resources.{embeddedResourceFileName}";

            using var stream = assembly.GetManifestResourceStream(resourceName)
                               ?? throw new FileNotFoundException(embeddedResourceFileName);

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
