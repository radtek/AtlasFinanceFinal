/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Simple WCF file chunking service- allows for uploading and downloading files- used by CopyData to 
 *     upload DBF/get dumps.
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
using System.ServiceModel;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;


namespace ASSServer.WCF.Implementation.DataFileChunk
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class DataFileServer : IDataFileServer
  {
    public DataFileServer(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }

    #region Public methods

    /// <summary>
    /// Gets the dize of the specified file, -1 if error
    /// </summary>
    /// <param name="fileName">The name of the file- must only contain alphanumerics</param>
    /// <returns>byte size of the file, else -1 if error</returns>
    public long GetFileSize(SourceRequest sourceRequest, string fileName)
    {
      return GetFileSize_Impl.Execute(_log, _config, sourceRequest, fileName);
    }


    /// <summary>
    /// Appends a chunk to a file- if file does not exist, it will be created
    /// </summary>
    /// <param name="fileName">Name of the file- must not contain any non alphanumerics</param>
    /// <param name="buffer">The data</param>
    /// <returns>true if successful, false if error</returns>
    public bool AppendFileChunk(SourceRequest sourceRequest, string fileName, byte[] buffer)
    {
      return AppendFileChunk_Impl.Execute(_log, _config, sourceRequest, fileName, buffer);
    }


    /// <summary>
    /// Reads a chunk of a file, from 'offset' with chunk size 'chunkSize'
    /// </summary>
    /// <param name="fileName">The file name</param>
    /// <param name="offset">The file offset</param>
    /// <param name="chunkSize">The size of the chunk</param>
    /// <returns>byte array with chunksize or less (if eof), else null if error</returns>
    public byte[] DownloadFileChunk(SourceRequest sourceRequest, string fileName, long offset, int chunkSize)
    {
      return DownloadFileChunk_Impl.Execute(_log, _config, sourceRequest, fileName, offset, chunkSize);
    }



    /// <summary>
    /// Return file hash
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="fileName"></param>
    /// <param name="method"></param>
    /// <returns>Hexadecimal string of hash, excluding '-'</returns>
    public string GetFileChecksum(SourceRequest sourceRequest, string fileName, string method)
    {
      return GetFileChecksum_Impl.Execute(_log, _config, sourceRequest, fileName, method);
    }

    #endregion


    #region Private fields

    private readonly ILogging _log;
    private readonly IConfigSettings _config;

    #endregion

  }
}
