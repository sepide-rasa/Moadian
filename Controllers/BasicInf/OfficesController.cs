using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
namespace Avarez.Controllers.BasicInf
{
    [Authorize]
    public class OfficesController : Controller
    {
        //
        // GET: /Offices/
        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 48))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->دفتر");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_OfficesSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);

        }
        public JsonResult GetOfficeType()
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            return Json(car.sp_OfficesTypeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldType }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeState()
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            return Json(car.sp_StateSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeCounty(int cboState)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var County = car.sp_CountySelect("fldStateID", cboState.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeZone(int cboCounty)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var Zone = car.sp_ZoneSelect("fldCountyID", cboCounty.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Zone.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        


        public JsonResult GetCascadeCity(int cboZone)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var City = car.sp_CitySelect("fldZoneID", cboZone.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(City.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeMunicipality(int cboCity)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var Municipality = car.sp_MunicipalitySelect("fldCityID", cboCity.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Municipality.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeLocal(int cboMnu)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var Local = car.sp_LocalSelect("fldMunicipalityID", cboMnu.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Local.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeArea(int cboMnu)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var Local = car.sp_AreaSelect("fldMunicipalityID", cboMnu.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Local.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldName", "fldOfficesType", "fldMunicipalityName", "fldLocalName", "fldAreaName", "fldLocalID", "fldAreaID", "fldOfficesTypeID" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_OfficesSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 50))
                {
                    Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_OfficesDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 });
                    }
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
                }
            }
            catch (Exception x)
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public ActionResult Save(Models.tblOffice Office)
        {
            try
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                if (Office.fldDesc == null)
                    Office.fldDesc = "";
                if (Office.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 49))
                    {
                        Car.sp_OfficesInsert(Office.fldName, Office.fldAddress, Office.fldOfficesTypeID, Office.fldMunicipalityID,
                            Office.fldLocalID, Office.fldAreaID, Convert.ToInt32(Session["UserId"]), Office.fldDesc, Office.fldTel, Session["UserPass"].ToString());
                        Car.SaveChanges();
                        return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 51))
                    {
                        Car.sp_OfficesUpdate(Office.fldID, Office.fldName, Office.fldAddress, Office.fldOfficesTypeID, Office.fldMunicipalityID,
                            Office.fldLocalID, Office.fldAreaID, Convert.ToInt32(Session["UserId"]), Office.fldDesc, Office.fldTel, Session["UserPass"].ToString());
                        return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }
                }

            }
            catch (Exception x)
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                var q = Car.sp_OfficesSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q1 = Car.sp_MunicipalitySelect("fldId", q.fldMunicipalityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q2 = Car.sp_CitySelect("fldId", q1.fldCityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q3 = Car.sp_ZoneSelect("fldId", q2.fldZoneID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q4 = Car.sp_CountySelect("fldId", q3.fldCountyID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                var StateID = 0; var CountyID = 0; var ZoneID = 0; var CityID = 0; var MunicipalityID = 0;
                StateID = q4.fldStateID;
                CountyID = q3.fldCountyID;
                ZoneID = q2.fldZoneID;
                CityID = q1.fldCityID;
                MunicipalityID = (int)q.fldMunicipalityID;

                var County = Car.sp_CountySelect("fldStateID", StateID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                var Zone = Car.sp_ZoneSelect("fldCountyID", CountyID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                var City = Car.sp_CitySelect("fldZoneID", ZoneID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                var Municipality = Car.sp_MunicipalitySelect("fldCityID", CityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                var Local = Car.sp_LocalSelect("fldMunicipalityID", MunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                var Area = Car.sp_AreaSelect("fldMunicipalityID", MunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());

                return Json(new
                {
                    County = County,
                    Zone = Zone,
                    City = City,
                    Municipality = Municipality,
                    Area = Area,
                    Local = Local,
                    fldId = q.fldID,
                    fldName = q.fldName,
                    fldAddress = q.fldAddress,
                    fldOfficesTypeID = q.fldOfficesTypeID,
                    fldMunicipalityID = q.fldMunicipalityID,
                    fldLocalID = q.fldLocalID,
                    fldAreaID = q.fldAreaID,
                    fldTel = q.fldTel,
                    fldDesc = q.fldDesc,
                    fldStateID = q4.fldStateID,
                    fldCountyID = q3.fldCountyID,
                    fldZoneID = q2.fldZoneID,
                    fldCityID = q1.fldCityID
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

    }
}
