using Carvia.Features.CarImages;
using Carvia.Features.Cars;
using Carvia.Features.Categories;
using Carvia.Features.CuratorItems;
using Carvia.Features.Roles;
using Carvia.Features.Users;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Carvia.Infrastructure.Contexts;

public class BaseDbContext : DbContext
{
  public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
  {
  }

  public DbSet<Car> Cars { get; set; }
  public DbSet<Category> Categories { get; set; }
  public DbSet<CarImage> CarImages { get; set; }
  public DbSet<CuratorItem> CuratorItems { get; set; }
  public DbSet<User> Users { get; set; }
  public DbSet<Role> Roles { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
