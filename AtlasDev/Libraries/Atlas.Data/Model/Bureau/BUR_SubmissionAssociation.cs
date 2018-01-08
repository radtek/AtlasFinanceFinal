namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public class BUR_SubmissionAssociation : XPLiteObject
  {
    public struct SubmissionAssociationKey
    {
      [Persistent("BranchId")]
      public BRN_Branch Branch;

      [Persistent("UniqueRefNo")]
      public string UniqueRefNo;

      [Persistent("SeqNo"), Size(20)]
      public string SeqNo;
    }

    [Key, Persistent]
    public SubmissionAssociationKey SubmissionAssociation;

    #region Constructors

    public BUR_SubmissionAssociation() : base() { }
    public BUR_SubmissionAssociation(Session session) : base(session) { }

    #endregion
  }
}
