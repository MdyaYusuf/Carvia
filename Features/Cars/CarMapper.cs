namespace Carvia.Features.Cars;

public class CarMapper
{
  public ShowcaseCarViewModel EntityToShowcaseViewModel(Car car)
  {
    return new ShowcaseCarViewModel(
      car.Id,
      car.Make,
      car.Model,
      car.Year,
      car.ImageUrl);
  }

  public List<ShowcaseCarViewModel> EntityToShowcaseViewModelList(
    IEnumerable<Car> cars)
  {
    return cars.Select(EntityToShowcaseViewModel).ToList();
  }

  public DetailedCarViewModel EntityToDetailedViewModel(
    Car car)
  {
    return new DetailedCarViewModel
    {
      Id = car.Id,
      Make = car.Make,
      Model = car.Model,
      Year = car.Year,
      Price = car.Price,
      Description = car.Description,
      CategoryName = car.Category?.Name ?? "Kategorisiz",
      MainImageUrl = car.ImageUrl,
      GalleryUrls = car.Images?.OrderBy(i => i.DisplayOrder).Select(i => i.Url) ?? new List<string>()
    };
  }

  public CreatedCarViewModel EntityToCreatedViewModel(
    Car car)
  {
    return new CreatedCarViewModel(
      car.Id,
      car.Make,
      car.Model);
  }

  public Car CreateViewModelToEntity(
    CreateCarViewModel viewModel)
  {
    return new Car
    {
      Make = viewModel.Make,
      Model = viewModel.Model,
      Year = viewModel.Year,
      Price = viewModel.Price,
      Description = viewModel.Description,
      CategoryId = viewModel.CategoryId,
      ImageUrl = string.Empty
    };
  }

  public void UpdateEntityFromViewModel(
    UpdateCarViewModel viewModel,
    Car car)
  {
    car.Make = viewModel.Make;
    car.Model = viewModel.Model;
    car.Year = viewModel.Year;
    car.Price = viewModel.Price;
    car.Description = viewModel.Description;
    car.CategoryId = viewModel.CategoryId;
  }
}
