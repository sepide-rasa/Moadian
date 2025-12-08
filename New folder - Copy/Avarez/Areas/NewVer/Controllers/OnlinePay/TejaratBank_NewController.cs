using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.NewVer.Controllers.OnlinePay
{
    public class TejaratBank_NewController : Controller
    {
        //
        // GET: /NewVer/TejaratBank_New/

        public ActionResult Index()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Back(string paymentId, string referenceId, string resultCode)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            paymentId = paymentId.PadLeft(10, '0');
            var q = car.sp_BankParameterSelect("fldBankID", "2", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id = 0;
            var id_url = 0;
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "MerchantId")
                {
                    id = item.fldID;
                }

            }
            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
                Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 2).FirstOrDefault();
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id).FirstOrDefault();
            string merchantId = q1.fldValue;
            var onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", paymentId, 0, 1, "").FirstOrDefault();
            int? guestUserId = null;
            if (Session["UserGeust"] != null)
                guestUserId = Convert.ToInt32(Session["UserGeust"]);

            car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, false, referenceId, 1, guestUserId, "","");
            decimal money = 0;
            bool R = false;
            if (resultCode == "100")
            {

                Tejarat.verifyRequest Verify = new Tejarat.verifyRequest();
                Verify.merchantId = merchantId;
                Verify.referenceNumber = referenceId;
                Tejarat.MerchantService Client = new Tejarat.MerchantService();
                try
                {
                    long result = Client.verify(Verify);
                    if (result > 0)
                    {
                        onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", paymentId, 0, 1, "").FirstOrDefault();

                        System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                        int? userid = null;
                        if (Session["UserId"] != null)
                            userid = Convert.ToInt32(Session["UserId"]);
                        else if (Session["GeustId"] != null)
                            userid = Convert.ToInt32(Session["GeustId"]);
                        car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, true, referenceId, userid, guestUserId, "","");
                        car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
                            Convert.ToInt32(onlinepay.fldMony), 10, null, onlinepay.fldID, ""
                            , userid, "", "", "", null, "", null, null, true, 1, DateTime.Now);
                        SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                        if (Convert.ToInt32(Session["UserMnu"]) == 1)
                        {
                            Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                            var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, onlinepay.fldID.ToString(), DateTime.Now, "");
                            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                   + k1 + "-" + onlinepay.fldID.ToString() + "\n");
                        }
                        money = Convert.ToDecimal(onlinepay.fldMony);
                        Session["ResidId"] = _id.Value;
                        //if (Session["IsOfficeUser"] != null)
                        //{
                        //    epishkhan.devpishkhannwsv1 epishkhan = new epishkhan.devpishkhannwsv1();
                        //    var shpeigiri = epishkhan.verify("atJ5+$J1RtFpj", Convert.ToInt32(Session["serviceCallSerial"]), _id.Value.ToString());
                        //    //تابع آپدیت جدول پرداخت                        
                        //    //car.sp_CollectionUpdatePishkhan(Convert.ToInt64(_id.Value), 1, "", shpeigiri.ToString());
                        //}
                        R = true;
                    }
                    else
                    {
                        onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", paymentId, 0, 1, "").FirstOrDefault();
                        car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, false, referenceId, 1, guestUserId, "","");
                        switch (result)
                        {
                            case -20:
                                ViewBag.Result = "وجود کاراکترهای غیر مجاز در درخواست";
                                break;
                            case -30:
                                ViewBag.Result = "تراکنش قبلا برگشت خورده است";
                                break;
                            case -50:
                                ViewBag.Result = "طول رشته درخواست غیر مجاز است";
                                break;
                            case -51:
                                ViewBag.Result = "خطا در درخواست";
                                break;
                            case -80:
                                ViewBag.Result = "تراکنش مورد نظر یافت نشد";
                                break;
                            case -81:
                                ViewBag.Result = "خطای داخلی بانک";
                                break;
                            case -90:
                                ViewBag.Result = "تراکنش قبلا تایید شده است";
                                break;

                        }
                    }
                }
                catch (Exception E)
                {
                    ViewBag.Result = E.Message;
                }
            }
            else
                switch (resultCode)
                {
                    case "110":
                        ViewBag.Result = "انصراف توسط دارنده کارت";
                        break;
                    case "120":
                        ViewBag.Result = "موجودی حساب کافی نیست";
                        break;
                    case "130":
                        ViewBag.Result = "اطلاعات کارت اشتباه است";
                        break;
                    case "131":
                        ViewBag.Result = "رمز کارت اشتباه است";
                        break;
                    case "132":
                        ViewBag.Result = "کارت مسدود شده است";
                        break;
                    case "133":
                        ViewBag.Result = "کارت منقضی شده است";
                        break;
                    case "140":
                        ViewBag.Result = "زمان مورد نظر به پایان رسیده است";
                        break;
                    case "150":
                        ViewBag.Result = "خطای داخلی بانک";
                        break;
                    case "160":
                        ViewBag.Result = "خطا در اطلاعات cvv2 یا ExpDate";
                        break;
                    case "166":
                        ViewBag.Result = "بانک صادرکننده کارت مجوز انجام تراکنش را صادر نکرده است";
                        break;
                    case "200":
                        ViewBag.Result = "مبلغ تراکنش بیشتر از سقف مجاز هر تراکنش می باشد";
                        break;
                    case "201":
                        ViewBag.Result = "مبلغ تراکنش بیشتر از سقف مجاز در روز می باشد";
                        break;
                    case "202":
                        ViewBag.Result = "مبلغ تراکنش بیشتر از سقف مجاز در ماه می باشد";
                        break;
                }
            ViewBag.ResNum = paymentId;
            ViewBag.RefNum = referenceId;
            ViewBag.Bank = "بانک تجارت";
            ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
            ViewBag.Money = Convert.ToDouble(money).ToString("#,###") + " ريال";
            if (R)
                ViewBag.Result = "پرداخت شما با موفقیت انجام شد.";
            else
                ViewBag.Result = "ناموفق";
            return View();

        }
    }
}
