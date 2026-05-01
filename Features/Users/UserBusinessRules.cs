using Carvia.Core.Exceptions;

namespace Carvia.Features.Users;

public class UserBusinessRules(IUserRepository _userRepository)
{
  public async Task<User> GetUserIfExistAsync(
    Guid id,
    Func<IQueryable<User>, IQueryable<User>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (user == null)
    {
      throw new NotFoundException($"{id} numaralı kullanıcı bulunamadı.");
    }

    return user;
  }

  public async Task UserEmailMustBeUniqueAsync(
    string email,
    Guid? id = null,
    CancellationToken cancellationToken = default)
  {
    var exists = await _userRepository.AnyAsync(u => u.Email == email && (id == null || u.Id != id), cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu e-posta adresi zaten kullanımda.");
    }
  }

  public async Task UsernameMustBeUniqueAsync(
    string username,
    Guid? id = null,
    CancellationToken cancellationToken = default)
  {
    var exists = await _userRepository.AnyAsync(u => u.Username == username && (id == null || u.Id != id), cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu kullanıcı adı zaten alınmış.");
    }
  }

  public void UserMustBeActive(
    User user)
  {
    if (!user.IsActive)
    {
      throw new BusinessException("Kullanıcı hesabı pasif durumdadır.");
    }
  }
}
