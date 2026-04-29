using Carvia.Infrastructure.Contexts;
using Carvia.Infrastructure.Data;

namespace Carvia.Features.Categories;

public class EfCategoryRepository : EfBaseRepository<BaseDbContext, Category, Guid>, ICategoryRepository
{
  public EfCategoryRepository(BaseDbContext context) : base(context)
  {

  }
}
