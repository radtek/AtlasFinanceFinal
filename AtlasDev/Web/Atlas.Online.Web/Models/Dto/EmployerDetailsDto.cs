using Atlas.Enumerators;
using Atlas.Online.Data;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Validations;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Atlas.Common.Extensions;

namespace Atlas.Online.Web.Models.Dto
{
  public class EmployerDetailsDto
  {
    [Required]
    [NgStringLength(25)]
    [Display(Name = "Name of company")]
    public string NameOfCompany { get; set; }

    [Required]
    [RegularExpression(@"^[0-9]{9,}$", ErrorMessage = "{0} must only contain numbers and be at least 9 characters.")]
    [NgStringLength(20)]
    [Display(Name = "Work number")]
    public string WorkNumber { get; set; }

    [Required]
    [RegularExpression(@"^[0-9]{9,}$", ErrorMessage = "{0} must only contain numbers and be at least 9 characters.")]
    [NgStringLength(20)]
    [Display(Name = "HR Contact number")]
    public string ContactNo { get; set; }

    [Required]
    [NgStringLength(25)]
    [Display(Name = "Address")]
    public string AddressLine1 { get; set; }
    [Required]
    [NgStringLength(25)]
    public string AddressLine2 { get; set; }

    [Required]
    [NgStringLength(25)]
    [Display(Name = "City / Town")]
    public string City { get; set; }
    [Required]
    public General.Province Province { get; set; }
    [Required]
    [RegularExpression(@"[0-9]{4}", ErrorMessage = "Postal code must be 4 numeric characters.")]
    [NgStringLength(5)]
    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; }
    [Required]
    public WebEnumerators.Industry Industry { get; set; }

    public static EmployerDetailsDto Create(Application application)
    {
      var result = new EmployerDetailsDto();

      result.NameOfCompany = application.Employer.Name;
      result.ContactNo = application.Employer.ContactNo;

      #region Employer address
      var address = application.Employer.Address;
      if (address != null)
      {
        result.AddressLine1 = address.AddressLine1;
        result.AddressLine2 = address.AddressLine2;
        result.City = address.AddressLine4;
        result.Province = (General.Province)address.Province.ProvinceId;
        result.PostalCode = address.PostalCode;
      }
      #endregion

      #region Industry
      if (application.Employer.Industry != null)
      {
        result.Industry = application.Employer.Industry.Type;
      }
      #endregion  

      #region Work Number
      var tel = application.Client.Contacts.FirstOrDefault(
        c => c.ContactType.ContactTypeId == Convert.ToInt32(General.ContactType.TelNoWork)
      );
      result.WorkNumber = (tel != null) ? tel.Value : String.Empty;
      #endregion
    
      return result;
    }

    public void SaveApplication(ref Application application)
    {
      var session = application.Session;
      var employer = application.Employer;
      if (employer == null)
      {
        application.Employer = employer = new Employer(session);
      }

      employer.ContactNo = ContactNo;
      employer.Name = NameOfCompany;

      #region Industry
        application.Employer.Industry = new XPQuery<Industry>(application.Session).FirstOrDefault(x => x.IndustryId == Convert.ToInt32(Industry));
      #endregion

      #region Address
      // Address
      var address = employer.Address;
      if (address == null) { address = new Address(session); }
      address.AddressLine1 = AddressLine1;
      address.AddressLine2 = AddressLine2;
      address.AddressLine4 = City;
      //Edited By Prashant
      address.AddressType = new XPQuery<AddressType>(session).First(a => a.AddressTypeId == (int)General.AddressType.Employer); //new XPQuery<AddressType>(session).First(a => a.Type == General.AddressType.Employer);
      address.Client = application.Client;

      address.Province = new XPQuery<Province>(session).First(a => a.ProvinceId== Province.ToInt());
      address.PostalCode = PostalCode;

      employer.Address = address;
      employer.Address.Save();
      #endregion

      #region Contact
      var contact = application.Client.Contacts.FirstOrDefault(c => c.ContactType.ContactTypeId == Convert.ToInt32(General.ContactType.TelNoWork));
      if (contact == null)
      {
        contact = new Contact(session)
        {
          ContactType = new XPQuery<ContactType>(session).First(c => c.ContactTypeId == Convert.ToInt32(General.ContactType.TelNoWork)),
          Value = WorkNumber
        };

        contact.Save();
        application.Client.Contacts.Add(contact);
      }
      else if (contact.Value != WorkNumber)
      {
        contact.Value = WorkNumber;
        contact.Save();
      }
      #endregion

      employer.Save();
    }

  }
}