// -----------------------------------------------------------------------
// <copyright file="RiskEnquiry.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class BUR_StorageDTO
  {
    [DataMember]
    public Int64 StorageId { get; set; }
    [DataMember]
    public BUR_EnquiryDTO Enquiry { get; set; }
    [DataMember]
    public byte[] OriginalResponse { get; set; }
    [DataMember]
    public byte[] ResponseMessage { get; set; }
    [DataMember]
    public byte[] RequestMessage { get; set; }
  }
}
