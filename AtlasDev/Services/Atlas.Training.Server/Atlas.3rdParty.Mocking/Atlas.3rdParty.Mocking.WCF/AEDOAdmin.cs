[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", ConfigurationName = "NuPayTransactionsServiceSoap")]
public interface AEDOAdmin
{
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Contrac" +
  "tActivation_AC")]
  ContractActivation_ACResponse ContractActivation_AC(ContractActivation_ACRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Contrac" +
  "tCancellation_CC")]
  ContractCancellation_CCResponse ContractCancellation_CC(ContractCancellation_CCRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Contrac" +
  "tDateChange_DC")]
  ContractDateChange_DCResponse ContractDateChange_DC(ContractDateChange_DCRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Employe" +
  "rCodeChange_EC")]
  EmployerCodeChange_ECResponse EmployerCodeChange_EC(EmployerCodeChange_ECRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Frequen" +
  "cyChange_FC")]
  FrequencyChange_FCResponse FrequencyChange_FC(FrequencyChange_FCRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Contrac" +
  "tNumberChangeActivation_NA")]
  ContractNumberChangeActivation_NAResponse ContractNumberChangeActivation_NA(ContractNumberChangeActivation_NARequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Contrac" +
  "tNumberChange_NC")]
  ContractNumberChange_NCResponse ContractNumberChange_NC(ContractNumberChange_NCRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Contrac" +
  "tTrackingCodeChange_TC")]
  ContractTrackingCodeChange_TCResponse ContractTrackingCodeChange_TC(ContractTrackingCodeChange_TCRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/IdNumbe" +
  "rChange_ZC")]
  IdNumberChange_ZCResponse IdNumberChange_ZC(IdNumberChange_ZCRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Contrac" +
  "tPendingDateChange_FD")]
  ContractPendingDateChange_FDResponse ContractPendingDateChange_FD(ContractPendingDateChange_FDRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Contrac" +
  "tDateChangeActivation_FA")]
  ContractDateChangeActivation_FAResponse ContractDateChangeActivation_FA(ContractDateChangeActivation_FARequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Instalm" +
  "entDateChange_DI")]
  InstalmentDateChange_DIResponse InstalmentDateChange_DI(InstalmentDateChange_DIRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Instalm" +
  "entCancellation_IC")]
  InstalmentCancellation_ICResponse InstalmentCancellation_IC(InstalmentCancellation_ICRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Instalm" +
  "entMaintenance_MI")]
  InstalmentMaintenance_MIResponse InstalmentMaintenance_MI(InstalmentMaintenance_MIRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Instalm" +
  "entResubmit_RI")]
  InstalmentResubmit_RIResponse InstalmentResubmit_RI(InstalmentResubmit_RIRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Instalm" +
  "entTrackingCodeChange_TI")]
  InstalmentTrackingCodeChange_TIResponse InstalmentTrackingCodeChange_TI(InstalmentTrackingCodeChange_TIRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Instalm" +
  "entPreventResubmission_PR")]
  InstalmentPreventResubmission_PRResponse InstalmentPreventResubmission_PR(InstalmentPreventResubmission_PRRequest request);
  [System.ServiceModel.OperationContractAttribute(Action = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx/Instalm" +
  "entResubmissionMaintenance_RM")]
  InstalmentResubmissionMaintenance_RMResponse InstalmentResubmissionMaintenance_RM(InstalmentResubmissionMaintenance_RMRequest request);
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractActivation_ACRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractActivation_AC", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractActivation_ACRequestBody Body;
  public ContractActivation_ACRequest()
  {
  }
  public ContractActivation_ACRequest(ContractActivation_ACRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractActivation_ACRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  public ContractActivation_ACRequestBody()
  {
  }
  public ContractActivation_ACRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractActivation_ACResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractActivation_ACResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractActivation_ACResponseBody Body;
  public ContractActivation_ACResponse()
  {
  }
  public ContractActivation_ACResponse(ContractActivation_ACResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractActivation_ACResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string ContractActivation_ACResult;
  public ContractActivation_ACResponseBody()
  {
  }
  public ContractActivation_ACResponseBody(string ContractActivation_ACResult)
  {
    this.ContractActivation_ACResult = ContractActivation_ACResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractCancellation_CCRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractCancellation_CC", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractCancellation_CCRequestBody Body;
  public ContractCancellation_CCRequest()
  {
  }
  public ContractCancellation_CCRequest(ContractCancellation_CCRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractCancellation_CCRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  public ContractCancellation_CCRequestBody()
  {
  }
  public ContractCancellation_CCRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractCancellation_CCResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractCancellation_CCResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractCancellation_CCResponseBody Body;
  public ContractCancellation_CCResponse()
  {
  }
  public ContractCancellation_CCResponse(ContractCancellation_CCResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractCancellation_CCResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string ContractCancellation_CCResult;
  public ContractCancellation_CCResponseBody()
  {
  }
  public ContractCancellation_CCResponseBody(string ContractCancellation_CCResult)
  {
    this.ContractCancellation_CCResult = ContractCancellation_CCResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractDateChange_DCRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractDateChange_DC", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractDateChange_DCRequestBody Body;
  public ContractDateChange_DCRequest()
  {
  }
  public ContractDateChange_DCRequest(ContractDateChange_DCRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractDateChange_DCRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public System.DateTime NewSubmitDate;
  public ContractDateChange_DCRequestBody()
  {
  }
  public ContractDateChange_DCRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, System.DateTime NewSubmitDate)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.NewSubmitDate = NewSubmitDate;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractDateChange_DCResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractDateChange_DCResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractDateChange_DCResponseBody Body;
  public ContractDateChange_DCResponse()
  {
  }
  public ContractDateChange_DCResponse(ContractDateChange_DCResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractDateChange_DCResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string ContractDateChange_DCResult;
  public ContractDateChange_DCResponseBody()
  {
  }
  public ContractDateChange_DCResponseBody(string ContractDateChange_DCResult)
  {
    this.ContractDateChange_DCResult = ContractDateChange_DCResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class EmployerCodeChange_ECRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "EmployerCodeChange_EC", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public EmployerCodeChange_ECRequestBody Body;
  public EmployerCodeChange_ECRequest()
  {
  }
  public EmployerCodeChange_ECRequest(EmployerCodeChange_ECRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class EmployerCodeChange_ECRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string NewEmployerCode;
  public EmployerCodeChange_ECRequestBody()
  {
  }
  public EmployerCodeChange_ECRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, string NewEmployerCode)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.NewEmployerCode = NewEmployerCode;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class EmployerCodeChange_ECResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "EmployerCodeChange_ECResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public EmployerCodeChange_ECResponseBody Body;
  public EmployerCodeChange_ECResponse()
  {
  }
  public EmployerCodeChange_ECResponse(EmployerCodeChange_ECResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class EmployerCodeChange_ECResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string EmployerCodeChange_ECResult;
  public EmployerCodeChange_ECResponseBody()
  {
  }
  public EmployerCodeChange_ECResponseBody(string EmployerCodeChange_ECResult)
  {
    this.EmployerCodeChange_ECResult = EmployerCodeChange_ECResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class FrequencyChange_FCRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "FrequencyChange_FC", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public FrequencyChange_FCRequestBody Body;
  public FrequencyChange_FCRequest()
  {
  }
  public FrequencyChange_FCRequest(FrequencyChange_FCRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class FrequencyChange_FCRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public System.DateTime SubmitDate;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 6)]
  public int NewFrequency;
  public FrequencyChange_FCRequestBody()
  {
  }
  public FrequencyChange_FCRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, System.DateTime SubmitDate, int NewFrequency)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.SubmitDate = SubmitDate;
    this.NewFrequency = NewFrequency;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class FrequencyChange_FCResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "FrequencyChange_FCResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public FrequencyChange_FCResponseBody Body;
  public FrequencyChange_FCResponse()
  {
  }
  public FrequencyChange_FCResponse(FrequencyChange_FCResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class FrequencyChange_FCResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string FrequencyChange_FCResult;
  public FrequencyChange_FCResponseBody()
  {
  }
  public FrequencyChange_FCResponseBody(string FrequencyChange_FCResult)
  {
    this.FrequencyChange_FCResult = FrequencyChange_FCResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractNumberChangeActivation_NARequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractNumberChangeActivation_NA", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractNumberChangeActivation_NARequestBody Body;
  public ContractNumberChangeActivation_NARequest()
  {
  }
  public ContractNumberChangeActivation_NARequest(ContractNumberChangeActivation_NARequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractNumberChangeActivation_NARequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string NewContractNumber;
  public ContractNumberChangeActivation_NARequestBody()
  {
  }
  public ContractNumberChangeActivation_NARequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, string NewContractNumber)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.NewContractNumber = NewContractNumber;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractNumberChangeActivation_NAResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractNumberChangeActivation_NAResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractNumberChangeActivation_NAResponseBody Body;
  public ContractNumberChangeActivation_NAResponse()
  {
  }
  public ContractNumberChangeActivation_NAResponse(ContractNumberChangeActivation_NAResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractNumberChangeActivation_NAResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string ContractNumberChangeActivation_NAResult;
  public ContractNumberChangeActivation_NAResponseBody()
  {
  }
  public ContractNumberChangeActivation_NAResponseBody(string ContractNumberChangeActivation_NAResult)
  {
    this.ContractNumberChangeActivation_NAResult = ContractNumberChangeActivation_NAResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractNumberChange_NCRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractNumberChange_NC", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractNumberChange_NCRequestBody Body;
  public ContractNumberChange_NCRequest()
  {
  }
  public ContractNumberChange_NCRequest(ContractNumberChange_NCRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractNumberChange_NCRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string NewContractNumber;
  public ContractNumberChange_NCRequestBody()
  {
  }
  public ContractNumberChange_NCRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, string NewContractNumber)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.NewContractNumber = NewContractNumber;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractNumberChange_NCResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractNumberChange_NCResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractNumberChange_NCResponseBody Body;
  public ContractNumberChange_NCResponse()
  {
  }
  public ContractNumberChange_NCResponse(ContractNumberChange_NCResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractNumberChange_NCResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string ContractNumberChange_NCResult;
  public ContractNumberChange_NCResponseBody()
  {
  }
  public ContractNumberChange_NCResponseBody(string ContractNumberChange_NCResult)
  {
    this.ContractNumberChange_NCResult = ContractNumberChange_NCResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractTrackingCodeChange_TCRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractTrackingCodeChange_TC", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractTrackingCodeChange_TCRequestBody Body;
  public ContractTrackingCodeChange_TCRequest()
  {
  }
  public ContractTrackingCodeChange_TCRequest(ContractTrackingCodeChange_TCRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractTrackingCodeChange_TCRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string NewTrackingCode;
  public ContractTrackingCodeChange_TCRequestBody()
  {
  }
  public ContractTrackingCodeChange_TCRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, string NewTrackingCode)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.NewTrackingCode = NewTrackingCode;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractTrackingCodeChange_TCResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractTrackingCodeChange_TCResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractTrackingCodeChange_TCResponseBody Body;
  public ContractTrackingCodeChange_TCResponse()
  {
  }
  public ContractTrackingCodeChange_TCResponse(ContractTrackingCodeChange_TCResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractTrackingCodeChange_TCResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string ContractTrackingCodeChange_TCResult;
  public ContractTrackingCodeChange_TCResponseBody()
  {
  }
  public ContractTrackingCodeChange_TCResponseBody(string ContractTrackingCodeChange_TCResult)
  {
    this.ContractTrackingCodeChange_TCResult = ContractTrackingCodeChange_TCResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class IdNumberChange_ZCRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "IdNumberChange_ZC", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public IdNumberChange_ZCRequestBody Body;
  public IdNumberChange_ZCRequest()
  {
  }
  public IdNumberChange_ZCRequest(IdNumberChange_ZCRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class IdNumberChange_ZCRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string NewIDNumber;
  public IdNumberChange_ZCRequestBody()
  {
  }
  public IdNumberChange_ZCRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, string NewIDNumber)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.NewIDNumber = NewIDNumber;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class IdNumberChange_ZCResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "IdNumberChange_ZCResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public IdNumberChange_ZCResponseBody Body;
  public IdNumberChange_ZCResponse()
  {
  }
  public IdNumberChange_ZCResponse(IdNumberChange_ZCResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class IdNumberChange_ZCResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string IdNumberChange_ZCResult;
  public IdNumberChange_ZCResponseBody()
  {
  }
  public IdNumberChange_ZCResponseBody(string IdNumberChange_ZCResult)
  {
    this.IdNumberChange_ZCResult = IdNumberChange_ZCResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractPendingDateChange_FDRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractPendingDateChange_FD", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractPendingDateChange_FDRequestBody Body;
  public ContractPendingDateChange_FDRequest()
  {
  }
  public ContractPendingDateChange_FDRequest(ContractPendingDateChange_FDRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractPendingDateChange_FDRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public System.DateTime NewSubmitDate;
  public ContractPendingDateChange_FDRequestBody()
  {
  }
  public ContractPendingDateChange_FDRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, System.DateTime NewSubmitDate)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.NewSubmitDate = NewSubmitDate;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractPendingDateChange_FDResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractPendingDateChange_FDResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractPendingDateChange_FDResponseBody Body;
  public ContractPendingDateChange_FDResponse()
  {
  }
  public ContractPendingDateChange_FDResponse(ContractPendingDateChange_FDResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractPendingDateChange_FDResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string ContractPendingDateChange_FDResult;
  public ContractPendingDateChange_FDResponseBody()
  {
  }
  public ContractPendingDateChange_FDResponseBody(string ContractPendingDateChange_FDResult)
  {
    this.ContractPendingDateChange_FDResult = ContractPendingDateChange_FDResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractDateChangeActivation_FARequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractDateChangeActivation_FA", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractDateChangeActivation_FARequestBody Body;
  public ContractDateChangeActivation_FARequest()
  {
  }
  public ContractDateChangeActivation_FARequest(ContractDateChangeActivation_FARequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractDateChangeActivation_FARequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public System.DateTime NewSubmitDate;
  public ContractDateChangeActivation_FARequestBody()
  {
  }
  public ContractDateChangeActivation_FARequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, System.DateTime NewSubmitDate)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.NewSubmitDate = NewSubmitDate;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class ContractDateChangeActivation_FAResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "ContractDateChangeActivation_FAResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public ContractDateChangeActivation_FAResponseBody Body;
  public ContractDateChangeActivation_FAResponse()
  {
  }
  public ContractDateChangeActivation_FAResponse(ContractDateChangeActivation_FAResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class ContractDateChangeActivation_FAResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string ContractDateChangeActivation_FAResult;
  public ContractDateChangeActivation_FAResponseBody()
  {
  }
  public ContractDateChangeActivation_FAResponseBody(string ContractDateChangeActivation_FAResult)
  {
    this.ContractDateChangeActivation_FAResult = ContractDateChangeActivation_FAResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentDateChange_DIRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentDateChange_DI", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentDateChange_DIRequestBody Body;
  public InstalmentDateChange_DIRequest()
  {
  }
  public InstalmentDateChange_DIRequest(InstalmentDateChange_DIRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentDateChange_DIRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public int Instalment;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 6)]
  public System.DateTime NewInstalmentDate;
  public InstalmentDateChange_DIRequestBody()
  {
  }
  public InstalmentDateChange_DIRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, int Instalment, System.DateTime NewInstalmentDate)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.Instalment = Instalment;
    this.NewInstalmentDate = NewInstalmentDate;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentDateChange_DIResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentDateChange_DIResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentDateChange_DIResponseBody Body;
  public InstalmentDateChange_DIResponse()
  {
  }
  public InstalmentDateChange_DIResponse(InstalmentDateChange_DIResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentDateChange_DIResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string InstalmentDateChange_DIResult;
  public InstalmentDateChange_DIResponseBody()
  {
  }
  public InstalmentDateChange_DIResponseBody(string InstalmentDateChange_DIResult)
  {
    this.InstalmentDateChange_DIResult = InstalmentDateChange_DIResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentCancellation_ICRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentCancellation_IC", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentCancellation_ICRequestBody Body;
  public InstalmentCancellation_ICRequest()
  {
  }
  public InstalmentCancellation_ICRequest(InstalmentCancellation_ICRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentCancellation_ICRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public int Instalment;
  public InstalmentCancellation_ICRequestBody()
  {
  }
  public InstalmentCancellation_ICRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, int Instalment)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.Instalment = Instalment;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentCancellation_ICResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentCancellation_ICResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentCancellation_ICResponseBody Body;
  public InstalmentCancellation_ICResponse()
  {
  }
  public InstalmentCancellation_ICResponse(InstalmentCancellation_ICResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentCancellation_ICResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string InstalmentCancellation_ICResult;
  public InstalmentCancellation_ICResponseBody()
  {
  }
  public InstalmentCancellation_ICResponseBody(string InstalmentCancellation_ICResult)
  {
    this.InstalmentCancellation_ICResult = InstalmentCancellation_ICResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentMaintenance_MIRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentMaintenance_MI", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentMaintenance_MIRequestBody Body;
  public InstalmentMaintenance_MIRequest()
  {
  }
  public InstalmentMaintenance_MIRequest(InstalmentMaintenance_MIRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentMaintenance_MIRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public int Instalment;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 6)]
  public System.DateTime NewInstalmentDate;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 7)]
  public string NewTrackingCode;
  public InstalmentMaintenance_MIRequestBody()
  {
  }
  public InstalmentMaintenance_MIRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, int Instalment, System.DateTime NewInstalmentDate, string NewTrackingCode)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.Instalment = Instalment;
    this.NewInstalmentDate = NewInstalmentDate;
    this.NewTrackingCode = NewTrackingCode;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentMaintenance_MIResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentMaintenance_MIResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentMaintenance_MIResponseBody Body;
  public InstalmentMaintenance_MIResponse()
  {
  }
  public InstalmentMaintenance_MIResponse(InstalmentMaintenance_MIResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentMaintenance_MIResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string InstalmentMaintenance_MIResult;
  public InstalmentMaintenance_MIResponseBody()
  {
  }
  public InstalmentMaintenance_MIResponseBody(string InstalmentMaintenance_MIResult)
  {
    this.InstalmentMaintenance_MIResult = InstalmentMaintenance_MIResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentResubmit_RIRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentResubmit_RI", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentResubmit_RIRequestBody Body;
  public InstalmentResubmit_RIRequest()
  {
  }
  public InstalmentResubmit_RIRequest(InstalmentResubmit_RIRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentResubmit_RIRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public int Instalment;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 6)]
  public System.DateTime NewInstalmentDate;
  public InstalmentResubmit_RIRequestBody()
  {
  }
  public InstalmentResubmit_RIRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, int Instalment, System.DateTime NewInstalmentDate)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.Instalment = Instalment;
    this.NewInstalmentDate = NewInstalmentDate;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentResubmit_RIResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentResubmit_RIResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentResubmit_RIResponseBody Body;
  public InstalmentResubmit_RIResponse()
  {
  }
  public InstalmentResubmit_RIResponse(InstalmentResubmit_RIResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentResubmit_RIResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string InstalmentResubmit_RIResult;
  public InstalmentResubmit_RIResponseBody()
  {
  }
  public InstalmentResubmit_RIResponseBody(string InstalmentResubmit_RIResult)
  {
    this.InstalmentResubmit_RIResult = InstalmentResubmit_RIResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentTrackingCodeChange_TIRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentTrackingCodeChange_TI", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentTrackingCodeChange_TIRequestBody Body;
  public InstalmentTrackingCodeChange_TIRequest()
  {
  }
  public InstalmentTrackingCodeChange_TIRequest(InstalmentTrackingCodeChange_TIRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentTrackingCodeChange_TIRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
  public int Instalment;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
  public string NewTrackingCode;
  public InstalmentTrackingCodeChange_TIRequestBody()
  {
  }
  public InstalmentTrackingCodeChange_TIRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, int Instalment, string NewTrackingCode)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.Instalment = Instalment;
    this.NewTrackingCode = NewTrackingCode;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentTrackingCodeChange_TIResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentTrackingCodeChange_TIResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentTrackingCodeChange_TIResponseBody Body;
  public InstalmentTrackingCodeChange_TIResponse()
  {
  }
  public InstalmentTrackingCodeChange_TIResponse(InstalmentTrackingCodeChange_TIResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentTrackingCodeChange_TIResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string InstalmentTrackingCodeChange_TIResult;
  public InstalmentTrackingCodeChange_TIResponseBody()
  {
  }
  public InstalmentTrackingCodeChange_TIResponseBody(string InstalmentTrackingCodeChange_TIResult)
  {
    this.InstalmentTrackingCodeChange_TIResult = InstalmentTrackingCodeChange_TIResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentPreventResubmission_PRRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentPreventResubmission_PR", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentPreventResubmission_PRRequestBody Body;
  public InstalmentPreventResubmission_PRRequest()
  {
  }
  public InstalmentPreventResubmission_PRRequest(InstalmentPreventResubmission_PRRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentPreventResubmission_PRRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string Instalment;
  public InstalmentPreventResubmission_PRRequestBody()
  {
  }
  public InstalmentPreventResubmission_PRRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, string Instalment)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.Instalment = Instalment;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentPreventResubmission_PRResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentPreventResubmission_PRResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentPreventResubmission_PRResponseBody Body;
  public InstalmentPreventResubmission_PRResponse()
  {
  }
  public InstalmentPreventResubmission_PRResponse(InstalmentPreventResubmission_PRResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentPreventResubmission_PRResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string InstalmentPreventResubmission_PRResult;
  public InstalmentPreventResubmission_PRResponseBody()
  {
  }
  public InstalmentPreventResubmission_PRResponseBody(string InstalmentPreventResubmission_PRResult)
  {
    this.InstalmentPreventResubmission_PRResult = InstalmentPreventResubmission_PRResult;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentResubmissionMaintenance_RMRequest
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentResubmissionMaintenance_RM", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentResubmissionMaintenance_RMRequestBody Body;
  public InstalmentResubmissionMaintenance_RMRequest()
  {
  }
  public InstalmentResubmissionMaintenance_RMRequest(InstalmentResubmissionMaintenance_RMRequestBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentResubmissionMaintenance_RMRequestBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string LendorID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
  public string LendorType;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
  public string Username;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
  public string Password;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
  public int TransactionID;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
  public string Instalment;
  [System.Runtime.Serialization.DataMemberAttribute(Order = 6)]
  public System.DateTime NewInstalmentDate;
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 7)]
  public string NewTrackingCode;
  public InstalmentResubmissionMaintenance_RMRequestBody()
  {
  }
  public InstalmentResubmissionMaintenance_RMRequestBody(string LendorID, string LendorType, string Username, string Password, int TransactionID, string Instalment, System.DateTime NewInstalmentDate, string NewTrackingCode)
  {
    this.LendorID = LendorID;
    this.LendorType = LendorType;
    this.Username = Username;
    this.Password = Password;
    this.TransactionID = TransactionID;
    this.Instalment = Instalment;
    this.NewInstalmentDate = NewInstalmentDate;
    this.NewTrackingCode = NewTrackingCode;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class InstalmentResubmissionMaintenance_RMResponse
{
  [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstalmentResubmissionMaintenance_RMResponse", Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx", Order = 0)]
  public InstalmentResubmissionMaintenance_RMResponseBody Body;
  public InstalmentResubmissionMaintenance_RMResponse()
  {
  }
  public InstalmentResubmissionMaintenance_RMResponse(InstalmentResubmissionMaintenance_RMResponseBody Body)
  {
    this.Body = Body;
  }
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace = "https://www.nupay.co.za/wsNupayTransactions/NupayTransactionsService.asmx")]
public partial class InstalmentResubmissionMaintenance_RMResponseBody
{
  [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
  public string InstalmentResubmissionMaintenance_RMResult;
  public InstalmentResubmissionMaintenance_RMResponseBody()
  {
  }
  public InstalmentResubmissionMaintenance_RMResponseBody(string InstalmentResubmissionMaintenance_RMResult)
  {
    this.InstalmentResubmissionMaintenance_RMResult = InstalmentResubmissionMaintenance_RMResult;
  }
}
