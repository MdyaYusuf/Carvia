using Carvia.Core.Models;
using Carvia.Features.Users;

namespace Carvia.Features.Authentication;

public interface IAuthenticationService
{
  Task<ReturnModel<AuthenticatedUserViewModel>> LoginAsync(
    LoginUserViewModel request,
    CancellationToken cancellationToken);

  Task<ReturnModel<CreatedUserViewModel>> RegisterAsync(
    RegisterUserViewModel request,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<AuthenticatedUserViewModel>> RefreshTokenAsync(
    string? refreshToken,
    CancellationToken cancellationToken);

  Task<ReturnModel<NoData>> RevokeRefreshTokenAsync(
    string? refreshToken,
    CancellationToken cancellationToken);
}
