using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Avarez.Areas.NewVer.Controllers.Guest
{
    public class Login_GuestController : Controller
    {
        //
        // GET: /NewVer/Login_Guest/

        public ActionResult Index()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();

            return PartialView;
        }

        public ActionResult Register()
        {
            return View();
        }
        public JsonResult GetCascadeState()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var ImageSetting = Convert.ToInt32(ConfigurationManager.AppSettings["ImageSetting"]);
            if (ImageSetting == 6)
                return Json(car.sp_StateSelect("fldid", "7", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
            else
                return Json(car.sp_StateSelect("", "", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeCounty(int ID)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(1, ID, 5).ToList().OrderBy(h => h.NodeName);
            var ImageSetting = Convert.ToInt32(ConfigurationManager.AppSettings["ImageSetting"]);
            if (ImageSetting == 6)
                County = car.sp_RegionTree(1, ID, 5).Where(l=>l.SourceID==406).ToList().OrderBy(h => h.NodeName);
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ForgetPass()
        {
            return View();
        }
        public ActionResult VerificationCode(string Mobile,string Name,string Gender)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();

            if (Mobile != "0")
            {
                PartialView.ViewBag.Mobile = Mobile;
                PartialView.ViewBag.Name = Name;
                PartialView.ViewBag.Gender = Gender;
                return PartialView;
            }
            else
            {
                ViewBag.Mobile = Mobile;
                ViewBag.Name = Name;
                ViewBag.Gender = Gender;
                return View();
            }
        }

        public ActionResult GetNationalCode(string Mobile)
        {
       
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.sp_GuestUserSelect("fldMobile", Mobile, "", 1).FirstOrDefault();

                return Json(new
                {
                    CodeMeli = q.fldNationalCode,
                    TarikhTavalod = q.fldTarikhTavalod
                }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Vorod(string UserName, string Password, string Capthalogin, string CboMnu, string cboState)
        {

            string Msg1 = ""; var Er = 0; var flag = false; string MsgTitle = ""; string Msg = "";
            Models.cartaxEntities p = new Models.cartaxEntities();
            if (Convert.ToInt32(Session["captchahave"]) > 1)
            {
                if (Capthalogin == "")
                {
                    Session["captchaLogin"] = "Error";
                    MsgTitle = "خطا";
                    Msg1 = "لطفا کد امنیتی را وارد نمایید.";
                    Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                    return Json(new
                    {
                        Msg = Msg1,
                        MsgTitle = MsgTitle,
                        ER = 1,
                        captchahave = Session["captchahave"]
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    if (Capthalogin.ToLower() != Session["captchaLogin"].ToString().ToLower())
                    {
                        Session["captchaLogin"] = "Error";
                        MsgTitle = "خطا";
                        Msg1 = "لطفا کد امنیتی را صحیح وارد نمایید.";
                        Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                        return Json(new
                        {
                            Msg = Msg1,
                            MsgTitle = MsgTitle,
                            ER = 1,
                            captchahave = Session["captchahave"]
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            Password = Password.GetHashCode().ToString();
            var u = p.sp_GuestUserSelect("CheckPass", UserName, Password, 0).FirstOrDefault();
            if (u != null)
            {
                Session["UserMnu"] = u.fldMunId;//آیدی شهرداری که کاربر با آن لاگین کرده
                Session["UserState"] = u.fldStateId;//آیدی استانی که کاربر با آن لاگین کرده
                Session["UserGeust"] = u.fldId;//آیدی کاربر خوداضحاری
                Session["UserPassGuest"] = Password;
                Session["CountryType"] = 5;
                Session["CountryCode"] = u.fldMunId;
                return RedirectToAction("Index", "First");
            }
            else
            {
                Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                return Json(new
                {
                    Msg = "نام کاربری یا کلمه عبور صحیح نیست.",
                    MsgTitle = "ورود ناموفق",
                    ER = 1,
                    captchahave = Session["captchahave"]
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult checkMobail(string Mobail)
        {
            var flag = false;
            Models.cartaxEntities p = new Models.cartaxEntities();
            var q = p.sp_GuestUserSelect("fldMobile", Mobail, "", 100).FirstOrDefault();
            if (q != null)
                flag = true;
            return Json(new { flag = flag }, JsonRequestBehavior.AllowGet);
        }

        public bool CheckMobileNumber(string MobileNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(MobileNumber, "(^(09|9)[0-9][0-9]\\d{7}$)");
        }

        public string SendMessage(string Mobile, string CodeTaaid, int fldMunId, string Name, string gender)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();

            System.Data.Entity.Core.Objects.ObjectParameter IDCountryDivision = new System.Data.Entity.Core.Objects.ObjectParameter("ID", typeof(int));
            p.sp_INSERT_IDCountryDivisions(5, fldMunId, 1, IDCountryDivision);

            var haveSmsPanel = p.sp_SMSSettingSelect("fldCountryDivisionId", IDCountryDivision.Value.ToString(), 1, 0, "").FirstOrDefault();
            if (haveSmsPanel == null&&fldMunId!=406)
            {
                return "امکان تشکیل پرونده در حوزه انتخابی وجود ندارد.";
            }
            byte a=1;
            string[] m =new string[] {Mobile};
            Random rand = new Random();
            string crand = rand.Next(11111111, 99999999).ToString();

            SmsPanel.RasaSMSPanel_Send Sms = new SmsPanel.RasaSMSPanel_Send();
            Sms.Timeout = 500000000;
            string MatnSMS = "";
            if (gender == "0")
            {
                MatnSMS = "خانم "+Name+" ثبت نام شما با موفقیت انجام شد برای تکمیل ثبت نام کد زیر را در سامانه وارد نمایید." + Environment.NewLine + "کد تائید: " + CodeTaaid + Environment.NewLine + "سامانه جامع هوشمند عوارض خودرو";
            }
            else if (gender == "1")
            {
                MatnSMS = "آقا " + Name + " ثبت نام شما با موفقیت انجام شد برای تکمیل ثبت نام کد زیر را در سامانه وارد نمایید." + Environment.NewLine + "کد تائید: " + CodeTaaid + Environment.NewLine + "سامانه جامع هوشمند عوارض خودرو";
            }
            else
            {
                MatnSMS = "ثبت نام شما با موفقیت انجام شد برای تکمیل ثبت نام کد زیر را در سامانه وارد نمایید." + Environment.NewLine + "کد تائید: " + CodeTaaid + Environment.NewLine + "سامانه جامع هوشمند عوارض خودرو";
            }
            try
            {
                if (Mobile != "")
                {
                    if (CheckMobileNumber(Mobile))
                    {
                        if (fldMunId != 406)
                        {
                            var returnCode = Sms.SendMessage(haveSmsPanel.fldUserName, haveSmsPanel.fldPassword, new string[] { Mobile }, MatnSMS, 1, haveSmsPanel.fldLineNumber);
                            int smsid;
                            if (int.TryParse(returnCode[0], out smsid) == true)
                                return "";
                            else
                                return "خطا";
                        }
                        else
                        {
                            var loginUrl = "http://192.168.100.42/api/login";
                            var sendUrl = "http://192.168.100.42/api/sms/SendSingle";
                            var httpWebRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
                            httpWebRequest.ContentType = "application/json";
                            httpWebRequest.Method = "POST";
                            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                            {
                                var json = "{\"username\":\"admin\",\"password\":\"admin$1\"}";
                                streamWriter.Write(json);
                                streamWriter.Flush();
                                streamWriter.Close();
                            }
                            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            var psid = httpResponse.Headers["PSID"].ToString();

                            httpWebRequest = (HttpWebRequest)WebRequest.Create(sendUrl);
                            httpWebRequest.ContentType = "application/json";
                            httpWebRequest.Method = "POST";
                            httpWebRequest.Headers.Add("PSID", psid);
                            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                            {
                                var json = "{\"recipients\":[\"" + Mobile + "\"],\"messageText\":\"" + MatnSMS + "\"}";
                                streamWriter.Write(json);
                                streamWriter.Flush();
                                streamWriter.Close();
                            }
                            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            return "";
                        }
                    }
                    else
                        return "شماره موبایل وارد شده نامعتبر است";
                }
                else
                    return "لطفا شماره موبایل خود را وارد نمایید.";
            }
            catch (Exception)
            {
                return "خطا در اتصال به سرور";
            }
        }

        public string SendMessageRegister(string Mobile, string MatnSMS,string CountryDivisionId)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var haveSmsPanel = p.sp_SMSSettingSelect("fldCountryDivisionId", CountryDivisionId, 1, 0, "").FirstOrDefault();
            var ImageSetting = Convert.ToInt32(ConfigurationManager.AppSettings["ImageSetting"]);
            if (haveSmsPanel == null && ImageSetting != 6)
            {
                return "امکان تشکیل پرونده در حوزه انتخابی وجود ندارد.";
            }

            SmsPanel.RasaSMSPanel_Send Sms = new SmsPanel.RasaSMSPanel_Send();
            Sms.Timeout = 500000000;
              if (Mobile != "")
            {
                if (CheckMobileNumber(Mobile))
                {
                    if (ImageSetting != 6)
                    {
                        var returnCode = Sms.SendMessage(haveSmsPanel.fldUserName, haveSmsPanel.fldPassword, new string[] { Mobile }, MatnSMS, 1, haveSmsPanel.fldLineNumber);
                        int smsid;
                        if (int.TryParse(returnCode[0], out smsid) == true)
                            return "";
                        else
                            return "خطا";
                    }
                    else
                    {
                        var loginUrl = "http://192.168.100.42/api/login";
                        var sendUrl = "http://192.168.100.42/api/sms/SendSingle";
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            var json = "{\"username\":\"admin\",\"password\":\"admin$1\"}";
                            streamWriter.Write(json);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        var psid = httpResponse.Headers["PSID"].ToString();

                        httpWebRequest = (HttpWebRequest)WebRequest.Create(sendUrl);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.Headers.Add("PSID", psid);
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            var json = "{\"recipients\":[\"" + Mobile + "\"],\"messageText\":\"" + MatnSMS + "\"}";
                            streamWriter.Write(json);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                        httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        return "";
                    }
                }
                else
                    return "شماره موبایل وارد شده نامعتبر است";
            }
            else
                return "لطفا شماره موبایل خود را وارد نمایید.";

        }
        public int checks(string codec)
        {
            char[] chArray = codec.ToCharArray();
            int[] numArray = new int[chArray.Length];
            string x = codec;
            switch (x)
            {
                case "0000000000":
                case "1111111111":
                case "2222222222":
                case "3333333333":
                case "4444444444":
                case "5555555555":
                case "6666666666":
                case "7777777777":
                case "8888888888":
                case "9999999999":
                case "0123456789":
                case "9876543210":

                    return 0;
                    break;
            }
            for (int i = 0; i < chArray.Length; i++)
            {
                numArray[i] = (int)char.GetNumericValue(chArray[i]);
            }
            int num2 = numArray[9];

            int num3 = ((((((((numArray[0] * 10) + (numArray[1] * 9)) + (numArray[2] * 8)) + (numArray[3] * 7)) + (numArray[4] * 6)) + (numArray[5] * 5)) + (numArray[6] * 4)) + (numArray[7] * 3)) + (numArray[8] * 2);
            int num4 = num3 - ((num3 / 11) * 11);
            if ((((num4 == 0) && (num2 == num4)) || ((num4 == 1) && (num2 == 1))) || ((num4 > 1) && (num2 == Math.Abs((int)(num4 - 11)))))
            {
                return 1;
            }
            else
            {
                return 0;

            }
        }
        public ActionResult Save(Models.sp_GuestUserSelect SabteName, string Name, string gender, string Captcha, string FatherName, string officeName, string ShenasnameNo, string ShenasnameSeri)
        {
            string Msg = "", MsgTitle = ""; var Er = 0; string s = "";var DateBirthday="";
            
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                if (Captcha == "")
                {
                    //Session["captchaLogin"] = "Error";
                     return Json(new
                    {
                        Msg = "لطفا کد امنیتی را وارد نمایید.",
                        MsgTitle = "خطا",
                        Err = 1
                        //captchahave = Session["captchahave"]
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    if (Captcha.ToLower() != Session["captchaLogin"].ToString().ToLower())
                    {
                        //Session["captchaLogin"] = "Error";
                   
                        return Json(new
                        {
                            Msg = "لطفا کد امنیتی را صحیح وارد نمایید.",
                            MsgTitle = "خطا",
                            Err = 1
                           // captchahave = Session["captchahave"]
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
                CaptchaImage img = new CaptchaImage(110, 40, family);
                string CodeTaeed = img.CreateRandomText(5);
                if (SabteName.fldType == true)
                {
                    var chck = checks(SabteName.fldNationalCode);
                    if (chck != 1)
                    {
                        return Json(new
                        {            
                            Msg = "کد ملی وارد شده نامعتبر است.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (SabteName.fldId == 0)
                {
                    //ذخیره
                    //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 3))
                    //{
                   // Avarez.RasaSabt.Estelam sabt = new Avarez.RasaSabt.Estelam();
                    var q = p.sp_GuestUserSelect("fldMobile", SabteName.fldMobile, "", 100).FirstOrDefault();
                    var q1 = p.sp_GuestUserSelect("fldNationalCode", SabteName.fldNationalCode, "", 100).FirstOrDefault();
                    if (SabteName.fldType == true)
                    {
                        DateBirthday=SabteName.fldTarikhTavalod;
                    }
                    else
                    {
                        DateBirthday="";
                    }
                    if (q == null && q1 == null)
                    {

                        s = SendMessage(SabteName.fldMobile, CodeTaeed, SabteName.fldMunId,Name,gender);
                        if (s != "")
                        {
                            MsgTitle = "خطا";
                            Msg = s;
                            Er = 1;
                        }
                        else
                        {
                            p.sp_GuestUserInsert(SabteName.fldMobile, SabteName.fldNationalCode, CodeTaeed, "", false, SabteName.fldType, SabteName.fldMunId, SabteName.fldStateId, DateBirthday, "", Name, FatherName,ShenasnameSeri, ShenasnameNo,  officeName);
                        }
                    }
                    else
                    {
                        if (q1 != null)
                        {
                            if (SabteName.fldType == true)
                            {
                                Msg = "کدملی وارد شده تکراری است.";
                            }
                            else
                            {
                                Msg = "کد اقتصادی وارد شده تکراری است.";
                            }
                        }
                        else if (q != null)
                        {
                            Msg = "موبایل وارد شده تکراری است.";
                        }
                        MsgTitle = "خطا";
                        Er = 1;

                    }
                    //}
                    //else
                    //{
                    //    return null;
                    //}
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {

                /// متدی برای چک کردن کد تایید ارسال شده به موبایل
                /// </summary>
                /// <param name="CodeTaaid"></param>
                /// <param name="Mobail"></param>              
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// <returns></returns>
        public ActionResult CheckCodeTaaid(string CodeTaaid, string Mobail, string Name, string Gender)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                var q = m.sp_GuestUserSelect("fldMobile", Mobail, "", 1).FirstOrDefault();
       
                System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
                CaptchaImage img = new CaptchaImage(110, 40, family);
                string Password = img.CreateRandomText(5);
                string Pas = Password.GetHashCode().ToString();
                //bool? Flag = true;
                if (q != null)
                {
                    if (q.fldAcceptCode == CodeTaaid)
                    {
                        if (q.fldFlag == false)
                        {
                            System.Data.Entity.Core.Objects.ObjectParameter IDCountryDivision = new System.Data.Entity.Core.Objects.ObjectParameter("ID", typeof(int));
                            m.sp_INSERT_IDCountryDivisions(5, q.fldMunId, 1, IDCountryDivision);
                           var Matn ="";
                           if (Gender == "0")
                               Matn = "خانم " + Name + " تکمیل ثبت نام شما با موفقیت انجام شد، شناسه کاربری و گذرواژه شما به صورت زیر است: " + Environment.NewLine + "شناسه کاربری شما: " + q.fldNationalCode + ' ' + Environment.NewLine + "گذرواژه شما: " + Password + Environment.NewLine + "سامانه جامع یکپارچه و هوشمند تشخیص و وصول عوارض خودرو";
                           else if (Gender == "1")
                               Matn = "آقای " + Name + " تکمیل ثبت نام شما با موفقیت انجام شد، شناسه کاربری و گذرواژه شما به صورت زیر است: " + Environment.NewLine + "شناسه کاربری شما: " + q.fldNationalCode + ' ' + Environment.NewLine + "گذرواژه شما: " + Password + Environment.NewLine + "سامانه جامع یکپارچه و هوشمند تشخیص و وصول عوارض خودرو";
                           else
                               Matn = "کاربر گرامی تکمیل ثبت نام شما با موفقیت انجام شد، شناسه کاربری و گذرواژه شما به صورت زیر است: " + Environment.NewLine + "شناسه کاربری شما: " + q.fldNationalCode + ' ' + Environment.NewLine + "گذرواژه شما: " + Password + Environment.NewLine + "سامانه جامع یکپارچه و هوشمند تشخیص و وصول عوارض خودرو";

                            
                            var s = SendMessageRegister(Mobail, Matn, IDCountryDivision.Value.ToString());
                            if (s != "")
                            {
                                MsgTitle = "خطا";
                                Msg = s;
                                Er = 1;
                            }
                            else
                            {
                                m.sp_GuestUserPassword(q.fldId, Pas,true);
                                Msg = "ثبت نام شما با موفقیت تکمیل شد و رمز عبور ارسال گردید.";
                                MsgTitle = "عملیات موفق";
                            }
                        }
                        else if (q.fldFlag == true)
                        {
                            return Json(new
                            {
                                Msg = "قبلا مشخصات برای شما ارسال شده است.",
                                MsgTitle = "اخطار",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        Msg = "کد تائید وارد شده صحیح نمی باشد.";
                        MsgTitle = "خطا";
                        Er = 1;
                        return Json(new
                        {
                            Msg = Msg,
                            MsgTitle = MsgTitle,
                            Er = Er
                        }, JsonRequestBehavior.AllowGet);

                    }

                }
                else
                {
                    return Json(new
                    {
                         Msg = "شماره موبایل وارد شده نامعتبر است.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveForget(string CodeMeli, string Mobail)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                var q = m.sp_GuestUserSelect("ForgetPass",CodeMeli, Mobail,  1).FirstOrDefault();

                System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
                CaptchaImage img = new CaptchaImage(110, 40, family);
                string Password = img.CreateRandomText(5);
                string Pas = Password.GetHashCode().ToString();
                //bool? Flag = true;
                if (q != null)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter IDCountryDivisionn = new System.Data.Entity.Core.Objects.ObjectParameter("ID", typeof(int));
                    m.sp_INSERT_IDCountryDivisions(5, q.fldMunId, 1, IDCountryDivisionn);
                        if (q.fldFlag == true)
                        {

                            var Matn = "کاربر گرامی بازنشانی رمز عبور با موفقیت انجام شد، شناسه کاربری و گذرواژه شما به صورت زیر است " + Environment.NewLine + "شناسه کاربری شما: " + q.fldNationalCode + ' ' + Environment.NewLine + "گذرواژه شما: " + Password + Environment.NewLine + "سامانه جامع یکپارچه و هوشمند تشخیص و وصول عوارض خودرو";

                            var s = SendMessageRegister(Mobail, Matn, IDCountryDivisionn.Value.ToString());
                            if (s != "")
                            {
                                MsgTitle = "خطا";
                                Msg = s;
                                Er = 1;
                            }
                            else
                            {
                                m.sp_UpdatePassGuestUser(Pas,q.fldId);
                                Msg = "بازنشانی رمز عبور با موفقیت انجام شد و رمز عبور ارسال گردید.";
                                MsgTitle = "عملیات موفق";
                            }
                        }
                        else if (q.fldFlag == true)
                        {
                            return Json(new
                            {
                                Msg = "ثبت نام شما تکمیل نشده است. ابتدا ثبت نام خود را تکمیل نمایید.",
                                MsgTitle = "اخطار",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                 

                }
                else
                {
                    return Json(new
                    {
                        Msg = "اطلاعات وارد شده نامعتبر است.",
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Destroy()
        {
            return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
        }
       /* public ActionResult PassSend(string fldEmail)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                var q = servic.GetFirstRegisterWithFilter("fldEmail", fldEmail, "", 1, out Err).FirstOrDefault();
                var p = servic.GetPageHtmlWithFilter("fldId", "1", 1, out Err).FirstOrDefault();
                var address = Regex.Replace(p.fldMohtavaHtml, "<.*?>", String.Empty);

                System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
                CaptchaImage img = new CaptchaImage(110, 40, family);
                string Password = img.CreateRandomText(5);
                var Pas = servic.HashPass(Password);

                if (q != null)
                {
                    if (q.fldFalg == true)
                    {

                        //MsgTitle = "ذخیره موفق";
                        //Msg = servic.Email_SendEmail(q.fldEmail, "کاربر گرامی، شناسه کاربری و گذرواژه شما به ترتیب زیر است:" + Environment.NewLine
                        //    + "شناسه کاربری:" + q.fldUserName + Environment.NewLine + "گذرواژه:" + Password + Environment.NewLine, "سامانه جامع هوشمند تشخیص صلاحیت ذینفعان راه آهن جمهوری اسلامی ایران ", "", out Err);

                        var EmailSetting = servic.GetEmailSettingWithFilter("", "", 1, out Err).FirstOrDefault();
                        MailAddress from = new MailAddress(EmailSetting.fldAddressEmail);

                        MailAddress to = new MailAddress(q.fldEmail);

                        MailMessage mail = new MailMessage(from, to);
                        string savePath = Server.MapPath(@"~\Content\header.png");
                        mail.IsBodyHtml = true;
                        var inlineLogo = new LinkedResource(savePath);
                        inlineLogo.ContentId = Guid.NewGuid().ToString();
                        mail.Subject = "سامانه تشخیص صلاحیت نظام فنی و اجرایی راه آهن جمهوری اسلامی ایران";
                        string body = string.Format(@"
                            <img src=""cid:{0}"" />
                            <p dir='rtl' align='right' style ='font-family:Tahoma;font-size: 11px;font-weight:bold;'>کاربر گرامی در خواست شما مبنی بر تغییر رمز عبور انجام شد، شناسه کاربری و گذرواژه شما به صورت زیر است :</p>
                            <p dir='rtl' align='right'  style ='font-family:Tahoma;font-size: 11px;font-weight:bold;'>شناسه کاربری شما: " + q.fldUserName + ' ' + "<br />" + "گذرواژه شما: " + Password + "</p>" +
                            "<p dir='rtl' align='right' style ='font-family:Tahoma;font-size: 11px;font-weight:bold;'>با احترام،</p>" +
                            "<p dir='rtl' align='right' style ='font-family:Tahoma;font-size: 11px;font-weight:bold;'>سامانه تشخیص صلاحیت نظام فنی و اجرایی راه آهن جمهوری اسلامی ایران</p>" +
                            "<p dir='rtl' align='right' style ='font-family:Tahoma;font-size: 11px;font-weight:bold;'>" + address + "</p>"
                        , inlineLogo.ContentId);

                        var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                        view.LinkedResources.Add(inlineLogo);
                        mail.AlternateViews.Add(view);

                        //string attachmentPath = savePath;
                        //Attachment inline = new Attachment(attachmentPath);
                        //inline.ContentDisposition.Inline = true;
                        //inline.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        //inline.ContentId = "1";
                        //inline.ContentType.MediaType = "image/png";
                        //inline.ContentType.Name = Path.GetFileName(attachmentPath);

                        // mail.Attachments.Add(inline);
                        // mail.Body = "<img src=\"cid:header.png\" alt=\"\" />";
                        //string htmlBody = "<html><body><h1>Picture</h1><br><img src=\"cid:/Content/header.png\"></body></html>";
                        //AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                        //   (htmlBody, null, MediaTypeNames.Text.Html);

                        //LinkedResource inline = new LinkedResource(savePath, MediaTypeNames.Image.Jpeg);
                        //inline.ContentId = Guid.NewGuid().ToString();
                        //avHtml.LinkedResources.Add(inline);

                        //mail.AlternateViews.Add(avHtml);   

                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = EmailSetting.fldSendServer;
                        smtp.Port = EmailSetting.fldSendPort;
                        smtp.EnableSsl = EmailSetting.fldSSL;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(
                        EmailSetting.fldAddressEmail, EmailSetting.fldPassword);
                        smtp.Send(mail);
                        servic.PasswordUpdate(Pas, q.fldId, out Err);
                        Msg = "بازنشانی رمز عبور شما با موفقیت انجام شد و رمز عبور به ایمیل شما ارسال گردید.";
                        MsgTitle = "عملیات موفق";


                        if (Err.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err.ErrorMsg;
                            Er = 1;
                        }
                    }
                    else
                    {
                        MsgTitle = "خطا";
                        Msg = "ابتدا ثبت نام خود را تکمیل نمایید.";
                        Er = 1;
                    }
                }
                else
                {
                    Msg = "شما با پست الکترونیک وارد شده ثبت نام نکرده اید";
                    MsgTitle = "اخطار";
                    Er = 1;
                }


            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }*/

    }
}
