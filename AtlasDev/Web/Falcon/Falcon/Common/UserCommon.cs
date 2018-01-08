using System.Web;
using Falcon.Gyrkin.Library.Common;
using Falcon.Gyrkin.Library.Service;
using Newtonsoft.Json;

namespace Falcon.Common
{
  public sealed class UserCommon
  {
    private Person _data = null;

    public UserCommon()
    {
      var personCookie = CookieHelper.SecureGetCookieValue(HttpContext.Current, CookieConst.FALCON_PERSON_DATA);
      if (!string.IsNullOrEmpty(personCookie))
        _data = JsonConvert.DeserializeObject<Person>(personCookie);
    }
    //public PersonData Get()
    //{
    //  return _data;
    //}

    public long GetPersonId()
    {
      if (_data == null)
        return 0;
      return _data.PersonId;
    }

    public long? GetBranchId()
    {
      if (_data == null)
        return 0;
      return 0;
      //return _data.BranchId;
    }
  }
}