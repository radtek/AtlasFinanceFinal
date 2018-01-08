/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
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
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Atlas.Common.Utils
{
  /// <summary>
  /// Basic crypto class
  /// </summary>
  public class BasicCrypto
  {
    /// <summary>
    /// Converts a byte array to its hex string representation
    /// </summary>
    /// <param name="value">The byte array to represent</param>
    /// <returns>string containing hex value of value parameter</returns>
    public static string ByteToHex(byte[] value)
    {
      return BitConverter.ToString(value).Replace("-", string.Empty);
    }


    /// <summary>
    /// Converts a hex string to its byte array
    /// </summary>
    /// <param name="value">Hex string</param>
    /// <returns>byte array of the hex string</returns>
    public static byte[] HexToByte(string value)
    {
      var result = new byte[(int)value.Length / 2];
      for (var i = 0; i < (int)value.Length / 2; i++)
      {
        result[i] = Byte.Parse(value.Substring(i * 2, 2), NumberStyles.HexNumber);
      }
      return result;
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
    /// Returns a random SHA-512 hasg as a byte array
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
    public static string Decrypt(string iv, string key, string data)
    {
      if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(iv) && !string.IsNullOrEmpty(key))
      {
        using (var crypt = new RijndaelManaged() { KeySize = 256 })
        {
          using (var derivedKey = new Rfc2898DeriveBytes(key, Encoding.ASCII.GetBytes(iv)))
          {
            crypt.Key = derivedKey.GetBytes(crypt.KeySize / 8);
            crypt.IV = derivedKey.GetBytes(crypt.BlockSize / 8);
          }

          var byteData = HexToByte(data);
          using (var ms = new MemoryStream())
          {
            var cs = new CryptoStream(ms, crypt.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(byteData, 0, byteData.Length);
            cs.Close();
            var result = Encoding.ASCII.GetString(ms.ToArray());
            // Strip off salt
            if (result.Length > 10)
              return result.Substring(10);
          }
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
        using (var crypt = new RijndaelManaged() { KeySize = 256 })
        {
          using (var derivedKey = new Rfc2898DeriveBytes(key, Encoding.ASCII.GetBytes(iv)))
          {
            crypt.Key = derivedKey.GetBytes(crypt.KeySize / 8);
            crypt.IV = derivedKey.GetBytes(crypt.BlockSize / 8);
          }

          #region Salt the data with 10 random hex chars
          var saltBytes = new byte[5];
          using (var rng = new RNGCryptoServiceProvider())
          {
            rng.GetNonZeroBytes(saltBytes);
          }
          data = string.Format("{0}{1}", ByteToHex(saltBytes), data);
          #endregion

          using (var ms = new MemoryStream())
          {
            var cs = new CryptoStream(ms, crypt.CreateEncryptor(), CryptoStreamMode.Write);
            var rawText = Encoding.ASCII.GetBytes(data);
            cs.Write(rawText, 0, rawText.Length);
            cs.Close();
            return ByteToHex(ms.ToArray());
          }
        }
      }

      return data;
    }

  }
}