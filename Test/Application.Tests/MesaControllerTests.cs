using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BarSystem.Infrastructure;
using BarSystem.Domain.Entities;
using BarSystem.Api.Controllers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq; // 👈 necesario para usar Count()

namespace Application.Tests
{
    public class MesasControllerTests
    {
        [Fact]
        public async Task GetMesas_DebeRetornarListaDeMesas()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new BarContext(options);
            context.Mesas.AddRange(
                new Mesa { Estado = "Libre", Capacidad = 4 },
                new Mesa { Estado = "Ocupada", Capacidad = 2 }
            );
            await context.SaveChangesAsync();

            var controller = new MesasController(context);

            // Act
            var result = await controller.GetMesas();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Mesa>>>(result);
            var mesas = Assert.IsAssignableFrom<IEnumerable<Mesa>>(actionResult.Value);
            Assert.Equal(2, mesas.Count()); // ✅ ahora sí compila y ejecuta correctamente
        }

        [Fact]
        public async Task CreateMesa_DebeAgregarMesaYRetornarCreatedAtAction()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new BarContext(options);
            var controller = new MesasController(context);

            var nuevaMesa = new Mesa { Estado = "Libre", Capacidad = 6 };

            // Act
            var result = await controller.CreateMesa(nuevaMesa);

            // Assert
            var createdAtAction = Assert.IsType<CreatedAtActionResult>(result.Result);
            var mesa = Assert.IsType<Mesa>(createdAtAction.Value);
            Assert.Equal("Libre", mesa.Estado);

            // Verificamos que se guardó en la BD
            Assert.Single(context.Mesas);
        }
    }
}
