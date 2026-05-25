using InterTrips___Turistička_Agencija.Background;
using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Models;
using InterTrips___Turistička_Agencija.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())
           .ConfigureWarnings(warnings => warnings.Ignore(
               Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning
           )));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<EmailAndDocumentService>();
builder.Services.AddHostedService<PozadinskiProcesiService>();
builder.Services.AddHostedService<PutovanjeReminderWorker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (app.Environment.IsDevelopment())
    {
        await db.Database.MigrateAsync();
    }

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "Agent", "Client" };
    foreach (var r in roles)
    {
        var role = await roleManager.FindByNameAsync(r);
        if (role == null)
        {
            await roleManager.CreateAsync(new IdentityRole(r));
        }
    }

    async Task EnsureUser(string email, string password, string role)
    {
        var u = await userManager.FindByEmailAsync(email);
        if (u == null)
        {
            u = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var created = await userManager.CreateAsync(u, password);
            if (!created.Succeeded)
                throw new Exception(string.Join(" | ", created.Errors.Select(e => e.Description)));
        }

        if (!await userManager.IsInRoleAsync(u, role))
            await userManager.AddToRoleAsync(u, role);
    }

    await EnsureUser("agent@intertrips.ba", "agent123", "Agent");
    await EnsureUser("admin@intertrips.ba", "admin123", "Admin");
    await EnsureUser("test@intertrips.ba", "password123", "Client");
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();