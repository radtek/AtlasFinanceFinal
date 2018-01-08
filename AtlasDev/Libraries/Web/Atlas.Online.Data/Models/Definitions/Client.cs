using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class Client : XPLiteObject
  {
    private Application _currentApplication = null;

    [Key(AutoGenerate = true)]
    public int ClientId { get; set; }
    [Indexed(Unique=true)]
    public long UserId { get; set; }
    [Indexed]
    public long? PersonId { get; set; }
    [Persistent, Size(4)]
    [Indexed]
    public string Title { get; set; }
    [Persistent, Size(50)]
    public string Firstname { get; set; }
    [Persistent, Size(100)]
    public string Surname { get; set; }
    [Persistent, Size(13)]
    public string IDNumber { get; set; }
    [Persistent]
    public DateTime? DateOfBirth { get; set; }
    [Persistent]
    public char Gender { get; set; }
    [Persistent("MaritalStatusId")]
    public MaritalStatus MaritalStatus { get; set; }
    [Persistent]
    public Atlas.Online.Data.WebEnumerators.MaritalAgreement? MaritalAgreement { get; set; }
    [Persistent("EthnicityId")]
		public Ethnicity Ethnicity { get; set; }
		
    [Persistent]
    public bool OTPVerified { get; set; }

    [Association("Application", typeof(Application))]
    public XPCollection<Application> Applications { get { return GetCollection<Application>("Applications"); } }

    [Association("Contact", typeof(Contact))]
    public XPCollection<Contact> Contacts { get { return GetCollection<Contact>("Contacts"); } }

    [Association("ClientAddress", typeof(Address), UseAssociationNameAsIntermediateTableName = false)]
    public XPCollection<Address> Addresses { get { return GetCollection<Address>("Addresses"); } }

    [Association("ClientBankDetails", typeof(BankDetail), UseAssociationNameAsIntermediateTableName = true)]
    public XPCollection<BankDetail> BankDetails { get { return GetCollection<BankDetail>("BankDetails"); } }

    [Association]
    public XPCollection<OTPRequest> OTPRequests { get { return GetCollection<OTPRequest>("OTPRequests"); } }

    [NonPersistent]
    public string FullName
    {
      get
      {
        return String.Concat(this.Firstname, " ", this.Surname);
      }
    }

    public Client() : base() { }
    public Client(Session session) : base(session) { }

    [NonPersistent]
    public Application CurrentApplication
    {
      get
      {
        if (_currentApplication != null && _currentApplication.Client.ClientId == this.ClientId)
        {
          return _currentApplication;
        }

        return _currentApplication = Application.GetFirstBy(this.Session,
          x => x.Client.ClientId == this.ClientId &&
          x.IsCurrent);
      }
    }

    /// <summary>
    /// Creates or Updates a contact of a certain ContactType for this Client
    /// </summary>
    /// <param name="contactType">Contact type</param>
    /// <param name="value">Value of contact</param>
    /// <returns>true if change was made, otherwise false</returns>
    public bool SetContact(General.ContactType contactType, string value)
    {
      var contact = this.Contacts.FirstOrDefault(c => c.ContactType.ContactTypeId == Convert.ToInt32(contactType));
      if (contact == null)
      {
        contact = new Contact(Session)
        {
          ContactType = new XPQuery<ContactType>(Session).First(c => c.ContactTypeId == Convert.ToInt32(contactType)),
          Value = value
        };

        contact.Save();
        this.Contacts.Add(contact);
        return true;
      }
      
      if (contact.Value != value)
      {
        contact.Value = value;
        contact.Save();
        return true;
      }

      return false;
    }
  }
}