using System;

namespace Atlas.Ass.Framework.Structures.Stream
{
  public interface IClientLoan
  {
    long LoanReference { get; set; }
    string OperatorCode { get; set; }
    long ClientReference { get; set; }
    string EmployerNo { get; set; }
    string Client { get; set; }
    string Loan { get; set; }
    string LegacyBranchNumber { get; set; }
    DateTime LoanDate { get; set; }
    DateTime EndDate { get; set; }
    decimal Cheque { get; set; }
    decimal OutstandingAmount { get; set; }
    string Surname { get; set; }
    string FirstName { get; set; }
    string OtherName { get; set; }
    string Title { get; set; }
    string IdentityNo { get; set; }
    DateTime DateOfBirth { get; set; }
    string HomeTel { get; set; }
    string WorkTel { get; set; }
    string WorkFax { get; set; }
    string SpouseWorkTel { get; set; }
    string Cell { get; set; }
    string Email { get; set; }
    string ResidentialAddress1 { get; set; }
    string ResidentialAddress2 { get; set; }
    string ResidentialAddress3 { get; set; }
    string ResidentialAddressPostalCode { get; set; }
    string WorkAddress1 { get; set; }
    string WorkAddress2 { get; set; }
    string WorkAddress3 { get; set; }
    string WorkAddress4 { get; set; }
    string WorkAddress5 { get; set; }
    string WorkAddress6 { get; set; }
    string WorkAddressPostalCode { get; set; }
    string LoanMethod { get; set; }
    int LoanTerm { get; set; }
  }
}
