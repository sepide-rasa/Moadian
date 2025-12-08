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
    public class OnlineUsers_NewController : Controller
    {
        //
        // GET: /NewVer/OnlineUsers_New/ 

        public ActionResult Index(string containerId)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "ابزارهای سیستم -> کاربران آنلاین");
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
      
        public ActionResult ReloadOnlineUser()
        {//جستجو
            Models.cartaxEntities m = new Models.cartaxEntities();
            string mun = Session["UserMnu"].ToString();
            var q = Models.OnlineUser.userObj.Where(k => k.cboMnu == mun).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();

            string mun = Session["UserMnu"].ToString();
            var q = Models.OnlineUser.userObj.Where(k => k.cboMnu == mun).ToList();
            return this.Store(q);
            //if (filterHeaders.Conditions.Count > 0)
            //{
            //    string field = "";
            //    string searchtext = "";
            //    List<Avarez.Models.sp_SupportRateSelect> data1 = null;
            //    foreach (var item in filterHeaders.Conditions)
            //    {
            //        var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

            //        switch (item.FilterProperty.Name)
            //        {
            //            case "fldId":
            //                searchtext = ConditionValue.Value.ToString();
            //                field = "fldId";
            //                break;
            //            case "fldMakeName":
            //                searchtext = "%" + ConditionValue.Value.ToString() + "%";
            //                field = "fldMakeName";
            //                break;
            //            case "fldCarTypeName":
            //                searchtext = "%" + ConditionValue.Value.ToString() + "%";
            //                field = "fldCarTypeName";
            //                break;

            //        }
            //        if (data != null)

            //            data1 = m.sp_SupportRateSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            //        else
            //            data = m.sp_SupportRateSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            //    }
            //    if (data != null && data1 != null)
            //        data.Intersect(data1);
            //}
            //else
            //{
            //    data = m.sp_SupportRateSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            //}

            //var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            //if (fc != null)
            //{
            //    foreach (var condition in fc.Conditions)
            //    {
            //        string field = condition.FilterProperty.Name;
            //        var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

            //        data.RemoveAll(
            //            item =>
            //            {
            //                object oValue = item.GetType().GetProperty(field).GetValue(item, null);
            //                return !oValue.ToString().Contains(value.ToString());
            //            }
            //        );
            //    }
            //}
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            //int limit = parameters.Limit;

            //if ((parameters.Start + parameters.Limit) > q.Count)
            //{
            //    limit = q.Count - parameters.Start;
            //}

            //List<Avarez.Models.sp_SupportRateSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            ////-- end paging ------------------------------------------------------------

            //return this.Store(rangeData, data.Count);
        }

    }
}
