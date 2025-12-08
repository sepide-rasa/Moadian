using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Areas.Tax.Models;
using System.IO;

namespace Avarez.Areas.Tax.Controllers
{
    public class SearchShakhsController : Controller
    {
        //
        // GET: /Tax/SearchShakhs/

        public ActionResult Index(int state, int UserType)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.state = state;
            result.ViewBag.UserType = UserType;
            return result;
        }
        public ActionResult GetShakhsType()
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var q = p.prs_tblTypeShakhsSelect("", "", 0).ToList().OrderBy(l => l.fldId).Select(l => new { ID = l.fldId, fldName = l.fldNameTypeShakhs });
            return this.Store(q);

        }
        public ActionResult Read(StoreRequestParameters parameters, int state, int UserType)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.prs_tblShakhsHaghighi_HoghoghiSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.prs_tblShakhsHaghighi_HoghoghiSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
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
                        case "fldNationalCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNationalCode";
                            break;
                        case "fldNameTypeShakhs":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameTypeShakhs";
                            break;
                    }
                    if (data != null)
                        data1 = p.prs_tblShakhsHaghighi_HoghoghiSelect(field, searchtext, 100).Where(l => l.fldTypeShakhsId == UserType).ToList();
                    else
                        data = p.prs_tblShakhsHaghighi_HoghoghiSelect(field, searchtext, 100).Where(l => l.fldTypeShakhsId == UserType).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.prs_tblShakhsHaghighi_HoghoghiSelect("", "", 100).Where(l => l.fldTypeShakhsId == UserType).ToList();
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

            List<Models.prs_tblShakhsHaghighi_HoghoghiSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
