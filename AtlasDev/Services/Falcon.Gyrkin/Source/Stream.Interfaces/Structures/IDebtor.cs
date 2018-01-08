using System;
using System.Collections.Generic;
using Falcon.Common.Interfaces.Structures;

namespace Stream.Framework.Structures
{
  public interface IDebtor
  {
    long DebtorId { get; set; }
    string IdNumber { get; set; }
    DateTime DateOfBirth { get; set; }
    string Title { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string OtherName { get; set; }
    long Reference { get; set; }
    string ThirdPartyReferenceNo { get; set; }
    string EmployerCode { get; set; }
    DateTime CreateDate { get; set; }
    List<IStreamAccount> Accounts { get; set; }
    List<IContact> Contacts { get; set; }
    List<IAddress> Addresses { get; set; }
  }
}