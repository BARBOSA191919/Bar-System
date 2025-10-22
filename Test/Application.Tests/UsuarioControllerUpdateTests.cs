using Xunit;
using BarSystem.Api.Controllers;
using BarSystem.Domain.Entities;
using BarSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Application.Tests
{
    public class UsuarioControllerUpdateTests
    {
        private BarContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<BarContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_Update")
                .Options;
            return new BarContext(options);
        }

        [Fact]
        public async Task ActualizarUsuario_DeberiaModificarNombreYGuardarCambios()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new UsuariosController(context);

            // Crear un usuario inicial
            var usuario = new Usuario
            {
                Nombre = "Juan",
                Email = "juan@test.com",
                PasswordHash = "1234"
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            // Act: actualizar el nombre del usuario
            usuario.Nombre = "Juan Actualizado";
            context.Usuarios.Update(usuario);
            await context.SaveChangesAsync();

            // Assert
            var usuarioGuardado = await context.Usuarios.FirstAsync(u => u.Email == "juan@test.com");
            Assert.Equal("Juan Actualizado", usuarioGuardado.Nombre);
        }

        [Fact]
        public async Task EliminarUsuario_DeberiaRemoverloDeLaBaseDeDatos()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new UsuariosController(context);

            var usuario = new Usuario
            {
                Nombre = "Carlos",
                Email = "carlos@test.com",
                PasswordHash = "abcd"
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            // Act: eliminar usuario
            context.Usuarios.Remove(usuario);
            await context.SaveChangesAsync();

            // Assert
            var existe = await context.Usuarios.AnyAsync(u => u.Email == "carlos@test.com");
            Assert.False(existe); // Ya no debería existir
        }
    }
}
