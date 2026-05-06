
namespace Carvia.Features.Cars;

public static class CarRegistration
{
  public static IServiceCollection AddCarDependencies(this IServiceCollection services)
  {
    services.AddScoped<ICarRepository, EfCarRepository>();
    services.AddScoped<ICarService, CarService>();
    services.AddScoped<CarBusinessRules>();
    services.AddSingleton<CarMapper>();

    return services;
  }
}
