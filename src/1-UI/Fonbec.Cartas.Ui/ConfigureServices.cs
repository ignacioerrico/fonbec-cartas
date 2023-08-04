using Azure.Communication.Email;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.DataAccess.Repositories.Admin;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.DataAccess.Repositories.Coordinador;
using Fonbec.Cartas.DataAccess.Repositories.Mediador;
using Fonbec.Cartas.Logic.Models.Admin.DataImport;
using Fonbec.Cartas.Logic.Properties;
using Fonbec.Cartas.Logic.Services;
using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.Services.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.Services.Identity;
using Fonbec.Cartas.Logic.Services.Mediador;
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

            services.AddScoped<IResourcesWrapper, ResourcesWrapper>();
            services.AddScoped<IMessageTemplateGetterService, MessageTemplateGetterService>();
            services.AddScoped<IMessageTemplateParser, MessageTemplateParser>();

            var communicationServiceConnectionString =
                configuration.GetConnectionString("CommunicationServiceConnection");
            services.AddSingleton(_ => new EmailClient(communicationServiceConnectionString));

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IFilialService, FilialService>();
            services.AddScoped<IUserWithAccountSharedService, UserWithAccountSharedService>();
            services.AddScoped<IUserWithAccountService<Coordinador>, CoordinadorService>();
            services.AddScoped<IUserWithAccountService<Mediador>, MediadorService>();
            services.AddScoped<IUserWithAccountService<Revisor>, RevisorService>();
            services.AddScoped<IPadrinoService, PadrinoService>();
            services.AddScoped<IBecarioService, BecarioService>();
            services.AddScoped<IApadrinamientoService, ApadrinamientoService>();
            services.AddScoped<IPlannedEventService, PlannedEventService>();
            services.AddScoped<IUploadDocumentService, UploadDocumentService>();

            services.AddScoped<IDataImportService, DataImportService>();
            services.AddScoped<FileParserBase<Coordinador, UserWithAccountToCreate>, UserWithAccountFileParser<Coordinador>>();
            services.AddScoped<FileParserBase<Mediador, UserWithAccountToCreate>, UserWithAccountFileParser<Mediador>>();
            services.AddScoped<FileParserBase<Revisor, UserWithAccountToCreate >, UserWithAccountFileParser<Revisor>>();
            services.AddScoped<FileParserBase<Padrino, Padrino>, PadrinosFileParser>();
            services.AddScoped<FileParserBase<SendAlsoTo, SendAlsoTo>, SendAltoToFileParser>();
            services.AddScoped<FileParserBase<Becario, Becario>, BecarioFileParser>();
            services.AddScoped<FileParserBase<Apadrinamiento, Apadrinamiento>, ApadrinamientosFileParser>();
            services.AddScoped<ICreateUserWithAccountService<Coordinador>, CreateUserWithAccountService<Coordinador>>();
            services.AddScoped<ICreateUserWithAccountService<Mediador>, CreateUserWithAccountService<Mediador>>();
            services.AddScoped<ICreateUserWithAccountService<Revisor>, CreateUserWithAccountService<Revisor>>();

            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<IFilialesRepository, FilialesRepository>();
            services.AddScoped<IUserWithAccountRepositoryBase<Coordinador>, CoordinadorRepository>();
            services.AddScoped<IUserWithAccountRepositoryBase<Mediador>, MediadorRepository>();
            services.AddScoped<IUserWithAccountRepositoryBase<Revisor>, RevisorRepository>();
            services.AddScoped<IPadrinoRepository, PadrinoRepository>();
            services.AddScoped<IBecarioRepository, BecarioRepository>();
            services.AddScoped<IApadrinamientoRepository, ApadrinamientoRepository>();
            services.AddScoped<IPlannedEventRepository, PlannedEventRepository>();
            services.AddScoped<IUploadDocumentRepository, UploadDocumentRepository>();

            services.AddScoped<ICreateUserWithAccountRepository<Coordinador>, CreateUserWithAccountRepository<Coordinador>>();
            services.AddScoped<ICreateUserWithAccountRepository<Mediador>, CreateUserWithAccountRepository<Mediador>>();
            services.AddScoped<ICreateUserWithAccountRepository<Revisor>, CreateUserWithAccountRepository<Revisor>>();
            services.AddScoped<IDataImportRepository, DataImportRepository>();
        }
    }
}
