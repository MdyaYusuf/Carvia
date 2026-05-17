using System.ComponentModel.DataAnnotations;

namespace Carvia.Features.Users;

public sealed record CreatedUserViewModel(
  Guid Id,
  string Username,
  string Email);

public sealed record ShowcaseUserViewModel(
  Guid Id,
  string Username,
  string? ProfileImageUrl,
  string? RoleName);

public class DetailedUserViewModel
{
  public Guid Id { get; set; }
  public string Username { get; set; } = null!;
  public string Email { get; set; } = null!;
  public string? ProfileImageUrl { get; set; }
  public string? Bio { get; set; }
  public bool IsActive { get; set; }
  public string? RoleName { get; set; }
  public int CuratorItemCount { get; set; }
}

public class UpdateUserViewModel
{
  [Required]
  public Guid Id { get; set; }
  [Required]
  [StringLength(50, MinimumLength = 3)]
  public string Username { get; set; } = null!;
  [Required]
  [EmailAddress]
  [StringLength(150)]
  public string Email { get; set; } = null!;
  [DataType(DataType.Password)]
  [Display(Name = "Current Password")]
  public string? CurrentPassword { get; set; }
  [DataType(DataType.Password)]
  [StringLength(100, MinimumLength = 12, ErrorMessage = "New password must be at least 12 characters.")]
  [Display(Name = "New Password")]
  public string? NewPassword { get; set; }
  [DataType(DataType.Password)]
  [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
  [Display(Name = "Confirm New Password")]
  public string? ConfirmNewPassword { get; set; }
}
