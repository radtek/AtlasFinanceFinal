using System;
using System.Linq;
using System.Text;

using DevExpress.Xpo;
using Atlas.Common.Interface;

using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.Common.Extensions;
using Atlas.Cache.Interfaces;
using Atlas.Server.Classes.CustomException;
using Atlas.Data.Repository;


namespace Atlas.Server.WCF.Implementation.ASS
{
  internal static class GetAppSettings_Impl
  {
    internal static int Execute(ILogging log, IConfigSettings config, ICacheServer cache,
      string branchNum,
      out string settings, out string errorMessage)
    {
      settings = string.Empty;
      errorMessage = string.Empty;
      var methodName = "GetAppSettings";
      try
      {
        #region Check parameters
        if (string.IsNullOrEmpty(branchNum))
        {
          throw new BadParamException($"Invalid branch number: '{branchNum}'");
        }
        #endregion

        var branch = AtlasData.FindBranch(branchNum);
        if (branch == null)
        {
          errorMessage = string.Format("Unable to locate branch {0} in branch listing", branchNum); ;
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }
        
        var result = new StringBuilder();
        using (var uow = new UnitOfWork())
        {          
          #region Get setting in xHarbour Hash string style: { "Val1" => "123", "Val2" => "4566" }
          //                                                                                   Non-secure items are below 10000
          var branchSettings = uow.Query<BRN_Config>().Where(s => s.Branch.BranchId == branch.BranchId && (int)s.DataType < 10000);

          result.Append("{");
          var firstLine = true;
          foreach (var setting in branchSettings)
          {
            result.AppendFormat("{0}\"{1}\" => \"{2}\"",
              EnumExtension.ToStringEnum(setting.DataType), setting.DataValue, firstLine ? "" : ", ");
            firstLine = false;
          }
          result.Append("}");

          var xHarbourHashStr = result.ToString();
          settings = ASSUtils.ASSEncrypt(xHarbourHashStr);
          #endregion
        }

        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = (err is BadParamException) ? err.Message : "Unexpected server error";
        return (err is BadParamException) ? (int)General.WCFCallResult.BadParams : (int)General.WCFCallResult.ServerError;
      }
    }
  }
}
