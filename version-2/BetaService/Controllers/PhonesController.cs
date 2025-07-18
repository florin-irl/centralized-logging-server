using BetaService.Data;
using BetaService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhonesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PhonesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/phones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Phone>>> GetPhones()
        {
            return await _context.Phones.ToListAsync();
        }

        // GET: api/phones/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Phone>> GetPhone(int id)
        {
            var phone = await _context.Phones.FindAsync(id);
            if (phone == null)
                return NotFound();
            return phone;
        }

        // POST: api/phones
        [HttpPost]
        public async Task<ActionResult<Phone>> AddPhone(Phone phone)
        {
            _context.Phones.Add(phone);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPhone), new { id = phone.PhoneId }, phone);
        }

        // PUT: api/phones/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhone(int id, Phone phone)
        {
            if (id != phone.PhoneId)
                return BadRequest();

            _context.Entry(phone).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/phones/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhone(int id)
        {
            var phone = await _context.Phones.FindAsync(id);
            if (phone == null)
                return NotFound();

            _context.Phones.Remove(phone);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
