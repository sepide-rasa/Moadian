using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.CarTax
{
    public class ChCarFilePelaquSearchController : Controller
    {
        //
        // GET: /ChCarFilePelaquSearch/

        public ActionResult Index(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 300))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعویض مالک");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                ViewBag.CarId = id;
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }            
        }

        public ActionResult Reload(int field, string value1)
        {//جستجو
            string[] _fiald = new string[] { "fldOwnerMelli_EconomicCode", "fldOwnerName", "fldPlaqueNumber" };
            Models.cartaxEntities m = new Models.cartaxEntities();

            var q = m.sp_CarPlaqueSelect(_fiald[field], value1, 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
    }
}
