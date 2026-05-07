using Carvia.Infrastructure.Contexts;
using Carvia.Infrastructure.Data;

namespace Carvia.Features.Roles;

public class EfRoleRepository : EfBaseRepository<BaseDbContext, Role, int>, IRoleRepository
{
  public EfRoleRepository(BaseDbContext context) : base(context)
  {

  }
}
