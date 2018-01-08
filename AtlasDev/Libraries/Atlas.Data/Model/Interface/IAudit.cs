// -----------------------------------------------------------------------
// <copyright file="IAudit.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Atlas.Domain.Interface
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using Atlas.Domain.Model;
  using Atlas.Domain.Security;

  public interface IAudit
  {
    PER_Person CreatedBy { get; set; }
    PER_Person DeletedBy { get; set; }
    PER_Person LastEditedBy { get; set; }
    DateTime? CreatedDT { get; set; }
    DateTime? DeletedDT { get; set; }
    DateTime? LastEditedDT { get; set; }
  }
}
