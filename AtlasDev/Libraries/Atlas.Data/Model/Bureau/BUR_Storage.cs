// -----------------------------------------------------------------------
// <copyright file="RiskEnquiry.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class BUR_Storage : XPLiteObject
  {
    private Int64 _storageId;
    [Key(AutoGenerate = true)]
    public Int64 StorageId
    {
      get
      {
        return _storageId;
      }
      set
      {
        SetPropertyValue("StorageId", ref _storageId, value);
      }
    }

    private BUR_Enquiry _enquiry;
    [Indexed]
    [Persistent("EnquiryId")]
    [Association("BUR_StorageEnquiry")]
    public BUR_Enquiry Enquiry
    {
      get
      {

        return _enquiry;
      }
      set
      {
        SetPropertyValue("Enquiry", ref _enquiry, value);
      }
    }

    private byte[] _OriginalResponse;
    [Persistent]
    public byte[] OriginalResponse
    {
      get
      {

        return _OriginalResponse;
      }
      set
      {
        SetPropertyValue("OriginalResponse", ref _OriginalResponse, value);
      }
    }

    private byte[] _ResponseMessage;
    [Persistent]
    public byte[] ResponseMessage
    {
      get
      {

        return _ResponseMessage;
      }
      set
      {
        SetPropertyValue("ResponseMessage", ref _ResponseMessage, value);
      }
    }

    private byte[] _RequestMessage;
    [Size(int.MaxValue)]
    public byte[] RequestMessage
    {
      get
      {
        return _RequestMessage;
      }
      set
      {
        SetPropertyValue("RequestMessage", ref _RequestMessage, value);
      }
    }

    #region Constructors

    public BUR_Storage() : base() { }
    public BUR_Storage(Session session) : base(session) { }

    #endregion
  }
}
