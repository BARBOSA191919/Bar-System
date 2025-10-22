using Microsoft.AspNetCore.Mvc;
using BarSystem.Infrastructure;
using BarSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MesasController : ControllerBase
{
    private readonly BarContext _context;

    public MesasController(BarContext context)
    {
        _context = context;
    }

    [HttpGet]

    public async Task<ActionResult <IEnumerable<Mesa>>> GetMesas()

    {
       return await _context.Mesas.ToListAsync();
   
    }

    [HttpPost]
    public async Task<ActionResult<Mesa>> CreateMesa(Mesa mesa)
    {
        _context.Mesas.Add(mesa);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMesas), new { id = mesa.Id }, mesa);
    }
}

