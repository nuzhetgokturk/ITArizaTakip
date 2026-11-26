
using ITArizaTakip.Components;
using ITArizaTakip.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL veritabanı bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Kullanıcı ve rol sistemi
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<DatabaseTest>();
builder.Services.AddScoped<ITArizaTakip.Services.ArizaService>();
builder.Services.AddScoped<ITArizaTakip.Services.PersonelService>();
builder.Services.AddScoped<ITArizaTakip.Services.DashboardService>();
builder.Services.AddScoped<ITArizaTakip.Services.RaporlamaService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true
);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost("/account/login", async (
    HttpContext context,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IAntiforgery antiforgery) =>
{
    await antiforgery.ValidateRequestAsync(context);

    var form = await context.Request.ReadFormAsync();

    var email = form["Email"].ToString();
    var sifre = form["Sifre"].ToString();
    var rol = form["Rol"].ToString();

    var kullanici = await userManager.FindByEmailAsync(email);

    if (kullanici == null || (rol != "Personel" && rol != "Yönetici"))
        return Results.Redirect("/giris?hata=1");

    var roller = await userManager.GetRolesAsync(kullanici);

    if (!roller.Contains(rol))
        return Results.Redirect("/giris?hata=1");

    var sonuc = await signInManager.CheckPasswordSignInAsync(
        kullanici,
        sifre,
        lockoutOnFailure: true
    );

    if (!sonuc.Succeeded)
        return Results.Redirect("/giris?hata=1");

    await signInManager.SignInAsync(kullanici, isPersistent: false);

    return Results.Redirect("/dashboard");
});

app.MapPost("/account/logout", async (
    HttpContext context,
    SignInManager<ApplicationUser> signInManager,
    IAntiforgery antiforgery) =>
{
    await antiforgery.ValidateRequestAsync(context);

    await signInManager.SignOutAsync();

    return Results.Redirect("/giris");
});

// Rolleri oluştur
await RoleSeeder.RolleriOlusturAsync(app.Services);

// Departmanları oluştur
await DepartmanSeeder.DepartmanlariOlusturAsync(app.Services);

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
