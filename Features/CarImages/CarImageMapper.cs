namespace Carvia.Features.CarImages;

public class CarImageMapper
{
  public CreatedCarImageViewModel EntityToCreatedViewModel(
    CarImage carImage)
  {
    return new CreatedCarImageViewModel(
      carImage.Id,
      carImage.Url);
  }

  public ShowcaseCarImageViewModel EntityToShowcaseViewModel(
    CarImage carImage)
  {
    return new ShowcaseCarImageViewModel(
      carImage.Id,
      carImage.Url,
      carImage.AltText,
      carImage.DisplayOrder);
  }

  public List<ShowcaseCarImageViewModel> EntityToShowcaseViewModelList(
    IEnumerable<CarImage> carImages)
  {
    return carImages.Select(EntityToShowcaseViewModel).ToList();
  }

  public DetailedCarImageViewModel EntityToDetailedViewModel(
    CarImage carImage)
  {
    return new DetailedCarImageViewModel
    {
      Id = carImage.Id,
      Url = carImage.Url,
      AltText = carImage.AltText,
      DisplayOrder = carImage.DisplayOrder,
      CarId = carImage.CarId
    };
  }

  public CarImage CreateViewModelToEntity(
    CreateCarImageViewModel viewModel)
  {
    return new CarImage
    {
      Url = viewModel.Url,
      AltText = viewModel.AltText,
      DisplayOrder = viewModel.DisplayOrder,
      CarId = viewModel.CarId
    };
  }

  public void UpdateEntityFromViewModel(
    UpdateCarImageViewModel viewModel,
    CarImage carImage)
  {
    carImage.Url = viewModel.ExistingImageUrl;
    carImage.AltText = viewModel.AltText;
    carImage.DisplayOrder = viewModel.DisplayOrder;
    carImage.CarId = viewModel.CarId;
  }
}
