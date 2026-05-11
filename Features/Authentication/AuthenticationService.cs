using Carvia.Core.Exceptions;
using Carvia.Core.Models;
using Carvia.Core.Persistence;
using Carvia.Core.Utilities.Security;
using Carvia.Features.Roles;
using Carvia.Features.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Carvia.Features.Authentication;

public class AuthenticationService(
  IUserRepository _userRepository,
  IRoleRepository _roleRepository,
  UserBusinessRules _userBusinessRules,
  AuthenticationBusinessRules _authBusinessRules,
  UserMapper _userMapper,
  IUnitOfWork _unitOfWork,
  IOptions<TokenOptions> _tokenOptions,
  IHttpContextAccessor _httpContextAccessor) : IAuthenticationService
{
  private readonly TokenOptions _options = _tokenOptions.Value;
  private const string CookieName = "refreshToken";

  public async Task<ReturnModel<(AuthenticatedUserViewModel, ClaimsPrincipal)>> LoginAsync(
    LoginUserViewModel request,
    CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetAsync(
      u => u.Email == request.Email,
      include: q => q.Include(u => u.Role),
      enableTracking: true,
      cancellationToken: cancellationToken);

    _authBusinessRules.UserCredentialsMustMatch(user, request.Password);
    _userBusinessRules.UserMustBeActive(user!);

    var accessToken = CreateToken(user!, out DateTime expiration);
    var principal = CreatePrincipal(user!);

    user!.RefreshToken = GenerateRefreshToken();
    user.RefreshTokenExpiration = DateTime.Now.AddDays(_options.RefreshTokenExpiration);
    SetRefreshTokenCookie(user.RefreshToken, user.RefreshTokenExpiration.Value);

    _userRepository.Update(user);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var userShowcase = _userMapper.EntityToShowcaseViewModel(user);
    var authResponse = new AuthenticatedUserViewModel(accessToken, expiration, user.RefreshToken!, userShowcase);

    return new ReturnModel<(AuthenticatedUserViewModel, ClaimsPrincipal)>
    {
      Data = (authResponse, principal),
      Success = true,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedUserViewModel>> RegisterAsync(
    RegisterUserViewModel request,
    CancellationToken cancellationToken = default)
  {
    await _userBusinessRules.UserEmailMustBeUniqueAsync(request.Email, cancellationToken: cancellationToken);
    await _userBusinessRules.UsernameMustBeUniqueAsync(request.Username, cancellationToken: cancellationToken);

    var defaultRole = await _roleRepository.GetAsync(r => r.Name == "Admin", cancellationToken: cancellationToken)
        ?? throw new BusinessException("Default role not found.");

    HashingHelper.CreatePasswordHash(request.Password, out string hash, out string key);

    var user = new User
    {
      Username = request.Username,
      Email = request.Email,
      PasswordHash = hash,
      PasswordKey = key,
      IsActive = true,
      RoleId = defaultRole.Id
    };

    await _userRepository.AddAsync(user, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<CreatedUserViewModel>
    {
      Success = true,
      Message = "User registered successfully.",
      Data = _userMapper.EntityToCreatedViewModel(user),
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<(AuthenticatedUserViewModel, ClaimsPrincipal)>> RefreshTokenAsync(
    string? refreshToken,
    CancellationToken cancellationToken)
  {
    var token = refreshToken ?? _httpContextAccessor.HttpContext?.Request.Cookies[CookieName];

    var user = await _userRepository.GetAsync(
      u => u.RefreshToken == token,
      include: q => q.Include(u => u.Role),
      enableTracking: true,
      cancellationToken: cancellationToken);

    _authBusinessRules.RefreshTokenMustBeValid(user);

    var accessToken = CreateToken(user!, out DateTime expiration);
    var principal = CreatePrincipal(user!);

    if (user!.RefreshTokenExpiration <= DateTime.Now.AddDays(1))
    {
      user.RefreshToken = GenerateRefreshToken();
      user.RefreshTokenExpiration = DateTime.Now.AddDays(_options.RefreshTokenExpiration);
      SetRefreshTokenCookie(user.RefreshToken, user.RefreshTokenExpiration.Value);
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var authResponse = new AuthenticatedUserViewModel(accessToken, expiration, user.RefreshToken!, _userMapper.EntityToShowcaseViewModel(user));

    return new ReturnModel<(AuthenticatedUserViewModel, ClaimsPrincipal)>
    {
      Data = (authResponse, principal),
      Success = true,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RevokeRefreshTokenAsync(
    string? refreshToken,
    CancellationToken cancellationToken)
  {
    var token = refreshToken ?? _httpContextAccessor.HttpContext?.Request.Cookies[CookieName];
    var user = await _userRepository.GetAsync(u => u.RefreshToken == token);

    _authBusinessRules.RefreshTokenUserMustExist(user);

    user!.RefreshToken = null;
    user.RefreshTokenExpiration = null;
    DeleteRefreshTokenCookie();

    _userRepository.Update(user);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Logged out.",
      StatusCode = 200
    };
  }
  private ClaimsPrincipal CreatePrincipal(User user)
  {
    var claims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Email, user.Email),
      new(ClaimTypes.Name, user.Username),
      new(ClaimTypes.Role, user.Role.Name)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    return new ClaimsPrincipal(identity);
  }

  private void SetRefreshTokenCookie(string token, DateTime expires)
  {
    _httpContextAccessor.HttpContext?.Response.Cookies.Append(CookieName, token, new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = expires,
      Path = "/"
    });
  }

  private void DeleteRefreshTokenCookie()
  {
    _httpContextAccessor.HttpContext?.Response.Cookies.Delete(CookieName, new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Path = "/"
    });
  }

  private string GenerateRefreshToken()
  {
    return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
  }

  private string CreateToken(User user, out DateTime expiration)
  {
    var claims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Email, user.Email),
      new(ClaimTypes.Name, user.Username),
      new(ClaimTypes.Role, user.Role.Name)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
    expiration = DateTime.Now.AddMinutes(_options.AccessTokenExpiration);

    var token = new JwtSecurityToken(_options.Issuer, _options.Audience, claims, expires: expiration, signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}