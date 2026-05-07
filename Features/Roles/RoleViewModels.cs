using System.ComponentModel.DataAnnotations;

namespace Carvia.Features.Roles;

public sealed record CreatedRoleViewModel(
  int Id,
  string Name);

public sealed record ShowcaseRoleViewModel(
  int Id,
  string Name);

public class CreateRoleViewModel
{
  [Required, StringLength(50)]
  public string Name { get; set; } = null!;
}

public class UpdateRoleViewModel
{
  [Required]
  public int Id { get; set; }
  [Required, StringLength(50)]
  public string Name { get; set; } = null!;
}
