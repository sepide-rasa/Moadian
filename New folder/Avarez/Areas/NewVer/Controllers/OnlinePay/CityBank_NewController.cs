using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.NewVer.Controllers.OnlinePay
{
    public class CityBank_NewController : Controller
    {
        //
        // GET: /NewVer/CityBank_New/
        //این متغییر برای نگهداری ژتون دریافت شده در نظر گرفته شده است
        string received_token_value = "";


        private string setSendingDataBill(string Amount, string tax, string BILLID, string PAYMENTID)
        {
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.sp_BankParameterSelect("fldBankID", "20", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id = 0;
            var id_url = 0;
            var id_Merchant = 0;
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "TId")
                {
                    id = item.fldID;
                }
                else if (item.fldPropertyNameEN == "RedirectUrl")
                {
                    id_url = item.fldID;
                }
                else if (item.fldPropertyNameEN == "MId")
                {
                    id_Merchant = item.fldID;
                }
            }
            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
               Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 20).FirstOrDefault();
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id).FirstOrDefault();
            var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_url).FirstOrDefault();
            var q3 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_Merchant).FirstOrDefault();


            //رمز شده شناسه قبض
            string EnBILLID;

            //رمز شده شناسه پرداخت
            string EnPAYMENTID;

            //این متغییر مبلغ قبض را بصورت رمزشده نگهداری می کند
            string EnAmount;

            //کد پذیرنده که از مبنا کارت دریافت شده است
            string MID = q3.fldValue;
            //رمز شده کد پذیرنده
            string EnMID;

            //کد خرید فروشنده که برای هر خرید تولید می شود
            string CRN = tax;
            //رمز شده کد خرید فروشنده 
            string EnCRN;

            //آدرس صفحه برگشت به سایت فروشنده
            string REFERALADRESS = q2.fldValue;
            //رمز شده آدرس برگشت به سایت فروشنده
            string EnREFERALADRESS = "";

            //شماره ترمینال که از سمت مبنا کارت در اختیار شما قرار داده شده است
            string TID = q1.fldValue;
            //رمز شده شماره مشتری
            string EnTID = "";

            //متغیر برای نگهداری امضای دیجیتال
            string Signature = "";

            //کلید عمومی که مبنا کارت در اختیار شما قرار می دهد برای عملیات رمز نگاری
            string PublicKey = "<RSAKeyValue><Modulus>hE7cwvaWfnZ0NM9KVy+qTa5J+LxnWy03934R+TLFWLBSD7rmlEgvRJ202vBvadQCCjbI0GgW835L4HbuU8/KhF2X2EFdm50grZf/OzO159K2gJhk640pYk3bMTMiRzm8wD5Gpox3pgMR3fVZMF32TjwIdNLnmoPm7kwJG8y9TJU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            //کلید خصوصی که مبنا کارت در اختیار ما قرار می دهد برایعملیات تولید امضای دیجیتال
            string PrivateKey = "<RSAKeyValue><Modulus>kjcDZ3zoPVeXKrKSdnCuPxluQes6Mx3FG39opnW4k5oqTHTr8ZHfX8ucIwcjNOySplE98szkZ5XFVi0HzGGOCGMq+09dHURrkq9zUkunBzsX5GT3SBHsUV4E0D+XZoTPL+14p9yIsg+f6R7GXX8pZ8lQ0t+4oqb1PdRJqvLV6aE=</Modulus><Exponent>AQAB</Exponent><P>/IGkd6BVIvgQog0RQLzSxZp1v36YQbL6fYDXbX51pNc+hEcfUlkw6qii+ntoma1DE2Voukl7qVqcN5nIap8fLQ==</P><Q>lDzlEAgQUypU4yMwbdhHxdCnpvMcay1x/lpPzBXMNP770RoOi3MJlu5YwJy+OPJpyO9prd9IBXhToHrrpyIcxQ==</Q><DP>VZOObOiO0gomgPVSypD0EfpWO68o5ONGl7BJ0pcQQeydCHGeQOdvd6ftjFy0x5h76h/tTW1IFs5ZsVJSJSiGhQ==</DP><DQ>RvJWFzQkzAjok8UVupzWzQAuHSMhqNIZSsjihCSylXKsBsnXyDoIjuoIJDdge1TZ+EdNZuAEUknijF4IRvd2mQ==</DQ><InverseQ>c394FAXSnvzbfn73Egs6HecPSZPPPUZPc+7IeYzli+/gRxZoIq+2vwPUGLX6m0nd7UdKnZ+i8uQJagXYBCgWbw==</InverseQ><D>d6FFSsX66j2hNFEY3olBN+VhvdNtMed4nw/2msOUukeXtiiv512XqrRX0p0DZEjvj97G1cJoWoxDAkgjqkhjQRennMxuAzlny+lZJ92rjcEKl2Il/jvdqG6W8XE23yh4O9Et3CXdI+B1U22MW7V9lxBQ+zzX3PGed7oTx67PEXE=</D></RSAKeyValue>";
            //تولید یک نمونه برای عملیات ایجاد امضا
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            //بارگذاری کلید خصوصی
            rsa.FromXmlString(PrivateKey);
            //داده ها ی بر اساس ترتیب خواسته شده از طرف مبنا کارت کد گذاری و امضا می شوند
            byte[] signMain = rsa.SignData(System.Text.Encoding.UTF8.GetBytes(BILLID + PAYMENTID + Amount + CRN + MID + REFERALADRESS + TID), new
            SHA1CryptoServiceProvider());
            //نگهداری و تبدیل امضای تولید شده به فرمت مورد نیاز
            Signature = Convert.ToBase64String(signMain);


            //تولید یک نمونه برای عملیات رمزنگاری 
            RSACryptoServiceProvider cipher = new RSACryptoServiceProvider();
            //بارگذاری کلید عمومی برای عملیات رمزنگار
            cipher.FromXmlString(PublicKey);

            //فرآیند رمزنگاری شناسه قبض
            byte[] data = Encoding.UTF8.GetBytes(BILLID);
            byte[] cipherText = cipher.Encrypt(data, false);
            EnBILLID = Convert.ToBase64String(cipherText);

            //فرآیند رمزنگاری شناسه پرداخت
            data = Encoding.UTF8.GetBytes(PAYMENTID);
            cipherText = cipher.Encrypt(data, false);
            EnPAYMENTID = Convert.ToBase64String(cipherText);

            //فرآیند رمزنگاری مبلغ قبض
            data = Encoding.UTF8.GetBytes(Amount);
            cipherText = cipher.Encrypt(data, false);
            EnAmount = Convert.ToBase64String(cipherText);

            //فرآیند رمزنگاری کدپذرنده
            data = Encoding.UTF8.GetBytes(MID);
            cipherText = cipher.Encrypt(data, false);
            EnMID = Convert.ToBase64String(cipherText);

            // فرآیند رمزنگاری کد خرید
            data = Encoding.UTF8.GetBytes(CRN);
            cipherText = cipher.Encrypt(data, false);
            EnCRN = Convert.ToBase64String(cipherText);

            //فرآیند رمز نگاری آدرس بازگشت
            data = Encoding.UTF8.GetBytes(REFERALADRESS);
            cipherText = cipher.Encrypt(data, false);
            EnREFERALADRESS = Convert.ToBase64String(cipherText);

            //فرآیند شماره ترمینال
            data = Encoding.UTF8.GetBytes(TID);
            cipherText = cipher.Encrypt(data, false);
            EnTID = Convert.ToBase64String(cipherText);


            //ساخت یک نمونه از وب سرویس
            WsCityNew.Token _services = new WsCityNew.Token();
            //ساخت یک نمونه از پارامترهای مورد نیاز تولید ژتون 
            WsCityNew.billTokenDTO _TokenParm = new WsCityNew.billTokenDTO();
            _TokenParm.BILLID = EnBILLID;
            _TokenParm.PAYMENTID = EnPAYMENTID;
            _TokenParm.AMOUNT = EnAmount;
            _TokenParm.CRN = EnCRN;
            _TokenParm.MID = EnMID;
            _TokenParm.REFERALADRESS = EnREFERALADRESS;
            _TokenParm.SIGNATURE = Signature;
            _TokenParm.TID = EnTID;

            //فراخوانی متد تولید ژتون
            WsCityNew.tokenResponse _TokenResponse = _services.reservation(_TokenParm);

            if (_TokenResponse.result == 0)
            {
                //در صورتی که مقدار 0 باشد عملیات موفقیت آمیز بوده است و ژتون دریافت شده
                //ژتون برای ارسال ست شده است
                return _TokenResponse.token;
            }

            else
                return _TokenResponse.result.ToString();

        }
        public ActionResult Index()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Token = setSendingDataBill(Session["Amount"].ToString(), Session["Tax"].ToString(), Session["shGhabz"].ToString(), Session["shPardakht"].ToString());

            return PartialView;
        }
        public ActionResult Back(string State, string ResNum, string RefNum)
        {
            decimal money = 0;
            bool R = false;
            Session.Remove("ResidId");
            Models.cartaxEntities car = new Models.cartaxEntities();
            if (State == "OK")
            {
                var q = car.sp_BankParameterSelect("fldBankID", "20", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
                var usernameid = 0;
                var passid = 0;
                foreach (var item in q)
                {
                    switch (item.fldPropertyNameEN)
                    {
                        case "UserName":
                            usernameid = item.fldID;
                            break;
                        case "Password":
                            passid = item.fldID;
                            break;
                    }
                }
                var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
                    Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 20).FirstOrDefault();
                var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == usernameid).FirstOrDefault();
                var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == passid).FirstOrDefault();

                WsCity.PaymentWebServiceService x = new WsCity.PaymentWebServiceService();
                WsCity.loginRequest z = new WsCity.loginRequest(); z.username = q1.fldValue; z.password = q2.fldValue;
                WsCity.wsContextEntry e = new WsCity.wsContextEntry(); e.key = "SESSION_ID"; e.value = x.login(z);
                WsCity.wsContext w = new WsCity.wsContext(); w.data = new WsCity.wsContextEntry[1]; w.data[0] = e;
                WsCity.verifyResponseResult[] r = x.verifyTransaction(w, new string[] { RefNum });
                var onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", ResNum, 0, 1, "").FirstOrDefault();
                if (r.Length > 0)
                {
                    R = r[0].amountSpecified ? (r[0].amount == onlinepay.fldMony) : true;
                    R = R && (r[0].refNum == RefNum);
                    R = R && (r[0].verificationErrorSpecified == false);
                }
                x.logout(w);
                if (R == true)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    int? userid = null;
                    int? guestUserId = null;
                    if (Session["UserId"] != null)
                        userid = Convert.ToInt32(Session["UserId"]);
                    else if (Session["GeustId"] != null)
                        userid = Convert.ToInt32(Session["GeustId"]);
                    else if (Session["UserGeust"] != null)
                        guestUserId = Convert.ToInt32(Session["UserGeust"]);
                    
                    car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, true, RefNum, userid, guestUserId, "","");
                    /*car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
                        Convert.ToInt32(onlinepay.fldMony), 10, null, onlinepay.fldID, "",
                        userid, "", "", "", null, "", null, null, true, 1, DateTime.Now);*/
                    car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
                        Convert.ToInt32(onlinepay.fldMony), 10,Convert.ToInt32( Session["OnlinefishId"]), null, "",
                        userid, "", "", "", null, "", null, null, true, 1, DateTime.Now);
                    SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                    if (Convert.ToInt32(Session["UserMnu"]) == 1)
                    {
                        Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                        var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, Session["OnlinefishId"].ToString(), DateTime.Now, "");
                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                               + k1 + "-" + Session["OnlinefishId"].ToString() + "\n");
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
                }
                else
                {
                    int? guestUserId = null;
                    if (Session["UserGeust"] != null)
                        guestUserId = Convert.ToInt32(Session["UserGeust"]);
                    car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, false, RefNum, 1, guestUserId, "","");
                }
            }
            else
            {
                var q = car.sp_OnlinePaymentsSelect("fldTemporaryCode", ResNum, 0, 1, "").FirstOrDefault();
                if (q != null)
                {
                    int? guestUserId = null;
                    if (Session["UserGeust"] != null)
                        guestUserId = Convert.ToInt32(Session["UserGeust"]);
                    car.sp_OnlinePaymentsFinalPaymentUpdate(q.fldID, false, RefNum, 1, guestUserId, "","");
                }
            }
            ViewBag.ResNum = ResNum;
            ViewBag.RefNum = RefNum;
            ViewBag.Bank = "بانک شهر";
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
