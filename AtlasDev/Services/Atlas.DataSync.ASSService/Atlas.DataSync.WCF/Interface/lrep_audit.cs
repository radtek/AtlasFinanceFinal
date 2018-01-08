using System;
using System.Runtime.Serialization;


namespace Atlas.DataSync.WCF.Interface
{ 
  [DataContract(Name = "SourceRequest", Namespace = "http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface")]
  [Serializable]
  public class lrep_audit
  {
    public lrep_audit()
    {

    }

    #region Public properties

    [DataMember]
    public long event_id { get; set; }
    [DataMember]
    public string table_name { get; set; }
    [DataMember]
    public string oper { get; set; }
    [DataMember]
    public decimal sr_recno { get; set; }
    [DataMember]
    public int relid { get; set; }
    [DataMember]
    public string session_user_name { get; set; }
    [DataMember]
    public DateTime action_tstamp_tx { get; set; }
    [DataMember]
    public DateTime action_tstamp_stm { get; set; }
    [DataMember]
    public DateTime action_tstamp_clk { get; set; }
    [DataMember]
    public long transaction_id { get; set; }
    [DataMember]
    public string application_name { get; set; }
    [DataMember]
    public string client_addr { get; set; }
    [DataMember]
    public int client_port { get; set; }
    [DataMember]
    public string client_query { get; set; }
    [DataMember]
    public string action { get; set; }
    [DataMember]
    public string row_data { get; set; }
    [DataMember]
    public string changed_fields { get; set; }
    [DataMember]
    public bool statement_only { get; set; }

    #endregion
  }
}
