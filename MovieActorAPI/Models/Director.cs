using System.Text.Json.Serialization;

namespace MovieActorAPI.Models
{
    public class Director
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }

        [JsonIgnore]
        public virtual ICollection<Movie>? Movies { get; set; }

        public static implicit operator Director(Movie v)
        {
            throw new NotImplementedException();
        }
    }
}
