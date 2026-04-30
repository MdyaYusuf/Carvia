namespace Carvia.Features.CuratorItems;

public static class CuratorItemRegistration
{
  public static IServiceCollection AddCuratorItemDependencies(this IServiceCollection services)
  {
    services.AddScoped<ICuratorItemRepository, EfCuratorItemRepository>();

    return services;
  }
}
