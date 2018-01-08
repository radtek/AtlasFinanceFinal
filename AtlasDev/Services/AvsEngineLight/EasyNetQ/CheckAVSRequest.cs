using System;

namespace BankVerification.EasyNetQ
{
  public class CheckAVSRequest
  {
    public CheckAVSRequest(Int64 transactionId)
    {
      TransactionId = transactionId;
    }


    public Int64 TransactionId { get; set; }

  }
}
