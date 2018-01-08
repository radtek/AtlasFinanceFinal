/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Task to synchronise the local time with the server
 *     
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-08-22 Created
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Runtime.InteropServices;

using Quartz;

using Serilog;
using Atlas.Fingerprint.WCF.Client.ClientProxies;


namespace AClientSvc.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class SyncLocalTime : IJob
  {
    /// <summary>
    /// Task to synchronise local time with server's time
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("Time sync- task starting");

        var serverDateTime = DateTime.MinValue;
        using (var client = new FPCommsClient(sendTimeout: TimeSpan.FromSeconds(20), openTimeout: TimeSpan.FromSeconds(10)))       
        {
          serverDateTime = client.GetServerDateTime();
        }       

        if (serverDateTime > DateTime.MinValue && Math.Abs(DateTime.Now.Subtract(serverDateTime).TotalSeconds) > 10)
        {
          try
          {
            var localTime = DateTime.Now;
            var systemTime = new NativeMethods.SYSTEMTIME(serverDateTime);
            NativeMethods.SetSystemTime(ref systemTime);
            _log.Information("Time sync- Set local time: Server: {0:yyyy-MM-dd HH:mm:ss}, Local: {1:yyyy-MM-dd HH:mm:ss}", serverDateTime, localTime);
          }
          catch (Exception err)
          {
            _log.Error(err, "SetSystemTime");
          }
        }

        _log.Information("Time sync- task completed");
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }
    }


    #region Private vars

    // Log4net
    private static readonly ILogger _log = Log.ForContext<SyncLocalTime>();

    #endregion    
  }


  internal static class NativeMethods
  {
    #region PInvoke members

    [DllImport("kernel32.dll")]
    internal static extern bool SetSystemTime(ref SYSTEMTIME time);

    [StructLayout(LayoutKind.Sequential)]
    internal struct SYSTEMTIME
    {
      [MarshalAs(UnmanagedType.U2)]
      public short Year;
      [MarshalAs(UnmanagedType.U2)]
      public short Month;
      [MarshalAs(UnmanagedType.U2)]
      public short DayOfWeek;
      [MarshalAs(UnmanagedType.U2)]
      public short Day;
      [MarshalAs(UnmanagedType.U2)]
      public short Hour;
      [MarshalAs(UnmanagedType.U2)]
      public short Minute;
      [MarshalAs(UnmanagedType.U2)]
      public short Second;
      [MarshalAs(UnmanagedType.U2)]
      public short Milliseconds;

      public SYSTEMTIME(DateTime dt)
      {
        dt = dt.ToUniversalTime();  // SetSystemTime expects the SYSTEMTIME in UTC
        Year = (short)dt.Year;
        Month = (short)dt.Month;
        DayOfWeek = (short)dt.DayOfWeek;
        Day = (short)dt.Day;
        Hour = (short)dt.Hour;
        Minute = (short)dt.Minute;
        Second = (short)dt.Second;
        Milliseconds = (short)dt.Millisecond;
      }

    }

    #endregion
  }
}