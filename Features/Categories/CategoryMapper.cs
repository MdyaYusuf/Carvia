namespace Carvia.Features.Categories;

public class CategoryMapper
{
  public CreatedCategoryViewModel EntityToCreatedViewModel(
    Category category)
  {
    return new CreatedCategoryViewModel(
      category.Id,
      category.Name,
      category.Slug);
  }

  public ShowcaseCategoryViewModel EntityToShowcaseViewModel(
    Category category)
  {
    return new ShowcaseCategoryViewModel(
      category.Id,
      category.Name,
      category.Slug);
  }

  public List<ShowcaseCategoryViewModel> EntityToShowcaseViewModelList(
    IEnumerable<Category> categories)
  {
    return categories.Select(EntityToShowcaseViewModel).ToList();
  }

  public DetailedCategoryViewModel EntityToDetailedViewModel(
    Category category)
  {
    return new DetailedCategoryViewModel
    {
      Id = category.Id,
      Name = category.Name,
      Slug = category.Slug,
      Description = category.Description,
      CarCount = category.Cars?.Count ?? 0
    };
  }

  public Category CreateViewModelToEntity(
    CreateCategoryViewModel viewModel)
  {
    return new Category
    {
      Name = viewModel.Name,
      Slug = viewModel.Slug,
      Description = viewModel.Description
    };
  }

  public void UpdateEntityFromViewModel(
    UpdateCategoryViewModel viewModel,
    Category category)
  {
    category.Name = viewModel.Name;
    category.Slug = viewModel.Slug;
    category.Description = viewModel.Description;
  }
}
