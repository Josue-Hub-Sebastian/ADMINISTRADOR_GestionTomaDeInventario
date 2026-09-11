using Gestion_TomaInventario.Repository.EmpresaRepo;
using Gestion_TomaInventario.Repository.UsuarioRepo;
using Gestion_TomaInventario.Services.EmpresaServ;
using Gestion_TomaInventario.Services.UsuarioServ;
using Gestion_TomaInventario.Repository.PlanRepo;
using Gestion_TomaInventario.Services.PlanServ;
using Microsoft.AspNetCore.Authentication.Cookies;
using Gestion_TomaInventario.Repository.BackUpRepo;
using Gestion_TomaInventario.Services.BackupServ;
using Gestion_TomaInventario.Services.BackUpManager;
using Gestion_TomaInventario.Services.ColaSincronizacionServ;
using Gestion_TomaInventario.Repository.ColaSincronizadoRepo;
using Gestion_TomaInventario.Services;

var builder = WebApplication.CreateBuilder(args);

// ya sabes :v
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IBackupRepository,BackupRepository>();
builder.Services.AddScoped<IBackupService,BackupService>();
builder.Services.AddScoped<IBackupManager,BackupManager>();
builder.Services.AddScoped<IColaSincronizacionRepository, ColaSincronizacionRepository>();
builder.Services.AddScoped<IColaSincronizacionService, ColaSincronizacionService>();
builder.Services.AddHostedService<SyncHostedService>();// aqui la logica de la hora dia mes para calular el backup :v ;// aqui la logica de la hora dia mes para calular el backup :v
builder.Services.AddHostedService<BackupHostedService>();// aqui la logica de la hora dia mes para calular el backup :v ;// aqui la logica de la hora dia mes para calular el backup :v
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "GestionTomaInventario.Auth";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Bienvenida}/{id?}");

app.Run();
