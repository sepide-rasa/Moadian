using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;
using System.IO;

namespace Avarez.Areas.NewVer.Controllers.Users
{
    public class SearchUser_NewController : Controller
    {
        //
        // GET: /NewVer/SearchUser_New/

        public ActionResult Index(int state,string UserIds)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.state = state;
            PartialView.ViewBag.UserIds = UserIds;
            return PartialView;
        }
        //public ActionResult Read()
        //{
        //    if (Session["UserId"] == null)
        //        return RedirectToAction("LogOn", "Account_New");
        //    Models.cartaxEntities m = new Models.cartaxEntities();
        //    List<Avarez.Models.sp_UserSelect> info = null;

        //    info = m.sp_UserSelect("", "", 0, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
        //    return Json(info, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_UserSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_UserSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamily";
                            break;
                        case "fldMelliCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMelliCode";
                            break;
                        case "fldUserName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldUserName";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_UserSelect(field, searchtext, 0, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_UserSelect(field, searchtext, 0, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_UserSelect("", "", 0, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            return !oValue.ToString().Contains(value.ToString());
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Avarez.Models.sp_UserSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult GetChecked(string UserID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities m = new Models.cartaxEntities();
            var UserId = UserID.Split(';');
            int[] checksId = new int[UserId.Count()-1];

            for (int i = 0; i < UserId.Count()-1; i++)
            {
                checksId[i] = Convert.ToInt32(UserId[i]);
            }
            return Json(checksId, JsonRequestBehavior.AllowGet);
        }
    }
}
