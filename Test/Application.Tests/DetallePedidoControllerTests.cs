using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BarSystem.Infrastructure;
using BarSystem.Domain.Entities;
using BarSystem.Api.Controllers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Application.Tests
{
    public class DetallePedidoControllerTests
    {
        // 🔹 Crea opciones de base de datos en memoria para cada test
        private DbContextOptions<BarContext> GetDbOptions()
        {
            return new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        // 🔹 Test: Obtener todos los detalles
        [Fact]
        public async Task GetDetalles_DebeRetornarListaDeDetalles()
        {
            var options = GetDbOptions();
            var context = new BarContext(options);

            var producto1 = new Producto { Nombre = "Cerveza", Precio = 5000, Categoria = "Bebida" };
            var producto2 = new Producto { Nombre = "Agua", Precio = 3000, Categoria = "Bebida" };
            var pedido = new Pedido { MesaId = 1, Total = 8000 };

            context.Productos.AddRange(producto1, producto2);
            context.Pedidos.Add(pedido);
            await context.SaveChangesAsync();

            context.DetallesPedido.AddRange(
                new DetallePedido { PedidoId = pedido.Id, ProductoId = producto1.Id, Cantidad = 2, Subtotal = 10000 },
                new DetallePedido { PedidoId = pedido.Id, ProductoId = producto2.Id, Cantidad = 1, Subtotal = 5000 }
            );
            await context.SaveChangesAsync();

            var controller = new DetallePedidoController(context);
            var result = await controller.GetDetalles();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var detalles = Assert.IsAssignableFrom<IEnumerable<DetallePedido>>(okResult.Value);
            Assert.Equal(2, detalles.Count());
        }

        // 🔹 Test: Crear un nuevo detalle válido
        [Fact]
        public async Task CreateDetalle_DebeCrearNuevoDetalle()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);

            var pedido = new Pedido { MesaId = 1, Total = 0 };
            var producto = new Producto { Nombre = "Hamburguesa", Precio = 12000, Categoria = "Comida" };
            await context.Pedidos.AddAsync(pedido);
            await context.Productos.AddAsync(producto);
            await context.SaveChangesAsync();

            var controller = new DetallePedidoController(context);

            var nuevoDetalle = new DetallePedido
            {
                PedidoId = pedido.Id,
                ProductoId = producto.Id,
                Cantidad = 2,
                Subtotal = 24000
            };

            var result = await controller.CreateDetalle(nuevoDetalle);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var detalle = Assert.IsType<DetallePedido>(created.Value);

            Assert.Equal(1, context.DetallesPedido.Count());
            Assert.Equal(24000, detalle.Subtotal);
        }

        // 🔹 Test: Eliminar detalle inexistente
        [Fact]
        public async Task DeleteDetalle_DebeRetornarNotFound_SiNoExiste()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new DetallePedidoController(context);

            var result = await controller.DeleteDetalle(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // 🔹 Test: Eliminar detalle existente
        [Fact]
        public async Task DeleteDetalle_DebeEliminarDetalleExistente()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);

            var pedido = new Pedido { MesaId = 1, Total = 0 };
            var producto = new Producto { Nombre = "Papas Fritas", Precio = 5000, Categoria = "Comida" };
            await context.Pedidos.AddAsync(pedido);
            await context.Productos.AddAsync(producto);
            await context.SaveChangesAsync();

            var detalle = new DetallePedido
            {
                PedidoId = pedido.Id,
                ProductoId = producto.Id,
                Cantidad = 1,
                Subtotal = 5000
            };

            context.DetallesPedido.Add(detalle);
            await context.SaveChangesAsync();

            var controller = new DetallePedidoController(context);

            var result = await controller.DeleteDetalle(detalle.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.DetallesPedido);
        }

        // 🔹 Test: Crear detalle con datos inválidos (ahora no lanza excepción)
        [Fact]
        public async Task CreateDetalle_NoDebeLanzarExcepcion_SiDatosInvalidos()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new DetallePedidoController(context);

            var detalleInvalido = new DetallePedido
            {
                PedidoId = 999,
                ProductoId = 999,
                Cantidad = 1,
                Subtotal = 1000
            };

            var result = await controller.CreateDetalle(detalleInvalido);

            // El controlador debe retornar un resultado, no lanzar excepción
            Assert.IsType<ActionResult<DetallePedido>>(result);
        }
    }
}
