using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieActorAPI.Data;
using MovieActorAPI.Models;

namespace MovieActorAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorController : ControllerBase
    {
        private readonly MovieActorDbContext _context;

        public ActorController(MovieActorDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActors()
        {
            return await _context.Actors.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Actor>> GetActor(int id)
        {
            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
            {
                return NotFound();
            }

            return actor;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Actor>>> SearchActors(string name)
        {
            var actors = await _context.Actors.Where(a => a.Name.Contains(name)).ToListAsync();

            if (actors == null || actors.Count == 0)
            {
                return NotFound();
            }

            return actors;
        }


        [HttpPost]
        public async Task<ActionResult<Actor>> PostActor(Actor actor)
        {
            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, actor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutActor(int id, Actor actor)
        {
            var actor1 = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            actor1.Name = actor.Name;
            actor1.Gender = actor.Gender;
            _context.Entry(actor1).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(actor1);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActor(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }
    }
}
