namespace Falcon.Common.Interfaces.Services
{
  public interface IPdfService
  {
    byte[] GetPdfForMhtml(string content);
  }
}
