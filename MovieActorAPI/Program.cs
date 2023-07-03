using Microsoft.EntityFrameworkCore;
using MovieActorAPI.Data;
using Microsoft.Extensions.DependencyInjection;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<MovieActorDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MovieActorContext")));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
                      });
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
