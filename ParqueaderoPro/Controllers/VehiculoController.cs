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