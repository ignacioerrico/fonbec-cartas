using Fonbec.Cartas.Logic.Data;
using Fonbec.Cartas.Ui.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace Fonbec.Cartas.Ui
{
    public static class ConfigureServices
    {
        public static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AdminUserOptions>(
                configuration.GetSection(AdminUserOptions.SectionName));
        }

        public static void RegisterServices(IServiceCollection services)
        {
            services.AddMudServices();

            services.AddScoped<InitialState>();

            services.AddSingleton<WeatherForecastService>();
        }
    }
}
