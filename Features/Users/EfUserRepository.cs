using Carvia.Infrastructure.Contexts;
using Carvia.Infrastructure.Data;

namespace Carvia.Features.Users;

public class EfUserRepository : EfBaseRepository<BaseDbContext, User, Guid>, IUserRepository
{
  public EfUserRepository(BaseDbContext context) : base(context)
  {

  }
}
