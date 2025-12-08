using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
namespace Avarez.Controllers.CarTax
{
    public class SearchClassController : Controller
    {
        //
        // GET: /SearchClass/

        public ActionResult Index()
        {
            Session["searchtext"] = "%%";
            Session["top"] = 30;
            return PartialView();
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_SearchClass(Session["searchtext"].ToString(), Convert.ToInt32(Session["top"])).ToList().ToDataSourceResult(request);          
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


        public ActionResult GetClassInf(int id)
        {
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
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
                CarMake = CarMake,
                CarAccount = AccountType,
                CabinType = _CabinType,
                CarSystem = _CarSystem,
                CarModel = _CarModel,
                CarClass = _CarClass,
                CarAccountId = CarAccount,
                CabinTypeId = CabinType,
                CarSystemId = CarSystem,
                CarModelId = CarModel,
                CarClassId = id,
                Symbol = Symbol
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
