[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace = "https://www.nupaytsp.co.za/", ConfigurationName = "wsNaedoSoap")]
public interface NAEDOAdmin
{
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/acountChange")]
  acountChangeResponse acountChange(acountChangeRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/aedoConvert")]
  aedoConvertResponse aedoConvert(aedoConvertRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/contractActivation")]
  contractActivationResponse contractActivation(contractActivationRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/contractCancellation")]
  contractCancellationResponse contractCancellation(contractCancellationRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/contractDateChange")]
  contractDateChangeResponse contractDateChange(contractDateChangeRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/contractTrackingChange")]
  contractTrackingChangeResponse contractTrackingChange(contractTrackingChangeRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/instalmentAmountChange")]
  instalmentAmountChangeResponse instalmentAmountChange(instalmentAmountChangeRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/instalmentCancellation")]
  instalmentCancellationResponse instalmentCancellation(instalmentCancellationRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/instalmentDateChange")]
  instalmentDateChangeResponse instalmentDateChange(instalmentDateChangeRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/instalmentMaintenance")]
  instalmentMaintenanceResponse instalmentMaintenance(instalmentMaintenanceRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/instalmentReschedule")]
  instalmentRescheduleResponse instalmentReschedule(instalmentRescheduleRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/instalmentRescheduleMaintenance")]
  instalmentRescheduleMaintenanceResponse instalmentRescheduleMaintenance(instalmentRescheduleMaintenanceRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/instalmentTrackingChange")]
  instalmentTrackingChangeResponse instalmentTrackingChange(instalmentTrackingChangeRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/instalmentRecall")]
  instalmentRecallResponse instalmentRecall(instalmentRecallRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/getTPFReport")]
  getTPFReportResponse getTPFReport(getTPFReportRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/checkTPF")]
  checkTPFResponse checkTPF(checkTPFRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/uploadTSPTransaction")]
  uploadTSPTransactionResponse uploadTSPTransaction(uploadTSPTransactionRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/uploadCCTransaction")]
  uploadCCTransactionResponse uploadCCTransaction(uploadCCTransactionRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/uploadNaedoTransaction")]
  uploadNaedoTransactionResponse uploadNaedoTransaction(uploadNaedoTransactionRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/updateInstalment")]
  updateInstalmentResponse updateInstalment(updateInstalmentRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/cancelTransaction")]
  cancelTransactionResponse cancelTransaction(cancelTransactionRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/cancelInstalment")]
  cancelInstalmentResponse cancelInstalment(cancelInstalmentRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/CDVCheck")]
  CDVCheckResponse CDVCheck(CDVCheckRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/updateTSPTransaction")]
  updateTSPTransactionResponse updateTSPTransaction(updateTSPTransactionRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/updateNaedoTransaction")]
  updateNaedoTransactionResponse updateNaedoTransaction(updateNaedoTransactionRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/updateCCTransaction")]
  updateCCTransactionResponse updateCCTransaction(updateCCTransactionRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/getReport")]
  getReportResponse getReport(getReportRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupaytsp.co.za/requestProgressNAEDO")]
  requestProgressNAEDOResponse requestProgressNAEDO(requestProgressNAEDORequest request);
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class acountChangeRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "acountChange", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public acountChangeRequestBody Body;
  public acountChangeRequest()
  {
  }
  public acountChangeRequest(acountChangeRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class acountChangeRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
  public string new_account_name;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string new_acount_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string new_acount_branch;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 7)]
  public string new_acount_type;
  public acountChangeRequestBody()
  {
  }
  public acountChangeRequestBody(string user_id, string password, int transaction_id, string card_acceptor, string new_account_name, string new_acount_number, string new_acount_branch, string new_acount_type)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.new_account_name = new_account_name;
    this.new_acount_number = new_acount_number;
    this.new_acount_branch = new_acount_branch;
    this.new_acount_type = new_acount_type;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class acountChangeResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "acountChangeResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public acountChangeResponseBody Body;
  public acountChangeResponse()
  {
  }
  public acountChangeResponse(acountChangeResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class acountChangeResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement acountChangeResult;
  public acountChangeResponseBody()
  {
  }
  public acountChangeResponseBody(System.Xml.XmlElement acountChangeResult)
  {
    this.acountChangeResult = acountChangeResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class aedoConvertRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "aedoConvert", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public aedoConvertRequestBody Body;
  public aedoConvertRequest()
  {
  }
  public aedoConvertRequest(aedoConvertRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class aedoConvertRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
  public string new_acount_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string new_acount_branch;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 6)]
  public System.DateTime new_submit_date;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 7)]
  public string new_tracking_indicator;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 8)]
  public int new_frequency;
  public aedoConvertRequestBody()
  {
  }
  public aedoConvertRequestBody(string user_id, string password, int transaction_id, string card_acceptor, string new_acount_number, string new_acount_branch, System.DateTime new_submit_date, string new_tracking_indicator, int new_frequency)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.new_acount_number = new_acount_number;
    this.new_acount_branch = new_acount_branch;
    this.new_submit_date = new_submit_date;
    this.new_tracking_indicator = new_tracking_indicator;
    this.new_frequency = new_frequency;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class aedoConvertResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "aedoConvertResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public aedoConvertResponseBody Body;
  public aedoConvertResponse()
  {
  }
  public aedoConvertResponse(aedoConvertResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class aedoConvertResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement aedoConvertResult;
  public aedoConvertResponseBody()
  {
  }
  public aedoConvertResponseBody(System.Xml.XmlElement aedoConvertResult)
  {
    this.aedoConvertResult = aedoConvertResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class contractActivationRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "contractActivation", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public contractActivationRequestBody Body;
  public contractActivationRequest()
  {
  }
  public contractActivationRequest(contractActivationRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class contractActivationRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  public contractActivationRequestBody()
  {
  }
  public contractActivationRequestBody(string user_id, string password, int transaction_id, string card_acceptor)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class contractActivationResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "contractActivationResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public contractActivationResponseBody Body;
  public contractActivationResponse()
  {
  }
  public contractActivationResponse(contractActivationResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class contractActivationResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement contractActivationResult;
  public contractActivationResponseBody()
  {
  }
  public contractActivationResponseBody(System.Xml.XmlElement contractActivationResult)
  {
    this.contractActivationResult = contractActivationResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class contractCancellationRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "contractCancellation", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public contractCancellationRequestBody Body;
  public contractCancellationRequest()
  {
  }
  public contractCancellationRequest(contractCancellationRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class contractCancellationRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  public contractCancellationRequestBody()
  {
  }
  public contractCancellationRequestBody(string user_id, string password, int transaction_id, string card_acceptor)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class contractCancellationResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "contractCancellationResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public contractCancellationResponseBody Body;
  public contractCancellationResponse()
  {
  }
  public contractCancellationResponse(contractCancellationResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class contractCancellationResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement contractCancellationResult;
  public contractCancellationResponseBody()
  {
  }
  public contractCancellationResponseBody(System.Xml.XmlElement contractCancellationResult)
  {
    this.contractCancellationResult = contractCancellationResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class contractDateChangeRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "contractDateChange", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public contractDateChangeRequestBody Body;
  public contractDateChangeRequest()
  {
  }
  public contractDateChangeRequest(contractDateChangeRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class contractDateChangeRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public System.DateTime new_submit_date;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string frequency;
  public contractDateChangeRequestBody()
  {
  }
  public contractDateChangeRequestBody(string user_id, string password, int transaction_id, string card_acceptor, System.DateTime new_submit_date, string frequency)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.new_submit_date = new_submit_date;
    this.frequency = frequency;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class contractDateChangeResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "contractDateChangeResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public contractDateChangeResponseBody Body;
  public contractDateChangeResponse()
  {
  }
  public contractDateChangeResponse(contractDateChangeResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class contractDateChangeResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement contractDateChangeResult;
  public contractDateChangeResponseBody()
  {
  }
  public contractDateChangeResponseBody(System.Xml.XmlElement contractDateChangeResult)
  {
    this.contractDateChangeResult = contractDateChangeResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class contractTrackingChangeRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "contractTrackingChange", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public contractTrackingChangeRequestBody Body;
  public contractTrackingChangeRequest()
  {
  }
  public contractTrackingChangeRequest(contractTrackingChangeRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class contractTrackingChangeRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
  public string tracking_indicator;
  public contractTrackingChangeRequestBody()
  {
  }
  public contractTrackingChangeRequestBody(string user_id, string password, int transaction_id, string card_acceptor, string tracking_indicator)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.tracking_indicator = tracking_indicator;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class contractTrackingChangeResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "contractTrackingChangeResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public contractTrackingChangeResponseBody Body;
  public contractTrackingChangeResponse()
  {
  }
  public contractTrackingChangeResponse(contractTrackingChangeResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class contractTrackingChangeResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement contractTrackingChangeResult;
  public contractTrackingChangeResponseBody()
  {
  }
  public contractTrackingChangeResponseBody(System.Xml.XmlElement contractTrackingChangeResult)
  {
    this.contractTrackingChangeResult = contractTrackingChangeResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentAmountChangeRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentAmountChange", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentAmountChangeRequestBody Body;
  public instalmentAmountChangeRequest()
  {
  }
  public instalmentAmountChangeRequest(instalmentAmountChangeRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentAmountChangeRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int instalment;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string new_amount;
  public instalmentAmountChangeRequestBody()
  {
  }
  public instalmentAmountChangeRequestBody(string user_id, string password, int transaction_id, string card_acceptor, int instalment, string new_amount)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.instalment = instalment;
    this.new_amount = new_amount;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentAmountChangeResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentAmountChangeResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentAmountChangeResponseBody Body;
  public instalmentAmountChangeResponse()
  {
  }
  public instalmentAmountChangeResponse(instalmentAmountChangeResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentAmountChangeResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement instalmentAmountChangeResult;
  public instalmentAmountChangeResponseBody()
  {
  }
  public instalmentAmountChangeResponseBody(System.Xml.XmlElement instalmentAmountChangeResult)
  {
    this.instalmentAmountChangeResult = instalmentAmountChangeResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentCancellationRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentCancellation", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentCancellationRequestBody Body;
  public instalmentCancellationRequest()
  {
  }
  public instalmentCancellationRequest(instalmentCancellationRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentCancellationRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int instalment;
  public instalmentCancellationRequestBody()
  {
  }
  public instalmentCancellationRequestBody(string user_id, string password, int transaction_id, string card_acceptor, int instalment)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.instalment = instalment;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentCancellationResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentCancellationResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentCancellationResponseBody Body;
  public instalmentCancellationResponse()
  {
  }
  public instalmentCancellationResponse(instalmentCancellationResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentCancellationResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement instalmentCancellationResult;
  public instalmentCancellationResponseBody()
  {
  }
  public instalmentCancellationResponseBody(System.Xml.XmlElement instalmentCancellationResult)
  {
    this.instalmentCancellationResult = instalmentCancellationResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentDateChangeRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentDateChange", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentDateChangeRequestBody Body;
  public instalmentDateChangeRequest()
  {
  }
  public instalmentDateChangeRequest(instalmentDateChangeRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentDateChangeRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int instalment;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public System.DateTime new_submit_date;
  public instalmentDateChangeRequestBody()
  {
  }
  public instalmentDateChangeRequestBody(string user_id, string password, int transaction_id, string card_acceptor, int instalment, System.DateTime new_submit_date)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.instalment = instalment;
    this.new_submit_date = new_submit_date;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentDateChangeResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentDateChangeResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentDateChangeResponseBody Body;
  public instalmentDateChangeResponse()
  {
  }
  public instalmentDateChangeResponse(instalmentDateChangeResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentDateChangeResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement instalmentDateChangeResult;
  public instalmentDateChangeResponseBody()
  {
  }
  public instalmentDateChangeResponseBody(System.Xml.XmlElement instalmentDateChangeResult)
  {
    this.instalmentDateChangeResult = instalmentDateChangeResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentMaintenanceRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentMaintenance", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentMaintenanceRequestBody Body;
  public instalmentMaintenanceRequest()
  {
  }
  public instalmentMaintenanceRequest(instalmentMaintenanceRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentMaintenanceRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int instalment;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public System.DateTime new_submit_date;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string new_tracking_indicator;
  public instalmentMaintenanceRequestBody()
  {
  }
  public instalmentMaintenanceRequestBody(string user_id, string password, int transaction_id, string card_acceptor, int instalment, System.DateTime new_submit_date, string new_tracking_indicator)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.instalment = instalment;
    this.new_submit_date = new_submit_date;
    this.new_tracking_indicator = new_tracking_indicator;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentMaintenanceResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentMaintenanceResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentMaintenanceResponseBody Body;
  public instalmentMaintenanceResponse()
  {
  }
  public instalmentMaintenanceResponse(instalmentMaintenanceResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentMaintenanceResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement instalmentMaintenanceResult;
  public instalmentMaintenanceResponseBody()
  {
  }
  public instalmentMaintenanceResponseBody(System.Xml.XmlElement instalmentMaintenanceResult)
  {
    this.instalmentMaintenanceResult = instalmentMaintenanceResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentRescheduleRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentReschedule", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentRescheduleRequestBody Body;
  public instalmentRescheduleRequest()
  {
  }
  public instalmentRescheduleRequest(instalmentRescheduleRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentRescheduleRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int instalment;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public System.DateTime new_submit_date;
  public instalmentRescheduleRequestBody()
  {
  }
  public instalmentRescheduleRequestBody(string user_id, string password, int transaction_id, string card_acceptor, int instalment, System.DateTime new_submit_date)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.instalment = instalment;
    this.new_submit_date = new_submit_date;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentRescheduleResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentRescheduleResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentRescheduleResponseBody Body;
  public instalmentRescheduleResponse()
  {
  }
  public instalmentRescheduleResponse(instalmentRescheduleResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentRescheduleResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement instalmentRescheduleResult;
  public instalmentRescheduleResponseBody()
  {
  }
  public instalmentRescheduleResponseBody(System.Xml.XmlElement instalmentRescheduleResult)
  {
    this.instalmentRescheduleResult = instalmentRescheduleResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentRescheduleMaintenanceRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentRescheduleMaintenance", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentRescheduleMaintenanceRequestBody Body;
  public instalmentRescheduleMaintenanceRequest()
  {
  }
  public instalmentRescheduleMaintenanceRequest(instalmentRescheduleMaintenanceRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentRescheduleMaintenanceRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int instalment;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public System.DateTime new_submit_date;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string new_tracking_indicator;
  public instalmentRescheduleMaintenanceRequestBody()
  {
  }
  public instalmentRescheduleMaintenanceRequestBody(string user_id, string password, int transaction_id, string card_acceptor, int instalment, System.DateTime new_submit_date, string new_tracking_indicator)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.instalment = instalment;
    this.new_submit_date = new_submit_date;
    this.new_tracking_indicator = new_tracking_indicator;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentRescheduleMaintenanceResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentRescheduleMaintenanceResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentRescheduleMaintenanceResponseBody Body;
  public instalmentRescheduleMaintenanceResponse()
  {
  }
  public instalmentRescheduleMaintenanceResponse(instalmentRescheduleMaintenanceResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentRescheduleMaintenanceResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement instalmentRescheduleMaintenanceResult;
  public instalmentRescheduleMaintenanceResponseBody()
  {
  }
  public instalmentRescheduleMaintenanceResponseBody(System.Xml.XmlElement instalmentRescheduleMaintenanceResult)
  {
    this.instalmentRescheduleMaintenanceResult = instalmentRescheduleMaintenanceResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentTrackingChangeRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentTrackingChange", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentTrackingChangeRequestBody Body;
  public instalmentTrackingChangeRequest()
  {
  }
  public instalmentTrackingChangeRequest(instalmentTrackingChangeRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentTrackingChangeRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int instalment;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string new_tracking_indicator;
  public instalmentTrackingChangeRequestBody()
  {
  }
  public instalmentTrackingChangeRequestBody(string user_id, string password, int transaction_id, string card_acceptor, int instalment, string new_tracking_indicator)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.instalment = instalment;
    this.new_tracking_indicator = new_tracking_indicator;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentTrackingChangeResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentTrackingChangeResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentTrackingChangeResponseBody Body;
  public instalmentTrackingChangeResponse()
  {
  }
  public instalmentTrackingChangeResponse(instalmentTrackingChangeResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentTrackingChangeResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement instalmentTrackingChangeResult;
  public instalmentTrackingChangeResponseBody()
  {
  }
  public instalmentTrackingChangeResponseBody(System.Xml.XmlElement instalmentTrackingChangeResult)
  {
    this.instalmentTrackingChangeResult = instalmentTrackingChangeResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentRecallRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentRecall", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentRecallRequestBody Body;
  public instalmentRecallRequest()
  {
  }
  public instalmentRecallRequest(instalmentRecallRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentRecallRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string user_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string card_acceptor;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int instalment;
  public instalmentRecallRequestBody()
  {
  }
  public instalmentRecallRequestBody(string user_id, string password, int transaction_id, string card_acceptor, int instalment)
  {
    this.user_id = user_id;
    this.password = password;
    this.transaction_id = transaction_id;
    this.card_acceptor = card_acceptor;
    this.instalment = instalment;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class instalmentRecallResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "instalmentRecallResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public instalmentRecallResponseBody Body;
  public instalmentRecallResponse()
  {
  }
  public instalmentRecallResponse(instalmentRecallResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class instalmentRecallResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement instalmentRecallResult;
  public instalmentRecallResponseBody()
  {
  }
  public instalmentRecallResponseBody(System.Xml.XmlElement instalmentRecallResult)
  {
    this.instalmentRecallResult = instalmentRecallResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class getTPFReportRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "getTPFReport", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public getTPFReportRequestBody Body;
  public getTPFReportRequest()
  {
  }
  public getTPFReportRequest(getTPFReportRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class getTPFReportRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
  public int service_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int report_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public System.DateTime date_from;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 6)]
  public System.DateTime date_to;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 7)]
  public int token_id;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 8)]
  public int block_id;
  public getTPFReportRequestBody()
  {
  }
  public getTPFReportRequestBody(int merchant_number, string username, string password, int service_type, int report_type, System.DateTime date_from, System.DateTime date_to, int token_id, int block_id)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.service_type = service_type;
    this.report_type = report_type;
    this.date_from = date_from;
    this.date_to = date_to;
    this.token_id = token_id;
    this.block_id = block_id;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class getTPFReportResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "getTPFReportResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public getTPFReportResponseBody Body;
  public getTPFReportResponse()
  {
  }
  public getTPFReportResponse(getTPFReportResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class getTPFReportResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement getTPFReportResult;
  public getTPFReportResponseBody()
  {
  }
  public getTPFReportResponseBody(System.Xml.XmlElement getTPFReportResult)
  {
    this.getTPFReportResult = getTPFReportResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class checkTPFRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "checkTPF", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public checkTPFRequestBody Body;
  public checkTPFRequest()
  {
  }
  public checkTPFRequest(checkTPFRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class checkTPFRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string card_acceptor;
  public checkTPFRequestBody()
  {
  }
  public checkTPFRequestBody(string card_acceptor)
  {
    this.card_acceptor = card_acceptor;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class checkTPFResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "checkTPFResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public checkTPFResponseBody Body;
  public checkTPFResponse()
  {
  }
  public checkTPFResponse(checkTPFResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class checkTPFResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement checkTPFResult;
  public checkTPFResponseBody()
  {
  }
  public checkTPFResponseBody(System.Xml.XmlElement checkTPFResult)
  {
    this.checkTPFResult = checkTPFResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class uploadTSPTransactionRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "uploadTSPTransaction", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public uploadTSPTransactionRequestBody Body;
  public uploadTSPTransactionRequest()
  {
  }
  public uploadTSPTransactionRequest(uploadTSPTransactionRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class uploadTSPTransactionRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
  public int service_type;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
  public string nominated_account_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string client_ref_1;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string client_ref_2;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 7)]
  public string account_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 8)]
  public string account_name;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 9)]
  public string branch_code;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 10)]
  public int account_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 11)]
  public int frequency;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 12)]
  public int entry_class;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 13)]
  public int no_of_instalments;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 14)]
  public System.DateTime action_date;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 15)]
  public string transaction_value;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 16)]
  public string client_id;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 17)]
  public int days_warning;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 18)]
  public int email_flag;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 19)]
  public string email_address;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 20)]
  public int sms_flag;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 21)]
  public string cell_no;
  public uploadTSPTransactionRequestBody()
  {
  }
  public uploadTSPTransactionRequestBody(
                int merchant_number,
                string username,
                string password,
                int service_type,
                string nominated_account_number,
                string client_ref_1,
                string client_ref_2,
                string account_number,
                string account_name,
                string branch_code,
                int account_type,
                int frequency,
                int entry_class,
                int no_of_instalments,
                System.DateTime action_date,
                string transaction_value,
                string client_id,
                int days_warning,
                int email_flag,
                string email_address,
                int sms_flag,
                string cell_no)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.service_type = service_type;
    this.nominated_account_number = nominated_account_number;
    this.client_ref_1 = client_ref_1;
    this.client_ref_2 = client_ref_2;
    this.account_number = account_number;
    this.account_name = account_name;
    this.branch_code = branch_code;
    this.account_type = account_type;
    this.frequency = frequency;
    this.entry_class = entry_class;
    this.no_of_instalments = no_of_instalments;
    this.action_date = action_date;
    this.transaction_value = transaction_value;
    this.client_id = client_id;
    this.days_warning = days_warning;
    this.email_flag = email_flag;
    this.email_address = email_address;
    this.sms_flag = sms_flag;
    this.cell_no = cell_no;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class uploadTSPTransactionResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "uploadTSPTransactionResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public uploadTSPTransactionResponseBody Body;
  public uploadTSPTransactionResponse()
  {
  }
  public uploadTSPTransactionResponse(uploadTSPTransactionResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class uploadTSPTransactionResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement uploadTSPTransactionResult;
  public uploadTSPTransactionResponseBody()
  {
  }
  public uploadTSPTransactionResponseBody(System.Xml.XmlElement uploadTSPTransactionResult)
  {
    this.uploadTSPTransactionResult = uploadTSPTransactionResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class uploadCCTransactionRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "uploadCCTransaction", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public uploadCCTransactionRequestBody Body;
  public uploadCCTransactionRequest()
  {
  }
  public uploadCCTransactionRequest(uploadCCTransactionRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class uploadCCTransactionRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
  public int service_type;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
  public string nominated_account_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string client_ref_1;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string client_ref_2;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 7)]
  public int frequency;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 8)]
  public int no_of_instalments;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 9)]
  public System.DateTime action_date;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 10)]
  public string transaction_value;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 11)]
  public string client_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 12)]
  public string credit_card_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 13)]
  public string cvv2;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 14)]
  public string expiry_date;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 15)]
  public int days_warning;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 16)]
  public int email_flag;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 17)]
  public string email_address;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 18)]
  public int sms_flag;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 19)]
  public string cell_no;
  public uploadCCTransactionRequestBody()
  {
  }
  public uploadCCTransactionRequestBody(
                int merchant_number,
                string username,
                string password,
                int service_type,
                string nominated_account_number,
                string client_ref_1,
                string client_ref_2,
                int frequency,
                int no_of_instalments,
                System.DateTime action_date,
                string transaction_value,
                string client_id,
                string credit_card_number,
                string cvv2,
                string expiry_date,
                int days_warning,
                int email_flag,
                string email_address,
                int sms_flag,
                string cell_no)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.service_type = service_type;
    this.nominated_account_number = nominated_account_number;
    this.client_ref_1 = client_ref_1;
    this.client_ref_2 = client_ref_2;
    this.frequency = frequency;
    this.no_of_instalments = no_of_instalments;
    this.action_date = action_date;
    this.transaction_value = transaction_value;
    this.client_id = client_id;
    this.credit_card_number = credit_card_number;
    this.cvv2 = cvv2;
    this.expiry_date = expiry_date;
    this.days_warning = days_warning;
    this.email_flag = email_flag;
    this.email_address = email_address;
    this.sms_flag = sms_flag;
    this.cell_no = cell_no;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class uploadCCTransactionResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "uploadCCTransactionResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public uploadCCTransactionResponseBody Body;
  public uploadCCTransactionResponse()
  {
  }
  public uploadCCTransactionResponse(uploadCCTransactionResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class uploadCCTransactionResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement uploadCCTransactionResult;
  public uploadCCTransactionResponseBody()
  {
  }
  public uploadCCTransactionResponseBody(System.Xml.XmlElement uploadCCTransactionResult)
  {
    this.uploadCCTransactionResult = uploadCCTransactionResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class uploadNaedoTransactionRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "uploadNaedoTransaction", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public uploadNaedoTransactionRequestBody Body;
  public uploadNaedoTransactionRequest()
  {
  }
  public uploadNaedoTransactionRequest(uploadNaedoTransactionRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class uploadNaedoTransactionRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
  public int service_type;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
  public string nominated_account_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string client_ref_1;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string client_ref_2;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 7)]
  public string account_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 8)]
  public string account_name;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 9)]
  public string branch_code;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 10)]
  public int account_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 11)]
  public int frequency;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 12)]
  public int tracking_indicator;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 13)]
  public int no_of_instalments;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 14)]
  public System.DateTime action_date;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 15)]
  public string transaction_value;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 16)]
  public string client_id;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 17)]
  public int days_warning;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 18)]
  public int email_flag;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 19)]
  public string email_address;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 20)]
  public int sms_flag;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 21)]
  public string cell_no;
  public uploadNaedoTransactionRequestBody()
  {
  }
  public uploadNaedoTransactionRequestBody(
                int merchant_number,
                string username,
                string password,
                int service_type,
                string nominated_account_number,
                string client_ref_1,
                string client_ref_2,
                string account_number,
                string account_name,
                string branch_code,
                int account_type,
                int frequency,
                int tracking_indicator,
                int no_of_instalments,
                System.DateTime action_date,
                string transaction_value,
                string client_id,
                int days_warning,
                int email_flag,
                string email_address,
                int sms_flag,
                string cell_no)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.service_type = service_type;
    this.nominated_account_number = nominated_account_number;
    this.client_ref_1 = client_ref_1;
    this.client_ref_2 = client_ref_2;
    this.account_number = account_number;
    this.account_name = account_name;
    this.branch_code = branch_code;
    this.account_type = account_type;
    this.frequency = frequency;
    this.tracking_indicator = tracking_indicator;
    this.no_of_instalments = no_of_instalments;
    this.action_date = action_date;
    this.transaction_value = transaction_value;
    this.client_id = client_id;
    this.days_warning = days_warning;
    this.email_flag = email_flag;
    this.email_address = email_address;
    this.sms_flag = sms_flag;
    this.cell_no = cell_no;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class uploadNaedoTransactionResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "uploadNaedoTransactionResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public uploadNaedoTransactionResponseBody Body;
  public uploadNaedoTransactionResponse()
  {
  }
  public uploadNaedoTransactionResponse(uploadNaedoTransactionResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class uploadNaedoTransactionResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement uploadNaedoTransactionResult;
  public uploadNaedoTransactionResponseBody()
  {
  }
  public uploadNaedoTransactionResponseBody(System.Xml.XmlElement uploadNaedoTransactionResult)
  {
    this.uploadNaedoTransactionResult = uploadNaedoTransactionResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class updateInstalmentRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "updateInstalment", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public updateInstalmentRequestBody Body;
  public updateInstalmentRequest()
  {
  }
  public updateInstalmentRequest(updateInstalmentRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class updateInstalmentRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
  public int service_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public int instalment;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string client_ref_2;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 7)]
  public System.DateTime action_date;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 8)]
  public string transaction_value;
  public updateInstalmentRequestBody()
  {
  }
  public updateInstalmentRequestBody(int merchant_number, string username, string password, int service_type, int transaction_id, int instalment, string client_ref_2, System.DateTime action_date, string transaction_value)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.service_type = service_type;
    this.transaction_id = transaction_id;
    this.instalment = instalment;
    this.client_ref_2 = client_ref_2;
    this.action_date = action_date;
    this.transaction_value = transaction_value;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class updateInstalmentResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "updateInstalmentResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public updateInstalmentResponseBody Body;
  public updateInstalmentResponse()
  {
  }
  public updateInstalmentResponse(updateInstalmentResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class updateInstalmentResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement updateInstalmentResult;
  public updateInstalmentResponseBody()
  {
  }
  public updateInstalmentResponseBody(System.Xml.XmlElement updateInstalmentResult)
  {
    this.updateInstalmentResult = updateInstalmentResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class cancelTransactionRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "cancelTransaction", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public cancelTransactionRequestBody Body;
  public cancelTransactionRequest()
  {
  }
  public cancelTransactionRequest(cancelTransactionRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class cancelTransactionRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
  public int service_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string client_ref_2;
  public cancelTransactionRequestBody()
  {
  }
  public cancelTransactionRequestBody(int merchant_number, string username, string password, int service_type, int transaction_id, string client_ref_2)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.service_type = service_type;
    this.transaction_id = transaction_id;
    this.client_ref_2 = client_ref_2;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class cancelTransactionResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "cancelTransactionResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public cancelTransactionResponseBody Body;
  public cancelTransactionResponse()
  {
  }
  public cancelTransactionResponse(cancelTransactionResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class cancelTransactionResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement cancelTransactionResult;
  public cancelTransactionResponseBody()
  {
  }
  public cancelTransactionResponseBody(System.Xml.XmlElement cancelTransactionResult)
  {
    this.cancelTransactionResult = cancelTransactionResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class cancelInstalmentRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "cancelInstalment", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public cancelInstalmentRequestBody Body;
  public cancelInstalmentRequest()
  {
  }
  public cancelInstalmentRequest(cancelInstalmentRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class cancelInstalmentRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
  public int service_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public int instalment;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string client_ref_2;
  public cancelInstalmentRequestBody()
  {
  }
  public cancelInstalmentRequestBody(int merchant_number, string username, string password, int service_type, int transaction_id, int instalment, string client_ref_2)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.service_type = service_type;
    this.transaction_id = transaction_id;
    this.instalment = instalment;
    this.client_ref_2 = client_ref_2;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class cancelInstalmentResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "cancelInstalmentResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public cancelInstalmentResponseBody Body;
  public cancelInstalmentResponse()
  {
  }
  public cancelInstalmentResponse(cancelInstalmentResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class cancelInstalmentResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement cancelInstalmentResult;
  public cancelInstalmentResponseBody()
  {
  }
  public cancelInstalmentResponseBody(System.Xml.XmlElement cancelInstalmentResult)
  {
    this.cancelInstalmentResult = cancelInstalmentResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class CDVCheckRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "CDVCheck", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public CDVCheckRequestBody Body;
  public CDVCheckRequest()
  {
  }
  public CDVCheckRequest(CDVCheckRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class CDVCheckRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string account_number;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int account_type;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string branch_code;
  public CDVCheckRequestBody()
  {
  }
  public CDVCheckRequestBody(int merchant_number, string username, string password, string account_number, int account_type, string branch_code)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.account_number = account_number;
    this.account_type = account_type;
    this.branch_code = branch_code;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class CDVCheckResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "CDVCheckResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public CDVCheckResponseBody Body;
  public CDVCheckResponse()
  {
  }
  public CDVCheckResponse(CDVCheckResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class CDVCheckResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement CDVCheckResult;
  public CDVCheckResponseBody()
  {
  }
  public CDVCheckResponseBody(System.Xml.XmlElement CDVCheckResult)
  {
    this.CDVCheckResult = CDVCheckResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class updateTSPTransactionRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "updateTSPTransaction", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public updateTSPTransactionRequestBody Body;
  public updateTSPTransactionRequest()
  {
  }
  public updateTSPTransactionRequest(updateTSPTransactionRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class updateTSPTransactionRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
  public int service_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string current_client_ref_2;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string client_ref_1;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 7)]
  public string client_ref_2;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 8)]
  public string account_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 9)]
  public string branch_code;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 10)]
  public int account_type;
  public updateTSPTransactionRequestBody()
  {
  }
  public updateTSPTransactionRequestBody(int merchant_number, string username, string password, int service_type, int transaction_id, string current_client_ref_2, string client_ref_1, string client_ref_2, string account_number, string branch_code, int account_type)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.service_type = service_type;
    this.transaction_id = transaction_id;
    this.current_client_ref_2 = current_client_ref_2;
    this.client_ref_1 = client_ref_1;
    this.client_ref_2 = client_ref_2;
    this.account_number = account_number;
    this.branch_code = branch_code;
    this.account_type = account_type;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class updateTSPTransactionResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "updateTSPTransactionResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public updateTSPTransactionResponseBody Body;
  public updateTSPTransactionResponse()
  {
  }
  public updateTSPTransactionResponse(updateTSPTransactionResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class updateTSPTransactionResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement updateTSPTransactionResult;
  public updateTSPTransactionResponseBody()
  {
  }
  public updateTSPTransactionResponseBody(System.Xml.XmlElement updateTSPTransactionResult)
  {
    this.updateTSPTransactionResult = updateTSPTransactionResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class updateNaedoTransactionRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "updateNaedoTransaction", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public updateNaedoTransactionRequestBody Body;
  public updateNaedoTransactionRequest()
  {
  }
  public updateNaedoTransactionRequest(updateNaedoTransactionRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class updateNaedoTransactionRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
  public int service_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string current_client_ref_2;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string client_ref_1;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 7)]
  public string client_ref_2;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 8)]
  public string account_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 9)]
  public string branch_code;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 10)]
  public int account_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 11)]
  public int tracking_indicator;
  public updateNaedoTransactionRequestBody()
  {
  }
  public updateNaedoTransactionRequestBody(int merchant_number, string username, string password, int service_type, int transaction_id, string current_client_ref_2, string client_ref_1, string client_ref_2, string account_number, string branch_code, int account_type, int tracking_indicator)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.service_type = service_type;
    this.transaction_id = transaction_id;
    this.current_client_ref_2 = current_client_ref_2;
    this.client_ref_1 = client_ref_1;
    this.client_ref_2 = client_ref_2;
    this.account_number = account_number;
    this.branch_code = branch_code;
    this.account_type = account_type;
    this.tracking_indicator = tracking_indicator;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class updateNaedoTransactionResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "updateNaedoTransactionResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public updateNaedoTransactionResponseBody Body;
  public updateNaedoTransactionResponse()
  {
  }
  public updateNaedoTransactionResponse(updateNaedoTransactionResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class updateNaedoTransactionResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement updateNaedoTransactionResult;
  public updateNaedoTransactionResponseBody()
  {
  }
  public updateNaedoTransactionResponseBody(System.Xml.XmlElement updateNaedoTransactionResult)
  {
    this.updateNaedoTransactionResult = updateNaedoTransactionResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class updateCCTransactionRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "updateCCTransaction", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public updateCCTransactionRequestBody Body;
  public updateCCTransactionRequest()
  {
  }
  public updateCCTransactionRequest(updateCCTransactionRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class updateCCTransactionRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
  public int service_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int transaction_id;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string current_client_ref_2;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string client_ref_1;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 7)]
  public string client_ref_2;
  public updateCCTransactionRequestBody()
  {
  }
  public updateCCTransactionRequestBody(int merchant_number, string username, string password, int service_type, int transaction_id, string current_client_ref_2, string client_ref_1, string client_ref_2)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.service_type = service_type;
    this.transaction_id = transaction_id;
    this.current_client_ref_2 = current_client_ref_2;
    this.client_ref_1 = client_ref_1;
    this.client_ref_2 = client_ref_2;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class updateCCTransactionResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "updateCCTransactionResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public updateCCTransactionResponseBody Body;
  public updateCCTransactionResponse()
  {
  }
  public updateCCTransactionResponse(updateCCTransactionResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class updateCCTransactionResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement updateCCTransactionResult;
  public updateCCTransactionResponseBody()
  {
  }
  public updateCCTransactionResponseBody(System.Xml.XmlElement updateCCTransactionResult)
  {
    this.updateCCTransactionResult = updateCCTransactionResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class getReportRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "getReport", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public getReportRequestBody Body;
  public getReportRequest()
  {
  }
  public getReportRequest(getReportRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class getReportRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
  public int merchant_number;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
  public int service_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int report_type;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public System.DateTime date_from;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 6)]
  public System.DateTime date_to;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 7)]
  public int token_id;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 8)]
  public int block_id;
  public getReportRequestBody()
  {
  }
  public getReportRequestBody(int merchant_number, string username, string password, int service_type, int report_type, System.DateTime date_from, System.DateTime date_to, int token_id, int block_id)
  {
    this.merchant_number = merchant_number;
    this.username = username;
    this.password = password;
    this.service_type = service_type;
    this.report_type = report_type;
    this.date_from = date_from;
    this.date_to = date_to;
    this.token_id = token_id;
    this.block_id = block_id;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class getReportResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "getReportResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public getReportResponseBody Body;
  public getReportResponse()
  {
  }
  public getReportResponse(getReportResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class getReportResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement getReportResult;
  public getReportResponseBody()
  {
  }
  public getReportResponseBody(System.Xml.XmlElement getReportResult)
  {
    this.getReportResult = getReportResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class requestProgressNAEDORequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "requestProgressNAEDO", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public requestProgressNAEDORequestBody Body;
  public requestProgressNAEDORequest()
  {
  }
  public requestProgressNAEDORequest(requestProgressNAEDORequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class requestProgressNAEDORequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string bank;
  public requestProgressNAEDORequestBody()
  {
  }
  public requestProgressNAEDORequestBody(string bank)
  {
    this.bank = bank;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class requestProgressNAEDOResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "requestProgressNAEDOResponse", Namespace = "https://www.nupaytsp.co.za/", Order = 0)]
  public requestProgressNAEDOResponseBody Body;
  public requestProgressNAEDOResponse()
  {
  }
  public requestProgressNAEDOResponse(requestProgressNAEDOResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupaytsp.co.za/")]
public partial class requestProgressNAEDOResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public System.Xml.XmlElement requestProgressNAEDOResult;
  public requestProgressNAEDOResponseBody()
  {
  }
  public requestProgressNAEDOResponseBody(System.Xml.XmlElement requestProgressNAEDOResult)
  {
    this.requestProgressNAEDOResult = requestProgressNAEDOResult;
  }
}
