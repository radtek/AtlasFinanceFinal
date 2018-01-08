using System;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Falcon.Gyrkin.Library.Security
{
  /// <summary>
  /// Currently used by the Web Services.Checks the authorization header from httprequest and set the current user 
  /// </summary>
  public class TokenValidationHandler : DelegatingHandler
  {
    /// <summary>
    /// Tries to retrieve token from requst header.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
    {
      token = null;
      var headers = request.Headers;
      if (headers.Authorization != null && headers.Authorization.Scheme.Equals("Bearer"))
      {
        token = headers.Authorization.Parameter;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>
    /// Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.
    /// </returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      string token;

      if (!TryRetrieveToken(request, out token))
      {
        //return base.SendAsync(request, cancellationToken);
        throw new HttpException(Convert.ToInt32(HttpStatusCode.Unauthorized), "Unable to locate bearer token");
      }
      else
      {
        if (string.IsNullOrEmpty(token)) //if token is blank,ignore for now
        {
          return base.SendAsync(request, cancellationToken);
        }
        try
        {
          var tokenHandlers =
            FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
          JwtSecurityToken jwt = tokenHandlers.ReadToken(token) as JwtSecurityToken;
          var identities = tokenHandlers.ValidateToken(jwt);
          var cp = new ClaimsPrincipal(identities);
          Thread.CurrentPrincipal = cp;
          HttpContext.Current.User = cp; //yes both are required.http://aspnetwebstack.codeplex.com/workitem/264

        }
        catch (SecurityTokenValidationException ex)
        {
          return Task.FromResult(request.CreateErrorResponse(HttpStatusCode.Unauthorized, ex));
        }
        catch (Exception ex)
        {
          return Task.FromResult(request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex));
        }
      }
      return base.SendAsync(request, cancellationToken);
    }
  }
}