using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Atlas.Domain.Serializable.Product
{
  public static class Helper
  {
    public static Type GetType(string assembly)
    {
      return Assembly.GetExecutingAssembly().GetType(assembly);
    }
  }
}
