// -----------------------------------------------------------------------
// <copyright file="Request.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Atlas.ThirdParty.Fraud.TransUnion.Fraud
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Runtime.Serialization;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  [DataContract]
  public sealed class Request
  {
    [DataMember]
    public string Firstname
    {
      get;
      set;
    }
    [DataMember]
    public string Surname
    {
      get;
      set;
    }
    [DataMember]
    public string IdentityNo
    { get; set; }
    [DataMember]
    public Enumerators.General.Gender Gender
    {
      get;
      set;
    }
    [DataMember]
    public DateTime DateOfBirth
    {
      get;
      set;
    }
    [DataMember]
    public string AddressLine1
    {
      get;
      set;
    }
    [DataMember]
    public string AddressLine2
    {
      get;
      set;
    }
    [DataMember]
    public string AddressLine3
    {
      get;
      set;
    }
    [DataMember]
    public string AddressLine4
    {
      get;
      set;
    }
    [DataMember]
    public string PostalCode
    {
      get;
      set;
    }
  }
}
