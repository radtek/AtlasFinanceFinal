/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Quartz.net task to download all NuCard transactions for reconciliation/alerts
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2014-03-17- Skeleton created
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;

using Quartz;
using Serilog;
using DevExpress.Xpo;

using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Domain.DTO.Nucard;


namespace Atlas.Server.NuCard.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class GetTutukaTrans : IJob
  {
    public void Execute(IJobExecutionContext context)
    {
      Log.Information("NuCard transactions download Task starting...");

      try
      {
        #region Get profiles
        List<NUC_NuCardProfileDTO> profiles;
        using (var unitOfWork = new UnitOfWork())
        {
          profiles = AutoMapper.Mapper.Map<List<NUC_NuCardProfileDTO>>(unitOfWork.Query<NUC_NuCardProfile>());
        }
        #endregion

        foreach (var profile in profiles)
        {
          var dateStart = DateTime.Now;          // TODO: Get last successful downloaded batch for this profile
          var dateEnd = dateStart.AddDays(1);
          var dateFormat = "yyyy-MM-dd";

          try
          {
            new TutukaNuCardService.wsNucardSoapClient().Using(client =>
            {
              var trans = client.GetProfileTransactionData("atlas", "AtlasNuCard123@", profile.ProfileNum, dateStart.ToString(dateFormat), dateEnd.ToString(dateFormat));
              if (trans != null)
              {
                // Convert to XmlDocument so we can use LINQ2XML
                var doc = new XmlDocument();
                doc.AppendChild(doc.ImportNode(trans, true));
                var transactions = XElement.Parse(doc.InnerXml);

                foreach (var transaction in transactions.Descendants("Transaction"))
                {
                  var transSource = transaction.Element("transaction_source").Value.ToLower(); // batch / terminal / web
                  var transAmount = Decimal.Parse(transaction.Element("transaction_amount").Value,
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.Number,
                    CultureInfo.InvariantCulture);

                  var transType = transaction.Element("transaction_type").Value.ToLower();
                  switch (transType)
                  {
                    case "fee":

                      break;

                    case "direct deposit":

                      break;

                    case "transfer":

                      break;

                    default:
                      throw new ArgumentOutOfRangeException("transaction_type", string.Format("Unknown type: '{0}'", transType));
                  }
                }
              }
            });
          }
          catch (Exception err)
          {
            Log.Error("Error with profile {0}: '{1}'", profile.ProfileNum, err.Message);
          }
        }
      }
      catch (Exception err)
      {
        Log.Error("Execute: {0}", err);
      }

      Log.Information("NuCard transactions download Task completed");
    }

  }
}