using Microsoft.EntityFrameworkCore;
using ParqueaderoPro.Models; // Asegúrate que tu modelo se llame así

namespace ParqueaderoPro.Data;

public class ParqueaderoContext : DbContext
{
    public ParqueaderoContext(DbContextOptions<ParqueaderoContext> options) : base(options) { }

    // Esta línea crea la tabla "Vehiculos" en la base de datos
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
}