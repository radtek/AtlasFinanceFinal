using Atlas.Online.Web.Resources;
using Atlas.Online.Web.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Atlas.Online.Web.Models
{
  public class UsersContext : DbContext
  {
    public UsersContext()
      : base("DefaultConnection")
    {
    }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<WebPagesMembership> WebPagesMembership { get; set; }
  }

  [Table("UserProfile")]
  public class UserProfile
  {
    [Key]
    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }    
    public string Email { get; set; }
  }

  [Table("webpages_Membership")]
  public class WebPagesMembership
  {
    [Key]
    public int UserId { get; set; }
    public DateTime? CreateDate { get; set; }
    public string ConfirmationToken { get; set; }
    public bool? IsConfirmed { get; set; }
    public DateTime? LastPasswordFailureDate { get; set; }
    public int PasswordFailuresSinceLastSuccess { get; set; }
    public string Password { get; set; }
    public DateTime? PasswordChangedDate { get; set; }
    public string PasswordSalt { get; set; }
    public string PasswordVerificationToken { get; set; }
    public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
  }

  public class LocalPasswordModel
  {
    [Required]
    [NgDataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [NgDataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; }

    [NgDataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
  }

  public class LoginModel
  {
    [Required]
    [Display(Name = "Email Address")]
    public string Email { get; set; }

    [Required]
    [NgDataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
  }

  public class RegisterModel
  {
    [Required]
    [NgDataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address.")]
    [NgStringLength(35)]
    [UniqueEmail(ErrorMessageResourceName = "Validation_Unique", ErrorMessageResourceType = typeof(ErrorMessages))]
    [Display(Name = "Email Address")]
    public string Email { get; set; }

    [Required]
    [NgDataType(DataType.EmailAddress, ErrorMessage = "Please confirm your email address.")]
    [NgStringLength(35)]
    [Display(Name = "Confirm Email Address")]
    [Compare("Email", ErrorMessage = "Your email address does not match.")]
    public string EmailConfirm { get; set; }

    [Required]
    [NgStringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [NgDataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [NgDataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string PasswordConfirm { get; set; }

    [Required]
    [NgStringLength(15, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
    [Display(Name = "Name")]
    public string FirstName { get; set; }

    [Required]
    [NgStringLength(25, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
    [Display(Name = "Surname")]
    public string Surname { get; set; }

    [Required]
    [NgDataType(DataType.PhoneNumber, ErrorMessage = "Please enter a valid mobile phone number.")]
    [NgStringLength(13)]
    [UniqueCell(ErrorMessageResourceName = "Validation_Unique", ErrorMessageResourceType = typeof(ErrorMessages))]
    [RegularExpression(@"^[0-9\+\- \(\)]{10,}$", ErrorMessage = "Cell number must only contain numbers, +, -, ( or ) and be at least 10 characters.")]
    [Display(Name = "Cell Number")]
    public string Cell { get; set; }

    [Required]
    [NgDataType(DataType.PhoneNumber, ErrorMessage = "Please enter a valid mobile phone number.")]
    [NgStringLength(13)]
    [Display(Name = "Confirm Cell Number")]
    [Compare("Cell", ErrorMessage = "The cell number and confirmation cell do not match.")]
    public string CellConfirm { get; set; }

    [Required(ErrorMessage = "Please accept that you have read and understand the Privacy Policy.")]    
    public bool PolicyAccepted { get; set; }
  }

  public class ForgotPasswordModel
  {
    [Required]
    [NgDataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address.")]    
    [Display(Name = "Email Address")]
    public string Email { get; set; }

     //[Required]     
     //[Display(Name = "ID Number")]
     //public string IDNumber { get; set; }
  }

  public class LoginDetailsModel
  {
    [Required]
    [NgDataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address.")]
    [UniqueEmail(ErrorMessageResourceName = "Validation_Unique", ErrorMessageResourceType = typeof(ErrorMessages))]
    [ClientSide(ErrorMessageResourceName = "Validation_RemoteVerifyFailed", ErrorMessageResourceType = typeof(ErrorMessages), ValidationType = ClientSideValidationType.RemoteValidityFailed)]
    [Display(Name = "New email address")]
    public string Email { get; set; }

    [NgDataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string CurrentPassword { get; set; }

    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [NgDataType(DataType.Password)]
    [Display(Name = "New password")]
    public string Password { get; set; }

    [NgDataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string PasswordConfirm { get; set; }
  }

  public class ResetPasswordModel
  {
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [NgDataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [NgDataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string PasswordConfirm { get; set; }

    [Required]
    public string Token { get; set; }
  }
}
