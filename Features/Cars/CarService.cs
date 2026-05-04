using Carvia.Core.Models;
using Carvia.Core.Persistence;
using Carvia.Core.Utilities.Files;
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

    await _carRepository.AddAsync(car, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = _mapper.EntityToCreatedViewModel(car);

    return new ReturnModel<CreatedCarViewModel>
    {
      Success = true,
      Message = "Araç ve ana görsel başarıyla eklendi.",
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

    var existingCar = await _businessRules.GetCarIfExistAsync(
      request.Id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    if (existingCar.CategoryId != request.CategoryId)
    {
      await _businessRules.CategoryMustExistAsync(request.CategoryId, cancellationToken);
    }

    if (existingCar.Make != request.Make || existingCar.Model != request.Model || existingCar.Year != request.Year)
    {
      await _businessRules.CarMustNotBeDuplicateAsync(request.Make, request.Model, request.Year, request.Id, cancellationToken);
    }

    _businessRules.PriceMustBePositive(request.Price);
    _businessRules.YearMustBeInValidRange(request.Year);

    existingCar.ImageUrl = await FileHelper.ReplaceImageOnDisk(
      request.NewMainImage,
      existingCar.ImageUrl,
      "cars",
      $"{request.Make}-{request.Model}",
      cancellationToken) ?? existingCar.ImageUrl;

    _mapper.UpdateEntityFromViewModel(request, existingCar);

    _carRepository.Update(existingCar);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Araç bilgileri ve görsel başarıyla güncellendi.",
      StatusCode = 200
    };
  }
}
