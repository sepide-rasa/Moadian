using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Avarez.Controllers.CarTax
{
    public class MnuSearchController : Controller
    {
        //
        // GET: /MnuSearch/

        public ActionResult Index(int id)
        {
            ViewBag.State = id;
            return PartialView();
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_MunicipalitySelect("", "", 30, 1, "").ToList().ToDataSourceResult(request);
            return Json(q);

        }

        public ActionResult Reload(string value, int top)
        {//جستجو
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[0], value);
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_MunicipalitySelect("fldName", searchtext, top, 1, "").ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetMunInf(int id)
        {
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
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
                State = State,
                Mun = _Mun,
                MunId = id
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
