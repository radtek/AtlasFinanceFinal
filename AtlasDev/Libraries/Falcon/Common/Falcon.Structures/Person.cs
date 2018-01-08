using System;
using System.Collections.Generic;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public sealed class Person : IPerson
  {
    public long PersonId { get; set; }
    public string Username { get; set; }

    public IBranch Branch { get; set; }

    public string LegacyClientCode { get; set; }

    public string ClientCode { get; set; }

    public string Firstname { get; set; }

    public string Middlename { get; set; }

    public string Lastname { get; set; }

    public string Othername { get; set; }
    public string Designation { get; set; }
    public General.EthnicGroup EthnicGroup { get; set; }

    public string Email { get; set; }
    public string IdNum { get; set; }

    public string SalaryFrequency { get; set; }

    public string Gender { get; set; }

    public string Race { get; set; }

    public DateTime DateOfBirth { get; set; }
    public List<WebRole> Roles { get; set; }
    public string WebReference { get; set; }

    public bool WebLinked { get; set; }

    public string FullName { get; set; }
  }
}
