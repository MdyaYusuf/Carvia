namespace Carvia.Features.Categories;

public static class CategoryRegistration
{
  public static IServiceCollection AddCategoryDependencies(this IServiceCollection services)
  {
    services.AddScoped<ICategoryRepository, EfCategoryRepository>();
    services.AddScoped<ICategoryService, CategoryService>();
    services.AddScoped<CategoryBusinessRules>();
    services.AddSingleton<CategoryMapper>();

    return services;
  }
}
