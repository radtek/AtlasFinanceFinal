using System.Web.Mvc;

namespace Falcon.Areas.Workflow.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Workflow/Dashboard/
        public ActionResult Index()
        {
          ViewBag.PersonId = 23120; //Atlas online
            return View();
        }

        //
        // GET: /Workflow/Dashboard/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Workflow/Dashboard/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Workflow/Dashboard/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Workflow/Dashboard/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Workflow/Dashboard/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Workflow/Dashboard/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Workflow/Dashboard/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
