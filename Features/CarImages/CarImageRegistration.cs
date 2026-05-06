namespace Carvia.Features.CarImages;

public static class CarImageRegistration
{
  public static IServiceCollection AddCarImageDependencies(this IServiceCollection services)
  {
    services.AddScoped<ICarImageRepository, EfCarImageRepository>();
    services.AddScoped<ICarImageService, CarImageService>();
    services.AddScoped<CarImageBusinessRules>();
    services.AddSingleton<CarImageMapper>();

    return services;
  }
}
