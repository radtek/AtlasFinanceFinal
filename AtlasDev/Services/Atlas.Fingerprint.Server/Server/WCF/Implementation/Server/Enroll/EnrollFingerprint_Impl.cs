using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model.Biometric;
using Atlas.Enumerators;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.WCF.FPServer.Comms;
using Atlas.Domain.Model;
using Atlas.Domain.Security;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Server.Enroll
{
  /// <summary>
  /// Enroll a single fingerprint- requires three 256 gray scale, compressed fingerprint bitmaps
  /// Steps: 
  ///   1. Decompress the bitmap buffer
  ///   2. Check the image quality
  ///   3. Create upside-down version of template
  ///   4. Create IB 'composite' template from bitmaps
  ///   5. Save results to the SQL session
  /// </summary>
  public static class EnrollFingerprint_Impl
  {
    public static int EnrollFingerprint(ILogging log, SourceRequest sourceRequest, Int64 startEnrollRef, List<FPRawBufferDTO> fpBitmaps,
      out string errorMessage)
    {
      var methodName = "EnrollFingerprint";
      errorMessage = null;
      try
      {
        log.Information("{MethodName} starting: {@Request}, {Reference}, {@fpBitmaps}", methodName, sourceRequest, startEnrollRef, fpBitmaps);

        #region Validate parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        #region Basics

        if (fpBitmaps == null || fpBitmaps.Count != 3)
        {
          errorMessage = "fpBitmaps must contain 3 bitmaps of the same finger!";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        #region Session from the same machine?
        Int64 personId = 0;
        using (var unitOfWork = new UnitOfWork())
        {
          var session = unitOfWork.Query<BIO_UploadSession>().FirstOrDefault(s => s.FPUploadSessionId == startEnrollRef);
          if (session == null)
          {
            errorMessage = "No session found!";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }

          if (session.Machine.MachineId != machine.Id)
          {
            errorMessage = "Invalid session- session belongs to another machine";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }

          personId = session.PersonId;
        }
        #endregion

        #region Ensure all bitmaps are for the same person, the same finger and contain data
        var fingerId = fpBitmaps[0].FingerId;
        foreach (var bitmap in fpBitmaps)
        {
          if (bitmap.FingerId != fingerId)
          {
            errorMessage = "Must be fingerprints for one finger";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }

          if (bitmap.RawBuffer.Length == 0)
          {
            errorMessage = "RawBuffer cannot be empty";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }
        }
        #endregion

        #endregion
                
        #region Add session data to SQL
        using (var unitfWork = new UnitOfWork())
        {
          var session = unitfWork.Query<BIO_UploadSession>().FirstOrDefault(s => s.FPUploadSessionId == startEnrollRef);
          if (session == null)
          {
            errorMessage = "Session could not be located";
            return (int)General.WCFCallResult.ServerError;
          }

          #region Ensure is unique in all FP templatess
          var stopwatch = Stopwatch.StartNew();
          var foundExistingPersonId = -1L;
          if (!DistCommUtils.IdentifyFingerprint(fpBitmaps.Select(s => s.RawBuffer).ToList(), 6, out foundExistingPersonId))
          {
            return (int)General.WCFCallResult.ServerError;
          }

          stopwatch.Stop();
          log.Information("{MethodName}: Check for ANY duplicates scanned in {1}ms- {Found}", methodName, stopwatch.Elapsed.Milliseconds, foundExistingPersonId);

          if (foundExistingPersonId > 0)
          {
            errorMessage = "The fingerprint matches an existing fingerprint- this fingerprint has already been enrolled in the system.";
            log.Warning(new Exception(errorMessage), methodName);
                       
            new BIO_LogRequest(unitfWork)
              {               
                BiometricClass = Biometric.BiometricClass.Fingerprint,
                StartDT = sourceRequest.MachineDateTime,
                EndDT = DateTime.Now,
                FPResult = Biometric.RequestStatus.EnrollmentDuplicated,
                LogRequestId = startEnrollRef,
                Machine = unitfWork.Query<COR_Machine>().First(s => s.MachineId == machine.Id),
                Person = unitfWork.Query<PER_Person>().First(s => s.PersonId == session.PersonId),
                UserPerson = unitfWork.Query<PER_Person>().First(s => s.PersonId == session.UserPersonId),
                Error = foundExistingPersonId.ToString(), // TODO: Change to own field... SecondaryPerson
                //RequestId = ??? Enrollment uses Int64...!
            };
            unitfWork.CommitChanges();

            return (int)General.WCFCallResult.BadParams;
          }
          #endregion
          
          #region Ensure fingerprint is unique in current session
          byte[] reversedTemplate;
          byte[] compositeTemplate;
          if (!DistCommUtils.CreateTemplates(fpBitmaps.Select(s => s.RawBuffer).ToList(), out compositeTemplate, out reversedTemplate))
          {
            errorMessage = "Failed to create composite template- please recapture";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }

          var thisSessionTemplates = unitfWork.Query<BIO_UploadTemplate>().Where(s => s.FPUploadSession == session)
            .Select(s => new Tuple<int, byte[]>(s.FingerId, s.FPTemplate))
            .ToList();

          var foundInSession = -1;
          if (!DistCommUtils.CheckTemplateMatch(compositeTemplate, thisSessionTemplates, 6, out foundInSession))
          {
            errorMessage = "Failed to check templates";
            return (int)General.WCFCallResult.ServerError;
          }

          var foundInSessionUSD = -1;
          if (!DistCommUtils.CheckTemplateMatch(reversedTemplate, thisSessionTemplates, 6, out foundInSessionUSD))
          {
            errorMessage = "Failed to check templates";
            return (int)General.WCFCallResult.ServerError;
          }

          if (foundInSession > 0 || foundInSessionUSD > 0)
          {
            errorMessage = "The fingerprint matches an existing fingerprint- this fingerprint has already been enrolled for this person";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }
          #endregion

          #region Delete existing template/bitmaps for this session and this finger (in case of user error, allow recapturing)
          unitfWork.Query<BIO_UploadTemplate>().Where(s => s.FPUploadSession == session && s.FingerId == fingerId).ToList().ForEach(s => s.Delete());
          unitfWork.Query<BIO_UploadBitmap>().Where(s => s.FPUploadSession == session && s.FingerId == fingerId).ToList().ForEach(s => s.Delete());
          #endregion

          #region Add the templates
          // Standard template
          var addTemplate = new BIO_UploadTemplate(unitfWork)
            {
              FPUploadSession = session,
              FingerId = fingerId,
              UploadedDate = DateTime.Now,
              Orientation = Biometric.OrientationType.RightSide,
              FPTemplate = compositeTemplate              
            };

          // Upside-down template
          var addReversedTemplate = new BIO_UploadTemplate(unitfWork)
            {
              FPUploadSession = session,
              FingerId = fingerId,
              UploadedDate = DateTime.Now,
              Orientation = Biometric.OrientationType.UpsideDown,
              FPTemplate = reversedTemplate
            };
          #endregion

          #region Add standard bitmaps
          foreach (var bitmap in fpBitmaps)
          {
            var newBitmap = new BIO_UploadBitmap(unitfWork)
            {
              FPUploadSession = session,
              FingerId = fingerId,
              UploadedDate = DateTime.Now,
              //Quality = bitmap.Quality,
              NFIQ = bitmap.NFIQ,
              FPBitmap = new byte[bitmap.RawBuffer.Length],
            };
            Array.Copy(bitmap.RawBuffer, newBitmap.FPBitmap, bitmap.RawBuffer.Length);
          }
          #endregion

          session.LastUploadDate = DateTime.Now;
          unitfWork.CommitChanges();
        }
        #endregion

        log.Information("{MethodName} completed successfully", methodName);
        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = FPActivation.SERVER_ERR_UNEXPECTED;
        return (int)General.WCFCallResult.ServerError;
      }
    }

  }
}
