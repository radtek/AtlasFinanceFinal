using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class RegisterEmployer
  {
    public RegisterEmployer(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }


    #region Properties

    public string LegacyBranchNo { get; set; }
    public string SequenceNo { get; set; }
    public string EmployeeNo { get; set; }
    public string EmployeeOccupation { get; set; }
    public string EmployeePayslipReference { get; set; }
    public string EmployeeSalaryFrequency { get; set; }
    public string EmployerName { get; set; }
    public string EmploymentType { get; set; }
    public string CountryOfOrigin { get; set; }
    public string IDNo { get; set; }
    public string ReferenceNo { get; set; }
    public string TransactionCreateDate { get; set; }
    public string TransactionCreateTime { get; set; }

    #endregion

  }
}
