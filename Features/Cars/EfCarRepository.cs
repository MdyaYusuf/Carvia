using Carvia.Infrastructure.Contexts;
using Carvia.Infrastructure.Data;

namespace Carvia.Features.Cars;

public class EfCarRepository : EfBaseRepository<BaseDbContext, Car, Guid>, ICarRepository
{
  public EfCarRepository(BaseDbContext context) : base(context)
  {

  }
}
