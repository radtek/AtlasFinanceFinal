using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_EscalationTemplate:XPLiteObject
  {
    private int _escalationTemplateId;
    [Key(AutoGenerate = true)]
    public int EscalationTemplateId
    {
      get
      {
        return _escalationTemplateId;
      }
      set
      {
        SetPropertyValue("EscalationTemplateId", ref _escalationTemplateId, value);
      }
    }

    private bool _isHTML;
    [Persistent]
    public bool IsHTML
    {
      get
      {

        return _isHTML;
      }
      set
      {
        SetPropertyValue("IsHTML", ref _isHTML, value);
      }
    }

    private string _subject;
    [Persistent]
    public string Subject
    {
      get
      {
        return _subject;
      }
      set
      {
        SetPropertyValue("Subject", ref _subject, value);
      }
    }

    private string _body;
    [Persistent]
    public string Body
    {
      get
      {

        return _body;
      }
      set
      {
        SetPropertyValue("Body", ref _body, value);
      }
    }

    #region Constructors

    public WFL_EscalationTemplate() : base() { }
    public WFL_EscalationTemplate(Session session) : base(session) { }

    #endregion
  }
}
