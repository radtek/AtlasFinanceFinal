using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Atlas.Common.Extensions;
using Atlas.Enumerators;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Service;


namespace Falcon.Controllers.Api
{
  public sealed class GeneralController : AppApiController
  {
    public GeneralController()
    {

    }


    [HttpGet]
    [WebApiAntiForgeryToken]
    [Authorize]
    public async Task<HttpResponseMessage> GetAddressTypes(int getAddressTypes)
    {
      IEnumerable<dynamic> addressTypes = null;
      try
      {
        await Task.Run(() =>
        {
          addressTypes = EnumUtil.GetValues<General.AddressType>().Where(a => a != General.AddressType.NotSet).Select(a => new { AddressTypeId = (int)a, Description = a.ToStringEnum() });
        });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to communicate with services.");
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { addressTypes });
    }


    [HttpGet]
    [WebApiAntiForgeryToken]
    [Authorize]
    public async Task<HttpResponseMessage> GetContactTypes(int getContactTypes)
    {
      IEnumerable<dynamic> contactTypes = null;
      try
      {
        await Task.Run(() =>
        {
          contactTypes = EnumUtil.GetValues<General.ContactType>().Where(a => a != General.ContactType.NotSet).Select(a => new { ContactTypeId = (int)a, Description = a.ToStringEnum() });
        });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to communicate with services.");
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { contactTypes });
    }


    [HttpGet]
    [WebApiAntiForgeryToken]
    [Authorize]
    public async Task<HttpResponseMessage> GetProvinces(int getProvinces)
    {
      IEnumerable<dynamic> provinces = null;
      try
      {
        await Task.Run(() =>
        {
          provinces = EnumUtil.GetValues<General.Province>().Where(a => a != General.Province.NotSet).Select(a => new { ProvinceId = (int)a, Description = a.ToStringEnum() });
        });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to communicate with services.");
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { provinces });
    }


    [HttpGet]
    [WebApiAntiForgeryToken]
    [Authorize]
    public async Task<HttpResponseMessage> GetPublicHolidaysFromToday(int getPublicHolidaysFromToday)
    {
      List<PublicHoliday> publicHolidays = null;
      try
      {
        publicHolidays = await Services.WebServiceClient.PublicHolidays_GetAsync(DateTime.Today);
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to communicate with services.");
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { publicHolidays });
    }
  }
}