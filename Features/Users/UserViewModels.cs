using System.ComponentModel.DataAnnotations;

namespace Carvia.Features.Users;

public sealed record CreatedUserViewModel(
  Guid Id,
  string Username,
  string Email);

public sealed record ShowcaseUserViewModel(
  Guid Id,
  string Username,
  string? ProfileImageUrl);

public class DetailedUserViewModel
{
  public Guid Id { get; set; }
  public string Username { get; set; } = null!;
  public string Email { get; set; } = null!;
  public string? ProfileImageUrl { get; set; }
  public string? Bio { get; set; }
  public bool IsActive { get; set; }
  public int CuratorItemCount { get; set; }
}

public class CreateUserViewModel
{
  [Required, StringLength(50)]
  public string Username { get; set; } = null!;
  [Required, EmailAddress, StringLength(150)]
  public string Email { get; set; } = null!;
  [Required, DataType(DataType.Password)]
  public string Password { get; set; } = null!;
  [DataType(DataType.Password), Compare("Password")]
  public string ConfirmPassword { get; set; } = null!;
  [StringLength(1000)]
  public string? Bio { get; set; }
  public IFormFile? ProfileImage { get; set; }
}

public class UpdateUserViewModel
{
  [Required]
  public Guid Id { get; set; }
  [Required, StringLength(50)]
  public string Username { get; set; } = null!;
  [Required, EmailAddress, StringLength(150)]
  public string Email { get; set; } = null!;
  [StringLength(1000)]
  public string? Bio { get; set; }
  public string? ExistingProfileImageUrl { get; set; }
  public IFormFile? NewProfileImage { get; set; }
}

public class LoginUserViewModel
{
  [Required, EmailAddress]
  public string Email { get; set; } = null!;
  [Required, DataType(DataType.Password)]
  public string Password { get; set; } = null!;
  public bool RememberMe { get; set; }
}
