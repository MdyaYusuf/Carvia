using Carvia.Core.Persistence;
using Carvia.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Carvia.Infrastructure.Data;

public static class DataRegistration
{
  public static IServiceCollection AddDataDependencies(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("SqlConnection");

    services.AddDbContext<BaseDbContext>(options =>
    {
      options.UseSqlServer(connectionString);
    });

    services.AddScoped<IUnitOfWork, UnitOfWork>();

    return services;
  }
}
