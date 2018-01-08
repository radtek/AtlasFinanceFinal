using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using DevExpress.Xpo;

using Atlas.Domain.Model.Biometric;
using Atlas.Enumerators;
using Atlas.MongoDB.Entities;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.WCF.FPServer.Comms;
using Atlas.WCF.FPServer.MongoDB;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Server
{
  public static class EndEnrollPerson_Impl
  {
    public static int EndEnrollPerson(ILogging log, SourceRequest sourceRequest, Int64 startEnrollRef, out string errorMessage)
    {
      var methodName = "EndEnrollPerson";
      errorMessage = null;
      try
      {
        log.Information("{MethodName} starting: {@Request}, {Reference}", methodName, sourceRequest, startEnrollRef);

        Int64 personId = 0;

        #region Validate parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        #region Check session

        using (var unitOfWork = new UnitOfWork())
        {
          #region Session from the same machine?
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
          #endregion

          var uploadedFingerCount = unitOfWork.Query<BIO_UploadBitmap>().Where(s => s.FPUploadSession == session).Select(s => s.FingerId).Distinct().Count();
          if (uploadedFingerCount < 2)
          {
            errorMessage = string.Format("Must capture at least 2 fingers- only {0} captured ", uploadedFingerCount);
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }

          personId = session.PersonId;
        }

        #endregion

        #region Check SQL

        #endregion

        #endregion

        using (var unitOfWork = new UnitOfWork())
        {
          var session = unitOfWork.Query<BIO_UploadSession>().First(s => s.FPUploadSessionId == startEnrollRef);
          var sessionTemplates = unitOfWork.Query<BIO_UploadTemplate>().Where(s => s.FPUploadSession == session);
          var sessionBitmaps = unitOfWork.Query<BIO_UploadBitmap>().Where(s => s.FPUploadSession == session);

          var uniqueTemplateFingers = sessionTemplates.Select(s => s.FingerId).Distinct().Count();
          if (uniqueTemplateFingers < 2)
          {
            errorMessage = "Must enroll at least 2 finger templates";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }

          var uniqueBitmapFingers = sessionBitmaps.Select(s => s.FingerId).Distinct().Count();
          if (uniqueBitmapFingers < 2)
          {
            errorMessage = "Must enroll at least 2 finger bitmaps";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }

          // We should have the same or more bitmaps, than templates
          if (uniqueBitmapFingers < uniqueTemplateFingers)
          {
            errorMessage = "More templates than bitmaps";
            log.Warning("EndEnrollPerson", new Exception(errorMessage));
            return (int)General.WCFCallResult.BadParams;
          }

          #region Create Mongo standard template objects
          var newTemplates = new List<FPTemplate2>();
          foreach (var template in sessionTemplates)
          {
            var newTemplate = new FPTemplate2()
              {
                CreatedDT = template.UploadedDate,
                FingerId = template.FingerId,
                PersonId = personId,
                Orientation = template.Orientation == Biometric.OrientationType.RightSide ? 0 : 1,
                CreatedPersonId = session.UserPersonId,
                TemplateBuffer = new byte[template.FPTemplate.Length]
              };
            Array.Copy(template.FPTemplate, newTemplate.TemplateBuffer, template.FPTemplate.Length);
            newTemplates.Add(newTemplate);
          }
          #endregion

          #region Create Mongo bitmap objects
          var newBitmaps = new List<FPBitmap2>();
          foreach (var bitmap in sessionBitmaps)
          {
            newBitmaps.Add(new FPBitmap2()
              {
                CreatedDT = bitmap.UploadedDate,
                FingerId = bitmap.FingerId,
                PersonId = personId,
                NFIQ = bitmap.NFIQ,
                Width = 288,
                Height = 352,
                Bitmap = bitmap.FPBitmap,
                CreatedPersonId = session.UserPersonId
              });
          }
          #endregion

          #region Add objects to MongoDB
          var client = MongoDbUtils.GetMongoClient();
          var database = client.GetDatabase("fingerprint");

          var task = Task.Run(async () =>
            {
              var mongoTemplates = database.GetCollection<FPTemplate2>("fpTemplate2");
              await mongoTemplates.InsertManyAsync(newTemplates);

              var mongoBitmaps = database.GetCollection<FPBitmap2>("fpBitmap2");
              await mongoBitmaps.InsertManyAsync(newBitmaps);
            });
          task.Wait();

          #endregion

          // Notify distributed identifiers of new templates
          DistCommUtils.PublishAddedFingerprint(sessionTemplates.Select(s => new Tuple<Int64, int, byte[], int>(
            session.PersonId, s.FingerId, s.FPTemplate, s.Orientation == Biometric.OrientationType.RightSide ? 0 : 1)).ToList());

          #region Clean SQL session
          foreach (var template in sessionTemplates)
          {
            template.Delete();
          }
          foreach (var bitmap in sessionBitmaps)
          {
            bitmap.Delete();
          }
          session.Delete();
          unitOfWork.CommitChanges();
          #endregion
        }

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
