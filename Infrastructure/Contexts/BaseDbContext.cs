using Carvia.Features.Cars;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Carvia.Infrastructure.Contexts;

public class BaseDbContext : DbContext
{
  public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
  {
  }

  public DbSet<Car> Cars { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
