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
    public class CarAccountTypeController : Controller
    {
        //
        // GET: /CarAccount/
        public ActionResult Index() 
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account"); 
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 69))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->نوع کاربری");
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
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_CarAccountTypeSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);

        }

        public JsonResult GetCascadeMake()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CarMakeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }


        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldName", "fldCarMakeName","fldCarMakeID" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
           var q = m.sp_CarAccountTypeSelect(_fiald[Convert.ToInt32(field)], searchtext, top,
                Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                Models.cartaxEntities Car = new Models.cartaxEntities();

                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 71))
                {
                    if (Convert.ToInt32(id) != 0)
                    {
                        var q = Car.sp_CarAccountTypeSelect("fldId", id, 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (UserId != 1 && !q.fldName.Contains("سواری") && !q.fldName.Contains("آمبولانس") && !q.fldName.Contains("وانت دوکابین") || UserId == 1)
                        {
                            Car.sp_CarAccountTypeDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public ActionResult Save(Models.sp_CarAccountTypeSelect CarAccountType)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter _CarAccountId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                Models.cartaxEntities Car = new Models.cartaxEntities();
                int UserId = Convert.ToInt32(Session["UserId"]);
                if (UserId != 1 && !CarAccountType.fldName.Contains("سواری") && !CarAccountType.fldName.Contains("آمبولانس") && !CarAccountType.fldName.Contains("وانت دوکابین") || UserId == 1)
                {
                    if (CarAccountType.fldDesc == null)
                        CarAccountType.fldDesc = "";
                    if (CarAccountType.fldID == 0)
                    {//ثبت رکورد جدید

                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 70))
                        {
                            Car.sp_CarAccountTypeInsert(_CarAccountId, CarAccountType.fldName, CarAccountType.fldCarMakeID,
                                Convert.ToInt32(Session["UserId"]), CarAccountType.fldDesc, Session["UserPass"].ToString());
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

                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 72))
                        {
                            Car.sp_CarAccountTypeUpdate(CarAccountType.fldID, CarAccountType.fldName,
                                CarAccountType.fldCarMakeID, Convert.ToInt32(Session["UserId"]), CarAccountType.fldDesc, Session["UserPass"].ToString());
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
                int UserId = Convert.ToInt32(Session["UserId"]);
                var q = Car.sp_CarAccountTypeSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (UserId != 1 && !q.fldName.Contains("سواری") && !q.fldName.Contains("آمبولانس") && !q.fldName.Contains("وانت دوکابین") || UserId == 1)
                {
                    return Json(new
                    {
                        fldId = q.fldID,
                        fldName = q.fldName,
                        fldCarMakeID = q.fldCarMakeID,
                        fldDesc = q.fldDesc,
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
