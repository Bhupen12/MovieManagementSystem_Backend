using Microsoft.EntityFrameworkCore;
using MovieActorAPI.Models;

namespace MovieActorAPI.Data
{
    public class MovieActorDbContext: DbContext
    {
        public MovieActorDbContext(DbContextOptions<MovieActorDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actor>()
                .HasMany(m => m.Movies)
                .WithOne(a => a.Actor)
                .HasForeignKey(a => a.ActorID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Director>()
                .HasMany(m => m.Movies)
                .WithOne(d => d.Director)
                .HasForeignKey(d => d.DirectorID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
