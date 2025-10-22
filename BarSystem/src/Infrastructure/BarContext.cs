using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BarSystem.Domain.Entities;

namespace BarSystem.Infrastructure
{
    public class BarContext : DbContext, IBarContext
    {
        public BarContext(DbContextOptions<BarContext> options) : base(options) { }

        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public new EntityEntry Entry(object entity) => base.Entry(entity);
    }
}
