using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.Guest
{
    public class SearchFileController : Controller
    {
        //
        // GET: /SearchFile/

        public ActionResult Index()
        {
            if (Session["UserState"] == null)
                return RedirectToAction("logon", "Account");
            return PartialView();
        }

        public ActionResult Reload(int field, string value1, string value2)
        {//جستجو
            string[] _fiald = new string[] { "fldVIN", "fldShasiAndMotorNumber" };
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (field == 0)
                value2 = "";
            var q = m.sp_CarUserGuestSelect(_fiald[field], value1, value2, 30).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
    }
}
