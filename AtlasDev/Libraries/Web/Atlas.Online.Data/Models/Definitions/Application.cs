using Atlas.Enumerators;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.Definitions
{
  public enum ApplicationStep
  {
    PersonalDetails = 1,
    EmployerDetails = 2,
    IncomeExpenses = 3,
    ConfirmVerify = 4,
    Verify = 5,
    QuoteAcceptance = 6
  }

  public sealed class Application : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int ApplicationId { get; set; }
    [Indexed]
    [Persistent("AffordabilityId")]
    public Affordability Affordability { get; set; }
    [Persistent, Size(20)]
    public string AccountNo { get; set; }
    [Indexed]
    [Persistent("NextOfKinId")]
    public NextOfKin NextOfKin { get; set; }
    [Indexed]
    [Persistent("ReasonId")]
    public LoanReason Reason { get; set; }
    [Indexed]
    public long? AccountId { get; set; }
    [Persistent("ClientId")]
    [Indexed]
    [Association("Application")]
    public Client Client { get; set; }
    [Indexed]
    [Persistent("EmployerId")]
    public Employer Employer { get; set; }
    [Persistent("BankDetailId")]
    [Indexed]
    public BankDetail BankDetail { get; set; }
    [Indexed]
    [Persistent("ResidentialAddressId")]
    public Address ResidentialAddress { get; set; }
    public decimal Amount { get; set; }
    public decimal RepaymentAmount { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal? OtherIncome { get; set; }
    [Indexed]
    [Persistent("SalaryTypeId")]
    public SalaryType SalaryType { get; set; }
    [Indexed]
    public int SalaryTypeNo { get; set; }
    [Indexed]
    public int Period { get; set; }
    [Indexed]
    public int Step { get; set; }
    [NonPersistent]
    public ApplicationStep CurrentStep { get { return (ApplicationStep)this.Step; } }
    [NonPersistent]
    public bool IsFinal
    {
      get
      {
        return (
          this.Status != Account.AccountStatus.Inactive &&
          this.Status != Account.AccountStatus.PreApproved
          );
      }
    }
    public bool IsCurrent { get; set; }
    [Indexed]
    public Account.AccountStatus Status { get; set; }
    [Indexed]
    [Persistent("SettlementId")]
    public ApplicationSettlement Settlement { get; set; }
    public DateTime RepaymentDate { get; set; }
    public string Hash { get; set; }
    public string IP { get; set; }
    public DateTime CreateDate { get; set; }

    [NonPersistent]
    public bool Expired
    {
      get
      {
        return DateTime.Now.Subtract(this.CreateDate).Days > 7;
      }
    }

    public Application() : base() { }
    public Application(Session session) : base(session) { }

    public static Application GetForUser(Session session, int userId, bool isCurrent = true)
    {
      return new XPQuery<Application>(session).FirstOrDefault(a =>
        a.Client.UserId == userId &&
        a.IsCurrent == isCurrent
      );
    }

    public static Application GetForUser(Session session, int userId, bool isCurrent = true, Account.AccountStatus status = Account.AccountStatus.Open)
    {
      return new XPQuery<Application>(session).FirstOrDefault(a =>
        a.Client.UserId == userId &&
        a.IsCurrent == isCurrent &&
        a.Status == status
      );
    }

    public static bool ExistsForUser(Session session, int userId, bool isCurrent = true, Account.AccountStatus status = Account.AccountStatus.Open)
    {
      return new XPQuery<Application>(session).Any(a =>
         a.Client.UserId == userId &&
         a.IsCurrent == isCurrent &&
         a.Status == status
       );
    }

    public static Application GetFirstBy(Session session, Expression<Func<Application, bool>> predicate)
    {
      return new XPQuery<Application>(session).FirstOrDefault(predicate);
    }

    public static void UpdateStatus(UnitOfWork unitOfWork, long applicationId, Account.AccountStatus status)
    {
      var application = new XPQuery<Application>(unitOfWork).FirstOrDefault(a => a.ApplicationId == applicationId);
      if (application != null)
      {
        application.Status = status;
        application.Save();
      }
    }

    public static void UpdateIsCurrent(UnitOfWork unitOfWork, long applicationId, bool isCurrent)
    {
      var application = new XPQuery<Application>(unitOfWork).FirstOrDefault(a => a.ApplicationId == applicationId);

      if (application != null)
      {
        application.IsCurrent = isCurrent;
        application.Save();
      }
    }
  }
}