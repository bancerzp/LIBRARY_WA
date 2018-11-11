using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;

namespace LIBRARY_WA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutController : ControllerBase
    {
        private readonly WorkoutContext _context;

        //konstruktor
        public WorkoutController(WorkoutContext context)
        {
            _context = context;
        }

        // GET: api/Workout
        [HttpGet("{id}{}{}{}{}")]
        public IEnumerable<Workout> GetWorkout()
        {
            return _context.Workout;
        }

        // GET: api/Workout/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkout([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workout = await _context.Workout.FindAsync(id);

            if (workout == null)
            {
                return NotFound();
            }

            return Ok(workout);
        }

        // PUT: api/Workout/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkout([FromRoute] int id, [FromBody] Workout workout)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != workout.Id)
            {
                return BadRequest();
            }

            _context.Entry(workout).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkoutExists(id))
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

        // POST: api/Workout
        [HttpPost]
        public async Task<IActionResult> PostWorkout([FromBody] Workout workout)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Workout.Add(workout);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkout", new { id = workout.Id }, workout);
        }

        // DELETE: api/Workout/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkout([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workout = await _context.Workout.FindAsync(id);
            if (workout == null)
            {
                return NotFound();
            }

            _context.Workout.Remove(workout);
            await _context.SaveChangesAsync();

            return Ok(workout);
        }

        private bool WorkoutExists(int id)
        {
            return _context.Workout.Any(e => e.Id == id);
        }
    }
}