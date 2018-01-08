using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Atlas.Online.Web.Extensions.Flash
{
  public class FlashStorage
  {
    public static readonly string Key = typeof(FlashStorage).FullName;

    private IEnumerable<KeyValuePair<string, string>> _messages = null;

    public FlashStorage(TempDataDictionary backingStore)
    {
      if (backingStore == null)
      {
        throw new ArgumentNullException("backingStore");
      }

      BackingStore = backingStore;
    }

    public TempDataDictionary BackingStore { get; private set; }

    public IEnumerable<KeyValuePair<string, string>> Messages
    {
      get
      {
        try
        {
          if (_messages != null)
          {
            return _messages;
          }

          object value;

          if (!BackingStore.TryGetValue(Key, out value))
          {
            return new List<KeyValuePair<string, string>>();
          }

          return _messages = (IEnumerable<KeyValuePair<string, string>>)value;
        }
        finally
        {
          BackingStore.Remove(Key);
        }
      }
    }

    public void Add(string type, string message)
    {
      if (string.IsNullOrWhiteSpace(message))
      {
        return;
      }

      IList<KeyValuePair<string, string>> messages;
      object temp;

      if (!BackingStore.TryGetValue(Key, out temp))
      {
        messages = new List<KeyValuePair<string, string>>();
        BackingStore.Add(Key, messages);
      }
      else
      {
        messages = (IList<KeyValuePair<string, string>>)temp;
      }

      var item = messages.SingleOrDefault(p => p.Key.Equals(type, StringComparison.OrdinalIgnoreCase));

      if (!string.IsNullOrWhiteSpace(item.Value))
      {
        messages.Remove(item);
      }

      messages.Add(new KeyValuePair<string, string>(type, message));

      BackingStore.Keep(Key);
    }

  }
}