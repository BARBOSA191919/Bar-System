namespace BarSystem.Domain.Entities;

public class Mesa
{
    public int Id { get; set; }
    public string Estado { get; set; } = "Libre";
    public int Capacidad { get; set; }

    // Relación con Pedido
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
