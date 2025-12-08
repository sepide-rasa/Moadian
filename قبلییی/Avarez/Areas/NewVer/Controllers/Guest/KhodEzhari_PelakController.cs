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
using System.Text.RegularExpressions;
using System.Globalization;
using Avarez.Models;
using System.Web.Configuration;

namespace Avarez.Areas.NewVer.Controllers.Guest
{
    public class KhodEzhari_PelakController : Controller
    {
        //
        // GET: /NewVer/KhodEzhari_Pelak/

        public ActionResult Index(long MalekId)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Malekid = MalekId;
            return PartialView;
        }
        public ActionResult HelpPelak()
        {//باز شدن پنجره
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
        }
        public ActionResult SavePelak(Models.sp_CarPlaqueSelect CarPlaque)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();

                if (CarPlaque.fldDesc == null)
                    CarPlaque.fldDesc = "";
                System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                if (CarPlaque.fldID == 0)
                {//ثبت رکورد جدید
                        if (CarPlaque.fldCharacterPersianPlaqueID == 0)
                            Car.sp_CarPlaqueInsert(id, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,
                            CarPlaque.fldPlaqueTypeID, CarPlaque.fldOwnerID, null,
                            CarPlaque.fldStatusPlaqeID, null, CarPlaque.fldDesc, null);
                        else
                            Car.sp_CarPlaqueInsert(id, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,
                            CarPlaque.fldPlaqueTypeID, CarPlaque.fldOwnerID, CarPlaque.fldCharacterPersianPlaqueID,
                            CarPlaque.fldStatusPlaqeID, null, CarPlaque.fldDesc, null);

                        return Json(new
                        {
                            Msg = "ذخیره با موفقیت انجام شد.",
                            MsgTitle = "ذخیره موفق",
                            Err = 0,
                            id = id.Value
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                {//ویرایش رکورد ارسالی
                        Car.sp_CarPlaqueUpdate(CarPlaque.fldID, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,
                                        CarPlaque.fldPlaqueTypeID, CarPlaque.fldOwnerID, CarPlaque.fldCharacterPersianPlaqueID,
                                        CarPlaque.fldStatusPlaqeID, null, CarPlaque.fldDesc, null);
                        return Json(new
                        {
                            Msg = "ویرایش با موفقیت انجام شد.",
                            MsgTitle = "ویرایش موفق",
                            Err = 0,
                            id = CarPlaque.fldID
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, null, x.Message, DateTime.Now, null);
                return Json(new
                {
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    MsgTitle = "خطا",
                    Err = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DetailSubSetting()
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var subSett = Car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), Car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                int? fldDefaultPelakSerial = 0;
                int? fldDefaultPelakChar = 0;
                if (subSett != null)
                {
                    if (subSett.fldDefaultPelakSerial != null)
                        fldDefaultPelakSerial = subSett.fldDefaultPelakSerial;
                    if (subSett.fldDefaultPelakChar != null)
                        fldDefaultPelakChar = subSett.fldDefaultPelakChar;
                }
                return Json(new
                {
                    fldDefaultPelakSerial = fldDefaultPelakSerial,
                    fldDefaultPelakChar = fldDefaultPelakChar
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, null, x.Message, DateTime.Now,"");
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }
        public JsonResult GetTypeP()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PlaqueTypeSelect("", "", 0, null, null).Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(l => l.Name), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCityP(string cboTypeP)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            if (cboTypeP == "ملی")
                return Json(car.sp_PlaqueCitySelect("fldName", "ایران", 0, null, null).Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(l => l.Name), JsonRequestBehavior.AllowGet);
            else
                return Json(car.sp_PlaqueCitySelect("", "", 0, null, null).Where(p => p.fldName != "ایران").Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(l => l.Name), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCharP()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CharacterPersianPlaqueSelect("", "", 0, null, null).Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStatusP()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StatusPlaqueSelect("", "", 0, null, null).Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSerialP()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PlaqueSerialSelect("", "", 0, null, null).Select(c => new { ID = c.fldID, Name = c.fldSerial }).OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadPelak(int MalekId)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            List<Models.sp_CarPlaqueSelect> data = null;
            data = car.sp_CarPlaqueSelect("fldOwnerID", MalekId.ToString(), 200, null, null).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadPelak(StoreRequestParameters parameters,string MalekId)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_CarPlaqueSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_CarPlaqueSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_CarPlaqueSelect(field, searchtext,  200, null, null).ToList();
                    else
                        data = m.sp_CarPlaqueSelect(field, searchtext,200, null, null).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_CarPlaqueSelect("fldOwnerID", MalekId.ToString(), 200, null, null).ToList();
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

            List<Avarez.Models.sp_CarPlaqueSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public JsonResult DetailsPelak(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var CarPlaque = Car.sp_CarPlaqueSelect("fldId", id.ToString(), 1, null, null).FirstOrDefault();
                var type = Car.sp_PlaqueTypeSelect("fldId", CarPlaque.fldPlaqueTypeID.ToString(), 1, null, null).FirstOrDefault();
                var IsAccept = false;
                if (CarPlaque.fldUserID != null)
                    IsAccept = true;
                int charId = 0;
                string two = "";
                string three = "";
                string charp = "";
                string plaqu = CarPlaque.fldPlaqueNumber; ;
                if (type.fldName == "ملی")
                {
                    two = plaqu.Substring(4, 2);
                    three = plaqu.Substring(0, 3);
                    charp = plaqu.Substring(3, 1);
                    var type1 = Car.sp_CharacterPersianPlaqueSelect("fldName", charp, 1, 1, "").FirstOrDefault();
                    charId = type1.fldID;
                    return Json(new
                    {
                        charId = charId.ToString(),
                        Two = two,
                        Three = three,
                        fldId = CarPlaque.fldID,
                        fldStatusPlaqeID = CarPlaque.fldStatusPlaqeID.ToString(),
                        fldPlaqueNumber = CarPlaque.fldPlaqueNumber.ToString(),
                        fldPlaqueCityID = CarPlaque.fldPlaqueCityID.ToString(),
                        fldPlaqueSerialID = CarPlaque.fldPlaqueSerialID.ToString(),
                        fldPlaqueTypeID = CarPlaque.fldPlaqueTypeID.ToString(),
                        fldOwnerID = CarPlaque.fldOwnerID.ToString(),
                        fldCharacterPersianPlaqueID = CarPlaque.fldPlaqueCityID.ToString(),
                        fldDesc = CarPlaque.fldDesc,
                        IsAccept = IsAccept
                    }, JsonRequestBehavior.AllowGet);

                }
                else if (type.fldName != "ملی")
                {
                    return Json(new
                    {
                        Three = CarPlaque.fldPlaqueNumber,
                        fldId = CarPlaque.fldID,
                        fldStatusPlaqeID = CarPlaque.fldStatusPlaqeID.ToString(),
                        fldPlaqueNumber = CarPlaque.fldPlaqueNumber.ToString(),
                        fldPlaqueCityID = CarPlaque.fldPlaqueCityID.ToString(),
                        fldPlaqueSerialID = CarPlaque.fldPlaqueSerialID.ToString(),
                        fldPlaqueTypeID = CarPlaque.fldPlaqueTypeID.ToString(),
                        fldOwnerID = CarPlaque.fldOwnerID.ToString(),
                        fldCharacterPersianPlaqueID = CarPlaque.fldPlaqueCityID.ToString(),
                        fldDesc = CarPlaque.fldDesc,
                        IsAccept = IsAccept
                    }, JsonRequestBehavior.AllowGet);

                }
                return Json("");
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException,null, x.Message, DateTime.Now, null);
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }
        public ActionResult DeletePelak(string id)
        {//حذف یک رکورد
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        var file = Car.sp_CarFileSelect("fldCarPlaqueID", id.ToString(), 1, null, null).FirstOrDefault();
                        if (file != null)
                        {
                            return Json(new
                            {
                                Msg = "برای پلاک موردنظر، پرونده ثبت شده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        Car.sp_CarPlaqueDelete(Convert.ToInt32(id),null, null);
                        return Json(new
                        {
                            Msg = "حذف با موفقیت انجام شد.",
                            MsgTitle = "حذف موفق",
                            Err = 0
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "رکوردی برای حذف انتخاب نشده است.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, null, x.Message, DateTime.Now, null);
                return Json(new
                {
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    MsgTitle = "خطا",
                    Err = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
