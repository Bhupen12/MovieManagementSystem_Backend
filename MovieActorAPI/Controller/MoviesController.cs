using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieActorAPI.Data;
using MovieActorAPI.Models;

namespace MovieActorAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly MovieActorDbContext _context;

        public MoviesController(MovieActorDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var movies = await _context.Movies
                .Include(m => m.Actor)
                .Include(m => m.Director)
                .Select(m => new Movie
                {
                    Id = m.Id,
                    Title = m.Title,
                    ReleaseYear = m.ReleaseYear,
                    Genre = m.Genre,
                    ActorID = m.ActorID,
                    DirectorID = m.DirectorID,
                    Actor = m.Actor,
                    Director = m.Director
                })
                .ToListAsync();

            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }

            return movies;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies.Include(m => m.Actor).Include(m => m.Director).FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {

            int actorID = (int)movie.ActorID;
            var actor = await _context.Actors.FindAsync(actorID);

            int directorID = (int)movie.DirectorID;
            var director = await _context.Directors.FindAsync(directorID);

            if (actor != null && directorID != null)
            {
                movie.Actor = actor;
                movie.Director = director;
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
            }
            else if (actor == null)
            {
                return BadRequest("Invalid Actor ID.");
            }
            else
            {
                return BadRequest("Invalid Director ID.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            var movie1 = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }
            movie1.Title = movie.Title;
            movie1.ReleaseYear = movie.ReleaseYear;
            movie1.Genre = movie.Genre;
            movie1.ActorID = movie.ActorID;
            movie1.DirectorID = movie.DirectorID;
            _context.Entry(movie1).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(movie1);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return Ok(movie);
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
