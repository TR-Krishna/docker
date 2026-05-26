using ApiMeter.Data;
using ApiMeter.DTOs;
using ApiMeter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMeter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MeterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MeterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var meters = await _context.Meters.ToListAsync();
            return Ok(meters);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var meter = await _context.Meters.FindAsync(id);
            if (meter == null) return NotFound();
            return Ok(meter);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMeterDto dto)
        {
            var meter = new Meter
            {
                Name = dto.Name,
                Description = dto.Description,
                Quantity = dto.Quantity,
                Price = dto.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Meters.Add(meter);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = meter.Id }, meter);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMeterDto dto)
        {
            var meter = await _context.Meters.FindAsync(id);
            if (meter == null) return NotFound();

            meter.Name = dto.Name;
            meter.Description = dto.Description;
            meter.Quantity = dto.Quantity;
            meter.Price = dto.Price;
            meter.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(meter);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var meter = await _context.Meters.FindAsync(id);
            if (meter == null) return NotFound();

            _context.Meters.Remove(meter);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}