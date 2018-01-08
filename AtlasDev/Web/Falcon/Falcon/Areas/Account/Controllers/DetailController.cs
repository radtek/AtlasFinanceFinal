using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Falcon.Base;
using Falcon.Common;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Security.Role;

namespace Falcon.Areas.Account.Controllers
{
  [ClaimsAuthorize(ClaimTypes.Role, new string[] { RoleType.ADMINISTRATOR})]
  public sealed class DetailController : AppController
  {

    public DetailController() : base()
    {

    }
    public ActionResult Index()
    
    {
      return View();
    }

    public ActionResult View(int id)
    {
      ViewBag.id = id;
      ViewBag.personId = new UserCommon().GetPersonId();
      return View("Index");
    }

    public ContentResult UploadDocument(HttpPostedFileBase file)
    {

        //string path = Server.MapPath(string.Format("~/Static/{0}", "xxx.png"));
        //Stream inputStream = Request.InputStream;

        //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);

        //inputStream.CopyTo(fileStream);
        //fileStream.Close();

      //System.IO.Directory.CreateDirectory(Server.MapPath(string.Format(" ~/App_Data/{0}", _BRN_CODE)));
      //var filename = Path.GetFileName(file.FileName);
      //var path = Path.Combine(Server.MapPath(string.Format("~/App_Data/{0}", _BRN_CODE)), Guid.NewGuid().ToString());
      //file.SaveAs(path);
      //return new ContentResult
      //{
      //  ContentType = "text/plain",
      //  Content = Base64.Base64Encode(path),
      //  ContentEncoding = Encoding.UTF8
      //};
      return null;
    }


    //[Authorize]
    //public ActionResult Avs()
    //{
    //  return View();
    //}
  }
}