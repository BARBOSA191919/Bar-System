using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarSystem.Infrastructure;
using BarSystem.Domain.Entities;

namespace BarSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IBarContext _context;

    public PedidosController(IBarContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Detalles)
            .Include(p => p.Mesa)
            .ToListAsync();
        return Ok(pedidos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pedido>> GetPedido(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Detalles)
            .Include(p => p.Mesa)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
            return NotFound();

        return Ok(pedido);
    }

    [HttpPost]
    public async Task<ActionResult<Pedido>> CreatePedido(Pedido pedido)
    {
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePedido(int id, Pedido pedido)
    {
        if (id != pedido.Id)
            return BadRequest();

        var existe = await _context.Pedidos.AnyAsync(p => p.Id == id);
        if (!existe)
            return NotFound();

        _context.Entry(pedido).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePedido(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null)
            return NotFound();

        _context.Pedidos.Remove(pedido);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}