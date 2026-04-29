using Carvia.Core.Entities;
using Carvia.Features.CuratorItems;
using System.Diagnostics.CodeAnalysis;

namespace Carvia.Features.Users;

public class User : BaseEntity<Guid>
{
  [SetsRequiredMembers]
  public User() : base()
  {
    CuratorItems = new HashSet<CuratorItem>();

    Username = default!;
    Email = default!;
    PasswordHash = default!;
    PasswordKey = default!;
  }

  public required string Username { get; set; }
  public required string Email { get; set; }
  public required string PasswordHash { get; set; }
  public required string PasswordKey { get; set; }
  public string? RefreshToken { get; set; }
  public DateTime? RefreshTokenExpiration { get; set; }
  public string? ProfileImageUrl { get; set; }
  public string? Bio { get; set; }
  public bool IsActive { get; set; } = true;

  // Navigational properties
  public virtual ICollection<CuratorItem> CuratorItems { get; set; }
}
