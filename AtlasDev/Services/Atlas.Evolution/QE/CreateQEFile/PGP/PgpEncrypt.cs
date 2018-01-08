using System;
using System.Collections.Generic;
using System.IO;

using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Security;


namespace Atlas.Evolution.Server.Code.PGP
{
  /// <summary>
  /// BCX/SACCRA compliant PGP public-key based file encryption- source code reflected from the DMA executable and cleaned up.
  /// </summary>
  internal class PgpEncrypt
  {
    /// <summary>
    /// PGP encrypt a file
    /// </summary>
    /// <param name="log">Logging</param>
    /// <param name="publicKeyFilePath">Path to where PGP public key files are located</param>
    /// <param name="inputFile">Full path to source file to encrypt</param>
    /// <param name="outputFile">Full path of encrypted file to create</param>
    /// <returns>true for success, false for failure</returns>
    public static bool EncryptFile(string publicKeyFilePath, string inputFile, string outputFile)
    {
      var publicKeys = LoadPublicKeys(publicKeyFilePath);
      if (publicKeys.Count == 0)
      {
        Console.WriteLine($"Failed to locate any public keys in {publicKeyFilePath}");
        return false;
      }

      var successful = false;
      try
      {
        using (var outputStream = File.Open(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
        {
          successful = EncryptFileForMultipleReceipients(outputStream, inputFile, publicKeys, true, true);
          outputStream.Flush();
          outputStream.Close();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      return successful;
    }


    /// <summary>
    /// Perform PGP encryption using given parameters
    /// </summary>
    /// <param name="log">Logging</param>
    /// <param name="outputStream">Output file stream</param>
    /// <param name="fileName">Name of file to encrypt</param>
    /// <param name="encKey">List of loaded public keys</param>
    /// <param name="armor">Enabled armor</param>
    /// <param name="withIntegrityCheck">Enable integrity check</param>
    /// <returns></returns>
    private static bool EncryptFileForMultipleReceipients(Stream outputStream, string fileName, 
      List<PgpPublicKey> encKey, bool armor = true, bool withIntegrityCheck = true)
    {
      var foundKey = false;
      if (armor)
      {
        outputStream = (Stream)new ArmoredOutputStream(outputStream);
      }

      try
      {
        var memoryStream = new MemoryStream();
        var compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
        PgpUtilities.WriteFileToLiteralData(compressedDataGenerator.Open(memoryStream), PgpLiteralData.Binary, new FileInfo(fileName));
        compressedDataGenerator.Close();
        var encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());

        foreach (var key in encKey)
        {          
          if (key.CreationTime.AddSeconds((double)key.GetValidSeconds()) >= DateTime.UtcNow)
          {
            encryptedDataGenerator.AddMethod(key);
            foundKey = true;
          }
        }

        if (foundKey)
        {
          var buffer = memoryStream.ToArray();
          var stream = encryptedDataGenerator.Open(outputStream, (long)buffer.Length);
          stream.Write(buffer, 0, buffer.Length);
          stream.Close();
          if (armor)
          {
            outputStream.Close();
          }
        }

        return foundKey;
      }
      catch (PgpException ex)
      {
        Console.WriteLine(ex.Message);
        return false;
      }
    }


    /// <summary>
    /// Loads public keys in given path
    /// </summary>
    /// <param name="log">Logging</param>
    /// <param name="publicKeyFilePath">Path of public key files</param>
    /// <returns></returns>
    private static List<PgpPublicKey> LoadPublicKeys(string publicKeyFilePath)
    {
      var files = Directory.GetFiles(publicKeyFilePath, "*Public.asc");
      var list = new List<PgpPublicKey>();
      for (var index = 0; index < files.Length; ++index)
      {
        try
        {
          using (var inputStream = File.OpenRead(files[index]))
          {
            list.Add(ReadPublicKey(inputStream));
            inputStream.Close();
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
      return list;
    }


    private static PgpPublicKey ReadPublicKey(Stream inputStream)
    {
      inputStream = PgpUtilities.GetDecoderStream(inputStream);
      foreach (PgpPublicKeyRing pgpPublicKeyRing in new PgpPublicKeyRingBundle(inputStream).GetKeyRings())
      {
        foreach (PgpPublicKey pgpPublicKey in pgpPublicKeyRing.GetPublicKeys())
        {
          if (pgpPublicKey.IsEncryptionKey)
            return pgpPublicKey;
        }
      }

      throw new Exception("Failed to locate");
    }
    
  }
}
