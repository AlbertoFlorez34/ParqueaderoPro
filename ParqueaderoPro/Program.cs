using Microsoft.EntityFrameworkCore;
using ParqueaderoPro.Data; // Asegúrate de tener esta carpeta creada

var builder = WebApplication.CreateBuilder(args);

// AGREGA ESTO:
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ParqueaderoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// CONFIGURA LA RUTA POR DEFECTO:
app.MapControllerRoute(
    name: "default",
  
    pattern: "{controller=Vehiculo}/{action=Index}/{id?}");

app.Run();