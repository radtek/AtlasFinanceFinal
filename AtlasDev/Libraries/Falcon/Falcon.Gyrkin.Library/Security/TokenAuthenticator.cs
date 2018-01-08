using RestSharp;
using System.Net.Http.Headers;
using System.Security.Claims;
using RestSharp.Authenticators;

namespace Falcon.Gyrkin.Library.Security
{
  public class TokenAuthenticator : IAuthenticator
  {
    public void Authenticate(IRestClient client, IRestRequest request)
    {
      var token = ClaimsPrincipal.Current.GetTokenString();
      if (!string.IsNullOrEmpty(token))
      {
        var header = new AuthenticationHeaderValue("Bearer", token);
        request.AddHeader("Authorization", header.ToString());
      }
    }
  }
}
