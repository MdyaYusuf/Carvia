using System.ComponentModel.DataAnnotations;

namespace Carvia.Features.CuratorItems;

public sealed record CreatedCuratorItemViewModel(
  Guid Id,
  Guid UserId,
  Guid CarId);

public sealed record ShowcaseCuratorItemViewModel(
  Guid Id,
  Guid CarId,
  string CarMake,
  string CarModel,
  string CarImageUrl,
  string? UserNote);

public class DetailedCuratorItemViewModel
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string Username { get; set; } = null!;
  public Guid CarId { get; set; }
  public string CarMake { get; set; } = null!;
  public string CarModel { get; set; } = null!;
  public string? UserNote { get; set; }
}

public class CreateCuratorItemViewModel
{
  [Required]
  public Guid UserId { get; set; }
  [Required]
  public Guid CarId { get; set; }
  [StringLength(500)]
  public string? UserNote { get; set; }
}

public class UpdateCuratorItemViewModel
{
  [Required]
  public Guid Id { get; set; }
  [StringLength(500)]
  public string? UserNote { get; set; }
}
