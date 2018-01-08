using System;
using System.Collections.Generic;
using System.Configuration;

using EasyNetQ;

using Atlas.FP.Identifier.MessageTypes.PubSub;
using Atlas.FP.Identifier.MessageTypes.RequestResponse;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.Comms
{
  /// <summary>
  /// Distributed communications utility methods
  /// </summary>
  internal static class DistCommUtils
  {
    #region Internal methods

    /// <summary>
    /// Initialize RabbitMQ
    /// </summary>
    internal static void Start(ILogging log)
    {
      _log = log;
      _log.Information("FP comms: Starting Bus...");
      // Fingerprint distributed identification Request/receive via RabbitMQ...
      var address = ConfigurationManager.AppSettings["fp-rabbitmq-address"];
      var virtualHost = ConfigurationManager.AppSettings["fp-rabbitmq-virtualhost"];
      var userName = ConfigurationManager.AppSettings["fp-rabbitmq-username"];
      var password = ConfigurationManager.AppSettings["fp-rabbitmq-password"];

      // FP messages do not need to be durable- they are short-lived, time-out is quick - avoid overloading
      var connectionString = string.Format("host={0};virtualHost={1};username={2};password={3};persistentMessages=false;timeout=30;product=fp.server;requestedHeartbeat=120", 
        address, virtualHost, userName, password);
      _bus = RabbitHutch.CreateBus(connectionString);
    }


    internal static void Stop()
    {
      try
      {
        if (_bus != null)
        {
          _bus.Dispose();
          _bus = null;
        }
      }
      catch
      {
      }
    }


    /// <summary>
    /// Notify distributed fingerprint identifier's, of a person's fingerprint addition
    /// </summary>
    /// <param name="personId"></param>
    public static void PublishAddedFingerprint(ICollection<Tuple<Int64, int, byte[], int>> templates)
    {
      _log.Information("AddedFingerprint starting: {@Templates}", templates);
      var fpNotification = new FPNewTemplates(templates);
      _bus.Publish<FPNewTemplates>(fpNotification, "pubsub");
    }


    /// <summary>
    /// Notify distributed fingerprint identifier's, of a person's fingerprint deletion
    /// </summary>
    /// <param name="personId"></param>
    public static void PublishDeletedFingerprint(Int64 personId)
    {
      _log.Information("DeletedFingerprint starting: {PersonId}", personId);
      var fpNotification = new FPDeleteTemplates(personId);
      _bus.Publish<FPDeleteTemplates>(fpNotification, "pubsub");
    }


    /// <summary>
    /// Request a fingerprint identification from one of the distributed fingerprint identifiers and return the person Id
    /// </summary>
    /// <param name="compressedImage"></param>
    /// <returns>PersonId matched, else -1</returns>
    public static bool IdentifyFingerprint(byte[] compressedImage, int securityLevel, out Int64 personId)
    {
      _log.Information("IdentifyFingerprint starting: {compressedImage}", compressedImage);
      personId = -1;

      try
      {
        var response = _bus.Request<IdentifyFromImageRequest, IdentifyFromImageResponse>(new IdentifyFromImageRequest(DateTime.Now, new List<byte[]> { compressedImage }, securityLevel));
        _log.Information("IdentifyFingerprint: {@response}", response);

        if (response == null)
        {
          return false;
        }

        personId = response.PersonId;
        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, "IdentifyFingerprint");
        return false;
      }
    }


    /// <summary>
    /// Request a fingerprint identification from one of the distributed fingerprint identifiers and return the person Id
    /// </summary>    
    /// <param name="compressedImages"></param>
    /// <param name="securityLevel"></param>
    /// <param name="personId"></param>
    /// <returns>true for success, false if error</returns>
    public static bool IdentifyFingerprint(ICollection<byte[]> compressedImages, int securityLevel, out Int64 personId)
    {
      _log.Information("IdentifyFingerprint starting");
      personId = -1;
      //var attempt = 1;
      //while (attempt++ <= 3) // this will this kill id server when it's under load...
      {
        try
        {
          var response = _bus.Request<IdentifyFromImageRequest, IdentifyFromImageResponse>(new IdentifyFromImageRequest(DateTime.Now, compressedImages, securityLevel));
          _log.Information("IdentifyFingerprint response: {@response}", response);

          if (response != null)
          {
            personId = response.PersonId;
            return true;
          }
        }
        catch (Exception err)
        {
          _log.Error(err, "IdentifyFingerprint");
          System.Threading.Thread.Sleep(new Random().Next(3000) + 500);
        }
      }

      return false;
    }


    /// <summary>
    /// Compare given compressed image against given templates and return the finger id matched
    /// </summary>
    /// <param name="compressedImage"></param>
    /// <param name="templates"></param>
    /// <returns></returns>
    public static bool CheckImageMatch(byte[] compressedImage, ICollection<Tuple<int, byte[]>> templates, int securityLevel, out int fingerId)
    {
      _log.Information("CheckImageMatch starting");
      fingerId = -1;

      try
      {        
        var ibTemplate = _bus.Request<CreateTemplateRequest, CreateTemplateResponse>(new CreateTemplateRequest(new List<byte[]> { compressedImage }));
        _log.Information("CheckImageMatch: {@IBTemplate}", ibTemplate);

        if (ibTemplate == null || !string.IsNullOrEmpty(ibTemplate.ErrorMessage))
        {
          return false;
        }

        var match = _bus.Request<CheckAnyCImagesMatchRequest, CheckAnyCImagesMatchResponse>(new CheckAnyCImagesMatchRequest(new List<byte[]> { ibTemplate.Template }, templates, securityLevel));
        _log.Information("CheckImageMatch: {Match}", match);

        if (match == null || !string.IsNullOrEmpty(match.ErrorMessage))
        {
          return false;
        }

        fingerId = match.FingerId;
        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, "CheckImageMatch");
        return false;
      }
    }


    /// <summary>
    /// Compare the given 'template' against 'templates' and return the finger id matched, -1 if no match
    /// </summary>
    /// <param name="template"></param>
    /// <param name="templates"></param>
    /// <returns>true if successful, else false if error</returns>
    public static bool CheckTemplateMatch(byte[] template, ICollection<Tuple<int, byte[]>> templates, int securityLevel, out int fingerId)
    {
      _log.Information("CheckTemplateMatch starting");
      fingerId = -1;

      try
      {
        var match = _bus.Request<CheckAnyTemplatesMatchRequest, CheckAnyTemplatesMatchResponse>(new CheckAnyTemplatesMatchRequest(template, templates, securityLevel));
        _log.Information("CheckTemplateMatch: {@Response}", match);

        if (match == null || !string.IsNullOrEmpty(match.ErrorMessage))
        {
          return false;
        }

        fingerId = match.FingerId;
        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, "CheckTemplateMatch");
        return false;
      }
    }


    /// <summary>
    /// Create reversed and standard template from 3 images
    /// </summary>
    /// <param name="compressedImages"></param>
    /// <param name="template"></param>
    /// <param name="reversedTemplate"></param>
    /// <returns>true if successful, else false if error</returns>
    public static bool CreateTemplates(ICollection<byte[]> compressedImages, out byte[] template, out byte[] reversedTemplate)
    {
      _log.Information("CreateTemplates starting");

      template = null;
      reversedTemplate = null;      

      try
      {
        var ibTemplate = _bus.Request<CreateTemplateRequest, CreateTemplateResponse>(new CreateTemplateRequest(compressedImages));
        _log.Information("CreateTemplates: {@Response}", ibTemplate);

        if (ibTemplate == null || ibTemplate.Template == null || ibTemplate.ReversedTemplate == null)
        {
          return false;
        }

        template = ibTemplate.Template;
        reversedTemplate = ibTemplate.ReversedTemplate;
        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, "CreateTemplates");
        return false;
      }
    }


    /// <summary>
    /// Create reversed and standard template from a single image
    /// </summary>
    /// <param name="compressedImage"></param>
    /// <param name="template"></param>
    /// <param name="reversedTemplate"></param>
    /// <returns></returns>
    public static bool CreateTemplates(byte[] compressedImage, out byte[] template, out byte[] reversedTemplate)
    {
      _log.Information("CreateTemplates starting");
      template = null;
      reversedTemplate = null;

      try
      {
        var ibTemplate = _bus.Request<CreateTemplateRequest, CreateTemplateResponse>(new CreateTemplateRequest(new List<byte[]> { compressedImage }));
        _log.Information("CreateTemplates: {@IBTemplate}", ibTemplate);
        if (ibTemplate == null || ibTemplate.Template == null || ibTemplate.ReversedTemplate == null)
        {
          return false;
        }

        template = ibTemplate.Template;
        reversedTemplate = ibTemplate.ReversedTemplate;
        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, "CreateTemplates");
        return false;
      }
    }


    /// <summary>
    /// Get finger ids (1-10) enrolled for person
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    public static ICollection<int> GetFingersEnrolled(Int64 personId)
    {
      _log.Information("GetFingersEnrolled starting: {PersonId}", personId);

      try
      { var fingers = _bus.Request<GetFingersRequest, GetFingersResponse>(new GetFingersRequest(personId));
        _log.Information("GetFingersEnrolled response: {@Fingers}", fingers);

        return (fingers != null) ? fingers.FingerIds : null;
      }
      catch (Exception err)
      {
        _log.Error(err, "GetFingersEnrolled");
        return null;
      }
    }


    /// <summary>
    /// Get templates for a specific person
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    public static ICollection<Tuple<int, byte[]>> GetTemplates(Int64 personId)
    {
      _log.Information("GetTemplates starting: {PersonId}", personId);

      try
      {
        var ibTemplates = _bus.Request<GetTemplatesForRequest, GetTemplatesForResponse>(new GetTemplatesForRequest(personId));
        _log.Information("GetTemplates: {@IBTemplates}", ibTemplates);
        return (ibTemplates != null) ? ibTemplates.Templates : null;
      }
      catch (Exception err)
      {
        _log.Error(err, "GetTemplates");
        return null;
      }
    }

    #endregion


    #region Private fields

    private static ILogging _log;

    /// <summary>
    /// EasyNetQ RabbitMQ
    /// </summary>
    private static IBus _bus;


    #endregion

  }

}
