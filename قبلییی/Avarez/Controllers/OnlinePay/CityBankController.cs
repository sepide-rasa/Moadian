using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.OnlinePay
{
    public class CityBankController : Controller
    {
        //
        // GET: /City/

        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult WinIndex()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
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
                }
                else
                {
                    int? guestUserId = null;
                    if (Session["UserGeust"] != null)
                        guestUserId = Convert.ToInt32(Session["UserGeust"]);
                    car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, false, RefNum, 1,guestUserId, "","");
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
                    car.sp_OnlinePaymentsFinalPaymentUpdate(q.fldID, false, RefNum, 1, guestUserId,"","");
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
