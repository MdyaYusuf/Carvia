using System.Diagnostics.CodeAnalysis;

namespace Carvia.Core.Entities;

public abstract class BaseEntity<TId> where TId : notnull
{
  [SetsRequiredMembers]
  protected BaseEntity()
  {
    Id = default!;
    CreatedDate = DateTime.UtcNow;
  }

  [SetsRequiredMembers]
  protected BaseEntity(TId id) : this()
  {
    Id = id;
  }

  public required TId Id { get; set; }
  public DateTime CreatedDate { get; set; }
  public DateTime? UpdatedDate { get; set; }
}