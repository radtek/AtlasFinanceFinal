using System;
using System.Collections.Generic;


namespace Atlas.Common.Extensions
{
  public static class Collections
  {
    /// <summary>
    /// Move an item to a specific location within the list- 
    /// if item is not found, does nothing. 
    /// If toIndex is not a valid index, item is appended at the end.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list to operate on</param>
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
