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

  public DetailedCarViewModel EntityToDetailedViewModel(Car car)
  {
    return new DetailedCarViewModel
    {
      Id = car.Id,
      Make = car.Make,
      Model = car.Model,
      Year = car.Year,
      Price = car.Price,
      Description = car.Description,
      CategoryId = car.CategoryId,
      CategoryName = car.Category?.Name ?? "Kategorisiz",
      MainImageUrl = car.ImageUrl,
      Gallery = car.Images?.OrderBy(i => i.DisplayOrder).Select(i => new GalleryImageDetails(i.Id, i.Url)) ?? new List<GalleryImageDetails>()
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
