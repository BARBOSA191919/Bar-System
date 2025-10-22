using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BarSystem.Domain.Entities; 
using System.Threading;
using System.Threading.Tasks;

namespace BarSystem.Infrastructure
{
	public interface IBarContext
	{
		DbSet<Mesa> Mesas { get; set; }
		DbSet<Pedido> Pedidos { get; set; }
		DbSet<DetallePedido> DetallesPedido { get; set; }
		DbSet<Producto> Productos { get; set; }
		DbSet<Cuenta> Cuentas { get; set; }
		DbSet<Usuario> Usuarios { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		EntityEntry Entry(object entity);
	}
}
