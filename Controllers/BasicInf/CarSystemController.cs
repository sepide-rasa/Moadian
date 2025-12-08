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
    public class CarSystemController : Controller
    {
        //
        // GET: /CarSystem/
        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 77))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->سیستم خودرو");
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
            var q = m.sp_CarSystemSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);

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

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldName", "fldCabinTypeName", "fldCabinTypeID" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_CarSystemSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 79))
                {
                    Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        var q2 = Car.sp_CarSystemSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        var q = Car.sp_CabinTypeSelect("fldId", q2.fldCabinTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (UserId != 1 && !q.fldCarAccountTypeName.Contains("سواری") && !q.fldCarAccountTypeName.Contains("آمبولانس") && !q.fldCarAccountTypeName.Contains("وانت دوکابین") || UserId == 1)
                        {
                            Car.sp_CarSystemDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                            return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
                        }
                        else
                        {
                            return Json(new { data = "شما مجاز به دسترسی نمی باشید.", state = 1 });
                        }
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

        public ActionResult Save(Models.tblCarSystem CarSystem)
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                System.Data.Entity.Core.Objects.ObjectParameter _CarSystemId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                var q = Car.sp_CabinTypeSelect("fldId", CarSystem.fldCabinTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (UserId != 1 && !q.fldCarAccountTypeName.Contains("سواری") && !q.fldCarAccountTypeName.Contains("آمبولانس") && !q.fldCarAccountTypeName.Contains("وانت دوکابین") || UserId == 1)
                {
                    if (CarSystem.fldDesc == null)
                        CarSystem.fldDesc = "";
                    if (CarSystem.fldID == 0)
                    {//ثبت رکورد جدید
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 78))
                        {
                            Car.sp_CarSystemInsert(_CarSystemId, CarSystem.fldName, CarSystem.fldCabinTypeID, Convert.ToInt32(Session["UserId"]),
                                CarSystem.fldDesc, Session["UserPass"].ToString());
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
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 80))
                        {
                            Car.sp_CarSystemUpdate(CarSystem.fldID, CarSystem.fldName, CarSystem.fldCabinTypeID, Convert.ToInt32(Session["UserId"]),
                                CarSystem.fldDesc, Session["UserPass"].ToString());
                            return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                        }
                        else
                        {
                            Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                            return RedirectToAction("error", "Metro");
                        }
                    }
                }
                else
                {
                    return Json(new { data = "شما مجاز به دسترسی نمی باشید.", state = 1 });
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
                int UserId = Convert.ToInt32(Session["UserId"]);
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                var q2 = Car.sp_CarSystemSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q = Car.sp_CabinTypeSelect("fldId", q2.fldCabinTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q1 = Car.sp_CarAccountTypeSelect("fldId", q.fldCarAccountTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var CarMakeID = 0; var CarAccountTypeID = 0;
                CarMakeID = q1.fldCarMakeID;
                CarAccountTypeID = q.fldCarAccountTypeID;
                var CarAccountType = Car.sp_CarAccountTypeSelect("fldCarMakeID", CarMakeID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var CabinType = Car.sp_CabinTypeSelect("fldCarAccountTypeID", CarAccountTypeID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });

                if (UserId != 1 && !q1.fldName.Contains("سواری") && !q1.fldName.Contains("آمبولانس") && !q1.fldName.Contains("وانت دوکابین") || UserId == 1)
                {
                    return Json(new
                    {
                        CarAccountType = CarAccountType,
                        CabinType = CabinType,
                        fldId = q2.fldID,
                        fldName = q2.fldName,
                        fldCabinTypeID = q2.fldCabinTypeID,
                        fldDesc = q2.fldDesc,
                        fldCarMakeID = q1.fldCarMakeID,
                        fldCarAccountTypeID = q.fldCarAccountTypeID,
                        er = ""
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { er = "شما مجاز به دسترسی نمی باشید." }, JsonRequestBehavior.AllowGet);
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
        public ActionResult loadFromWebService(string CarMakeType, string CarAccountType, string CarCabin, string CarSystem, string CarTip, string CarClass)
        {
            System.Data.Entity.Core.Objects.ObjectParameter _CarMakeId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            System.Data.Entity.Core.Objects.ObjectParameter _CarAccountId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            System.Data.Entity.Core.Objects.ObjectParameter _CarCabinId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            System.Data.Entity.Core.Objects.ObjectParameter _CarSystemId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            System.Data.Entity.Core.Objects.ObjectParameter _CarModelId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            RateWebService.Rate a = new RateWebService.Rate();
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();

            var mun = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            var Check = a.CheckAccountCharge(mun.fldRWUserName, mun.fldRWPass, mun.fldName);
            if (Check == true)
            {
                var g = a.GetClass(mun.fldRWUserName, mun.fldRWPass, mun.fldName, CarMakeType, CarAccountType, CarCabin, CarSystem, CarTip, CarClass).ToList();
                int CarMakeId = 0, CarAccountId = 0, CarCabinId = 0, CarSystemId = 0, CarModelId = 0; string[] ClassName;
                foreach (var item in g)
                {
                    CarMakeId = CarAccountId = CarCabinId = CarSystemId = CarModelId = 0;
                    ClassName = item.className.Split('|');
                    var CarMake = p.sp_CarMakeSelect("fldName", ClassName[0], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    if (CarMake == null)
                    {
                        p.sp_CarMakeInsert(_CarMakeId, ClassName[0], Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                        CarMakeId = Convert.ToInt32(_CarMakeId.Value);
                    }
                    else
                        CarMakeId = CarMake.fldID;
                    var CarAccount = p.sp_CarAccountTypeSelect("fldName", ClassName[1], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(h => h.fldCarMakeID == CarMakeId).FirstOrDefault();
                    if (CarAccount == null)
                    {
                        p.sp_CarAccountTypeInsert(_CarAccountId, ClassName[1], CarMakeId, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                        CarAccountId = Convert.ToInt32(_CarAccountId.Value);
                    }
                    else
                        CarAccountId = CarAccount.fldID;
                    var Cabin = p.sp_CabinTypeSelect("fldName", ClassName[2], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(h => h.fldCarAccountTypeID == CarAccountId).FirstOrDefault();
                    if (Cabin == null)
                    {
                        p.sp_CabinTypeInsert(_CarCabinId, ClassName[2].ToString(), CarAccountId, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                        CarCabinId = Convert.ToInt32(_CarCabinId.Value);
                    }
                    else
                        CarCabinId = Cabin.fldID;
                    var Car_System = p.sp_CarSystemSelect("fldName", ClassName[3], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(h => h.fldCabinTypeID == CarCabinId).FirstOrDefault();
                    if (Car_System == null)
                    {
                        p.sp_CarSystemInsert(_CarSystemId, ClassName[3], CarCabinId, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                        CarSystemId = Convert.ToInt32(_CarSystemId.Value);
                    }
                    else
                        CarSystemId = Car_System.fldID;
                    var CarModel = p.sp_CarModelSelect("fldName", ClassName[4], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(h => h.fldCarSystemID == CarSystemId).FirstOrDefault();
                    if (CarModel == null)
                    {
                        p.sp_CarModelInsert(_CarModelId, ClassName[4], CarSystemId, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                        CarModelId = Convert.ToInt32(_CarModelId.Value);
                    }
                    else
                        CarModelId = CarModel.fldID;
                    var Car_Class = p.sp_CarClassSelect("fldName", ClassName[5], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(h => h.fldCarModelID == CarModelId).FirstOrDefault();
                   
                    if (Car_Class == null)
                    {
                        var q = a.GetCarClassInf(mun.fldRWUserName, mun.fldRWPass, mun.fldName, ClassName[0], ClassName[1], ClassName[2], ClassName[3], ClassName[4], ClassName[5]);
                        p.sp_CarClassInsert(ClassName[5], CarModelId, q.fldCarPatternModelID, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                    }
                }
                return Json(new { data = "بارگذاری با موفقیت انجام شد.", state = 0 });
            }
            else
            {
                return Json(new { data = "شما مجاز به استفاده از خدمات پشتیبانی نمی باشید، لطفا با واحد پشتیبانی تماس بگیرید.", state = 1 });
            }
        }
    }
}