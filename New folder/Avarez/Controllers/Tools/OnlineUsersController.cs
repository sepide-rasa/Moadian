using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.Tools
{
    public class OnlineUsersController : Controller
    {
        //
        // GET: /OnlineUsers/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 373))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            string mun = Session["UserMnu"].ToString();
            var q = Models.OnlineUser.userObj.Where(k => k.cboMnu == mun).ToList().ToDataSourceResult(request);
            return Json(q);

        }
        public ActionResult Reload()
        {//جستجو
            Models.cartaxEntities m = new Models.cartaxEntities();
            string mun = Session["UserMnu"].ToString();
            var q = Models.OnlineUser.userObj.Where(k => k.cboMnu == mun).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
    }
}
