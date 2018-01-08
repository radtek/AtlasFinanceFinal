using System;
using Atlas.Ass.Framework.Structures.Stream;

namespace Atlas.Ass.Structures.Stream
{
  public class ClientLoan:IClientLoan
  {
    public long LoanReference { get; set; }
    public string OperatorCode { get; set; }
    public long ClientReference { get; set; }
    public string EmployerNo { get; set; }
    public string Client { get; set; }
    public string Loan { get; set; }
    public string LegacyBranchNumber { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Cheque { get; set; }
    public decimal OutstandingAmount { get; set; }
    public string Surname { get; set; }
    public string FirstName { get; set; }
    public string OtherName { get; set; }
    public string Title { get; set; }
    public string IdentityNo { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string HomeTel { get; set; }
    public string WorkTel { get; set; }
    public string WorkFax { get; set; }
    public string SpouseWorkTel { get; set; }
    public string Cell { get; set; }
    public string Email { get; set; }
    public string ResidentialAddress1 { get; set; }
    public string ResidentialAddress2 { get; set; }
    public string ResidentialAddress3 { get; set; }
    public string ResidentialAddressPostalCode { get; set; }
    public string WorkAddress1 { get; set; }
    public string WorkAddress2 { get; set; }
    public string WorkAddress3 { get; set; }
    public string WorkAddress4 { get; set; }
    public string WorkAddress5 { get; set; }
    public string WorkAddress6 { get; set; }
    public string WorkAddressPostalCode { get; set; }
    public string LoanMethod { get; set; }
    public int LoanTerm { get; set; }
  }
}
