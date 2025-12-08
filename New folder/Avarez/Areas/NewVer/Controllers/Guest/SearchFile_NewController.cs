using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace Avarez.Areas.NewVer.Controllers.Guest
{
    public class SearchFile_NewController : Controller
    {
        //
        // GET: /NewVer/SearchFile_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserState"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult Search(int SearchField, string Value1, string Value2, int SearchType)
        {//جستجو
            string[] _fiald = new string[] { "fldVIN", "fldShasiAndMotorNumber" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[SearchType], Value1);
            string searchtext2 = string.Format(searchType[SearchType], Value2);
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (SearchField == 0)
                Value2 = "";
            var q = m.sp_CarUserGuestSelect(_fiald[SearchField], searchtext, searchtext2, 30).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
    }
}
