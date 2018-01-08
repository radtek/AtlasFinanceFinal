/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 * 
 * 
 *  Description:
 *  ------------------
 *    Basic ASS crypto routines
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 *     
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


namespace Atlas.Common.Utils
{
  /// <summary>
  /// Basic Ass/Atlas clipper crypto implementation
  /// </summary>
  public static class ClipperCrypto
  {  
    public static string ASSEncrypt(string m_text, int m_sign)
    {
      var CHARSTR = "S R0Q9P8O7N6M5L4K3J2I1HZGYFXEWDVCUBTAzmylxkwjviuhtgsfreqdpcobna";
      var CODESTR = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890 abcdefghijklmnopqrstuvwxyz";

      var m_maxlen = CHARSTR.Length;                                                // local m_maxlen := len(CHARSTR)   && in lansoft.ch NOTE
      var m_convert = string.Empty;                                                 // local m_convert := ''

      var m_chrno = 0;
      char m_repchr;
      string m_chrstr1;
      string m_chrstr2;

      if (string.IsNullOrEmpty(m_text))                                            // if empty(m_text)
      {
        m_convert = m_text;
        m_text = m_text.Trim();
      }

      if (m_sign == 1)
      {
        m_chrstr1 = CHARSTR;
        m_chrstr2 = CODESTR;
      }
      else
      {
        m_chrstr1 = CODESTR;
        m_chrstr2 = CHARSTR;
      }

      for (var i = 1; i <= m_text.Length; i++)                                      // for i=1 to len(m_text)
      {
        m_chrno = (m_chrstr1.IndexOf((char)m_text[i - 1]) + 1) + i * m_sign;      // m_chrno = at(substr(m_text,i,1),m_chrstr1) + i * m_sign
        if (m_chrno == i * m_sign)                                                 // if m_chrno = i * m_sign
        {
          m_repchr = m_text[i - 1];                                               // m_repchr = substr(m_text,i,1)             && uncoded character
        }
        else
        {
          if (m_chrno > m_maxlen || m_chrno < 1)
          {
            m_chrno = m_chrno - m_maxlen * m_sign;
          }
          m_repchr = m_chrstr2[m_chrno - 1];                         // m_repchr = substr(m_chrstr2,m_chrno,1)      && coded character
        }
        m_convert = m_convert + m_repchr;
      }
      return m_convert;
    }


    /// <summary>
    /// ASS xHarbour simple XOR encryption- only supports ASCII!!!
    /// </summary>
    /// <param name="input">String to encrypt</param>
    /// <param name="key">Key for encryption</param>
    /// <returns>Hex encoded encrypted string</returns>
    public static string ASSCommsEncrypt(string input, string key)
    {
      var bytes = ASCIIEncoding.ASCII.GetBytes(input);
      var keyBytes = ASCIIEncoding.ASCII.GetBytes(key);

      var rnd = new Random();

      var result = new List<byte>();
      // Pad start
      for (var i = 0; i < 4; i++)
      {
        result.Add((byte)rnd.Next(255));
      }
      // Add padded data
      foreach (var ch in bytes)
      {
        result.Add((byte)rnd.Next(255));
        result.Add(ch);
      }
      // Pad end
      for (var i = 0; i < 4; i++)
      {
        result.Add((byte)rnd.Next(255));
      }

      // XOR Crypt
      var keyPos = 0;
      for (var i = 0; i < result.Count; i++)
      {
        result[i] = ((byte)((byte)result[i] ^ (byte)keyBytes[keyPos++]));
        if (keyPos >= keyBytes.Length)
          keyPos = 0;
      }

      // Pad result
      for (var i = 0; i < 3; i++)
      {
        result.Insert(0, (byte)rnd.Next(255));
        result.Add((byte)rnd.Next(255));
      }

      return ToHexString(result);
    }


    /// <summary>
    /// ASS xHarbour simple XOR encryption
    /// </summary>
    /// <param name="input">Text to encrypt</param>
    /// <param name="key">Key to encrypt</param>
    /// <returns>Decoded string</returns>
    public static string ASSCommsDecrypt(string input, string key)
    {
      try
      {
        if (input.Length < 16)
        {
          return "";
        }

        var bytes = FromHexString(input);
        var keyBytes = ASCIIEncoding.ASCII.GetBytes(key);

        for (var i = 0; i < 3; i++)
        {
          bytes.RemoveAt(0);
          bytes.RemoveAt(bytes.Count - 1);
        }

        var keyPos = 0;
        for (var i = 0; i < bytes.Count; i++)
        {
          bytes[i] = ((byte)((byte)bytes[i] ^ (byte)keyBytes[keyPos++]));
          if (keyPos >= keyBytes.Length)
            keyPos = 0;
        }

        var result = new List<byte>(input.Length / 2);
        for (var i = 0; i < bytes.Count; i++)
        {
          if ((i % 2) == 1)
          {
            result.Add(bytes[i]);
          }
        }

        for (var i = 0; i < 4; i++)
        {
          result.RemoveAt(0);
          result.RemoveAt(result.Count - 1);
        }

        return ASCIIEncoding.ASCII.GetString(result.ToArray());
      }
      catch
      {
        return "";
      }
    }


    public static string ToHexString(byte[] input)
    {
      var sb = new StringBuilder();
      foreach (var srcByte in input)
      {
        sb.AppendFormat("{0:X2}", srcByte);
      }
      return sb.ToString();
    }


    public static string ToHexString(List<byte> input)
    {
      var sb = new StringBuilder();
      foreach (var srcByte in input)
      {
        sb.AppendFormat("{0:X2}", srcByte);
      }

      return sb.ToString();
    }


    public static List<byte> FromHexString(string text)
    {
      if ((text.Length % 2) != 0)
      {
        throw new ArgumentException("Invalid length: " + text.Length);
      }

      var result = new List<byte>();
      var arrayLength = text.Length / 2;
      for (var i = 0; i < arrayLength; i++)
      {
        result.Add(byte.Parse(text.Substring(i * 2, 2), NumberStyles.HexNumber));
      }

      return result;
    }


    /// <summary>
    /// Decrypt input string 
    /// </summary>
    public static string Decrypt(string text)
    {
      return ASSEncrypt(text, -1);
    }
  }
}
