using Avarez.Models;
using Ext.Net;
using Ext.Net.MVC;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.NewVer.Controllers.OnlinePay
{
    public class SamanKishController : Controller
    {
        //
        // GET: /NewVer/SamanKish/

        //public ActionResult Index() برای زمانی بود که میخواستیم از سامانه عوارض به سامانه اس ام اس درخواست بزنیم و از آنجا به درگاه پرداخت
        //{
        //    Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
        //    SmsPanel.RasaSMSPanel_Send Panel = new SmsPanel.RasaSMSPanel_Send();
        //    List<IBANKInfo> BIList = new List<IBANKInfo>();
        //    IBANKInfo BI = new IBANKInfo();
        //    Type type = typeof(SepTxn);
        //    var obj = Activator.CreateInstance(type);

        //    var q = car.sp_BankParameterSelect("fldBankID", "31", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
        //    var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
        //       Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 31).FirstOrDefault();
        //    var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").ToList();

        //    foreach (var kv in q)
        //    {
        //        if (type.GetProperty(kv.fldPropertyNameEN)!=null)
        //            type.GetProperty(kv.fldPropertyNameEN).SetValue(obj, q1.Where(l=>l.fldParametrID==kv.fldID).FirstOrDefault().fldValue,null);
        //    }
        //    SepTxn txn = (SepTxn)obj;
        //    BI.Amount = Convert.ToInt64(Session["Amount"]);
        //    BI.IBAN = q1.Where(l => l.fldPropertyNameEN == "IBAN").FirstOrDefault().fldValue;
        //    BIList.Add(BI);
        //    txn.SettlementIBANInfo = BIList.ToArray();
        //    txn.ResNum = Session["Tax"].ToString();
        //    txn.Amount = Convert.ToInt64(Session["Amount"]);
        //    txn.Action = "Token";
        //    Session["SamanInfo"] = JsonConvert.SerializeObject(txn);
        //    System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"SamanKish\" + Session["Tax"].ToString(),
        //            Session["Tax"].ToString() + "," + Session["UserId"] + ","
        //            + Session["ReturnUrl"] + "," + Session["UserPass"] + "," + Session["UserMnu"] + "," +
        //            Session["UserState"] + "," + Session["CountryType"] + "," + Session["CountryCode"]
        //            + "," + Session["area"] + "," + Session["office"] + "," + Session["Location"] + ","
        //            + Session["GeustId"] + "," + Session["OnlinefishId"]);
        //    Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
        //    return PartialView;
        //}
        public ActionResult Index()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        [HttpPost]
        public ActionResult SendViaToken()
        {
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            List<IBANKInfo> BIList = new List<IBANKInfo>();
            IBANKInfo BI = new IBANKInfo();
            Type type = typeof(SepTxn);
            var obj = Activator.CreateInstance(type);
            var q = car.sp_BankParameterSelect("fldBankID", "33", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
            var info = car.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(Session["CountryCode"]),
               Convert.ToInt32(Session["CountryType"])).Where(k => k.BankId == 33).FirstOrDefault();
            var q1 = car.sp_BankInformationSelect("fldCountryDiv", info.fldCountryDiv.ToString(), 0, 1, "").ToList();

            foreach (var kv in q)
            {
                if (type.GetProperty(kv.fldPropertyNameEN) != null)
                {
                    if (kv.fldPropertyNameEN == "RedirectURL")
                    {
                        var BaseUrl = q1.Where(l => l.fldPropertyNameEN == kv.fldPropertyNameEN).FirstOrDefault().fldValue;
                        type.GetProperty(kv.fldPropertyNameEN).SetValue(obj, BaseUrl + "/NewVer/SamanKish/Back", null);
                    }
                    else
                    {
                        type.GetProperty(kv.fldPropertyNameEN).SetValue(obj, q1.Where(l => l.fldParametrID == kv.fldID).FirstOrDefault().fldValue, null);
                    }
                }
            }
            SepTxn txn = (SepTxn)obj;
            BI.Amount = Convert.ToInt64(Session["Amount"]);
            BI.IBAN = q1.Where(l => l.fldPropertyNameEN == "IBAN").FirstOrDefault().fldValue;
            BIList.Add(BI);
            txn.SettlementIBANInfo = BIList.ToArray();
            txn.ResNum = Session["Tax"].ToString();
            txn.Amount = Convert.ToInt64(Session["Amount"]);
            txn.Action = "Token";
            string Url = "https://sep.shaparak.ir/Onli nePG/OnlinePG";
            var restClient = new RestClient(Url);
            var request = new RestRequest()
            {
                RequestFormat = DataFormat.Json,
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };
            request.AddBody(txn);
            var SepResult = restClient.Execute(request);
            var Token = JsonConvert.DeserializeObject<TokenInf>(SepResult.Content);
            if (Token.status != "-1")
            {
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"SamanKish\" + txn.ResNum,
                   Session["UserId"] + "," + Session["GeustId"] + "," + Session["UserGeust"] + "," +
                   Session["UserMnu"] + "," + Session["UserState"] + "," + Session["area"] + "," + Session["office"] + "," + Session["Location"] + "," + Session["CountryType"] +
                   "," + Session["CountryCode"] + "," + Session["UserPass"] + "," + Session["OnlinefishId"] + "," + Session["ReturnUrl"] + "," + txn.ResNum + "," + txn.Amount);
                Session["Token"] = Token.token;
                return View();
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطای شماره " + Token.errorCode,
                    Message = "شرح خطا: " + Token.errorDesc
                });
                DirectResult dr = new DirectResult();
                return dr;
            }
        }

        [HttpPost]
        public ActionResult Back(string MID, string State, string Status, string RRN, string RefNum, string ResNum, string TerminalId, string TraceNo, long Amount,
            long? Wage, string SecurePan, long? HashedCardNumber)
        {
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            CentralOnlinePaymentTransaction Co = new CentralOnlinePaymentTransaction();
            var FileSavedSession = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\SamanKish\" + ResNum);
            var SavedSession = FileSavedSession.Split(',');
            Session["UserId"] = SavedSession[0] == "" ? null : SavedSession[0];
            Session["GeustId"] = SavedSession[1] == "" ? null : SavedSession[1];
            Session["UserGeust"] = SavedSession[2] == "" ? null : SavedSession[2];
            Session["UserMnu"] = SavedSession[3];
            Session["UserState"] = SavedSession[4];
            Session["area"] = SavedSession[5];
            Session["office"] = SavedSession[6];
            Session["Location"] = SavedSession[7];
            Session["CountryType"] = SavedSession[8];
            Session["CountryCode"] = SavedSession[9];
            Session["UserPass"] = SavedSession[10];
            Session["OnlinefishId"] = SavedSession[11];
            Session["ReturnUrl"] = SavedSession[12];
            Session["ResNum"] = SavedSession[13];
            Session["Amount"] = SavedSession[14];

            if (Status == "2")//تراکنش موفق
            {
                if (ResNum == Session["ResNum"].ToString())//بررسی کد فروشنده
                {
                    try
                    {
                        var ExistRefNum = Co.ExistRefNum(RefNum);
                        if (!ExistRefNum)
                        {
                            string Url = "https://sep.shaparak.ir/verifyTxnRandomSessionkey/ipg/VerifyTransaction";
                            var restClient = new RestClient(Url);
                            var request = new RestRequest()
                            {
                                RequestFormat = DataFormat.Json,
                                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
                            };
                            IPGInputModel Input = new IPGInputModel();
                            Input.RefNum = RefNum;
                            Input.TerminalNumber = Convert.ToInt32(TerminalId);
                            Input.IgnoreNationalcode = true;
                            request.AddBody(Input);
                            var SepResult = restClient.Execute(request);
                            var Result = JsonConvert.DeserializeObject<IPGOutputModel>(SepResult.Content);
                            if (Result.Success == true)
                            {
                                if (Result.VerifyInfo.AffectiveAmount > 0 && Result.VerifyInfo.AffectiveAmount == Convert.ToInt32(Session["Amount"]))
                                {
                                    var onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", ResNum, 0, 1, "").Where(l => l.fldBankID == 33).FirstOrDefault();
                                    int? userid = null;
                                    int? guestUserId = null;
                                    System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));

                                    if (Session["UserId"] != null)
                                        userid = Convert.ToInt32(Session["UserId"]);
                                    else if (Session["GeustId"] != null)
                                        userid = Convert.ToInt32(Session["GeustId"]);
                                    else if (Session["UserGeust"] != null)
                                        guestUserId = Convert.ToInt32(Session["UserGeust"]);
                                    //آپدیت جداول مربوطه
                                    var Description = string.Format("RRN= {0}; CardNumber= {1}; StraceDate= {2}", Result.VerifyInfo.RRN, Result.VerifyInfo.HashedPan, Result.VerifyInfo.StraceDate);

                                    Co.UpdateSamanRefNum(Convert.ToInt32(ResNum), RefNum);
                                    car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, true, RefNum/*رسید دیجیتالی خرید*/, userid, guestUserId, "", Description);

                                    car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
                                                        Convert.ToInt32(onlinepay.fldMony), 10, null, onlinepay.fldID,
                                                        "", userid, "کد رهگیری:" + Result.VerifyInfo.StraceNo + " شماره کارت:" + Result.VerifyInfo.MaskedPan, "", "", null, "", null, null, true, 1, DateTime.Now);
                                    
                                    if (Convert.ToInt32(Session["UserMnu"]) == 1)
                                    {
                                        Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                                        var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, onlinepay.fldID.ToString(), DateTime.Now, "");
                                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                               + k1 + "-" + onlinepay.fldID.ToString() + "\n");
                                    }
                                    try
                                    {
                                        SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                                    }
                                    catch (Exception)
                                    {
                                        ViewBag.InvoiceNamber = Result.VerifyInfo.StraceNo;
                                        ViewBag.Result = "تراکنش موفق";
                                        ViewBag.ResNum = ResNum;
                                        ViewBag.RefNum = RefNum;
                                        ViewBag.Bank = "بانک سامان";
                                        ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
                                        ViewBag.Money = Convert.ToDouble(onlinepay.fldMony).ToString("#,###") + " ريال";
                                        Session["ResidId"] = _id.Value.ToString();
                                        return View();
                                    }
                                    
                                    ViewBag.InvoiceNamber = Result.VerifyInfo.StraceNo;
                                    ViewBag.Result = "تراکنش موفق";
                                    ViewBag.ResNum = ResNum;
                                    ViewBag.RefNum = RefNum;
                                    ViewBag.Bank = "بانک سامان";
                                    ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
                                    ViewBag.Money = Convert.ToDouble(onlinepay.fldMony).ToString("#,###") + " ريال";
                                    Session["ResidId"] = _id.Value.ToString();
                                    return View();
                                }
                                else if (Result.VerifyInfo.AffectiveAmount > 0 && Result.VerifyInfo.AffectiveAmount != Convert.ToInt32(Session["Amount"]))
                                {
                                    string UrlR = "https://sep.shaparak.ir/verifyTxnRandomSessionkey/ipg/ReverseTranscation";
                                    var restClientR = new RestClient(UrlR);
                                    var requestR = new RestRequest()
                                    {
                                        RequestFormat = DataFormat.Json,
                                        OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
                                    };
                                    IPGInputModel InputR = new IPGInputModel();
                                    InputR.RefNum = RefNum;
                                    InputR.TerminalNumber = Convert.ToInt32(TerminalId);
                                    requestR.AddBody(InputR);
                                    var SepResultR = restClientR.Execute(requestR);
                                    var ResultR = JsonConvert.DeserializeObject<IPGOutputModel>(SepResultR.Content);
                                    ViewBag.ResNum = ResNum;
                                    ViewBag.Result = "تراکنش ناموفق: مبلغ به کارت خریدار بازگشت داده شد.";
                                }
                                else if (Result.VerifyInfo.AffectiveAmount < 0)
                                {
                                    ViewBag.Result = "تراکنش ناموفق: خطای " + Result.ResultCode + "(" + Result.ResultDescription + ")";
                                    //Verify.ResNum = ResNum;
                                    //FinalOutPut.ResultDescription = "تراکنش ناموفق: خطای " + Result.ResultCode + "(" + Result.ResultDescription + ")";
                                    //FinalOutPut.Success = false;
                                }
                            }
                            else
                            {
                                ViewBag.Result = "تراکنش ناموفق: خطای " + Result.ResultCode + "(" + Result.ResultDescription + ")";
                                //Verify.ResNum = ResNum;
                                //FinalOutPut.ResultDescription = "تراکنش ناموفق: خطای " + Result.ResultCode + "(" + Result.ResultDescription + ")";
                                //FinalOutPut.Success = false;
                            }
                        }
                        else
                        {
                            ViewBag.Result = "تراکنش ناموفق: تکراری بودن رسید دیجیتالی";
                            //Verify.ResNum = ResNum;
                            //FinalOutPut.ResultDescription = "تراکنش ناموفق: تکراری بودن رسید دیجیتالی";
                            //FinalOutPut.Success = false;
                        }
                    }
                    catch (Exception x)
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                        car.sp_ErrorProgramInsert(Eid, x.InnerException != null ? x.InnerException.Message : "", Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                        ViewBag.Result = "تراکنش ناموفق: خطای شماره " + Eid.Value;
                        //Verify.ResNum = ResNum;
                        //FinalOutPut.ResultDescription = x.Message + x.InnerException != null ? x.InnerException.Message : "";
                        //FinalOutPut.Success = false;
                    }
                }
                else
                {
                    ViewBag.Result = "تراکنش ناموفق: عدم تطابق شماره خرید ارسالی از سمت فروشنده با شماره خرید بازگشتی";
                    //Verify.ResNum = ResNum;
                    //FinalOutPut.ResultDescription = "تراکنش ناموفق: تطابق شماره خرید ارسالی از سمت فروشنده با شماره خرید بازگشتی";
                    //FinalOutPut.Success = false;
                }
            }
            else
            {
                ViewBag.Result = "";
                switch (Status)
                {
                    case "1":
                        ViewBag.Result = "تراکنش ناموفق: کاربر انصراف داده است.";
                        break;
                    case "3":
                        ViewBag.Result = "تراکنش ناموفق: پرداخت انجام نشد.";
                        break;
                    case "4":
                        ViewBag.Result = "تراکنش ناموفق: کاربر در بازه زمانی تعیین شده پاسخی ارسال نکرده است.";
                        break;
                    case "5":
                        ViewBag.Result = "تراکنش ناموفق: پارامترهای ارسالی نامعتبر است.";
                        break;
                    case "8":
                        ViewBag.Result = "تراکنش ناموفق: آدرس سرور پذیرنده نامعتبر است.";
                        break;
                    case "10":
                        ViewBag.Result = "تراکنش ناموفق: توکن ارسال شده یافت نشد.";
                        break;
                    case "11":
                        ViewBag.Result = "تراکنش ناموفق: با این شماره ترمینال فقط تراکنش های توکنی قابل پرداخت هستند.";
                        break;
                    case "12":
                        ViewBag.Result = "تراکنش ناموفق: شماره ترمینال ارسال شده یافت نشد.";
                        break;
                }
                //Verify.ResNum = ResNum;
                //FinalOutPut.ResultDescription = "تراکنش ناموفق: " + State;
                //FinalOutPut.Success = false;
            }
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\SamanKish\" + ResNum);
            //FinalOutPut.VerifyInfo = Verify;
            //Session["FinalOutPut"] = JsonConvert.SerializeObject(FinalOutPut);
            ViewBag.Money = Convert.ToDouble(Session["Amount"]).ToString("#,###") + " ريال";
            ViewBag.Bank = "بانک سامان";
            ViewBag.InvoiceNamber = "";
            ViewBag.RefNum = "";
            ViewBag.ResNum = ResNum;
            ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
            return View();
        }

        //[HttpPost]
        //public ActionResult Back(string BackInfo)
        //{
        //    Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
        //    var Info = JsonConvert.DeserializeObject<IPGOutputModel>(BackInfo);

        //    try
        //    {
        //        var FileSavedSession = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\SamanKish\" + Info.VerifyInfo.ResNum);
        //        var SavedSession = FileSavedSession.Split(',');
        //        Session["Tax"] = SavedSession[0];
        //        Session["UserId"] = SavedSession[1];
        //        Session["ReturnUrl"] = SavedSession[2];
        //        Session["UserPass"] = SavedSession[3];
        //        Session["UserMnu"] = SavedSession[4];
        //        Session["UserState"] = SavedSession[5];
        //        Session["CountryType"] = SavedSession[6];
        //        Session["CountryCode"] = SavedSession[7];
        //        Session["area"] = SavedSession[8];
        //        Session["office"] = SavedSession[9];
        //        Session["Location"] = SavedSession[10];
        //        Session["GeustId"] = SavedSession[11];
        //        Session["OnlinefishId"] = SavedSession[12];
        //        System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\SamanKish\" + Info.VerifyInfo.ResNum);

        //        var onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", Info.VerifyInfo.ResNum, 0, 1, "").Where(l => l.fldBankID == 31).FirstOrDefault();
        //        if (Info.Success)
        //        {
        //            int? userid = null;
        //            int? guestUserId = null;
        //            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));

        //            if (Session["UserId"] != null)
        //                userid = Convert.ToInt32(Session["UserId"]);
        //            else if (Session["GeustId"] != null)
        //                userid = Convert.ToInt32(Session["GeustId"]);
        //            else if (Session["UserGeust"] != null)
        //                guestUserId = Convert.ToInt32(Session["UserGeust"]);
        //            //آپدیت جداول مربوطه
        //            car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, true, Info.VerifyInfo.RefNum, userid, guestUserId, "", BackInfo);
        //            car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
        //                                Convert.ToInt32(onlinepay.fldMony), 10, Convert.ToInt32(Session["OnlinefishId"]), null,
        //                                "", userid, "کد پیگیری:" + Info.VerifyInfo.StraceNo + " شماره کارت:" + Info.VerifyInfo.MaskedPan, "", "", null, "", null, null, true, 1, DateTime.Now);
        //            SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
        //            if (Convert.ToInt32(Session["UserMnu"]) == 1)
        //            {
        //                Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
        //                var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, onlinepay.fldID.ToString(), DateTime.Now, "");
        //                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
        //                       + k1 + "-" + onlinepay.fldID.ToString() + "\n");
        //            }

        //            ViewBag.InvoiceNamber = Info.VerifyInfo.StraceNo;
        //            ViewBag.Result = "تراکنش موفق";
        //            ViewBag.ResNum = Info.VerifyInfo.ResNum;
        //            ViewBag.RefNum = Info.VerifyInfo.RefNum;
        //            ViewBag.Bank = "بانک سامان";
        //            ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
        //            ViewBag.Money = Convert.ToDouble(onlinepay.fldMony).ToString("#,###") + " ريال";
        //            Session["ResidId"] = _id.Value.ToString();
        //        }
        //        else
        //        {
        //            ViewBag.InvoiceNamber = "";
        //            ViewBag.Result = Info.ResultDescription;
        //            ViewBag.ResNum = Info.VerifyInfo.ResNum;
        //            ViewBag.RefNum = "";
        //            ViewBag.Bank = "بانک سامان";
        //            ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
        //            ViewBag.Money = Convert.ToDouble(onlinepay.fldMony).ToString("#,###") + " ريال";
        //            Session["ResidId"] = "";
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
        //        string InnerException = x.InnerException!=null?x.InnerException.Message:"";
        //        car.sp_ErrorProgramInsert(Eid, x.Message, 1, InnerException, DateTime.Now, "");
        //        ViewBag.InvoiceNamber = "";
        //        ViewBag.Result = "خطای شماره " + Eid.Value.ToString();
        //        ViewBag.ResNum = Info.VerifyInfo.ResNum;
        //        ViewBag.RefNum = "";
        //        ViewBag.Bank = "بانک سامان";
        //        ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
        //        ViewBag.Money = Convert.ToDouble(Info.VerifyInfo.AffectiveAmount).ToString("#,###") + " ريال";
        //        Session["ResidId"] = "";
        //    }
        //    return View();
        //}
    }
}
