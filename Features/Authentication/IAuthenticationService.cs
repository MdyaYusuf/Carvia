using Carvia.Core.Models;
using Carvia.Features.Authentication;
using Carvia.Features.Users;
using System.Security.Claims;

public interface IAuthenticationService
{
  Task<ReturnModel<(AuthenticatedUserViewModel AuthResponse, ClaimsPrincipal Principal)>> LoginAsync(
    LoginUserViewModel request,
    CancellationToken cancellationToken);

  Task<ReturnModel<CreatedUserViewModel>> RegisterAsync(
    RegisterUserViewModel request,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<(AuthenticatedUserViewModel AuthResponse, ClaimsPrincipal Principal)>> RefreshTokenAsync(
    string? refreshToken,
    CancellationToken cancellationToken);

  Task<ReturnModel<NoData>> RevokeRefreshTokenAsync(
    string? refreshToken,
    CancellationToken cancellationToken);
}