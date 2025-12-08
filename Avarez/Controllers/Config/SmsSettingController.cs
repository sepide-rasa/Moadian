using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections;
using Avarez.Controllers.Users;
namespace Avarez.Controllers.Config
{
    [Authorize]
    public class SmsSettingController : Controller
    {
        //
        // GET: /SmsSetting/
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 204))
            {
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
            var q = m.sp_SMSSettingSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.SmsSetting SMSSetting)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (SMSSetting.fldDesc == null)
                    SMSSetting.fldDesc = "";
                if (SMSSetting.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 205))
                    {
                        Car.sp_SMSSettingInsert(SMSSetting.fldUserName, SMSSetting.fldPassword, SMSSetting.fldLineNumber, SMSSetting.fldCountryType,
                            SMSSetting.fldCountryCode, Convert.ToInt32(Session["UserId"]), SMSSetting.fldDesc, Session["UserPass"].ToString());

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 207))
                    {
                        Car.sp_SMSSettingUpdate(SMSSetting.fldID, SMSSetting.fldUserName, SMSSetting.fldPassword, SMSSetting.fldLineNumber, SMSSetting.fldCountryType,
                            SMSSetting.fldCountryCode, Convert.ToInt32(Session["UserId"]), SMSSetting.fldDesc, Session["UserPass"].ToString());
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
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public JsonResult _CountryTree(int? id)
        {
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            //پروسیجر تشخیص کاربر در تقسیمات کشوری برای
            //لود درخت همان ناحیه ای کاربر در سطح آن تعریف شده 
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (id != null)
            {
                var rols = (from k in p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var rols = (from k in p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
        }

       

        public JsonResult CountryPosition(int id)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var nodes = car.sp_SelectUpTreeCountryDivisions(id, 1, "").ToList();
            ArrayList ar = new ArrayList();

            foreach (var item in nodes)
            {
                ar.Add(item.fldNodeName);
            }
            string nodeNames = "";
            for (int i = 0; i < ar.Count; i++)
            {
                if (i < ar.Count - 1)
                    nodeNames += ar[i].ToString() + "-->";
                else
                    nodeNames += ar[i].ToString();
            }

            return Json(new { Position = nodeNames });
        }
        
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldCountryDivisionName", "fldName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_SMSSettingSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 206))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_SMSSettingDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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
                var SmsSetting = Car.sp_SMSSettingSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var countryId = Car.sp_TableTreeSelect("fldSourceID", SmsSetting.CoutryDivisionCode.ToString(), 0, 0, 0).Where(h => h.fldNodeType == SmsSetting.CoutryDivisionType).FirstOrDefault();
                return Json(new
                {
                    fldId = SmsSetting.fldID,
                    fldUserName = SmsSetting.fldUserName,
                    fldPassword = SmsSetting.fldPassword,
                    fldLineNumber = SmsSetting.fldLineNumber,
                    fldDesc = SmsSetting.fldDesc,
                    fldCountryType = SmsSetting.CoutryDivisionType,
                    fldCountryCode = SmsSetting.CoutryDivisionCode,
                    countryId = countryId.fldID
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
