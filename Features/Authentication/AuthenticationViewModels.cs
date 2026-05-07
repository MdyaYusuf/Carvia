using Carvia.Features.Users;
using System.ComponentModel.DataAnnotations;

namespace Carvia.Features.Authentication;

public sealed record AuthenticatedUserViewModel(
  string AccessToken,
  DateTime Expiration,
  string RefreshToken,
  ShowcaseUserViewModel User);

public class RegisterUserViewModel
{
  [Required, StringLength(50)]
  public string Username { get; set; } = null!;

  [Required, EmailAddress, StringLength(150)]
  public string Email { get; set; } = null!;

  [Required, DataType(DataType.Password)]
  public string Password { get; set; } = null!;

  [DataType(DataType.Password), Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
  public string ConfirmPassword { get; set; } = null!;
}

public class LoginUserViewModel
{
  [Required, EmailAddress]
  public string Email { get; set; } = null!;
  [Required, DataType(DataType.Password)]
  public string Password { get; set; } = null!;
}

public class RefreshTokenViewModel
{
  [Required]
  public string RefreshToken { get; set; } = null!;
}
