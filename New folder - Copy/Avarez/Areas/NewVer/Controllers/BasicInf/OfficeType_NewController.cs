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
    public class OfficeType_NewController : Controller
    {
        //
        // GET: /NewVer/OfficeType_New/

        public ActionResult Index(string containerId)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->نوع دفتر");
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
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 46))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                        Car.sp_OfficesTypeDelete(Convert.ToByte(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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

        public ActionResult Save(Models.sp_OfficesTypeSelect OfficesType)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (OfficesType.fldDesc == null)
                    OfficesType.fldDesc = "";
                if (OfficesType.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 45))
                    {
                        Car.sp_OfficesTypeInsert(OfficesType.fldType, Convert.ToInt32(Session["UserId"]), OfficesType.fldDesc, Session["UserPass"].ToString());
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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 47))
                    {
                        Car.sp_OfficesTypeUpdate(OfficesType.fldID, OfficesType.fldType, Convert.ToInt32(Session["UserId"]), OfficesType.fldDesc, Session["UserPass"].ToString());
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
                var q = Car.sp_OfficesTypeSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldID,
                    fldType = q.fldType,
                    fldDesc = q.fldDesc,
                    Er=0
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
            List<Avarez.Models.sp_OfficesTypeSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_OfficesTypeSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldType":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldType";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_OfficesTypeSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_OfficesTypeSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_OfficesTypeSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_OfficesTypeSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
