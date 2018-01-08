using System;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace Atlas.WCF.Interface
{
  [ServiceContract]
  public interface IAssThirdParty
  {
    [OperationContract]
    Int64 AddUserOverride(AddUserOverrideArgs addUserOverrideArgs);
  }


  [DataContract]
  public class AddUserOverrideArgs
  {
    public AddUserOverrideArgs(DateTime startDate, DateTime endDate, string userOperatorCode, string branchNum,
      string regionalOperatorId, byte newLevel, string reason)
    {
      StartDate = startDate;
      EndDate = endDate;
      UserOperatorCode = userOperatorCode;
      BranchNum = branchNum;
      RegionalOperatorId = regionalOperatorId;
      NewLevel = newLevel;
      Reason = reason;
    }


    [DataMember]
    public DateTime StartDate { get; set; }

    [DataMember]
    public DateTime EndDate { get; set; }

    [DataMember]
    public string UserOperatorCode { get; set; }

    [DataMember]
    public string BranchNum { get; set; }

    [DataMember]
    public string RegionalOperatorId { get; set; }

    [DataMember]
    public byte NewLevel { get; set; }

    [DataMember]
    public string Reason { get; set; }
  }

}

