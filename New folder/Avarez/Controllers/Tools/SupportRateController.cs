using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
namespace Avarez.Controllers.BasicInf
{
    [Authorize]
    public class SupportRateController : Controller
    {
        //
        // GET: /SupportRate/

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 368))
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
            var q = m.sp_SupportRateSelect("Type", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Taeed(int id)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_SupportRateUpdateType(id,Convert.ToInt32(Session["UserId"]));
            return Json("",JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] {"fldCabinTypeName","fldCarTypeName" ,"fldCarModelName","fldMakeName", "fldNameClass","fldYear", "Type" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_SupportRateSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HaveData()
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var Have = 0;
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 369))
            {
                var q = m.sp_SupportRateSelect("Type", "", 30).ToList();
                Have = q.Count;
            }
            return Json(new { Have = Have }, JsonRequestBehavior.AllowGet);
        }
    }
}