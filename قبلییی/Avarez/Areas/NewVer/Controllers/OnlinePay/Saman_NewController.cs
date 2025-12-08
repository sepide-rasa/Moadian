using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.NewVer.Controllers.OnlinePay
{
    public class Saman_NewController : Controller
    {
        //
        // GET: /NewVer/Saman_New/

        public ActionResult Index()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            try
            {
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

                var url = q2.fldValue + "/NewVer/Saman_New/Back";
                Session["BankUserName"] = q3.fldValue;
                Session["BankPass"] = q4.fldValue;
                PartialView.ViewBag.TerminalId = q1.fldValue;
                PartialView.ViewBag.RedirectUrl = url;
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\saman\" + Session["Tax"].ToString(),
                    Session["Tax"].ToString() + "," + Session["UserId"] + "," + Session["shGhabz"].ToString() + ","+
                     Session["shPardakht"].ToString() + "," + Session["UserPass"] + "," + Session["UserMnu"] + "," +
                    Session["UserState"] + "," + Session["CountryType"] + "," + Session["CountryCode"]
                    + "," + Session["area"] + "," + Session["office"] + "," + Session["Location"] + "," +
                    Session["GeustId"] + "," + Session["OnlinefishId"] + "," + Session["ReturnUrl"].ToString());
                car.tblOnlineLogs.Add(new Models.tblOnlineLog
                {
                    fldText = "log1: TerminalId:" + q1.fldValue + " RedirectUrl: " + url,
                    fllOnlineId = Convert.ToInt32(Session["Tax"]),
                    fldDate = DateTime.Now
                });
                car.SaveChanges();
                return PartialView;
            }
            catch (Exception x)
            {
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                var userid = 0;
                if (Session["UserId"] != null)
                    userid = Convert.ToInt32(Session["UserId"]);
                else if (Session["GeustId"] != null)
                    userid = Convert.ToInt32(Session["GeustId"]);
                car.sp_ErrorProgramInsert(Eid, InnerException, userid, x.Message, DateTime.Now, "");
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message ="خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید."
                });
                DirectResult resultt = new DirectResult();
                return resultt;
            }
        }
        public ActionResult Back()
        {
            try
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                var ss = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\saman\" + Request.Form["ResNum"].ToString());
                var ss1 = ss.Split(',');
                Session["Tax"] = ss1[0];
                Session["UserId"] = ss1[1];
                Session["shGhabz"] = ss1[2];
                Session["shPardakht"] = ss1[3];
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
                Session["ReturnUrl"] = ss1[14];
                if (Session["UserId"] != null)
                {
                    if (Session["UserId"].ToString() == "")
                        Session["UserId"] = null;
                }
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\saman\" + Request.Form["ResNum"].ToString());
                string State = Request.Form["State"].ToString();
                long ResNum = Convert.ToInt64(Request.Form["ResNum"]);
                car.tblOnlineLogs.Add(new Models.tblOnlineLog
                {
                    fldText = "log2 in back: State:" + State + " ResNum:" + ResNum,
                    fllOnlineId = Convert.ToInt32(Session["Tax"]),
                    fldDate = DateTime.Now
                });
                car.SaveChanges();
                if (State == "OK" && Session["Tax"].ToString() == ResNum.ToString())
                {
                    car.tblOnlineLogs.Add(new Models.tblOnlineLog
                    {
                        fldText = "log3 in back: result:" + Request.Form["Bills[0].BillId"] + "_" + Request.Form["Bills[0].PayId"] + "_" + Request.Form["Bills[0].State"],
                        fllOnlineId = Convert.ToInt32(Session["Tax"]),
                        fldDate = DateTime.Now
                    });
                    car.SaveChanges();
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

                        car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, true, InvoiceNamber.ToString(), userid, guestUserId, "", "");
                        car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
                            Convert.ToInt32(onlinepay.fldMony), 10, Convert.ToInt32(Session["OnlinefishId"])
                            , null, "", userid, "پرداخت اینترنتی از طریق شناسه قبض و پرداخت: کد رهگیری:"
                            + InvoiceNamber + " و کد پرداخت:" + onlinepay.fldID.ToString(), "", "", null, "", null, null, true, 1, DateTime.Now);
                        SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                        if (Convert.ToInt32(Session["UserMnu"]) == 1)
                        {
                            Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                            var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, Session["OnlinefishId"].ToString(), DateTime.Now, "");
                            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                  + k1 + "-" + Session["OnlinefishId"].ToString() + "\n");
                        }
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
                        //    //تابع آپدیت جدول پرداخت                        
                        //    //car.sp_CollectionUpdatePishkhan(Convert.ToInt64(_id.Value), 1, "", shpeigiri.ToString());
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
            }
            catch (Exception x)
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message+" onlineId:"+Session["Tax"].ToString(), DateTime.Now, Session["UserPass"].ToString());

            }
            return View();
        }
    }
}
