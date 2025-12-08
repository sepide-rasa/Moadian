using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.OnlinePay
{
    public class ParsianController : Controller
    {
        //
        // GET: /Parsian/

        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult WinIndex()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Pay()
        {
            Parsian.EShopService e = new Parsian.EShopService();
            long authority = 0;
            byte status = 0;
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.sp_BankParameterSelect("fldBankID", "15", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id = 0;
            var id_url = 0;
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "PIN")
                {
                    id = item.fldID;
                }
                else if (item.fldPropertyNameEN == "BackUrl")
                {
                    id_url = item.fldID;
                }
            }
            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
                Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 15).FirstOrDefault();
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id).FirstOrDefault();
            var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_url).FirstOrDefault();
            var url = "http://" + q2.fldValue + "/Parsian/Back";
            Session["PIN"] = q1.fldValue;
            e.PinPaymentRequest(Session["PIN"].ToString(), Convert.ToInt32(Session["Amount"]),Convert.ToInt32(Session["Tax"]), url, ref authority, ref status);
            if (status == 0)
            {
                Session["authority"] = authority;
                return Redirect("https://pec.shaparak.ir/pecpaymentgateway/default.aspx?au=" + authority);                
            }
            ViewBag.status = status;
            return View();
        }
        public ActionResult Back(int rs, long au)
        {
            Parsian.EShopService e = new Parsian.EShopService();
            long InvoiceNamber = 0;
            byte status = 0;
            if (rs == 0 && Session["authority"].ToString()==au.ToString())
            {
                e.PaymentEnquiry(Session["PIN"].ToString(), Convert.ToInt64(Session["authority"]), ref status, ref InvoiceNamber);
                if (status == 0 && InvoiceNamber != -1)
                {
                    Models.cartaxEntities car = new Models.cartaxEntities();
                    string ResNum = Session["Tax"].ToString();
                    var onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", ResNum, 0, 1, "").FirstOrDefault();

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
                        Convert.ToInt32(onlinepay.fldMony), 10, null, onlinepay.fldID, "",
                        userid, "", "", "", null, "", null, null, true, 1, DateTime.Now);
                    decimal money = Convert.ToDecimal(onlinepay.fldMony);
                    ViewBag.InvoiceNamber = InvoiceNamber;
                    ViewBag.Result = "موفق";
                    ViewBag.ResNum = ResNum;
                    ViewBag.RefNum = InvoiceNamber;
                    ViewBag.Bank = "بانک پارسیان";
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
            {
                ViewBag.Result = "نا موفق";
            }

            return View();
        }
    }
}
