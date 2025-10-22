using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarSystem.Infrastructure;
using BarSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuentasController : ControllerBase
    {
        private readonly BarContext _context;

        public CuentasController(BarContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cuenta>>> GetCuentas()
        {
            var cuentas = await _context.Cuentas.ToListAsync();
            return Ok(cuentas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cuenta>> GetCuenta(int id)
        {
            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta == null)
                return NotFound();

            return Ok(cuenta);
        }

        [HttpPost]
        public async Task<ActionResult<Cuenta>> CreateCuenta(Cuenta cuenta)
        {
            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCuenta), new { id = cuenta.Id }, cuenta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCuenta(int id, Cuenta cuenta)
        {
            if (id != cuenta.Id)
                return BadRequest();

            var cuentaExistente = await _context.Cuentas.FindAsync(id);
            if (cuentaExistente == null)
                return NotFound();

            cuentaExistente.Total = cuenta.Total;
            cuentaExistente.MetodoPago = cuenta.MetodoPago;
            cuentaExistente.Estado = cuenta.Estado;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCuenta(int id)
        {
            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta == null)
                return NotFound();

            _context.Cuentas.Remove(cuenta);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
