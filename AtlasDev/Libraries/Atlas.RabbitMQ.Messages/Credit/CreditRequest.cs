using Atlas.Enumerators;
using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class CreditRequest : IMessage
  {
    public CreditRequest(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }


    #region Properties

    public long? AccountId { get; set; }
    public string Firstname { get;set;}
    public string Surname { get;set;}
    public string IDNumber { get;set;}
    public string Gender { get;set;}
    public DateTime DateOfBirth {get;set;}
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string Suburb { get; set; }
    public string City { get; set; }
    public string PostalCode {get;set;}
    public string Province { get; set; }
    public string HomeTelCode { get;set;}
    public string HomeTelNo { get;set;}
    public string WorkTelCode { get;set;}
    public string WorkTelNo { get;set;}
    public string CellNo { get;set;}
    public bool IsIDPassportNo { get;set;}
    public bool IsExistingClient { get;set;}
    public long RequestUser { get;set;}

    public General.Host Host { get; set; }

    #endregion

  }
}
