using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Atlas.Online.Web.Models.Steps
{
  public abstract class ApplicationStepBase
  {
    public abstract int Id { get; }

    public int ApplicationId { get; set; }

    public bool IsDirty { get; set; }

    public abstract void Populate(Application application);
    public abstract void Save(ref Application application, HttpRequestBase request);

    public virtual void AfterCommit(Application application)
    {
      // Optionally override this      
    }

    public virtual bool IsValid()
    {
      return Validation.IsValidObject(this);
    }
  }
}
