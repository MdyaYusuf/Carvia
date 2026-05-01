namespace Carvia.Features.CuratorItems;

public class CuratorItemMapper
{
  public CreatedCuratorItemViewModel EntityToCreatedViewModel(
    CuratorItem curatorItem)
  {
    return new CreatedCuratorItemViewModel(
      curatorItem.Id,
      curatorItem.UserId,
      curatorItem.CarId);
  }

  public ShowcaseCuratorItemViewModel EntityToShowcaseViewModel(
    CuratorItem curatorItem)
  {
    return new ShowcaseCuratorItemViewModel(
      curatorItem.Id,
      curatorItem.CarId,
      curatorItem.Car?.Make ?? "Unknown",
      curatorItem.Car?.Model ?? "Unknown",
      curatorItem.Car?.ImageUrl ?? string.Empty,
      curatorItem.UserNote);
  }

  public List<ShowcaseCuratorItemViewModel> EntityToShowcaseViewModelList(
    IEnumerable<CuratorItem> curatorItems)
  {
    return curatorItems.Select(EntityToShowcaseViewModel).ToList();
  }

  public DetailedCuratorItemViewModel EntityToDetailedViewModel(
    CuratorItem curatorItem)
  {
    return new DetailedCuratorItemViewModel
    {
      Id = curatorItem.Id,
      UserId = curatorItem.UserId,
      Username = curatorItem.User?.Username ?? "Unknown",
      CarId = curatorItem.CarId,
      CarMake = curatorItem.Car?.Make ?? "Unknown",
      CarModel = curatorItem.Car?.Model ?? "Unknown",
      UserNote = curatorItem.UserNote
    };
  }

  public CuratorItem CreateViewModelToEntity(CreateCuratorItemViewModel viewModel)
  {
    return new CuratorItem
    {
      UserId = viewModel.UserId,
      CarId = viewModel.CarId,
      UserNote = viewModel.UserNote
    };
  }

  public void UpdateEntityFromViewModel(
    UpdateCuratorItemViewModel viewModel,
    CuratorItem curatorItem)
  {
    curatorItem.UserNote = viewModel.UserNote;
  }
}
