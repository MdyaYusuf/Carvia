namespace Carvia.Features.Users;

public class UserMapper
{
  public CreatedUserViewModel EntityToCreatedViewModel(
    User user)
  {
    return new CreatedUserViewModel(
      user.Id,
      user.Username,
      user.Email);
  }

  public ShowcaseUserViewModel EntityToShowcaseViewModel(
    User user)
  {
    return new ShowcaseUserViewModel(
      user.Id,
      user.Username,
      user.ProfileImageUrl,
      user.Role?.Name);
  }

  public List<ShowcaseUserViewModel> EntityToShowcaseViewModelList(
    IEnumerable<User> users)
  {
    return users.Select(EntityToShowcaseViewModel).ToList();
  }

  public DetailedUserViewModel EntityToDetailedViewModel(
    User user)
  {
    return new DetailedUserViewModel
    {
      Id = user.Id,
      Username = user.Username,
      Email = user.Email,
      ProfileImageUrl = user.ProfileImageUrl,
      Bio = user.Bio,
      IsActive = user.IsActive,
      RoleName = user.Role?.Name,
      CuratorItemCount = user.CuratorItems?.Count ?? 0
    };
  }

  public void UpdateEntityFromViewModel(
    UpdateUserViewModel viewModel,
    User user)
  {
    user.Username = viewModel.Username;
    user.Email = viewModel.Email;
  }
}
