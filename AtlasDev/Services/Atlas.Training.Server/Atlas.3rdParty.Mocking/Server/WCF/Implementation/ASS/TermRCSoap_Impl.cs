using System;
using System.Threading;

using Atlas.ThirdParty.WCF;


namespace Atlas.Server.Training
{
  internal class TermRCSoap_Impl : TermRCSoap
  {
    public aedo_auth_reqResponse aedo_auth_req(aedo_auth_reqRequest request)
    {
      throw new NotImplementedException();
    }


    public aedo_naedo_auth_reqResponse aedo_naedo_auth_req(aedo_naedo_auth_reqRequest request)
    {
      throw new NotImplementedException();
    }


    public miniStatementResponse miniStatement(miniStatementRequest request)
    {
      throw new NotImplementedException();
    }


    public merchTermVerifyWebResponse merchTermVerifyWeb(merchTermVerifyWebRequest request)
    {
      throw new NotImplementedException();
    }


    public printCustSlipResponse printCustSlip(printCustSlipRequest request)
    {
      throw new NotImplementedException();
    }


    public OneCor_AEDO_Auth_ReqResponse OneCor_AEDO_Auth_Req(OneCor_AEDO_Auth_ReqRequest request)
    {
      throw new NotImplementedException();
    }


    public ABILCardRegResponse ABILCardReg(ABILCardRegRequest request)
    {
      throw new NotImplementedException();
    }


    public handshakeResponse handshake(handshakeRequest request)
    {
      return new handshakeResponse
      {
        Body = new handshakeResponseBody
        {
          handshakeResult = new HandShakeRsp()
          {
            DeviceID = request.Body.Merchant_ID,
            status = "00"
          }
        }
      };
    }


    public genericDataCaptureConfirmResponse genericDataCaptureConfirm(genericDataCaptureConfirmRequest request)
    {
      throw new NotImplementedException();
    }


    public genericDataCaptureResponse genericDataCapture(genericDataCaptureRequest request)
    {
      throw new NotImplementedException();
    }


    public cashSendTestAppResponse cashSendTestApp(cashSendTestAppRequest request)
    {
      throw new NotImplementedException();
    }


    public UpfrontBinCheckWebResponse UpfrontBinCheckWeb(UpfrontBinCheckWebRequest request)
    {
      throw new NotImplementedException();
    }


    public TermStatusCheckResponse TermStatusCheck(TermStatusCheckRequest request)
    {
      return new TermStatusCheckResponse
      {
        Body = new TermStatusCheckResponseBody
        {
          TermStatusCheckResult = new TermStatus
          {
            lastUpdated = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss tt"),
            responseCode = "00",
            status = "O" // O= online, F= ?
          }
        }
      };
    }


    public UpfrontBinCheckResponse UpfrontBinCheck(UpfrontBinCheckRequest request)
    {
      throw new NotImplementedException();
    }


    public tranIDQueryResponse tranIDQuery(tranIDQueryRequest request)
    {
      throw new NotImplementedException();
    }


    public AFS_AN_Auth_ReqResponse AFS_AN_Auth_Req(AFS_AN_Auth_ReqRequest request)
    {
      Thread.Sleep(5000);

      return new AFS_AN_Auth_ReqResponse
      {
        Body = new AFS_AN_Auth_ReqResponseBody
        {
          AFS_AN_Auth_ReqResult = new AFSRsp
          {
            AccountNumber = request.Body.accountNumberIn,
            AccountType = "1",
            AdjRule = request.Body.adj_rule,
            ApprovalCode = "",
            ContractAmount = request.Body.contract_amnt,
            contractType = "A",
            Frequency = request.Body.frequency,
            PAN = request.Body.panIn,
            ResponseCode = "00",
            ResponseDescription = "",
            Tracking = request.Body.tracking,
            TransactionID = new Random().Next(int.MaxValue / 2 + int.MaxValue / 3).ToString("N")
          }
        }
      };
    }


    public AFS_AN_Auth_Req_TestResponse AFS_AN_Auth_Req_Test(AFS_AN_Auth_Req_TestRequest request)
    {
      return new AFS_AN_Auth_Req_TestResponse(
        new AFS_AN_Auth_Req_TestResponseBody(
          new AFSRsp
            {
              AccountNumber = request.Body.accountNumberIn,
              AccountType = request.Body.account_Type,
              contractType = "A",
              PAN = request.Body.panIn,
              ResponseCode = "00",
              TransactionID = new Random().Next(int.MaxValue / 2 + int.MaxValue / 3).ToString("N")
            }));
    }


    public aedo_naedo_auth_req_testResponse aedo_naedo_auth_req_test(aedo_naedo_auth_req_testRequest request)
    {
      var rand = new Random();
      return new aedo_naedo_auth_req_testResponse(
        new aedo_naedo_auth_req_testResponseBody(
          new AuthRspN
          {
            AccountNumber = request.Body.accountNumberIn,
            AccountType = "A",
            AdjRule = request.Body.adj_rule,
            Frequency = request.Body.frequency,
            PAN = request.Body.panIn,
            ResponseCode = "00",
            ResponseDescription = "",
            Tracking = request.Body.tracking,
            TransactionID = new Random().Next(int.MaxValue / 2 + int.MaxValue / 3).ToString("N")
          }
      ));
    }


    public displayMessageResponse displayMessage(displayMessageRequest request)
    {
      throw new NotImplementedException();
    }
  }
}
