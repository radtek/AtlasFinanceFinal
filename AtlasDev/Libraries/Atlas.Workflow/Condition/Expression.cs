using Atlas.Common.Utils;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo.Metadata;

namespace Atlas.Workflow.Condition
{
  public static class Expression
  {
    /* The Actual Stored Expression should look like the below
     * ((PV[1] >= CV[1]) && (PV[2] == CV[2])) || (PV[5] == CV[5])
     * where PV[n] is the actual record paramater and CV[n] is the conditional value 
     * n represents the ConditionId
     */

    private enum Parameter
    {
      [Description("PV[{0}]")]
      PropertyValue = 1,
      [Description("CV[{0}]")]
      ConditionValue = 2
    }

    public static string BuildExpression(WFL_ConditionGroupDTO conditionGroup, WFL_ProcessJobDTO processJob)
    {
      var expression = conditionGroup.Expression;
      using (var uow = new UnitOfWork())
      {
        var conditions = new XPQuery<WFL_Condition>(uow).Where(c => c.ConditionGroup.ConditionGroupId == conditionGroup.ConditionGroupId);

        foreach (var condition in conditions)
        {
          var classInfo = uow.GetClassInfo(condition.ConditionClassProperty.ConditionPrimaryKey.ConditionClass.Assembly, condition.ConditionClassProperty.ConditionPrimaryKey.ConditionClass.Namespace);

          var processDataExt = new XPQuery<WFL_ProcessDataExt>(uow).FirstOrDefault(p => p.ProcessJob.ProcessJobId == processJob.ProcessJobId
            && p.DataExtType.Type == condition.ConditionClassProperty.ConditionPrimaryKey.PrimaryKeyDataExtType.Type);

          var assemblyObject = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "\\" + processDataExt.DataExtType.Assembly);

          var data = (Xml.DeSerialize(assemblyObject.GetType(processDataExt.DataExtType.Namespace), processDataExt.Data));

          var primaryKeyValue = GetValue(data, condition.ConditionClassProperty.ConditionPrimaryKey.PrimaryKeyProcessDataProperty);

          var resultData = uow.GetObjectByKey(classInfo, primaryKeyValue);

          var propertyValue = GetValue(resultData, condition.ConditionClassProperty.Property, uow);

          UpdateExpression(condition.ConditionId, propertyValue, condition.ConditionValue, ref expression);
        }
      }

      return expression;
    }

    private static void UpdateExpression(object id, object propertyValue, string conditionValue, ref string expression)
    {
      var propertyValueString = propertyValue.ToString();
      if (propertyValue.GetType() == typeof(string))
      {
        propertyValueString = "\"" + propertyValue.ToString() + "\"";
        conditionValue = "\"" + conditionValue + "\"";
      }
      else if (propertyValue.GetType() == typeof(char))
      {
        propertyValueString = "'" + propertyValue.ToString() + "'";
        conditionValue = "'" + conditionValue + "'";
      }

      expression = expression.Replace(string.Format(Parameter.ConditionValue.ToStringEnum(), id), conditionValue);
      expression = expression.Replace(string.Format(Parameter.PropertyValue.ToStringEnum(), id), propertyValueString);
    }

    private static object GetValue(object data, string propertyName, UnitOfWork uow = null)
    {
      foreach (var property in data.GetType().GetProperties())
      {
        var attributes = property.GetCustomAttributes(typeof(PersistentAttribute));
        if (attributes.Count() > 0)
        {
          var keyAttribute = (PersistentAttribute)attributes.FirstOrDefault();
          if (!string.IsNullOrEmpty(keyAttribute.MapTo))
          {
            if (string.Compare(keyAttribute.MapTo, propertyName) == 0)
            {
              if (property.PropertyType.BaseType == typeof(Enum))
              {
                return (int)property.GetValue(data);
              }
              else if (property.PropertyType.BaseType == typeof(XPObject)
                || property.PropertyType.BaseType == typeof(XPCustomObject)
                || property.PropertyType.BaseType == typeof(XPLiteObject)
                || property.PropertyType.BaseType == typeof(XPBaseObject))
              {
                if (uow != null)
                {
                  var obj = property.GetValue(data);
                  return uow.GetKeyValue(obj);
                }
              }

              return property.GetValue(data);
            }
          }
        }
        if (string.Compare(property.Name, propertyName) == 0)
        {
          return property.GetValue(data);
        }
      }

      return null;
    }
  }
}
