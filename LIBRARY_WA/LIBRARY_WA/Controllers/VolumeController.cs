using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;
using Microsoft.AspNetCore.Cors;

namespace LIBRARY_WA.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class VolumeController : ControllerBase
    {
        private readonly LibraryContext _context;

        public VolumeController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Volumes
        [HttpGet]
        public IEnumerable<Volume> GetVolume()
        {
            return _context.Volume;
        }


         [HttpPost("{id}")]
          public async Task<IActionResult> AddVolume([FromRoute] String id)
          {
              if (!ModelState.IsValid)
              {
                  return BadRequest(ModelState);
              }
              //  book.is_available = true;
              _context.Volume.Add(new Volume(null, Int32.Parse(id),true));
              await _context.SaveChangesAsync();

              return CreatedAtAction("AddVolume", new { id = id }, true);
          }

   






        // GET: api/Volumes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVolume([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var volume = await _context.Volume.FindAsync(id);

            if (volume == null)
            {
                return NotFound();
            }

            return Ok(volume);
        }

        // PUT: api/Volumes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVolume([FromRoute] int id, [FromBody] Volume volume)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != volume.volume_id)
            {
                return BadRequest();
            }

            _context.Entry(volume).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VolumeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Volumes
        [HttpPost]
        public async Task<IActionResult> PostVolume([FromBody] Volume volume)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Volume.Add(volume);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVolume", new { id = volume.volume_id }, volume);
        }

        // DELETE: api/Volumes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVolume([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var volume = await _context.Volume.FindAsync(id);
            if (volume == null)
            {
                return NotFound();
            }

            _context.Volume.Remove(volume);
            await _context.SaveChangesAsync();

            return Ok(volume);
        }

        private bool VolumeExists(int id)
        {
            return _context.Volume.Any(e => e.volume_id == id);
        }
    }
}