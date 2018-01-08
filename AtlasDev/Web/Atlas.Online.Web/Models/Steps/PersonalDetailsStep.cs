using Atlas.Enumerators;
using Atlas.Online.Data;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Helpers;
using Atlas.Online.Web.Models.Dto;
using Atlas.Online.Web.Resources;
using Atlas.Online.Web.Validations;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Atlas.Online.Web.Models.Steps
{
  public class PersonalDetailsStep : ApplicationStepBase
  {
    public override int Id
    {
      get { return 1; }
    }

    [Required(ErrorMessageResourceName="Validation_Required", ErrorMessageResourceType=typeof(ErrorMessages))]
    [RegularExpression(@"^[\d]{13}$", ErrorMessageResourceName="Validation_IdNumberRegex", ErrorMessageResourceType=typeof(ErrorMessages))]
    [UniqueIdNumber(ErrorMessageResourceName="Validation_Unique", ErrorMessageResourceType=typeof(ErrorMessages))]
    [ClientSide(ErrorMessageResourceName="Validation_RemoteVerifyFailed", ErrorMessageResourceType=typeof(ErrorMessages), ValidationType = ClientSideValidationType.RemoteValidityFailed)]
    [Display(Name = "Id Number")]
    public string IDNumber { get; set; }

    [Required]
    [NgStringLength(25)]
    [Display(Name="Address")]
    public string Address1 { get; set; }
    [Required]
    [NgStringLength(25)]
    [Display(Name = "Address")]
    public string Address2 { get; set; }
    //[Required]
    [NgStringLength(25)]
    [Display(Name = "Address")]
    public string Address3 { get; set; }
    [Required]
    [NgStringLength(25)]
    [Display(Name = "City / Town")]
    public string City { get; set; }
    [Required]
    public General.Province Province { get; set; }
    [Required]
    [RegularExpression(@"[0-9]{4}", ErrorMessage="Postal code must be 4 numeric characters.")]
    [NgStringLength(5)]
    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; }

    [Required]
    [EnumRequired(WebEnumerators.MaritalStatus.NotSet)]
    [Display(Name="Marital Status")]
    public WebEnumerators.MaritalStatus MaritalStatus { get; set; }
    [Display(Name = "Martial Agreement")]
    public WebEnumerators.MaritalAgreement? MaritalAgreement { get; set; }
    
    [Required]
    [RegularExpression(@"^[0-9]{10,}$", ErrorMessage = "{0} must only contain numbers and be at least 10 characters.")]
    [NgStringLength(20)]
    [Display(Name = "Home Telephone")]
    public string HomeTelephone { get; set; }

    [Required]
    [NgStringLength(15, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
    [Display(Name = "Next of kin Name")]
    public string FirstName { get; set; }

    [Required]
    [NgStringLength(25, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
    [Display(Name = "Next of kin Surname")]
    public string Surname { get; set; }

    [Required]
    [EnumRequired(General.RelationType.NotSet)]
    [Display(Name="Next of kin relationship")]
    public General.RelationType NextOfKinRelation { get; set; }

    [Required]
    [RegularExpression(@"^[0-9]{10,}$", ErrorMessage = "{0} must only contain numbers and be at least 10 characters.")]
    [NgStringLength(20)]
    [Display(Name = "Next of kin contact number")]
    public string NextOfKinContactNumber { get; set; }

    [Display(Name="Ethnic group")]
    public WebEnumerators.EthnicGroup EthnicGroup { get; set; }    

    [Required]
    public BankDetailDto BankDetail { get; set; }

    public override void AfterCommit(Application application)
    {
      using (var services = new SharedServices())
      {
        var client = services.CurrentClient;

        // Submit AVS -- Fabian -- Removd in order to change the order of the process
       // services.WebServiceClient.AVS_SubmitAsync(client.ClientId);
      }
    }
    
    public override void Populate(Application application)
    {      
      IDNumber = application.Client.IDNumber;
      
      #region Address
      if (application.ResidentialAddress != null)
      {
        Address1 = application.ResidentialAddress.AddressLine1;
        Address2 = application.ResidentialAddress.AddressLine2;
        Address3 = application.ResidentialAddress.AddressLine3;
        City = application.ResidentialAddress.AddressLine4;
        PostalCode = application.ResidentialAddress.PostalCode;
        Province = application.ResidentialAddress.Province != null ?
          (General.Province)application.ResidentialAddress.Province.ProvinceId
          : General.Province.NotSet;
      }
      else
      {
        var address = application.Client.Addresses.FirstOrDefault(x => x.AddressType.AddressTypeId == Convert.ToInt32(General.AddressType.Residential));
        if (address != null)
        {
          Address1 = address.AddressLine1;
          Address2 = address.AddressLine2;
          Address3 = address.AddressLine3;
          City = address.AddressLine4;
          PostalCode = address.PostalCode;
          Province = address.Province != null ? (General.Province)address.Province.ProvinceId: General.Province.NotSet;
        }
      }
      #endregion

      #region Bank Detail
      BankDetail = BankDetailDto.Create(application, x => x.IsEnabled);
      #endregion

      #region Contacts
      var tel = application.Client.Contacts
        .FirstOrDefault(c => c.ContactType.ContactTypeId == Convert.ToInt32(General.ContactType.TelNoHome));
      HomeTelephone = (tel != null) ? tel.Value : String.Empty;
      #endregion

      #region Marital
      if (application.Client.MaritalStatus != null)
      {
        MaritalStatus = application.Client.MaritalStatus.Type;
      }
      MaritalAgreement = application.Client.MaritalAgreement;
      #endregion

      #region Next of kin
      if (application.NextOfKin != null) 
      {
        FirstName = application.NextOfKin.FirstName;
        Surname = application.NextOfKin.Surname;
        NextOfKinContactNumber = application.NextOfKin.ContactNo;
        NextOfKinRelation = application.NextOfKin.Relation;
      }
      #endregion

      #region Ethnicity
      if (application.Client.Ethnicity != null)
      {
        EthnicGroup = application.Client.Ethnicity.Type;
      }
      #endregion
    }

    public override void Save(ref Application application, HttpRequestBase request)
    {
      var session = (UnitOfWork)application.Session;

      // Only save if Client doesnt have an existing ID number
      if (string.IsNullOrEmpty(application.Client.IDNumber))
      {
        application.Client.IDNumber = IDNumber;
        application.Client.DateOfBirth = new Atlas.Common.Utils.IDValidator(IDNumber).GetDateOfBirthAsDateTime();
      }

      #region Address
      // Address
      var address = application.ResidentialAddress;
      if (address == null) 
      {         
        address = application.Client.Addresses.FirstOrDefault(x => x.AddressType.Type == General.AddressType.Residential);
        if (address == null || address.IsDeleted)
        {
          address = new Address(session);
          
          application.Client.Addresses.Add(address);
        }
      }
      address.AddressLine1 = Address1;
      address.AddressLine2 = Address2;
      address.AddressLine3 = Address3;
      address.AddressLine4 = City;
      //Edited By Prashant
      address.AddressType = new XPQuery<AddressType>(session).FirstOrDefault(a => a.AddressTypeId == (int)General.AddressType.Residential); //new XPQuery<AddressType>(session).FirstOrDefault(a => a.Type == General.AddressType.Residential);
      address.Province = new XPQuery<Province>(session).FirstOrDefault(x => x.ProvinceId == Convert.ToInt32(Province));
      address.PostalCode = PostalCode;
      address.Client = application.Client;
      application.ResidentialAddress = address;
      application.ResidentialAddress.Save();
      #endregion     

      #region Banking Details
      BankDetail.SaveApplication(ref application);     
      #endregion

      #region Ethnicity
      application.Client.Ethnicity = new XPQuery<Ethnicity>(session).FirstOrDefault(x => x.EthnicityId == Convert.ToInt32(EthnicGroup));
      #endregion

      #region Contact
      application.Client.SetContact(General.ContactType.TelNoHome, HomeTelephone);
      #endregion

      #region Marital Status
      application.Client.MaritalStatus = new XPQuery<MaritalStatus>(session).FirstOrDefault(x => x.MaritalStatusId == Convert.ToInt32(MaritalStatus));
      application.Client.MaritalAgreement = MaritalAgreement;
      #endregion

      #region Next of kin
      if (application.NextOfKin == null)
      {
        application.NextOfKin = new NextOfKin(session);
      }

      application.NextOfKin.FirstName = FirstName;
      application.NextOfKin.Surname = Surname;
      application.NextOfKin.ContactNo = NextOfKinContactNumber;
      application.NextOfKin.Relation = NextOfKinRelation;

      application.NextOfKin.Save();
      #endregion

      application.Client.Save();
      application.IP = string.IsNullOrEmpty(application.IP) ? IP.GetIP(request) : application.IP;
      application.Save();      
    }
  }
}