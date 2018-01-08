using Atlas.Enumerators;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Helpers;
using Atlas.Online.Web.Models.Dto;
using Atlas.Online.Web.Models.Steps.Interfaces;
using Atlas.Online.Web.Resources;
using Atlas.Online.Web.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Models.Steps
{
  public class ConfirmVerifyStep : ApplicationStepBase, IBankDetailsDtoWrapper
  {
    public override int Id
    {
      get { return 4; }
    }

    public BankDetailDto BankDetail { get; set; } 
    public LoanDto Loan { get; set; }

    public override void AfterCommit(Application application)
    {
      // Submit application
      using (var service = new SharedServices())
      {
        service.WebServiceClient.APP_Submit(application.Client.ClientId);
      }
    }

    public override void Populate(Application application)
    {
      this.BankDetail = BankDetailDto.Create(application, x => x.IsEnabled);
      this.Loan = new LoanDto(application);
    }    

    public override void Save(ref Application application, HttpRequestBase request)
    {
      // Do nothing, this has no input fields
    }
  }
}