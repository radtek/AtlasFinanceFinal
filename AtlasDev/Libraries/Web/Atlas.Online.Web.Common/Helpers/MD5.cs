using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Web.Common.Helpers
{
  public static class MD5
  {
    public static string Digest(string str)
    {
      // build up image url, including MD5 hash for supplied email:
      MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

      UTF8Encoding encoder = new UTF8Encoding();
      MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

      byte[] hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(str));

      StringBuilder sb = new StringBuilder(hashedBytes.Length * 2);
      for (int i = 0; i < hashedBytes.Length; i++)
      {
        sb.Append(hashedBytes[i].ToString("X2"));
      }

      return sb.ToString();
    }
  }
}
