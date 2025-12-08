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
    public class ShortTermController : Controller
    {
        //
        // GET: /ShortTerm/
        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 61))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->کشورهای سازنده");
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
            var q = m.sp_ShortTermCountrySelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);
        }


        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldEnglishName", "fldPersianName", "fldSymbol" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_ShortTermCountrySelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            var pic = p.sp_ShortTermCountrySelect("fldID", id.ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldIcon != null)
                {
                    return File((byte[])pic.fldIcon, "jpg");
                }
            }
            return null;

        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                if (UserId == 1)
                {
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 63))
                    {
                        Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                        if (Convert.ToInt32(id) != 0)
                        {
                            Car.sp_ShortTermCountryDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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

        public ActionResult Save(Models.ShorTerm shortterm)
        {
            try
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                int UserId = Convert.ToInt32(Session["UserId"]);
                if (UserId == 1)
                {
                    if (shortterm.fldDesc == null)
                        shortterm.fldDesc = "";
                    if (shortterm.fldID == 0)
                    {//ثبت رکورد جدید
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 62))
                        {
                            byte[] image = null;
                            if (shortterm.fldImage != null)
                                image = Avarez.Helper.ClsCommon.Base64ToImage(shortterm.fldImage);
                            Car.sp_ShortTermCountryInsert(shortterm.fldEnglishName, shortterm.fldPersianName, shortterm.fldSymbol, image, Convert.ToInt32(Session["UserId"]),
                                shortterm.fldDesc, Session["UserPass"].ToString());
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
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 64))
                        {
                            Car.sp_ShortTermCountryUpdate(shortterm.fldID, shortterm.fldEnglishName, shortterm.fldPersianName, shortterm.fldSymbol,
                                Avarez.Helper.ClsCommon.Base64ToImage(shortterm.fldImage), Convert.ToInt32(Session["UserId"]), shortterm.fldDesc, Session["UserPass"].ToString());
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

        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                var q = Car.sp_ShortTermCountrySelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                return Json(new
                {
                    fldID = q.fldID,
                    fldEnglishName = q.fldEnglishName,
                    fldPersianName = q.fldPersianName,
                    fldSymbol = q.fldSymbol,
                    fldDesc = q.fldDesc
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

    }
}
