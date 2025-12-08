using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Avarez.Controllers.OnlinePay
{
    public class MeliBankController : Controller
    {
        //
        // GET: /MeliBank/

        public WsMeli.MerchantUtility Bmi = new WsMeli.MerchantUtility();        
        public ActionResult Index()
        {
            Session["TimeStamp"] = ViewBag.TimeStamp = TimeStamp();
            Session["FP"] = ViewBag.FP = FP(Convert.ToInt64(Session["Amount"]),
                Convert.ToInt64(Session["Tax"]), ViewBag.TimeStamp);
            Session["Request"] = ViewBag.Request = RequestId(
                Convert.ToInt64(Session["Tax"]), ViewBag.FP, ViewBag.TimeStamp);
            return PartialView();
        }
        public ActionResult WinIndex()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Session["TimeStamp"] = PartialView.ViewBag.TimeStamp = TimeStamp();
            Session["FP"] = PartialView.ViewBag.FP = FP(Convert.ToInt64(Session["Amount"]),
                Convert.ToInt64(Session["Tax"]), PartialView.ViewBag.TimeStamp);
            Session["Request"] = PartialView.ViewBag.Request = RequestId(
                Convert.ToInt64(Session["Tax"]), PartialView.ViewBag.FP, PartialView.ViewBag.TimeStamp);
            return PartialView;
        }
        public string TimeStamp() { return Bmi.CalcTimeStamp(); }
        public string FP(long Amount, long TaxId, string TimeStamp)
        {
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.sp_BankParameterSelect("fldBankID", "1", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id = 0;
            var id_TransactionKey = 0;
            var id_line = 0;
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "MerchantId")
                {
                    id = item.fldID;
                }
                else if (item.fldPropertyNameEN == "TransactionKey")
                {
                    id_TransactionKey = item.fldID;
                }
                else if (item.fldPropertyNameEN == "Line")
                {
                    id_line = item.fldID;
                }
            }
            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
                Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 1).FirstOrDefault();
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_line).FirstOrDefault();
            var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id).FirstOrDefault();
            var q3 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_TransactionKey).FirstOrDefault();
            var MerchantId = q2.fldValue;
            var TransactionKey = q3.fldValue;
            Bmi.Url = string.Format("https://bmiutility{0}.bmi.ir/merchantutility.asmx", q1.fldValue);

            string textInput = string.Concat(MerchantId, TaxId.ToString(), Amount.ToString(), TransactionKey, TimeStamp);
            MD5 hash = new MD5CryptoServiceProvider();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] Input = encoding.GetBytes(textInput);
            byte[] result = hash.ComputeHash(Input);
            return BitConverter.ToString(result);
        }
        public string RequestId(long TaxId, string FP, string TimeStamp)
        {
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.sp_BankParameterSelect("fldBankID", "1", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id = 0;
            var id_TransactionKey = 0;
            var id_line = 0;
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "MerchantId")
                {
                    id = item.fldID;
                }
                else if (item.fldPropertyNameEN == "TransactionKey")
                {
                    id_TransactionKey = item.fldID;
                }
                else if (item.fldPropertyNameEN == "Line")
                {
                    id_line = item.fldID;
                }
            }
            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
               Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 1).FirstOrDefault();
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_line).FirstOrDefault();
            var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id).FirstOrDefault();
            var q3 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_TransactionKey).FirstOrDefault();
            var MerchantId = q2.fldValue;
            var TransactionKey = q3.fldValue;
            Bmi.Url = string.Format("https://bmiutility{0}.bmi.ir/merchantutility.asmx", q1.fldValue);

            string textInput = string.Concat(MerchantId, TaxId.ToString(), FP, TransactionKey);
            MD5 hash = new MD5CryptoServiceProvider();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] Input = encoding.GetBytes(textInput);
            byte[] result = hash.ComputeHash(Input);
            string Rq = TimeStamp + BitConverter.ToString(result);
            return Rq.Replace("-", "").ToLower();
        }
        public ActionResult Back(string OrderId)
        {
            decimal money = 0;
            bool R = false;
            Session.Remove("ResidId");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q = car.sp_BankParameterSelect("fldBankID", "1", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id = 0;
            var id_TransactionKey = 0;
            var id_line = 0;
            var id_TerminalId = 0;
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "MerchantId")
                {
                    id = item.fldID;
                }
                else if (item.fldPropertyNameEN == "TransactionKey")
                {
                    id_TransactionKey = item.fldID;
                }
                else if (item.fldPropertyNameEN == "Line")
                {
                    id_line = item.fldID;
                }
                else if (item.fldPropertyNameEN == "TerminalId")
                {
                    id_TerminalId = item.fldID;
                }
            }
            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
               Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 1).FirstOrDefault();
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_line).FirstOrDefault();
            var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id).FirstOrDefault();
            var q3 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_TransactionKey).FirstOrDefault();
            var q4 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_TerminalId).FirstOrDefault();
            var MerchantId = q2.fldValue;
            var TransactionKey = q3.fldValue;
            var TerminalId = q4.fldValue;
            Bmi.Url = string.Format("Https://Sadad.Shaparak.Ir/Services/Merchantutility.Asmx");
            string RefNum = "", Status = "";
            Bmi.CheckRequestStatus(
                    Convert.ToInt64(OrderId),
                    MerchantId, TerminalId, TransactionKey, Session["Request"].ToString(),
                    Convert.ToInt64(Session["Amount"]), out RefNum, out Status);
            if (Status == "COMMIT")
            {
                var onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", Convert.ToInt64(OrderId).ToString(), 0, 1, "").FirstOrDefault();

                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                int? userid = null;
                int? guestUserId = null;

                if (Session["UserId"] != null)
                    userid = Convert.ToInt32(Session["UserId"]);
                else if (Session["GeustId"] != null)
                    userid = Convert.ToInt32(Session["GeustId"]);
                if (Session["UserGeust"] != null)
                    guestUserId = Convert.ToInt32(Session["UserGeust"]);
                car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, true, RefNum, userid,guestUserId, "","");
                car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
                    Convert.ToInt32(onlinepay.fldMony), 10, null, onlinepay.fldID, "",
                    userid, "", "", "", null, "", null, null, true, 1, DateTime.Now);
                money = Convert.ToDecimal(onlinepay.fldMony);
                Session["ResidId"] = _id.Value;
                //if (Session["IsOfficeUser"] != null)
                //{
                //    epishkhan.devpishkhannwsv1 epishkhan = new epishkhan.devpishkhannwsv1();
                //    var shpeigiri = epishkhan.verify("atJ5+$J1RtFpj", Convert.ToInt32(Session["serviceCallSerial"]), _id.Value.ToString());
                    //تابع آپدیت جدول پرداخت                        
                    //car.sp_CollectionUpdatePishkhan(Convert.ToInt64(_id.Value), 1, "", shpeigiri.ToString());
                //}
                R = true;
            }
            else
            {
                int? guestUserId = null;
                if (Session["UserGeust"] != null)
                    guestUserId = Convert.ToInt32(Session["UserGeust"]);

                var onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode",Convert.ToInt64( OrderId).ToString(), 0, 1, "").FirstOrDefault();
                car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, false, RefNum, 1,guestUserId, "","");
            }

            ViewBag.ResNum = OrderId;
            ViewBag.RefNum = RefNum;
            ViewBag.Bank = "بانک ملی";
            ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
            ViewBag.Money = Convert.ToDouble(money).ToString("#,###") + " ريال";
            if (R)
                ViewBag.Result = "پرداخت شما با موفقیت انجام شد.";
            else
                ViewBag.Result = "ناموفق";
            return View();
        }
        public ActionResult Pay()
        {
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.sp_BankParameterSelect("fldBankID", "1", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id = 0;
            var id_url = 0;
            var id_line = 0;
            var id_TerminalId = 0;
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "MerchantId")
                {
                    id = item.fldID;
                }
                else if (item.fldPropertyNameEN == "BackURL")
                {
                    id_url = item.fldID;
                }
                else if (item.fldPropertyNameEN == "Line")
                {
                    id_line = item.fldID;
                }
                else if (item.fldPropertyNameEN == "TerminalId")
                {
                    id_TerminalId = item.fldID;
                }
            }
            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
                Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 1).FirstOrDefault();
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id).FirstOrDefault();
            var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_url).FirstOrDefault();
            var q3 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_line).FirstOrDefault();
            var q4 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_TerminalId).FirstOrDefault();
            var url = "http://" + q2.fldValue + "/MeliBank/Back";
            var Line = "https://" + "epayment" + q3.fldValue + ".bmi.ir/epayment/paymentform.aspx";

                return Redirect(Line);
           
        }
    }
}
