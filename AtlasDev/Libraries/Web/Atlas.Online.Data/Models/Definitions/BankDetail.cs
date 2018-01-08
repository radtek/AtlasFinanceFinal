using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.Definitions
{
  public class BankDetail : XPLiteObject
  {
    [Key( AutoGenerate = true)]
    public int BankDetailId { get; set; }
    [Indexed]
    [Persistent]
    public long? TransactionId { get; set; }
    [Indexed]
    [Persistent]
    public long? ReferenceId { get; set; }
    [Indexed]
    [Persistent("BankId")]
    public Bank Bank { get; set; }    
    [Persistent("AccountTypeId")]
    public BankAccountType AccountType { get; set; }
    [Persistent("PeriodId")]
    public BankPeriod Period { get; set; }
    [Persistent, Size(20)]
    public string AccountNo { get; set; }
    [Persistent, Size(100)]
    public string AccountName { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreateDate { get; set; }

    [Association("ClientBankDetails", typeof(Client), UseAssociationNameAsIntermediateTableName = true)]
    public XPCollection<Client> Clients { get { return GetCollection<Client>("Clients"); } }

    public BankDetail() : base() { }
    public BankDetail(Session session) : base(session) { }
  }
}