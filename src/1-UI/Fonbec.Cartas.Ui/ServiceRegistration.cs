using Fonbec.Cartas.Logic.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Fonbec.Cartas.Ui
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection services)
        {
            return services.AddSingleton<WeatherForecastService>();
        }
    }
}
