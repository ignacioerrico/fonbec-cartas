using Fonbec.Cartas.DataAccess;
using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.Ui.Identity;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Fonbec.Cartas.DataAccess.Triggers;

var builder = WebApplication.CreateBuilder(args);

// Entity Framework
var connectionString = builder.Configuration.GetConnectionString("FonbecCartasDbContextConnection")
                       ?? throw new InvalidOperationException("Connection string 'FonbecCartasDbContextConnection' not found.");
builder.Services
    .AddDbContextFactory<FonbecCartasIdentityDbContext>(options =>
        options.UseSqlServer(connectionString)
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services
    .AddDbContextFactory<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString)
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseTriggers(triggerOptions =>
            {
                triggerOptions.AddTrigger<PlannedEventAfterSaveTrigger>();
                triggerOptions.AddTrigger<ApadrinamientoAfterSaveTrigger>();
            }));

// Identity
builder.Services
    .AddDefaultIdentity<FonbecUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FonbecCartasIdentityDbContext>();

// Overrides registration done internally by AddDefaultIdentity.
builder.Services.AddScoped<IUserClaimsPrincipalFactory<FonbecUser>, FonbecUserClaimsPrincipalFactory>();

// Google external login
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (googleClientId is not null && googleClientSecret is not null)
{
    builder.Services
        .AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
        });
}

// Mapster (each View Model declares its own mapping)
var logicAssembly = Assembly.Load("Fonbec.Cartas.Logic");
TypeAdapterConfig.GlobalSettings.Scan(logicAssembly);

// Razor Pages (required by ASP.NET Core Identity)
builder.Services.AddRazorPages();

// Blazor Server
builder.Services.AddServerSideBlazor();

// Options
Fonbec.Cartas.Ui.ConfigureServices.RegisterOptions(builder.Services, builder.Configuration);

// Services
Fonbec.Cartas.Ui.ConfigureServices.RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

Fonbec.Cartas.Ui.Configure.SeedAminUser(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
