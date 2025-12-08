using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
using System.Net;
using System.Configuration;

namespace Avarez.Controllers.CarTax
{
    [Authorize]
    public class OwnerController : Controller
    {
        //
        public ActionResult Index(int id)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            var Per = 0;
            if (id == 1||id==4)
                Per = 226;
            else if (id == 2)
                Per = 229;
            else if (id == 3)
                Per = 237;
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), Per))
            {
                ViewBag.State = id;
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعریف مالک");
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
            var q = m.sp_OwnerSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);

        }
        public ActionResult EstelamSabt(string CodeMeli,string Tarikhtavalod)
        {
            try
            {
                InSabt.InternalSabtWebServise a = new InSabt.InternalSabtWebServise();
                string p = a.GetData(CodeMeli, Tarikhtavalod, "CArtaXWebSrv").Replace("_", " ");
                if (p != null)
                    return Json(new { state = "0", Name = p }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { state = "1" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = "1" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldName", "fldMelli_EconomicCode", "fldAddress" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_OwnerSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), -100))//فعلا امکان حذف ندارد
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_OwnerDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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

        public ActionResult Save(Models.sp_OwnerSelect Owner)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                int chck = 1;
                if (Owner.fldDesc == null)
                    Owner.fldDesc = "";
                if (Owner.fldOwnerType == 1)
                    chck = checks(Owner.fldMelli_EconomicCode);
                if (chck == 1)
                {
                    if (Owner.fldID == 0)
                    {//ثبت رکورد جدید
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 227))
                        {
                            var ow = Car.sp_OwnerSelect("fldMelli_EconomicCode", Owner.fldMelli_EconomicCode, 0,
                                Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            if (ow == null)
                            {
                                Car.sp_OwnerInsert(Owner.fldName, Owner.fldMelli_EconomicCode, Owner.fldOwnerType,
                                    Owner.fldEmail, Owner.fldMobile, Owner.fldAddress, Owner.fldPostalCode,
                                    Convert.ToInt32(Session["UserId"]), Owner.fldDesc, Session["UserPass"].ToString()
                                    , Owner.fldType, MyLib.Shamsi.Shamsi2miladiDateTime(Owner.fldDateShamsi),false);

                                var q = Car.sp_OwnerSelect("fldMelli_EconomicCode", Owner.fldMelli_EconomicCode, 30,
                                Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0, id = q.fldID });
                            }else
                            {
                                return Json(new { data = "کد ملی وارد شده تکراری است.", state = 1, id = 0 });
                            }
                        }
                        else
                        {
                            Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                            return RedirectToAction("error", "Metro");
                        }
                    }
                    else
                    {//ویرایش رکورد ارسالی
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 228))
                        {
                            Car.sp_OwnerUpdate(Owner.fldID, Owner.fldName, Owner.fldMelli_EconomicCode, Owner.fldOwnerType,
                                Owner.fldEmail, Owner.fldMobile, Owner.fldAddress, Owner.fldPostalCode,
                                Convert.ToInt32(Session["UserId"]), Owner.fldDesc, Session["UserPass"].ToString(),MyLib.Shamsi.Shamsi2miladiDateTime(Owner.fldDateShamsi),Owner.fldType);
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
                    return Json(new { data = "کد ملی وارد شده معتبر نمی باشد.", state = 1 });
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

        public JsonResult Details(string id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_OwnerSelect("fldID", id.ToString(), 1,
                    Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (q != null)
                    return Json(new
                    {
                        fldId = q.fldID,
                        fldName = q.fldName,
                        fldMelli_EconomicCode = q.fldMelli_EconomicCode,
                        fldOwnerType = q.fldOwnerType,
                        fldEmail = q.fldEmail,
                        fldMobile = q.fldMobile,
                        fldAddress = q.fldAddress,
                        fldPostalCode = q.fldPostalCode,
                        fldDesc = q.fldDesc,
                        fldDateShamsi = q.fldDateShamsi

                    }, JsonRequestBehavior.AllowGet);
                else
                {
                    return Json(new
                    {
                        fldId = 0,
                        fldName = "",
                        fldMelli_EconomicCode = id,
                        fldOwnerType = true,
                        fldEmail = "",
                        fldMobile = "",
                        fldAddress = "",
                        fldPostalCode = "",
                        fldDesc = "",
                        fldDateShamsi = ""
                    }, JsonRequestBehavior.AllowGet);
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


        public int checks(string codec)
        {
            char[] chArray = codec.ToCharArray();
            int[] numArray = new int[chArray.Length];
            string x = codec;
            switch (x)
            {
                case "0000000000":
                case "1111111111":
                case "2222222222":
                case "3333333333":
                case "4444444444":
                case "5555555555":
                case "6666666666":
                case "7777777777":
                case "8888888888":
                case "9999999999":
                case "0123456789":
                case "9876543210":

                    return 0;
                    break;
            }
            for (int i = 0; i < chArray.Length; i++)
            {
                numArray[i] = (int)char.GetNumericValue(chArray[i]);
            }
            int num2 = numArray[9];

            int num3 = ((((((((numArray[0] * 10) + (numArray[1] * 9)) + (numArray[2] * 8)) + (numArray[3] * 7)) + (numArray[4] * 6)) + (numArray[5] * 5)) + (numArray[6] * 4)) + (numArray[7] * 3)) + (numArray[8] * 2);
            int num4 = num3 - ((num3 / 11) * 11);
            if ((((num4 == 0) && (num2 == num4)) || ((num4 == 1) && (num2 == 1))) || ((num4 > 1) && (num2 == Math.Abs((int)(num4 - 11)))))
            {
                return 1;
            }
            else
            {
                return 0;

            }


        }
    }
}
