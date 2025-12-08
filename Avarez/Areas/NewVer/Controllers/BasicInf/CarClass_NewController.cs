using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;

namespace Avarez.Areas.NewVer.Controllers.BasicInf
{
    public class CarClass_NewController : Controller
    {
        //
        // GET: /NewVer/CarClass_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->کلاس خودرو");
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

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_CarClassSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_CarClassSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldNameCarMake":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameCarMake";
                            break;
                        case "fldNameCarAccountType":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameCarAccountType";
                            break;
                        case "fldNameCabinType":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameCabinType";
                            break;
                        case "fldNameCarSystem":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameCarSystem";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldCarModelName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldCarPatternModelName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCarPatternModelName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_CarClassSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_CarClassSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_CarClassSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_CarClassSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult loadFromWebServiceCarClass()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
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
        public ActionResult GetCarMake()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities m = new Models.cartaxEntities();            
            var q = m.sp_CarMakeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().Select(c => new { fldId = c.fldID, fldTitle = c.fldName });
            return this.Store(q);
        }

        public ActionResult GetCarAccountTypes(string CarMakeID)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            var q = m.sp_CarAccountTypeSelect("fldCarMakeID", CarMakeID, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().Select(c => new { fldId = c.fldID, fldTitle = c.fldName });
            return this.Store(q);
        }

        public ActionResult GetCarCabin(string CarAccountTypesID)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            var q = m.sp_CabinTypeSelect("fldCarAccountTypeID", CarAccountTypesID, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().Select(c => new { fldId = c.fldID, fldTitle = c.fldName });
            return this.Store(q);
        }

        public ActionResult GetSystem(string CarCabinID)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            var q = m.sp_CarSystemSelect("fldCabinTypeID", CarCabinID, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().Select(c => new { fldId = c.fldID, fldTitle = c.fldName });
            return this.Store(q);
        }

        public ActionResult GetCarTip(string CarSystemID)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            var q = m.sp_CarModelSelect("fldCarSystemID", CarSystemID, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().Select(c => new { fldId = c.fldID, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetCarPattern()
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            var q = m.sp_CarPatternModelSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().Select(c => new { fldId = c.fldID, fldTitle = c.fldName });
            return this.Store(q);
        }

        public ActionResult Save(Models.sp_CarClassSelect carClass)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q3 = Car.sp_CarModelSelect("fldId", carClass.fldCarModelID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q2 = Car.sp_CarSystemSelect("fldId", q3.fldCarSystemID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q = Car.sp_CabinTypeSelect("fldId", q2.fldCabinTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (UserId != 1 && !q.fldCarAccountTypeName.Contains("سواری") && !q.fldCarAccountTypeName.Contains("آمبولانس") && !q.fldCarAccountTypeName.Contains("وانت دوکابین") || UserId == 1)
                {
                    if (carClass.fldDesc == null)
                        carClass.fldDesc = "";
                    if (carClass.fldID == 0)
                    {//ثبت رکورد جدید
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 90))
                        {
                            Car.sp_CarClassInsert(carClass.fldName, carClass.fldCarModelID,
                                carClass.fldCarPatternModelID, Convert.ToInt32(Session["UserId"]), carClass.fldDesc, Session["UserPass"].ToString());
                            return Json(new
                            {
                                MsgTitle = "ذخیره موفق",
                                Msg = "ذخیره با موفقیت انجام شد.",
                                Er = 0
                            });
                        }
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        });
                    }
                    else
                    {//ویرایش رکورد ارسالی
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 92))
                        {
                            Car.sp_CarClassUpdate(carClass.fldID, carClass.fldName,
                                carClass.fldCarModelID, carClass.fldCarPatternModelID, Convert.ToInt32(Session["UserId"])
                                , carClass.fldDesc, Session["UserPass"].ToString());
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

        public ActionResult Details(int Id)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q4 = Car.sp_CarClassSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q3 = Car.sp_CarModelSelect("fldId", q4.fldCarModelID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q2 = Car.sp_CarSystemSelect("fldId", q3.fldCarSystemID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q = Car.sp_CabinTypeSelect("fldId", q2.fldCabinTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q1 = Car.sp_CarAccountTypeSelect("fldId", q.fldCarAccountTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var CarMakeID = 0; var CarAccountTypeID = 0; var CabinTypeID = 0; var CarSystemID = 0;
                CarMakeID = q1.fldCarMakeID;
                CarAccountTypeID = q.fldCarAccountTypeID;
                CabinTypeID = (int)q2.fldCabinTypeID;
                CarSystemID = q3.fldCarSystemID;
                var CarAccountType = Car.sp_CarAccountTypeSelect("fldCarMakeID", CarMakeID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var CabinType = Car.sp_CabinTypeSelect("fldCarAccountTypeID", CarAccountTypeID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var CarSystem = Car.sp_CarSystemSelect("fldCabinTypeID", CabinTypeID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var CarModel = Car.sp_CarModelSelect("fldCarSystemID", CarSystemID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });

                if (UserId != 1 && !q1.fldName.Contains("سواری") && !q1.fldName.Contains("آمبولانس") && !q1.fldName.Contains("وانت دوکابین") || UserId == 1)
                {
                    return Json(new
                    {
                        CarAccountType = CarAccountType,
                        CabinType = CabinType,
                        CarSystem = CarSystem,
                        CarModel = CarModel,
                        fldId = q4.fldID,
                        fldName = q4.fldName,
                        fldCarModelID = q4.fldCarModelID,
                        fldCarPatternModelID = q4.fldCarPatternModelID,
                        fldCarSystemID = q3.fldCarSystemID,
                        fldCabinTypeID = q2.fldCabinTypeID,
                        fldCarMakeID = q1.fldCarMakeID,
                        fldCarAccountTypeID = q.fldCarAccountTypeID,
                        fldDesc = q4.fldDesc
                    }, JsonRequestBehavior.AllowGet);
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

        public ActionResult Delete(string Id)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New");
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 91))
                {
                    int UserId = Convert.ToInt32(Session["UserId"]);
                    Models.cartaxEntities Car = new Models.cartaxEntities();

                    var q4 = Car.sp_CarClassSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    var q3 = Car.sp_CarModelSelect("fldId", q4.fldCarModelID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    var q2 = Car.sp_CarSystemSelect("fldId", q3.fldCarSystemID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    var q = Car.sp_CabinTypeSelect("fldId", q2.fldCabinTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    if (UserId != 1 && !q.fldCarAccountTypeName.Contains("سواری") && !q.fldCarAccountTypeName.Contains("آمبولانس") && !q.fldCarAccountTypeName.Contains("وانت دوکابین") || UserId == 1)
                    {
                        Car.sp_CarClassDelete(Convert.ToInt32(Id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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
    }
}
