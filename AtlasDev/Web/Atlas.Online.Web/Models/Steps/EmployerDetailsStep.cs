using Atlas.Enumerators;
using Atlas.Online.Data;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Helpers;
using Atlas.Online.Web.Models.Dto;
using Atlas.Online.Web.Models.Steps.Interfaces;
using Atlas.Online.Web.Validations;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Atlas.Online.Web.Models.Steps
{
  public class EmployerDetailsStep : ApplicationStepBase, IEmployerDetailsDtoWrapper
  {
    public override int Id
    {
      get { return 2; }
    }

    [Required]
    public EmployerDetailsDto EmployerDetails { get; set; }
   
    [Required]
    [Display(Name = "Income Frequency")]
    public General.SalaryFrequency SalaryFrequency { get; set; }
    
    [Range(1, 31)]
    [Display(Name = "Salary pay day")]
    public int? SalaryPayDayNumber { get; set; }
    [Display(Name = "Salary pay day")]
    public General.Days? SalaryPayDay { get; set; }
   
    public override void Populate(Application application)
    {
      #region Employer
      if (application.Employer != null)
      {
        this.EmployerDetails = EmployerDetailsDto.Create(application);   
      }
      #endregion            

      #region Salary
      if (application.SalaryType != null)
      {
        this.SalaryFrequency = application.SalaryType.Type;
        this.SalaryPayDayNumber = application.SalaryTypeNo;
        this.SalaryPayDay = (General.Days)(application.SalaryTypeNo <= 6 ? application.SalaryTypeNo : 0);
      }
      #endregion
    }

    public override void Save(ref Application application, HttpRequestBase request)
    {
      var session = (UnitOfWork)application.Session;      

      #region Employer
      this.EmployerDetails.SaveApplication(ref application);
            #endregion

            #region Salary

            if (application.SalaryType == null)
            {
                //Edited By Prashant
                application.SalaryType = new XPQuery<SalaryType>(session).FirstOrDefault(x => x.AddressTypeId == (int)this.SalaryFrequency); //new XPQuery<SalaryType>(session).FirstOrDefault(x => x.Type == this.SalaryFrequency);
            }

      application.SalaryType.Type = this.SalaryFrequency;
      switch (this.SalaryFrequency)
      {
        case General.SalaryFrequency.Monthly:
          application.SalaryTypeNo = this.SalaryPayDayNumber.HasValue ? this.SalaryPayDayNumber.Value : 0;
          break;
        case General.SalaryFrequency.Fortnightly:
        case General.SalaryFrequency.Weekly:
          application.SalaryTypeNo = Convert.ToInt32(this.SalaryPayDay);
          break;
      }
      
      #endregion

      application.Save();
    }
  }
}