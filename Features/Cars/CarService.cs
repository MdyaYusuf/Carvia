using Carvia.Core.Exceptions;
using Carvia.Core.Models;
using Carvia.Core.Persistence;
using Carvia.Core.Utilities.Files;
using Carvia.Features.CarImages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Carvia.Features.Cars;

public class CarService(
  ICarRepository _carRepository,
  CarBusinessRules _businessRules,
  CarMapper _mapper,
  IUnitOfWork _unitOfWork) : ICarService
{
  public async Task<ReturnModel<List<ShowcaseCarViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<Car, bool>>? filter = null,
    Func<IQueryable<Car>, IQueryable<Car>>? include = null,
    Func<IQueryable<Car>, IOrderedQueryable<Car>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var cars = await _carRepository.GetAllAsync(
      filter,
      include: query => query.Include(c => c.Category),
      orderBy,
      enableTracking,
      cancellationToken: cancellationToken);

    var response = _mapper.EntityToShowcaseViewModelList(cars);

    return new ReturnModel<List<ShowcaseCarViewModel>>
    {
      Success = true,
      Message = "Araç listesi başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<DetailedCarViewModel>> GetAsync(
    Expression<Func<Car, bool>> predicate,
    string? userRole,
    Func<IQueryable<Car>, IQueryable<Car>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var car = await _carRepository.GetAsync(
      predicate,
      include: query => query.Include(c => c.Category).Include(c => c.Images),
      enableTracking,
      cancellationToken);

    if (car == null)
    {
      return new ReturnModel<DetailedCarViewModel>
      {
        Success = true,
        Message = "Eşleşen araç bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    var response = _mapper.EntityToDetailedViewModel(car);

    return new ReturnModel<DetailedCarViewModel>
    {
      Success = true,
      Message = "Araç detayları başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<DetailedCarViewModel>> GetByIdAsync(
    Guid id,
    string? userRole,
    Func<IQueryable<Car>, IQueryable<Car>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var car = await _businessRules.GetCarIfExistAsync(
      id,
      include: query => query.Include(c => c.Category).Include(c => c.Images),
      enableTracking: false,
      cancellationToken: cancellationToken);

    var response = _mapper.EntityToDetailedViewModel(car);

    return new ReturnModel<DetailedCarViewModel>
    {
      Success = true,
      Message = "Araç bilgileri başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedCarViewModel>> AddAsync(
  CreateCarViewModel request,
  string userRole,
  CancellationToken cancellationToken = default)
  {
    _businessRules.UserRoleMustBeAdmin(userRole);

    await _businessRules.CategoryMustExistAsync(request.CategoryId, cancellationToken);
    await _businessRules.CarMustNotBeDuplicateAsync(request.Make, request.Model, request.Year, cancellationToken: cancellationToken);
    _businessRules.PriceMustBePositive(request.Price);
    _businessRules.YearMustBeInValidRange(request.Year);

    Car car = _mapper.CreateViewModelToEntity(request);

    if (request.MainImage is { Length: > 0 })
    {
      FileHelper.ValidateImage(request.MainImage);
      car.ImageUrl = await FileHelper.SaveImageToDisk(
        request.MainImage,
        "cars",
        $"{car.Make}-{car.Model}",
        cancellationToken);
    }

    if (request.GalleryImages != null && request.GalleryImages.Any())
    {
      foreach (var imageFile in request.GalleryImages)
      {
        if (imageFile.Length > 0)
        {
          FileHelper.ValidateImage(imageFile);

          var galleryPath = await FileHelper.SaveImageToDisk(
              imageFile,
              "gallery",
              $"{car.Make}-{car.Model}-gallery",
              cancellationToken);

          car.Images.Add(new CarImage
          {
            Url = galleryPath,
            DisplayOrder = 0
          });
        }
      }
    }

    await _carRepository.AddAsync(car, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = _mapper.EntityToCreatedViewModel(car);

    return new ReturnModel<CreatedCarViewModel>
    {
      Success = true,
      Message = "Araç ve tüm görseller başarıyla eklendi.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.UserRoleMustBeAdmin(userRole);

    var car = await _businessRules.GetCarIfExistAsync(
      id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    FileHelper.DeleteImageFromDisk(car.ImageUrl);

    _carRepository.Delete(car);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Araç ve dosyaları başarıyla silindi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
  UpdateCarViewModel request,
  string userRole,
  CancellationToken cancellationToken = default)
  {
    _businessRules.UserRoleMustBeAdmin(userRole);

    var existingCar = await _carRepository.GetAsync(
      c => c.Id == request.Id,
      include: q => q.Include(c => c.Images),
      enableTracking: true,
      cancellationToken: cancellationToken);

    if (existingCar == null)
    {
      throw new BusinessException("Araç bulunamadı.");
    }

    existingCar.ImageUrl = await FileHelper.ReplaceImageOnDisk(
      request.NewMainImage,
      existingCar.ImageUrl,
      "cars",
      $"{request.Make}-{request.Model}",
      cancellationToken) ?? existingCar.ImageUrl;

    if (request.DeletedImageIds != null)
    {
      foreach (var imageId in request.DeletedImageIds)
      {
        var img = existingCar.Images.FirstOrDefault(i => i.Id == imageId);

        if (img != null)
        {
          FileHelper.DeleteImageFromDisk(img.Url);
          existingCar.Images.Remove(img);
        }
      }
    }

    if (request.NewGalleryImages != null)
    {
      foreach (var file in request.NewGalleryImages)
      {
        var path = await FileHelper.SaveImageToDisk(file, "gallery", $"{request.Make}-{request.Model}-detail", cancellationToken);
        existingCar.Images.Add(new CarImage { Url = path });
      }
    }

    _mapper.UpdateEntityFromViewModel(request, existingCar);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Güncelleme başarılı.",
      StatusCode = 200
    };
  }
}
