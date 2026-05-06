namespace Carvia.Features.Authentication;

public static class AuthenticationRegistration
{
  public static IServiceCollection AddAuthenticationDependencies(this IServiceCollection services)
  {
    services.AddScoped<IAuthenticationService, AuthenticationService>();
    services.AddScoped<AuthenticationBusinessRules>();

    return services;
  }
}
