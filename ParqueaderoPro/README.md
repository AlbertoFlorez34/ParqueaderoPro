ParqueaderoPro - Sistema de Gestión de Estacionamiento

Mini sistema desarrollado en **ASP.NET Core MVC** para el control de entrada y salida de vehículos con cálculo automático de cobro por tiempo.

## ✨ Funcionalidades
- **Registro de Entrada:** Captura de placa y tipo de vehículo (Carro/Moto).
- **Control de Tiempo:** Registro exacto de la hora de ingreso mediante `DateTime.Now`.
- **Cálculo de Cobro:** Lógica automatizada que calcula el valor a pagar basado en los minutos transcurridos (Tarifa: $50/min).
- **Panel de Control:** Interfaz con estados visuales (En Parqueadero / Salió).

## 🛠️ Tecnologías Utilizadas
- **Backend:** .NET 8 / C#
- **Frontend:** ASP.NET Core MVC (Razor Pages)
- **Base de Datos:** SQL Server Express / LocalDB
- **ORM:** Entity Framework Core
- **Estilos:** Bootstrap 5


2. instalar EntityFrameworkCore
EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.tools

3. crear archivo Producto.cs en carpeta Models
El Modelo (Models/Producto.cs)
Aquí definimos qué datos tiene un medicamento:

namespace ParqueaderoPro.Models;

public class Vehiculo
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Tipo { get; set; } // Carro o Moto
    public DateTime FechaEntrada { get; set; } = DateTime.Now;
    public DateTime? FechaSalida { get; set; } // Puede ser null si no ha salido
    public decimal TotalPagar { get; set; }
}

4. el contextocrear el contexto

El Contexto (Data/ParqueaderoPro.cs)


using Microsoft.EntityFrameworkCore;
using ParqueaderoPro.Models; // Asegúrate que tu modelo se llame así

namespace ParqueaderoPro.Data;

public class ParqueaderoContext : DbContext
{
    public ParqueaderoContext(DbContextOptions<ParqueaderoContext> options) : base(options) { }

    // Esta línea crea la tabla "Vehiculos" en la base de datos
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
}

5. crear el controlador
El Controlador (Controllers/InventarioController.cs)
Este es el cerebro que conecta la base de datos con la pantalla:



using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParqueaderoPro.Data;
using ParqueaderoPro.Models;

namespace ParqueaderoPro.Controllers
{
   
    public class VehiculoController : Controller
    {
        private readonly ParqueaderoContext _context;

        public VehiculoController(ParqueaderoContext context)
        {
            _context = context;
        }

        // 2. Acción para ver la lista de carros
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Vehiculos.ToListAsync();
            return View(lista);
        }

        // 3. Acción para la pantalla de registro
        public IActionResult Crear()
        {
            return View();
        }

        // 4. Acción para guardar el carro que entra
        [HttpPost]
        public async Task<IActionResult> Crear(Vehiculo vehiculo)
        {
            _context.Add(vehiculo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 5. Acción para cobrar y dar salida
        [HttpPost]
        public async Task<IActionResult> Salida(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo != null)
            {
                vehiculo.FechaSalida = DateTime.Now;

                // Cálculo de dinero: 50 pesos por minuto
                var minutos = (vehiculo.FechaSalida.Value - vehiculo.FechaEntrada).TotalMinutes;
                vehiculo.TotalPagar = (decimal)minutos * 50;

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    } 
} 

6. crear la vista 

crear cerpte Inventario dentro de Views, crear archivo Index.cshtml en la carpeta Inventario

La Vista (Views/Vehiculo/Index.cshtml)
Para que se vea como un sistema real, usamos una tabla de HTML:

@model IEnumerable<ParqueaderoPro.Models.Vehiculo>

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h2 class="text-primary">🚗 Control de Parqueadero</h2>
        <a asp-action="Crear" class="btn btn-success font-weight-bold">+ REGISTRAR ENTRADA</a>
    </div>

    <div class="card shadow">
        <div class="card-body">
            <table class="table table-hover">
                <thead class="table-dark">
                    <tr>
                        <th>Placa</th>
                        <th>Tipo</th>
                        <th>Fecha Entrada</th>
                        <th>Estado / Total</th>
                        <th>Acción</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var item in Model)
                    {
                        <tr>
                            <td class="fw-bold">@item.Placa.ToUpper()</td>
                            <td>
                                <span class="badge @(item.Tipo == "Carro" ? "bg-info" : "bg-secondary")">
                                    @item.Tipo
                                </span>
                            </td>
                            <td>@item.FechaEntrada.ToString("dd/MM/yyyy HH:mm")</td>
                            <td>
                                @if (item.FechaSalida == null)
                                {
                                    <span class="text-warning font-weight-bold">● En Parqueadero</span>
                                }
                                else
                                {
                                    <span class="text-success">
                                        Salió (@item.TotalPagar.ToString("C0"))
                                    </span>
                                }
                            </td>
                            <td>
                                @if (item.FechaSalida == null)
                                {
                                    <form asp-action="Salida" asp-route-id="@item.Id" method="post">
                                        <button type="submit" class="btn btn-danger btn-sm shadow-sm"
                                                onclick="return confirm('¿Confirmar salida y cobro?')">
                                            Cobrar y Salir
                                        </button>
                                    </form>
                                }
                                else
                                {
                                    <button class="btn btn-outline-secondary btn-sm" disabled>Completado</button>
                                }
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    </div>
</div>


9. cadena de conexion 

{

  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ParqueaderoDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}


8. program.cs
using Microsoft.EntityFrameworkCore;
using ParqueaderoPro.Data; // Asegúrate de tener esta carpeta creada

var builder = WebApplication.CreateBuilder(args);

// AGREGA servicio:
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ParqueaderoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

//  RUTA POR DEFECTO:
app.MapControllerRoute(
    name: "default",
  
    pattern: "{controller=Vehiculo}/{action=Index}/{id?}");

app.Run();


9.Crea la base de datos

Crea la base de datos: Abre la Consola de Administrador de Paquetes y corre:

Add-Migration InicialParqueadero

Update-Database


10.Crea la Vista "Crear"
Crea la Vista "Crear": Falta el archivo donde escribes el nombre del remedio. Crea un archivo en Views/Inventario/Crear.cshtml y pega esto:

@model ParqueaderoPro.Models.Vehiculo

<div class="container mt-4">
    <h2>🚗 Registrar Entrada de Vehículo</h2>
    <hr />
    <form asp-action="Crear">
        <div class="mb-3">
            <label class="form-label">Placa del Vehículo</label>
            <input asp-for="Placa" class="form-control" placeholder="Ej: ABC-123" required />
        </div>
        <div class="mb-3">
            <label class="form-label">Tipo de Vehículo</label>
            <select asp-for="Tipo" class="form-select">
                <option value="Carro">Carro</option>
                <option value="Moto">Moto</option>
            </select>
        </div>
        <button type="submit" class="btn btn-primary">Registrar Entrada</button>
        <a asp-action="Index" class="btn btn-secondary">Cancelar</a>
    </form>
</div>

11. start debugging f5