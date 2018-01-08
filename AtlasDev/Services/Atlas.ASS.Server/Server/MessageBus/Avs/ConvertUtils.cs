namespace Atlas.Server.MessageBus.Avs
{
  public static class ConvertUtils
  {
    public static Atlas.WCF.Interface.AVSResponse ToAVSResponse(BankVerification.EasyNetQ.AVSResponse response)
    {
      return new Atlas.WCF.Interface.AVSResponse
      {
        AccountAcceptsCredits = response.AccountAcceptsCredits,
        AccountAcceptsDebits = response.AccountAcceptsDebits,
        AccountExists = response.AccountExists,
        AccountOpen = response.AccountOpen,
        AccountOpen90days = response.AccountOpen90days,
        FinalResult = response.FinalResult,
        IdNumberMatch = response.IdNumberMatch,
        InitialsMatch = response.InitialsMatch,
        LastNameMatch = response.LastNameMatch,
        TransactionId = response.TransactionId,
        WaitingReply = response.WaitingReply
      };
    }
  }
}
