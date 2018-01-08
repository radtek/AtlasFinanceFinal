/* Reference : http://tools.ietf.org/html/rfc4226#section-5.4
 *    The following code example describes the extraction of a dynamic
 *  binary code given that hmac_result is a byte array with the HMAC-
 *  SHA-1 result:
 *
 *       int offset   =  hmac_result[19] & 0xf ;
 *       int bin_code = (hmac_result[offset]  & 0x7f) << 24
 *          | (hmac_result[offset+1] & 0xff) << 16
 *          | (hmac_result[offset+2] & 0xff) <<  8
 *          | (hmac_result[offset+3] & 0xff) ;
 *
 *  SHA-1 HMAC Bytes (Example)
 *
 *  -------------------------------------------------------------
 *  | Byte Number                                               |
 *  -------------------------------------------------------------
 *  |00|01|02|03|04|05|06|07|08|09|10|11|12|13|14|15|16|17|18|19|
 *  -------------------------------------------------------------
 *  | Byte Value                                                |
 *  -------------------------------------------------------------
 *  |1f|86|98|69|0e|02|ca|16|61|85|50|ef|7f|19|da|8e|94|5b|55|5a|
 *  -------------------------------***********----------------++|
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AtlasOnline.Web.Core.Security
{
  public static class OneTimePinGenerator
  {
    public static string GeneratePassword(string s, long iter, int length = 6)
    {
      byte[] counter = BitConverter.GetBytes(iter);

      if (BitConverter.IsLittleEndian)
        Array.Reverse(counter);

      byte[] key = Encoding.ASCII.GetBytes(s);
      HMACSHA1 hmac = new HMACSHA1(key, true);
      byte[] hash = hmac.ComputeHash(counter);
      int offset = hash[hash.Length - 1] & 0xf;
      int binary = ((hash[offset] & 0x7f) << 24) | ((hash[offset + 1] & 0xff) << 16) | ((hash[offset + 2] & 0xff) << 8) | (hash[offset + 3] & 0xff);
      int password = binary % (int)Math.Pow(10, length);
      return password.ToString(new string('0', length));
    }
  }
}