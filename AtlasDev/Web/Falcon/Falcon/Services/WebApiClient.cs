using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;


namespace Falcon.Services
{
  public class WebApiClient
  {
    public class CheckLinkModel
    {
      public Guid UserId { get; set; }
    }

    public class UserLinkModel
    {
      public string IDNo { get; set; }
      public Guid UserId { get; set; }
    }

    public class UserLinkV2Model
    {
      public long PersonId { get; set; }
      public Guid UserId { get; set; }
    }


    public static async Task<JObject> CheckIfUserLinked(Guid userId)
    {
      using (var client = InitializeClient())
      {
        var model = new CheckLinkModel() { UserId = userId };

        var response = await client.PostAsJsonAsync<CheckLinkModel>("api/User/CheckLink/", model);
        if (response.IsSuccessStatusCode)
        {
          var result = await response.Content.ReadAsStringAsync();
          return result != "null" ? JObject.Parse(result) : null;
        }
      }
      return null;
    }


    public static async Task<bool> LinkUser(string idNo, Guid userId)
    {
      using (var client = InitializeClient())
      {
        var model = new UserLinkModel() { UserId = userId, IDNo = idNo };
        var response = await client.PostAsJsonAsync<UserLinkModel>("api/User/LinkUser/", model);
        return response.IsSuccessStatusCode ? true : false;
      }
    }


    public static async Task<bool> LinkUser(long personId, Guid userId)
    {
      using (var client = InitializeClient())
      {
        var model = new UserLinkV2Model() { PersonId = personId, UserId = userId };
        var response = await client.PostAsJsonAsync<UserLinkV2Model>("api/User/LinkUserv2/", model);
        return response.IsSuccessStatusCode ? true : false;
      }
    }


    public static async Task<JObject> GetUserDetails(Guid userId)
    {
      using (var client = InitializeClient())
      {
        var model = new CheckLinkModel() { UserId = userId };
        var response = await client.PostAsJsonAsync<CheckLinkModel>("api/User/GetUserDetail/", model);
        if (response.IsSuccessStatusCode)
        {
          var result = await response.Content.ReadAsStringAsync();
          return JObject.Parse(result);
        }
      }
      return null;
    }


    internal static HttpClient InitializeClient()
    {
      var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["apiBase"]) };
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      return client;
    }

  }
}