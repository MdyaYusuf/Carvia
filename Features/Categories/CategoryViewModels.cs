using System.ComponentModel.DataAnnotations;

namespace Carvia.Features.Categories;

public sealed record CreatedCategoryViewModel(
  Guid Id,
  string Name,
  string Slug);

public sealed record ShowcaseCategoryViewModel(
  Guid Id,
  string Name,
  string Slug);

public class DetailedCategoryViewModel
{
  public Guid Id { get; set; }
  public string Name { get; set; } = null!;
  public string Slug { get; set; } = null!;
  public string? Description { get; set; }
  public int CarCount { get; set; }
}

public class CreateCategoryViewModel
{
  [Required, StringLength(100)]
  public string Name { get; set; } = null!;
  [Required, StringLength(100)]
  public string Slug { get; set; } = null!;
  [StringLength(500)]
  public string? Description { get; set; }
}

public class UpdateCategoryViewModel
{
  [Required]
  public Guid Id { get; set; }
  [Required, StringLength(100)]
  public string Name { get; set; } = null!;
  [Required, StringLength(100)]
  public string Slug { get; set; } = null!;
  [StringLength(500)]
  public string? Description { get; set; }
}
