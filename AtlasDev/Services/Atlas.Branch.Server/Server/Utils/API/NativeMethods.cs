/*************************************************************************************************
 * 
 *  Source: http://ithoughthecamewithyou.com/post/Reboot-computer-in-C-NET.aspx
 * 
 * 
 * 
 * 
 * 
 *************************************************************************************************/
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using Serilog;


namespace ASSSyncClient.API.Windows
{
  static class NativeMethods
  {
    #region Public methods

    /// <summary>
    /// Try reboot the local computer
    /// </summary>
    public static void Reboot()
    {
      try
      {
        var tokenHandle = IntPtr.Zero;

        try
        {
          // get process token
          if (!OpenProcessToken(Process.GetCurrentProcess().Handle,
              TOKEN_QUERY | TOKEN_ADJUST_PRIVILEGES,
              out tokenHandle))
          {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                "Failed to open process token handle");
          }

          // lookup the shutdown privilege
          var tokenPrivs = new TOKEN_PRIVILEGES() { PrivilegeCount = 1, Privileges = new LUID_AND_ATTRIBUTES[1] };
          tokenPrivs.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

          if (!LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, out tokenPrivs.Privileges[0].Luid))
          {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                "Failed to open lookup shutdown privilege");
          }

          // add the shutdown privilege to the process token
          if (!AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivs, 0, IntPtr.Zero, IntPtr.Zero))
          {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                "Failed to adjust process token privileges");
          }

          // reboot
          if (!ExitWindowsEx(ExitWindows.Reboot, ShutdownReason.MajorApplication | ShutdownReason.MinorMaintenance | ShutdownReason.FlagPlanned))
          {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to reboot system");
          }
        }
        finally
        {
          // close the process token
          if (tokenHandle != IntPtr.Zero)
          {
            CloseHandle(tokenHandle);
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Reboot");
      }
    }


    /// <summary>
    /// Gets the serial number for drive C:
    /// </summary>
    /// <returns></returns>
    public static string GetCDriveSerial()
    {
      try
      {
        var volname = new StringBuilder(261);
        var fsname = new StringBuilder(261);
        uint sernum, maxlen;
        FileSystemFeature flags;
        if (!GetVolumeInformation(@"c:\", volname, volname.Capacity, out sernum, out maxlen, out flags, fsname, fsname.Capacity))
        {
          Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return sernum.ToString("X8");
      }
      catch (Exception err)
      {
        _log.Error(err, "GetCDriveSerial");
        throw;
      }
    }

    public static void SetLocalTime(DateTime dateTime)
    {
      var setTime = new NativeMethods.SYSTEMTIME(dateTime);
      SetSystemTime(ref setTime);
    }


    #endregion


    #region Private

    // everything from here on is from pinvoke.net

    [Flags]
    private enum ExitWindows : uint
    {
      // ONE of the following five:
      LogOff = 0x00,
      ShutDown = 0x01,
      Reboot = 0x02,
      PowerOff = 0x08,
      RestartApps = 0x40,
      // plus AT MOST ONE of the following two:
      Force = 0x04,
      ForceIfHung = 0x10,
    }


    [Flags]
    private enum ShutdownReason : uint
    {
      MajorApplication = 0x00040000,
      MajorHardware = 0x00010000,
      MajorLegacyApi = 0x00070000,
      MajorOperatingSystem = 0x00020000,
      MajorOther = 0x00000000,
      MajorPower = 0x00060000,
      MajorSoftware = 0x00030000,
      MajorSystem = 0x00050000,

      MinorBlueScreen = 0x0000000F,
      MinorCordUnplugged = 0x0000000b,
      MinorDisk = 0x00000007,
      MinorEnvironment = 0x0000000c,
      MinorHardwareDriver = 0x0000000d,
      MinorHotfix = 0x00000011,
      MinorHung = 0x00000005,
      MinorInstallation = 0x00000002,
      MinorMaintenance = 0x00000001,
      MinorMMC = 0x00000019,
      MinorNetworkConnectivity = 0x00000014,
      MinorNetworkCard = 0x00000009,
      MinorOther = 0x00000000,
      MinorOtherDriver = 0x0000000e,
      MinorPowerSupply = 0x0000000a,
      MinorProcessor = 0x00000008,
      MinorReconfig = 0x00000004,
      MinorSecurity = 0x00000013,
      MinorSecurityFix = 0x00000012,
      MinorSecurityFixUninstall = 0x00000018,
      MinorServicePack = 0x00000010,
      MinorServicePackUninstall = 0x00000016,
      MinorTermSrv = 0x00000020,
      MinorUnstable = 0x00000006,
      MinorUpgrade = 0x00000003,
      MinorWMI = 0x00000015,

      FlagUserDefined = 0x40000000,
      FlagPlanned = 0x80000000
    }


    [StructLayout(LayoutKind.Sequential)]
    private struct LUID
    {
      public uint LowPart;
      public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct LUID_AND_ATTRIBUTES
    {
      public LUID Luid;
      public UInt32 Attributes;
    }

    private struct TOKEN_PRIVILEGES
    {
      public UInt32 PrivilegeCount;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
      public LUID_AND_ATTRIBUTES[] Privileges;
    }

    private const UInt32 TOKEN_QUERY = 0x0008;
    private const UInt32 TOKEN_ADJUST_PRIVILEGES = 0x0020;
    private const UInt32 SE_PRIVILEGE_ENABLED = 0x00000002;
    private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ExitWindowsEx(ExitWindows uFlags,
        ShutdownReason dwReason);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool OpenProcessToken(IntPtr ProcessHandle,
        UInt32 DesiredAccess,
        out IntPtr TokenHandle);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool LookupPrivilegeValue(string lpSystemName,
        string lpName,
        out LUID lpLuid);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
        [MarshalAs(UnmanagedType.Bool)]bool DisableAllPrivileges,
        ref TOKEN_PRIVILEGES NewState,
        UInt32 Zero,
        IntPtr Null1,
        IntPtr Null2);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool GetVolumeInformation(
      string RootPathName,
      StringBuilder VolumeNameBuffer,
      int VolumeNameSize,
      out uint VolumeSerialNumber,
      out uint MaximumComponentLength,
      out FileSystemFeature FileSystemFlags,
      StringBuilder FileSystemNameBuffer,
      int nFileSystemNameSize);


    [Flags]
    internal enum FileSystemFeature : uint
    {
      /// <summary>
      /// The file system supports case-sensitive file names.
      /// </summary>
      CaseSensitiveSearch = 1,
      /// <summary>
      /// The file system preserves the case of file names when it places a name on disk.
      /// </summary>
      CasePreservedNames = 2,
      /// <summary>
      /// The file system supports Unicode in file names as they appear on disk.
      /// </summary>
      UnicodeOnDisk = 4,
      /// <summary>
      /// The file system preserves and enforces access control lists (ACL).
      /// </summary>
      PersistentACLS = 8,
      /// <summary>
      /// The file system supports file-based compression.
      /// </summary>
      FileCompression = 0x10,
      /// <summary>
      /// The file system supports disk quotas.
      /// </summary>
      VolumeQuotas = 0x20,
      /// <summary>
      /// The file system supports sparse files.
      /// </summary>
      SupportsSparseFiles = 0x40,
      /// <summary>
      /// The file system supports re-parse points.
      /// </summary>
      SupportsReparsePoints = 0x80,
      /// <summary>
      /// The specified volume is a compressed volume, for example, a DoubleSpace volume.
      /// </summary>
      VolumeIsCompressed = 0x8000,
      /// <summary>
      /// The file system supports object identifiers.
      /// </summary>
      SupportsObjectIDs = 0x10000,
      /// <summary>
      /// The file system supports the Encrypted File System (EFS).
      /// </summary>
      SupportsEncryption = 0x20000,
      /// <summary>
      /// The file system supports named streams.
      /// </summary>
      NamedStreams = 0x40000,
      /// <summary>
      /// The specified volume is read-only.
      /// </summary>
      ReadOnlyVolume = 0x80000,
      /// <summary>
      /// The volume supports a single sequential write.
      /// </summary>
      SequentialWriteOnce = 0x100000,
      /// <summary>
      /// The volume supports transactions.
      /// </summary>
      SupportsTransactions = 0x200000,
    }



    [DllImport("kernel32.dll")]
    private static extern bool SetSystemTime(ref SYSTEMTIME time);


    [StructLayout(LayoutKind.Sequential)]
    private struct SYSTEMTIME
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

    // Logging
    private static readonly ILogger _log = Log.Logger;

    #endregion

  }
}
