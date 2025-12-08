using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Avarez.Controllers.CarTax
{
    public class SearchColorController : Controller
    {
        //
        // GET: /SearchColor/

        public ActionResult Index()
        {
            Session["searchtext"] = "%%";
            Session["top"] = 30;
            return PartialView();
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_ColorCarSelect("fldColor", Session["searchtext"].ToString(), Convert.ToInt32(Session["top"]), 1, "").ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Reload(string value, int top)
        {//جستجو
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[0], value);
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            Session["searchtext"] = searchtext;
            Session["top"] = top;
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
