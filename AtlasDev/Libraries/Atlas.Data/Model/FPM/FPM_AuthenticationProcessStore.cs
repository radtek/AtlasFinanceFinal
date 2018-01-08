
namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_AuthenticationProcessStore : XPLiteObject
  {
    private Int64 _authenticationProcessStoreId;
    [Key(AutoGenerate = true)]
    public Int64 AuthenticationProcessStoreId
    {
      get
      {
        return _authenticationProcessStoreId;
      }
      set
      {
        SetPropertyValue("AuthenticationProcessStoreId", ref _authenticationProcessStoreId, value);
      }
    }

    private FPM_Authentication _authentication;
    [Persistent("AuthenticationId")]
    [Indexed]
    public FPM_Authentication Authentication
    {
      get
      {
        return _authentication;
      }
      set
      {
        SetPropertyValue("Authentication", ref _authentication, value);
      }
    }

    private Byte[] _processDocument;
    [Persistent("ProcessDocument")]
    public Byte[] ProcessDocument
    {
      get
      {
        return _processDocument;
      }
      set
      {
        SetPropertyValue("ProcessDocument", ref _processDocument, value);
      }
    }

    #region Constructors

    public FPM_AuthenticationProcessStore() : base() { }
    public FPM_AuthenticationProcessStore(Session session) : base(session) { }

    #endregion
  }
}
