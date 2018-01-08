using System.ComponentModel.DataAnnotations;

namespace Falcon.Areas.Security.Models
{
  public sealed class LockOutViewModel
  {

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Text)]
    [Display(Name = "Username")]
    public string UserName { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
  }
}