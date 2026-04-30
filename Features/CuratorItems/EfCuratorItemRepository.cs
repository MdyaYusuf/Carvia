using Carvia.Infrastructure.Contexts;
using Carvia.Infrastructure.Data;

namespace Carvia.Features.CuratorItems;

public class EfCuratorItemRepository : EfBaseRepository<BaseDbContext, CuratorItem, Guid>, ICuratorItemRepository
{
  public EfCuratorItemRepository(BaseDbContext context) : base(context)
  {

  }
}
