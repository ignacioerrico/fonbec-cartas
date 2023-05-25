using Azure.Communication.Email;
using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.Logic.Data;
using Fonbec.Cartas.Logic.Services;
using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Ui.Areas.Identity;
using Fonbec.Cartas.Ui.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
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

        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMudServices();

            services.AddScoped<InitialState>();

            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ICommunicationService, CommunicationService>();

            var communicationServiceConnectionString =
                configuration.GetConnectionString("CommunicationServiceConnection");
            services.AddSingleton(_ => new EmailClient(communicationServiceConnectionString));

            services.AddScoped<IFilialService, FilialService>();
            
            services.AddScoped<IFilialesRepository, FilialesRepository>();

            services.AddSingleton<WeatherForecastService>();
        }
    }
}
