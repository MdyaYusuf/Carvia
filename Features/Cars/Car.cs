using Carvia.Core.Entities;
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
  }

  public required string Make { get; set; }
  public required string Model { get; set; }
  public int Year { get; set; }
  public decimal Price { get; set; }
  public required string ImageUrl { get; set; }
  public required string Description { get; set; }
}
