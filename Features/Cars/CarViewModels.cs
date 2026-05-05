using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public sealed record CreatedCarViewModel(
  Guid Id,
  string Make,
  string Model);

public sealed record ShowcaseCarViewModel(
  Guid Id,
  string Make,
  string Model,
  int Year,
  string ImageUrl);

public class DetailedCarViewModel
{
  public Guid Id { get; set; }
  public string Make { get; set; } = null!;
  public string Model { get; set; } = null!;
  public int Year { get; set; }
  public decimal Price { get; set; }
  public string Description { get; set; } = null!;
  public Guid CategoryId { get; set; }
  public string CategoryName { get; set; } = null!;
  public string MainImageUrl { get; set; } = null!;
  public IEnumerable<string> GalleryUrls { get; set; } = new List<string>();
}

public class CreateCarViewModel
{
  [Required, StringLength(100)]
  public string Make { get; set; } = null!;
  [Required, StringLength(100)]
  public string Model { get; set; } = null!;
  [Range(1886, 2100)]
  public int Year { get; set; }
  [Required, DataType(DataType.Currency)]
  public decimal Price { get; set; }
  [Required, StringLength(2000)]
  public string Description { get; set; } = null!;
  [Display(Name = "Category")]
  public Guid CategoryId { get; set; }
  public IFormFile? MainImage { get; set; }
  public IEnumerable<SelectListItem>? Categories { get; set; }
}

public class UpdateCarViewModel
{
  [Required]
  public Guid Id { get; set; }
  [Required, StringLength(100)]
  public string Make { get; set; } = null!;
  [Required, StringLength(100)]
  public string Model { get; set; } = null!;
  [Range(1886, 2100)]
  public int Year { get; set; }
  [Required, DataType(DataType.Currency)]
  public decimal Price { get; set; }
  [Required, StringLength(2000)]
  public string Description { get; set; } = null!;
  [Display(Name = "Category")]
  public Guid CategoryId { get; set; }
  [StringLength(500)]
  public string? ExistingImageUrl { get; set; }
  public IFormFile? NewMainImage { get; set; }
  public IEnumerable<SelectListItem>? Categories { get; set; }
}