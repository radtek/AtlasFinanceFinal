/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Stores secured application settings- persists to the local Windows Registry, for off-line service starts
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     14 June 2013 - Created
 * 
 * 
 *  Comments:
 *  ------------------
 *     This is by no-means enterprise-level security of local creds storage, but provides a reasonable level 
 *     of security/obscurity.
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using Microsoft.Win32;

using Atlas.Desktop.Utils.Crypto;
using Atlas.DataSync.WCF.Client.ClientProxies;
using Atlas.DataSync.WCF.Interface;


namespace ASSSyncClient.Utils.Settings
{
  /// <summary>
  /// Class to get/access/persist application settings.  NOTE: This class violates SRP- need to extract functionalities...?
  ///  
  /// </summary>
  public static class AppSettings
  {
    /// <summary>
    /// Static constructor
    /// </summary>
    static AppSettings()
    {
      #region Try get settings from WCF server
      List<KeyValueItem> branchSettings = null;
      try
      {
        using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(20), sendTimeout: TimeSpan.FromSeconds(40)))
        {
          branchSettings = client.GetBranchSettings(ASSSyncClient.Utils.WCF.SyncSourceRequest.CreateSourceRequest());
        }
      }
      catch
      {
        // Ignore errors- we can revert to the local, persisted settings
      }
      #endregion

      #region Extract key settings
      if (branchSettings != null && branchSettings.Count > 0)
      {
        lock (_locker)
        {
          foreach (var setting in branchSettings)
          {
            switch (setting.Key)
            {
              case "A":
                _A = setting.Value;
                break;

              case "B":
                _B = setting.Value;
                break;

              case "C":
                _C = setting.Value;
                break;

              case "D":
                _D = setting.Value;
                break;
            }
          }
        }
      }
      #endregion

      #region Save to/restore from persisted storage
      var keyName = "Software\\Atlas\\ASSSyncClient\\Settings";
      var regRoot = Registry.CurrentUser;
      RegistryKey key = null;
      if (string.IsNullOrEmpty(A) || string.IsNullOrEmpty(B) || string.IsNullOrEmpty(C) || string.IsNullOrEmpty(D))
      {
        #region Try get values from persisted storage
        if ((key = regRoot.OpenSubKey(keyName, true)) == null)
        {
          key = regRoot.CreateSubKey(keyName);
          throw new Exception("Unable to connect to server and persistent storage does not contain any of the required values");
        }
        else
        {
          if (string.IsNullOrEmpty(A))
          {
            lock (_locker)
            {
              _A = (string)key.GetValue("A");
            }
          }
          if (string.IsNullOrEmpty(B))
          {
            lock (_locker)
            {
              _B = (string)key.GetValue("B");
            }
          }
          if (string.IsNullOrEmpty(C))
          {
            lock (_locker)
            {
              _C = (string)key.GetValue("C");
            }
          }
          if (string.IsNullOrEmpty(D))
          {
            lock (_locker)
            {
              _D = (string)key.GetValue("D");
            }
          }
        }
        #endregion

        if (string.IsNullOrEmpty(A) || string.IsNullOrEmpty(B) || string.IsNullOrEmpty(C) || string.IsNullOrEmpty(D))
        {
          throw new Exception("[FATAL] Unable to connect to server and persistent storage is missing required values");
        }
      }
      else
      {
        #region Try save to local persisted storage
        if ((key = regRoot.OpenSubKey(keyName, true)) == null)
        {
          key = regRoot.CreateSubKey(keyName);
        }
        key.SetValue("A", A, Microsoft.Win32.RegistryValueKind.String);
        key.SetValue("B", B, Microsoft.Win32.RegistryValueKind.String);
        key.SetValue("C", C, Microsoft.Win32.RegistryValueKind.String);
        key.SetValue("D", D, Microsoft.Win32.RegistryValueKind.String);
        key.Flush();
        #endregion
      }
      #endregion
    }


    #region Public properties

    /// <summary>
    /// Encryption salt to be used (hex string)
    /// </summary>
    public static string A
    {
      get
      {
        lock (_locker)
        {
          return _A;
        }
      }
    }

    /// <summary>
    /// Encryption password (hex string)
    /// </summary>
    public static string B
    {
      get
      {
        lock (_locker)
        {
          return _B;
        }
      }
    }

    /// <summary>
    /// Database username (encrypted hex string)
    /// </summary>
    public static string C
    {
      get
      {
        lock (_locker)
        {
          return _C;
        }
      }
    }

    /// <summary>
    /// Database password (encrypted hex string)
    /// </summary>
    public static string D
    {
      get
      {
        lock (_locker)
        {
          return _D;
        }
      }
    }

    /// <summary>
    /// Returns the unencrypted connection string
    /// </summary>
    public static string NPGSQLConnStr
    {
      get
      {
        return string.Format("Server=127.0.0.1;Port=5432;Database=ass;User Id={0};Password={1};Timeout=10;", 
          BasicCrypto.Decrypt(A, B, C), BasicCrypto.Decrypt(A, B, D));
      }
    }

    #endregion


    #region Private members

    /// <summary>
    /// Locking object for items below
    /// </summary>
    private static object _locker = new object();

    /// <summary>
    /// Backing storage for A property- Encryption salt to be used (hex string)
    /// </summary>
    private static string _A { get; set; }

    /// <summary>
    /// Backing storage for B property- Encryption password (hex string)
    /// </summary>
    private static string _B { get; set; }

    /// <summary>
    /// Backing storage for C property- Database username (encrypted hex string)
    /// </summary>
    private static string _C { get; set; }

    /// <summary>
    /// Backing storage for D property- Database username (encrypted hex string)
    /// </summary>
    private static string _D { get; set; }

    #endregion

  }
}
