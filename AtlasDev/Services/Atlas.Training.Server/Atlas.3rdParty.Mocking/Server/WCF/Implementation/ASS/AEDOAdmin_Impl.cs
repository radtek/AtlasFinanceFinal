using System;


namespace Atlas.Server.Training
{
  internal class AEDOAdmin_Impl : AEDOAdmin
  {
    public ContractActivation_ACResponse ContractActivation_AC(ContractActivation_ACRequest request)
    {
      throw new NotImplementedException();
    }

    public ContractCancellation_CCResponse ContractCancellation_CC(ContractCancellation_CCRequest request)
    {
      return new ContractCancellation_CCResponse { Body = new ContractCancellation_CCResponseBody { ContractCancellation_CCResult = "00" } };
    }

    public ContractDateChange_DCResponse ContractDateChange_DC(ContractDateChange_DCRequest request)
    {
      throw new NotImplementedException();
    }

    public EmployerCodeChange_ECResponse EmployerCodeChange_EC(EmployerCodeChange_ECRequest request)
    {
      throw new NotImplementedException();
    }

    public FrequencyChange_FCResponse FrequencyChange_FC(FrequencyChange_FCRequest request)
    {
      throw new NotImplementedException();
    }

    public ContractNumberChangeActivation_NAResponse ContractNumberChangeActivation_NA(ContractNumberChangeActivation_NARequest request)
    {
      throw new NotImplementedException();
    }

    public ContractNumberChange_NCResponse ContractNumberChange_NC(ContractNumberChange_NCRequest request)
    {
      throw new NotImplementedException();
    }

    public ContractTrackingCodeChange_TCResponse ContractTrackingCodeChange_TC(ContractTrackingCodeChange_TCRequest request)
    {
      throw new NotImplementedException();
    }

    public IdNumberChange_ZCResponse IdNumberChange_ZC(IdNumberChange_ZCRequest request)
    {
      throw new NotImplementedException();
    }

    public ContractPendingDateChange_FDResponse ContractPendingDateChange_FD(ContractPendingDateChange_FDRequest request)
    {
      throw new NotImplementedException();
    }

    public ContractDateChangeActivation_FAResponse ContractDateChangeActivation_FA(ContractDateChangeActivation_FARequest request)
    {
      throw new NotImplementedException();
    }

    public InstalmentDateChange_DIResponse InstalmentDateChange_DI(InstalmentDateChange_DIRequest request)
    {
      throw new NotImplementedException();
    }

    public InstalmentCancellation_ICResponse InstalmentCancellation_IC(InstalmentCancellation_ICRequest request)
    {
      throw new NotImplementedException();
    }

    public InstalmentMaintenance_MIResponse InstalmentMaintenance_MI(InstalmentMaintenance_MIRequest request)
    {
      throw new NotImplementedException();
    }

    public InstalmentResubmit_RIResponse InstalmentResubmit_RI(InstalmentResubmit_RIRequest request)
    {
      throw new NotImplementedException();
    }

    public InstalmentTrackingCodeChange_TIResponse InstalmentTrackingCodeChange_TI(InstalmentTrackingCodeChange_TIRequest request)
    {
      throw new NotImplementedException();
    }

    public InstalmentPreventResubmission_PRResponse InstalmentPreventResubmission_PR(InstalmentPreventResubmission_PRRequest request)
    {
      throw new NotImplementedException();
    }

    public InstalmentResubmissionMaintenance_RMResponse InstalmentResubmissionMaintenance_RM(InstalmentResubmissionMaintenance_RMRequest request)
    {
      throw new NotImplementedException();
    }
  }
}
