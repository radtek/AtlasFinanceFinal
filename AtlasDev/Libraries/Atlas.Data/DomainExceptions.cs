
namespace Atlas.Domain
{
  #region Using
  using System;
  using DevExpress.Xpo;
  #endregion

  
  public class RequiredPropertyValueMissing : Exception
  {
    public RequiredPropertyValueMissing(XPBaseObject theObject, string propertyName) :
      base(String.Format("The '{0}' property for type '{1}' must have a value",
           propertyName, theObject.GetType().Name))
    {
    }
  }
}
