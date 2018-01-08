using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Helpers;
using Atlas.Online.Web.Models;
using Atlas.Online.Web.WebService;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Controllers
{
  [Authorize]
  public class ReportsController : AppController
  {
    //
    // GET: /Report/
    public ActionResult PaidUpLetter(long? id)
    {
      if (id == null)
        throw new NullReferenceException("ApplicationId is null");

      var application = Application.GetFirstBy(Services.XpoUnitOfWork, x => x.Client.UserId == WebSecurity.CurrentUserId && x.ApplicationId == id);

      if (application == null)
        return new HttpUnauthorizedResult();

      PaidUpLetterModel paidUpLetter = null;

      using (var result = new WebServiceClient("WebServer.NET"))
      {
        var paidUp = result.MYC_GetPaidUpLetter((long)id);

        paidUpLetter = new PaidUpLetterModel()
        {
          AccountNo = paidUp.AccountNo,
          ClientName = paidUp.ClientName,
          Email = paidUp.Email,
          IdNo = paidUp.IdNo
        };
      }


     // byte[] buf = sc.Convert(new ObjectConfig(), htmlText.Text);
   

      PDFServer.PDFServerClient pdf = new PDFServer.PDFServerClient("PDFServer.NET");

      var rr = pdf.GetPdf(RenderRazorViewToString("PaidUpLetter", paidUpLetter));

      MemoryStream workStream = new MemoryStream();

      byte[] byteInfo = rr.Bytes;
      workStream.Write(byteInfo, 0, byteInfo.Length);


      workStream.Position = 0;

      HttpContext.Response.AddHeader("content-disposition", string.Format("attachment; filename=PaidUpLetter-{0}.pdf", paidUpLetter.AccountNo));

      return new FileStreamResult(workStream, "application/pdf"); 
    }

    public ActionResult Contract(long? id)
    {
      if (id == null)
        throw new NullReferenceException("ApplicationId is null");

      var application = Application.GetFirstBy(Services.XpoUnitOfWork, x => x.Client.UserId == WebSecurity.CurrentUserId && x.ApplicationId == id);

      if (application == null)
        return new HttpUnauthorizedResult();

      ContractModel ContractModel = null;

      using (var result = new WebServiceClient("WebServer.NET"))
      {
        var quotation = result.MYC_GetQuote((long)id);

        ContractModel = new ContractModel()
        {
          AccountId = quotation.AccountId,
          AccountNo = quotation.AccountNo,
          Amount = quotation.Amount,
          Bank = quotation.Bank,
          BankAccountName = quotation.BankAccountName,
          BankAccountNo = quotation.BankAccountNo,
          BankAccountType = quotation.BankAccountType,
          BankBranch = quotation.BankBranch,
          ContactNumber = quotation.ContactNumber,
          DateOfDebit = quotation.DateOfDebit,
          DebitAmount = quotation.DebitAmount,
          FirstName = quotation.FirstName,
          IdNumber = quotation.IdNumber,
          InitiationFeeAndServiceFee = (quotation.InitiationFee + quotation.ServiceFee),
          InterestRate = quotation.InterestRate,
          LastName = quotation.LastName,
          QuotationId = quotation.QuotationId,
          QuotationNo = quotation.QuotationNo,
          QuoteDate = quotation.QuoteDate,
          RepaymentAmount = quotation.RepaymentAmount,
          RepaymentDate = quotation.RepaymentDate,
          ResidentialAddressCode = quotation.ResidentialAddressCode,
          ResidentialAddressLine1 = quotation.ResidentialAddressLine1,
          ResidentialAddressLine2 = quotation.ResidentialAddressLine2,
          ResidentialAddressLine3 = quotation.ResidentialAddressLine3,
          ResidentialAddressLine4 = quotation.ResidentialAddressLine4
        };
      };

      PDFServer.PDFServerClient pdf = new PDFServer.PDFServerClient("PDFServer.NET");

      var rr = pdf.GetPdf(RenderRazorViewToString("Contract", ContractModel));

      MemoryStream workStream = new MemoryStream();

      byte[] byteInfo = rr.Bytes;
      workStream.Write(byteInfo, 0, byteInfo.Length);


      workStream.Position = 0;

      HttpContext.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}.pdf", ContractModel.AccountNo));

      return new FileStreamResult(workStream, "application/pdf"); 
    }

    public ActionResult Statement(long? id)
    {
      if (id == null)
        throw new NullReferenceException("ApplicationId is null");

      StatementModel statement = null;

      var application = Application.GetFirstBy(Services.XpoUnitOfWork, x => x.Client.UserId == WebSecurity.CurrentUserId && x.ApplicationId == id);

      if (application == null)
        return new HttpUnauthorizedResult();


      using (var result = new WebServiceClient("WebServer.NET"))
      {
        var r = result.MYC_GetClientStatement((long)id);
        statement = new StatementModel()
        {
          AccountNo = r.AccountNo,
          IDNo = r.IdNumber,
          DaysOverdue = r.DaysOverdue,
          AdminFeesLevied = r.DefaulAdminFeesLevied,
          AmountOverdue = r.AmountOverdue,
          Arrears = r.Arrears,
          Arrears120 = r.ArrearsAging120Days,
          Arrears150 = r.ArrearsAging150Days,
          Arrears30 = r.ArrearsAging30Days,
          Arrears60 = r.ArrearsAging60Days,
          Arrears90 = r.ArrearsAging90Days,
          ArrearsCurrent = r.ArrearsAgingCurrent,
          ArrearsTotalDue = r.ArrearsAgingTotalDue,
          ClientName = r.ClientName,
          ContactNo = r.ContactNumber,
          CurrentBalance = r.CurrentBalance,
          CurrentDue = r.CurrentDue,
          Interestedaccured = r.InterestAccrued,
          InterestLevied = r.FeesLevied,
          LegalFeesLevied = r.LegalFeesLevied,
          LoanAmount = r.LoanAmount,
          OtherCredits = r.OtherCredits,
          MonlthyInterest = r.MonthlyInterestRate,
          OtherDebits = r.OtherDebits,
          PaymentReceived = r.PaymentReceived,
          PhysicalAddress = r.PhysicalAddress,
          RepaymentAmount = r.RepaymentAmount,
          RepaymentDate = r.RepaymentDate,
          StartDate = (DateTime)r.StartDate,
          StatementDate = r.StatementDate,
          Term = r.Term,
          TotalDue = r.TotalDue,
          StatementList = new List<StatementModel.StatementListModel>()
        };
        foreach (var i in r.StatementTransactions)
        {
          statement.StatementList.Add(new StatementModel.StatementListModel()
          {
            Date = i.TransactionDate,
            Description = i.Description,
            Debit = i.DebitAmount,
            Credit = i.CreditAmount,
            Balance = i.Balance
          });
        }
      }

      PDFServer.PDFServerClient pdf = new PDFServer.PDFServerClient("PDFServer.NET");

      var rr = pdf.GetPdf(RenderRazorViewToString("Statement", statement));

      MemoryStream workStream = new MemoryStream();

      byte[] byteInfo = rr.Bytes;
      workStream.Write(byteInfo, 0, byteInfo.Length);


      workStream.Position = 0;

      HttpContext.Response.AddHeader("content-disposition", string.Format("attachment; filename=Statement-{0}-{1}-{2}.pdf", statement.AccountNo, statement.StartDate, statement.StatementDate));

      return new FileStreamResult(workStream, "application/pdf"); 
    }

    public ActionResult SettlementContract(long? id)
    {

      if (id == null)
        throw new NullReferenceException("ApplicationId is null");

      var application = Application.GetFirstBy(Services.XpoUnitOfWork, x => x.Client.UserId == WebSecurity.CurrentUserId && x.ApplicationId == id);

      if (application == null)
        return new HttpUnauthorizedResult();

      SettlementContractModel settlement = null;

      using (var result = new WebServiceClient("WebServer.NET"))
      {
        var r = result.MYC_GetSettlementQuotation((long)id);
        settlement = new SettlementContractModel()
        {
          AccountNo = r.AccountNo,
          Amount = r.Amount,
          ExpirationDate = r.ExpirationDate,
          Fees = r.Fees,
          FirstName = r.FirstName,
          IdNumber = r.IdNumber,
          Interest = r.Interest,
          LastName = r.LastName,
          ResidentialAddress = r.ResidentialAddress,
          SettlementDate = r.SettlementDate,
          SettlementId = r.SettlementId,
          TotalAmount = r.TotalAmount,
          ValidDaysLeft = r.ValidDaysLeft,
          ValidFrom = r.ValidFrom
        };
      }

      PDFServer.PDFServerClient pdf = new PDFServer.PDFServerClient("PDFServer.NET");

      var rr = pdf.GetPdf(RenderRazorViewToString("SettlementContract", settlement));

      MemoryStream workStream = new MemoryStream();

      byte[] byteInfo = rr.Bytes;
      workStream.Write(byteInfo, 0, byteInfo.Length);


      workStream.Position = 0;

      HttpContext.Response.AddHeader("content-disposition", string.Format("attachment; filename=Settlement-{0}.pdf", settlement.AccountNo));

      return new FileStreamResult(workStream, "application/pdf"); 
    }
  }
}
