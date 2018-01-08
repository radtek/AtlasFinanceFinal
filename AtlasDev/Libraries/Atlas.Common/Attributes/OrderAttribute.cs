using System;

namespace Atlas.Common.Attributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  public class OrderAttribute : Attribute
  {
    public int Order { get; set; }

    public OrderAttribute(int order)
    {
      Order = order;
    }
  }
}
