using System.ComponentModel.DataAnnotations;

namespace Carvia.Features.CarImages;

public sealed record CreatedCarImageViewModel(
  Guid Id,
  string Url);

public sealed record ShowcaseCarImageViewModel(
  Guid Id,
  string Url,
  string? AltText,
  int DisplayOrder);

public class DetailedCarImageViewModel
{
  public Guid Id { get; set; }
  public string Url { get; set; } = null!;
  public string? AltText { get; set; }
  public int DisplayOrder { get; set; }
  public Guid CarId { get; set; }
}

public class CreateCarImageViewModel
{
  [Required, StringLength(500)]
  public string Url { get; set; } = null!;
  [StringLength(200)]
  public string? AltText { get; set; }
  [Required]
  public int DisplayOrder { get; set; }
  [Required]
  public Guid CarId { get; set; }
  public IFormFile? ImageFile { get; set; }
}

public class UpdateCarImageViewModel
{
  public Guid Id { get; set; }
  [Required, StringLength(500)]
  public string ExistingImageUrl { get; set; } = null!;
  [StringLength(200)]
  public string? AltText { get; set; }
  [Required]
  public int DisplayOrder { get; set; }
  [Required]
  public Guid CarId { get; set; }
  public IFormFile? NewImageFile { get; set; }
}
