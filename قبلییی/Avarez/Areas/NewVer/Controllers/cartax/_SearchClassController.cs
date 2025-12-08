using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Avarez.Controllers.Users;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class _SearchClassController : Controller
    {
        //
        // GET: /NewVer/_SearchClass/

        public ActionResult Index(int State)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New"); 
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<Models.sp_SearchClass> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.sp_SearchClass> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldID";
                            break;
                        case "ClassName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "ClassName";
                            break;
                        case "fldCylinderNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCylinderNumber";
                            break;
                        case "fldWheelNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldWheelNumber";
                            break;

                    }
                    if (data != null)
                        data1 = p.sp_SearchClass(searchtext, 100,field ).ToList();
                    else
                        data = p.sp_SearchClass(searchtext, 100, field).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.sp_SearchClass("", 100,"").ToList();
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
                            //return !oValue.ToString().Contains(value.ToString());
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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

            List<Models.sp_SearchClass> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult GetClassInf(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New"); 
            Models.cartaxEntities p = new Models.cartaxEntities();
            int CarMake = 0, CarAccount = 0, CabinType = 0, CarSystem = 0, CarModel = 0;
            var c_class = p.sp_CarClassSelect("fldId", id.ToString(), 1, 1, "").FirstOrDefault();
            var c_model = p.sp_CarModelSelect("fldId", c_class.fldCarModelID.ToString(), 1, 1, "").FirstOrDefault();
            var c_system = p.sp_CarSystemSelect("fldId", c_model.fldCarSystemID.ToString(), 1, 1, "").FirstOrDefault();
            var c_CabinType = p.sp_CabinTypeSelect("fldId", c_system.fldCabinTypeID.ToString(), 1, 1, "").FirstOrDefault();
            var c_Account = p.sp_CarAccountTypeSelect("fldId", c_CabinType.fldCarAccountTypeID.ToString(), 1, 1, "").FirstOrDefault();
            var c_Make = p.sp_CarMakeSelect("fldId", c_Account.fldCarMakeID.ToString(), 0, 1, "").FirstOrDefault();

            var Symbol = p.sp_ShortTermCountrySelect("fldSymbol", "IR", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldSymbol });
            if (c_Make.fldName != "داخلی")
                Symbol = p.sp_ShortTermCountrySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(p1 => p1.fldSymbol != "IR").Select(c => new { fldID = c.fldID, fldName = c.fldSymbol });


            CarMake = c_Account.fldCarMakeID;
            CarAccount = c_CabinType.fldCarAccountTypeID;
            CabinType = (int)c_system.fldCabinTypeID;
            CarSystem = c_model.fldCarSystemID;
            CarModel = c_class.fldCarModelID;

            var AccountType = p.sp_CarAccountTypeSelect("fldCarMakeID", CarMake.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
            var _CabinType = p.sp_CabinTypeSelect("fldCarAccountTypeID", CarAccount.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
            var _CarSystem = p.sp_CarSystemSelect("fldCabinTypeID", CabinType.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
            var _CarModel = p.sp_CarModelSelect("fldCarSystemID", CarSystem.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
            var _CarClass = p.sp_CarClassSelect("fldCarModelID", CarModel.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });

            return Json(new
            {
                CarMake = CarMake.ToString(),
                CarAccount = AccountType,
                CabinType = _CabinType,
                CarSystem = _CarSystem,
                CarModel = _CarModel,
                CarClass = _CarClass,
                CarAccountId = CarAccount.ToString(),
                CabinTypeId = CabinType.ToString(),
                CarSystemId = CarSystem.ToString(),
                CarModelId = CarModel.ToString(),
                CarClassId = id.ToString(),
                Symbol = Symbol
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
