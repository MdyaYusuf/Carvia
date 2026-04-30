namespace Carvia.Features.Categories;

public static class CategoryRegistration
{
  public static IServiceCollection AddCategoryDependencies(this IServiceCollection services)
  {
    services.AddScoped<ICategoryRepository, EfCategoryRepository>();

    return services;
  }
}
