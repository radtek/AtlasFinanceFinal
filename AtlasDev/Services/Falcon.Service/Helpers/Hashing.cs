using System.Security.Cryptography;
using System.Text;

namespace Falcon.Service.Helpers
{
  public static class Hashing
  {
    public static string GetSHA256(string strPlain)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(strPlain);
      byte[] buffer = new SHA256Managed().ComputeHash(bytes);
      string str = string.Empty;
      foreach (byte num in buffer)
      {
        str = str + string.Format("{0:x2}", num);
      }
      return str;
    }
  }
}
