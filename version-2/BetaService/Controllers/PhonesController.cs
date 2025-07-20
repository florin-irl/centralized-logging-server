using BetaService.Data;
using BetaService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

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

        // ✅ Helper method to log request and response
        private void LogRequestAndResponse(string method, string url, object? requestBody, int statusCode, object? responseData)
        {
            Log.ForContext("Method", method)
               .ForContext("Url", url)
               .ForContext("RequestBody", requestBody != null ? JsonSerializer.Serialize(requestBody) : "None")
               .ForContext("StatusCode", statusCode)
               .ForContext("ResponseBody", responseData != null ? JsonSerializer.Serialize(responseData) : "None")
               .Information("HTTP {Method} | URL: {Url} | Status: {StatusCode}");
        }

        private void LogError(string method, string url, object? requestBody, Exception ex)
        {
            Log.ForContext("Method", method)
               .ForContext("Url", url)
               .ForContext("RequestBody", requestBody != null ? JsonSerializer.Serialize(requestBody) : "None")
               .Error(ex, "❌ ERROR during {Method} {Url}");
        }



        // GET: api/phones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Phone>>> GetPhones()
        {
            string url = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            try
            {
                var phones = await _context.Phones.ToListAsync();
                LogRequestAndResponse("GET", url, null, 200, phones);
                return Ok(phones);
            }
            catch (Exception ex)
            {
                LogError("GET", url, null, ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // GET: api/phones/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Phone>> GetPhone(int id)
        {
            string url = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            try
            {
                var phone = await _context.Phones.FindAsync(id);
                if (phone == null)
                {
                    LogRequestAndResponse("GET", url, null, 404, null);
                    return NotFound();
                }

                LogRequestAndResponse("GET", url, null, 200, phone);
                return Ok(phone);
            }
            catch (Exception ex)
            {
                LogError("GET", url, null, ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // POST: api/phones
        [HttpPost]
        public async Task<ActionResult<Phone>> AddPhone([FromBody] Phone phone)
        {
            string url = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            try
            {
                _context.Phones.Add(phone);
                await _context.SaveChangesAsync();

                LogRequestAndResponse("POST", url, phone, 201, phone);
                return CreatedAtAction(nameof(GetPhone), new { id = phone.PhoneId }, phone);
            }
            catch (Exception ex)
            {
                LogError("POST", url, phone, ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // PUT: api/phones/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhone(int id, [FromBody] Phone phone)
        {
            string url = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            try
            {
                if (id != phone.PhoneId)
                {
                    LogRequestAndResponse("PUT", url, phone, 400, null);
                    return BadRequest();
                }

                _context.Entry(phone).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                LogRequestAndResponse("PUT", url, phone, 204, null);
                return NoContent();
            }
            catch (Exception ex)
            {
                LogError("PUT", url, phone, ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // DELETE: api/phones/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhone(int id)
        {
            string url = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            try
            {
                var phone = await _context.Phones.FindAsync(id);
                if (phone == null)
                {
                    LogRequestAndResponse("DELETE", url, null, 404, null);
                    return NotFound();
                }

                _context.Phones.Remove(phone);
                await _context.SaveChangesAsync();

                LogRequestAndResponse("DELETE", url, null, 204, null);
                return NoContent();
            }
            catch (Exception ex)
            {
                LogError("DELETE", url, null, ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
