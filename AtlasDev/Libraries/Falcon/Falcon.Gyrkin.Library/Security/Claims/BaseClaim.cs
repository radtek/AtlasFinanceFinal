using Newtonsoft.Json;

namespace Falcon.Gyrkin.Library.Security.Claims
{
  public abstract class BaseClaim
  {
    public abstract string ValueType();

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }
  }
}
