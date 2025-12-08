using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Avarez.Controllers.Users;
using System.Text.RegularExpressions;
using System.Globalization;
using Avarez.Models;
using System.Web.Configuration;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class Fast_FishController : Controller
    {
        //
        // GET: /NewVer/Fast_Fish/

        public ActionResult Index(string containerId)
        {//باز شدن پرونده جدید
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            // this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult GetFishPrice(string id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
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

        public ActionResult GetSettleType()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Settle = car.sp_SettleTypeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Settle.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Models.sp_CollectionSelect Collection)
            {
            var Er = 0;
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 350))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
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
                                    Car.sp_CollectionInsert(_id, Convert.ToInt32(Session["CarFileID"]), MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                        Collection.fldPrice, Collection.fldSettleTypeID, (int)Collection.fldPeacockeryCode, null,
                                       Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]), Collection.fldDesc, Session["UserPass"].ToString(),
                                       "", null, "", null, null, true, 1, DateTime.Now);
                                    Car.SaveChanges();
                                    if (Convert.ToInt32(Session["UserMnu"]) == 1)
                                    {
                                        Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                                        var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, Collection.fldPeacockeryCode.ToString(), MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate), "");
                                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                               + k1 + "-" + Collection.fldPeacockeryCode.ToString() + "\n");
                                    }
                                    SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                                    SmsSender sms = new SmsSender();
                                 //   sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, Collection.fldPrice.ToString(), "", "", "");
                                    return Json(new
                                    {
                                        Msg = "ذخیره با موفقیت انجام شد.",
                                        MsgTitle = "ذخیره موفق",
                                        Err = 0
                                    }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        Msg = "فیش فعلی قبلا در سیستم ثبت گردیده است.",
                                        MsgTitle = "خطا",
                                        Err = 1
                                    }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                                return Json(new
                                {
                                    Msg = "فیش فعلی مربوط به این پرونده نمی باشد.",
                                    MsgTitle = "خطا",
                                    Err = 1
                                }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new
                            {
                                Msg = "فیش فعلی در سیستم وجود ندارد.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);

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
                                if (Convert.ToInt32(Session["UserMnu"]) == 1)
                                {
                                    Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                                    toRevenueSystems.VitiationRecovrySendedFiche(3, 1, Collection.fldPeacockeryCode.ToString());
                                    var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, Collection.fldPeacockeryCode.ToString(), MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate), "");
                                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                           + k1 + "-" + Collection.fldPeacockeryCode.ToString() + "\n");
                                }
                                return Json(new
                                {
                                    Msg = "ویرایش با موفقیت انجام شد.",
                                    MsgTitle = "ویرایش موفق",
                                    Err = 0
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                                return Json(new
                                {
                                    Msg = "فیش فعلی مربوط به این پرونده نمی باشد.",
                                    MsgTitle = "خطا",
                                    Err = 1
                                }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new
                            {
                                Msg = "فیش فعلی در سیستم وجود ندارد.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);

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
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Err = 1, MsgTitle="خطا" });
            }
        }        
    }
}
