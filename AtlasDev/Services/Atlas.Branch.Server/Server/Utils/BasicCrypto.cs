/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Basic AES Encryption routines
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *    
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Atlas.Desktop.Utils.Crypto
{
  /// <summary>
  /// Basic crypto class
  /// </summary>
  public class BasicCrypto
  {
    /// <summary>
    /// Converts a byte array to its hex ASCII string representation
    /// </summary>
    /// <param name="value">The byte array to represent</param>
    /// <returns>string containing hex value of value parameter</returns>
    public static string ByteToHex(byte[] bytes)
    {
      var c = new char[bytes.Length * 2];
      int b;
      for (var i = 0; i < bytes.Length; i++)
      {
        b = bytes[i] >> 4;
        c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
        b = bytes[i] & 0xF;
        c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
      }
      return new string(c);
    }


    /// <summary>
    /// Converts a hex ASCII string to its byte array
    /// </summary>
    /// <param name="value">Hex string</param>
    /// <returns>byte array of the hex string</returns>
    public static byte[] HexToByte(string hex)
    {
      if (hex.Length % 2 == 1)
        throw new Exception("The binary key cannot have an odd number of digits");

      var arr = new byte[hex.Length >> 1];

      for (var i = 0; i < hex.Length >> 1; ++i)
      {
        arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
      }

      return arr;
    }

    public static int GetHexVal(char hex)
    {
      var val = (int)hex;
      //For uppercase A-F letters:
      return val - (val < 58 ? 48 : 55);
      //For lowercase a-f letters:
      //return val - (val < 58 ? 48 : 87);
      //Or the two combined, but a bit slower:
      //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }


    /// <summary>
    /// Returns a random SHA-512 hash as a hex string
    /// </summary>
    /// <returns></returns>
    public static string GetRandomSHA512Hash()
    {
      var result = "";

      // Create an arbitrary buffer and fill it with random bytes
      const int RAND_CHARS = 1000;
      var buffer = new byte[RAND_CHARS];
      var rnd = new Random();
      rnd.NextBytes(buffer);

      // Hash this random buffer
      using (var hash = new SHA512Managed())
      {
        var hashVal = hash.ComputeHash(buffer);
        // Convert hash byte array to hex string
        result = ByteToHex(hashVal);
      }
      return result;
    }


    /// <summary>
    /// Returns a random SHA-512 hash as a byte array
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetSHA512Hash(string value)
    {
      var result = "";

      // Hash string
      using (var hash = new SHA512Managed())
      {
        var buffer = Encoding.ASCII.GetBytes(value);
        var hashVal = hash.ComputeHash(buffer);
        // Convert hash byte array to hex string
        result = ByteToHex(hashVal);
      }
      return result;
    }


    /// <summary>
    /// Takes a 'hex' represented string and decrypts it to a string
    /// </summary>
    /// <param name="iv">Initialization vector</param>
    /// <param name="key">Key/password</param>
    /// <param name="data">'Hex' encrypted string</param>
    /// <returns>Decrypted string data</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    public static string Decrypt(string iv, string key, string data)
    {
      if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(iv) && !string.IsNullOrEmpty(key))
      {
        Rfc2898DeriveBytes derivedKey = null;
        MemoryStream ms = null;
        CryptoStream cs = null;
        try
        {
          using (var crypt = new RijndaelManaged() { KeySize = 256 })
          {
            derivedKey = new Rfc2898DeriveBytes(key, Encoding.ASCII.GetBytes(iv));
            crypt.Key = derivedKey.GetBytes(crypt.KeySize / 8);
            crypt.IV = derivedKey.GetBytes(crypt.BlockSize / 8);

            var byteData = HexToByte(data);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, crypt.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(byteData, 0, byteData.Length);
            cs.Close();
          }

          var result = Encoding.ASCII.GetString(ms.ToArray());
          // Strip off salt
          if (result.Length > 10)
            return result.Substring(10);
        }
        finally
        {
          if (derivedKey != null)
            derivedKey.Dispose();
          if (ms != null)
            ms.Dispose();
          if (cs != null)
            cs.Dispose();
        }
      }

      return data;
    }


    /// <summary>
    /// Takes a string and encrypts to a hex string (ASCII only!)
    /// </summary>
    /// <param name="ivHash">IV string</param>
    /// <param name="keyHash">Password/key</param>
    /// <param name="data">Plain string to encrypt</param>
    /// <returns>Hex-coded encrypted string</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    public static string Encrypt(string iv, string key, string data)
    {
      if (iv.Length < 8)
      {
        throw new ArgumentException("Parameter must be at least 8 characters", "iv");
      }

      if (key.Length < 5)
      {
        throw new ArgumentException("Parameter must be at least 8 characters", "key");
      }

      if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(iv) && !string.IsNullOrEmpty(key))
      {
        Rfc2898DeriveBytes derivedKey = null;
        RNGCryptoServiceProvider rng = null;
        MemoryStream ms = null;
        CryptoStream cs = null;
        try
        {
          using (var crypt = new RijndaelManaged() { KeySize = 256 })
          {
            derivedKey = new Rfc2898DeriveBytes(key, Encoding.ASCII.GetBytes(iv));
            crypt.Key = derivedKey.GetBytes(crypt.KeySize / 8);
            crypt.IV = derivedKey.GetBytes(crypt.BlockSize / 8);

            #region Salt the data with 10 random hex chars
            var saltBytes = new byte[5];
            rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(saltBytes);
            #endregion

            ms = new MemoryStream();
            cs = new CryptoStream(ms, crypt.CreateEncryptor(), CryptoStreamMode.Write);
            var rawText = Encoding.ASCII.GetBytes(data);
            cs.Write(rawText, 0, rawText.Length);
            cs.Close();

            return ByteToHex(ms.ToArray());
          }
        }
        finally
        {
          if (derivedKey != null)
            derivedKey.Dispose();
          if (rng != null)
            rng.Dispose();
          if (ms != null)
            ms.Dispose();
          if (cs != null)
            cs.Dispose();
        }
      }

      return data;
    }
  }
}