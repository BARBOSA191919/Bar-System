using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BarSystem.Api.Controllers;
using BarSystem.Infrastructure;
using BarSystem.Domain.Entities;
using BarSystem.Application.DTOs;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace Application.Tests
{
    public class UsuariosControllerTests
    {
        private BarContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BarContext(options);
        }

        [Fact]
        public async Task Register_DeberiaRegistrarUsuarioCorrectamente()
        {
            var context = GetInMemoryContext();
            var controller = new UsuariosController(context);

            var dto = new UsuarioRegisterDto
            {
                Nombre = "Santiago",
                Email = "santi@test.com",
                Password = "12345"
            };

            var result = await controller.Register(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Usuario registrado con éxito.", okResult.Value);

            // Verificar que el usuario registrado tiene el Rol por defecto
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            Assert.NotNull(usuario);
            Assert.Equal("User", usuario.Rol); // Cubre la línea de Rol
        }

        [Fact]
        public async Task Register_DeberiaFallarSiElUsuarioYaExiste()
        {
            var context = GetInMemoryContext();
            context.Usuarios.Add(new Usuario
            {
                Nombre = "Santiago",
                Email = "santi@test.com",
                PasswordHash = "hash"
            });
            await context.SaveChangesAsync();

            var controller = new UsuariosController(context);
            var dto = new UsuarioRegisterDto
            {
                Nombre = "Santiago",
                Email = "santi@test.com",
                Password = "12345"
            };

            var result = await controller.Register(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El usuario ya existe.", badRequest.Value);
        }

        [Fact]
        public async Task Login_DeberiaPermitirIngresoCorrecto()
        {
            var context = GetInMemoryContext();
            var controller = new UsuariosController(context);

            var usuario = new Usuario
            {
                Nombre = "Santiago",
                Email = "santi@test.com",
                PasswordHash = Convert.ToBase64String(
                    SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("12345")))
            };
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var dto = new UsuarioLoginDto { Email = "santi@test.com", Password = "12345" };

            var result = await controller.Login(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Bienvenido", okResult.Value.ToString());
        }

        [Fact]
        public async Task Login_DeberiaFallarConContrasenaIncorrecta()
        {
            var context = GetInMemoryContext();
            var controller = new UsuariosController(context);

            var usuario = new Usuario
            {
                Nombre = "Santiago",
                Email = "santi@test.com",
                PasswordHash = Convert.ToBase64String(
                    SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("12345")))
            };
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var dto = new UsuarioLoginDto { Email = "santi@test.com", Password = "wrong" };

            var result = await controller.Login(dto);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Contraseña incorrecta.", unauthorized.Value);
        }

        [Fact]
        public async Task Login_DeberiaFallarSiUsuarioNoExiste()
        {
            var context = GetInMemoryContext();
            var controller = new UsuariosController(context);

            var dto = new UsuarioLoginDto { Email = "noexiste@test.com", Password = "12345" };

            var result = await controller.Login(dto);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Usuario no encontrado.", unauthorized.Value);
        }
        [Fact]
        public async Task Register_DebeDevolverBadRequest_SiUsuarioYaExiste()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new BarContext(options);
            context.Usuarios.Add(new Usuario
            {
                Nombre = "Juan",
                Email = "juan@test.com",
                PasswordHash = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            });
            await context.SaveChangesAsync();

            var controller = new UsuariosController(context);

            var dto = new UsuarioRegisterDto
            {
                Nombre = "Juan",
                Email = "juan@test.com",
                Password = "1234"
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El usuario ya existe.", badRequest.Value);
        }

        [Fact]
        public async Task Login_DebeRetornarOk_CuandoCredencialesSonCorrectas()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new BarContext(options);

            var password = "1234";
            var controllerTemp = new UsuariosController(context);
            var hash = controllerTemp.GetType()
                .GetMethod("HashPassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(controllerTemp, new object[] { password })!.ToString();

            context.Usuarios.Add(new Usuario
            {
                Nombre = "Maria",
                Email = "maria@test.com",
                PasswordHash = hash
            });
            await context.SaveChangesAsync();

            var controller = new UsuariosController(context);
            var dto = new UsuarioLoginDto { Email = "maria@test.com", Password = "1234" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Bienvenido", okResult.Value!.ToString());
        }

        [Fact]
        public async Task Login_DebeRetornarUnauthorized_CuandoPasswordIncorrecta()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new BarContext(options);
            var controllerTemp = new UsuariosController(context);
            var hash = controllerTemp.GetType()
                .GetMethod("HashPassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(controllerTemp, new object[] { "1234" })!.ToString();

            context.Usuarios.Add(new Usuario
            {
                Nombre = "Pedro",
                Email = "pedro@test.com",
                PasswordHash = hash
            });
            await context.SaveChangesAsync();

            var controller = new UsuariosController(context);
            var dto = new UsuarioLoginDto { Email = "pedro@test.com", Password = "9999" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Contraseña incorrecta.", unauthorized.Value);
        }
        [Fact]
        public async Task Register_DebeRetornarBadRequest_SiEmailYaExiste()
        {
            var options = new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new BarContext(options);
            context.Usuarios.Add(new Usuario { Nombre = "Juan", Email = "juan@test.com", PasswordHash = "123" });
            await context.SaveChangesAsync();

            var controller = new UsuariosController(context);

            var dto = new UsuarioRegisterDto
            {
                Nombre = "Pedro",
                Email = "juan@test.com",
                Password = "123"
            };

            var result = await controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_DebeRetornarUnauthorized_SiPasswordIncorrecta()
        {
            var options = new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new BarContext(options);
            var usuario = new Usuario { Nombre = "Ana", Email = "ana@test.com", PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("1234")) };
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var controller = new UsuariosController(context);

            var dto = new UsuarioLoginDto
            {
                Email = "ana@test.com",
                Password = "incorrecta"
            };

            var result = await controller.Login(dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }


    }
}
