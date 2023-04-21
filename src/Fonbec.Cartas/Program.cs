using Fonbec.Cartas.DataAccess;
using Fonbec.Cartas.DataAccess.Identity;
using MudBlazor.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("FonbecCartasDbContextConnection")
                       ?? throw new InvalidOperationException("Connection string 'FonbecCartasDbContextConnection' not found.");
builder.Services.AddDbContext<FonbecCartasDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
builder.Services.AddDefaultIdentity<FonbecUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<FonbecCartasDbContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddMudServices();

Fonbec.Cartas.Ui.ServiceRegistration.Register(builder.Services);

var app = builder.Build();

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
