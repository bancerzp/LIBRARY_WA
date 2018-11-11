using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;

namespace LIBRARY_WA.Data
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private readonly ResourceContext _context;

        public ResourcesController(ResourceContext context)
        {
            _context = context;
        }

        // GET: api/Resources
        [HttpGet]
        public IEnumerable<Resource> GetResource([FromBody] Resource resource)
        {
            return _context.Resource;
        }

        // GET: api/Resources/5
        [HttpGet("{id}")]
        public  IEnumerable<Resource> GetResource([FromRoute] int id)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //var resource = await _context.Resource.FindAsync(id);

            //if (resource == null)
            //{
            //    return NotFound();
            //}

            //return Ok(resource);
            return _context.Resource;
        }

        // PUT: api/Resources/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResource([FromRoute] int id, [FromBody] Resource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != resource.Id)
            {
                return BadRequest();
            }

            _context.Entry(resource).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResourceExists(id))
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

        // POST: api/Resources
        [HttpPost]
        public async Task<IActionResult> PostResource([FromBody] Resource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Resource.Add(resource);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResource", new { id = resource.Id }, resource);
        }

        // DELETE: api/Resources/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resource = await _context.Resource.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            _context.Resource.Remove(resource);
            await _context.SaveChangesAsync();

            return Ok(resource);
        }

        private bool ResourceExists(int id)
        {
            return _context.Resource.Any(e => e.Id == id);
        }
    }
}