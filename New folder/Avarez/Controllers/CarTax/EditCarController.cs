using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;

namespace Avarez.Controllers.CarTax
{
    public class EditCarController : Controller
    {
        //
        // GET: /EditCar/

        public ActionResult Index(int carid)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->ویرایش پرونده خودرو");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            ViewBag.carid = carid;
            return PartialView();

        }
        public JsonResult GetModel(int? Noo)
        {
            if (Noo == null)
                Noo = 1;
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Noo == 1)
            {
                for (int i = 1340; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = 1950; i <= DateTime.Now.Year + 1; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.OrderByDescending(l => l.Text).Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeShort(string cboCarMake)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            if (cboCarMake == "داخلی")
                return Json(car.sp_ShortTermCountrySelect("fldSymbol", "IR", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldSymbol }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
            else
                return Json(car.sp_ShortTermCountrySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(p => p.fldSymbol != "IR").Select(c => new { fldID = c.fldID, fldName = c.fldSymbol }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetCascadeColor()
        {

            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_ColorCarSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldColor }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadePattern()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CarPatternModelSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeMake()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CarMakeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeAccount(int? cboCarMake)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarAccountTypeSelect("fldCarMakeID", cboCarMake.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeCabin(int? cboCarAccountTypes)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CabinTypeSelect("fldCarAccountTypeID", cboCarAccountTypes.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeSystem(int? cboCarCabin)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarSystemSelect("fldCabinTypeID", cboCarCabin.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeModel(int? cboSystem)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarModelSelect("fldCarSystemID", cboSystem.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeClass(int? cboModel)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarClassSelect("fldCarModelID", cboModel.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Models.Car care)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (care.fldDesc == null)
                    care.fldDesc = "";
                if (care.fldVIN == null || care.fldVIN.Length < 17)
                    care.fldVIN = "";
                if (care.fldID != 0 )
                {
                    //Car.sp_CarUpdate(care.fldID, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN, care.fldCarModelID, care.fldCarClassID,
                    //    care.fldCarColorID, care.fldModel, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance), Convert.ToInt32(Session["UserId"]), care.fldDesc, Session["UserPass"].ToString());

                    return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                }
                else
                    return Json(new { data = "ویرایش با موفقیت انجام نشد.", state = 1 });
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
        public FileContentResult Image(int id)
        {//برگرداندن عکس  
            Models.cartaxEntities p = new Models.cartaxEntities();
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
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldVIN", "fldShasiNumber", "fldMotorNumber" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_CarSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().OrderBy(p => p.fldID);
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Details(int id)
        {
            //نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var file = Car.sp_CarFileSelect("fldCarID", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var care = Car.sp_CarSelect("fldId", file.fldCarID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var plaq = Car.sp_CarPlaqueSelect("fldID", file.fldCarPlaqueID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                int CarMake = 0, CarAccount = 0, CabinType = 0, CarSystem = 0, CarModel = 0;
                var c_class = Car.sp_CarClassSelect("fldId", care.fldCarClassID.ToString(), 1, 1, "").FirstOrDefault();
                var c_model = Car.sp_CarModelSelect("fldId", c_class.fldCarModelID.ToString(), 1, 1, "").FirstOrDefault();
                var c_system = Car.sp_CarSystemSelect("fldId", c_model.fldCarSystemID.ToString(), 1, 1, "").FirstOrDefault();
                var c_CabinType = Car.sp_CabinTypeSelect("fldId", c_system.fldCabinTypeID.ToString(), 1, 1, "").FirstOrDefault();
                var c_Account = Car.sp_CarAccountTypeSelect("fldId", c_CabinType.fldCarAccountTypeID.ToString(), 1, 1, "").FirstOrDefault();
                var c_Make = Car.sp_CarMakeSelect("fldId", c_Account.fldCarMakeID.ToString(), 0, 1, "").FirstOrDefault();

                var Symbol = Car.sp_ShortTermCountrySelect("fldSymbol", "IR", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldSymbol });
                if (c_Make.fldName != "داخلی")
                    Symbol = Car.sp_ShortTermCountrySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(p1 => p1.fldSymbol != "IR").Select(c => new { fldID = c.fldID, fldName = c.fldSymbol });


                CarMake = c_Account.fldCarMakeID;
                CarAccount = c_CabinType.fldCarAccountTypeID;
                CabinType = (int)c_system.fldCabinTypeID;
                CarSystem = c_model.fldCarSystemID;
                CarModel = c_class.fldCarModelID;

                var AccountType = Car.sp_CarAccountTypeSelect("fldCarMakeID", CarMake.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CabinType = Car.sp_CabinTypeSelect("fldCarAccountTypeID", CarAccount.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarSystem = Car.sp_CarSystemSelect("fldCabinTypeID", CabinType.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarModel = Car.sp_CarModelSelect("fldCarSystemID", CarSystem.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarClass = Car.sp_CarClassSelect("fldCarModelID", CarModel.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });

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
                int sumbolid = 0;
                if (q != null)
                    sumbolid = q.fldID;
                return Json(new
                {
                    sumbolid = sumbolid,
                    symbol = fldVIN,
                    fldMotorNumber = care.fldMotorNumber,
                    fldShasiNumber = care.fldShasiNumber,
                    fldVIN = care.fldVIN,
                    fldCarModelID = care.fldCarModelID,
                    fldCarClassID = care.fldCarClassID,
                    fldCarColorID = care.fldCarColorID,
                    fldColorName = care.fldColor,
                    fldModel = care.fldModel,
                    fldStartDateInsurance = care.fldStartDateInsurance,
                    fldCarID = care.fldID,
                    fldCarPlaqueID = file.fldCarPlaqueID,
                    fldCarPlaquenum = plaq.fldPlaqueNumber,
                    fldDatePlaque = file.fldDatePlaque,
                    fldId = care.fldID,
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
                    CarClassId = care.fldCarClassID,
                    Symbol = Symbol,
                    fldDesc = care.fldDesc
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
