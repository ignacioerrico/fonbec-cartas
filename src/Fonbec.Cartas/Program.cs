using Fonbec.Cartas.DataAccess;
using Fonbec.Cartas.DataAccess.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Entity Framework
var connectionString = builder.Configuration.GetConnectionString("FonbecCartasDbContextConnection")
                       ?? throw new InvalidOperationException("Connection string 'FonbecCartasDbContextConnection' not found.");
builder.Services
    .AddDbContext<FonbecCartasIdentityDbContext>(options =>
        options.UseSqlServer(connectionString));

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

// Identity
builder.Services
    .AddDefaultIdentity<FonbecUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FonbecCartasIdentityDbContext>();

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

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

Fonbec.Cartas.Ui.ConfigureServices.RegisterOptions(builder.Services, builder.Configuration);

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
