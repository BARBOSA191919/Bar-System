namespace BarSystem.Domain.Entities;

public class Cuenta
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }
    public decimal Total { get; set; }
    public string MetodoPago { get; set; } = "Pendiente"; // efectivo, QR, etc.
    public string Estado { get; set; } = "Pendiente"; // pagada / pendiente
}
