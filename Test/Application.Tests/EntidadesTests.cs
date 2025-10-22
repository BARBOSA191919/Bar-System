using Xunit;
using BarSystem.Domain.Entities;

namespace Application.Tests
{
    public class EntidadesTests
    {
        [Fact]
        public void Cuenta_DebeInicializarConValoresPredeterminados()
        {
            var cuenta = new Cuenta();
            Assert.Equal("Pendiente", cuenta.MetodoPago);
            Assert.Equal("Pendiente", cuenta.Estado);
        }

        [Fact]
        public void DetallePedido_DebeCalcularSubtotalCorrectamente()
        {
            var detalle = new DetallePedido
            {
                Cantidad = 3,
                Producto = new Producto { Precio = 2000 }
            };

            detalle.Subtotal = detalle.Cantidad * detalle.Producto.Precio;

            Assert.Equal(6000, detalle.Subtotal);
        }

        [Fact]
        public void Mesa_DebeTenerEstadoPorDefectoLibre()
        {
            var mesa = new Mesa();
            Assert.Equal("Libre", mesa.Estado);
            Assert.Empty(mesa.Pedidos);
        }

        [Fact]
        public void Pedido_DebeTenerEstadoEnPreparacionPorDefecto()
        {
            var pedido = new Pedido();
            Assert.Equal("En preparación", pedido.Estado);
            Assert.NotNull(pedido.Detalles);
        }

        [Fact]
        public void Producto_DebeAsignarValoresCorrectos()
        {
            var producto = new Producto
            {
                Nombre = "Cerveza",
                Precio = 5000,
                Categoria = "Bebida"
            };

            Assert.Equal("Cerveza", producto.Nombre);
            Assert.Equal(5000, producto.Precio);
            Assert.Equal("Bebida", producto.Categoria);
        }
    }
}
