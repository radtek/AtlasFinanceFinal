using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Serilog;

using Atlas.FP.Identifier.ThreadSafe;
using Atlas.FP.Identifier.SDK.Utils;
using Atlas.FP.Identifier.MessageTypes.RequestResponse;
using Atlas.Common.Interface;

namespace Atlas.FP.Identifier.EasyNetQ
{
  /// <summary>
  /// Handles incoming RabbitMQ requests and responds
  /// </summary>
  public static class IBConsumer
  {
    /// <summary>
    /// Identify person from image(s), using in-memory templates
    /// </summary>
    /// <param name="context"></param>
    /// <returns>IdentifyFromImageResponse response</returns>
    public static IdentifyFromImageResponse IdentifyFromImage(ILogging log, IdentifyFromImageRequest context)
    {
      Log.Information("IdentifyFromImageRequest: {@Request}", context);

      try
      {
        int fingerId;
        long personId;
        int score;
        string errorMessage;

        var timer = Stopwatch.StartNew();
        IBUtils.IdentifyPersonByImages(log, context.Images, context.SecurityLevel, out score, out personId, out fingerId, out errorMessage);
        var result = new IdentifyFromImageResponse(personId, fingerId, timer.ElapsedMilliseconds, score);

        Log.Information("IdentifyFromImageRequest: {@Result}", result);
        return result;
      }
      catch (Exception err)
      {
        Log.Error(err, "IdentifyFromImage");
        return null;
      }
    }


    /// <summary>
    /// Check for match using compressed image(s) and given templates
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static CheckAnyCImagesMatchResponse CheckCImageMatch(ILogging log, CheckAnyCImagesMatchRequest context)
    {
      Log.Information("CheckCImageMatch: {@Request}", context);

      try
      {
        string errorMessage = null;
        var fingerId = -1;
        var score = -1;
        IBUtils.CheckForMatchByImages(log, context.CompressedImages.ToList(), context.Templates.ToList(),
          context.SecurityLevel, out score, out fingerId, out errorMessage);

        var result = new CheckAnyCImagesMatchResponse(fingerId, errorMessage);
        Log.Information("CheckCImageMatch: {@Result}", result);

        return result;
      }
      catch (Exception err)
      {
        Log.Error(err, "CheckCImageMatch");
        return new CheckAnyCImagesMatchResponse(0, err.Message);
      }
    }


    /// <summary>
    /// Check for match using template and given templates
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static CheckAnyTemplatesMatchResponse CheckTemplatesMatch(ILogging log, CheckAnyTemplatesMatchRequest context)
    {
      Log.Information("CheckTemplatesMatch: {@Request}", context);

      try
      {
        string errorMessage = null;
        var fingerId = -1;
        var score = -1;
        IBUtils.CheckForMatchByTemplate(log, context.Template, context.Templates, context.SecurityLevel, out score, out fingerId, out errorMessage);

        var result = new CheckAnyTemplatesMatchResponse(fingerId, errorMessage);
        Log.Information("CheckTemplatesMatch: {@Result}", result);

        return result;
      }
      catch (Exception err)
      {
        Log.Error(err, "CheckTemplatesMatch");
        return new CheckAnyTemplatesMatchResponse(0, err.Message);
      }
    }


    /// <summary>
    /// Get template for a person
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static GetTemplatesForResponse GetTemplatesFor(ILogging log, GetTemplatesForRequest context)
    {
      Log.Information("GetTemplatesFor: {@Request}", context);

      try
      {
        var templates = FPThreadSafe.GetIBTemplatesFor(log, context.PersonId);

        // Make copy
        var result = new List<Tuple<int, byte[]>>();
        foreach (var template in templates)
        {
          var data = new byte[template.Data.Length];
          Array.Copy(template.Data, data, template.Data.Length);
          result.Add(new Tuple<int, byte[]>(template.FingerId, data));
        }

        Log.Information("GetTemplatesFor: {@Result}", result);
        return new GetTemplatesForResponse(result);
      }
      catch (Exception err)
      {
        Log.Error(err, "GetTemplatesFor");
        return null;
      }
    }

    
    /// <summary>
    /// Get fingers listing enrolled for a person
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static GetFingersResponse GetFingers(GetFingersRequest context)
    {
      Log.Information("GetFingers: {@Request}", context);
      try
      {
        var result = new GetFingersResponse(FPThreadSafe.GetAllFingerIds(context.PersonId));
        Log.Information("GetFingers: {@Result}", result);
        return result;
      }
      catch (Exception err)
      {
        Log.Error(err, "GetFingers");
        return null; 
      }
    }


    /// <summary>
    /// Create a template from given compressed images
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static CreateTemplateResponse CreateTemplate(CreateTemplateRequest context)
    {
      Log.Information("CreateTemplate: {@Request}", context);

      try
      {
        byte[] template;
        byte[] reversedTemplate;
        string errorMessage;

        IBUtils.CreateTemplate(context.CompressedImages.ToList(), out template, out reversedTemplate, out errorMessage);
        var result = new CreateTemplateResponse(template, reversedTemplate, errorMessage);
        Log.Information("CreateTemplate: {@Result}", result);
        return result;
      }
      catch (Exception err)
      {
        Log.Error(err, "CreateTemplate");
        return new CreateTemplateResponse(null, null, err.Message);
      }
    }

  }
}