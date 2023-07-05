using Azure.Communication.Email;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.DataAccess.Repositories.Admin;
using Fonbec.Cartas.Logic.Services;
using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.Services.Identity;
using Fonbec.Cartas.Logic.Services.MessageTemplate;
using Fonbec.Cartas.Ui.Identity;
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
            services.AddMudServices(config =>
                config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight);

            services.AddScoped<InitialState>();

            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ICommunicationService, CommunicationService>();

            services.AddScoped<IMessageTemplateGetterService, MessageTemplateGetterService>();
            services.AddScoped<IMessageTemplateParser, MessageTemplateParser>();
            services.AddScoped<IEmbeddedResourceFileReader, EmbeddedResourceFileReader>();

            var communicationServiceConnectionString =
                configuration.GetConnectionString("CommunicationServiceConnection");
            services.AddSingleton(_ => new EmailClient(communicationServiceConnectionString));

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IFilialService, FilialService>();
            services.AddScoped<IUserWithAccountService<Coordinador>, CoordinadorService>();
            services.AddScoped<IUserWithAccountService<Mediador>, MediadorService>();
            services.AddScoped<IUserWithAccountService<Revisor>, RevisorService>();
            services.AddScoped<IPadrinoService, PadrinoService>();
            services.AddScoped<IBecarioService, BecarioService>();
            services.AddScoped<IApadrinamientoService, ApadrinamientoService>();
            services.AddScoped<IPlanService, PlanService>();

            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<IFilialesRepository, FilialesRepository>();
            services.AddScoped<IUserWithAccountRepositoryBase<Coordinador>, CoordinadorRepository>();
            services.AddScoped<IUserWithAccountRepositoryBase<Mediador>, MediadorRepository>();
            services.AddScoped<IUserWithAccountRepositoryBase<Revisor>, RevisorRepository>();
            services.AddScoped<IPadrinoRepository, PadrinoRepository>();
            services.AddScoped<IBecarioRepository, BecarioRepository>();
            services.AddScoped<IApadrinamientoRepository, ApadrinamientoRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
        }
    }
}
