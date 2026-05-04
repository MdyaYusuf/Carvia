using Carvia.Core.Models;
using Carvia.Core.Persistence;
using Carvia.Core.Utilities.Security;
using Carvia.Features.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Carvia.Features.Authentication;

public class AuthenticationService(
  IUserRepository _userRepository,
  UserBusinessRules _userBusinessRules,
  AuthenticationBusinessRules _authBusinessRules,
  UserMapper _userMapper,
  IUnitOfWork _unitOfWork,
  IOptions<TokenOptions> _tokenOptions,
  IHttpContextAccessor _httpContextAccessor) : IAuthenticationService
{
  private readonly TokenOptions _options = _tokenOptions.Value;
  private const string CookieName = "refreshToken";

  public async Task<ReturnModel<AuthenticatedUserViewModel>> LoginAsync(
    LoginUserViewModel request,
    CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetAsync(
      u => u.Email == request.Email,
      enableTracking: true,
      cancellationToken: cancellationToken);

    _authBusinessRules.UserCredentialsMustMatch(user, request.Password);
    _userBusinessRules.UserMustBeActive(user!);

    var accessToken = CreateToken(user!, out DateTime expiration);
    user!.RefreshToken = GenerateRefreshToken();
    user.RefreshTokenExpiration = DateTime.Now.AddDays(_options.RefreshTokenExpiration);

    SetRefreshTokenCookie(user.RefreshToken, user.RefreshTokenExpiration.Value);

    _userRepository.Update(user);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var userShowcase = _userMapper.EntityToShowcaseViewModel(user);
    var authResponse = new AuthenticatedUserViewModel(accessToken, expiration, user.RefreshToken, userShowcase);

    return new ReturnModel<AuthenticatedUserViewModel>
    {
      Data = authResponse,
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

    HashingHelper.CreatePasswordHash(request.Password, out string hash, out string key);

    var user = new User
    {
      Username = request.Username,
      Email = request.Email,
      PasswordHash = hash,
      PasswordKey = key,
      IsActive = true
    };

    await _userRepository.AddAsync(user, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var createdUserResponse = _userMapper.EntityToCreatedViewModel(user);

    return new ReturnModel<CreatedUserViewModel>
    {
      Success = true,
      Message = "Kullanıcı başarıyla kaydedildi.",
      Data = createdUserResponse,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<AuthenticatedUserViewModel>> RefreshTokenAsync(
    string? refreshToken,
    CancellationToken cancellationToken)
  {
    var token = refreshToken ?? _httpContextAccessor.HttpContext?.Request.Cookies[CookieName];

    var user = await _userRepository.GetAsync(
      u => u.RefreshToken == token,
      enableTracking: true,
      cancellationToken: cancellationToken);

    _authBusinessRules.RefreshTokenMustBeValid(user);

    var accessToken = CreateToken(user!, out DateTime expiration);

    if (user!.RefreshTokenExpiration <= DateTime.Now.AddDays(1))
    {
      user.RefreshToken = GenerateRefreshToken();
      user.RefreshTokenExpiration = DateTime.Now.AddDays(_options.RefreshTokenExpiration);
      SetRefreshTokenCookie(user.RefreshToken, user.RefreshTokenExpiration.Value);
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var userShowcase = _userMapper.EntityToShowcaseViewModel(user);
    var authResponse = new AuthenticatedUserViewModel(accessToken, expiration, user.RefreshToken!, userShowcase);

    return new ReturnModel<AuthenticatedUserViewModel>
    {
      Data = authResponse,
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
      Message = "Oturum başarıyla sonlandırıldı.",
      StatusCode = 200
    };
  }

  private void SetRefreshTokenCookie(
    string token,
    DateTime expires)
  {
    var cookieOptions = new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = expires,
      Path = "/"
    };

    _httpContextAccessor.HttpContext?.Response.Cookies.Append(CookieName, token, cookieOptions);
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
    byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

    return Convert.ToBase64String(randomBytes);
  }

  private string CreateToken(
    User user,
    out DateTime expiration)
  {
    var claims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Email, user.Email),
      new(ClaimTypes.Name, user.Username)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
    expiration = DateTime.Now.AddMinutes(_options.AccessTokenExpiration);

    var token = new JwtSecurityToken(
      issuer: _options.Issuer,
      audience: _options.Audience,
      claims: claims,
      expires: expiration,
      signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
