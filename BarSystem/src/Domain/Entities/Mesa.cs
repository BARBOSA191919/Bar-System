namespace BarSystem.Domain.Entities;

public class Mesa
{
    public int Id { get; set; }
    public string Estado { get; set; } = "Libre";
    public int Capacidad { get; set; }

    // Relaci�n con Pedido
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
