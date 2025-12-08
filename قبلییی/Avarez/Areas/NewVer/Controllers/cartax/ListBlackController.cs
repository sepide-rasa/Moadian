using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class ListBlackController : Controller
    {
        //
        // GET: /NewVer/ListBlack/

        public ActionResult Index(string containerId,string CarId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 308))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->لیست سیاه");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };
                Session["CarId"] = CarId;
                result.ViewBag.CarId = CarId;
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return result;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                }
            );
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 311))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();

                    Car.sp_ListeSiyahDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
                    return Json(new { Msg = "حذف با موفقیت انجام شد.", MsgTitle = "حذف موفق", Err = 0 });

                }
                else
                {
                    //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    //return RedirectToAction("error", "Metro");
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "شما مجاز به دسترسی نمی باشید."
                    });
                    DirectResult result = new DirectResult();
                    return result;
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Err = 1 });
            }
        }

        public ActionResult Save(Models.sp_ListeSiyahSelect BlackList)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (BlackList.fldDesc == null)
                    BlackList.fldDesc = "";
                if (BlackList.fldId == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 309))
                    {
                        Car.sp_ListeSiyahInsert(BlackList.fldCarId, BlackList.fldType, BlackList.fldMsg,
                             Convert.ToInt32(Session["UserId"]), BlackList.fldDesc);
                        return Json(new { Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = "ذخیره موفق", Err = 0 });
                    }
                    else
                    {
                        //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        //return RedirectToAction("error", "Metro");
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "شما مجاز به دسترسی نمی باشید."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 310))
                    {
                        Car.sp_ListeSiyahUpdate(BlackList.fldId, BlackList.fldCarId, BlackList.fldType, BlackList.fldMsg,
                              Convert.ToInt32(Session["UserId"]), BlackList.fldDesc);
                        return Json(new { Msg = "ویرایش با موفقیت انجام شد.", MsgTitle = "ویرایش موفق", Err = 0 });
                    }
                    else
                    {
                        //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        //return RedirectToAction("error", "Metro");
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "شما مجاز به دسترسی نمی باشید."
                        });
                        DirectResult result = new DirectResult();
                        return result;
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Err = 1 });
            }
        }

        public ActionResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_ListeSiyahSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldId,
                    CarId = q.fldCarId,
                    fldType = q.fldType.ToString(),
                    fldMsg = q.fldMsg,
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }
        public ActionResult Read(StoreRequestParameters parameters, string CarId)
        {
            //if (Session["UserId"] == null)
            //    return RedirectToAction("LogOn", "Account");
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities p = new Models.cartaxEntities();
            List<Models.sp_ListeSiyahSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.sp_ListeSiyahSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldMsg":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMsg";
                            break;
                        case "fldTypeS":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeS";
                            break;
                    }
                    if (data != null)
                        data1 = p.sp_ListeSiyahSelect(field, searchtext, 0).Where(l => l.fldCarId == Convert.ToInt32(CarId)).ToList();
                    else
                        data = p.sp_ListeSiyahSelect(field, searchtext, 0).Where(l => l.fldCarId == Convert.ToInt32(CarId)).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.sp_ListeSiyahSelect("fldCarID", CarId, 0).ToList();
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

            List<Models.sp_ListeSiyahSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
