/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2015 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    In-memory, thread-safe storage for fingerprint templates using the new MS Immutable collections.
 *    
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2015-10-06- Created
 *     
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Collections.Concurrent;

using MongoDB.Driver;
using MongoDB.Bson;

using Atlas.MongoDB.Entities;
using Atlas.Enumerators;
using Atlas.Common.Interface;


namespace Atlas.FP.Identifier.ThreadSafe
{
  sealed class FPThreadSafe
  {
    #region Public methods

    internal static void Initialize(ILogging log, IConfigSettings config, Action callback = null)
    {
      if (!_fpInitialized)
      {
        Populate(log, config, callback);
      }
    }


    /// <summary>
    /// Adds new templates to cache, for addition to immutable
    /// </summary>
    /// <param name="fpData">Fingerprint data to add to in-memory list</param>
    internal static void AddNewFingerprint(ILogging log, ICollection<Tuple<Int64, int, byte[], int>> templates)
    {
      var methodName = "AddNewFingerprint";
      log.Information("{MethodName}- {@Templates}", methodName, templates);

      foreach (var template in templates)
      {
        _personTemplatesAddedPending.Add(new FPBasicTemplate(template.Item1, template.Item2, template.Item3,
          template.Item4 == 0 ? Biometric.OrientationType.RightSide : Biometric.OrientationType.UpsideDown));
      }
    }


    /// <summary>
    /// Removes fingerprint templates for personId
    /// </summary>
    /// <param name="personId">The PER_Person.PersonId</param>
    internal static void DeleteFingerprint(ILogging log, Int64 personId)
    {
      var methodName = "DeleteFingerprint";
      log.Information("{MethodName}- {PersonId}", methodName, personId);

      _personIdsRemovedPending.Add(personId);
    }


    internal static void ProcessPendingUpdates(ILogging log)
    {
      if (_busyUpdatingTemplates || !_fpInitialized) // Avoid re-entrancy...
      {
        return;
      }

      _busyUpdatingTemplates = true;
      try
      {
        var methodName = "ProcessPendingRequests";

        #region Remove from immutable     
        var personIdsRemovedPending = new List<Int64>();
        Int64 personIdRemoved;
        while (_personIdsRemovedPending.TryTake(out personIdRemoved))
        {
          personIdsRemovedPending.Add(personIdRemoved);
        }

        if (personIdsRemovedPending.Any())
        {
          var removeItems = new List<string>();
          foreach (var thisPersonId in personIdsRemovedPending)
          {
            // Create all possible keys for this person...          
            for (var finger = 1; finger <= 10; finger++)
            {
              var key = string.Format(DICTIONARY_KEYFORMAT, thisPersonId, finger, (int)Biometric.OrientationType.RightSide);
              if (_fpTemplatesIB.ContainsKey(key))
              {
                removeItems.Add(key);
              }
              key = string.Format(DICTIONARY_KEYFORMAT, thisPersonId, finger, (int)Biometric.OrientationType.UpsideDown);
              if (_fpTemplatesIB.ContainsKey(key))
              {
                removeItems.Add(key);
              }
            }
          }

          if (removeItems.Any())
          {
            log.Information("{MethodName}- Removing {@RemoveItems}", methodName, removeItems);
            _fpTemplatesIB = _fpTemplatesIB.RemoveRange(removeItems);
          }
        }
        #endregion

        #region Add to immutable
        var templatesAdded = new List<FPBasicTemplate>();
        FPBasicTemplate templateAdded;
        while (_personTemplatesAddedPending.TryTake(out templateAdded))
        {
          templatesAdded.Add(templateAdded);
        }

        if (templatesAdded.Any())
        {
          // Only add unique templates
          var newItems = new Dictionary<string, FPBasicTemplate>();
          foreach (var fp in templatesAdded)
          {
            var key = string.Format(DICTIONARY_KEYFORMAT, fp.PersonId, fp.FingerId, (int)fp.Orientation);
            if (!newItems.ContainsKey(key) && !_fpTemplatesIB.ContainsKey(key))
            {
              newItems.Add(key, fp);
            }
          }

          if (newItems.Any())
          {
            log.Information("{MethodName}- Adding {@NewTemplates}", methodName, newItems);
            _fpTemplatesIB = _fpTemplatesIB.AddRange(newItems);
          }
        }
        #endregion
      }
      finally
      {
        _busyUpdatingTemplates = false;
      }
    }


    /// <summary>
    /// Get snapshot of all templates- not deep copy- NOTE: make a deep copy of the data when passing to unmanaged code!
    /// </summary>
    /// <returns>All fingerprint templates</returns>
    internal static ImmutableList<FPBasicTemplate> SnapshotOfTemplates(ILogging log)
    {
      return _fpTemplatesIB.Select(s => s.Value).ToImmutableList();
    }


    /// <summary>
    /// Returns templates for specific personIds
    /// </summary>
    /// <param name="personIds">List of Person.PersonId</param>
    /// <returns>FPData list containing the templates</returns>
    internal static ImmutableList<FPBasicTemplate> GetIBTemplatesFor(ILogging log, Int64 personId)
    {
      if (personId == 0)
      {
        throw new ArgumentNullException("personId");
      }

      #region Build list of matching dictionary keys for these people
      var keys = new List<string>();
      for (int i = 1; i <= 10; i++) // for each person's finger
      {
        foreach (Biometric.OrientationType orientation in Enum.GetValues(typeof(Biometric.OrientationType))) // for each person's finger and finger image orientation
        {
          keys.Add(string.Format(DICTIONARY_KEYFORMAT, personId, i, (int)orientation));
        }
      }
      #endregion

      // Return as list of matching templates
      return keys.Where(s => _fpTemplatesIB.ContainsKey(s)).Select(s => _fpTemplatesIB[s]).ToImmutableList();
    }


    /// <summary>
    /// Returns FingerIds (1-10) enrolled for this person
    /// </summary>
    /// <param name="personId">PersonId of person to locate</param>
    /// <returns>Int List of FingerIds</returns>
    internal static ICollection<int> GetAllFingerIds(Int64 personId)
    {
      var result = new List<int>();

      if (personId <= 0)
      {
        return result;
      }

      for (int i = 1; i <= 10; i++) // for each finger...
      {
        foreach (Biometric.OrientationType orientation in Enum.GetValues(typeof(Biometric.OrientationType)))
        {
          var key = string.Format(DICTIONARY_KEYFORMAT, personId, i, (int)orientation);
          if (_fpTemplatesIB.ContainsKey(key) && !result.Contains(i))
          {
            result.Add(i);
          }
        }
      }

      return result;
    }

    #endregion


    #region Private methods

    /// <summary>
    /// Loads templates into memory- all staff fingers, client load index and middle
    /// </summary>
    private static void Populate(ILogging log, IConfigSettings config, Action callback = null)
    {
      try
      {
        #region Get listing of employees' PersonIds- we need to load all fingers...
        var employeeIds = new HashSet<long>();
        using (var conn = new Npgsql.NpgsqlConnection(config.GetAtlasCoreConnectionString()))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "SELECT \"PersonId\" FROM \"PER_Person\" WHERE \"TypeId\" != 1"; // anybody but client
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {                
                employeeIds.Add(rdr.GetInt64(0));
              }
            }
          }
        }
        #endregion

        log.Information("Loading templates...");
        var client = new MongoClient(GetSettings());
        #region Load all templates into memory
        var task = Task.Run(async () =>
          {
            var database = client.GetDatabase("fingerprint");
            var fpAllTemplates = database.GetCollection<FPTemplate2>("fpTemplate2");

            var getTemplates = new Dictionary<string, FPBasicTemplate>();

            var elapsed = Stopwatch.StartNew();
            var loaded = 0;
            var clientFingers = new List<int> { 2, 3, 7, 8 };
            await fpAllTemplates.Find(new BsonDocument()).ForEachAsync(s =>
              {
                var isEmployee = employeeIds.Contains(s.PersonId);
                if (isEmployee || (!isEmployee && clientFingers.Contains(s.FingerId)))
                {
                  var orientation = s.Orientation == 0 ? Biometric.OrientationType.RightSide : Biometric.OrientationType.UpsideDown;
                  var fingerprint = new FPBasicTemplate(
                    personId: s.PersonId,
                    fingerId: s.FingerId,
                    data: s.TemplateBuffer,
                    orientation: orientation);

                  var key = string.Format(DICTIONARY_KEYFORMAT, s.PersonId, s.FingerId, (int)orientation);
                  if (!getTemplates.ContainsKey(key))
                  {
                    getTemplates.Add(key, fingerprint);
                  }
                  else
                  {
                    log.Error("Duplicate key: {Key}", key);
                  }

                  loaded++;
                  if (loaded % 10000 == 0)
                  {
                    log.Information("Loaded templates: {Loaded:N0}", loaded);
                  }
                }

                if (callback != null && elapsed.Elapsed > TimeSpan.FromSeconds(25))
                {
                  callback.Invoke();
                  elapsed.Restart();
                }
              });

            _fpTemplatesIB = getTemplates.ToImmutableDictionary();
            _fpInitialized = true;
            log.Information("Populate- Loaded {0} fingerprint templates", _fpTemplatesIB.Count);
          });
        task.Wait();
        #endregion
      }

      catch (Exception err)
      {
        log.Error(err, "Populate");
      }
    }


    /// <summary>
    /// Returns the connection string- WTF does this not work?
    /// </summary>
    /// <returns></returns>
    //private static string GetConnectionString()
    //{
    //  var host = ConfigurationManager.AppSettings["mongodb-host"] ?? "127.0.0.1";
    //  var port = ConfigurationManager.AppSettings["mongodb-port"] ?? "27017";
    //  var user = ConfigurationManager.AppSettings["mongodb-user"];
    //  var pass = ConfigurationManager.AppSettings["mongodb-pass"];
    //  var dbName = ConfigurationManager.AppSettings["mongodb-db"] ?? "fingerprint";
    //  var parameters = ConfigurationManager.AppSettings["mongodb-params"];

    //  var result = string.Format("mongodb://{0}:{1}@{2}:{3}/?database={4}&{5}", user, pass, host, port, dbName, parameters); 
    //  return result;
    //}


    private static MongoClientSettings GetSettings()
    {
      var host = ConfigurationManager.AppSettings["mongodb-host"] ?? "127.0.0.1";
      var user = ConfigurationManager.AppSettings["mongodb-user"];
      var pass = ConfigurationManager.AppSettings["mongodb-pass"];
      var dbName = ConfigurationManager.AppSettings["mongodb-db"] ?? "fingerprint";
      var dataCentre = ConfigurationManager.AppSettings["mongodb-dc"] ?? string.Empty; // Data centre tag- 'mweb' 'ho'

      return new MongoClientSettings
      {
        Credentials = new[] { MongoCredential.CreateCredential(dbName, user, pass) },
        ConnectionMode = ConnectionMode.ReplicaSet,
        ConnectTimeout = TimeSpan.FromSeconds(5),
        ReadPreference = !string.IsNullOrEmpty(dataCentre) ?
           new ReadPreference(ReadPreferenceMode.Nearest, new List<TagSet> { new TagSet(new List<Tag> { new Tag("dc", dataCentre) }) }) :
           ReadPreference.Nearest,
        ReplicaSetName = "rs1",
        Servers = host.Split(new char[] { ',', ';' }).Select(s => new MongoServerAddress(s, 27017)),
        SocketTimeout = TimeSpan.FromSeconds(5),
        ServerSelectionTimeout = TimeSpan.FromSeconds(30),
        WriteConcern = WriteConcern.W1,
        UseSsl = false,
      };
    }

    #endregion


    #region Private vars

    /// <summary>db.auth("rootAdmin", "eij5hgtjr5bb4vg87bn")
    /// Dictionary key format to use:
    ///     [PersonId(20)][FingerId(2)][Orientation(1)] , i. e. 00000000000000000001010, 00000000000000000001021
    /// </summary>
    private const string DICTIONARY_KEYFORMAT = "{0:D20}{1:D2}{2:D1}";

    /// <summary>
    /// Have fingerprint templates been loaded into memory?
    /// </summary>
    private static bool _fpInitialized = false;

    /// <summary>
    /// Dictionary holding all fingerprints in memory
    /// </summary>
    private static ImmutableDictionary<string, FPBasicTemplate> _fpTemplatesIB;

    /// <summary>
    /// New templates added
    /// </summary>
    private static ConcurrentBag<FPBasicTemplate> _personTemplatesAddedPending = new ConcurrentBag<FPBasicTemplate>();

    /// <summary>
    /// Templates deleted
    /// </summary>
    private static ConcurrentBag<Int64> _personIdsRemovedPending = new ConcurrentBag<Int64>();


    private static bool _busyUpdatingTemplates;

    #endregion

  }
}