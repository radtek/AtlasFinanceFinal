using System;
using System.Collections.Generic;
using Falcon.Common.Interfaces.Structures;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class Debtor : IDebtor
  {
    public long DebtorId { get; set; }

    public string IdNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }
    public string OtherName { get; set; }

    public long Reference { get; set; }
    public string ThirdPartyReferenceNo { get; set; }
    public string EmployerCode { get; set; }
    public DateTime CreateDate { get; set; }
    public List<IStreamAccount> Accounts { get; set; }
    public List<IContact> Contacts { get; set; }
    public List<IAddress> Addresses { get; set; }
  }
}