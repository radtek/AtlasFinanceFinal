using System;
using Atlas.Enumerators;

namespace Atlas.RabbitMQ.Messages.AVS
{
    [Serializable]
    public class AddAVSRequest : IMessage
    {
        public long? MessageId { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? AccountId { get; set; }
        public long? PersonId { get; set; }
        public long? CreateUserId { get; set; }
        public General.BankPeriod? BankAccountPeriod { get; set; }
        public bool? ForceAVS { get; set; }
        public bool? FireAndForget { get; set; }

        public General.Host Host { get; set; }
        public General.BankName Bank { get; set; }
        public long? CompanyId { get; set; }
        public string Initials { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        public string BranchCode { get; set; }
        public string AccountNo { get; set; }

        public Guid CorrelationId { get; }
    }
}
