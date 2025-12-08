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
    public class CarController : Controller
    {
        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 233))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعریف وسیله نقلیه");
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
        public FileContentResult Image(int id)
        {//برگرداندن عکس  
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            var pic = p.sp_ShortTermCountrySelect("fldId", id.ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldIcon != null)
                {
                    return File((byte[])pic.fldIcon, "jpg");
                }
            }
            return null;

        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_CarSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);

        }

        public JsonResult GetCascadeShort(string cboCarMake)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            if (cboCarMake == "داخلی")
                return Json(car.sp_ShortTermCountrySelect("fldSymbol", "IR", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldSymbol }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
            else
                return Json(car.sp_ShortTermCountrySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(p => p.fldSymbol != "IR").Select(c => new { fldID = c.fldID, fldName = c.fldSymbol }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetCascadeColor()
        {

            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            return Json(car.sp_ColorCarSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldColor }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadePattern()
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            return Json(car.sp_CarPatternModelSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeMake()
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            return Json(car.sp_CarMakeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeAccount(int cboCarMake)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var County = car.sp_CarAccountTypeSelect("fldCarMakeID", cboCarMake.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeCabin(int cboCarAccountTypes)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var County = car.sp_CabinTypeSelect("fldCarAccountTypeID", cboCarAccountTypes.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeSystem(int cboCarCabin)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var County = car.sp_CarSystemSelect("fldCabinTypeID", cboCarCabin.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeModel(int cboSystem)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var County = car.sp_CarModelSelect("fldCarSystemID", cboSystem.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeClass(int cboModel)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var County = car.sp_CarClassSelect("fldCarModelID", cboModel.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldVIN", "fldShasiNumber", "fldMotorNumber" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_CarSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().OrderBy(p => p.fldID);
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 235))
                {
                    Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_CarDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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

        public ActionResult Save(Models.Car care)
        {
            try
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                var exist = Car.sp_CarSelect("fldVIN", care.fldVIN.ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().FirstOrDefault();
                if (care.fldDesc == null )
                    care.fldDesc = "";
                System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));

                if (care.fldID == 0 && exist==null )
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 234))
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(long));
                        Car.sp_CarInsert(_id, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN, care.fldCarModelID,
                            care.fldCarClassID, care.fldCarColorID, care.fldModel,
                            MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance),
                            Convert.ToInt32(Session["UserId"]), care.fldDesc, Session["UserPass"].ToString());

                        return Json(new { data = "ذخیره با موفقیت انجام شد. کد پرونده: " + id.Value, state = 0 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }
                }               
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 236))
                    {
                        Car.sp_CarUpdate(care.fldID, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN, care.fldCarModelID, care.fldCarClassID,
                            care.fldCarColorID, care.fldModel, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance), Convert.ToInt32(Session["UserId"]), care.fldDesc, Session["UserPass"].ToString());
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
        {
            //نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                var care = Car.sp_CarSelect("fldID", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var classs = Car.sp_CarClassSelect("fldID", care.fldCarClassID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var modell = Car.sp_CarModelSelect("fldID", care.fldCarModelID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var syst = Car.sp_CarSystemSelect("fldID", modell.fldCarSystemID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var cabin = Car.sp_CabinTypeSelect("fldID", syst.fldCabinTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var account = Car.sp_CarAccountTypeSelect("fldID", cabin.fldCarAccountTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var make = Car.sp_CarMakeSelect("fldID", account.fldCarMakeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                string fldVIN = care.fldVIN.ToString();
                if (fldVIN.Length == 17)
                    fldVIN = fldVIN.Substring(0, 2);
                else
                    fldVIN = "..";
                if (fldVIN == "..")
                {
                    fldVIN = "IR";

                }

                var q = Car.sp_ShortTermCountrySelect("fldSymbol", fldVIN, 1, 1, "").FirstOrDefault();
                int sybolid = 101;
                if (q != null)
                    sybolid = q.fldID;
                return Json(new
                {
                    sumbolid = sybolid,
                    symbol = fldVIN,
                    fldMotorNumber = care.fldMotorNumber,
                    fldShasiNumber = care.fldShasiNumber,
                    fldVIN = care.fldVIN,
                    fldCarModelID = care.fldCarModelID,
                    fldCarClassID = care.fldCarClassID,
                    fldCarColorID = care.fldCarColorID,
                    fldModel = care.fldModel,
                    fldStartDateInsurance = care.fldStartDateInsurance,
                    fldCarID = care.fldID,
                    fldId = care.fldID,
                    systfldID = syst.fldID,
                    cabinfldID = cabin.fldID,
                    accountfldID = account.fldID,
                    makefldID = make.fldID,
                    fldDesc = care.fldDesc
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
        public JsonResult sub(string id)
        {//نمایش اطلاعات جهت رویت کاربر
            return Json(new
            {

                model = id.Substring(1, 2)
            }, JsonRequestBehavior.AllowGet);

        }
    }
}
