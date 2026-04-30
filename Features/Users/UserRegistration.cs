namespace Carvia.Features.Users;

public static class UserRegistration
{
  public static IServiceCollection AddUserDependencies(this IServiceCollection services)
  {
    services.AddScoped<IUserRepository, EfUserRepository>(); 

    return services;
  }
}
