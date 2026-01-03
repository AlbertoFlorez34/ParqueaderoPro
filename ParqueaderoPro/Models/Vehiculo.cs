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