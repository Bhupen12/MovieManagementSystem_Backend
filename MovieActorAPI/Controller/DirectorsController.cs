using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieActorAPI.Data;
using MovieActorAPI.Models;

namespace MovieActorAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class DirectorsController : ControllerBase
    {
        private readonly MovieActorDbContext _context;

        public DirectorsController(MovieActorDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Director>>> GetDirectors()
        {
            var director = await _context.Directors.ToListAsync();
            if (director == null)
            {
                return NotFound();
            }
            return director;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Director>> GetDirector(int id)
        {
            var director = await _context.Directors.FindAsync(id);

            if (director == null)
            {
                return NotFound();
            }

            return director;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Director>>> SearchDirectors(string name)
        {
            var directors = await _context.Directors.Where(a => a.Name.Contains(name)).ToListAsync();

            if (directors == null || directors.Count == 0)
            {
                return NotFound();
            }

            return directors;
        }

        [HttpPost]
        public async Task<ActionResult<Director>> PostDirector(Director director)
        {
            _context.Directors.Add(director);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDirector), new { id = director.Id }, director);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDirector(int id, Director director)
        {
            var director1 = await _context.Directors.FindAsync(id);
            if (director == null)
            {
                return NotFound();
            }
            director1.Name = director.Name;
            director1.Gender = director.Gender;
            _context.Entry(director1).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(director1);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDirector(int id)
        {
            var director = await _context.Directors.FindAsync(id);
            if (director == null)
            {
                return NotFound();
            }

            _context.Directors.Remove(director);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DirectorExists(int id)
        {
            return _context.Directors.Any(e => e.Id == id);
        }
    }
}
