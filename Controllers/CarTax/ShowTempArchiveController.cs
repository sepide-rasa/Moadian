using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.CarTax
{
    public class ShowTempArchiveController : Controller
    {
        //
        // GET: /ShowTempArchive/

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 316))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->مشاهده بایگانی دیجیتال");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult Reload(string value1, string value2)
        {//
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_AllDigitalArchives(MyLib.Shamsi.Shamsi2miladiDateTime(value1), MyLib.Shamsi.Shamsi2miladiDateTime(value2)).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
    }
}
