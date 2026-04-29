using Carvia.Core.Entities;
using Carvia.Features.Cars;
using System.Diagnostics.CodeAnalysis;

namespace Carvia.Features.CarImages;

public class CarImage : BaseEntity<Guid>
{
  [SetsRequiredMembers]
  public CarImage() : base()
  {
    Url = default!;
  }

  public required string Url { get; set; }
  public string? AltText { get; set; }
  public int DisplayOrder { get; set; }

  // Navigational properties
  public Guid CarId { get; set; }
  public virtual Car Car { get; set; } = default!;
}
