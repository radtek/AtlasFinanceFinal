namespace Falcon.Models
{
  public class ResetPasswordModel
  {
    public string Username { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
  }
}