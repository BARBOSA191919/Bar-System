using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarSystem.Infrastructure;
using BarSystem.Domain.Entities;

namespace BarSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetallePedidoController : ControllerBase
{
    private readonly IBarContext _context;

    public DetallePedidoController(IBarContext context)
    {
        _context = context;
    }

    // GET: api/DetallePedido
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetallePedido>>> GetDetalles()
    {
        var detalles = await _context.DetallesPedido
            .Include(d => d.Pedido)
            .Include(d => d.Producto)
            .ToListAsync();
        return Ok(detalles);
    }

    // GET: api/DetallePedido/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DetallePedido>> GetDetalle(int id)
    {
        var detalle = await _context.DetallesPedido
            .Include(d => d.Pedido)
            .Include(d => d.Producto)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (detalle == null)
            return NotFound();

        return Ok(detalle);
    }

    // POST: api/DetallePedido
    [HttpPost]
    public async Task<ActionResult<DetallePedido>> CreateDetalle(DetallePedido detalle)
    {
        _context.DetallesPedido.Add(detalle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDetalle), new { id = detalle.Id }, detalle);
    }

    // PUT: api/DetallePedido/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDetalle(int id, DetallePedido detalle)
    {
        if (id != detalle.Id)
            return BadRequest();

        var existe = await _context.DetallesPedido.AnyAsync(d => d.Id == id);
        if (!existe)
            return NotFound();

        _context.Entry(detalle).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/DetallePedido/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetalle(int id)
    {
        var detalle = await _context.DetallesPedido.FindAsync(id);
        if (detalle == null)
            return NotFound();

        _context.DetallesPedido.Remove(detalle);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
