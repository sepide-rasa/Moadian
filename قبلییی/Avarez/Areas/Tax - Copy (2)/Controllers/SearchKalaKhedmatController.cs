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
    public class SearchKalaKhedmatController : Controller
    {
        //
        // GET: /Tax/SearchKalaKhedmat/

        public ActionResult Index(int rowIdx, byte State)
        {//باز شدن تب جدید
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.rowIdx = rowIdx;
            result.ViewBag.State = State;
            return result;


        }
        public ActionResult Read(StoreRequestParameters parameters, int state)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.prs_tblKala_KhedmatSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.prs_tblKala_KhedmatSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldDescriptionOfID":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDescriptionOfID";
                            break;
                        case "fldCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCode";
                            break;
                    }
                    if (data != null)
                        data1 = p.prs_tblKala_KhedmatSelect(field, searchtext, 100).ToList();
                    else
                        data = p.prs_tblKala_KhedmatSelect(field, searchtext, 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.prs_tblKala_KhedmatSelect("", "", 100).ToList();
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

            List<Models.prs_tblKala_KhedmatSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
