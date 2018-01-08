using System;
using System.IO;

using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;


namespace TestPGP
{
  // http://stackoverflow.com/questions/4192296/c-sharp-how-to-simply-encrypt-a-text-file-with-a-pgp-public-key
  class Program
  {
    static void Main()
    {
      var pkr = AsciiPublicKeyToRing("PubicKey.pkp");
      if (pkr != null)
      {
        try
        {
          EncryptFile("Source.txt", "Dest.txt.pgp", GetFirstPublicEncryptionKeyFromRing(pkr), true, true);
        }
        catch (Exception ex)
        {

        }
      }
      else
      {
        // " is not a public key.");
      }
    }


    public static void EncryptFile(string inputFile, string outputFile, PgpPublicKey encKey, bool armor,
        bool withIntegrityCheck)
    {
      using (var bOut = new MemoryStream())
      {
        var comData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
        PgpUtilities.WriteFileToLiteralData(comData.Open(bOut), PgpLiteralData.Binary,
            new FileInfo(inputFile));

        comData.Close();
        var cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, withIntegrityCheck, new SecureRandom());

        cPk.AddMethod(encKey);
        var bytes = bOut.ToArray();

        using (var outputStream = File.Create(outputFile))
        {
          if (armor)
          {
            using (var armoredStream = new ArmoredOutputStream(outputStream))
            using (var cOut = cPk.Open(armoredStream, bytes.Length))
            {
              cOut.Write(bytes, 0, bytes.Length);
            }
          }
          else
          {
            using (var cOut = cPk.Open(outputStream, bytes.Length))
            {
              cOut.Write(bytes, 0, bytes.Length);
            }
          }
        }
      }
    }

        
    private static PgpPublicKeyRing AsciiPublicKeyToRing(string ascfilein)
    {
      using (var pubFis = File.OpenRead(ascfilein))
      {
        var pubArmoredStream = new ArmoredInputStream(pubFis);

        var pgpFact = new PgpObjectFactory(pubArmoredStream);
        Object opgp = pgpFact.NextPgpObject();
        var pkr = opgp as PgpPublicKeyRing;
        return pkr;
      }
    }


    private static PgpPublicKey GetFirstPublicEncryptionKeyFromRing(PgpPublicKeyRing pkr)
    {
      foreach (PgpPublicKey k in pkr.GetPublicKeys())
      {
        if (k.IsEncryptionKey)
          return k;
      }
      throw new ArgumentException("Can't find encryption key in key ring.");
    }


  }
}
