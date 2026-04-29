using Carvia.Infrastructure.Contexts;
using Carvia.Infrastructure.Data;

namespace Carvia.Features.CarImages;

public class EfCarImageRepository : EfBaseRepository<BaseDbContext, CarImage, Guid>, ICarImageRepository
{
  public EfCarImageRepository(BaseDbContext context) : base(context)
  {

  }
}
