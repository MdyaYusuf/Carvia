using Carvia.Core.Entities;
using Carvia.Features.Users;
using System.Diagnostics.CodeAnalysis;

namespace Carvia.Features.Roles;

public class Role : BaseEntity<int>
{
  [SetsRequiredMembers]
  public Role()
  {
    Users = new HashSet<User>();

    Name = default!;
  }

  public required string Name { get; set; }

  // Navigation Properties
  public virtual ICollection<User> Users { get; set; }
}
