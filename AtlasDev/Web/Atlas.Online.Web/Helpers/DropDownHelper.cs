using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Web.Helpers
{
  public static class DropDownHelper
  {
    public static SelectListItem[] FromEnum(Type enumType, object value = null)
    {
      var names = Enum.GetNames(enumType);
      var values = Enum.GetValues(enumType);

      if (!String.IsNullOrEmpty(value as string))
      {
        object result;
        if (value.ToString().TryToEnum(enumType, out result))
        {
          value = result;
        }
      }
     
      SelectListItem[] items = new SelectListItem[names.Length];
      for (int i = 0; i < names.Length; i++)
      {
        var v = values.GetValue(i);
        var intValue = Convert.ToInt32(v);
        var name = names[i];
        bool isNotSet = (name == "NotSet");        

        items[i] = new SelectListItem()
        {
          Selected = (value != null && v.Equals(value)),
          Text = !isNotSet ? GetEnumText(enumType, name) : "Make Selection...",
          Value = !isNotSet ? intValue.ToString() : String.Empty
        };
      }

      return items;
    }    

    public static IEnumerable<SelectListItem> FromEnum<T>(object value = null)
    {
      return FromEnum(typeof(T), value);
    }

    #region Private Methods

    private static string GetEnumText(Type type, string name)
    {
      string text = EnumStringExtension.GetDescription(type, name);
      if (String.IsNullOrEmpty(text))
      {
        return HumanifyName(name);
      }

      return text;
    }    

    private static string HumanifyName(string name)
    {
      return Regex.Replace(
        Regex.Replace(
          name.Replace('_', ' '),
          @"(\P{Ll})(\P{Ll}\p{Ll})",
          "$1 $2"
        ),
        @"(\p{Ll})(\P{Ll})",
        "$1 $2"
      );
    }

    #endregion
  }
}