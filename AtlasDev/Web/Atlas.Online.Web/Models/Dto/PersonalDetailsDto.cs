using Atlas.Enumerators;
using Atlas.Online.Data;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Resources;
using Atlas.Online.Web.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Models.Dto
{
  public class PersonalDetailsDto
  {
    [Required]
    [RegularExpression(@"^[0-9]{9,}$", ErrorMessage = "{0} must only contain numbers, - and be at least 9 characters.")]
    [UniqueCell(ErrorMessageResourceName = "Validation_Unique", ErrorMessageResourceType = typeof(ErrorMessages))]
    [ClientSide(ErrorMessageResourceName = "Validation_RemoteVerifyFailed", ErrorMessageResourceType = typeof(ErrorMessages), ValidationType = ClientSideValidationType.RemoteValidityFailed)]
    [Display(Name = "Cell Number")]
    public string CellNo { get; set; }

    [Required]
    [Display(Name = "Address Line 1")]
    public string Address1 { get; set; }
    [Required]
    [Display(Name = "Address Line 2")]
    public string Address2 { get; set; }
    [Display(Name = "Address Line 3")]
    public string Address3 { get; set; }

    [Required]
    [Display(Name = "City / Town")]
    public string City { get; set; }

    [Required]
    public Enumerators.General.Province Province { get; set; }

    [Required]
    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; }

    public static PersonalDetailsDto Create(Client client)
    {
      var result = new PersonalDetailsDto();
      
      var contact = client.Contacts.FirstOrDefault(x => x.ContactType.ContactTypeId == Convert.ToInt32(General.ContactType.CellNo));
      if (contact != null) 
      {
        result.CellNo = contact.Value;
      }      

      var address = client.Addresses.FirstOrDefault(x => x.AddressType.AddressTypeId == Convert.ToInt32(General.AddressType.Residential));
      if (address != null) 
      {
        result.Address1 = address.AddressLine1;
        result.Address2 = address.AddressLine2;
        result.Address3 = address.AddressLine3;
        result.City = address.AddressLine4;
        result.Province = (address.Province != null) ? (General.Province)address.Province.ProvinceId : default(General.Province);
        result.PostalCode = address.PostalCode;
      }

      return result;
    }
  }
}