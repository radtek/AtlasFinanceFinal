using System.Net;
using Newtonsoft.Json.Linq;
using Ninject.Activation;
using RestSharp;
using Serilog;
using StackExchange.Redis;
using System;
using System.Configuration;
using System.Security.Authentication;

namespace Atlas.Colony.Integration.Service.Eurocom
{
  public sealed class EurocomImpl : IDisposable
  {
    #region Private

    private static readonly ILogger _log = Log.Logger;
    private static IDatabase _redisDatabase = RedisConnection.Current.GetDatabase();
    private const string COUPON_KEY = "coupon.oauth2.token";

    private static string BASE_URL = ConfigurationManager.AppSettings["euro.base.url"];
    private static string OAUTH_PATH = ConfigurationManager.AppSettings["euro.oauth.path"];
    private static string CLIENT_ID = ConfigurationManager.AppSettings["client.id"];
    private static string CLIENT_SECRET = ConfigurationManager.AppSettings["client.secret"];
    private static string CLIENT_USERNAME = ConfigurationManager.AppSettings["client.username"];
    private static string CLIENT_PASSWORD = ConfigurationManager.AppSettings["client.password"];

    #endregion

    /// <summary>
    /// Determines whether session is authenticated, if not attempts to authenticate and store token
    /// </summary>
    /// <returns>Bool</returns>
    public bool Authenticated()
    {
      var oauth2_token = _redisDatabase.StringGet(COUPON_KEY);
      if (!oauth2_token.HasValue)
        return Authenticate();
      else
        return true;

      throw new AuthenticationException("Unable to authenticate against service.");
    }

    /// <summary>
    /// Authenticate against service
    /// </summary>
    internal bool Authenticate()
    {
      var client = new RestClient(BASE_URL);
      var request = new RestRequest(OAUTH_PATH, Method.POST);

      request.AddParameter("client_id", CLIENT_ID);
      request.AddParameter("client_secret", CLIENT_SECRET);
      request.AddParameter("username", CLIENT_USERNAME);
      request.AddParameter("password", CLIENT_PASSWORD);
      request.AddParameter("grant_type", "password");

      IRestResponse restResponse = client.Execute(request);
      JObject authContent = JObject.Parse(restResponse.Content);

      if (!string.IsNullOrEmpty(authContent["access_token"].ToString()))
      {
        _redisDatabase.StringSet(COUPON_KEY, authContent["access_token"].ToString(), new TimeSpan(0, 30, 0));
        return true;
      }
      else
        return false;
    }

    /// <summary>
    /// Apply bearer token to RestRequest header
    /// </summary>
    private void ApplyBearerToken(ref IRestRequest restRequest)
    {
      var oauth2_token = _redisDatabase.StringGet(COUPON_KEY);

      if (!oauth2_token.HasValue)
      {
        Authenticate();
        ApplyBearerToken(ref restRequest);
      }
      else
      {
        restRequest.AddHeader("Authorization", string.Format("Bearer {0}", oauth2_token));
      }
    }

    /// <summary>
    /// Returns instance of base request client
    /// </summary>
    /// <returns>IRestClient object</returns>
    public IRestClient RequestClient()
    {
      return new RestClient(BASE_URL);
    }

    /// <summary>
    /// Returns instance of base rest request
    /// </summary>
    /// <param name="path">Path to rest resource</param>
    /// <param name="method">HTTP Method (get, post, ...)</param>
    /// <returns>IRestRequest object</returns>
    public IRestRequest RestRequest(string path, Method method)
    {
      return new RestRequest(path, method);
    }

    public void Dispose()
    {

    }

    /// <summary>
    /// Gets fields for injection methods
    /// </summary>
    public void GetInjectorFields()
    {
      // Gets an instance of request.
      var client = RequestClient();

      // Set rest end point
      var request = RestRequest("/construct/injectors/profiles/availablefields/", Method.GET);

      // Apply auth header
      ApplyBearerToken(ref request);

      // Perform request
      IRestResponse restResponse = client.Execute(request);
    }


    /// <summary>
    /// Submits the profile data for the campaign
    /// </summary>
    public bool InjectProfile(int campaignId, string name, string idNo, string mobileNo, string gender, string branch, string region)
    {
      // Get instance of request client
      var client = RequestClient();

      // Set rest end point
      var request = RestRequest(string.Format("/construct/injectors/profiles/{0}/", campaignId), Method.POST);

      request.AddParameter("name", name);
      request.AddParameter("id_number", idNo);
      request.AddParameter("mobile_number", mobileNo);
      request.AddParameter("gender", gender);
      request.AddParameter("branch", branch);
      request.AddParameter("region", region);

      // Apply auth header
      ApplyBearerToken(ref request);

      // Perform request
      IRestResponse restResponse = client.Execute(request);

      

      if (restResponse.StatusCode != HttpStatusCode.Created)
      {
        _log.Fatal(
          "[InjectProfile] - Failed to register profile Name: {name}, ID Number: {idnumber}, Mobile Number: {mobilenumber}, Gender: {gender}, Error:{error}",
          name, idNo, mobileNo, gender);
        return false;
      }
      _log.Information(
          "[InjectProfile] - Registered profile Name: {name}, ID Number: {idnumber}, Mobile Number: {mobilenumber}, Gender: {gender}",
          name, idNo, mobileNo, gender);
      return true;
    }

    /// <summary>
    /// Activate loyalty for the customer
    /// </summary>
    /// <param name="campaignId">Campaign that is currently being used</param>
    /// <param name="idNo">ID Number used in the inject profile method.</param>
    public bool ActivateLoyalty(int campaignId, string idNo)
    {
      // Get instance of request client
      var client = RequestClient();

      // Set rest end point
      var request = RestRequest("/collate/codes/accesscode/", Method.POST);

      request.AddParameter("code", idNo);
      request.AddParameter("campaign", campaignId);

      // Apply auth header
      ApplyBearerToken(ref request);

      IRestResponse restResponse = client.Execute(request);

      if (restResponse.StatusCode != HttpStatusCode.Created)
      {
        _log.Fatal(
          "[ActivateLoyalty] - Failed to activate loyalty Campaign Id: {campaignId}, ID Number: {idnumber}",
          campaignId, idNo);
        return false;
      }
      _log.Information(
          "[ActivateLoyalty] - Activated loyalty Campaign Id: {campaignId}, ID Number: {idnumber}",
          campaignId, idNo);
      return true;
    }

    /// <summary>
    /// Send SMS
    /// </summary>
    public bool SendSMS(int? campaignId, string mobileNo, string message)
    {
      // Get instance of request client
      var client = RequestClient();

      // Set rest end point
      var request = RestRequest("/contact/dispatchers/send-message/", Method.POST);

      if (campaignId != null)
        request.AddParameter("campaign", campaignId);

      request.AddParameter("message", message);
      request.AddParameter("destination", mobileNo);

      // Apply auth header
      ApplyBearerToken(ref request);

      IRestResponse restResponse = client.Execute(request);

      JObject obj = JObject.Parse(restResponse.Content);
      if (restResponse.StatusCode != HttpStatusCode.Created)
      {

        _log.Fatal(
          "[SendSMS] - Failed to send message Campaign Id: {campaignId}, Mobile No: {mobileNo}, Message: {message}, Error: {error}",
          campaignId, mobileNo, message,obj["errors"].ToString());
        return false;
      }

      

      _log.Information(
        "[SendSMS] - Message delivered Campaign Id: {campaignId}, Mobile No: {mobileNo}, Message: {message}, Success: {status}",
        campaignId, mobileNo, message, obj["success"].ToString());

      return true;
    }
  }
}
