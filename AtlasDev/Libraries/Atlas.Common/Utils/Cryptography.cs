using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace Atlas.Common.Utils
{
  public class Cryptography
  {
    #region Public Methods 
    /// <summary>
    /// Encrypt the given string using AES.  The string can be decrypted using Decrypt
    ///</summary>
    public static string Encrypt(string plainText, string salt, string password)
    {
      if (string.IsNullOrEmpty(plainText))
        throw new ArgumentNullException("No text provided");

      if (string.IsNullOrEmpty(password))
        throw new ArgumentNullException("Password not provided");

      if (string.IsNullOrEmpty(salt))
        throw new ArgumentNullException("Salt not provided");

      string outStr = null;                       // Encrypted string to return
      RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data. 
      try
      {
        // generate the key from the shared secret and the salt
        var key = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(salt));

        // Create a RijndaelManaged object
        aesAlg = new RijndaelManaged();
        aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

        // Create a decrytor to perform the stream transform.
        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        // Create the streams used for encryption.
        using (var msEncrypt = new MemoryStream())
        {
          // prepend the IV
          msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
          msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
          using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
          {
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
              //Write all data to the stream.
              swEncrypt.Write(plainText);
            }
          }
          outStr = Convert.ToBase64String(msEncrypt.ToArray());
        }
      }
      finally
      {
        // Clear the RijndaelManaged object.
        if (aesAlg != null)
          aesAlg.Clear();
      }

      // Return the encrypted bytes from the memory stream.
      return outStr;
    }


    /// <summary>
    /// Decrypt the given string.  Assumes the string was encrypted using 
    /// EncryptStringAES()
    /// </summary>
    public static string Decrypt(string cipherText, string salt, string password)
    {
      if (string.IsNullOrEmpty(cipherText))
        throw new ArgumentNullException("No cipher text provided");

      if (string.IsNullOrEmpty(password))
        throw new ArgumentNullException("Password not provided");

      if (string.IsNullOrEmpty(salt))
        throw new ArgumentNullException("Salt not provided");

      // Declare the RijndaelManaged object
      // used to decrypt the data.
      RijndaelManaged aesAlg = null;

      // Declare the string used to hold
      // the decrypted text.
      string plaintext = null;

      try
      {
        // generate the key from the shared secret and the salt
        var key = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(salt));

        // Create the streams used for decryption.                
        var bytes = Convert.FromBase64String(cipherText);
        using (var msDecrypt = new MemoryStream(bytes))
        {
          // Create a RijndaelManaged object
          // with the specified key and IV.
          aesAlg = new RijndaelManaged();
          aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
          // Get the initialization vector from the encrypted stream
          aesAlg.IV = ReadByteArray(msDecrypt);
          // Create a decrytor to perform the stream transform.
          var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
          using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
          {
            using (var srDecrypt = new StreamReader(csDecrypt))

              // Read the decrypted bytes from the decrypting stream
              // and place them in a string.
              plaintext = srDecrypt.ReadToEnd();
          }
        }
      }
      catch (Exception)
      {
        throw;
      }
      finally
      {
        // Clear the RijndaelManaged object.
        if (aesAlg != null)
          aesAlg.Clear();
      }

      return plaintext;
    }


    /// <summary>
    /// Generates a salt/hash based on the password
    /// </summary>
    public static Tuple<string,string> HashPassword(string password)
    {
      byte[] salt;
      byte[] hash;

      using (var deriveBytes = new Rfc2898DeriveBytes(password, 64))  // 32-byte salt
      {
        salt = deriveBytes.Salt;
        hash = deriveBytes.GetBytes(32);  // 32-byte hash        
      }

      return new Tuple<string, string>(Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }


    /// <summary>
    /// Check if password matches
    /// </summary>
    public static bool CheckPassword(string password, string salt, string hash)
    {
      var _salt = Convert.FromBase64String(salt);
      var _hash = Convert.FromBase64String(hash);

      using (var deriveBytes = new Rfc2898DeriveBytes(password, _salt))
      {
        return deriveBytes.GetBytes(32).SequenceEqual(_hash);
      }
    }


    #endregion

    #region Private Methods

    private static byte[] ReadByteArray(Stream s)
    {
      byte[] rawLength = new byte[sizeof(int)];
      if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
      {
        throw new SystemException("Stream did not contain properly formatted byte array");
      }

      byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
      if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
      {
        throw new SystemException("Did not read byte array properly");
      }

      return buffer;
    }

    #endregion
    
  }
}
