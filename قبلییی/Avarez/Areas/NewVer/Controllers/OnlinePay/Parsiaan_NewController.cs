using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.NewVer.Controllers.OnlinePay
{
    public class Parsiaan_NewController : Controller
    {
        //
        // GET: /NewVer/Parsiaan_New/

        public ActionResult Index()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }

        [HttpPost]
        public ActionResult Back(long Token, short status,long OrderId,string TerminalNo,string RRN,string HashCardNumber,string Amount)
        {
            Parsian_New.SaleService service = new Parsian_New.SaleService();
            ConfirmService.ConfirmService conService = new ConfirmService.ConfirmService();
            ConfirmService.ClientConfirmRequestData CrequestData = new ConfirmService.ClientConfirmRequestData();
            ConfirmService.ClientConfirmResponseData CresponseData = new ConfirmService.ClientConfirmResponseData();

            try
            {
                var ss = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\parsian\" + Token.ToString());
                var ss1 = ss.Split(',');
                Session["Tax"] = ss1[0];
                Session["LoginAccount"] = ss1[1];
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

                if (Session["UserId"] != null)
                {
                    if (Session["UserId"].ToString() == "")
                        Session["UserId"] = null;
                }
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\parsian\" + Token.ToString());
                if (status == 0 && Token > 0)
                {
                    CrequestData.Token = Token;
                    CrequestData.LoginAccount = Session["LoginAccount"].ToString();//pin
                    CresponseData = conService.ConfirmPayment(CrequestData);

                    if (CresponseData.Status == 0 && CresponseData.RRN > 0)
                    {
                        Models.cartaxEntities car = new Models.cartaxEntities();
                        string ResNum = Session["Tax"].ToString();
                        var onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", ResNum, 0, 1, "").Where(l => l.fldBankID == 30 && l.fldMunID == Convert.ToInt32(Session["UserMnu"])).FirstOrDefault();

                        System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                        int? userid = null;
                        int? guestUserId = null;
                        if (Session["UserId"] != null)
                            userid = Convert.ToInt32(Session["UserId"]);
                        else if (Session["GeustId"] != null)
                            userid = Convert.ToInt32(Session["GeustId"]);
                        else if (Session["UserGeust"] != null)
                            guestUserId = Convert.ToInt32(Session["UserGeust"]);

                        var Description = "Token=" + CresponseData.Token.ToString() + ";" + "CardNumber=" + CresponseData.CardNumberMasked.ToString() +
                            "TerminalNo=" + TerminalNo;

                        car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, true, CresponseData.RRN.ToString(), userid, guestUserId, "", Description);
                        if (Session["UserId"] != null || Session["GeustId"] != null)
                        {
                            car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
                                Convert.ToInt32(onlinepay.fldMony), 10, Convert.ToInt32(Session["OnlinefishId"]), null, "",
                                userid, "", "", "", null, "", null, null, true, 1, DateTime.Now);
                            SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                            SmsSender sms = new SmsSender();
                            sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, onlinepay.fldCarFileID, (int)onlinepay.fldMony, "", "", "");
                        }
                        else
                        {
                            car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
                                Convert.ToInt32(onlinepay.fldMony), 10, Convert.ToInt32(Session["OnlinefishId"]), null, "",
                                null, "", "", "", null, "", null, null, true, 1, DateTime.Now);
                            SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                        }
                        //System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt","step1:"+onlinepay.fldID.ToString() + "\n");
                        if (Convert.ToInt32(Session["UserMnu"]) == 1)
                        {
                            //System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt", "step2:" + onlinepay.fldID.ToString() + "\n");
                            Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                            var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, Session["OnlinefishId"].ToString(), DateTime.Now, "");
                            //System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                   //+ k1 + "-" + onlinepay.fldID.ToString() + "\n");
                            //System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt", "step3:" + onlinepay.fldID.ToString() + "\n");
                        }
                        //System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt", "step4:" + onlinepay.fldID.ToString() + "\n");
                        
                        decimal money = Convert.ToDecimal(onlinepay.fldMony);
                        ViewBag.InvoiceNamber = CresponseData.RRN.ToString();
                        ViewBag.Result = "موفق";
                        ViewBag.ResNum = ResNum;
                        ViewBag.RefNum = CresponseData.RRN.ToString();
                        ViewBag.Bank = "بانک پارسیان";
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
                        ViewBag.Result = "پرداخت شما با موفقیت انجام شد.";
                    }
                    else
                        ViewBag.Result = "نا موفق";
                }
                else
                {
                    ViewBag.Result = "نا موفق";
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "خطای مربوط به بانک پارسیان:"+x.Message;
                if (x.InnerException != null)
                    InnerException = "خطای مربوط به بانک پارسیان:"+x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                //return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
            
            return View();            
        }
        public ActionResult Pay()
        {
            Parsian_New.SaleService pservice = new Parsian_New.SaleService();
            Parsian_New.ClientSaleRequestData requestData = new Parsian_New.ClientSaleRequestData();
            Parsian_New.ClientSaleResponseData responseData = new Parsian_New.ClientSaleResponseData();

            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.sp_BankParameterSelect("fldBankID", "30", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var id = 0;//pin
            var id_url = 0;//backurl
            foreach (var item in q)
            {
                if (item.fldPropertyNameEN == "LoginAccount")
                {
                    id = item.fldID;
                }
                else if (item.fldPropertyNameEN == "CallBackUrl")
                {
                    id_url = item.fldID;
                }
            }

            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
                Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 30).FirstOrDefault();
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id).FirstOrDefault();
            var q2 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").Where(h => h.fldParametrID == id_url).FirstOrDefault();
            Session["LoginAccount"] = q1.fldValue;
            var url = q2.fldValue + "/NewVer/Parsiaan_New/Back";
            requestData.CallBackUrl = url;
            requestData.LoginAccount = q1.fldValue;
            requestData.Amount = Convert.ToInt64(Session["Amount"]);
            requestData.OrderId = Convert.ToInt64(Session["Tax"]);
            responseData = pservice.SalePaymentRequest(requestData);

            if (responseData.Status == 0 && responseData.Token > 0)
            {
                Session["Token"] = responseData.Token;//شماره درخواست در دروازه پرداخت
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"parsian\" + responseData.Token,
                    Session["Tax"].ToString() + "," + Session["LoginAccount"] + "," + Session["UserId"] + ","
                    + Session["ReturnUrl"] + "," + Session["UserPass"] + "," + Session["UserMnu"] + "," +
                    Session["UserState"] + "," + Session["CountryType"] + "," + Session["CountryCode"]
                    + "," + Session["area"] + "," + Session["office"] + "," + Session["Location"] + ","
                    + Session["GeustId"] + "," + Session["OnlinefishId"]);
                return Redirect("https://pec.shaparak.ir/NewIPG/?Token=" + responseData.Token.ToString());
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "كد خطا:" + responseData.Status.ToString()
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
    }
}
