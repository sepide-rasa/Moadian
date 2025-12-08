using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
namespace Avarez.Controllers.Tools
{
    public class ErrorController : Controller
    {

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 280))
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
            var q = m.sp_ErrorProgramSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);

        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldId", "fldUserName","fldDate" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_ErrorProgramSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

    }
}
