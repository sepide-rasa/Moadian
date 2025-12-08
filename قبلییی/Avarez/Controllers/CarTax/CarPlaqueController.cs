using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
namespace Avarez.Controllers.CarTax
{
    [Authorize]
    public class CarPlaqueController : Controller
    {
        //
        
        public ActionResult Index(int  id,int State)
        {//بارگذاری صفحه اصلی 
            
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 229))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعریف پلاک");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                Models.cartaxEntities m = new Models.cartaxEntities();
                ViewBag.Owner = id;
                ViewBag.State = State;
                Session["ownerid"] = id;
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
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_CarPlaqueSelect("fldOwnerID", Session["ownerid"].ToString(), 200, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            Session.Remove("ownerid");
            return Json(q);

        }
        public JsonResult GetCascadeType()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PlaqueTypeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadePlaque(string cboTypeP)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            if(cboTypeP=="ملی")
                return Json(car.sp_PlaqueCitySelect("fldName", "ایران", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
            else
                return Json(car.sp_PlaqueCitySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(p => p.fldName != "ایران").Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeCityP()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PlaqueCitySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(p => p.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeChar()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CharacterPersianPlaqueSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(p => p.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeStatus()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StatusPlaqueSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(p => p.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadecboSerialP()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PlaqueSerialSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldSerial }).OrderBy(p => p.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            //coment
            string[] _fiald = new string[] { "fldOwnerID" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_CarPlaqueSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 231))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_CarPlaqueDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public ActionResult Save(Models.sp_CarPlaqueSelect CarPlaque)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();

                if (CarPlaque.fldDesc == null)
                    CarPlaque.fldDesc = "";
                System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                if (CarPlaque.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 230))
                    {
                        if (CarPlaque.fldCharacterPersianPlaqueID == 0)
                            Car.sp_CarPlaqueInsert(id, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,
                            CarPlaque.fldPlaqueTypeID, CarPlaque.fldOwnerID, null,
                            CarPlaque.fldStatusPlaqeID, Convert.ToInt32(Session["UserId"]), CarPlaque.fldDesc, Session["UserPass"].ToString());
                        else
                            Car.sp_CarPlaqueInsert(id, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,
                            CarPlaque.fldPlaqueTypeID, CarPlaque.fldOwnerID, CarPlaque.fldCharacterPersianPlaqueID,
                            CarPlaque.fldStatusPlaqeID, Convert.ToInt32(Session["UserId"]), CarPlaque.fldDesc, Session["UserPass"].ToString());

                        return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0, id = id.Value });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 232))
                    {
                        Car.sp_CarPlaqueUpdate(CarPlaque.fldID, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,
                                        CarPlaque.fldPlaqueTypeID, CarPlaque.fldOwnerID, CarPlaque.fldCharacterPersianPlaqueID,
                                        CarPlaque.fldStatusPlaqeID, Convert.ToInt32(Session["UserId"]), CarPlaque.fldDesc, Session["UserPass"].ToString());
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
                Models.cartaxEntities Car = new Models.cartaxEntities();
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
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var CarPlaque = Car.sp_CarPlaqueSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var type = Car.sp_PlaqueTypeSelect("fldId", CarPlaque.fldPlaqueTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                        charId = charId,
                        Two = two,
                        Three = three,
                        fldId = CarPlaque.fldID,
                        fldStatusPlaqeID = CarPlaque.fldStatusPlaqeID,
                        fldPlaqueNumber = CarPlaque.fldPlaqueNumber,
                        fldPlaqueCityID = CarPlaque.fldPlaqueCityID,
                        fldPlaqueSerialID = CarPlaque.fldPlaqueSerialID,
                        fldPlaqueTypeID = CarPlaque.fldPlaqueTypeID,
                        fldOwnerID = CarPlaque.fldOwnerID,
                        fldCharacterPersianPlaqueID = CarPlaque.fldPlaqueCityID,
                        fldDesc = CarPlaque.fldDesc
                    }, JsonRequestBehavior.AllowGet);

                }
                else if (type.fldName != "ملی")
                {
                    return Json(new
                    {
                        Three = CarPlaque.fldPlaqueNumber,
                        fldId = CarPlaque.fldID,
                        fldStatusPlaqeID = CarPlaque.fldStatusPlaqeID,
                        fldPlaqueNumber = CarPlaque.fldPlaqueNumber,
                        fldPlaqueCityID = CarPlaque.fldPlaqueCityID,
                        fldPlaqueSerialID = CarPlaque.fldPlaqueSerialID,
                        fldPlaqueTypeID = CarPlaque.fldPlaqueTypeID,
                        fldOwnerID = CarPlaque.fldOwnerID,
                        fldCharacterPersianPlaqueID = CarPlaque.fldPlaqueCityID,
                        fldDesc = CarPlaque.fldDesc
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
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
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
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }
    }
}
