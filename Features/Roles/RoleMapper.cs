namespace Carvia.Features.Roles;

public class RoleMapper
{
  public Role CreateViewModelToEntity(CreateRoleViewModel viewModel)
  {
    return new Role
    {
      Name = viewModel.Name.ToUpper(),
      CreatedDate = DateTime.Now
    };
  }

  public void UpdateEntityFromViewModel(UpdateRoleViewModel viewModel, Role role)
  {
    role.Name = viewModel.Name.ToUpper();
    role.UpdatedDate = DateTime.Now;
  }

  public ShowcaseRoleViewModel EntityToShowcaseViewModel(Role role)
  {
    return new ShowcaseRoleViewModel(role.Id, role.Name);
  }

  public List<ShowcaseRoleViewModel> EntityToShowcaseViewModelList(List<Role> roles)
  {
    return roles.Select(EntityToShowcaseViewModel).ToList();
  }

  public CreatedRoleViewModel EntityToCreatedViewModel(Role role)
  {
    return new CreatedRoleViewModel(role.Id, role.Name);
  }
}
