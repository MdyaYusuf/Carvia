using Carvia.Core.Entities;
using Carvia.Features.Cars;
using System.Diagnostics.CodeAnalysis;

namespace Carvia.Features.Categories;

public class Category : BaseEntity<Guid>
{
  [SetsRequiredMembers]
  public Category() : base()
  {
    Name = default!;
    Slug = default!;
    Cars = new HashSet<Car>();
  }

  public required string Name { get; set; }
  public required string Slug { get; set; }
  public string? Description { get; set; }

  // Navigational properties
  public virtual ICollection<Car> Cars { get; set; }
}
