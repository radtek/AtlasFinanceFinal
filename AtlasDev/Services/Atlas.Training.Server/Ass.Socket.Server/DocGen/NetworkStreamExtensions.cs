using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Text;


namespace Atlas.Desktop.Utils.Network
{
  internal static class NetworkStreamExtensions
  {
    /// <summary>
    /// Send a buffer and perform a Readline (ASCII terminated by #13) or time-out
    /// </summary>
    /// <param name="stream">Network stream to read from/write to</param>
    /// <param name="sendBuffer">Any buffer to be sent</param>
    /// <param name="timeout">read time-out, in milliseconds</param>
    /// <returns>ASCII string containing the read response</returns>
    public static string ReadLine(this NetworkStream stream, byte[] sendBuffer = null, int timeout = 500)
    {
      if (sendBuffer != null)
      {
        stream.Write(sendBuffer, 0, sendBuffer.Length);
      }

      #region Wait for data
      var wait = Stopwatch.StartNew();
      var timeoutTime = TimeSpan.FromMilliseconds(timeout);
      while (!stream.DataAvailable && wait.Elapsed < timeoutTime)
      {
        Thread.Sleep(50);
      }
      #endregion

      var readByte = 0;
      var buffer = new byte[5000000];
      var bufferPos = 0;
      while (wait.Elapsed < timeoutTime && bufferPos < buffer.Length && readByte > -1 && readByte != 13)
      {
        readByte = stream.ReadByte();
        if (readByte > -1 && readByte != 13)
        {
          buffer[bufferPos++] = (byte)readByte;
        }
      }

      return (bufferPos > 0 && readByte == 13) ? ASCIIEncoding.ASCII.GetString(buffer, 0, bufferPos) : null;
    }


    public static void WriteString(this NetworkStream stream, string value)
    {
      var buffer = Encoding.ASCII.GetBytes(string.Format("{0}\r", value));
      stream.Write(buffer, 0, buffer.Length);
    }

  }
}
