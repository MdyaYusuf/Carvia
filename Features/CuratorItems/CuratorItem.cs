using Carvia.Core.Entities;
using Carvia.Features.Cars;
using Carvia.Features.Users;
using System.Diagnostics.CodeAnalysis;

namespace Carvia.Features.CuratorItems;

public class CuratorItem : BaseEntity<Guid>
{
  [SetsRequiredMembers]
  public CuratorItem() : base()
  {

  }

  public string? UserNote { get; set; }

  // Navigational properties
  public Guid UserId { get; set; }
  public virtual User User { get; set; } = default!;
  public Guid CarId { get; set; }
  public virtual Car Car { get; set; } = default!;
}
