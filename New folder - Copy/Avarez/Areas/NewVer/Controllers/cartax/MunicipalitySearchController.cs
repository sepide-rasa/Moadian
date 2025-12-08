using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class MunicipalitySearchController : Controller
    {
        //
        // GET: /NewVer/MunicipalitySearch/

        public ActionResult Index(string State)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.State = State;
            return result;
        }

        public ActionResult Read(StoreRequestParameters parameters, string CarId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities p = new Models.cartaxEntities();
            List<Models.sp_MunicipalitySelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.sp_MunicipalitySelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldCityName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCityName";
                            break;
                    }
                    if (data != null)
                        data1 = p.sp_MunicipalitySelect(field, searchtext, 30, 1, "").ToList();
                    else
                        data = p.sp_MunicipalitySelect(field, searchtext, 30, 1,"").ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.sp_MunicipalitySelect("", "", 30, 1,"").ToList();
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

            List<Models.sp_MunicipalitySelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult GetMunInf(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            int State = 0, County = 0, Zone = 0, city = 0;
            var c_Mun = p.sp_MunicipalitySelect("fldId", id.ToString(), 1, 1, "").FirstOrDefault();
            var c_city = p.sp_CitySelect("fldId", c_Mun.fldCityID.ToString(), 1, 1, "").FirstOrDefault();
            var c_Zone = p.sp_ZoneSelect("fldid", c_city.fldZoneID.ToString(), 1, 1, "").FirstOrDefault();
            var c_County = p.sp_CountySelect("fldid", c_Zone.fldCountyID.ToString(), 1, 1, "").FirstOrDefault();
            var c_State = p.sp_CarModelSelect("fldId", c_County.fldStateID.ToString(), 1, 1, "").FirstOrDefault();


            State = c_County.fldStateID;
            County = c_Zone.fldCountyID;
            Zone = c_city.fldZoneID;
            city = c_Mun.fldCityID;

            var _County = p.sp_CountySelect("fldStateID", State.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
            var _zone = p.sp_ZoneSelect("fldCountyID", County.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
            var _city = p.sp_CitySelect("fldZoneID", Zone.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
            var _Mun = p.sp_MunicipalitySelect("fldCityID", city.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });

            return Json(new
            {
                State = State.ToString(),
                Mun = _Mun,
                MunId = id.ToString()
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
