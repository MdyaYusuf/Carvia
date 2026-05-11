using Carvia.Core.Models;
using Carvia.Core.Persistence;
using Carvia.Core.Utilities.Files;
using Carvia.Features.Cars;
using System.Linq.Expressions;

namespace Carvia.Features.CarImages;

public class CarImageService(
  ICarImageRepository _carImageRepository,
  CarImageBusinessRules _businessRules,
  CarBusinessRules _carBusinessRules,
  CarImageMapper _mapper,
  IUnitOfWork _unitOfWork) : ICarImageService
{
  public async Task<ReturnModel<List<ShowcaseCarImageViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<CarImage, bool>>? filter = null,
    Func<IQueryable<CarImage>, IQueryable<CarImage>>? include = null,
    Func<IQueryable<CarImage>, IOrderedQueryable<CarImage>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var carImages = await _carImageRepository.GetAllAsync(
      filter,
      include,
      orderBy ?? (q => q.OrderBy(ci => ci.DisplayOrder)),
      enableTracking,
      cancellationToken: cancellationToken);

    var response = _mapper.EntityToShowcaseViewModelList(carImages);

    return new ReturnModel<List<ShowcaseCarImageViewModel>>
    {
      Success = true,
      Message = "Araç görselleri başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<DetailedCarImageViewModel>> GetAsync(
    Expression<Func<CarImage, bool>> predicate,
    string? userRole,
    Func<IQueryable<CarImage>, IQueryable<CarImage>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var carImage = await _carImageRepository.GetAsync(
      predicate,
      include,
      enableTracking,
      cancellationToken);

    if (carImage == null)
    {
      return new ReturnModel<DetailedCarImageViewModel>
      {
        Success = true,
        Message = "Eşleşen görsel bulunamadı.",
        StatusCode = 200
      };
    }

    var response = _mapper.EntityToDetailedViewModel(carImage);

    return new ReturnModel<DetailedCarImageViewModel>
    {
      Success = true,
      Message = "Görsel detayları başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<DetailedCarImageViewModel>> GetByIdAsync(
    Guid id,
    string? userRole,
    Func<IQueryable<CarImage>, IQueryable<CarImage>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var carImage = await _businessRules.GetCarImageIfExistAsync(
      id,
      include,
      enableTracking,
      cancellationToken);

    var response = _mapper.EntityToDetailedViewModel(carImage);

    return new ReturnModel<DetailedCarImageViewModel>
    {
      Success = true,
      Message = "Görsel başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedCarImageViewModel>> AddAsync(
  CreateCarImageViewModel request,
  string userRole,
  CancellationToken cancellationToken = default)
  {
    _carBusinessRules.UserRoleMustBeAdmin(userRole);

    var car = await _carBusinessRules.GetCarIfExistAsync(request.CarId, cancellationToken: cancellationToken);

    _businessRules.DisplayOrderCannotBeNegative(request.DisplayOrder);

    CarImage carImage = _mapper.CreateViewModelToEntity(request);

    if (request.ImageFile is { Length: > 0 })
    {
      FileHelper.ValidateImage(request.ImageFile);

      carImage.Url = await FileHelper.SaveImageToDisk(
        request.ImageFile,
        "gallery",
        $"{car.Make}-{car.Model}-detail",
        cancellationToken);
    }

    await _businessRules.ImageUrlMustBeUniqueForCarAsync(carImage.Url, request.CarId, cancellationToken: cancellationToken);

    await _carImageRepository.AddAsync(carImage, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = _mapper.EntityToCreatedViewModel(carImage);

    return new ReturnModel<CreatedCarImageViewModel>
    {
      Success = true,
      Message = "Galeri görseli başarıyla eklendi.",
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

    var carImage = await _businessRules.GetCarImageIfExistAsync(
      id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    FileHelper.DeleteImageFromDisk(carImage.Url);

    _carImageRepository.Delete(carImage);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Görsel başarıyla silindi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
  UpdateCarImageViewModel request,
  string userRole,
  CancellationToken cancellationToken = default)
  {
    _carBusinessRules.UserRoleMustBeAdmin(userRole);

    var existingImage = await _businessRules.GetCarImageIfExistAsync(
      request.Id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    var car = await _carBusinessRules.GetCarIfExistAsync(request.CarId, cancellationToken: cancellationToken);

    _businessRules.DisplayOrderCannotBeNegative(request.DisplayOrder);

    existingImage.Url = await FileHelper.ReplaceImageOnDisk(
      request.NewImageFile,
      existingImage.Url,
      "gallery",
      $"{car.Make}-{car.Model}-detail",
      cancellationToken) ?? existingImage.Url;

    await _businessRules.ImageUrlMustBeUniqueForCarAsync(existingImage.Url, request.CarId, request.Id, cancellationToken);

    _mapper.UpdateEntityFromViewModel(request, existingImage);

    _carImageRepository.Update(existingImage);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Görsel bilgileri başarıyla güncellendi.",
      StatusCode = 200
    };
  }
}
