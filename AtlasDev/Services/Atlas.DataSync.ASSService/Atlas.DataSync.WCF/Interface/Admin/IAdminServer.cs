using System;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace Atlas.DataSync.WCF.Interface
{
  [ServiceContract(Namespace = "urn:Atlas/ASS/DataSync/Admin/2013/08")]
  public interface IAdminServer
  {
    [OperationContract]
    byte[] GetMasterTableData(SourceRequest sourceRequest, string masterTableName);
    
    [OperationContract]
    bool UploadMasterTableRow(SourceRequest sourceRequest, string masterTableName, byte[] dataTable);

    [OperationContract]
    bool XHarbourConvertedAMasterTable(SourceRequest sourceRequest, string tableName);

    [OperationContract]
    bool UpgradeDatabase(SourceRequest sourceRequest, string version, string description, string sqlUpdateScript);
    
    [OperationContract]
    void UpdatedBranchServer(SourceRequest sourceRequest, string legacyBranchNum);
  }


  [ServiceContract(Namespace="http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface.Admin")]
  public class CreateNewBranch
  {
    [DataMember]
    public string Name;
    [DataMember]
    public string LegacyBranchNum;
    [DataMember]
    public DateTime OpenDT;
    [DataMember]
    public string Comment;
    [DataMember]
    public Int64 CompanyId;
    [DataMember]
    public Int64 BranchId;
    [DataMember]
    public string ErrorMessage;
  }   
}