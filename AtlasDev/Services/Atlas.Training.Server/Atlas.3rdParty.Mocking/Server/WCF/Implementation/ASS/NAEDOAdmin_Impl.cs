using System;


namespace Atlas.Server.Training
{
  internal class NAEDOAdmin_Impl: NAEDOAdmin
  {
    public acountChangeResponse acountChange(acountChangeRequest request)
    {
      throw new NotImplementedException();
    }


    public aedoConvertResponse aedoConvert(aedoConvertRequest request)
    {
      throw new NotImplementedException();
    }


    public contractActivationResponse contractActivation(contractActivationRequest request)
    {
      throw new NotImplementedException();
    }


    public contractCancellationResponse contractCancellation(contractCancellationRequest request)
    {
      var doc = new System.Xml.XmlDocument();
      doc.LoadXml("<CancellationReportHeader><status>00</status></CancellationReportHeader>");
      return new contractCancellationResponse
      {
        Body = new contractCancellationResponseBody
        {          
          contractCancellationResult = doc.DocumentElement
        }
      };
    }


    public contractDateChangeResponse contractDateChange(contractDateChangeRequest request)
    {
      throw new NotImplementedException();
    }


    public contractTrackingChangeResponse contractTrackingChange(contractTrackingChangeRequest request)
    {
      throw new NotImplementedException();
    }


    public instalmentAmountChangeResponse instalmentAmountChange(instalmentAmountChangeRequest request)
    {
      throw new NotImplementedException();
    }


    public instalmentCancellationResponse instalmentCancellation(instalmentCancellationRequest request)
    {
      throw new NotImplementedException();
    }


    public instalmentDateChangeResponse instalmentDateChange(instalmentDateChangeRequest request)
    {
      throw new NotImplementedException();
    }


    public instalmentMaintenanceResponse instalmentMaintenance(instalmentMaintenanceRequest request)
    {
      throw new NotImplementedException();
    }


    public instalmentRescheduleResponse instalmentReschedule(instalmentRescheduleRequest request)
    {
      throw new NotImplementedException();
    }

    public instalmentRescheduleMaintenanceResponse instalmentRescheduleMaintenance(instalmentRescheduleMaintenanceRequest request)
    {
      throw new NotImplementedException();
    }

    public instalmentTrackingChangeResponse instalmentTrackingChange(instalmentTrackingChangeRequest request)
    {
      throw new NotImplementedException();
    }

    public instalmentRecallResponse instalmentRecall(instalmentRecallRequest request)
    {
      throw new NotImplementedException();
    }

    public getTPFReportResponse getTPFReport(getTPFReportRequest request)
    {
      throw new NotImplementedException();
    }


    public checkTPFResponse checkTPF(checkTPFRequest request)
    {
      throw new NotImplementedException();
    }


    public uploadTSPTransactionResponse uploadTSPTransaction(uploadTSPTransactionRequest request)
    {
      throw new NotImplementedException();
    }


    public uploadCCTransactionResponse uploadCCTransaction(uploadCCTransactionRequest request)
    {
      throw new NotImplementedException();
    }


    public uploadNaedoTransactionResponse uploadNaedoTransaction(uploadNaedoTransactionRequest request)
    {
      throw new NotImplementedException();
    }


    public updateInstalmentResponse updateInstalment(updateInstalmentRequest request)
    {
      throw new NotImplementedException();
    }


    public cancelTransactionResponse cancelTransaction(cancelTransactionRequest request)
    {
      throw new NotImplementedException();
    }


    public cancelInstalmentResponse cancelInstalment(cancelInstalmentRequest request)
    {
      throw new NotImplementedException();
    }


    public CDVCheckResponse CDVCheck(CDVCheckRequest request)
    {
      throw new NotImplementedException();
    }


    public updateTSPTransactionResponse updateTSPTransaction(updateTSPTransactionRequest request)
    {
      throw new NotImplementedException();
    }

    public updateNaedoTransactionResponse updateNaedoTransaction(updateNaedoTransactionRequest request)
    {
      throw new NotImplementedException();
    }


    public updateCCTransactionResponse updateCCTransaction(updateCCTransactionRequest request)
    {
      throw new NotImplementedException();
    }


    public getReportResponse getReport(getReportRequest request)
    {
      throw new NotImplementedException();
    }


    public requestProgressNAEDOResponse requestProgressNAEDO(requestProgressNAEDORequest request)
    {
      throw new NotImplementedException();
    }
  }
}
