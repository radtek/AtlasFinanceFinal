using System;
using System.Configuration;
using System.Threading.Tasks;

using MongoDB.Driver;

using Atlas.WCF.FPServer.Common;
using Atlas.WCF.FPServer.MongoDB;
using Atlas.Enumerators;
using Atlas.MongoDB.Entities;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Server.Admin
{
  public static class DeleteData_Impl
  {
    public static int DeleteData(ILogging log, SourceRequest sourceRequest, Int64 personId, FPDataType dataType,
       out string errorMessage)
    {
      var methodName = "DeleteData";
      errorMessage = null;
      try
      {
        #region Check request parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        if (personId <= 0)
        {
          errorMessage = "Invalid personId parameter";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        #region Check user
        if (sourceRequest.UserPersonId == 0)
        {
          errorMessage = "userPersonId cannot be blank!";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        if (!Utils.ServerUtils.CheckUserHasRole(sourceRequest.UserPersonId, General.RoleType.CanEnrollFingerprints, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        #endregion
                
        var client = MongoDbUtils.GetMongoClient();
        var database = client.GetDatabase("fingerprint");
        string err = null;

        switch (dataType)
        {
          case FPDataType.Bitmap:
            var taskBitmaps = Task.Run<int>(async () =>
             {
               var fpAllBitmaps = database.GetCollection<FPBitmap2>("fpBitmap2");
               var filterBmp = Builders<FPBitmap2>.Filter.Eq("PersonId", personId);
               var bmpCount = await fpAllBitmaps.CountAsync(filterBmp);
               if (bmpCount > 0)
               {
                 await fpAllBitmaps.DeleteManyAsync(filterBmp);
                 return (int)General.WCFCallResult.OK;
               }
               else
               {
                 err = "No bitmaps exist";
                 return (int)General.WCFCallResult.BadParams;
               }
             });
            taskBitmaps.Wait();
            errorMessage = err;
            return taskBitmaps.Result;

          case FPDataType.Template:
            var taskTemplate = Task.Run<int>(async () =>
             {
               var fpAllTemplates = database.GetCollection<FPTemplate2>("fpTemplate2");
               var filterTemp = Builders<FPTemplate2>.Filter.Eq("PersonId", personId);
               var tempCount = await fpAllTemplates.CountAsync(filterTemp);
               if (tempCount > 0)
               {
                 await fpAllTemplates.DeleteManyAsync(filterTemp);
                 return (int)General.WCFCallResult.OK;
               }
               else
               {
                 err = "No templates exist";
                 return (int)General.WCFCallResult.BadParams;
               }
             });
            taskTemplate.Wait();
            errorMessage = err;
            return taskTemplate.Result;

          default:
            errorMessage = "Parameter dataType cannot be empty";
            return (int)General.WCFCallResult.BadParams;

        }
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
