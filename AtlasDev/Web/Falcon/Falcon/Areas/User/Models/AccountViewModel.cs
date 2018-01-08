using System;
using System.ComponentModel.DataAnnotations;

namespace Falcon.Areas.User.Models
{
  public class ManageUserViewModel
  {
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
  }

  public class LoginViewModel
  {
    [Required]
    [Display(Name = "User name")]
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
  }

  public class RegisterViewModel
  {
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; }

    [Required]
    [Display(Name = "Username")]
    public int IDNo { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Cell")]
    public string CellNo { get; set; }
  }

  public class ForgotPasswordModel
  {
    [Required]
    //[NgDataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; }
  }

  public class SaveUserModel
  {
    public long PersonId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
  }

  public class DeleteUserModel
  {
    public Guid UserId { get; set; }
  }

  public class ChangePasswordModel
  {
    [Required]
    [Display(Name = "Email Address")]
    public string Email { get; set; }
  }
}