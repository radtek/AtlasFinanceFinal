using Newtonsoft.Json;
using System.Security.Claims;

namespace Falcon.Gyrkin.Library.Security.Claims
{
  public class ComplexClaim<T> : Claim where T : BaseClaim
  {
    public ComplexClaim(string claimType, T claimValue)
      : this(claimType, claimValue, string.Empty)
    {
    }

    public ComplexClaim(string claimType, T claimValue, string issuer)
      : this(claimType, claimValue, issuer, string.Empty)
    {
    }

    public ComplexClaim(string claimType, T claimValue, string issuer, string originalIssuer)
      : base(claimType, claimValue.ToString(), claimValue.ValueType(), issuer, originalIssuer)
    {
    }

    public new T Value
    {
      get { return JsonConvert.DeserializeObject<T>(base.Value); }
    }
  }
}
