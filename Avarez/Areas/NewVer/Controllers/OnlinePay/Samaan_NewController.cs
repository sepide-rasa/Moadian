using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.NewVer.Controllers.OnlinePay
{
    public class Samaan_NewController : Controller
    {
        //
        // GET: /NewVer/Samaan_New/

        public ActionResult Index()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.sp_BankParameterSelect("fldBankID", "31", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id = 0;
            var id_url = 0;
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "MID")
                {
                    id = item.fldID;
                }
                else if (item.fldPropertyNameEN == "RedirectURL")
                {
                    id_url = item.fldID;
                }
            }
            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
               Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 31).FirstOrDefault();
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id).FirstOrDefault();
            var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_url).FirstOrDefault();

            var url = q2.fldValue + "/NewVer/Samaan_New/Back";
            //Session["BankUserName"] = q3.fldValue;
            //Session["BankPass"] = q4.fldValue;
            Session["TerminalId"] = q1.fldValue;
            Session["RedirectUrl"] = url;
            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"Samaan\" + Session["Tax"].ToString(),
                    Session["Tax"].ToString() + "," + Session["TerminalId"] + "," + Session["UserId"] + ","
                    + Session["ReturnUrl"] + "," + Session["UserPass"] + "," + Session["UserMnu"] + "," +
                    Session["UserState"] + "," + Session["CountryType"] + "," + Session["CountryCode"]
                    + "," + Session["area"] + "," + Session["office"] + "," + Session["Location"] + ","
                    + Session["GeustId"] + "," + Session["OnlinefishId"]);
            return PartialView;
        }
        [HttpPost]
        public ActionResult Back(string State, string StateCode, string ResNum, string MID, string RefNum, string CID, string TRACENO,string SecurePan)
        {

            var ss = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Samaan\" + ResNum);
            var ss1 = ss.Split(',');
            Session["Tax"] = ss1[0];
            Session["TerminalId"] = ss1[1];
            Session["UserId"] = ss1[2];
            Session["ReturnUrl"] = ss1[3];
            Session["UserPass"] = ss1[4];
            Session["UserMnu"] = ss1[5];
            Session["UserState"] = ss1[6];
            Session["CountryType"] = ss1[7];
            Session["CountryCode"] = ss1[8];
            Session["area"] = ss1[9];
            Session["office"] = ss1[10];
            Session["Location"] = ss1[11];
            Session["GeustId"] = ss1[12];
            Session["OnlinefishId"] = ss1[13];
            Models.cartaxEntities car = new Models.cartaxEntities();
            int? userid = null;
            int? guestUserId = null;

            if (Session["UserId"] != null)
                userid = Convert.ToInt32(Session["UserId"]);
            else if (Session["GeustId"] != null)
                userid = Convert.ToInt32(Session["GeustId"]);
            else if (Session["UserGeust"] != null)
                guestUserId = Convert.ToInt32(Session["UserGeust"]);
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\Samaan\" + ResNum);
            var onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", ResNum.ToString(), 0, 1, "").FirstOrDefault();
            car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, false, RefNum, userid, guestUserId, "", "");

            if (State == "OK" && Session["Tax"].ToString() == ResNum)
            {
                try
                {
                    var ReferenceNumber = car.sp_OnlinePaymentsSelect("fldTrackCode", RefNum,0, 1, "").ToList();
                    if (ReferenceNumber.Count == 0)
                    {
                        Saman_New.PaymentIFBinding saman = new Saman_New.PaymentIFBinding();
                        double M = saman.verifyTransaction(RefNum, MID);
                        if (M > 0 && M == Convert.ToDouble(Session["Amount"]))
                        {
                            string InvoiceNamber = TRACENO;
                            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                            car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, true, RefNum, userid, guestUserId, "", TRACENO);
                            car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
                                Convert.ToInt32(onlinepay.fldMony), 10,Convert.ToInt32( Session["OnlinefishId"]), null,
                                "", userid, "کد پیگیری:" + TRACENO + " شماره کارت:" + SecurePan, "", "", null, "", null, null, true, 1, DateTime.Now);
                            SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                            if (Convert.ToInt32(Session["UserMnu"]) == 1)
                            {
                                Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                                var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, onlinepay.fldID.ToString(), DateTime.Now, "");
                                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                       + k1 + "-" + onlinepay.fldID.ToString() + "\n");
                            }
                            decimal money = Convert.ToDecimal(onlinepay.fldMony);
                            ViewBag.InvoiceNamber = InvoiceNamber;
                            ViewBag.Result = "موفق";
                            ViewBag.ResNum = ResNum;
                            ViewBag.RefNum = RefNum;
                            ViewBag.Bank = "بانک سامان";
                            ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
                            ViewBag.Money = Convert.ToDouble(money).ToString("#,###") + " ريال";
                            Session["ResidId"] = _id.Value;
                            //if (Session["IsOfficeUser"] != null)
                            //{
                            //    epishkhan.devpishkhannwsv1 epishkhan = new epishkhan.devpishkhannwsv1();
                            //    var shpeigiri = epishkhan.verify("atJ5+$J1RtFpj", Convert.ToInt32(Session["serviceCallSerial"]), _id.Value.ToString());
                            //    //تابع آپدیت جدول پرداخت                        
                            //    //car.sp_CollectionUpdatePishkhan(Convert.ToInt64(_id.Value), 1, "", shpeigiri.ToString());
                            //}
                            SmsSender sms = new SmsSender();
                            sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, onlinepay.fldCarFileID, (int)onlinepay.fldMony, "", "", "");
                            ViewBag.Result = "پرداخت شما با موفقیت انجام شد.";
                        }
                        else if (M < 0)
                        {
                            ViewBag.Result = "ناموفق (کد خطا:)" + M.ToString();
                        }
                        else if (M > 0 && M != Convert.ToDouble(Session["Amount"]))
                        {
                            var q = car.sp_BankParameterSelect("fldBankID", "31", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
                            int PassId = 0;
                            foreach (var item in q)
                            {
                                if (item.fldPropertyNameEN == "Password")
                                {
                                    PassId = item.fldID;
                                }
                            }
                            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
                                Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 31).FirstOrDefault();
                            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == PassId).FirstOrDefault();
                            saman.reverseTransaction(RefNum, MID, MID, q1.fldValue);
                            ViewBag.Result = "تراکنش ناموفق؛ مبلغ به کارت خریدار بازگردانده شد.";
                        }
                    }
                    else
                    {
                        ViewBag.Result = "ناموفق (تکراری بودن رسید دیجیتالی)";
                    }
                }
                catch (Exception x)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    string InnerException = "";
                    if (x.InnerException != null)
                        InnerException = x.InnerException.Message;
                    car.sp_ErrorProgramInsert(Eid, InnerException, 1, x.Message, DateTime.Now,"");
                    ViewBag.Result = "ناموفق (شماره خطا:)" + Eid.Value;
                }
            }
            else
            {
                ViewBag.Result = "ناموفق (کد خطا:)" + State;
            }
            return View();
        }
    }
}
