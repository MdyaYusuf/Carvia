using Carvia.Core.Entities;
using Carvia.Features.CarImages;
using Carvia.Features.Categories;
using Carvia.Features.CuratorItems;
using System.Diagnostics.CodeAnalysis;

namespace Carvia.Features.Cars;

public class Car : BaseEntity<Guid>
{
  [SetsRequiredMembers]
  public Car() : base()
  {
    Make = default!;
    Model = default!;
    ImageUrl = default!;
    Description = default!;
    Images = new HashSet<CarImage>();
    CuratorItems = new HashSet<CuratorItem>();
  }

  public required string Make { get; set; }
  public required string Model { get; set; }
  public int Year { get; set; }
  public decimal Price { get; set; }
  public required string ImageUrl { get; set; }
  public required string Description { get; set; }

  // Navigational properties
  public Guid CategoryId { get; set; }
  public virtual Category Category { get; set; } = default!;
  public virtual ICollection<CarImage> Images { get; set; }
  public virtual ICollection<CuratorItem> CuratorItems { get; set; }
}
