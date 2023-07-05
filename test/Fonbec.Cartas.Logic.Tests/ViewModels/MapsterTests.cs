using Mapster;
using System.Reflection;

namespace Fonbec.Cartas.Logic.Tests.ViewModels
{
    public abstract class MapsterTests
    {
        protected MapsterTests()
        {
            var logicAssembly = Assembly.Load("Fonbec.Cartas.Logic");
            TypeAdapterConfig.GlobalSettings.Scan(logicAssembly);
        }
    }
}
