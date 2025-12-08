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

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class MonitoringCarExperienceController : Controller
    {
        //
        // GET: /NewVer/MonitoringCarExperience/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            /*Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "مانیتورینگ انتقال سوابق");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();*/
            var result = new Ext.Net.MVC.PartialViewResult();
            //this.GetCmp<Panel>("PnlMonitoringCarExp").Add(this.GetCmp<Panel>("MonitoringCarExperience"));
            return result;
        }

        public ActionResult GetNotAccept()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            List<Models.sp_GetNotVerifiedCarExperience> data = null;
            data = car.sp_GetNotVerifiedCarExperience(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).ToList();
            //لیست سوابقی که از تاریخ آپدیت به بعد ثبت می شوند و تآیید نشده هستند 
            //و مربوط به یک شهرداری خاص می باشند.

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Models.sp_GetNotVerifiedCarExperience> data = null;
            data = m.sp_GetNotVerifiedCarExperience(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).ToList();
            return this.Store(data);
        }
    }
}
