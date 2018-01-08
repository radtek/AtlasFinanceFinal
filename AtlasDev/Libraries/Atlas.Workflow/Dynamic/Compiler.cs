using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.Dynamic
{
  public static class Compiler
  {
    public static bool GetExpressionResult(string expression)
    {
      var loCompiler = CodeDomProvider.CreateProvider("CSharp");
      CompilerParameters loParameters = new CompilerParameters();

      // *** Start by adding any referenced assemblies
      loParameters.ReferencedAssemblies.Add("System.dll");

      string lcCode = @"using System; 
        namespace ExpressYourSelf
        {
          public class Expression
          {
            public bool GetResult(string Expression)
            {
              return " + expression + ";} } } ";

      // *** Load the resulting assembly into memory
      loParameters.GenerateInMemory = true;

      // *** Now compile the whole thing
      CompilerResults loCompiled =
              loCompiler.CompileAssemblyFromSource(loParameters, lcCode);

      if (loCompiled.Errors.HasErrors)
      {
        string lcErrorMsg = "";

        lcErrorMsg = loCompiled.Errors.Count.ToString() + " Errors:";
        for (int x = 0; x < loCompiled.Errors.Count; x++)
          lcErrorMsg = lcErrorMsg + "\r\nLine: " +
                       loCompiled.Errors[x].Line.ToString() + " - " +
                       loCompiled.Errors[x].ErrorText;

        throw new Exception("Dynamic Compiler: " + lcErrorMsg + "\r\n\r\n" + lcCode);
      }

      Assembly loAssembly = loCompiled.CompiledAssembly;

      // *** Retrieve an obj ref – generic type only
      object loObject = loAssembly.CreateInstance("ExpressYourSelf.Expression");
      if (loObject == null)
      {
        throw new Exception("Couldn't load class.");
      }

      object[] loCodeParms = new object[1];

      try
      {
        object loResult = loObject.GetType().InvokeMember(
                         "GetResult", BindingFlags.InvokeMethod,
                         null, loObject, loCodeParms);

        return (bool)loResult;
      }
      catch (Exception loError)
      {
        throw new Exception("Dynamic Compiler: " + loError.Message);
      }
    }
  }
}
