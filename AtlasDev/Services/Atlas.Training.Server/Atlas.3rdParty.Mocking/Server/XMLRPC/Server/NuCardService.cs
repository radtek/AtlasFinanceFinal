/***************************************************************************************************
 * 
 * 
 *  NuCard XML-RPC mocking server
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 ***************************************************************************************************/

using System;

using CookComputing.XmlRpc;

using Atlas.ThirdParty.XMLRPC.Classes;


namespace Atlas.Server.Training.XMLRPCServer
{  
  [XmlRpcService()]
  public class NuCardService : XmlRpcListenerService, INuCardService
  {
    public AllocateCard_Output AllocateCard(string terminalID, string profileNumber, string cardNumber, string firstName,
      string lastName, string idOrPassportNumber, string cellPhoneNumber, string transactionID,
      DateTime transactionDate, string checksum)
    {
      return new AllocateCard_Output
      {
        cardNumber = cardNumber,
        cellPhoneNumber = cellPhoneNumber,
        clientTransactionID = transactionID,
        firstName = firstName,
        idOrPassportNumber = idOrPassportNumber,
        lastName = lastName,
        profileNumber = profileNumber,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        terminalID = terminalID
      };
    }


    public LinkCardCard_Output LinkCard(string terminalID, string profileNumber, string cardNumber, string transactionID, DateTime transactionDate, string checksum)
    {
      return new LinkCardCard_Output
      {
        cardNumber = cardNumber,
        clientTransactionID = transactionID,
        profileNumber = profileNumber,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        terminalID = terminalID
      };
    }

   
    public Balance_Output Balance(string terminalID, string profileNumber, string cardNumber, string transactionID, DateTime transactionDate, string checksum)
    {
      return new Balance_Output
      {
        balanceAmount = 123456,
        cardNumber = cardNumber,
        clientTransactionID = transactionID,
        expired = "NO",
        expiryDate = DateTime.Now.AddDays(200),
        lost = "NO",
        profileNumber = profileNumber,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        stolen = "NO",
        stopped = "NO",
        terminalID = terminalID
      };
    }


    public DeductCardLoadProfile_Output DeductCardLoadProfile(string terminalID, string profileNumber, string cardNumber, int requestAmount, string transactionID, DateTime transactionDate, string checksum)
    {
      return new DeductCardLoadProfile_Output
      {
        authNumber = "1234567890",
        balanceAmount = 123456,
        cardNumber = cardNumber,
        clientTransactionID = transactionID,
        expiryDate = DateTime.Now.AddDays(200),
        profileNumber = profileNumber,
        requestAmount = requestAmount,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        terminalID = terminalID,
        transactionFee = 500
      };
    }


    public LoadCardDeductProfile_Output LoadCardDeductProfile(string terminalID, string profileNumber, string cardNumber, int requestAmount, string transactionID, DateTime transactionDate, string checksum)
    {
      return new LoadCardDeductProfile_Output
      {
        authNumber = "9876543210",
        balanceAmount = 123456,
        cardNumber = cardNumber,
        clientTransactionID = transactionID,
        expiryDate = DateTime.Now.AddDays(200),
        profileNumber = profileNumber,
        requestAmount = requestAmount,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        terminalID = terminalID,
        transactionFee = 500
      };
      
    }


    public TransferFunds_Output TransferFunds(string terminalID, string profileNumber, string cardNumberFrom, string cardNumberTo, int requestAmount, string transactionID, DateTime transactionDate, string checksum)
    {
      return new TransferFunds_Output
      {
        authNumber = "56789012345",
        balanceAmount = 123456,
        cardNumberFrom = cardNumberFrom,
        cardNumberTo = cardNumberTo,
        clientTransactionID = transactionID,
        expiryDate = DateTime.Now.AddDays(200),
        profileNumber = profileNumber,
        requestAmount = requestAmount,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        terminalID = terminalID,
        transactionFee = 500
      };
    }


    public Statement_Output Statement(string terminalID, string profileNumber, string cardNumber, string transactionID, DateTime transactionDate, string checksum)
    {
      var line = new Statement_Output_LineItem
      {
        transactionAmount = 112234,
        transactionDate = DateTime.Now,
        transactionDescription = "Testing the system",
        transactionType = 1
      };
      return new Statement_Output
      {
        balanceAmount = 112233,
        cardNumber = cardNumber,
        clientTransactionID = transactionID,        
        expiryDate = DateTime.Now.AddDays(200),
        profileNumber = profileNumber,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        statement = new Statement_Output_LineItem[] { line },
        terminalID = terminalID        
      };
    }


    public StopCard_Output StopCard(string terminalID, string profileNumber, string cardNumber, int stopReasonID, string transactionID, DateTime transactionDate, string checksum)
    {
      return new StopCard_Output
      {
        cardNumber = cardNumber,
        clientTransactionID = transactionID,
        profileNumber = profileNumber,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        stopReasonID = 1,
        terminalID = terminalID
      };
    }


    public UpdateAllocatedCard_Output UpdateAllocatedCard(string terminalID, string profileNumber, string cardNumber, string cellphoneNumber, string transactionID, DateTime transactionDate, string checksum)
    {
      return new UpdateAllocatedCard_Output
      {
        cardNumber = cardNumber,
        cellPhoneNumber = cellphoneNumber,
        clientTransactionID = transactionID,
        profileNumber = profileNumber,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        terminalID = terminalID
      };
    }


    public Status_Output Status(string terminalID, string profileNumber, string cardNumber, string transactionID, DateTime transactionDate, string checksum)
    {
      return new Status_Output
      {
        activated = "Yes",
        cancelled = "NO",
        empty = "NO",
        expired = "NO",
        loaded = "YES",
        lost = "NO",
        pinBlocked = "NO",
        redeemed = "",
        resultCode = 1,
        resultText = "OK",
        retired = "YES",
        serverTransactionID = Guid.NewGuid().ToString(),
        stolen = "NO",
        stopped = "NO",
        valid = "YES"
      };
    }


    public CancelStopCard_Output CancelStopCard(string terminalID, string profileNumber, string cardNumber, string transactionID, DateTime transactionDate, string checksum)
    {
      return new CancelStopCard_Output
      {
        cardNumber = cardNumber,
        clientTransactionID = transactionID,
        profileNumber = profileNumber,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        terminalID = terminalID
      };
    }


    public CheckAuthorisation_Output CheckAuthorisation(string terminalID, string profileNumber, string cardNumber, int requestAmount, string transactionID, DateTime transactionDate, string checksum)
    {
      return new CheckAuthorisation_Output
      {
        cardNumber = cardNumber,
        clientTransactionID = transactionID,
        profileNumber = profileNumber,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        terminalID = terminalID
      };
    }


    public CheckLoad_Output CheckLoad(string terminalID, string profileNumber, string cardNumber, int requestAmount, string transactionID, DateTime transactionDate, string checksum)
    {
      return new CheckLoad_Output
      {
        cardNumber = cardNumber,
        clientTransactionID = transactionID, 
        profileNumber = profileNumber,
        resultCode = 1,
        resultText = "OK",
        serverTransactionID = Guid.NewGuid().ToString(),
        terminalID = terminalID
      };
    }
  }

  
  public interface INuCardService
  {
    [XmlRpcMethod("LinkCard")]
    LinkCardCard_Output LinkCard(string terminalID, string profileNumber, string cardNumber, string transactionID,
      DateTime transactionDate, string checksum);

    [XmlRpcMethod("AllocateCard")]
    AllocateCard_Output AllocateCard(string terminalID, string profileNumber, string cardNumber, string firstName,
      string lastName, string idOrPassportNumber, string cellPhoneNumber, string transactionID,
      DateTime transactionDate, string checksum);
     
    [XmlRpcMethod("Balance")]
    Balance_Output Balance(string terminalID, string profileNumber, string cardNumber, string transactionID,
      DateTime transactionDate, string checksum);

    [XmlRpcMethod("DeductCardLoadProfile")]
    DeductCardLoadProfile_Output DeductCardLoadProfile(string terminalID, string profileNumber, string cardNumber,
      int requestAmount, string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("LoadCardDeductProfile")]
    LoadCardDeductProfile_Output LoadCardDeductProfile(string terminalID, string profileNumber, string cardNumber,
      int requestAmount, string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("TransferFunds")]
    TransferFunds_Output TransferFunds(string terminalID, string profileNumber, string cardNumberFrom, string cardNumberTo,
      int requestAmount, string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("Statement")]
    Statement_Output Statement(string terminalID, string profileNumber, string cardNumber,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("StopCard")]
    StopCard_Output StopCard(string terminalID, string profileNumber, string cardNumber, int stopReasonID,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("UpdateAllocatedCard")]
    UpdateAllocatedCard_Output UpdateAllocatedCard(string terminalID, string profileNumber, string cardNumber, string cellphoneNumber,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("Status")]
    Status_Output Status(string terminalID, string profileNumber, string cardNumber,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("CancelStopCard")]
    CancelStopCard_Output CancelStopCard(string terminalID, string profileNumber, string cardNumber,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("CheckAuthorisation")]
    CheckAuthorisation_Output CheckAuthorisation(string terminalID, string profileNumber, string cardNumber, int requestAmount,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("CheckLoad")]
    CheckLoad_Output CheckLoad(string terminalID, string profileNumber, string cardNumber, int requestAmount,
      string transactionID, DateTime transactionDate, string checksum);
  }

}