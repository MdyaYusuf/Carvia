using Carvia.Core.Exceptions;

namespace Carvia.Features.Categories;

public class CategoryBusinessRules(ICategoryRepository _categoryRepository)
{
  public async Task<Category> GetCategoryIfExistAsync(
    Guid id,
    Func<IQueryable<Category>, IQueryable<Category>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var category = await _categoryRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (category == null)
    {
      throw new NotFoundException($"{id} numaralı kategori bulunamadı.");
    }

    return category;
  }

  public async Task CategoryNameMustBeUniqueAsync(
    string name,
    Guid? id = null,
    CancellationToken cancellationToken = default)
  {
    var exists = await _categoryRepository.AnyAsync(c => c.Name == name && (id == null || c.Id != id), cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu kategori ismi zaten kullanımda.");
    }
  }
  public async Task CategorySlugMustBeUniqueAsync(
    string slug,
    Guid? id = null,
    CancellationToken cancellationToken = default)
  {
    var exists = await _categoryRepository.AnyAsync(c => c.Slug == slug && (id == null || c.Id != id), cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu kategori slug değeri zaten mevcut.");
    }
  }
}
