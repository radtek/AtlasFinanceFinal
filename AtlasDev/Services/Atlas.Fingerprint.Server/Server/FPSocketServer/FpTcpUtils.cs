using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;


namespace Atlas.WCF.FPServer.FPSocketServer
{
  /// <summary>
  /// Class to provide utility methods for Fingerprint comms packet decoding/encoding
  /// </summary>
  internal class FpTcpUtils
  {
    #region Internal methods

    /// <summary>
    /// Creates the encrypted data packet to send to the client
    /// </summary>
    /// <param name="salt">The salt to use for encryption</param>
    /// <param name="messageType">The function/message type</param>
    /// <param name="messageNum">The current message number</param>
    /// <param name="jsonParams">JSon string cotaining optionals</param>
    /// <returns>byte array containing network packet to send</returns>
    internal static byte[] CreateMessagePacket(byte[] salt, byte[] key, FpFunctions function, UInt32 messageNum, string jsonParams = null)
    {
      #region Convert Json to UTF8
      byte[] json = null;
      var jsonLen = 0;
      if (!string.IsNullOrEmpty(jsonParams))
      {
        json = System.Text.UTF8Encoding.UTF8.GetBytes(jsonParams);
        jsonLen = json.Length;
      }
      #endregion

      #region Create data packet
      var rawPacket = new byte[jsonLen + 6];   // Message num + function
      SetUInt32(ref rawPacket, messageNum, 0);  // bytes 0..3
      SetUInt16(ref rawPacket, (UInt16)function, 4); // bytes 4..5
      if (json != null)
      {
        Array.Copy(json, 0, rawPacket, 6, jsonLen);
      }
      #endregion

      var encrypted = AES_Encrypt(rawPacket, salt, key);

      using (var ms = new MemoryStream())
      {
        #region Write the packet Header
        ms.WriteByte(0xFF);
        var len = BitConverter.GetBytes(encrypted.Length);
        ms.Write(len, 0, len.Length);
        ms.Write(salt, 0, salt.Length);
        #endregion

        ms.Write(encrypted, 0, encrypted.Length);

        return ms.ToArray();
      }
    }


    /// <summary>
    /// Read a packet from the network stream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="currPacketRecv"></param>
    /// <returns></returns>
    internal static Tuple<FpFunctions, string> DecodeMessagePacket(NetworkStream stream, UInt32 currPacketRecv, byte[] key)
    {
      Tuple<FpFunctions, string> result = null;

      #region Read and check packet header
      stream.ReadTimeout = (int)TimeSpan.FromSeconds(2).TotalMilliseconds;
      var packetHeader = new byte[21];
      if (stream.Read(packetHeader, 0, 21) != 21)
      {
        throw new FormatException("Packet header incomplete");
      }
      if (packetHeader[0] != 0xFF)
      {
        throw new FormatException("Packet header corrupted"); ;
      }
      var msgLen = (int)GetUInt32(packetHeader, 1); // bytes 1..5
      if (msgLen < 0 || msgLen > MAX_MESSAGE_SIZE)
      {
        throw new FormatException(string.Format("Packet header contained invalid message length: {0}", msgLen));
      }

      var salt = new byte[16];
      Array.Copy(packetHeader, 5, salt, 0, 16);
      #endregion

      #region Read and process body
      var encodedBody = new byte[msgLen];
      stream.ReadTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
      if (stream.Read(encodedBody, 0, msgLen) == msgLen)
      {
        // Decode body
        var decoded = AES_Decrypt(encodedBody, salt, key);

        // Check packet order
        var packetNum = GetUInt32(decoded, 0);
        if (currPacketRecv != packetNum)
        {
          throw new Exception(string.Format("Out of sequence packet received- Expected: {0}, Received: {1}", packetNum, currPacketRecv));
        }

        string json = null;
        if (decoded.Length > 6)
        {
          json = System.Text.UTF8Encoding.UTF8.GetString(decoded, 6, decoded.Length - 6);
        }

        result = new Tuple<FpFunctions, string>((FpFunctions)GetUInt16(decoded, 4), json);
      }
      #endregion

      return result;
    }

    #endregion


    #region Private methods

    /// <summary>
    /// Derives a UInt32 from 4 bytes
    /// </summary>
    /// <param name="b0"></param>
    /// <param name="b1"></param>
    /// <param name="b2"></param>
    /// <param name="b3"></param>
    /// <returns></returns>
    private static UInt32 GetUInt32(byte[] values, int index = 0)
    {
      return BitConverter.ToUInt32(values, index);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static UInt16 GetUInt16(byte[] values, int index = 0)
    {
      return BitConverter.ToUInt16(values, index);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <param name="value"></param>
    /// <param name="index"></param>
    private static void SetUInt32(ref byte[] values, UInt32 value, int index = 0)
    {
      var bytes = BitConverter.GetBytes(value);
      Array.Copy(bytes, 0, values, index, 4);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <param name="value"></param>
    /// <param name="index"></param>
    private static void SetUInt16(ref byte[] values, UInt16 value, int index = 0)
    {
      var bytes = BitConverter.GetBytes(value);
      Array.Copy(bytes, 0, values, index, 2);
    }


    /// <summary>
    /// Server AES encrypt a byte[] array using key and given salt
    /// </summary>
    /// <param name="value">The byte array to encrypt</param>
    /// <param name="salt">The salt to use for encryption</param>
    /// <returns>Encrypted bytes array, 8-byte aligned</returns>
    private static byte[] AES_Encrypt(byte[] value, byte[] salt, byte[] key)
    {
      byte[] encryptedBytes = null;

      using (var ms = new MemoryStream())
      {
        using (var AES = new AesCryptoServiceProvider())
        {
          AES.KeySize = 256;
          AES.BlockSize = 128;

          var derived = new Rfc2898DeriveBytes(key, salt, 10);
          AES.Key = derived.GetBytes(AES.KeySize / 8);
          AES.IV = derived.GetBytes(AES.BlockSize / 8);

          AES.Mode = CipherMode.CBC;

          using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
          {
            cs.Write(value, 0, value.Length);
            cs.Close();
          }
          encryptedBytes = ms.ToArray();
        }
      }

      return encryptedBytes;
    }


    /// <summary>
    /// Server AES decrypt a given byte[] array. 
    /// Not a corollary of the above- client uses different key to send.
    /// </summary>
    /// <param name="value">The encrypted byte array to decrypt</param>
    /// <param name="salt">The salt to use for encryption</param>
    /// <returns>Decrypted byte array</returns>
    private static byte[] AES_Decrypt(byte[] value, byte[] salt, byte[] key)
    {
      byte[] decryptedBytes = null;

      using (var ms = new MemoryStream())
      {
        using (var AES = new AesCryptoServiceProvider())
        {
          AES.KeySize = 256;
          AES.BlockSize = 128;

          var derived = new Rfc2898DeriveBytes(key, salt, 10);
          AES.Key = derived.GetBytes(AES.KeySize / 8);
          AES.IV = derived.GetBytes(AES.BlockSize / 8);

          AES.Mode = CipherMode.CBC;

          using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
          {
            cs.Write(value, 0, value.Length);
            cs.Close();
          }
          decryptedBytes = ms.ToArray();
        }
      }

      return decryptedBytes;
    }

    #endregion

    internal enum FpFunctions
    {
      Register = 1,
      Ping = 2,
      Enroll = 3,
      Verify = 4,
      Identify = 5,
      Abort = 6
    };


    #region Private consts

    private const int MAX_MESSAGE_SIZE = 65000;

    #endregion

  }


  #region Classes for Json parameters

  /// <summary>
  /// Class for 'Register' message
  /// </summary>
  internal class RegisterMachine
  {
    public string IPAddressesCsv { get; set; }

    public string MachineName { get; set; }

    public string MachineFingerprint { get; set; }
  }


  /// <summary>
  /// Class for 'Client Activation' message
  /// </summary>
  internal class ClientActivateInfo
  {
    public long AdminPersonId { get; set; }
    public string MessageId { get; set; }
    public string Message1 { get; set; }
    public string Message2 { get; set; }
    public string Message3 { get; set; }
     public long PersonId { get; set; }
    public int TimeoutSeconds { get; set; }
    public long UserPersonId { get; set; }
  }

  #endregion

}
