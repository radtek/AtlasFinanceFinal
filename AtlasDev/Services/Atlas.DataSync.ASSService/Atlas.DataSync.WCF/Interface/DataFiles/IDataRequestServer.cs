using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace Atlas.DataSync.WCF.Interface
{
  [ServiceContract(Namespace = "urn:Atlas/ASS/DataSync/Request/2013/08")]
  public interface IDataRequestServer
  {
    [OperationContract]
    ProcessStatus StartGetBranchDBFs(SourceRequest sourceRequest);

    [OperationContract]
    ProcessStatus StartGetBranchPSQL(SourceRequest sourceRequest);

    [OperationContract]
    ProcessStatus GetProcessStatus(SourceRequest sourceRequest, string transactionId);

    [OperationContract]
    ProcessStatus ProcessUploadedBranchZIPDBF(SourceRequest sourceRequest, string clientTransactionId, string fileName);
  }


  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface.DataFiles")]
  public class ProcessStatus
  {
    /// <summary>
    /// The unique transaction id
    /// </summary>
    [DataMember]
    public string TransactionId;

    /// <summary>
    /// Status of task
    /// </summary>
    [DataMember]
    public CurrentStatus Status;

    /// <summary>
    /// Error message
    /// </summary>
    [DataMember]
    public string ErrorMessage;

    /// <summary>
    /// Filename created
    /// </summary>
    [DataMember]
    public string Filename;


    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface.DataFiles")]
    public enum CurrentStatus
    {
      [EnumMember]
      Completed,
      [EnumMember]
      Failed,
      [EnumMember]
      NotSet,
      [EnumMember]
      Started
    };
  }

}
