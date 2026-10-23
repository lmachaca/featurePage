using FeatureFlags.Api.Components;
using FeatureFlags.Api.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/login";                  // sin sesión, te manda aquí
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();               // habilita [Authorize]
builder.Services.AddCascadingAuthenticationState(); // Blazor sabe quién entró

var app = builder.Build();


app.MapGet("/api/flags", async (AppDbContext db) =>
    await db.FeatureFlags
        .AsNoTracking()          // solo lectura: más rápido
        .OrderBy(f => f.Name)
        .ToListAsync());         // .NET lo convierte a JSON automáticamente

// ===== Login: recibe el formulario de Login.razor =====
app.MapPost("/auth/login", async (HttpContext ctx, IConfiguration config,
    [FromForm] string usuario, [FromForm] string password, [FromForm] string? returnUrl) =>
{
    // Compara con el admin de appsettings.json
    var admin = config.GetSection("Admin");
    if (usuario != admin["User"] || password != admin["Password"])
        return Results.Redirect("/login?error=1");

    // Crea la "identidad" del usuario (su nombre)
    var identity = new ClaimsIdentity(
        [new Claim(ClaimTypes.Name, usuario)],
        CookieAuthenticationDefaults.AuthenticationScheme);

    // Escribe la cookie de sesión en el navegador
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity));

    // Regresa a la página que pidió, solo si es del mismo sitio
    var destino = !string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith('/')
                  && !returnUrl.StartsWith("//") ? returnUrl : "/flags";
    return Results.LocalRedirect(destino);
});

// ===== Logout: borra la cookie =====
app.MapGet("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
