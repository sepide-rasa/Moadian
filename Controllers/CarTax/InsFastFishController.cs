using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.CarTax
{
    public class InsFastFishController : Controller
    {
        //
        // GET: /InsFastFish/

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 349))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->ثبت فیش سریع");
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

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldCarID" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_CollectionSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFishPrice(string id)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var q = car.sp_PeacockerySelect("fldid", id, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var fldShowMoney = "";
            if (q != null)
            {
                ViewBag.CarfileId = q.fldCarFileID;
                Session["CarFileID"] = q.fldCarFileID;
                fldShowMoney = q.fldShowMoney.ToString();
            }
            return Json(fldShowMoney, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSettleType()
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var Settle = car.sp_SettleTypeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Settle.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Models.sp_CollectionSelect Collection)
        {
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 350))
                {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                if (Collection.fldDesc == null)
                    Collection.fldDesc = "";
                if (Collection.fldSerialBarChasb == null)
                    Collection.fldSerialBarChasb = "";
                if (Collection.fldID == 0)
                {//ثبت رکورد جدید
                    
                        var fish = Car.sp_PeacockerySelect("fldId", Collection.fldPeacockeryCode.ToString(), 1,
                            Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (fish != null)
                        {
                            if (fish.fldCarFileID == Convert.ToInt32(Session["CarFileID"]))
                            {
                                var q = Car.sp_CollectionSelect("fldPeacockeryCode", Collection.fldPeacockeryCode.ToString(), 1,
                                    Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (q == null)
                                {
                                    System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", sizeof(int));
                                    Car.sp_CollectionInsert(_id,Convert.ToInt32( Session["CarFileID"]), MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                        Collection.fldPrice, Collection.fldSettleTypeID, (int)Collection.fldPeacockeryCode, null,
                                       Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]), Collection.fldDesc, Session["UserPass"].ToString(), "", null, "", null, null);
                                    Car.SaveChanges();
                                    SmsSender sms = new SmsSender();
                                    sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, Collection.fldPrice.ToString(), "", "", "");
                                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                                }
                                else
                                {
                                    return Json(new { data = "فیش فعلی قبلا در سیستم ثبت گردیده است.", state = 1 });
                                }
                            }
                            else
                                return Json(new { data = "فیش فعلی مربوط به این پرونده نمی باشد.", state = 1 });
                        }
                        else
                            return Json(new { data = "فیش فعلی در سیستم وجود ندارد.", state = 1 });
                    
                }
                else
                {//ویرایش رکورد ارسالی
                    
                        var fish = Car.sp_PeacockerySelect("fldId", Collection.fldPeacockeryCode.ToString(), 1,
                           Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (fish != null)
                        {
                            if (fish.fldCarFileID == Convert.ToInt32(Session["CarFileID"]))
                            {
                                Car.sp_CollectionUpdate(Collection.fldID, Convert.ToInt32(Session["CarFileID"]), MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                    Collection.fldPrice, Collection.fldSettleTypeID, (int)Collection.fldPeacockeryCode, null,
                                     Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]), Collection.fldDesc, Session["UserPass"].ToString(), "", null, "", null, null);
                                return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                            }
                            else
                                return Json(new { data = "فیش فعلی مربوط به این پرونده نمی باشد.", state = 1 });
                        }
                        else
                            return Json(new { data = "فیش فعلی در سیستم وجود ندارد.", state = 1 });
                    
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

    }
}
