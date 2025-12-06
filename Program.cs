using Microsoft.EntityFrameworkCore;
using PoliclinicoWeb.Data;

var builder = WebApplication.CreateBuilder(args);

// Cambiar puerto
builder.WebHost.UseUrls("http://localhost:5555");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Agregar soporte para sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registrar servicios de acceso a datos (Inyección de Dependencias)
builder.Services.AddScoped<PoliclinicoWeb.Data.IEspecialidadData, PoliclinicoWeb.Data.EspecialidadData>();
builder.Services.AddScoped<PoliclinicoWeb.Data.IDoctorData, PoliclinicoWeb.Data.DoctorData>();
// Use EF implementation for PacienteData (falls back to ADO.NET class if needed)
builder.Services.AddScoped<PoliclinicoWeb.Data.IPacienteData, PoliclinicoWeb.Data.PacienteEfData>();
builder.Services.AddScoped<PoliclinicoWeb.Data.IUsuarioData, PoliclinicoWeb.Data.UsuarioData>();
builder.Services.AddScoped<PoliclinicoWeb.Data.ICitaData, PoliclinicoWeb.Data.CitaData>();
builder.Services.AddScoped<PoliclinicoWeb.Data.IHorarioData, PoliclinicoWeb.Data.HorarioData>();
builder.Services.AddScoped<PoliclinicoWeb.Data.IReporteData, PoliclinicoWeb.Data.ReporteData>();

// Registrar ApplicationDbContext con la connection string de appsettings
var cs = builder.Configuration.GetConnectionString("PoliclinicoDb");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(cs)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Anti-cache headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
    context.Response.Headers.Append("Pragma", "no-cache");
    context.Response.Headers.Append("Expires", "0");
    await next();
});

app.UseRouting();

// Habilitar sesiones
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
