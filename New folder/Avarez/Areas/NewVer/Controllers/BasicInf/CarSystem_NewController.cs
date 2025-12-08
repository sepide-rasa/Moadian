using Avarez.Controllers.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;


namespace Avarez.Areas.NewVer.Controllers.BasicInf
{
    public class CarSystem_NewController : Controller
    {
        //
        // GET: /NewVer/CarSystem_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->سیستم خودرو");
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
        public ActionResult GetCascadeMake()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CarMakeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult loadFromWebService()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }

        public ActionResult GetCascadeAccount(int cboCarMake)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarAccountTypeSelect("fldCarMakeID", cboCarMake.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCascadeCabin(int cboCarAccountTypes)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CabinTypeSelect("fldCarAccountTypeID", cboCarAccountTypes.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult New(int Id)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
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

        public ActionResult loadFromWebServiceSystem(string CarMakeType, string CarAccountType, string CarCabin, string CarSystem, string CarTip, string CarClass)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            System.Data.Entity.Core.Objects.ObjectParameter _CarMakeId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            System.Data.Entity.Core.Objects.ObjectParameter _CarAccountId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            System.Data.Entity.Core.Objects.ObjectParameter _CarCabinId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            System.Data.Entity.Core.Objects.ObjectParameter _CarSystemId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            System.Data.Entity.Core.Objects.ObjectParameter _CarModelId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            RateWebService.Rate a = new RateWebService.Rate();
            Models.cartaxEntities p = new Models.cartaxEntities();

            var mun = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            
            int i = 0;
            var Check = a.CheckAccountCharge(mun.fldRWUserName, mun.fldRWPass, mun.fldName);
            if (Check == true)
            {
                var g = a.GetClass(mun.fldRWUserName, mun.fldRWPass, mun.fldName, CarMakeType, CarAccountType, CarCabin, CarSystem, CarTip, CarClass).ToList();
                int CarMakeId = 0, CarAccountId = 0, CarCabinId = 0, CarSystemId = 0, CarModelId = 0; string[] ClassName;
                int itemsCount = g.Count;
                foreach (var item in g)
                {
                    i++;
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
                    //Functions.SendProgress("Process in progress...", i, itemsCount);
                }
                return Json(new
                {
                    MsgTitle = "عملیات موفق",
                    Msg = "بارگذاری با موفقیت انجام شد.",
                    Er = 0
                },JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "شما مجاز به استفاده از خدمات پشتیبانی نمی باشید، لطفا با واحد پشتیبانی تماس بگیرید.",
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Save(Models.sp_CarSystemSelect CarSystem)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New");
                int UserId = Convert.ToInt32(Session["UserId"]);
                System.Data.Entity.Core.Objects.ObjectParameter _CarSystemId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                Models.cartaxEntities Car = new Models.cartaxEntities();
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
                            return Json(new
                            {
                                MsgTitle = "ذخیره موفق",
                                Msg = "ذخیره با موفقیت انجام شد.",
                                Er = 0
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                MsgTitle = "خطا",
                                Msg = "شما مجاز به دسترسی نمی باشید.",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {//ویرایش رکورد ارسالی
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 80))
                        {
                            Car.sp_CarSystemUpdate(CarSystem.fldID, CarSystem.fldName, CarSystem.fldCabinTypeID, Convert.ToInt32(Session["UserId"]),
                                CarSystem.fldDesc, Session["UserPass"].ToString());
                            return Json(new
                            {
                                MsgTitle = "ویرایش موفق",
                                Msg = "ویرایش با موفقیت انجام شد.",
                                Er = 0
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                MsgTitle = "خطا",
                                Msg = "شما مجاز به دسترسی نمی باشید.",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
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
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(int Id)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New");
                int UserId = Convert.ToInt32(Session["UserId"]);
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 79))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    var q2 = Car.sp_CarSystemSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    var q = Car.sp_CabinTypeSelect("fldId", q2.fldCabinTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    if (UserId != 1 && !q.fldCarAccountTypeName.Contains("سواری") && !q.fldCarAccountTypeName.Contains("آمبولانس") && !q.fldCarAccountTypeName.Contains("وانت دوکابین") || UserId == 1)
                    {
                        Car.sp_CarSystemDelete(Convert.ToInt32(Id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        return Json(new
                        {
                            MsgTitle = "حذف موفق",
                            Msg = "حذف با موفقیت انجام شد.",
                            Er = 0
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
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
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Details(int Id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New");
                int UserId = Convert.ToInt32(Session["UserId"]);
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q2 = Car.sp_CarSystemSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                        fldCabinTypeID = q2.fldCabinTypeID.ToString(),
                        fldDesc = q2.fldDesc,
                        fldCarMakeID = q1.fldCarMakeID.ToString(),
                        fldCarAccountTypeID = q.fldCarAccountTypeID.ToString(),
                        Er = 0
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
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
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_CarSystemSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_CarSystemSelect> data1 = null;
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
                        case "fldCabinTypeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCabinTypeName";
                            break;
                        case "fldCarAccountType":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCarAccountType";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_CarSystemSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_CarSystemSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_CarSystemSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_CarSystemSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
