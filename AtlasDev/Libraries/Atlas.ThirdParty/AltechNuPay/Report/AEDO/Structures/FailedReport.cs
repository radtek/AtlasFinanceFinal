using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Domain.DTO;
using System.Xml.Linq;
using Atlas.ThirdParty.AltechNuPay.Report;
using System.Globalization;

namespace Altech.NuPay.Business.FileStructures.Reports.AEDO
{
    public class FailedReport
    {
        #region public Fields

        public AEDOReportFailedDTO aedoReport;

        #endregion

        public FailedReport(XElement item)
        {
            aedoReport = new AEDOReportFailedDTO();

            aedoReport.ReportFailed.TransactionId = Int64.Parse(item.Element("transaction_id").Value);
            aedoReport.ReportFailed.ContractRef = item.Element("contract_ref").Value;
            aedoReport.ReportFailed.ServiceType = item.Element("service_type").Value;
            aedoReport.ReportFailed.ValueDT = DateTime.ParseExact(item.Element("value_date").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            aedoReport.ReportFailed.FailDT = DateTime.ParseExact(item.Element("fail_date").Value + " " + item.Element("fail_time").Value, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            aedoReport.StartDT = DateTime.ParseExact(item.Element("start_date").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            aedoReport.ContractNum = item.Element("contract_no").Value;
            aedoReport.Reason = item.Element("reason").Value;
            aedoReport.ContractAmount = FileStructureHelper.ConvertStringToDecimalPoint(item.Element("contract_amount").Value);
            aedoReport.InstalmentNum = int.Parse(item.Element("instalment_no").Value);
            aedoReport.InstalmentAmount = int.Parse(item.Element("instalment_amount").Value);
            aedoReport.EmployerCode = item.Element("employer_code").Value;
            aedoReport.TrackingIndicator = item.Element("tracking_indicator").Value;
            aedoReport.Frequency = int.Parse(item.Element("frequency").Value);
            aedoReport.Pan = item.Element("pan").Value;
            aedoReport.TerminalNum = int.Parse(item.Element("terminal_no").Value);
            aedoReport.Resubmit = item.Element("resubmit").Value == "1";
            aedoReport.CardAcceptor = item.Element("card_acceptor").Value;
            aedoReport.InstitutionId = item.Element("institution_id").Value;
            aedoReport.IdNumber = item.Element("id_number").Value;
        }
    }
}