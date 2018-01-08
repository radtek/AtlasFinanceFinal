using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddFPsForPerformance
{
  public static class FPBufferUtils
  {
    public const int SIZE_WIDTH = 352;
    public const int SIZE_HEIGHT = 288;

    /// <summary>
    /// Creates a new 8-bit, 256-grayscale bitmap from 'buffer'
    /// </summary>
    /// <param name="buffer">Source raw image grayscale buffer</param>
    /// <param name="width">Width of resultant Bitmap</param>
    /// <param name="height">Height of resultant Bitmap</param>
    /// <returns>Bitmap with orientation and colour specified via class properties</returns>
    public static Bitmap BufferToBitmap(byte[] buffer, int width, int height)
    {
      var result = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
      // Convert the raw fingerprint data to bitmap...
      var data = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
      int stride = data.Stride;
      unsafe
      {
        byte* ptr = (byte*)data.Scan0;

        for (int y = 0; y < height; y++)
        {
          for (int x = 0; x < width; x++)
          {
            ptr[y * stride + x] = buffer[y * stride + x];
          }
        }
      }
      result.UnlockBits(data);

      // This code is faster, but it is not reliable/fit for production.
      // I believe the background buffer is be summarily destroyed by the GAC, 
      // rendering the image void and causing the PictureBox to seize up (big nasty X).
      // *** DO NOT USE ***

      /*Bitmap result;
      // This is 2-3 times faster than the method above!
      var newBuffer = new byte[buffer.Length];
      Array.Copy(buffer, newBuffer, buffer.Length);
      unsafe
      {
        fixed (byte* ptr = newBuffer)
        {
          var iptr = new IntPtr(ptr);
          result = new Bitmap(width, height, width, PixelFormat.Format8bppIndexed, iptr);
        }          
      }*/

      // Create 256-grayscale palette
      var palette = result.Palette;
      var entries = palette.Entries;
      var entriesCount = entries.Length;
      for (int i = 0; i < entriesCount; i++)
      {
        entries[i] = Color.FromArgb(255, i, i, i);
      }
      result.Palette = palette;

      return result;
    }

    /// <summary>
    /// Inverts a byte bitmap (only works with a 256 grayscale image)
    /// </summary>
    /// <param name="buffer">buffer to invert</param>
    /// <returns>Inverted/negative version of buffer</returns>
    public static byte[] BufferInvert(byte[] buffer)
    {
      var len = buffer.Length;
      var result = new byte[len];
      for (int i = 0; i < len; i++)
      {
        result[i] = ((byte)((int)(buffer[i] - 255) * -1));
      }
      return result;
    }


    public static byte[] Rotate90Left(byte[] buffer)
    {
      var result = new byte[buffer.Length];
      for (int y = 0; y < SIZE_HEIGHT; y++)
        for (int x = 0; x < SIZE_WIDTH; x++)        
        {
          result[y + ((SIZE_WIDTH - x - 1) * SIZE_HEIGHT)] = buffer[x + (y * SIZE_WIDTH)];          
        }

      return result;
    }   
  }
}
