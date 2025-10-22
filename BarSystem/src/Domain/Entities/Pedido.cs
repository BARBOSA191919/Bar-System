namespace BarSystem.Domain.Entities;

public class Pedido
{
    public int Id{ get; set; }
    public int MesaId{ get; set; }
    public Mesa ? Mesa{ get; set; }
    public DateTime Fecha{ get; set; } = DateTime.Now;
    public string Estado{ get; set; } = "En preparación";
    public decimal Total{ get; set; }

    public ICollection<DetallePedido> Detalles{ get; set; } = new List<DetallePedido>();
}
