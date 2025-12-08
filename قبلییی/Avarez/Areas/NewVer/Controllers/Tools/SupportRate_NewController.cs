using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;
using System.Collections;

namespace Avarez.Areas.NewVer.Controllers.Tools
{
    public class SupportRate_NewController : Controller
    {
        //
        // GET: /NewVer/SupportRate_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "ابزارهای سیستم-> پشتیبانی نرخ ها");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }

        public ActionResult Taeed(int id)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_SupportRateUpdateType(id, Convert.ToInt32(Session["UserId"]));
            return Json("", JsonRequestBehavior.AllowGet);
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
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();

            string mun = Session["UserMnu"].ToString();
            var q = m.sp_SupportRateSelect("Type", "", 30).ToList();
            return this.Store(q);
        }

    }
}
