using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
using Ext.Net;
using Ext.Net.MVC;

namespace Avarez.Areas.NewVer.Controllers.BasicInf
{
    public class Offices_NewController : Controller
    {
        //
        // GET: /NewVer/Offices_New/
        public ActionResult Index(string containerId)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->دفتر");
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
        public ActionResult New(int Id)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }

        public ActionResult Help()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult loadFromWebService()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetOfficeType()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_OfficesTypeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldType }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeState()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StateSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCascadeCounty(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CountySelect("fldStateID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeZone(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Zone = car.sp_ZoneSelect("fldCountyID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Zone.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCascadeCity(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var City = car.sp_CitySelect("fldZoneID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(City.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeMunicipality(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Municipality = car.sp_MunicipalitySelect("fldCityID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Municipality.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeLocal(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Local = car.sp_LocalSelect("fldMunicipalityID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Local.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeArea(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Local = car.sp_AreaSelect("fldMunicipalityID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Local.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 50))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();

                        Car.sp_OfficesDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        return Json(new
                        {
                            MsgTitle = "حذف موفق",
                            Msg = "حذف با موفقیت انجام شد.",
                            Er = 0
                        });
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    });
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                m.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }

        public ActionResult Save(Models.sp_OfficesSelect Office)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Office.fldDesc == null)
                    Office.fldDesc = "";
                if (Office.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 49))
                    {
                        Car.sp_OfficesInsert(Office.fldName, Office.fldAddress, Office.fldOfficesTypeID, Office.fldMunicipalityID,
                            Office.fldLocalID, Office.fldAreaID, Convert.ToInt32(Session["UserId"]), Office.fldDesc,
                            Office.fldTel, Session["UserPass"].ToString(), Office.fldExpire);
                        Car.SaveChanges();
                        return Json(new
                        {
                            MsgTitle = "ذخیره موفق",
                            Msg = "ذخیره با موفقیت انجام شد.",
                            Er = 0
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        });
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 51))
                    {
                        Car.sp_OfficesUpdate(Office.fldID, Office.fldName, Office.fldAddress, Office.fldOfficesTypeID, Office.fldMunicipalityID,
                            Office.fldLocalID, Office.fldAreaID, Convert.ToInt32(Session["UserId"]), Office.fldDesc, Office.fldTel, Session["UserPass"].ToString(), Office.fldExpire);
                        return Json(new
                        {
                            MsgTitle = "ویرایش موفق",
                            Msg = "ویرایش با موفقیت انجام شد.",
                            Er = 0
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        });
                    }
                }

            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }

        public ActionResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New");
                Models.cartaxEntities Car = new Models.cartaxEntities();
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
                    fldOfficesTypeID = q.fldOfficesTypeID.ToString(),
                    fldMunicipalityID = q.fldMunicipalityID.ToString(),
                    fldLocalID = q.fldLocalID.ToString(),
                    fldAreaID = q.fldAreaID.ToString(),
                    fldTel = q.fldTel,
                    fldDesc = q.fldDesc,
                    fldStateID = q4.fldStateID.ToString(),
                    fldCountyID = q3.fldCountyID.ToString(),
                    fldZoneID = q2.fldZoneID.ToString(),
                    fldCityID = q1.fldCityID.ToString(),
                    fldExpire = q.fldExpire,
                    Er = 0
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_OfficesSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_OfficesSelect> data1 = null;
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
                        case "fldOfficesType":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOfficesType";
                            break;
                        case "fldAreaName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAreaName";
                            break;
                        case "fldMunicipalityName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMunicipalityName";
                            break;
                        case "fldLocalName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldLocalName";
                            break;
                        case "fldAddress":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAddress";
                            break;
                        case "fldTel":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTel";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_OfficesSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_OfficesSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_OfficesSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_OfficesSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
