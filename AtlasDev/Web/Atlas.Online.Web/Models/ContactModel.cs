using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Models
{
  public enum PreferredContact
  {
    Email,
    Call
  }

  public class ContactModel
  {
    [Required]
    [Display(Name = "Name")]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Cell Number")]
    public string CellNo { get; set; }

    [Required]
    public PreferredContact PreferredContact { get; set; }

    [Display(Name = "Question")]
    [Required]
    public string Question { get; set; }
  }
}