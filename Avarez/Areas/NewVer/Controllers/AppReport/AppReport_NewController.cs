using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
//using Microsoft.Reporting.WebForms;
using System.Xml;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.Configuration;
using Aspose.Cells;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Text;

namespace Avarez.Areas.NewVer.Controllers.AppReport
{
    public class AppReport_NewController : Controller
    {
        //
        // GET: /NewVer/AppReport_New/


        public ActionResult FromDateToDate(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->فیش های صادره و پرداخت شده");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            if (Session["UserMnu"].ToString() == "612")
            {
                result.ViewBag.StartDate = DateTime.Now.Date.AddDays(-30);
            }
            else
            {
                result.ViewBag.StartDate = "0";
            }
            return result;
        }

        public ActionResult INDate(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->تعداد مفاصاهای صادر شده در روز");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            Models.cartaxEntities m = new Models.cartaxEntities();
            var datee = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.datee = datee;
            return result;
        }

        public ActionResult FromDateToDateScan(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->پرونده های اسکن شده");
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

        public ActionResult GetInfoTrn()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->پیگیری تراکنش ناموفق بانک سامان");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult();
            return result;
        }

        public ActionResult UnsuccessfulPayments(string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->پیگیری پرداخت های pose");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.CarFileId = CarFileId;
            return result;
        }

        public ActionResult UnsuccessfulReciept(string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->پیگیری رسیدهای pose");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.CarFileId = CarFileId;
            return result;
        }

        public JsonResult PishkhanPosVerify(string Trace, string TermId, string RRN, int PishkhanPosId, int price)
        {
            if (Session["UserId"] == null)
                return null;
            epishkhan_pos.devpishkhanposws pishkhan = new epishkhan_pos.devpishkhanposws();
            Models.cartaxEntities p = new Models.cartaxEntities();

            if (Trace.Length < 6)
            {
                Trace = Trace.PadLeft('0', 6);
            }

            var q = p.sp_PishkhanPosSelect("fldid", PishkhanPosId.ToString()).FirstOrDefault();
            
            string Msg = "", MsgTitle = "", MsgType = "";
            if(q.fldPrice!=price)
                return Json(new { Msg = "مبلغ وارد شده معتبر نیست.", MsgTitle = "خطا", MsgType = "0" }, JsonRequestBehavior.AllowGet);
            var res = pishkhan.verify("7sFi[8r-${R5{B", "autotax4", Session["ver_code"].ToString(),
                    Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ServiceId"]),
                    q.fldSerial, PishkhanPosId.ToString(), Trace, TermId, RRN);
            int a;
            int.TryParse(res, out a);
            if (a > 0)
            {
                p.sp_PishkhanPosVerify(PishkhanPosId, 3, TermId, Trace, RRN, Convert.ToInt32(Session["UserId"]), "کد رهگیری: " + a);
                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                var fish = p.sp_PeacockerySelect("fldid", q.fldFishId.ToString(), 0, 1, "").FirstOrDefault();
                var date = p.sp_GetDate().FirstOrDefault().CurrentDateTime;
                p.sp_CollectionInsert(_id, fish.fldCarFileID, date, q.fldPrice,
                    9, (int)q.fldFishId, null, "", Convert.ToInt32(Session["UserId"]),
                    "پرداخت از طریق pc-pos پیشخوان دولت", "", a.ToString(), null, "", null, null, true, 1, date);
                SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                Msg = "عملیات با موفقیت انجام شد.";
                MsgTitle = "عملیات موفق";
                MsgType = "1";
            }
            else
            {
                switch (a)
                {
                    case -44:
                        Msg = "امکان تایید نیست، تراکنش هنوز پرداخت نشده است.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -45:
                        Msg = "خطای پایگاه داده در زمان تایید تراکنش کسر از اعتبار";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -46:
                        Msg = "خطای پایگاه داده در زمان تایید تراکنش کسر از کارت";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -50:
                        Msg = "نحوه پرداخت این سرویس تعریف نشده است";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -51:
                        Msg = "تراکنش پرداخت یافت نشد.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -58:
                        Msg = "خطای اتصال به سرویس تایید تراکنش.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -59:
                        Msg = "خطای دریافت اطلاعات از سرویس تایید تراکنش.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -60:
                        Msg = "تراکنش مورد تایید نیست.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                }
            }
            return Json(new { Msg = Msg, MsgTitle = MsgTitle, MsgType = MsgType }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PishkhanResiptVerify(string Trace, string TermId, string RRN, int PishkhanRecipetId, int price)
        {
            if (Session["UserId"] == null)
                return null;
            epishkhan_pos.devpishkhanposws pishkhan = new epishkhan_pos.devpishkhanposws();
            Models.cartaxEntities p = new Models.cartaxEntities();
            if (Trace.Length < 6)
            {
                Trace = Trace.PadLeft('0', 6);
            }
            var q = p.Sp_tblPishkhanResiptSelect("fldid", PishkhanRecipetId.ToString()).FirstOrDefault();
            
            string Msg = "", MsgTitle = "", MsgType = "";
            if (q.fldPrice != price)
                return Json(new { Msg = "مبلغ وارد شده معتبر نیست.", MsgTitle = "خطا", MsgType = "0" }, JsonRequestBehavior.AllowGet);
            var res = pishkhan.verify("7sFi[8r-${R5{B", "autotax4", Session["ver_code"].ToString(),
                    Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ResiptServiceId"]),
                    q.fldSerial, q.fldId.ToString(), Trace, TermId, RRN);
            int a;
            int.TryParse(res, out a);
            if (a > 0)
            {
                p.Sp_tblPishkhanResiptVerify(PishkhanRecipetId, "", Convert.ToInt32(Session["UserId"]), 3, TermId, Trace, RRN, a.ToString());
                Msg = "عملیات با موفقیت انجام شد.";
                MsgTitle = "عملیات موفق";
                MsgType = "1";
            }
            else
            {
                switch (a)
                {
                    case -44:
                        Msg = "امکان تایید نیست، تراکنش هنوز پرداخت نشده است.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -45:
                        Msg = "خطای پایگاه داده در زمان تایید تراکنش کسر از اعتبار";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -46:
                        Msg = "خطای پایگاه داده در زمان تایید تراکنش کسر از کارت";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -50:
                        Msg = "نحوه پرداخت این سرویس تعریف نشده است";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -51:
                        Msg = "تراکنش پرداخت یافت نشد.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -58:
                        Msg = "خطای اتصال به سرویس تایید تراکنش.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -59:
                        Msg = "خطای دریافت اطلاعات از سرویس تایید تراکنش.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -60:
                        Msg = "تراکنش مورد تایید نیست.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                }
            }
            return Json(new { Msg = Msg, MsgTitle = MsgTitle, MsgType = MsgType }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInfoPose(int PishkhanPosId,string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.PishkhanPosId = PishkhanPosId;
            result.ViewBag.CarFileId = CarFileId;

            return result;
        }

        public ActionResult GetInfoReciept(int PishkhanRecipetId,string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.PishkhanRecipetId = PishkhanRecipetId;
            result.ViewBag.CarFileId = CarFileId;
            return result;
        }
        //public ActionResult Verify(string RRN, string TermId, string Trace)
        //{
        //    Trace = Trace.PadLeft(6, '0');
        //}
        public ActionResult SaveInCollection(string ShGhabz, string ShPardakht, string coderahgiri, string carFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                Models.cartaxEntities car = new Models.cartaxEntities();

                var fish = car.Sp_PeacockeryShGhabz_Pardakht(ShGhabz, ShPardakht, 1).FirstOrDefault();
                 var desc = ShGhabz + "|" + ShPardakht;
                 var onlinepaymentt = car.sp_OnlinePaymentsSelect("fldPeacockeryID", carFileId.ToString(), 0, 1, "").Where(l => l.fldBankID == 17 && l.fldMony == fish.fldShowMoney&&l.fldFinalPayment==false).ToList();

                if (onlinepaymentt.Count == 1)
                {
                    car.sp_CollectionInsert(_id,Convert.ToInt32(carFileId), DateTime.Now,
                    Convert.ToInt32(fish.fldShowMoney), 10,Convert.ToInt32(fish.fldID)
                    , null, "", Convert.ToInt32(Session["UserId"]), "پرداخت اینترنتی از طریق شناسه قبض و پرداخت و ثبت از قسمت تراکنش ناموفق: کد رهگیری:"
                    + coderahgiri + " و کد پرداخت:" + onlinepaymentt[0].fldID.ToString(), "", "", null, "", null, null, true, 1, DateTime.Now);

                    car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepaymentt[0].fldID, true, coderahgiri, Convert.ToInt32(Session["UserId"]), null, "", "");
                    SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                    return Json(new { MsgTitle = "عملیات موفق", Msg = "اطلاعات با موفقیت در سیستم ثبت گردید.", Er = 0 });
                }
                else if (onlinepaymentt.Count == 0)
                {
                    return Json(new { MsgTitle = "خطا", Msg = "شناسه قبض و پرداخت وارد شده در سیستم موجود نمی باشد.", Er = 1 });
                }
                else if (onlinepaymentt.Count > 1)
                {
                    return Json(new { MsgTitle = "خطا", Msg = "شناسه قبض و پرداخت وارد شده معتبر نیست.", Er = 1 });
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
            return null;
        }

        public ActionResult GetTrnInfo(string ShGhabz, string ShPardakht)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            string Msg = "";
            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.sp_BankParameterSelect("fldBankID", "17", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id_username = 0;
            var id_Pass = 0;
            var saveVariz = true;
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "BankUserName")
                {
                    id_username = item.fldID;
                }
                else if (item.fldPropertyNameEN == "BankPass")
                {
                    id_Pass = item.fldID;
                }
            }
            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
               Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 17).FirstOrDefault();
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_username).FirstOrDefault();
            var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_Pass).FirstOrDefault();

            ServiceReference1.BillStateServiceClient saman = new ServiceReference1.BillStateServiceClient();
            string re = saman.VerifyBillPaymentWithAddData(ShGhabz, ShPardakht, q1.fldValue, q2.fldValue);

            if (re == "-1" || re == "-2" || re == "-3" || re == "-5")
            {
                if (re == "-1")
                {
                    Msg = "قبض پرداخت نشده است.";
                }
                else if (re == "-5")
                {
                    Msg = "خطای سامانه.";
                }
                else if (re == "-3")
                {
                    Msg = "شناسه قبض و یا شناسه پرداخت صحیح نمی باشد.";
                }
                return Json(new { Msg = Msg});
            }
            else
            {
                var CarfileId = 0;
                var fish = car.Sp_PeacockeryShGhabz_Pardakht(ShGhabz, ShPardakht, 1).FirstOrDefault();
                if (fish != null)
                {
                    CarfileId = Convert.ToInt32(fish.fldCarFileID);
                    var varizi = car.sp_CollectionSelect("fldPeacockeryCode", fish.fldID.ToString(), 1, 1, "").FirstOrDefault();
                    if (varizi == null)
                    {
                        saveVariz = false;
                    }
                }
                return Json(new { saveVariz = saveVariz, Msg = Msg, re = re, CarfileId = CarfileId });
            }
        }

        public ActionResult ReadPishkhanPose(string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_SelectPishkhanPos> data= null;
            if (CarFileId == "0")
            {
                data = m.sp_SelectPishkhanPos("", Convert.ToInt32(Session["UserId"]),0).ToList();
            }
            else
            {
                data = m.sp_SelectPishkhanPos("CarFileId", Convert.ToInt32(Session["UserId"]),Convert.ToInt32(CarFileId)).ToList();
            }
            return this.Store(data);
        }

        public ActionResult ReadPishkhanResipt(string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_selectPishkhanResipt> data = null;
            if (CarFileId == "0")
            {
                data = m.sp_selectPishkhanResipt("",Convert.ToInt32(Session["UserId"]),0).ToList();
            }
            else
            {
                data = m.sp_selectPishkhanResipt("CarFileId", Convert.ToInt32(Session["UserId"]), Convert.ToInt32(CarFileId)).ToList();
            }
            return this.Store(data);
        }
        public ActionResult Read(StoreRequestParameters parameters, string AzTarikh, string TaTarikh, string UserId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Models.cartaxEntities m = new Models.cartaxEntities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Avarez.Models.sp_RptCarFile_Scan> data = null;
            if (UserId == "")
            {
                UserId = "0";
            }
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_RptCarFile_Scan> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldShasiNumber":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldShasiNumber";
                            break;
                        case "fldMotorNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMotorNumber";
                            break;
                        case "fldVIN":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldVIN";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOwnerName";
                            break;
                        case "fldPlaqueNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPlaqueNumber";
                            break;
                        case "NameCar":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "NameCar";
                            break;

                    }
                    if (data != null)
                        data1 = m.sp_RptCarFile_Scan(MyLib.Shamsi.Shamsi2miladiDateTime(AzTarikh), MyLib.Shamsi.Shamsi2miladiDateTime(TaTarikh), Convert.ToInt32(UserId), field, searchtext).ToList();
                    else
                        data = m.sp_RptCarFile_Scan(MyLib.Shamsi.Shamsi2miladiDateTime(AzTarikh), MyLib.Shamsi.Shamsi2miladiDateTime(TaTarikh), Convert.ToInt32(UserId), field, searchtext).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_RptCarFile_Scan(MyLib.Shamsi.Shamsi2miladiDateTime(AzTarikh), MyLib.Shamsi.Shamsi2miladiDateTime(TaTarikh), Convert.ToInt32(UserId), "", "").ToList();
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

            List<Avarez.Models.sp_RptCarFile_Scan> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult FromDateToDate_NotPaid(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->فیش های صادره و پرداخت نشده");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            if (Session["UserMnu"].ToString() == "612")
            {
                result.ViewBag.StartDate = DateTime.Now.Date.AddDays(-30);
            }
            else
            {
                result.ViewBag.StartDate = "0";
            }
            return result;
        }

        public ActionResult FromDate(string containerId,string State)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (State == "1")
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->تاریخچه پرداخت ها");
            }
            else{
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->گزارش وصول");
            }
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();

            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            result.ViewBag.State = State;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            if (Session["UserMnu"].ToString() == "612")
            {
                result.ViewBag.StartDate = DateTime.Now.Date.AddDays(-30);
            }
            else
            {
                result.ViewBag.StartDate = "0";
            }
            return result;
        }
        public ActionResult FromYear(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->فیش وصولی به تفکیک کاربران");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var k = car.sp_GetDate().FirstOrDefault().DateShamsi;
            var sal = k.Substring(0, 4);
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.sal = sal;
            return result;
        }

        public ActionResult FromYear_Nerkh(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            else
            {
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                var k = car.sp_GetDate().FirstOrDefault().DateShamsi;
                var sal = k.Substring(0, 4);
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo

                };
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                result.ViewBag.sal = sal;


                return result;
            }
        }

        public ActionResult FromUser(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->کاربرهای تعریف شده");
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
        public ActionResult NodeLoadTreeCountry(string nod)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (nod == "0" || nod == null)
            {
                var child = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldNodeName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.IconFile = url + ch.fldNodeType + ".png";
                    childNode.DataPath = ch.fldNodeType.ToString();
                    childNode.Cls = ch.fldSourceID.ToString();
                    nodes.Add(childNode);
                }
            }
            else
            {

                var child = p.sp_TableTreeSelect("fldPId", nod, 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();
                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldNodeName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.IconFile = url + ch.fldNodeType + ".png";
                    childNode.DataPath = ch.fldNodeType.ToString();
                    childNode.Cls = ch.fldSourceID.ToString();
                    nodes.Add(childNode);
                }

            }
            return this.Direct(nodes);
        }
        public ActionResult CountryPosition(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var nodes = car.sp_SelectUpTreeCountryDivisions(id, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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
        public ActionResult LoadPath(string Path)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            List<string> a = Path.Split('/').Skip(1).Skip(1).ToList();
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var node = new Ext.Net.Node();
            var node2 = new Ext.Net.Node();

            string url = Url.Content("~/Content/images/");
            int m = 0;
            for (var i = 0; i < a.Count - 1; i++)
            {
                var p = new Models.cartaxEntities();
                var child = p.sp_TableTreeSelect("fldPId", a[i].ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9).ToList();
                if (i == 0)
                {
                    for (int j = 0; j < child.Count; j++)
                    {
                        Node childNode = new Node();
                        if (child[j].fldID == Convert.ToInt32(a[i + 1])) { node = childNode; }
                        childNode.Text = child[j].fldNodeName;
                        childNode.NodeID = child[j].fldID.ToString();
                        childNode.IconFile = url + child[j].fldNodeType + ".png";
                        childNode.DataPath = child[j].fldNodeType.ToString();
                        childNode.Cls = child[j].fldSourceID.ToString();
                        nodes.Add(childNode);
                    }
                }
                else
                {
                    for (int j = 0; j < child.Count; j++)
                    {
                        Node childNode = new Node();
                        if (child[j].fldID == Convert.ToInt32(a[i + 1])) { node2 = childNode; }
                        childNode.Text = child[j].fldNodeName;
                        childNode.NodeID = child[j].fldID.ToString();
                        childNode.IconFile = url + child[j].fldNodeType + ".png";
                        childNode.DataPath = child[j].fldNodeType.ToString();
                        childNode.Cls = child[j].fldSourceID.ToString();
                        node.Children.Add(childNode);
                    };
                    node = node2;
                }
            }
            return this.Direct(nodes);
        }
        public JsonResult GetYear()
        {
            if (Session["UserId"] == null)
                return null;
            List<SelectListItem> sal = new List<SelectListItem>();

            for (int i = 1390; i <= 1405; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString();
                item.Value = i.ToString();
                sal.Add(item);
            }

            return Json(sal.OrderByDescending(k => k.Text).Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUsers()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q = car.sp_SelectCountryDivTemp(5, Convert.ToInt32(Session["UserMnu"])).FirstOrDefault();
            var tree = car.sp_SelectDownTreeCountryDivisions(q.fldID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString())
                .Where(k => k.fldNodeType == 9).ToList();
            tree.Add(new Models.sp_SelectDownTreeCountryDivisions { fldSourceID = 20, fldNodeName = "کاربر اینترنتی", fldID = 200000 });
            return Json(tree.Select(c => new { fldID = c.fldSourceID, fldName = c.fldNodeName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSettleType()
        {
            if (Session["UserId"] == null)
                return null;
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_SettleTypeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Tree(int? id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            //پروسیجر تشخیص کاربر در تقسیمات کشوری برای
            //لود درخت همان ناحیه ای کاربر در سطح آن تعریف شده 
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (id != null)
            {
                var rols = (from k in p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0).Any()
                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var rols = (from k in p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }

        }

        /*   public ActionResult IDCountryDivisions(string Code, string NType)
           {
               Models.cartaxEntities m = new Models.cartaxEntities();
               var IdCountryDivisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(NType), Convert.ToInt32(Code)).FirstOrDefault();
               return Json(new { IdCountryDivisions = IdCountryDivisions.CountryDivisionId }, JsonRequestBehavior.AllowGet);
           }*/
        public ActionResult GeneratePDFPlaqueSerial()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_PlaqueSerialSelectTableAdapter sp_plaqueserial = new Avarez.DataSet.DataSet1TableAdapters.sp_PlaqueSerialSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                sp_plaqueserial.Fill(dt.sp_PlaqueSerialSelect, "", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_PlaqueSerial.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet1");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.RegisterData(dt, "DataSet1");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintPlaqueSerial(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->سریال پلاک");
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

        public ActionResult GeneratePDFColorCar()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_ColorCarSelectTableAdapter sp_carcolor = new Avarez.DataSet.DataSet1TableAdapters.sp_ColorCarSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                sp_carcolor.Fill(dt.sp_ColorCarSelect, "", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_CarColor.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet1");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());

                Report.RegisterData(dt, "DataSet1");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintColorCar(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->رنگ خودرو");
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

        public string calcu(long carid)
        {
            if (Session["UserId"] == null)
                return "";
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
            var DateTime = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
            var car = m.sp_CarFileSelect("fldCarId", carid.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime).Substring(0, 4));
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";
            //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                Transaction Tr = new Transaction();
                var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["UserId"]));
            }
            //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
            //      null, DateTime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
            var bedehi = m.prs_newCarFileCalc(DateTime, Convert.ToInt32(Session["CountryType"]),
               Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"])).Where(k => k.fldCollectionId == 0).ToList();
            string _year = "";
            if (bedehi != null)
            {
                var nullYears = bedehi.Where(k => k.fldPrice == null).ToList();
                foreach (var item in nullYears)
                {
                    _year += item.fldYear;
                }
            }
            if (_year.ToString() == "")
            {
                int? mablagh = 0;
                int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0, fldMainDiscount = 0, fldFineDiscount = 0,
                    fldValueAddDiscount = 0, fldOtherDiscount = 0;
                ArrayList Years = new ArrayList();
                DataSet.DataSet1.sp_jCalcCarFileDataTable a = new DataSet.DataSet1.sp_jCalcCarFileDataTable();

                foreach (var item in bedehi)
                {
                    int? jam = (item.fldFinalPrice + item.fldMashmol + item.fldNoMashmol + item.fldMablaghJarime + item.fldOtherPrice) -
                        (item.fldDiscontJarimePrice + item.fldDiscontMoaserPrice + item.fldDiscontOtherPrice + item.fldDiscontValueAddPrice);
                    mablagh += jam;
                    fldFine += (int)item.fldMablaghJarime;
                    fldValueAddPrice += (int)item.fldValueAdded;
                    fldPrice += (int)((item.fldFinalPrice - item.fldValueAdded) + item.fldMashmol + item.fldNoMashmol);
                    Years.Add(item.fldYear);
                    fldOtherPrice += (int)item.fldOtherPrice;
                    fldMainDiscount += (int)item.fldDiscontMoaserPrice;
                    fldFineDiscount += (int)item.fldDiscontJarimePrice;
                    fldValueAddDiscount += (int)item.fldDiscontValueAddPrice;
                    fldOtherDiscount += (int)item.fldDiscontOtherPrice;
                    a.Addsp_jCalcCarFileRow((int)item.fldYear, (int)item.fldPrice, (int)item.fldMablaghMoaser, (int)item.fldValueAdded, (int)item.fldFinalPrice,
                       (int)item.fldMablaghJarime, (int)item.fldTedadJarime, (int)item.fldDiscontMoaserPrice, (int)jam, item.fldCalcDate, (int)item.fldOtherPrice, (int)item.fldDiscontValueAddPrice,
                       (int)item.fldDiscontJarimePrice, (int)item.fldDiscontOtherPrice);
                }

                int sal = 0, mah = 0;
                //mablagh += Convert.ToInt32(_Bed.Value);
                //fldPrice += Convert.ToInt32(_Bed.Value);
                // Session["mablagh"] = mablagh;
                //Session["Fine"] = fldFine;
                // Session["ValueAddPrice"] = fldValueAddPrice;
                //Session["Price"] = fldPrice;
                Session["Year"] = Years;
                //Session["Bed"] = Convert.ToInt32(_Bed.Value);
                Session["OtherPrice"] = fldOtherPrice;
                Session["fldMainDiscount"] = fldMainDiscount;
                Session["fldFineDiscount"] = fldFineDiscount;
                Session["fldValueAddDiscount"] = fldValueAddDiscount;
                Session["fldOtherDiscount"] = fldOtherDiscount;
                Session["Joziyat"] = a;
                if (mablagh < 1000)
                {
                    mablagh = 0;
                    //bedehi = null;
                }

                string shGhabz = "", ShParvande = "",
                    shPardakht = "",
                    barcode = "";
                //return Json(new
                //{
                //    bedehi = bedehi,
                //    mablagh = mablagh,
                //    shGhabz = shGhabz,
                //    shPardakht = shPardakht,
                //    barcode = barcode,
                //    msg = "",

                //    fldFine = fldFine,
                //    fldValueAddPrice = fldValueAddPrice,
                //    fldPrice = fldPrice,
                //    Years = Years,
                //    Bed = Convert.ToInt32(_Bed.Value),
                //    fldOtherPrice = fldOtherPrice,
                //    fldMainDiscount = fldMainDiscount,
                //    fldFineDiscount = fldFineDiscount,
                //    fldValueAddDiscount = fldValueAddDiscount,
                //    fldOtherDiscount = fldOtherDiscount//,
                //    // Joziyat=a
                //}, JsonRequestBehavior.AllowGet);
                return mablagh.ToString();
            }
            return "";
        }


        public ActionResult GeneratePDFSooratVaziyat(long carId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter sp_SelectCarDetils = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter sp_Receipt = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter sp_CarExperience = new Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.Sp_AcceptStatusTableAdapter sp_AcceptStatus = new Avarez.DataSet.DataSet1TableAdapters.Sp_AcceptStatusTableAdapter();
                var Accept = m.sp_SelectCarDetils(carId).FirstOrDefault();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                sp_SelectCarDetils.Fill(dt.sp_SelectCarDetils, carId);
                sp_Receipt.Fill(dt.rpt_Receipt, carId, 2);
                sp_AcceptStatus.Fill(dt.Sp_AcceptStatus, Convert.ToInt32(Accept.fldID));
                sp_CarExperience.Fill(dt.sp_CarExperienceSelect, "fldCarID", carId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_SooratVaziyat.frx");
                Report.RegisterData(dt, "carTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                string Mablagh = calcu(carId);
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("Mablagh", Mablagh);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());

                Report.RegisterData(dt, "DataSet1");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintSooratVaziyat(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }




        public ActionResult GeneratePDFCarSystem()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_CarSystemSelectTableAdapter rpt_carsystem = new Avarez.DataSet.DataSet1TableAdapters.sp_CarSystemSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                rpt_carsystem.Fill(dt.sp_CarSystemSelect, "", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_CarSystem.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet1");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());

                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrintCarSystem(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->سیستم خودرو");
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
        public ActionResult GeneratePDFCarSystemCom()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_SelectFullCarForSetMonyTableAdapter sp_carsyscom = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectFullCarForSetMonyTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                sp_carsyscom.Fill(dt.sp_SelectFullCarForSetMony, 0, 0, true, "1392", 0, 0);
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_FullSystemCar.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet1");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());

                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult PrintCarSystemCom(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->سیستم های کامل خودرو");
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
        public ActionResult GeneratePDFOrganization()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_OrganizationSelectTableAdapter sp_Organ = new Avarez.DataSet.DataSet1TableAdapters.sp_OrganizationSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                sp_Organ.Fill(dt.sp_OrganizationSelect, "", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Organization.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());


                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintOrganization(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->سازمان ها");
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
        public ActionResult GeneratePDFCost()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_CostSelectTableAdapter sp_cost = new Avarez.DataSet.DataSet1TableAdapters.sp_CostSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                sp_cost.Fill(dt.sp_CostSelect, "", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Cost.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet");
                Report.Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());


                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintCost(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->هزینه ها");
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
        public ActionResult GeneratePDFPlaqueType()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_PlaqueTypeSelectTableAdapter sp_PlaqueT = new Avarez.DataSet.DataSet1TableAdapters.sp_PlaqueTypeSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                sp_PlaqueT.Fill(dt.sp_PlaqueTypeSelect, "", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_PlaqueType.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet1");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());


                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintPlaqueType(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->انواع پلاک");
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
        public ActionResult GeneratePDFStatusPlaque()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_StatusPlaqueSelectTableAdapter StatusPlaque = new Avarez.DataSet.DataSet1TableAdapters.sp_StatusPlaqueSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                StatusPlaque.Fill(dt.sp_StatusPlaqueSelect, "", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_PlaqueStatus.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet1");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());


                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintStatusPlaque(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->انواع وضعیت پلاک");
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
        public ActionResult GeneratePDFCarFile()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_CarFileSelectTableAdapter carfile = new Avarez.DataSet.DataSet1TableAdapters.sp_CarFileSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                carfile.Fill(dt.sp_CarFileSelect, "", "", 200, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\CarFile.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());


                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintCarFile(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->پرونده های خودرو");
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
        public ActionResult GeneratePDFOwner()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_OwnerSelectTableAdapter owner = new Avarez.DataSet.DataSet1TableAdapters.sp_OwnerSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                owner.Fill(dt.sp_OwnerSelect, "", "", 200, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Owner.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());


                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintOwner(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->مالکان خودرو");
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
        public ActionResult GeneratePDFCharacterPersianPlaque()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_CharacterPersianPlaqueSelectTableAdapter character = new Avarez.DataSet.DataSet1TableAdapters.sp_CharacterPersianPlaqueSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                character.Fill(dt.sp_CharacterPersianPlaqueSelect, "", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\CharacterPersianPlaque.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());


                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintCharacterPersianPlaque(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->کاراکترهای مجاز پلاک ملی");
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
        public ActionResult GeneratePDFCarPlaque()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_CarPlaqueSelectTableAdapter carplaque = new Avarez.DataSet.DataSet1TableAdapters.sp_CarPlaqueSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                carplaque.Fill(dt.sp_CarPlaqueSelect, "", "", 200, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Plaque.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());

                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintCarPlaque(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->شماره پلاک ها");
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



        public ActionResult PrintComplicationsRate(string containerId, int Sal)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->نرخ عوارض سالیانه");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Sal = Sal;
            return result;
        }
        public ActionResult GeneratePDFComplicationsRate(int Sal)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_ComplicationsRateSelectTableAdapter complication = new Avarez.DataSet.DataSet1TableAdapters.sp_ComplicationsRateSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                complication.Fill(dt.sp_ComplicationsRateSelect, "fldYear", Sal.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\ComplicationsRate.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("Year", Sal.ToString());
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());

                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);


                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrintuserCount(string containerId, int Sal)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Sal = Sal;
            return result;
        }
        public ActionResult GeneratePDFuserCount(int Sal)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountTableAdapter MonthlyChart = new Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountTableAdapter();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


                MonthlyChart.Fill(dt.sp_RptMonthlyUser_Count, Convert.ToInt32(Sal), Convert.ToInt32(Session["UserMnu"]));
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMonthlyUser.frx");
                Report.RegisterData(dt, "CarTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("sal", Sal.ToString());
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());


                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);


                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UserCount_Date(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->فیش وصولی به تفکیک کاربران(بازه زمانی)");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {

                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            if (Session["UserMnu"].ToString() == "612")
            {
                result.ViewBag.StartDate = DateTime.Now.Date.AddDays(-30);
            }
            else
            {
                result.ViewBag.StartDate = "0";
            }
            return result;
        }

        public ActionResult PrintuserCount_Date(string containerId, string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.SDate = SDate;
            result.ViewBag.EDate = EDate;
            return result;
        }
        public ActionResult GeneratePDFuserCount_Date(string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountWithDateTableAdapter MonthlyChart = new Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountWithDateTableAdapter();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


                MonthlyChart.Fill(dt.sp_RptMonthlyUser_CountWithDate, MyLib.Shamsi.Shamsi2miladiDateTime(SDate),
                    MyLib.Shamsi.Shamsi2miladiDateTime(EDate), Convert.ToInt32(Session["UserMnu"]));
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMonthlyUser_date.frx");
                Report.RegisterData(dt, "CarTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("sal", "از تاریخ: " + SDate + " تا تاریخ:" + EDate);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());


                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);


                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintuserCount_Date_notpay(string containerId, string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.SDate = SDate;
            result.ViewBag.EDate = EDate;
            return result;
        }
        public ActionResult rptuserCount_Date_notpay(string startDate, string EndDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account", new { area = "NewVer" });

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountWithDate_NotPayTableAdapter MonthlyChart = new Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountWithDate_NotPayTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


            MonthlyChart.Fill(dt.sp_RptMonthlyUser_CountWithDate_NotPay, MyLib.Shamsi.Shamsi2miladiDateTime(startDate),
                MyLib.Shamsi.Shamsi2miladiDateTime(EndDate), Convert.ToInt32(Session["UserMnu"]));
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMonthlyUser_date_notpay.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("sal", "از تاریخ: " + startDate + " تا تاریخ:" + EndDate);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("OfficeName", Session["office"].ToString());

            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")
            {
                Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
            }
            else if (ImageSetting == "2")
            {
                Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
            }
            else
                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");

        }
        public ActionResult PrintPaid(string containerId, string SDate, string EDate, string User)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.SDate = SDate;
            result.ViewBag.EDate = EDate;
            result.ViewBag.User = User;
            return result;
        }
        public ActionResult GeneratePDFPaid(string SDate, string EDate, string User)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (User == "null")
                User = "0";
            try
            {
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                var DateTime = car.sp_GetDate().FirstOrDefault().CurrentDateTime;
                FastReport.Report Report = new FastReport.Report();
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptPeacockery2TableAdapter Peacockery = new Avarez.DataSet.DataSet1TableAdapters.sp_RptPeacockery2TableAdapter();

                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Peacockery.Fill(dt.sp_RptPeacockery2, "Paid", Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate.ToString()), MyLib.Shamsi.Shamsi2miladiDateTime(EDate.ToString()), Convert.ToInt32(User));

                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_Paid.frx");
                Report.RegisterData(dt, "carTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());

                Report.SetParameterValue("TitleGozaresh", "فیش های صادر شده و پرداخت شده");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);

                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                //var Msg = x.Message;
                //if (x.InnerException != null)
                //{
                //    Msg = x.InnerException.Message;
                //}
                //return Json(new { Msg = Msg }, JsonRequestBehavior.AllowGet);
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult PrintNotPaid(string containerId, string SDate, string EDate, string User)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.SDate = SDate;
            result.ViewBag.EDate = EDate;
            result.ViewBag.User = User;
            return result;
        }
        public ActionResult GeneratePDFNotPaid(string SDate, string EDate, string User)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (User == "null")
                User = "0";
            try
            {
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                var DateTime = car.sp_GetDate().FirstOrDefault().CurrentDateTime;

                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptPeacockery2TableAdapter Peacockery = new Avarez.DataSet.DataSet1TableAdapters.sp_RptPeacockery2TableAdapter();

                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Peacockery.Fill(dt.sp_RptPeacockery2 , "NotPaid", Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate.ToString()), MyLib.Shamsi.Shamsi2miladiDateTime(EDate.ToString()), Convert.ToInt32(User));

                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_NotPaid.frx");
                /*Report.RegisterData(dt, "CarTaxDataSet");*/
                Report.RegisterData(dt, "carTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());

                Report.SetParameterValue("TitleGozaresh", "فیش های صادر شده و پرداخت نشده");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);


                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Collection(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->جدول وصول");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            if (Session["UserMnu"].ToString() == "612")
            {
                result.ViewBag.StartDate = DateTime.Now.Date.AddDays(-30);
            }
            else
            {
                result.ViewBag.StartDate = "0";
            }
            return result;
        }

        public ActionResult PrintCollection(string containerId, string SDate, string EDate, int ReportType, int treeid, string SettleTypeId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.SDate = SDate;
            result.ViewBag.EDate = EDate;
            result.ViewBag.ReportType = ReportType;
            result.ViewBag.treeid = treeid;
            result.ViewBag.SettleTypeId = SettleTypeId;
            return result;
        }
        public ActionResult GeneratePDFCollection(string SDate, string EDate, int ReportType, int treeid, string SettleTypeId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {

                if (SettleTypeId == "null")
                    SettleTypeId = 0.ToString();
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                dt.EnforceConstraints = false;
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptCollectionTableAdapter Collection = new Avarez.DataSet.DataSet1TableAdapters.sp_RptCollectionTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                Collection.Fill(dt.sp_RptCollection, Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), ReportType, treeid, Convert.ToInt32(SettleTypeId));
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rptCollection.frx");
                Report.RegisterData(dt, "CarTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("AzTarikh", SDate);
                Report.SetParameterValue("TaTarikh", EDate);
                Report.SetParameterValue("SettleTypeId", SettleTypeId);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadPDFCollection(string SDate, string EDate, int ReportType, int treeid, string SettleTypeId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                if (SettleTypeId == "null")
                    SettleTypeId = 0.ToString();
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                dt.EnforceConstraints = false;
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptCollectionTableAdapter Collection = new Avarez.DataSet.DataSet1TableAdapters.sp_RptCollectionTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                Collection.Fill(dt.sp_RptCollection, Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), ReportType, treeid, Convert.ToInt32(SettleTypeId));
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rptCollection.frx");
                Report.RegisterData(dt, "CarTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("AzTarikh", SDate);
                Report.SetParameterValue("TaTarikh", EDate);
                Report.SetParameterValue("SettleTypeId", SettleTypeId);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), MimeType.Get(".pdf"), "Collection.pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult CollectionExcelTank(string Checked, string SDate, string EDate, int ReportType, int treeid, string SettleTypeId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                if (SettleTypeId == "null")
                    SettleTypeId = 0.ToString();
                // Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();

                //System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                //gv.DataSource = car.sp_RptCollection(Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), ReportType, treeid, Convert.ToInt32(SettleTypeId));
                //gv.DataBind();

                //Response.ClearContent();
                //Response.Buffer = true;
                //Response.AddHeader("content-disposition", "attachment; filename=پیش نمایش.xls");
                //Response.ContentType = "application/ms-excel";
                //Response.Charset = "";
                //StringWriter sw = new StringWriter();
                //HtmlTextWriter htw = new HtmlTextWriter(sw);
                //gv.RenderControl(htw);
                //Response.Output.Write(sw.ToString());
                //Response.Flush();
                //Response.End();
                //return View();


                //--------------------------
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
                int index = 0;
                var StatusCheck = Checked.Split(';');
                var Check = "";
                var fldCarFileID = ""; var fldName = ""; var fldMelli_EconomicCode = ""; var fldMotorNumber = ""; var fldShasiNumber = "";
                var fldCollectionDate = ""; var fldPrice = ""; var fldBankName = ""; var userName = "";var fldPeacockeryId="";var fldFile="";
                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];

                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                foreach (var item in StatusCheck)
                {
                    var coll = car.sp_RptCollection(Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), ReportType, treeid, Convert.ToInt32(SettleTypeId)).ToList(); 
                    switch (item)
                    {
                        case "fldCarFileID":
                            Check = "شماره پرونده";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int i = 0;
                            foreach (var _item in coll)
                            {
                                fldCarFileID = _item.fldCarFileID.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                                Cell.PutValue(fldCarFileID);
                                i++;
                            }
                            index++;
                            break;
                        case "fldName":
                            Check = "نام مالک";
                            Cell cell1 = sheet.Cells[alpha[index] + "1"];
                            cell1.PutValue(Check);
                            int j = 0;
                            foreach (var _item in coll)
                            {
                                fldName = _item.fldName;
                                Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                                Cell.PutValue(fldName);
                                j++;
                            }
                            index++;
                            break;
                        case "fldMelli_EconomicCode":
                            Check = "کدملی";
                            Cell cell2 = sheet.Cells[alpha[index] + "1"];
                            cell2.PutValue(Check);
                            int k = 0;
                            foreach (var _item in coll)
                            {
                                fldMelli_EconomicCode = _item.fldMelli_EconomicCode;
                                Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                                Cell.PutValue(fldMelli_EconomicCode);
                                k++;
                            }
                            index++;
                            break;
                        case "fldMotorNumber":
                            Check = "شماره موتور";
                            Cell cell3 = sheet.Cells[alpha[index] + "1"];
                            cell3.PutValue(Check);
                            int q = 0;
                            foreach (var _item in coll)
                            {
                                fldMotorNumber = _item.fldMotorNumber;
                                Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                                Cell.PutValue(fldMotorNumber);
                                q++;
                            }
                            index++;
                            break;
                        case "fldShasiNumber":
                            Check = "شماره شاسی";
                            Cell cell4 = sheet.Cells[alpha[index] + "1"];
                            cell4.PutValue(Check);
                            int w = 0;
                            foreach (var _item in coll)
                            {
                                fldShasiNumber = _item.fldShasiNumber;
                                Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                                Cell.PutValue(fldShasiNumber);
                                w++;
                            }
                            index++;
                            break;
                        case "fldCollectionDate":
                            Check = "تاریخ پرداخت";
                            Cell cell5 = sheet.Cells[alpha[index] + "1"];
                            cell5.PutValue(Check);
                            int z = 0;
                            foreach (var _item in coll)
                            {
                                fldCollectionDate = _item.fldCollectionDate;
                                Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                                Cell.PutValue(fldCollectionDate);
                                z++;
                            }
                            index++;
                            break;
                        case "fldPrice":
                            Check = "مبلغ";
                            Cell cell6 = sheet.Cells[alpha[index] + "1"];
                            cell6.PutValue(Check);
                            int x = 0;
                            foreach (var _item in coll)
                            {
                                fldPrice = _item.fldPrice.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
                                Cell.PutValue(fldPrice);
                                x++;
                            }
                            index++;
                            break;
                        case "fldBankName":
                            Check = "بانک عامل";
                            Cell cell7 = sheet.Cells[alpha[index] + "1"];
                            cell7.PutValue(Check);
                            int y = 0;
                            foreach (var _item in coll)
                            {
                                fldBankName = _item.fldBankName;
                                Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
                                Cell.PutValue(fldBankName);
                                y++;
                            }
                            index++;
                            break;
                        case "userName":
                            Check = "نام کاربر";
                            Cell cell8 = sheet.Cells[alpha[index] + "1"];
                            cell8.PutValue(Check);
                            int n = 0;
                            foreach (var _item in coll)
                            {
                                userName = _item.username;
                                Cell Cell = sheet.Cells[alpha[index] + (n + 2)];
                                Cell.PutValue(userName);
                                n++;
                            }
                            index++;
                            break;
                        case "fldPeacockeryId":
                            Check = "شماره فیش";
                            Cell cell9 = sheet.Cells[alpha[index] + "1"];
                            cell9.PutValue(Check);
                            int m = 0;
                            foreach (var _item in coll)
                            {
                                fldPeacockeryId = _item.fldPeacockeryId.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (m + 2)];
                                Cell.PutValue(fldPeacockeryId);
                                m++;
                            }
                            index++;
                            break;
                        case "fldFile":
                            Check = "تصویر فیش";
                            Cell cell10 = sheet.Cells[alpha[index] + "1"];
                            cell10.PutValue(Check);
                            int s = 0;
                            foreach (var _item in coll)
                            {
                                Cell Cell = sheet.Cells[alpha[index] + (s + 2)];
                                if (_item.fldFile.Length > 16384)
                                {
                                    int part = _item.fldFile.Length / 16384;
                                    Cell.PutValue(_item.fldFile.Substring(0, 16384));
                                    for (int kk = 0; kk < part; kk++)
                                    {
                                        index = index + 1;
                                        Cell Cell2 = sheet.Cells[alpha[index] + (s + 2)];
                                        if (kk == part - 1)
                                        {
                                            Cell2.PutValue(_item.fldFile.Substring(16384 * (kk + 1)));
                                        }
                                        else
                                        {
                                            Cell2.PutValue(_item.fldFile.Substring(16384 * (kk + 1), 16384));
                                        }
                                    }
                                }
                                else
                                {
                                    Cell.PutValue(_item.fldFile);
                                }
                                s++;
                            }
                            index++;
                            break;
                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "xls", "Collection.xls");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CollectionExcel(string Checked, string SDate, string EDate, int ReportType, int treeid, string SettleTypeId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                if (SettleTypeId == "null")
                    SettleTypeId = 0.ToString();
                // Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();

                //System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                //gv.DataSource = car.sp_RptCollection(Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), ReportType, treeid, Convert.ToInt32(SettleTypeId));
                //gv.DataBind();

                //Response.ClearContent();
                //Response.Buffer = true;
                //Response.AddHeader("content-disposition", "attachment; filename=پیش نمایش.xls");
                //Response.ContentType = "application/ms-excel";
                //Response.Charset = "";
                //StringWriter sw = new StringWriter();
                //HtmlTextWriter htw = new HtmlTextWriter(sw);
                //gv.RenderControl(htw);
                //Response.Output.Write(sw.ToString());
                //Response.Flush();
                //Response.End();
                //return View();


                //--------------------------
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
                int index = 0;
                var StatusCheck = Checked.Split(';');
                var Check = "";
                var fldCarFileID = ""; var fldName = ""; var fldMelli_EconomicCode = ""; var fldMotorNumber = ""; var fldShasiNumber = "";
                var fldCollectionDate = ""; var fldPrice = ""; var fldBankName = ""; var userName = "";
                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];

                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                foreach (var item in StatusCheck)
                {
                    var coll = car.sp_RptCollection(Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), ReportType, treeid, Convert.ToInt32(SettleTypeId)).ToList();
                    switch (item)
                    {
                        case "fldCarFileID":
                            Check = "شماره پرونده";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int i = 0;
                            foreach (var _item in coll)
                            {
                                fldCarFileID = _item.fldCarFileID.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                                Cell.PutValue(fldCarFileID);
                                i++;
                            }
                            index++;
                            break;
                        case "fldName":
                            Check = "نام مالک";
                            Cell cell1 = sheet.Cells[alpha[index] + "1"];
                            cell1.PutValue(Check);
                            int j = 0;
                            foreach (var _item in coll)
                            {
                                fldName = _item.fldName;
                                Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                                Cell.PutValue(fldName);
                                j++;
                            }
                            index++;
                            break;
                        case "fldMelli_EconomicCode":
                            Check = "کدملی";
                            Cell cell2 = sheet.Cells[alpha[index] + "1"];
                            cell2.PutValue(Check);
                            int k = 0;
                            foreach (var _item in coll)
                            {
                                fldMelli_EconomicCode = _item.fldMelli_EconomicCode;
                                Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                                Cell.PutValue(fldMelli_EconomicCode);
                                k++;
                            }
                            index++;
                            break;
                        case "fldMotorNumber":
                            Check = "شماره موتور";
                            Cell cell3 = sheet.Cells[alpha[index] + "1"];
                            cell3.PutValue(Check);
                            int q = 0;
                            foreach (var _item in coll)
                            {
                                fldMotorNumber = _item.fldMotorNumber;
                                Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                                Cell.PutValue(fldMotorNumber);
                                q++;
                            }
                            index++;
                            break;
                        case "fldShasiNumber":
                            Check = "شماره شاسی";
                            Cell cell4 = sheet.Cells[alpha[index] + "1"];
                            cell4.PutValue(Check);
                            int w = 0;
                            foreach (var _item in coll)
                            {
                                fldShasiNumber = _item.fldShasiNumber;
                                Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                                Cell.PutValue(fldShasiNumber);
                                w++;
                            }
                            index++;
                            break;
                        case "fldCollectionDate":
                            Check = "تاریخ پرداخت";
                            Cell cell5 = sheet.Cells[alpha[index] + "1"];
                            cell5.PutValue(Check);
                            int z = 0;
                            foreach (var _item in coll)
                            {
                                fldCollectionDate = _item.fldCollectionDate;
                                Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                                Cell.PutValue(fldCollectionDate);
                                z++;
                            }
                            index++;
                            break;
                        case "fldPrice":
                            Check = "مبلغ";
                            Cell cell6 = sheet.Cells[alpha[index] + "1"];
                            cell6.PutValue(Check);
                            int x = 0;
                            foreach (var _item in coll)
                            {
                                fldPrice = _item.fldPrice.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
                                Cell.PutValue(fldPrice);
                                x++;
                            }
                            index++;
                            break;
                        case "fldBankName":
                            Check = "بانک عامل";
                            Cell cell7 = sheet.Cells[alpha[index] + "1"];
                            cell7.PutValue(Check);
                            int y = 0;
                            foreach (var _item in coll)
                            {
                                fldBankName = _item.fldBankName;
                                Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
                                Cell.PutValue(fldBankName);
                                y++;
                            }
                            index++;
                            break;
                        case "userName":
                            Check = "نام کاربر";
                            Cell cell8 = sheet.Cells[alpha[index] + "1"];
                            cell8.PutValue(Check);
                            int n = 0;
                            foreach (var _item in coll)
                            {
                                userName = _item.username;
                                Cell Cell = sheet.Cells[alpha[index] + (n + 2)];
                                Cell.PutValue(userName);
                                n++;
                            }
                            index++;
                            break;
                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "xls", "Collection.xls");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult PaidExcel(string Checked, string SDate, string EDate, string User)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
                int index = 0;
                var StatusCheck = Checked.Split(';');
                var Check = "";
                var fldID = ""; var fldCarFileID = ""; var fldName = ""; var fldMelli_EconomicCode = ""; var fldMotorNumber = ""; var fldShasiNumber = "";
                var fldIssuanceDate = ""; var fldBedehi = ""; var fldCollectionDate = ""; var fldShowMoney = ""; var fldFromDate = ""; var fldToDate = ""; var userName = "";
                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                if (User == "null")
                    User = "0";
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                foreach (var item in StatusCheck)
                {
                    var fish=car.sp_RptPeacockery2("Paid", Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), Convert.ToInt32(User)).ToList();
                    switch (item)
                    {
                        case "fldID":
                            Check = "شماره فیش";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int m = 0;
                            foreach (var _item in fish)
                            {
                                fldID = _item.fldID.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (m + 2)];
                                Cell.PutValue(fldID);
                                m++;
                            }
                            index++;
                            break;
                        case "fldCarFileID":
                            Check = "شماره پرونده";
                            Cell cell0 = sheet.Cells[alpha[index] + "1"];
                            cell0.PutValue(Check);
                            int i = 0;
                            foreach (var _item in fish)
                            {
                                fldCarFileID = _item.fldCarFileID.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                                Cell.PutValue(fldCarFileID);
                                i++;
                            }
                            index++;
                            break;
                        case "fldName":
                            Check = "نام مالک";
                            Cell cell1 = sheet.Cells[alpha[index] + "1"];
                            cell1.PutValue(Check);
                            int j = 0;
                            foreach (var _item in fish)
                            {
                                fldName = _item.OwnerName;
                                Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                                Cell.PutValue(fldName);
                                j++;
                            }
                            index++;
                            break;
                        case "fldMelli_EconomicCode":
                            Check = "کدملی";
                            Cell cell2 = sheet.Cells[alpha[index] + "1"];
                            cell2.PutValue(Check);
                            int k = 0;
                            foreach (var _item in fish)
                            {
                                fldMelli_EconomicCode = _item.fldMelli_EconomicCode;
                                Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                                Cell.PutValue(fldMelli_EconomicCode);
                                k++;
                            }
                            index++;
                            break;
                        case "fldMotorNumber":
                            Check = "شماره موتور";
                            Cell cell3 = sheet.Cells[alpha[index] + "1"];
                            cell3.PutValue(Check);
                            int q = 0;
                            foreach (var _item in fish)
                            {
                                fldMotorNumber = _item.fldMotorNumber;
                                Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                                Cell.PutValue(fldMotorNumber);
                                q++;
                            }
                            index++;
                            break;
                        case "fldShasiNumber":
                            Check = "شماره شاسی";
                            Cell cell4 = sheet.Cells[alpha[index] + "1"];
                            cell4.PutValue(Check);
                            int w = 0;
                            foreach (var _item in fish)
                            {
                                fldShasiNumber = _item.fldShasiNumber;
                                Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                                Cell.PutValue(fldShasiNumber);
                                w++;
                            }
                            index++;
                            break;
                        case "fldIssuanceDate":
                            Check = "تاریخ صدور";
                            Cell cell5 = sheet.Cells[alpha[index] + "1"];
                            cell5.PutValue(Check);
                            int z = 0;
                            foreach (var _item in fish)
                            {
                                fldIssuanceDate = _item.fldIssuanceDate;
                                Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                                Cell.PutValue(fldIssuanceDate);
                                z++;
                            }
                            index++;
                            break;
                        case "fldCollectionDate":
                            Check = "تاریخ واریزی";
                            Cell cell6 = sheet.Cells[alpha[index] + "1"];
                            cell6.PutValue(Check);
                            int x = 0;
                            foreach (var _item in fish)
                            {
                                fldCollectionDate = _item.fldDateShamsi;
                                Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
                                Cell.PutValue(fldCollectionDate);
                                x++;
                            }
                            index++;
                            break;
                            break;
                        case "fldBedehi":
                            Check = "بدهی/بستانکاری";
                            Cell cell7 = sheet.Cells[alpha[index] + "1"];
                            cell7.PutValue(Check);
                            int y = 0;
                            foreach (var _item in fish)
                            {
                                fldBedehi = _item.fldBedehi.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
                                Cell.PutValue(fldBedehi);
                                y++;
                            }
                            index++;
                            break;
                        case "fldShowMoney":
                            Check = "مبلغ";
                            Cell cell8 = sheet.Cells[alpha[index] + "1"];
                            cell8.PutValue(Check);
                            int n = 0;
                            foreach (var _item in fish)
                            {
                                fldShowMoney = _item.fldShowMoney.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (n + 2)];
                                Cell.PutValue(fldShowMoney);
                                n++;
                            }
                            index++;
                            break;
                        case "fldFromDate":
                            Check = "از تاریخ";
                            Cell cell9 = sheet.Cells[alpha[index] + "1"];
                            cell9.PutValue(Check);
                            int p = 0;
                            foreach (var _item in fish)
                            {
                                fldFromDate = _item.fldFromDate;
                                Cell Cell = sheet.Cells[alpha[index] + (p + 2)];
                                Cell.PutValue(fldFromDate);
                                p++;
                            }
                            index++;
                            break;
                        case "fldToDate":
                            Check = "تا تاریخ";
                            Cell cell10 = sheet.Cells[alpha[index] + "1"];
                            cell10.PutValue(Check);
                            int r = 0;
                            foreach (var _item in fish)
                            {
                                fldToDate = _item.fldToDate;
                                Cell Cell = sheet.Cells[alpha[index] + (r + 2)];
                                Cell.PutValue(fldToDate);
                                r++;
                            }
                            index++;
                            break;
                        case "userName":
                            Check = "نام کاربر";
                            Cell cell11 = sheet.Cells[alpha[index] + "1"];
                            cell11.PutValue(Check);
                            int s = 0;
                            foreach (var _item in fish)
                            {
                                userName = _item.userName;
                                Cell Cell = sheet.Cells[alpha[index] + (s + 2)];
                                Cell.PutValue(userName);
                                s++;
                            }
                            index++;
                            break;
                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "xls", "Peacockery_Paid.xls");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult NotPaidExcel(string Checked, string SDate, string EDate, string User)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
                int index = 0;
                var StatusCheck = Checked.Split(';');
                var Check = "";
                var fldID = ""; var fldCarFileID = ""; var fldName = ""; var fldMelli_EconomicCode = ""; var fldMotorNumber = ""; var fldShasiNumber = "";
                var fldIssuanceDate = ""; var fldShowMoney = ""; var fldFromDate = ""; var fldToDate = ""; var userName = "";
                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                if (User == "null")
                    User = "0";
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                foreach (var item in StatusCheck)
                {
                    var fish = car.sp_RptPeacockery2("NotPaid", Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), Convert.ToInt32(User)).ToList();
                    switch (item)
                    {
                        case "fldID":
                            Check = "شماره فیش";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int m = 0;
                            foreach (var _item in fish)
                            {
                                fldID = _item.fldID.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (m + 2)];
                                Cell.PutValue(fldID);
                                m++;
                            }
                            index++;
                            break;
                        case "fldCarFileID":
                            Check = "شماره پرونده";
                            Cell cell0 = sheet.Cells[alpha[index] + "1"];
                            cell0.PutValue(Check);
                            int i = 0;
                            foreach (var _item in fish)
                            {
                                fldCarFileID = _item.fldCarFileID.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                                Cell.PutValue(fldCarFileID);
                                i++;
                            }
                            index++;
                            break;
                        case "fldName":
                            Check = "نام مالک";
                            Cell cell1 = sheet.Cells[alpha[index] + "1"];
                            cell1.PutValue(Check);
                            int j = 0;
                            foreach (var _item in fish)
                            {
                                fldName = _item.OwnerName;
                                Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                                Cell.PutValue(fldName);
                                j++;
                            }
                            index++;
                            break;
                        case "fldMelli_EconomicCode":
                            Check = "کدملی";
                            Cell cell2 = sheet.Cells[alpha[index] + "1"];
                            cell2.PutValue(Check);
                            int k = 0;
                            foreach (var _item in fish)
                            {
                                fldMelli_EconomicCode = _item.fldMelli_EconomicCode;
                                Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                                Cell.PutValue(fldMelli_EconomicCode);
                                k++;
                            }
                            index++;
                            break;
                        case "fldMotorNumber":
                            Check = "شماره موتور";
                            Cell cell3 = sheet.Cells[alpha[index] + "1"];
                            cell3.PutValue(Check);
                            int q = 0;
                            foreach (var _item in fish)
                            {
                                fldMotorNumber = _item.fldMotorNumber;
                                Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                                Cell.PutValue(fldMotorNumber);
                                q++;
                            }
                            index++;
                            break;
                        case "fldShasiNumber":
                            Check = "شماره شاسی";
                            Cell cell4 = sheet.Cells[alpha[index] + "1"];
                            cell4.PutValue(Check);
                            int w = 0;
                            foreach (var _item in fish)
                            {
                                fldShasiNumber = _item.fldShasiNumber;
                                Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                                Cell.PutValue(fldShasiNumber);
                                w++;
                            }
                            index++;
                            break;
                        case "fldIssuanceDate":
                            Check = "تاریخ صدور";
                            Cell cell5 = sheet.Cells[alpha[index] + "1"];
                            cell5.PutValue(Check);
                            int z = 0;
                            foreach (var _item in fish)
                            {
                                fldIssuanceDate = _item.fldIssuanceDate;
                                Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                                Cell.PutValue(fldIssuanceDate);
                                z++;
                            }
                            index++;
                            break;
                        case "fldShowMoney":
                            Check = "مبلغ";
                            Cell cell8 = sheet.Cells[alpha[index] + "1"];
                            cell8.PutValue(Check);
                            int n = 0;
                            foreach (var _item in fish)
                            {
                                fldShowMoney = _item.fldShowMoney.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (n + 2)];
                                Cell.PutValue(fldShowMoney);
                                n++;
                            }
                            index++;
                            break;
                        case "fldFromDate":
                            Check = "از تاریخ";
                            Cell cell9 = sheet.Cells[alpha[index] + "1"];
                            cell9.PutValue(Check);
                            int p = 0;
                            foreach (var _item in fish)
                            {
                                fldFromDate = _item.fldFromDate;
                                Cell Cell = sheet.Cells[alpha[index] + (p + 2)];
                                Cell.PutValue(fldFromDate);
                                p++;
                            }
                            index++;
                            break;
                        case "fldToDate":
                            Check = "تا تاریخ";
                            Cell cell10 = sheet.Cells[alpha[index] + "1"];
                            cell10.PutValue(Check);
                            int r = 0;
                            foreach (var _item in fish)
                            {
                                fldToDate = _item.fldToDate;
                                Cell Cell = sheet.Cells[alpha[index] + (r + 2)];
                                Cell.PutValue(fldToDate);
                                r++;
                            }
                            index++;
                            break;
                        case "userName":
                            Check = "نام کاربر";
                            Cell cell11 = sheet.Cells[alpha[index] + "1"];
                            cell11.PutValue(Check);
                            int s = 0;
                            foreach (var _item in fish)
                            {
                                userName = _item.userName;
                                Cell Cell = sheet.Cells[alpha[index] + (s + 2)];
                                Cell.PutValue(userName);
                                s++;
                            }
                            index++;
                            break;
                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "xls", "Peacockery_NotPaid.xls");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrintUser(string containerId, string Code, string NType)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Code = Code;
            result.ViewBag.NType = NType;
            return result;
        }

        public ActionResult printAlluser(string containerId)////
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult GeneratePDFUser(string Code, string NType)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                var IdCountryDivisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(NType), Convert.ToInt32(Code)).FirstOrDefault();

                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                dt.EnforceConstraints = false;
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.rpt_UserSelectTableAdapter rpt_user = new Avarez.DataSet.DataSet1TableAdapters.rpt_UserSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                rpt_user.Fill(dt.rpt_UserSelect, "fldCountryDivisionsID", IdCountryDivisions.CountryDivisionId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());

                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\User.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GeneratePDFAllUser()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                dt.EnforceConstraints = false;
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_SelectUserByUserIdTableAdapter rpt_Alluser = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectUserByUserIdTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                rpt_Alluser.Fill(dt.sp_SelectUserByUserId, "", "", 0, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());

                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\UserByUserId.frx");
                Report.RegisterData(dt, "carTax");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult PrintCollectionLog(string containerId, string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.SDate = SDate;
            result.ViewBag.EDate = EDate;

            return result;
        }
        public ActionResult GeneratePDFCollectionLog(string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {

                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_Collection_LogSelectTableAdapter CollectionLog = new DataSet.DataSet1TableAdapters.sp_Collection_LogSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                CollectionLog.Fill(dt.sp_Collection_LogSelect, SDate, EDate, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTabligh = new Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\CollectionLog.frx");
                Report.RegisterData(dt, "CarTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("AzTarikh", SDate);
                Report.SetParameterValue("TaTarikh", EDate);
                //Report.SetParameterValue("MyTablighat", MyTabligh.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTabligh.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrintCollectionByUserId(string containerId, string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.SDate = SDate;
            result.ViewBag.EDate = EDate;
            return result;
        }
        public ActionResult GeneratePDFCollectionByUserId(string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                dt.EnforceConstraints = false;
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptCollectionByUserTableAdapter CollectionByUser = new Avarez.DataSet.DataSet1TableAdapters.sp_RptCollectionByUserTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                CollectionByUser.Fill(dt.sp_RptCollectionByUser, Convert.ToInt32(Session["UserMnu"]), Convert.ToInt32(Session["UserId"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate));
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rptCollectionByUser.frx");
                Report.RegisterData(dt, "CarTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("SettleTypeId", "0");
                Report.SetParameterValue("AzTarikh", SDate);
                Report.SetParameterValue("TaTarikh", EDate);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrintCountMafasa(string containerId, string Tarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Tarikh = Tarikh;
            return result;
        }
        public ActionResult GeneratePDFCountMafasa(string Tarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptCountMafasaTableAdapter CountMafasa = new Avarez.DataSet.DataSet1TableAdapters.sp_RptCountMafasaTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                CountMafasa.Fill(dt.sp_RptCountMafasa, Tarikh);
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptCountMafasa.frx");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());

                Report.RegisterData(dt, "carTaxDataSet");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RptCarExperience_File(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities m = new Models.cartaxEntities();
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->انتقال سوابق ثبت شده در روز");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var date = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.date = date;
            return result;
        }
        public ActionResult PrintCarExperience_File(string containerId, string User, string Tarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.User = User;
            result.ViewBag.Tarikh = Tarikh;
            return result;
        }
        public ActionResult GeneratePDFCarExperience_File(string User, string Tarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (User == "null")
                User = "0";
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptCarExperience_FileTableAdapter CarExperience = new Avarez.DataSet.DataSet1TableAdapters.sp_RptCarExperience_FileTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                CarExperience.Fill(dt.sp_RptCarExperience_File, Convert.ToInt32(User), Tarikh);
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptCarExperience_File.frx");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());

                Report.RegisterData(dt, "carTaxDataSet");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RptLogCarFile(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->تاریخچه ویرایش پرونده");
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
        public ActionResult PrintLogCarFile(string containerId, string User, string AzTarikh, string TaTarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.User = User;
            result.ViewBag.AzTarikh = AzTarikh;
            result.ViewBag.TaTarikh = TaTarikh;
            return result;
        }
        public ActionResult GeneratePDFLogCarFile(string User, string AzTarikh, string TaTarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (User == "null")
                User = "0";
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptLogCarFileTableAdapter RptLogCarFile = new Avarez.DataSet.DataSet1TableAdapters.sp_RptLogCarFileTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                RptLogCarFile.Fill(dt.sp_RptLogCarFile, Convert.ToInt32(User), MyLib.Shamsi.Shamsi2miladiDateTime(AzTarikh), MyLib.Shamsi.Shamsi2miladiDateTime(TaTarikh));
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptLogCarFile.frx");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());

                Report.RegisterData(dt, "carTaxDataSet");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
