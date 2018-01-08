using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.Common
{
  public static class Step
  {
    public static dynamic CreateDynamicStepInstance(string assemblyName, string @namespace, object[] constructorParamters)
    {
      Assembly assembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "\\" + assemblyName);
      Type type = assembly.GetType(@namespace);
      return Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.Instance,
                                                   null, constructorParamters, null);
    }
  }
}
