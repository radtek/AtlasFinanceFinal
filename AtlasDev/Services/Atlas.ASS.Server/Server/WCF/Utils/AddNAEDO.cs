using System;

using Atlas.Common.Extensions;


namespace Atlas.Server.WCF.Utils
{
  public static class AddNAEDO
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="clientRef1">Group ref</param>
    /// <param name="clientRef2">Client number</param>
    /// <param name="bankBranchCode"></param>
    /// <param name="bankAccountNum"></param>
    /// <param name="accountType">1- Current, 2- Saving, 3- Transmission</param>
    /// <param name="frequency">0- once off, 1- weekly, 2- fortnightly, 3- monthly, 4- first working day, 5- last working day</param>
    /// <param name="tracking">12- no tracking, 13- 1 day, 14- 3 day, 02- 4 day, 15- 7 day, 16- 14 day, 17- 21 day, 18- 32 day</param>
    /// <param name="instalments">Number of instalments</param>
    /// <param name="actionDate">First debit date</param>
    /// <param name="instalAmount">Amount to be debited</param>
    /// <param name="idOrPassport">SA id</param>
    /// <param name="accountHolderName">Name of client</param>
    public static void AddNAEDOContract(string clientRef1, string clientRef2, string bankBranchCode, string bankAccountNum, int accountType,
      int frequency, string tracking, int instalments, DateTime actionDate, decimal instalAmount, string idOrPassport, string accountHolderName)
    {      
      new wsNaedoSoapClient().Using(client =>
        {
          //client.uploadNaedoTransaction(111, "rerr", "ffsdfd", 19 /* NAEDO */, "our account?", clientRef1, clientRef2, bankAccountNum,  accountHolderName, 
          //  bankBranchCode, accountType, frequency, 14,  )

        });
    }

  }
}
