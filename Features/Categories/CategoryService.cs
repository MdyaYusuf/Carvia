using Carvia.Core.Models;
using Carvia.Core.Persistence;
using Carvia.Features.Cars;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Carvia.Features.Categories;

public class CategoryService(
  ICategoryRepository _categoryRepository,
  CategoryBusinessRules _businessRules,
  CarBusinessRules _carBusinessRules,
  CategoryMapper _mapper,
  IUnitOfWork _unitOfWork) : ICategoryService
{
  public async Task<ReturnModel<List<ShowcaseCategoryViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<Category, bool>>? filter = null,
    Func<IQueryable<Category>, IQueryable<Category>>? include = null,
    Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var categories = await _categoryRepository.GetAllAsync(
      filter,
      include,
      orderBy ?? (q => q.OrderBy(c => c.Name)),
      enableTracking,
      cancellationToken: cancellationToken);

    var response = _mapper.EntityToShowcaseViewModelList(categories);

    return new ReturnModel<List<ShowcaseCategoryViewModel>>
    {
      Success = true,
      Message = "Kategoriler başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<DetailedCategoryViewModel>> GetAsync(
    Expression<Func<Category, bool>> predicate,
    string? userRole,
    Func<IQueryable<Category>, IQueryable<Category>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var category = await _categoryRepository.GetAsync(
      predicate,
      include: query => query.Include(c => c.Cars),
      enableTracking,
      cancellationToken);

    if (category == null)
    {
      return new ReturnModel<DetailedCategoryViewModel>
      {
        Success = true,
        Message = "Eşleşen kategori bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    var response = _mapper.EntityToDetailedViewModel(category);

    return new ReturnModel<DetailedCategoryViewModel>
    {
      Success = true,
      Message = "Kategori detayları başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<DetailedCategoryViewModel>> GetByIdAsync(
    Guid id,
    string? userRole,
    Func<IQueryable<Category>, IQueryable<Category>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var category = await _businessRules.GetCategoryIfExistAsync(
      id,
      include: query => query.Include(c => c.Cars),
      enableTracking: false,
      cancellationToken: cancellationToken);

    var response = _mapper.EntityToDetailedViewModel(category);

    return new ReturnModel<DetailedCategoryViewModel>
    {
      Success = true,
      Message = "Kategori başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedCategoryViewModel>> AddAsync(
    CreateCategoryViewModel request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _carBusinessRules.UserRoleMustBeAdmin(userRole);

    await _businessRules.CategoryNameMustBeUniqueAsync(request.Name, cancellationToken: cancellationToken);
    await _businessRules.CategorySlugMustBeUniqueAsync(request.Slug, cancellationToken: cancellationToken);

    Category category = _mapper.CreateViewModelToEntity(request);

    await _categoryRepository.AddAsync(category, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = _mapper.EntityToCreatedViewModel(category);

    return new ReturnModel<CreatedCategoryViewModel>
    {
      Success = true,
      Message = "Kategori başarıyla eklendi.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _carBusinessRules.UserRoleMustBeAdmin(userRole);

    var category = await _businessRules.GetCategoryIfExistAsync(
      id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    _categoryRepository.Delete(category);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Kategori başarıyla silindi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCategoryViewModel request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _carBusinessRules.UserRoleMustBeAdmin(userRole);

    var existingCategory = await _businessRules.GetCategoryIfExistAsync(
      request.Id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    if (existingCategory.Name != request.Name)
    {
      await _businessRules.CategoryNameMustBeUniqueAsync(request.Name, request.Id, cancellationToken);
    }

    if (existingCategory.Slug != request.Slug)
    {
      await _businessRules.CategorySlugMustBeUniqueAsync(request.Slug, request.Id, cancellationToken);
    }

    _mapper.UpdateEntityFromViewModel(request, existingCategory);

    _categoryRepository.Update(existingCategory);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Kategori başarıyla güncellendi.",
      StatusCode = 200
    };
  }
}
