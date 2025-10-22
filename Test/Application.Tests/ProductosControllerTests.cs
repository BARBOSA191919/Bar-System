using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarSystem.Infrastructure;
using BarSystem.Domain.Entities;
using BarSystem.Api.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Application.Tests
{
    public class ProductosControllerTests
    {
        private DbContextOptions<BarContext> GetDbOptions()
        {
            return new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetProductos_DebeRetornarListaDeProductos()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);

            context.Productos.AddRange(
                new Producto { Nombre = "Cerveza", Precio = 5000 },
                new Producto { Nombre = "Gaseosa", Precio = 3000 }
            );
            await context.SaveChangesAsync();

            var controller = new ProductosController(context);
            var result = await controller.GetProductos();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var productos = Assert.IsAssignableFrom<IEnumerable<Producto>>(okResult.Value);
            Assert.Equal(2, productos.Count());
        }

        [Fact]
        public async Task CreateProducto_DebeCrearProducto()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new ProductosController(context);

            var nuevoProducto = new Producto { Nombre = "Whisky", Precio = 20000 };
            var result = await controller.CreateProducto(nuevoProducto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var producto = Assert.IsType<Producto>(created.Value);
            Assert.Equal("Whisky", producto.Nombre);
            Assert.Equal(1, context.Productos.Count());
        }

        [Fact]
        public async Task UpdateProducto_DebeActualizarProductoExistente()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var producto = new Producto { Nombre = "Ron", Precio = 10000 };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();

            var controller = new ProductosController(context);
            producto.Precio = 12000;

            var result = await controller.UpdateProducto(producto.Id, producto);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(12000, context.Productos.First().Precio);
        }

        [Fact]
        public async Task UpdateProducto_DebeRetornarNotFound_SiNoExiste()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new ProductosController(context);

            var producto = new Producto { Id = 99, Nombre = "Agua", Precio = 1000 };
            var result = await controller.UpdateProducto(99, producto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProducto_DebeEliminarProducto()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var producto = new Producto { Nombre = "Tequila", Precio = 8000 };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();

            var controller = new ProductosController(context);
            var result = await controller.DeleteProducto(producto.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Productos);
        }
        [Fact]
        public async Task GetProducto_DebeRetornarProducto_SiExiste()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var producto = new Producto { Nombre = "Cerveza", Precio = 5000 };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();

            var controller = new ProductosController(context);
            var result = await controller.GetProducto(producto.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var productoResult = Assert.IsType<Producto>(okResult.Value);
            Assert.Equal("Cerveza", productoResult.Nombre);
        }

        [Fact]
        public async Task GetProducto_DebeRetornarNotFound_SiNoExiste()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new ProductosController(context);

            var result = await controller.GetProducto(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateProducto_DebeRetornarBadRequest_SiIdsNoCoinciden()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var producto = new Producto { Id = 1, Nombre = "Vino", Precio = 7000 };

            var controller = new ProductosController(context);
            var result = await controller.UpdateProducto(2, producto);

            Assert.IsType<BadRequestResult>(result);
        }

    }
}
