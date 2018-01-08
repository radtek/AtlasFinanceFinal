/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Attempts to repair a DBF, by checking for and replacing invalid bytes in the data portion (after header) 
 *     
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-07-01 Created
 * 
 * ----------------------------------------------------------------------------------------------------------------- */


using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;



namespace ASSServer.Utils.DBF
{
  public static class RepairDBFBytes
  {
    #region Public methods

    /// <summary>
    /// Will try to replace invalid bytes with spaces
    /// </summary>
    /// <param name="dbfFileName"></param>
    /// <returns></returns>
    public static bool Execute(string dbfFileName, ConcurrentQueue<string> progressMessages)
    {
      var fileNameOnly = Path.GetFileNameWithoutExtension(dbfFileName);
      progressMessages.Enqueue(string.Format("  [{0}] REPAIR- Scanning file started ", fileNameOnly));

      try
      {
        var buffer = new byte[65535];
        using (var fileStream = new FileStream(dbfFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
          #region Check DBF header
          var bytesRead = 0;
          var headerSize = Marshal.SizeOf(typeof(DBFHeader));
          var header = new byte[headerSize];
          bytesRead = fileStream.Read(header, 0, headerSize);
          if (bytesRead != headerSize)
          {
            throw new FileLoadException("Invalid file header!");
          }
          // Marshall the header into a DBFHeader structure
          var handle = GCHandle.Alloc(header, GCHandleType.Pinned);
          var headerRec = (DBFHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DBFHeader));
          handle.Free();
          #endregion

          #region Perform basic header details validation
          if (headerRec.version != 3)
          {
            throw new FileLoadException(string.Format("Unsupported dBase file version: {0}", headerRec.version));
          }

          if (fileStream.Length < (headerRec.headerLen + headerRec.numRecords * headerRec.recordLen))
          {
            throw new FileLoadException("File is too short");
          }         
          #endregion
          
          #region Replace invalid characters in file, after DBF header
          fileStream.Seek(headerRec.headerLen + 1, SeekOrigin.Begin);
          while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
          {
            var changed = false;
            for (var i = 0; i < bytesRead; i++)
            {
              if (buffer[i] < 32 &&
                buffer[i] != 13 && buffer[i] != 10 && // NUCHTTP has these?
                (buffer[i] != 26 && fileStream.Position < fileStream.Length))
              {
                progressMessages.Enqueue(string.Format("  [{0}] REPAIR- Replaced low-order byte at position {1:N0}, value: {2}",
                  fileNameOnly, fileStream.Position - bytesRead + i, buffer[i].ToString("X2")));
                changed = true;
                buffer[i] = 32;
              }
              else if (buffer[i] > 127 && buffer[i] != 130 && buffer[i] != 131 && buffer[i] != 136 && buffer[i] != 138 && buffer[i] != 137 && buffer[i] != 144) // Allow Afrikaans/French accented chars
              {
                progressMessages.Enqueue(string.Format("  [{0}] REPAIR- Replaced high-order byte at position {1:N0}, value: {2}",
                  fileNameOnly, fileStream.Position - bytesRead + i, buffer[i].ToString("X2")));
                changed = true;
                buffer[i] = 32;
              }
            }

            if (changed)
            {
              fileStream.Seek(bytesRead * -1, SeekOrigin.Current);
              fileStream.Write(buffer, 0, bytesRead);
            }
          }
          #endregion
        }

        progressMessages.Enqueue(string.Format("[{0}] REPAIR- Process completed successfully", fileNameOnly));

        return true;
      }
      catch (Exception err)
      {
        progressMessages.Enqueue(string.Format("[{0}] REPAIR- >> FATAL << Error: '{1}'", fileNameOnly, err.Message));
        return false;
      }
    }

    #endregion


    // This is the file header for a DBF. We do this special layout with everything
    // packed so we can read straight from disk into the structure to populate it
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct DBFHeader
    {
      public byte version;
      public byte updateYear;
      public byte updateMonth;
      public byte updateDay;
      public Int32 numRecords;
      public Int16 headerLen;
      public Int16 recordLen;
      public Int16 reserved1;
      public byte incompleteTrans;
      public byte encryptionFlag;
      public Int32 reserved2;
      public Int64 reserved3;
      public byte MDX;
      public byte language;
      public Int16 reserved4;
    }
  }
}
