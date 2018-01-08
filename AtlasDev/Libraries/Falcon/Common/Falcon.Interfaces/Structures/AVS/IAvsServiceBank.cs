namespace Falcon.Common.Interfaces.Structures.AVS
{
  public interface IAvsServiceBank
  {
    int ServiceId { get; set; }
    long BankId { get; set; }
    string BankName { get; set; }
    bool IsLinked { get; set; }
  }
}