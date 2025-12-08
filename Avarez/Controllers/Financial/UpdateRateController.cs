using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.Financial
{
    [Authorize]
    public class UpdateRateController : Controller
    {
        //
        // GET: /UpdateRate/

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 119))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public JsonResult GetCascadePattern()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CarPatternModelSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCascadeMake()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CarMakeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeAccount(int cboCarMake)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarAccountTypeSelect("fldCarMakeID", cboCarMake.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeCabin(int cboCarAccountTypes)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CabinTypeSelect("fldCarAccountTypeID", cboCarAccountTypes.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeSystem(int cboCarCabin)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarSystemSelect("fldCabinTypeID", cboCarCabin.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeModel(int cboSystem)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarModelSelect("fldCarSystemID", cboSystem.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeClass(int cboModel)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarClassSelect("fldCarModelID", cboModel.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFromYear(int value)
        {
            List<SelectListItem> sal = new List<SelectListItem>();
          
                for (int i = value; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            
            return Json(sal.OrderByDescending(k=>k.Text).Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetToYear(int value)
        {
            List<SelectListItem> sal = new List<SelectListItem>();
         
                for (int i = value; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            return Json(sal.Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Update(string FromYear, string ToYear, string CarMakeType, string CarAccountType, string CarCabin,string CarSystem, string CarTip, string CarClass)
        {
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 120))
                {
                    var Year2 = ToYear;
                    var Year = FromYear.Split(',');
                    var Loop = Year.Count();

                    Models.cartaxEntities m = new Models.cartaxEntities();
                    var Mun = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    string make = CarMakeType, acc = CarAccountType, cabin = CarCabin, sys = CarSystem, tip = CarTip, cls = CarClass;
                    //make = m.sp_CarMakeSelect("fldid", CarMakeType, 0, 1, "").FirstOrDefault().fldName;
                    //acc = m.sp_CarAccountTypeSelect("fldid", CarAccountType, 0, 1, "").FirstOrDefault().fldName;
                    //cabin = m.sp_CabinTypeSelect("fldid", CarCabin, 0, 1, "").FirstOrDefault().fldName;
                    //sys = m.sp_CarSystemSelect("fldid", CarSystem, 0, 1, "").FirstOrDefault().fldName;
                    //tip = m.sp_CarModelSelect("fldid", CarTip, 0, 1, "").FirstOrDefault().fldName;
                    //cls = m.sp_CarClassSelect("fldid", CarTip, 0, 1, "").FirstOrDefault().fldName;

                    RateWebService.Rate WebRate = new RateWebService.Rate();
                    var Check = WebRate.CheckAccountCharge(Mun.fldRWUserName, Mun.fldRWPass, Mun.fldName);
                    if (Check == true)
                    {
                        int UserId = Convert.ToInt32(Session["UserId"]);

                        for (int i = 0; i < Loop; i++)
                        {
                            if (Year2 == "")
                            {
                                FromYear = ToYear = Year[i];
                            }
                            var Rate = WebRate.GetRate(Mun.fldRWUserName, Mun.fldRWPass, Mun.fldName, Convert.ToInt32(FromYear), Convert.ToInt32(ToYear), make, acc, cabin, sys, tip, cls);
                            var Type = 1;
                            var Code = 0;
                            Models.sp_CarAccountTypeSelect Acc = null;
                            Models.sp_CabinTypeSelect Cabin = null;
                            Models.sp_CarSystemSelect Sys = null;
                            Models.sp_CarModelSelect Tip = null;
                            Models.sp_CarClassSelect Class = null;

                            if (Rate!=null)
                                foreach (var item in Rate)
                                {
                                    var Name = item.fldName.Split('|');

                                    var Make = m.sp_CarMakeSelect("fldName", Name[0], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                    if (Make != null && Name.Count() > 1)
                                        Acc = m.sp_CarAccountTypeSelect("fldName", Name[1], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldCarMakeID == Make.fldID).FirstOrDefault();
                                    if (Acc != null && Name.Count() > 2)
                                        Cabin = m.sp_CabinTypeSelect("fldName", Name[2], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldCarAccountTypeID == Acc.fldID).FirstOrDefault();
                                    if (Cabin != null && Name.Count() > 3)
                                        Sys = m.sp_CarSystemSelect("fldName", Name[3], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldCabinTypeID == Cabin.fldID).FirstOrDefault();
                                    if (Sys != null && Name.Count() > 4)
                                        Tip = m.sp_CarModelSelect("fldName", Name[4], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldCarSystemID == Sys.fldID).FirstOrDefault();
                                    if (Tip != null && Name.Count() > 5)
                                        Class = m.sp_CarClassSelect("fldName", Name[5], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldCarModelID == Tip.fldID).FirstOrDefault();

                                    switch (Name.Count() - 1)
                                    {
                                        case 1:
                                            Type = 1;
                                            Code = Make.fldID;
                                            break;
                                        case 2:
                                            Type = 2;
                                            Code = Acc.fldID;
                                            break;
                                        case 3:
                                            Type = 3;
                                            Code = Cabin.fldID;
                                            break;
                                        case 4:
                                            Type = 4;
                                            Code = Sys.fldID;
                                            break;
                                        case 5:
                                            Type = 5;
                                            Code = Tip.fldID;
                                            break;
                                        case 6:
                                            Type = 6;
                                            Code = Class.fldID;
                                            break;
                                    }

                                    var CarSeries = m.sp_GET_IDCarSeries(Type, Code).FirstOrDefault();
                                    short? fromModel = null, ToModel = null;
                                    byte? ToWheel = null, FromWheel = null, ToCylinder = null, FromCylinder = null;
                                    if (item.fldFromModel != null)
                                        fromModel = (short)item.fldFromModel;
                                    if (item.fldToModel != null)
                                        ToModel = (short)item.fldToModel;
                                    if (item.fldToWheel != null)
                                        ToWheel = (byte)item.fldToWheel;
                                    if (item.fldFromWheel != null)
                                        FromWheel = (byte)item.fldFromWheel;
                                    if (item.fldToCylinder != null)
                                        ToCylinder = (byte)item.fldToCylinder;
                                    if (item.fldFromCylinder != null)
                                        FromCylinder = (byte)item.fldFromCylinder;

                                    var CRate = m.sp_ComplicationsRateSelect("fldYear", item.fldYear.ToString(), 0,
                                        Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString())
                                        .Where(k => k.fldCarSeriesID == CarSeries.CarSeriesId && k.fldCountryDivisions == 1
                                            && k.fldFromModel == fromModel && k.fldToModel == ToModel
                                            && k.fldFromWheel == FromWheel && k.fldToWheel == ToWheel
                                            && k.fldFromCylinder == FromCylinder && k.fldToCylinder == ToCylinder
                                            && k.fldFromContentMotor == item.fldFromContentMotor && 
                                            k.fldToContentMotor == item.fldToContentMotor).FirstOrDefault();
                                    if (CRate != null)
                                        m.sp_ComplicationsRateUpdate(CRate.fldID, Type, Code, 0, 0, Convert.ToInt16(item.fldYear), FromCylinder, ToCylinder, FromWheel, ToWheel, fromModel, ToModel, item.fldFromContentMotor, item.fldToContentMotor, item.fldPrice, Convert.ToInt64(Session["UserId"]), "", Session["UserPass"].ToString());
                                    else
                                        m.sp_ComplicationsRateInsert(Type, Code, 0, 0, Convert.ToInt16(item.fldYear), FromCylinder, ToCylinder, FromWheel, ToWheel, fromModel, ToModel, item.fldFromContentMotor, item.fldToContentMotor, item.fldPrice, Convert.ToInt64(Session["UserId"]), "", Session["UserPass"].ToString());
                                }

                        }
                        return Json(new { data = "بارگذاری با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        return Json(new { data = "شما مجاز به استفاده از خدمات پشتیبانی نمی باشید، لطفا با واحد پشتیبانی تماس بگیرید.", state = 1 });
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
    }
}
