using System;
using System.Collections.Generic;


namespace ASSServer.Extensions
{
  public static class ListUtils
  {
    /// <summary>
    /// Move an item to a specific location within the list- if not foumd, does nothing
    /// </summary>    
    /// <param name="list">The list to update</param>
    /// <param name="value">The value to locate</param>
    /// <param name="toIndex">The required index</param>
    /// <exception cref="System.ArgumentOutOfRangeException">Argument out of range</exception>    
    public static void MoveItem<T>(this List<T> list, T value, int toIndex)
    {
      if (toIndex < 0)
      {
        throw new ArgumentOutOfRangeException("toIndex", "Cannot be less than zero");
      }

      var index = list.IndexOf(value);
      if (index > -1)
      {
        list.RemoveAt(index);
        list.Insert(toIndex < list.Count ? toIndex : list.Count - 1, value);
      }
    }
  }
}
