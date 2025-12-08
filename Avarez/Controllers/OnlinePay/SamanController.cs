using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.OnlinePay
{
    public class SamanController : Controller
    {
        //
        // GET: /Saman/

        public ActionResult Index()
        {
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.sp_BankParameterSelect("fldBankID", "17", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id = 0;
            var id_url = 0;
            var id_username = 0;
            var id_Pass = 0;
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "TerminalId")
                {
                    id = item.fldID;
                }
                else if (item.fldPropertyNameEN == "RedirectUrl")
                {
                    id_url = item.fldID;
                }
                else if (item.fldPropertyNameEN == "BankUserName")
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
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id).FirstOrDefault();
            var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_url).FirstOrDefault();
            var q3 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_username).FirstOrDefault();
            var q4 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_Pass).FirstOrDefault();

            var url = "http://" + q2.fldValue + "/Saman/Back";
            Session["BankUserName"] = q3.fldValue;
            Session["BankPass"] = q4.fldValue;
            ViewBag.TerminalId = q1.fldValue;
            ViewBag.RedirectUrl = url;
            return PartialView();
        }
        public ActionResult WinIndex()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.sp_BankParameterSelect("fldBankID", "17", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id = 0;
            var id_url = 0;
            var id_username = 0;
            var id_Pass = 0;
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "TerminalId")
                {
                    id = item.fldID;
                }
                else if (item.fldPropertyNameEN == "RedirectUrl")
                {
                    id_url = item.fldID;
                }
                else if (item.fldPropertyNameEN == "BankUserName")
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
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id).FirstOrDefault();
            var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_url).FirstOrDefault();
            var q3 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_username).FirstOrDefault();
            var q4 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_Pass).FirstOrDefault();

            var url = "http://" + q2.fldValue + "/Saman/Back";
            Session["BankUserName"] = q3.fldValue;
            Session["BankPass"] = q4.fldValue;
            PartialView.ViewBag.TerminalId = q1.fldValue;
            PartialView.ViewBag.RedirectUrl = url;
            return PartialView;
        }
        [HttpPost]
        public ActionResult Back()
        {
            string State = Request.Form["State"].ToString();
            long ResNum = Convert.ToInt64(Request.Form["ResNum"]);
            if (State == "OK" && Session["Tax"].ToString() == ResNum.ToString())
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                Saman.BillStateService saman = new Saman.BillStateService();

                if (Request.Form["Bills[0].BillId"] == Session["shGhabz"].ToString()
                    && Request.Form["Bills[0].PayId"] == Session["shPardakht"].ToString()
                    && Request.Form["Bills[0].State"] == "OK")
                {
                    string InvoiceNamber = Request.Form["Bills[0].TraceNo"];
                    var onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", ResNum.ToString(), 0, 1, "").FirstOrDefault();

                    System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    int? userid = null;
                    int? guestUserId = null;
                    if (Session["UserId"] != null)
                        userid = Convert.ToInt32(Session["UserId"]);
                    else if (Session["GeustId"] != null)
                        userid = Convert.ToInt32(Session["GeustId"]);
                    else if (Session["UserGeust"] != null)
                        guestUserId = Convert.ToInt32(Session["UserGeust"]);

                    car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, true, InvoiceNamber.ToString(), userid,guestUserId, "","");
                    car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
                        Convert.ToInt32(onlinepay.fldMony), 10, Convert.ToInt32(Session["OnlinefishId"])
                        , null, "", userid, "پرداخت اینترنتی از طریق شناسه قبض و پرداخت: کد رهگیری:"
                        + InvoiceNamber + " و کد پرداخت:" + onlinepay.fldID.ToString(), "", "", null, "", null, null, true, 1, DateTime.Now);
                    decimal money = Convert.ToDecimal(onlinepay.fldMony);
                    ViewBag.InvoiceNamber = InvoiceNamber;
                    ViewBag.Result = "موفق";
                    ViewBag.ResNum = ResNum;
                    ViewBag.RefNum = InvoiceNamber;
                    ViewBag.Bank = "بانک سامان";
                    ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
                    ViewBag.Money = Convert.ToDouble(money).ToString("#,###") + " ريال";
                    Session["ResidId"] = _id.Value;
                    //if (Session["IsOfficeUser"] != null)
                    //{
                    //    epishkhan.devpishkhannwsv1 epishkhan = new epishkhan.devpishkhannwsv1();
                    //    var shpeigiri = epishkhan.verify("atJ5+$J1RtFpj", Convert.ToInt32(Session["serviceCallSerial"]), _id.Value.ToString());
                        //تابع آپدیت جدول پرداخت                        
                        //car.sp_CollectionUpdatePishkhan(Convert.ToInt64(_id.Value), 1, "", shpeigiri.ToString());
                    //}
                    ViewBag.Result = "پرداخت شما با موفقیت انجام شد.";                    
                }
                else
                    ViewBag.Result = "نا موفق";
            }
            else
                ViewBag.Result = "نا موفق";
            Session["shGhabz"] = null;
            Session["shPardakht"] = null;
            Session["BankUserName"] = null;
            Session["BankPass"] = null;
            return View();
        }
    }
    
}
