using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Avarez.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using Avarez.Models;
using Ext.Net;
using Ext.Net.MVC;

namespace Avarez.Areas.NewVer.Controllers.OnlinePay
{
    public class SadadController : Controller
    {
        //
        // GET: /NewVer/Sadad/
        public ActionResult Index()
        {
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            try
            {
                Type type = typeof(PaymentRequest);
                var obj = Activator.CreateInstance(type);

                var q = car.sp_BankParameterSelect("fldBankID", "32", 0, 1, "", Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["CountryType"])).ToList();
                foreach (var kv in q)
                {
                    if (type.GetProperty(kv.fldPropertyNameEN) != null)
                        type.GetProperty(kv.fldPropertyNameEN).SetValue(obj, q.Where(l => l.fldPropertyNameEN == kv.fldPropertyNameEN).FirstOrDefault().value, null);
                }
                //ایجاد آبجکت درخواست
                PaymentRequest PR = (PaymentRequest)obj;
                PR.Amount = Convert.ToInt64(Session["Amount"]);
                PR.OrderId = Session["Tax"].ToString();
                PR.TerminalId = PR.TerminalId;
                Session["ReturnUrl"] = string.Format("{0}/NewVer/First/Index", PR.ReturnUrl);
                //رمز نگاری            
                string serializedPR = Newtonsoft.Json.JsonConvert.SerializeObject(PR);
                byte[] pwd = Encoding.Unicode.GetBytes("This Is a pass for post data to ecartax");
                byte[] salt = CreateRandomSalt(7);
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(pwd, salt);
                TripleDESCryptoServiceProvider objt = new TripleDESCryptoServiceProvider();
                byte[] EnctArray = UTF8Encoding.UTF8.GetBytes(serializedPR);
                objt.Key = pdb.CryptDeriveKey("TripleDES", "SHA1", 192, objt.IV);
                objt.Mode = CipherMode.ECB;
                objt.Padding = PaddingMode.PKCS7;
                ICryptoTransform crptotrns = objt.CreateEncryptor();
                byte[] resArray = crptotrns.TransformFinalBlock(EnctArray, 0, EnctArray.Length);
                objt.Clear();
                Session["SadadInfo"] = Convert.ToBase64String(resArray, 0, resArray.Length);
                //نگهداری سشن ها جهت استفاده از آنها در زمان بازگشت
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"Sadad\" + Session["Tax"].ToString(),
                        Session["Tax"].ToString() + "," + Session["Amount"] + "," + Session["UserId"] + ","
                        + Session["ReturnUrl"] + "," + Session["UserMnu"] + "," + Session["UserGeust"] + ","
                        + Session["GeustId"] + "," + Session["OnlinefishId"]);
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }
            catch (Exception x)
            {
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                var Msg = x.InnerException != null ? x.InnerException.Message : x.Message;
                car.sp_ErrorProgramInsert(Eid, Msg, 1, "Index/Catch", DateTime.Now, "");                
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                });
                DirectResult result = new DirectResult();
                return result;
            }            
        }
        public static byte[] CreateRandomSalt(int length)
        {
            // Create a buffer
            byte[] randBytes;

            if (length >= 1)
            {
                randBytes = new byte[length];
            }
            else
            {
                randBytes = new byte[1];
            }
            // Create a new RNGCryptoServiceProvider.
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
            // Fill the buffer with random bytes.
            rand.GetBytes(randBytes);
            // return the bytes.
            return randBytes;
        }
        [HttpPost]
        public ActionResult PaymentReq(string SerializedPay)
        {
            try
            {
                PaymentRequest PR = new PaymentRequest();
                byte[] pwd = Encoding.Unicode.GetBytes("This Is a pass for post data to ecartax");
                byte[] salt = CreateRandomSalt(7);
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(pwd, salt);
                TripleDESCryptoServiceProvider objt = new TripleDESCryptoServiceProvider();

                byte[] EnctArray = Convert.FromBase64String(SerializedPay);
                objt.Key = pdb.CryptDeriveKey("TripleDES", "SHA1", 192, objt.IV);
                objt.Mode = CipherMode.ECB;
                objt.Padding = PaddingMode.PKCS7;
                ICryptoTransform crptotrns = objt.CreateDecryptor();
                byte[] resArray = crptotrns.TransformFinalBlock(EnctArray, 0, EnctArray.Length);
                objt.Clear();
                var SerializedPR=UTF8Encoding.UTF8.GetString(resArray);
                PR = JsonConvert.DeserializeObject<PaymentRequest>(SerializedPR);
                //افزودن اطلاعات مورد نیاز به آبجکت درخواست
                var dataBytes = Encoding.UTF8.GetBytes(string.Format("{0};{1};{2}", PR.TerminalId, PR.OrderId, PR.Amount));
                var symmetric = SymmetricAlgorithm.Create("TripleDes");
                symmetric.Mode = CipherMode.ECB;
                symmetric.Padding = PaddingMode.PKCS7;
                var encryptor = symmetric.CreateEncryptor(Convert.FromBase64String("R3nf8GnOIkm+5V5JIAfcr0UicxtbxX03"), new byte[8]);

                PR.SignData = Convert.ToBase64String(encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length));
                PR.LocalDateTime = DateTime.Now;
                PR.MerchantId = "000000140341524";
                PR.PurchasePage = "https://sadad.shaparak.ir";
                PR.MultiplexingData = null;
                PR.ReturnUrl = "http://ecartax.ir/NewVer/Sadad/Verify";//string.Format("{0}://{1}{2}NewVer/Sadad/Verify", Request.Url.Scheme, PR.ReturnUrl, Url.Content("~"));
                //ارسال اطلاعات و دریافت توکن
                var ipgUri = string.Format("{0}/api/v0/Request/PaymentRequest",PR.PurchasePage);
                var res = CallApi<PayResultData>(ipgUri, PR);
                res.Wait();
                if (res != null && res.Result != null)
                {
                    if (res.Result.ResCode == "0")
                    {
                        Response.Redirect(string.Format("{0}/Purchase/Index?token={1}", "https://sadad.shaparak.ir", res.Result.Token));
                    }
                    return Json(res.Result.Description, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("خطای نامشخص", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                var msg = x.InnerException != null?x.InnerException.Message:x.Message;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Verify(PurchaseResult result)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();            
            try
            {
                var dataBytes = Encoding.UTF8.GetBytes(result.Token);
                var symmetric = SymmetricAlgorithm.Create("TripleDes");
                symmetric.Mode = CipherMode.ECB;
                symmetric.Padding = PaddingMode.PKCS7;
                var encryptor = symmetric.CreateEncryptor(Convert.FromBase64String("R3nf8GnOIkm+5V5JIAfcr0UicxtbxX03"), new byte[8]);
                var signedData = Convert.ToBase64String(encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length));
                var data = new
                {
                    token = result.Token,
                    SignData = signedData
                };
                var ipgUri = string.Format("{0}/api/v0/Advice/Verify", "https://sadad.shaparak.ir");
                var res = CallApi<VerifyResultData>(ipgUri, data);
                if (res != null && res.Result != null)
                {
                    var ss = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Sadad\" + result.OrderId);
                    var ss1 = ss.Split(',');
                    Session["Tax"] = ss1[0];
                    Session["Amount"] = ss1[1];
                    Session["UserId"] = ss1[2];
                    Session["ReturnUrl"] = ss1[3];
                    Session["UserMnu"] = ss1[4];
                    Session["UserGeust"] = ss1[5];
                    Session["GeustId"] = ss1[6];
                    Session["OnlinefishId"] = ss1[7];

                    if (res.Result.ResCode == "0" && Session["Tax"].ToString() == result.OrderId &&
                        Session["Amount"].ToString() == res.Result.Amount)
                    {
                        int? userid = null;
                        int? guestUserId = null;
                        if (Session["UserId"] != null)
                            userid = Convert.ToInt32(Session["UserId"]);
                        else if (Session["GeustId"] != null)
                            userid = Convert.ToInt32(Session["GeustId"]);
                        else if (Session["UserGeust"] != null)
                            guestUserId = Convert.ToInt32(Session["UserGeust"]);

                        System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\Sadad\" + result.OrderId);
                        var onlinepay = car.sp_OnlinePaymentsSelect("fldTemporaryCode", result.OrderId.ToString(), 0, 1, "")
                            .Where(l => l.fldBankID == 32 && l.fldMunID == Convert.ToInt32(Session["UserMnu"])).FirstOrDefault();
                        //اینزرت در جداول مربوطه
                        car.sp_OnlinePaymentsFinalPaymentUpdate(onlinepay.fldID, true, res.Result.RetrivalRefNo, userid, guestUserId,
                            "", res.Result.SystemTraceNo);

                        System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                        car.sp_CollectionInsert(_id, onlinepay.fldCarFileID, DateTime.Now,
                            Convert.ToInt32(onlinepay.fldMony), 10, Convert.ToInt32(Session["OnlinefishId"]), null,
                            "", userid, "کد پیگیری:" + res.Result.SystemTraceNo + " شرح نتیجه:" +
                            res.Result.Description, "", "", null, "", null, null, true, 1, DateTime.Now);
                        SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));

                        if (Convert.ToInt32(Session["UserMnu"]) == 1)
                        {
                            Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                            var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, onlinepay.fldID.ToString(), DateTime.Now, "");
                            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                   + k1 + "-" + onlinepay.fldID.ToString() + "\n");
                        }

                        SmsSender sms = new SmsSender();
                        sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, onlinepay.fldCarFileID, (int)onlinepay.fldMony, "", "", "");

                        Session["ResidId"] = _id.Value;
                        res.Result.Amount = Convert.ToDouble(res.Result.Amount).ToString("#,###") + " ريال";
                        ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
                        return View("Verify",res.Result);
                    }
                    else
                    {
                        return View("Verify", res.Result);
                    }
                }
                return View("Verify", res.Result);
            }
            catch (Exception ex)
            {
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                var Msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                car.sp_ErrorProgramInsert(Eid, Msg, 1,"Verify/Catch", DateTime.Now, "");
                return Json("خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", JsonRequestBehavior.AllowGet);
            }
        }
        public static async Task<T> CallApi<T>(string apiUrl, object value)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    var w = client.PostAsJsonAsync(apiUrl, value);
                    w.Wait();
                    HttpResponseMessage response = w.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsAsync<T>();
                        result.Wait();
                        return result.Result;
                    }
                    return default(T);
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                var Msg = x.InnerException != null ? x.InnerException.Message : x.Message;
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                car.sp_ErrorProgramInsert(Eid, Msg, 1, "CallApi/Catch", DateTime.Now, "");
                return default(T);
            }            
        }
    }
}
