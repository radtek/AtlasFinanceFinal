
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Atlas.Domain.Interface;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public sealed class PER_OTP : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public Int64 OTPId { get; set; }

    [Persistent("PersonId")]
    public PER_Person Person { get; set; }

    public int OTP { get; set; }
    public string Key { get; set; }

    public DateTime CreateDate { get; set; }

    #region Constructors

    public PER_OTP() : base() { }
    public PER_OTP(Session session)
      : base(session)
    {

    }

    #endregion
  }
}
