using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Immutable;

using IBscanMatcher;
using IBscanUltimate;
using Serilog;

using Atlas.Common.Utils;
using Atlas.FP.Identifier.ThreadSafe;
using Atlas.Common.Interface;


namespace Atlas.FP.Identifier.SDK.Utils
{
  /// <summary>
  /// IB template and matching utils.
  /// 
  /// The IB SDK ** DOES NOT ** like TPL/dynamic (Task.Run/Task.Factory) threads and continually initializing the SDK.  
  /// As such, I create a static IB threadpool and pass messages to them & wait for response(s). This is also more efficient, 
  /// because OpenMatcher() takes considerable time (>30ms).
  /// </summary>
  public static class IBUtils
  {
    #region Internal methods

    /// <summary>
    /// Create the worker threads to process requests
    /// </summary>
    internal static void Initialize()
    {
      CreateIdWorkerThreads();
      CreateTemplateWorkerThreads();
    }


    /// <summary>
    /// Use in-memory templates and try to identify person from given template
    /// </summary>
    /// <param name="template"></param>
    /// <param name="securityLevel">Security matching level</param>
    /// <param name="score">Matching score</param>
    /// <param name="personId">Matching person</param>
    /// <param name="fingerId">Matching finger</param>
    /// <param name="errorMessage"></param>   
    internal static void IdentifyPersonByTemplate(ILogging log, byte[] template, int securityLevel,
      out int score, out Int64 personId, out int fingerId, out string errorMessage)
    {
      Log.Information("IdentifyPersonByTemplate: {@Request}", new { template, securityLevel });
      errorMessage = null;
      score = -1;
      personId = -1;
      fingerId = -1;

      var timer = Stopwatch.StartNew();
      var id = IdentifyTask(log, template, securityLevel);
      timer.Stop();

      if (!string.IsNullOrEmpty(id.ErrorMessage))
      {
        Log.Error("IdentifyPersonByTemplate: {@Error}", id);
        errorMessage = id.ErrorMessage;
        return;
      }

      score = id.Score;
      personId = id.PersonId;
      fingerId = id.FingerId;
      
      Int64 today;
      Int64 total;
      GetScanStats(out today, out total);
      Log.Information("IdentifyPersonByTemplate: ({Time}ms) - {@Response} (Scans: {@Today:N0} - {@Total:N0})",
        timer.ElapsedMilliseconds, new { score, personId, fingerId }, today, total);
    }


    /// <summary>
    /// Use in-memory templates and try to identify person from given image(s)
    /// </summary>
    /// <param name="compressedImages">Images to use to create template</param>
    /// <param name="securityLevel">Matching security level</param>
    /// <param name="score">Matching score -1 to 20000...</param>
    /// <param name="personId">Matching person id, else <1</param>
    /// <param name="fingerId">Matching finger id</param>
    /// <param name="errorMessage"></param>  
    internal static void IdentifyPersonByImages(ILogging log, ICollection<byte[]> compressedImages, int securityLevel,
      out int score, out Int64 personId, out int fingerId, out string errorMessage)
    {
      Log.Information("IdentifyPersonByImages: {@Request}", new { compressedImages, securityLevel });

      errorMessage = null;
      score = -1;
      personId = -1;
      fingerId = -1;

      var timer = Stopwatch.StartNew();
      var template = CreateTemplateTask(compressedImages.ToList(), true);
      if (!string.IsNullOrEmpty(template.ErrorMessage))
      {
        Log.Error("IdentifyPersonByImages: CreateTemplateTask- {Error}", template.ErrorMessage);
        errorMessage = template.ErrorMessage;
        return;
      }

      var id = IdentifyTask(log, template.Template, securityLevel);
      if (!string.IsNullOrEmpty(id.ErrorMessage))
      {
        Log.Error("IdentifyPersonByImages: IdentifyTask- {Error}", id.ErrorMessage);
        errorMessage = id.ErrorMessage;
        return;
      }
      timer.Stop();

      score = id.Score;
      personId = id.PersonId;
      fingerId = id.FingerId;

      Int64 today;
      Int64 total;
      GetScanStats(out today, out total);
      Log.Information("IdentifyPersonByImages: ({Time}ms) - {@Response} (Scans: {@Today:N0} - {@Total:N0})",
        timer.ElapsedMilliseconds, new { score, personId, fingerId }, today, total);
    }


    /// <summary>
    /// Create normal and upside down templates from given image(s)
    /// </summary>
    /// <param name="compressedImages">THe source raw compressed byte bitmap</param>
    /// <param name="template">The resultant normal template</param>
    /// <param name="reversedTemplate">The resultant upsde-down template</param>
    /// <param name="errorMessage"></param>   
    internal static void CreateTemplate(ICollection<byte[]> compressedImages,
      out byte[] template, out byte[] reversedTemplate, out string errorMessage)
    {
      Log.Information("CreateTemplate: {@Request}", new { compressedImages });

      var timer = Stopwatch.StartNew();
      template = null;
      reversedTemplate = null;
      errorMessage = null;

      var convert = CreateTemplateTask(compressedImages.ToList(), true);
      if (!string.IsNullOrEmpty(convert.ErrorMessage))
      {
        Log.Error("CreateTemplate: {@Error}", convert);
        errorMessage = convert.ErrorMessage;
        return;
      }

      template = convert.Template;
      reversedTemplate = convert.ReversedTemplate;
      timer.Stop();
      Log.Information("CreateTemplate: ({Time}ms) - {@Request}", timer.ElapsedMilliseconds, new { template, reversedTemplate });
    }


    /// <summary>
    /// Check if template matches any of the given templates
    /// </summary> 
    internal static void CheckForMatchByTemplate(ILogging log, byte[] template, ICollection<Tuple<int, byte[]>> templates, int securityLevel,
      out int score, out int fingerId, out string errorMessage)
    {
      var timer = Stopwatch.StartNew();
      Log.Information("CheckForMatchByTemplate: {@Request}", new { template, securityLevel });
      score = -1;
      fingerId = -1;
      errorMessage = null;

      var id = IdentifyTask(log, template, securityLevel,
        templates.Select(s => new FPBasicTemplate(1, s.Item1, s.Item2, Enumerators.Biometric.OrientationType.RightSide)).ToList());
      if (!string.IsNullOrEmpty(id.ErrorMessage))
      {
        Log.Error("CheckForMatchByTemplate: {@Error}", id);
        errorMessage = id.ErrorMessage;
        return;
      }

      score = id.Score;
      fingerId = id.FingerId;
      timer.Stop();

      Int64 today;
      Int64 total;
      GetScanStats(out today, out total);
      Log.Information("CheckForMatchByTemplate: ({Time}ms) - {@Response} (Scans: {@Today:N0} - {@Total:N0})",
        timer.ElapsedMilliseconds, new { score, fingerId }, today, total);
    }


    /// <summary>
    /// Take given images and see if it matches any of the given templates
    /// </summary> 
    internal static void CheckForMatchByImages(ILogging log, ICollection<byte[]> compressedImages, ICollection<Tuple<int, byte[]>> templates, int securityLevel,
      out int score, out int fingerId, out string errorMessage)
    {
      var timer = Stopwatch.StartNew();
      Log.Information("CheckForMatchByImages: {@Request}", new { compressedImages, templates, securityLevel });
      score = -1;
      fingerId = -1;
      errorMessage = null;

      // Convert 'compressedImages' to a template
      var template = CreateTemplateTask(compressedImages.ToList(), true);
      if (!string.IsNullOrEmpty(template.ErrorMessage))
      {
        Log.Error("CheckForMatchByImages: CreateTemplate- {@Error}", template.ErrorMessage);
        errorMessage = template.ErrorMessage;
        return;
      }

      // Perform ID against 'templates'
      var id = IdentifyTask(log, template.Template, securityLevel, templates.Select(s => new FPBasicTemplate(0, s.Item1, s.Item2, Enumerators.Biometric.OrientationType.RightSide)).ToList());
      if (!string.IsNullOrEmpty(id.ErrorMessage))
      {
        Log.Error("CheckForMatchByImages: IdentifyTask- {@Error}", id.ErrorMessage);
        errorMessage = id.ErrorMessage;
        return;
      }

      score = id.Score;
      fingerId = id.FingerId;
      timer.Stop();
      Log.Information("CheckForMatchByImages: ({Time}ms) - {@Request}", timer.ElapsedMilliseconds, new { score, fingerId });
    }

    #endregion


    #region Private methods

    /// <summary>
    /// Identify a 'template' in 'searchTemplates' with worker threads
    /// </summary>
    /// <param name="template"></param>
    /// <param name="searchTemplates"></param>
    /// <returns></returns>
    private static FpIdResponse IdentifyTask(ILogging log, byte[] template, int securityLevel = 6, List<FPBasicTemplate> searchTemplates = null)
    {
      var incremented = false;      
      try
      {
        if (searchTemplates == null || searchTemplates.Count > 100) // a slow request? limit to allowing a maximum of 3 in the queue, to avoid continuous time-outs
        {
          if (_longRunningIdTasksInProgress > 3)
          {
            return new FpIdResponse { ErrorMessage = "Too many requests in progress- please wait and try again" };
          }

          incremented = true;
          Interlocked.Increment(ref _longRunningIdTasksInProgress);
        }
        
        var scanTemplates = (searchTemplates == null) ?
              FPThreadSafe.SnapshotOfTemplates(log) :
              searchTemplates.Select(s => new FPBasicTemplate(s.PersonId, s.FingerId, s.Data, s.Orientation)).ToImmutableList();

        // Determine number of workers to use and max time-out
        var useThreadCount = scanTemplates.Count > 20 ? _idWorkerThreadCount : 1;  // Use all available worker threads if scanning the entire db, else just one worker     
        var maxThreadWaitTime = scanTemplates.Count > 20 ?
          TimeSpan.FromSeconds(Math.Ceiling(((double)scanTemplates.Count / (double)useThreadCount / (double)WORKER_SCAN_RATE_PER_SEC) * 2) + 5) : // 5 seconds leeway for heavy load + allow for 1 pending job
          TimeSpan.FromSeconds(5);

        // Evenly partition all the templates amongst all id worker threads
        var taskGuids = new List<string>();
        var templatesPerThread = (int)Math.Ceiling((decimal)scanTemplates.Count / (decimal)useThreadCount);
        var jobWaitHandles = new List<WaitHandle>();
        var jobToAbort = new ManualResetEventSlim(false);

        for (var thisTaskNum = 0; thisTaskNum < useThreadCount; thisTaskNum++)
        {
          var startIndex = (thisTaskNum * templatesPerThread);
          var endIndex = startIndex + templatesPerThread - 1;
          if (endIndex >= scanTemplates.Count)
          {
            endIndex = scanTemplates.Count - 1;
          }

          Log.Information("IdentifyTask: {@Task}", new { scanTemplates.Count, useThreadCount, thisTaskNum, templatesPerThread, startIndex, endIndex, maxThreadWaitTime });

          var taskGuid = Guid.NewGuid().ToString("N");
          var taskWait = new ManualResetEvent(false);
          _idRequests.Add(new FpIdRequest
          {
            RequestId = taskGuid,
            ThisTemplate = template,
            SearchTemplates = scanTemplates.GetRange(startIndex, endIndex - startIndex + 1).ToList(),
            SecurityLevel = securityLevel,
            Timeout = maxThreadWaitTime,
            Timer = Stopwatch.StartNew(),
            TaskWait = taskWait,
            JobToAbort = jobToAbort
          });
          jobWaitHandles.Add(taskWait);
          taskGuids.Add(taskGuid);
        }

        // Wait for all queued tasks to complete, or for a time-out    
        var maxWaitAllTime = maxThreadWaitTime.Add(TimeSpan.FromSeconds(1)); //  give threads additional 1 second for orchestration  
        WaitHandle.WaitAll(jobWaitHandles.ToArray(), maxWaitAllTime); // wait for all tasks to run to completion

        // Get threadpool responses
        var taskResponses = new List<FpIdResponse>();
        foreach (var taskGuid in taskGuids)
        {
          FpIdResponse response;
          if (_idResponses.TryRemove(taskGuid, out response))
          {
            taskResponses.Add(response);
          }
        }

        Log.Information("IdentifyTask response: {@Response} - {ResponseCount} / {Tasks}", taskResponses, taskResponses.Count, useThreadCount);

        // Any threads error -> indicate task failure
        var errors = taskResponses.Where(s => !string.IsNullOrEmpty(s.ErrorMessage));
        if (errors.Any())
        {
          return new FpIdResponse { ErrorMessage = string.Join(", ", errors.Select(s => s.ErrorMessage)) };
        }

        if (taskResponses.Count == useThreadCount) // all threads responded without errors
        {
          var found = taskResponses.FirstOrDefault(s => s.Score > 0);
          return found != null ?
            new FpIdResponse { PersonId = found.PersonId, FingerId = found.FingerId, Score = found.Score } :
            new FpIdResponse();
        }

        // This is bad- we shouldn't ever get here...
        Log.Fatal("IdentifyTask: Task workers timed-out: {Responses} / {Tasks}", taskResponses.Count, useThreadCount);
        return new FpIdResponse { ErrorMessage = "Task workers timed-out" };
      }
      catch (Exception err)
      {
        return new FpIdResponse { ErrorMessage = err.Message };
      }
      finally
      {
        if (incremented)
        {
          Interlocked.Decrement(ref _longRunningIdTasksInProgress);
        }
      }
    }


    /// <summary>
    /// Create template using worker threads
    /// </summary>
    /// <param name="images"></param>
    /// <param name="areGZipped"></param>
    /// <returns></returns>
    private static FpTemplateCreateResponse CreateTemplateTask(List<byte[]> images, bool areGZipped)
    {
      try
      {
        var taskGuid = Guid.NewGuid().ToString("N");
        _templateRequests.Add(new FpTemplateCreateRequest
        {
          RequestId = taskGuid,
          Images = images,
          IsCompressed = areGZipped
        });
        var timer = Stopwatch.StartNew();
        FpTemplateCreateResponse response = null;
        while (timer.Elapsed < TimeSpan.FromSeconds(10) && response == null)
        {
          Thread.Sleep(50);
          if (_templateResponses.ContainsKey(taskGuid))
          {
            _templateResponses.TryRemove(taskGuid, out response);
          }
        }

        if (response != null)
        {
          return new FpTemplateCreateResponse { Template = response.Template, ReversedTemplate = response.ReversedTemplate, ErrorMessage = response.ErrorMessage };
        }
        else
        {
          return new FpTemplateCreateResponse { ErrorMessage = "Request timed-out!" };
        }
      }
      catch (Exception err)
      {
        return new FpTemplateCreateResponse { ErrorMessage = err.Message };
      }
    }


    /// <summary>
    /// Create dedicated IB API worker threads for fingerprint identification
    /// </summary>
    private static unsafe void CreateIdWorkerThreads()
    {
      if (_idWorkerThreads != null)
      {
        return;
      }

      _idWorkerThreads = new Thread[_idWorkerThreadCount];
      for (var i = 0; i < _idWorkerThreadCount; i++)
      {
        _idWorkerThreads[i] = new Thread(() =>
        {
          Log.Information("Worker thread started");
          var matcher_handle = -1;

          // IB DLL does not seem to be fully thread-safe? Ensure only 1 thread at a time calls MDLL._IBSM_OpenMatcher...
          lock (_ibOpenMatcherLocker)
          {
            var open = MDLL._IBSM_OpenMatcher(ref matcher_handle);
            if (open != MDLL.IBSM_STATUS_OK || matcher_handle < 0)
            {
              Log.Fatal("Failed to open matcher: {Error}", open);
              return; // abort thread
            }

            Log.Information("Got matcher (identify): {Thread} - {Matcher}", i, matcher_handle);
          }

          while (true)
          {
            FpIdRequest request = null;
            if (_idRequests.TryTake(out request, Timeout.Infinite))
            {
              FpIdResponse result = null;
              var actual = Stopwatch.StartNew();
              var sdkTimer = new Stopwatch();
              var serializeTimer = new Stopwatch();
              var scannedCount = 0;
              try
              {
                sdkTimer.Start();
                int matchLevel = -1;
                if (MDLL._IBSM_GetMatchingLevel(matcher_handle, ref matchLevel) == MDLL.IBSM_STATUS_OK && matchLevel != request.SecurityLevel)
                {
                  MDLL._IBSM_SetMatchingLevel(matcher_handle, request.SecurityLevel);
                }
                sdkTimer.Stop();

                serializeTimer.Start();
                var personTemplate = IBTemplateUtils.Deserialize(request.ThisTemplate);
                serializeTimer.Stop();

                var templateCount = request.SearchTemplates.Count;
                for (var templateIdx = 0; templateIdx < templateCount; templateIdx++)
                {
                  if (request.JobToAbort.IsSet) // This Job marked as aborted (either another matched or ran into a severe error/time-out)?
                  {
                    break;
                  }
                                    
                  if (request.Timer.Elapsed > request.Timeout) //  Our task has exceeded the prescribed timeout?
                  {
                    result = new FpIdResponse
                    {
                      ErrorMessage = string.Format(
                          "Task has exceeded the maximum time-out: {0}ms > {1}ms, scanned: {2} ({3}%), Total: {4}ms, SDK: {5}ms",
                          request.Timer.ElapsedMilliseconds, request.Timeout.TotalMilliseconds,
                          templateIdx, ((double)templateIdx / (double)request.SearchTemplates.Count) * 100D,
                          actual.ElapsedMilliseconds, sdkTimer.ElapsedMilliseconds)
                    };
                    break;
                  }

                  serializeTimer.Start();
                  var thisTemplate = request.SearchTemplates[templateIdx];
                  var ibTemplate = IBTemplateUtils.Deserialize(thisTemplate.Data);
                  serializeTimer.Stop();

                  var thisScore = -1;
                  sdkTimer.Start();
                  var match = MDLL._IBSM_MatchingTemplate(matcher_handle, ibTemplate, personTemplate, ref thisScore);
                  sdkTimer.Stop();
                  scannedCount++;

                  // FATAL error?
                  if (match != MDLL.IBSM_STATUS_OK && match != MDLL.IBSM_ERR_MATCHING_FAILED)
                  {
                    Log.Error("FATAL: Matcher error: {Error}, {@Template}", match, new { thisTemplate.PersonId, thisTemplate.FingerId, thisTemplate.Data });
                    result = new FpIdResponse { ErrorMessage = string.Format("FATAL matcher error: {0} ", match) };

                    break;
                  }

                  // Non-fatal error?
                  if (match != MDLL.IBSM_STATUS_OK)
                  {
                    Log.Error("WARN: Matcher error: {@Template}", new { thisTemplate.PersonId, thisTemplate.FingerId, thisTemplate.Data });
                  }

                  if (match == MDLL.IBSM_STATUS_OK && thisScore > 0) // we matched
                  {
                    result = new FpIdResponse { PersonId = thisTemplate.PersonId, FingerId = thisTemplate.FingerId, Score = thisScore };
                    break;
                  }
                }
              }
              catch (Exception err)
              {
                Log.Error(err, "CreateWorkerThreads()");
                result = new FpIdResponse { ErrorMessage = err.Message };                
              }

              IncrementScanStats(scannedCount);
              Log.Information("ID Result: {@Result}: Actual scan: {Actual}ms, elapsed: {Elapsed}ms, scanned: {Scanned}, SDK: {SDK}ms, Serialize: {Serialize}ms",
                result, actual.ElapsedMilliseconds, request.Timer.ElapsedMilliseconds, scannedCount, sdkTimer.ElapsedMilliseconds, serializeTimer.ElapsedMilliseconds);

              if (result == null)
              {
                result = new FpIdResponse();
              }
              _idResponses.TryAdd(request.RequestId, result);

              if (!string.IsNullOrEmpty(result.ErrorMessage) || result.Score > 0)
              {
                request.JobToAbort.Set(); // All associated tasks to abort this job
              }

              // Tell orchestration thread we are done
              ((ManualResetEvent)request.TaskWait).Set();
            }
          }
        });

        _idWorkerThreads[i].IsBackground = true;
        _idWorkerThreads[i].Start();
      }
    }


    /// <summary>
    /// Create dedicated IB API worker threads for fingerprint template creation
    /// </summary>
    private static unsafe void CreateTemplateWorkerThreads()
    {
      if (_templateWorkerThreads != null)
      {
        return;
      }

      _templateWorkerThreads = new Thread[_templateWorkerThreadCount];
      for (var i = 0; i < _templateWorkerThreadCount; i++)
      {
        _templateWorkerThreads[i] = new Thread(() =>
        {
          var matcher_handle = -1;

          // IB DLL does not seem to be fully thread-safe? Ensure only 1 thread at a time calls MDLL._IBSM_OpenMatcher...
          lock (_ibOpenMatcherLocker)
          {
            var open = MDLL._IBSM_OpenMatcher(ref matcher_handle);
            if (open != MDLL.IBSM_STATUS_OK || matcher_handle < 0)
            {
              Log.Fatal("Failed to open matcher: {Error}", open);
              return; // abort thread
            }

            Log.Information("Got matcher (template): {Matcher}", matcher_handle);
          }

          while (true)
          {
            FpTemplateCreateRequest request;
            if (_templateRequests.TryTake(out request, Timeout.Infinite))
            {
              try
              {
                var images = request.Images.ToList();

                if (images.Count == 1)
                {
                  var ibImage = new DLL.IBSM_ImageData();
                  var ibUSDImage = new DLL.IBSM_ImageData();
                  var imageBytes = (request.IsCompressed) ? GZipUtils.DecompressToByte(images[0]) : images[0];

                  try
                  {
                    ibImage = IBTemplateUtils.RawImageToIBSMImage(imageBytes);
                    ibUSDImage = IBTemplateUtils.RawImageToIBSMImage(IBMatrixUtils.Rotate180(imageBytes));
                    var template = new MDLL.IBSM_Template();
                    var templateUSD = new MDLL.IBSM_Template();
                    var createTemplate = MDLL._IBSM_ExtractTemplate(matcher_handle, ibImage, ref template);
                    var createUSDTemplate = MDLL._IBSM_ExtractTemplate(matcher_handle, ibUSDImage, ref templateUSD);

                    _templateResponses.TryAdd(request.RequestId,
                      createTemplate == MDLL.IBSM_STATUS_OK && createUSDTemplate == MDLL.IBSM_STATUS_OK ?
                      new FpTemplateCreateResponse { Template = IBTemplateUtils.Serialize(template), ReversedTemplate = IBTemplateUtils.Serialize(templateUSD) } :
                      new FpTemplateCreateResponse { ErrorMessage = string.Format("_IBSM_ExtractTemplate Error: {0}/{1}", createTemplate, createUSDTemplate) });
                  }
                  finally
                  {
                    if (ibImage.ImageData != IntPtr.Zero)
                    {
                      Marshal.FreeHGlobal(ibImage.ImageData);
                    }
                    if (ibUSDImage.ImageData != IntPtr.Zero)
                    {
                      Marshal.FreeHGlobal(ibUSDImage.ImageData);
                    }
                  }
                }
                else if (request.Images.Count == 3)
                {
                  var ibImages = new List<DLL.IBSM_ImageData>(3);
                  var ibUSDImages = new List<DLL.IBSM_ImageData>(3);
                  try
                  {
                    foreach (var image in images)
                    {
                      var rawBuffer = (request.IsCompressed) ? GZipUtils.DecompressToByte(image) : image;
                      ibImages.Add(IBTemplateUtils.RawImageToIBSMImage(rawBuffer));
                      ibUSDImages.Add(IBTemplateUtils.RawImageToIBSMImage(IBMatrixUtils.Rotate180(rawBuffer)));
                    }

                    var template = new MDLL.IBSM_Template();
                    var createTemplate = MDLL._IBSM_SingleEnrollment(matcher_handle, ibImages[0], ibImages[1], ibImages[2], ref template);

                    var usdTemplate = new MDLL.IBSM_Template();
                    var createUSDTemplate = MDLL._IBSM_SingleEnrollment(matcher_handle, ibUSDImages[0], ibUSDImages[1], ibUSDImages[2], ref usdTemplate);

                    _templateResponses.TryAdd(request.RequestId,
                      createTemplate == MDLL.IBSM_STATUS_OK && createUSDTemplate == MDLL.IBSM_STATUS_OK ?
                      new FpTemplateCreateResponse { Template = IBTemplateUtils.Serialize(template), ReversedTemplate = IBTemplateUtils.Serialize(usdTemplate) } :
                      new FpTemplateCreateResponse { ErrorMessage = string.Format("_IBSM_SingleEnrollment Error: {0}/{1}", createTemplate, createUSDTemplate) });
                  }
                  finally
                  {
                    for (var imageIdx = 0; imageIdx < 3; imageIdx++)
                    {
                      if (ibImages[imageIdx].ImageData != IntPtr.Zero)
                      {
                        Marshal.FreeHGlobal(ibImages[imageIdx].ImageData);
                      }
                      if (ibUSDImages[imageIdx].ImageData != IntPtr.Zero)
                      {
                        Marshal.FreeHGlobal(ibUSDImages[imageIdx].ImageData);
                      }
                    }
                  }
                }
                else
                {
                  _templateResponses.TryAdd(request.RequestId,
                    new FpTemplateCreateResponse { ErrorMessage = string.Format("Invalid image count: {0}", request.Images.Count) });
                }
              }
              catch (Exception err)
              {
                Log.Error(err, "CreateTemplateThreads");
                _templateResponses.TryAdd(request.RequestId, new FpTemplateCreateResponse { ErrorMessage = err.Message });
              }
            }
          }
        });

        _templateWorkerThreads[i].IsBackground = true;
        _templateWorkerThreads[i].Start();
      }
    }


    private static void IncrementScanStats(int scansDone)
    {
      lock (_requestLocker)
      {
        if (_lastDay != DateTime.Today)
        {
          _lastDay = DateTime.Today;
          _requestsToday = 0;
        }

        _requestsAll += scansDone;
        _requestsToday += scansDone;
      }
    }


    private static void GetScanStats(out Int64 today, out Int64 total)
    {
      lock (_requestLocker)
      {
        today = _requestsToday;
        total = _requestsAll;
      }
    }

    #endregion


    #region Private enums

    private enum RequestTypes
    {
      IDTemplate = 1,
      CreateTemplate = 2
    }

    #endregion


    #region Private worker comms classes- Request/Response

    class FpIdRequest
    {
      public string RequestId { get; set; }

      public byte[] ThisTemplate { get; set; }

      public List<FPBasicTemplate> SearchTemplates { get; set; }

      public TimeSpan Timeout { get; set; }

      public Stopwatch Timer { get; set; }

      public int SecurityLevel { get; set; }

      public WaitHandle TaskWait { get; set; }
      public ManualResetEventSlim JobToAbort { get; set; }
    }

    class FpIdResponse
    {
      public Int64 PersonId { get; set; }

      public int FingerId { get; set; }

      public int Score { get; set; }

      public string ErrorMessage { get; set; }

    }


    class FpTemplateCreateRequest
    {
      public string RequestId { get; set; }
      public List<byte[]> Images { get; set; }
      public bool IsCompressed { get; internal set; }
    }


    class FpTemplateCreateResponse
    {
      public byte[] Template { get; set; }
      public byte[] ReversedTemplate { get; set; }

      public string ErrorMessage { get; set; }
    }

    #endregion


    #region Private const

    /// <summary>
    /// Worst case number of templates which we can scan/second on >= 2.2GHz
    /// </summary>
    private const int WORKER_SCAN_RATE_PER_SEC = 5000;

    #endregion


    #region Private fields

    /// <summary>
    /// Request/response for FP identification
    /// </summary>
    private static readonly BlockingCollection<FpIdRequest> _idRequests = new BlockingCollection<FpIdRequest>();
    private static readonly ConcurrentDictionary<string, FpIdResponse> _idResponses = new ConcurrentDictionary<string, FpIdResponse>();

    /// <summary>
    /// Request/response for template creation 
    /// </summary>
    private static readonly BlockingCollection<FpTemplateCreateRequest> _templateRequests = new BlockingCollection<FpTemplateCreateRequest>();
    private static readonly ConcurrentDictionary<string, FpTemplateCreateResponse> _templateResponses = new ConcurrentDictionary<string, FpTemplateCreateResponse>();


    /// <summary>
    /// IB SDK identification threads- usually match number of threads on machine (Environment.ProcessorCount=> 1 thread per core)
    /// </summary>
    private static readonly int _idWorkerThreadCount = Environment.ProcessorCount - 2;
    private static Thread[] _idWorkerThreads = null;

    /// <summary>
    /// IB SDK template conversion threads
    /// </summary>
    private static readonly int _templateWorkerThreadCount = 4;
    private static Thread[] _templateWorkerThreads = null;


    /// <summary>
    /// Single access to OpenMatcher call at any one time, as IB SDK is not a happy camper when threads make call the simultaneously
    /// </summary>
    private readonly static object _ibOpenMatcherLocker = new object();

    /// <summary>
    /// Number of requests handled today
    /// </summary>
    private static Int64 _requestsToday = 0;

    /// <summary>
    /// Number of requests handled since started
    /// </summary>
    private static Int64 _requestsAll = 0;

    /// <summary>
    /// To reset daily counter
    /// </summary>
    private static DateTime _lastDay = DateTime.Today;

    /// <summary>
    /// Safe concurrent access to above stats
    /// </summary>
    private readonly static object _requestLocker = new object();

    private static int _longRunningIdTasksInProgress = 0;

    #endregion

  }

}
