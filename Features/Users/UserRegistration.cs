namespace Carvia.Features.Users;

public static class UserRegistration
{
  public static IServiceCollection AddUserDependencies(this IServiceCollection services)
  {
    services.AddScoped<IUserRepository, EfUserRepository>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<UserBusinessRules>();
    services.AddScoped<UserMapper>();

    return services;
  }
}
