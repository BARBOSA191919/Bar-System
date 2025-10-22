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
    public class CuentasControllerTests
    {
        private DbContextOptions<BarContext> GetDbOptions()
        {
            return new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetCuentas_DebeRetornarListaDeCuentas()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);

            context.Cuentas.AddRange(
                new Cuenta { Total = 30000, MetodoPago = "Efectivo" },
                new Cuenta { Total = 15000, MetodoPago = "QR" }
            );
            await context.SaveChangesAsync();

            var controller = new CuentasController(context);
            var result = await controller.GetCuentas();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var cuentas = Assert.IsAssignableFrom<IEnumerable<Cuenta>>(okResult.Value);
            Assert.Equal(2, System.Linq.Enumerable.Count(cuentas));
        }

        [Fact]
        public async Task CreateCuenta_DebeCrearNuevaCuenta()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new CuentasController(context);

            var nuevaCuenta = new Cuenta { Total = 25000, MetodoPago = "Efectivo" };
            var result = await controller.CreateCuenta(nuevaCuenta);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var cuenta = Assert.IsType<Cuenta>(created.Value);

            Assert.Equal(1, context.Cuentas.Count());
            Assert.Equal(25000, cuenta.Total);
        }

        [Fact]
        public async Task DeleteCuenta_DebeEliminarCuenta()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);

            var cuenta = new Cuenta { Total = 15000, MetodoPago = "QR" };
            context.Cuentas.Add(cuenta);
            await context.SaveChangesAsync();

            var controller = new CuentasController(context);
            var result = await controller.DeleteCuenta(cuenta.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Cuentas);
        }
        [Fact]
        public async Task DeleteCuenta_DebeRetornarNotFound_SiNoExiste()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new CuentasController(context);

            // Act
            var result = await controller.DeleteCuenta(999); // ID inexistente

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCuenta_DebeRetornarCuenta_SiExiste()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var cuenta = new Cuenta { Total = 20000, MetodoPago = "Efectivo" };
            context.Cuentas.Add(cuenta);
            await context.SaveChangesAsync();

            var controller = new CuentasController(context);
            var result = await controller.GetCuenta(cuenta.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var cuentaResult = Assert.IsType<Cuenta>(okResult.Value);
            Assert.Equal("Efectivo", cuentaResult.MetodoPago);
        }

        [Fact]
        public async Task GetCuenta_DebeRetornarNotFound_SiNoExiste()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new CuentasController(context);

            var result = await controller.GetCuenta(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCuenta_DebeActualizarCuentaExistente()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var cuenta = new Cuenta { Total = 10000, MetodoPago = "QR" };
            context.Cuentas.Add(cuenta);
            await context.SaveChangesAsync();

            var controller = new CuentasController(context);
            cuenta.Total = 15000;
            cuenta.MetodoPago = "Efectivo";

            var result = await controller.UpdateCuenta(cuenta.Id, cuenta);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(15000, context.Cuentas.First().Total);
            Assert.Equal("Efectivo", context.Cuentas.First().MetodoPago);
        }

        [Fact]
        public async Task UpdateCuenta_DebeRetornarBadRequest_SiIdsNoCoinciden()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new CuentasController(context);

            var cuenta = new Cuenta { Id = 1, Total = 5000, MetodoPago = "Efectivo" };

            var result = await controller.UpdateCuenta(99, cuenta);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateCuenta_DebeRetornarNotFound_SiNoExiste()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new CuentasController(context);

            var cuenta = new Cuenta { Id = 5, Total = 20000, MetodoPago = "QR" };

            var result = await controller.UpdateCuenta(5, cuenta);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCuentas_DebeRetornarListaVacia_SiNoHayCuentas()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new CuentasController(context);

            var result = await controller.GetCuentas();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var cuentas = Assert.IsAssignableFrom<IEnumerable<Cuenta>>(okResult.Value);
            Assert.Empty(cuentas);
        }

        [Fact]
        public async Task CreateCuenta_DebeAsignarEstadoPorDefecto()
        {
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var controller = new CuentasController(context);

            var nuevaCuenta = new Cuenta { Total = 10000, MetodoPago = "QR" };
            var result = await controller.CreateCuenta(nuevaCuenta);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var cuenta = Assert.IsType<Cuenta>(created.Value);

            Assert.Equal("Pendiente", cuenta.Estado); // 🔹 Valor por defecto
        }

        [Fact]
        public async Task UpdateCuenta_DebeActualizarEstadoCorrectamente()
        {
            // Arrange
            var options = GetDbOptions();
            using var context = new BarContext(options);
            var cuenta = new Cuenta { Total = 15000, MetodoPago = "Efectivo", Estado = "Pendiente" };
            context.Cuentas.Add(cuenta);
            await context.SaveChangesAsync();

            var controller = new CuentasController(context);

            // Act
            var cuentaActualizada = new Cuenta
            {
                Id = cuenta.Id,
                Total = cuenta.Total,
                MetodoPago = cuenta.MetodoPago,
                Estado = "Pagada" 
            };

            var result = await controller.UpdateCuenta(cuenta.Id, cuentaActualizada);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var cuentaDesdeDb = await context.Cuentas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cuenta.Id);
            Assert.NotNull(cuentaDesdeDb);
            Assert.Equal("Pagada", cuentaDesdeDb.Estado); 
        }

    }

}
