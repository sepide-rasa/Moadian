using Avarez.Models;
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Avarez.Areas.NewVer.Controllers
{
    public class Account_NewController : Controller
    {
        //
        // GET: /NewVer/Account_New/
        [AllowAnonymous]
        public FileContentResult generateCaptcha(string dc)
        {
            System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
            CaptchaImage img = new CaptchaImage(90, 40, family);
            string text = img.CreateRandomText(5);
            text = text.ToUpper();
            text = text.Replace("O", "P").Replace("0", "2").Replace("1", "3").Replace("I", "M");
            img.SetText(text);
            img.GenerateImage();
            MemoryStream stream = new MemoryStream();
            img.Image.Save(stream,
            System.Drawing.Imaging.ImageFormat.Png);
            Session["captchaLogin"] = text;
            return File(stream.ToArray(), "jpg");
        }
        public FileContentResult generateCaptchaD(string dc)
        {
            System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
            CaptchaImage img = new CaptchaImage(130, 60, family);
            string text = img.CreateRandomText(5);
            text = text.ToUpper();
            text = text.Replace("O", "P").Replace("0", "2").Replace("1", "3").Replace("I", "M");
            img.SetText(text);
            img.GenerateImageOffic();
            MemoryStream stream = new MemoryStream();
            img.Image.Save(stream,
            System.Drawing.Imaging.ImageFormat.Png);
            Session["captchaLoginD"] = text;
            return File(stream.ToArray(), "jpg");
        }
        [AllowAnonymous]
         
        public ActionResult Index()
        {
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (Session["captchahaveD"] == null)
                Session["captchahaveD"] = 0;
            if (ImageSetting == "1")
                return View("logont");
                //return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (ImageSetting == "2")
                return View("logonQ"); 
            if (ImageSetting == "3")
                return View("logonz");
            
            ViewBag.captchahaveD = Convert.ToInt32(Session["captchahaveD"]);
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var News = car.sp_NewsSelect("ISNULL", "", 0, 1, "").ToList();
            var KhabarText = "";
            var KhabarID = "";
            foreach (var item in News)
            {
                KhabarText = KhabarText + item.fldSubject + ";";
                KhabarID = KhabarID + item.fldID + ";";
            }
            ViewBag.KhabarText = KhabarText;
            ViewBag.KhabarID = KhabarID;
            return View();
        }

        public FileContentResult DownloadHelp(string state)

        {
            if (state == "1")
            {
                string savePath = Server.MapPath(@"~\Help\PDF-Helps\guest.pdf");
                MemoryStream st = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                return File(st.ToArray(), MimeType.Get(".pdf"), "guest.pdf");
            }
            if(state == "2")
            {
                string savePath = Server.MapPath(@"~\Help\PDF-Helps\HelpKhodEzhari.pdf");
                MemoryStream st = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                return File(st.ToArray(), MimeType.Get(".pdf"), "KhodEzhari.pdf");
            }
            return null;
        }

        public FileContentResult MunImage(int id)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_PictureMunicipalityImageSelect("fldPictureMunicipalityID", id.ToString(), 30, 1, "").FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldPic != null)
                {
                    return File((byte[])pic.fldPic, "jpg");
                }
            }
            return null;
        }
        public ActionResult Login(int id)
        {
            cartaxEntities car = new cartaxEntities();
            string State = "";
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")
            {
                if (id == 2)//کاربرمهمان
                    return View("logont_guest");
                else
                    return View("logont_setad");
            }
            if (ImageSetting == "2")
            {
                if (id == 2)//کاربرمهمان
                    return View("logonQ_guest");
                else
                    return View("logonQ_setad");
            }
            if (ImageSetting == "3")
            {
                if (id == 2)//کاربرمهمان
                    return View("logonz_guest");
                else
                    return View("logonz_setad");
            }
            if (ImageSetting == "6")
            {
                switch (id)
                {
                    case 1:
                        State = "استان سمنان";
                        break;
                    case 2:
                        State = "استان کرمان";
                        break;
                    case 3:
                        State = "استان تهران";
                        break;
                    case 4:
                        State = "استان خراسان رضوی";
                        break;
                    case 5:
                        State = "استان مازندران";
                        break;
                    case 6:
                        State = "استان فارس";
                        break;
                    case 7:
                        State = "استان خوزستان";
                        break;
                    case 8:
                        State = "استان اصفهان";
                        break;
                    case 9:
                        State = "استان آذربایجان غربی";
                        break;
                    case 10:
                        State = "استان آذربایجان شرقی";
                        break;
                    case 11:
                        State = "استان گیلان";
                        break;
                    case 12:
                        State = "استان لرستان";
                        break;
                    case 13:
                        State = "استان کرمانشاه";
                        break;
                    case 14:
                        State = "استان گلستان";
                        break;
                    case 15:
                        State = "استان کردستان";
                        break;
                    case 16:
                        State = "استان مرکزی";
                        break;
                    case 17:
                        State = "استان همدان";
                        break;
                    case 18:
                        State = "استان یزد";
                        break;
                    case 19:
                        State = "استان قزوین";
                        break;
                    case 20:
                        State = "استان کهگیلویه وبویراحمد";
                        break;
                    case 21:
                        State = "استان چهارمحال وبختیاری";
                        break;
                    case 22:
                        State = "استان اردبیل";
                        break;
                    case 23:
                        State = "استان زنجان";
                        break;
                    case 24:
                        State = "استان قم";
                        break;
                    case 25:
                        State = "استان سیستان وبلوچستان";
                        break;
                    case 26:
                        State = "استان بوشهر";
                        break;
                    case 27:
                        State = "استان هرمزگان";
                        break;
                    case 28:
                        State = "استان ایلام";
                        break;
                    case 29:
                        State = "استان خراسان شمالی";
                        break;
                    case 30:
                        State = "استان خراسان جنوبی";
                        break;
                    case 31:
                        State = "استان البرز";
                        break;
                }
            }
            else
            {
                switch (id)
                {
                    case 1:
                        State = "استان سمنان";
                        break;
                    case 2:
                        State = "استان کرمان";
                        break;
                    case 3:
                        State = "استان تهران";
                        break;
                    case 4:
                        State = "استان خراسان رضوی";
                        break;
                    case 5:
                        State = "استان مازندران";
                        break;
                    case 6:
                        State = "استان فارس";
                        break;
                    case 7:
                        State = "استان خوزستان";
                        break;
                    case 8:
                        State = "استان اصفهان";
                        break;
                    case 9:
                        State = "استان آذربایجان غربی";
                        break;
                    case 10:
                        State = "استان آذربایجان شرقی";
                        break;
                    case 11:
                        State = "استان گیلان";
                        break;
                    case 12:
                        State = "استان لرستان";
                        break;
                    case 13:
                        State = "استان کرمانشاه";
                        break;
                    case 14:
                        State = "استان گلستان";
                        break;
                    case 15:
                        State = "استان کردستان";
                        break;
                    case 16:
                        State = "استان مرکزی";
                        break;
                    case 17:
                        State = "استان همدان";
                        break;
                    case 18:
                        State = "استان یزد";
                        break;
                    case 19:
                        State = "استان قزوین";
                        break;
                    case 20:
                        State = "استان کهگیلویه وبویراحمد";
                        break;
                    case 21:
                        State = "استان چهارمحال وبختیاری";
                        break;
                    case 22:
                        State = "استان اردبیل";
                        break;
                    case 23:
                        State = "استان زنجان";
                        break;
                    case 24:
                        State = "استان قم";
                        break;
                    case 25:
                        State = "استان سیستان وبلوچستان";
                        break;
                    case 26:
                        State = "استان بوشهر";
                        break;
                    case 27:
                        State = "استان هرمزگان";
                        break;
                    case 28:
                        State = "استان ایلام";
                        break;
                    case 29:
                        State = "استان خراسان شمالی";
                        break;
                    case 30:
                        State = "استان خراسان جنوبی";
                        break;
                    case 31:
                        State = "استان البرز";
                        break;
                }
            }
            var q = car.sp_StateSelect("fldName", State, 1, 1, "").FirstOrDefault();
            ViewBag.StateId = q.fldID.ToString();
            return View("Logon");
        }
        public ActionResult LogOnDafater(string UserName, string Password, string Captcha)
        {
            if (Captcha.ToLower() != Session["captchaLoginD"].ToString().ToLower() && Convert.ToInt32(Session["captchahaveD"]) > 1)
            {
                return Json(new { Msg = "لطفا کد امنیتی را صحیح وارد نمایید.", MsgTitle = "خطا", state = "0", HaveCaptcha2 = Session["captchahaveD"] });
            }
            if (Membership.ValidateUser(UserName, Password))
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.sp_UserSelect("cheakPass", UserName, 1, Password.GetHashCode().ToString(), 1, "").FirstOrDefault();
                if (q.fldOfficeUserKey != null)
                {
                    return Json(new { Msg = "کاربر گرامی؛ شما ملزم به ورود از طریق درگاه پیشخوان می باشید.", MsgTitle = "خطا", state = "0", HaveCaptcha2 = Session["captchahaveD"] });
                }
                var UserLocation = p.sp_SelectTreeNodeID(Convert.ToInt32(q.fldID)).FirstOrDefault();
                if (q.fldStatus == false)
                {
                    return Json(new { Msg = "شما مجاز به ورود نمی باشید.", MsgTitle = "خطا", state = "0", HaveCaptcha2 = Session["captchahaveD"] });
                }
                Session["UserId"] = q.fldID;
                Session["UserPass"] = Password.GetHashCode().ToString();
                Session["UserName"] = UserName;
                var MunID = "";
                var expiredate = "";
                switch (q.CountryType)
                {
                    case 5:
                        Session["CountryType"] = 5;
                        Session["CountryCode"] = q.CountryCode;
                        MunID = q.CountryCode.ToString();
                        break;
                    case 6:
                        Session["CountryType"] = 6;
                        Session["CountryCode"] = q.CountryCode;
                        var Area = p.sp_AreaSelect("fldId", q.CountryCode.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        MunID = Area.fldMunicipalityID.ToString();
                        break;
                    case 7:
                        Session["CountryType"] = 7;
                        Session["CountryCode"] = q.CountryCode;
                        var Local = p.sp_LocalSelect("fldId", q.CountryCode.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        MunID = Local.fldMunicipalityID.ToString();
                        break;
                    case 8:
                        Session["CountryType"] = 8;
                        Session["CountryCode"] = q.CountryCode;
                        var Offices = p.sp_OfficesSelect("fldId", q.CountryCode.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        MunID = Offices.fldMunicipalityID.ToString();
                        expiredate = Offices.fldExpire;
                        break;
                }
                if (MunID == "")
                {
                    Session.RemoveAll();
                    return Json(new { Msg = "", MsgTitle = "", state = "1", HaveCaptcha2 = Session["captchahaveD"] });
                }
                   
                if (expiredate != "" && MyLib.Shamsi.Shamsi2miladiDateTime(expiredate) < DateTime.Now)
                {
                    Session.RemoveAll();
                    return Json(new
                    {
                        Msg = "شما بدلیل اتمام تاریخ انقضای دفتر مجاز به ورود نمی باشید. لطفا با پشتیبانی تماس حاصل فرمایید",
                        MsgTitle = "خطا",
                        state = "0"
                    }, JsonRequestBehavior.AllowGet);
                }
                var Municipality = p.sp_MunicipalitySelect("fldId", MunID, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var City = p.sp_CitySelect("fldId", Municipality.fldCityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var Zon = p.sp_ZoneSelect("fldId", City.fldZoneID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var County = p.sp_CountySelect("fldId", Zon.fldCountyID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                Session["UserMnu"] = Municipality.fldID;
                Session["UserState"] = County.fldStateID;
                FormsAuthentication.SetAuthCookie(UserName, false);
                var co = p.sp_SelectUpTreeCountryDivisions(UserLocation.fldID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                string area = "", offic = "";
                foreach (var item in co)
                {
                    switch (item.fldNodeType)
                    {
                        case 6:
                            area = area + " --> " + item.fldNodeName;
                            break;
                        case 7:
                            area = area + " --> " + item.fldNodeName;
                            break;
                        case 8:
                            offic = " --> " + item.fldNodeName;
                            break;
                    }
                }
                Session["area"] = area.Replace("--> ", "");
                Session["office"] = offic.Replace(" --> ", "");
                Session["Location"] = area + offic;
                OnlineUser.AddOnlineUser(q.fldID.ToString(), Request.ServerVariables["REMOTE_ADDR"].ToString(), Municipality.fldID.ToString());

                return Json(new { Msg = "", MsgTitle = "", state = "0", HaveCaptcha2 = Session["captchahaveD"] });

            }
            else
            {
                Session["captchahaveD"] = Convert.ToInt32(Session["captchahaveD"]) + 1;
                return Json(new { Msg = "نام کاربری یا کلمه عبور صحیح نیست.", MsgTitle = "خطا", state = "0", HaveCaptcha2 = Session["captchahaveD"] });
            }


        }
        public ActionResult LogOn(int? id)
        {
            if (Session["captchahave"] == null)
                Session["captchahave"] = 0;
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            ViewBag.Title = "ورود به سامانه";
            ViewBag.captchahave = Convert.ToInt32(Session["captchahave"]);
            ViewBag.ImageSetting = ImageSetting;
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            if (ImageSetting == "1")
            {
                if (id == 2)//کاربرمهمان
                    return View("logont_guest");
                else
                    return View("logont_setad");
            } if (ImageSetting == "2")
            {
                if (id == 2)//کاربرمهمان
                    return View("logonQ_guest");
                else
                    return View("logonQ_setad");
            }
            if (ImageSetting == "3")
            {
                if (id == 2)//کاربرمهمان
                    return null; //View("logonz_guest");
                else
                {
                    ViewBag.TypeUser = id;
                    return View("logonz_setad");
                }
            }            
            return View();
        }

        public ActionResult GetSetting()
        {
            ViewBag.captchahave = Convert.ToInt32(Session["captchahave"]);
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            return Json(ImageSetting, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNews(string StateId, string MnuId) 
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var News=new List<Models.sp_NewsSelect>();
             
            if(MnuId!="")
                News = car.sp_NewsSelect("fldMunicipalityID", MnuId, 0, 1, "").ToList();
            else
                News = car.sp_NewsSelect("fldStateId", StateId, 0, 1, "").ToList();

            var KhabarText = "";
            var KhabarID = "";
            foreach (var item in News)
            {
                KhabarText = KhabarText + item.fldSubject + ";";
                KhabarID = KhabarID + item.fldID + ";";
            }
            return Json(new {KhabarText = KhabarText, KhabarID = KhabarID}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeState()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "6")
            {
                var q = car.sp_StateSelect("", "", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName });
                return Json(q, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var q = car.sp_StateSelect("", "", 0, 1, "")/*.Where(l => l.fldID != 11)*/.OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName });
                return Json(q, JsonRequestBehavior.AllowGet);
            }            
        }

        public JsonResult GetCascadeCounty(int ID)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(1, ID, 5).ToList().OrderBy(h => h.NodeName);
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeCounty2(int ID)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            List<string> CountyName = new List<string>();
            List<int?> CountyId = new List<int?>();

            var County = car.sp_RegionTree(1, ID, 5).OrderBy(h => h.NodeName).ToList();
            for (int i = 0; i < County.Count(); i++)
            {
                CountyName.Add(County[i].NodeName);
                CountyId.Add(County[i].SourceID);
            }

            return Json(new
            {
                CountyName = CountyName,
                CountyId = CountyId
            }, JsonRequestBehavior.AllowGet);
        }
        public bool CheckMobileNumber(string MobileNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(MobileNumber, "(^(09|9)[0-9][0-9]\\d{7}$)");
        }
        public int checkCodeMeli(string codec)
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
        bool invalid = false;
        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();
            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }        
        public bool checkEmail(string Email)
        {

            if (String.IsNullOrEmpty(Email))
                invalid = false;
            else
            {
                Email = Regex.Replace(Email, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);

                invalid = Regex.IsMatch(Email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);
            }
            return invalid;
        }
        [HttpPost]
        public ActionResult SaveGeust(Models.sp_tblGuestInfoSelect Geust1)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            cartaxEntities m = new cartaxEntities();
            
            try
            {
                if (Geust1.fldDesc == null)
                    Geust1.fldDesc = "";
                if (checkCodeMeli(Geust1.fldCodeMeli) == 1)
                {
                    if (CheckMobileNumber(Geust1.fldMobile))
                    {
                        if (Geust1.fldDesc == "" || (Geust1.fldDesc != "" && checkEmail(Geust1.fldDesc)))
                        {
                            var q = m.sp_tblGuestInfoSelect("fldCodeMeli", Geust1.fldCodeMeli, "", 1).FirstOrDefault();
                            if (q == null)
                            {
                                var u = m.sp_UserSelect("fldId", "1", 0, "", 1, "").FirstOrDefault();
                               
                                try
                                {
                                    var Division = m.sp_CountryDivisionsSelect("fldMunicipalityID", Geust1.fldMunId.ToString(), 1, 1, "").FirstOrDefault();
                                    var transaction=m.sp_TransactionInfSelect("fldDivId", Division.fldID.ToString(), 0).OrderBy(l=>l.fldId).FirstOrDefault();
                                    WebTransaction.TransactionWebService trn = new WebTransaction.TransactionWebService();
                                    if (trn.RegisterGuest(u.fldDesc, transaction.fldUserName, transaction.fldPass, Geust1.fldName
                                        , Geust1.fldFamily, Geust1.fldCodeMeli, Geust1.fldMobile, Division.fldMunicipalityName))
                                    {
                                        m.sp_tblGuestInfoInsert(Geust1.fldName, Geust1.fldFamily, Geust1.fldCodeMeli, Geust1.fldMobile, Geust1.fldMunId, Geust1.fldUserName,
                                            Geust1.fldPasssword.GetHashCode().ToString(), Geust1.fldDesc);
                                        m.sp_TransactionInfInsert(Geust1.fldCodeMeli, 5, Geust1.fldMunId, CodeDecode.stringcode(Geust1.fldCodeMeli), 1, "", false);
                                        return View("logonz_guest");
                                        //Msg = "ثبت نام شما با موفقیت انجام شد.";
                                        //MsgTitle = "عملیات موفق";
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "خطای 969 رخ داده است. لطفا جهت رفع خطا با پشتیبانی سامانه تماس حاصل فرمایید.");
                                        return View("RegisterGuest", Geust1);
                                    }
                                }
                                catch (Exception x)
                                {
                                    throw;
                                }                                
                            }
                            else
                            {
                                ModelState.AddModelError("", "کد ملی وارد شده قبلا در سیستم ثبت شده است.");
                                return View("RegisterGuest", Geust1);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "ایمیل وارد شده نامعتبر است.");
                            return View("RegisterGuest",Geust1);
                        }
                    }
                    else
                    {
                        //return Json(new
                        //{
                        //    Er = 1,
                        //    Msg = "شماره موبایل وارد شده صحیح نیست.",
                        //    MsgTitle = "خطا"
                        //}, JsonRequestBehavior.AllowGet);
                        ModelState.AddModelError("", "شماره موبایل وارد شده صحیح نیست.");
                        return View("RegisterGuest", Geust1);
                    }
                }
                else
                {
                    return Json(new
                    {
                        Er = 1,
                        Msg = "کد ملی وارد شده نامعتبر است.",
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                m.sp_ErrorProgramInsert(Eid, InnerException, 1, x.Message, DateTime.Now, "");
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Vorod(Models.LogOnModel model, string returnUrl, string Capthalogin)
        {

            string Msg1 = ""; var Er = 0; var flag = false; string MsgTitle = ""; string Msg = "";
            HttpCookie MunCookies = new HttpCookie("MunCookies");
            HttpCookie StateCookies = new HttpCookie("StateCookies");
            MunCookies.Value = model.cboMnu;
            StateCookies.Value = model.cboState;


            if (Convert.ToInt32(Session["captchahave"]) > 1 && model.UserType == 1)
            {
                if (Capthalogin == "")
                {
                    Session["captchaLogin"] = "Error";
                    MsgTitle = "خطا";
                    Msg1 = "لطفا کد امنیتی را وارد نمایید.";
                    Er = 1;
                    Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                    return Json(new
                    {
                        Msg = Msg1,
                        MsgTitle = MsgTitle,
                        flag = flag,
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
                        Er = 1;
                        Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                        return Json(new
                        {
                            Msg = Msg1,
                            MsgTitle = MsgTitle,
                            flag = flag,
                            captchahave = Session["captchahave"]
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            if (model.UserType == 1)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    Models.cartaxEntities p = new Models.cartaxEntities();
                    var q = p.sp_UserSelect("cheakPass", model.UserName, 1, model.Password.GetHashCode().ToString(), 1, "").FirstOrDefault();
                    if (q.fldOfficeUserKey != null)
                    {
                        MsgTitle = "ورود ناموفق";
                        Msg1 = "کاربر گرامی؛ شما ملزم به ورود از طریق درگاه پیشخوان می باشید.";
                        Er = 1;
                        return Json(new
                        {
                            Msg = Msg1,
                            MsgTitle = MsgTitle,
                            flag = flag
                        }, JsonRequestBehavior.AllowGet);

                    }
                    var UserLocation = p.sp_SelectTreeNodeID(Convert.ToInt32(q.fldID)).FirstOrDefault();
                    var mnu = p.sp_TableTreeSelect("fldSourceID", model.cboMnu, 0, 0, 0).Where(h => h.fldNodeType == 5).FirstOrDefault();
                    var user_Down = p.sp_SelectDownTreeCountryDivisions(mnu.fldID, 1, "").Where(h => h.fldSourceID == UserLocation.fldSourceID && h.fldNodeType != 9);
                    var user_Up = p.sp_SelectUpTreeCountryDivisions(mnu.fldID, 1, "").Where(h => h.fldSourceID == UserLocation.fldSourceID && h.fldNodeType != 9);
                    bool isUserInCurrentTree = false;
                    if (user_Down.Any())
                        isUserInCurrentTree = true;
                    else if (user_Up.Any())
                        isUserInCurrentTree = true;
                    //string userid = q.fldUserID.ToStrisng();
                    //var isonline = OnlineUser.userObj.Where(k => k.userId == userid).Any();
                    //if (isonline == true)
                    //{
                    //    ModelState.AddModelError("", "شخص دیگری با کاربری شما وارد سیستم شده است و شما مجاز به ورود نمی باشید");
                    //    return View(model);
                    //}
                    if (isUserInCurrentTree && q.fldStatus == true)
                    {
                        Session["UserId"] = q.fldID;
                        Session["UserPass"] = model.Password.GetHashCode().ToString();
                        Session["UserMnu"] = model.cboMnu;//آیدی شهرداری که کاربر با آن لاگین کرده
                        Session["UserState"] = model.cboState;//آیدی استانی که کاربر با آن لاگین کرده
                        string user = model.UserName;
                        Session["UserName"] = user;

                        var co = p.sp_SelectUpTreeCountryDivisions(UserLocation.fldID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                        string area = "", offic = "";
                        Session["CountryType"] = 5;
                        Session["CountryCode"] = model.cboMnu;
                        var officeexpire = "";
                        foreach (var item in co)
                        {
                            switch (item.fldNodeType)
                            {
                                case 6:
                                    area = area + " --> " + item.fldNodeName;
                                    Session["CountryType"] = 6;
                                    Session["CountryCode"] = item.fldSourceID;
                                    break;
                                case 7:
                                    area = area + " --> " + item.fldNodeName;
                                    Session["CountryType"] = 7;
                                    Session["CountryCode"] = item.fldSourceID;
                                    break;
                                case 8:
                                    offic = " --> " + item.fldNodeName;
                                    Session["CountryType"] = 8;
                                    Session["CountryCode"] = item.fldSourceID;
                                    var off = p.sp_OfficesSelect("fldid", item.fldSourceID.ToString(), 0, 1, "").FirstOrDefault();
                                    officeexpire = off.fldExpire;
                                    break;
                            }
                        }
                        if (officeexpire != "" && MyLib.Shamsi.Shamsi2miladiDateTime(officeexpire) < DateTime.Now)
                        {
                            Session.RemoveAll();
                            return Json(new
                            {
                                Msg = "شما بدلیل اتمام تاریخ انقضای دفتر مجاز به ورود نمی باشید. لطفا با پشتیبانی تماس حاصل فرمایید",
                                MsgTitle = "خطا",
                                flag = flag
                            }, JsonRequestBehavior.AllowGet);
                        }
                        Session["area"] = area.Replace("--> ", "");
                        Session["office"] = offic.Replace(" --> ", "");                        
                        Session["Location"] = area + offic;
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        OnlineUser.AddOnlineUser(q.fldID.ToString(), Request.ServerVariables["REMOTE_ADDR"].ToString(), model.cboMnu);
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "First", new { area = "NewVer" });
                        }
                    }
                    else
                    {
                        Msg = "شما مجاز به ورود در موقعیت انتخاب شده نمی باشید.";
                        MsgTitle = "ورود ناموفق";

                        //ModelState.AddModelError("", "شما مجاز به ورود در موقعیت انتخاب شده نمی باشید.");
                        //return View(model);
                    }
                }
                else
                {
                    Msg = "نام کاربری یا کلمه عبور صحیح نیست.";
                    MsgTitle = "ورود ناموفق";
                    Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                    // return View(model);
                }
            }
            else
            {
                Session["UserMnu"] = model.cboMnu;//آیدی شهرداری که کاربر با آن لاگین کرده
                Session["UserState"] = model.cboState;//آیدی استانی که کاربر با آن لاگین کرده
                Session["GeustId"] = 20;//آیدی کاربر مهمان
                Session["UserPass"] = "";
                Session["CountryType"] = 5;
                Session["CountryCode"] = model.cboMnu;
                //return RedirectToAction("Guest", "Home");
                return RedirectToAction("Index", "First", new { area = "NewVer" });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                flag = flag,
                captchahave = Session["captchahave"]
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult LogOn(string btn,string Mun, Models.LogOnModel model, string returnUrl, string Capthalogin)
        {
            string Msg1 = ""; var Er = 0; var flag = false; string MsgTitle = ""; string Msg = "";
            if (btn == "log" || btn == null)
            {
                HttpCookie MunCookies = new HttpCookie("MunCookies");
                HttpCookie StateCookies = new HttpCookie("StateCookies");
                MunCookies.Value = Mun;
                StateCookies.Value = "11";


                if (Convert.ToInt32(Session["captchahave"]) > 1)
                {
                    if (Capthalogin == "")
                    {
                        Session["captchaLogin"] = "Error";
                        /*MsgTitle = "خطا";
                        Msg1 = "لطفا کد امنیتی را وارد نمایید.";
                        Er = 1;*/

                        Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;

                        ModelState.AddModelError("", "لطفا کد امنیتی را وارد نمایید.");
                        var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                        if (ImageSetting == "1")
                            return View("logont_setad", model);
                        if (ImageSetting == "2")
                            return View("logonQ_setad", model);
                        if (ImageSetting == "3")
                        {
                            ViewBag.TypeUser = model.UserType;
                            return View("logonz_setad", model);
                        }
                        return View(model);

                        /*return Json(new
                        {
                            Msg = Msg1,
                            MsgTitle = MsgTitle,
                            flag = flag,
                            captchahave = Session["captchahave"]
                        }, JsonRequestBehavior.AllowGet);*/
                    }
                    else
                    {

                        if (Capthalogin.ToLower() != Session["captchaLogin"].ToString().ToLower())
                        {
                            Session["captchaLogin"] = "Error";

                            ModelState.AddModelError("", "لطفا کد امنیتی را صحیح وارد نمایید.");
                            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                            if (ImageSetting == "1")
                                return View("logont_setad", model);
                            if (ImageSetting == "2")
                                return View("logonQ_setad", model);
                            if (ImageSetting == "3")
                            {
                                ViewBag.TypeUser = model.UserType;
                                return View("logonz_setad", model);
                            }
                            return View(model);

                            /*MsgTitle = "خطا";
                            Msg1 = "لطفا کد امنیتی را صحیح وارد نمایید.";
                            Er = 1;*/
                            Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                            /*return Json(new
                            {
                                Msg = Msg1,
                                MsgTitle = MsgTitle,
                                flag = flag,
                                captchahave = Session["captchahave"]
                            }, JsonRequestBehavior.AllowGet);*/
                        }
                    }
                }


                if (model.UserType == 1)
                {
                    if (Membership.ValidateUser(model.UserName, model.Password))
                    {
                        Models.cartaxEntities p = new Models.cartaxEntities();
                        var q = p.sp_UserSelect("cheakPass", model.UserName, 1, model.Password.GetHashCode().ToString(), 1, "").FirstOrDefault();

                        var UserLocation = p.sp_SelectTreeNodeID(Convert.ToInt32(q.fldID)).FirstOrDefault();
                        var mnu = p.sp_TableTreeSelect("fldSourceID", Mun, 0, 0, 0).Where(h => h.fldNodeType == 5).FirstOrDefault();
                        var user_Down = p.sp_SelectDownTreeCountryDivisions(mnu.fldID, 1, "").Where(h => h.fldSourceID == UserLocation.fldSourceID && h.fldNodeType != 9);
                        var user_Up = p.sp_SelectUpTreeCountryDivisions(mnu.fldID, 1, "").Where(h => h.fldSourceID == UserLocation.fldSourceID && h.fldNodeType != 9);
                        bool isUserInCurrentTree = false;
                        if (user_Down.Any())
                            isUserInCurrentTree = true;
                        else if (user_Up.Any())
                            isUserInCurrentTree = true;

                        if (isUserInCurrentTree && q.fldStatus == true)
                        {
                            Session["UserId"] = q.fldID;
                            Session["UserPass"] = model.Password.GetHashCode().ToString();
                            Session["UserMnu"] = Mun;//آیدی شهرداری که کاربر با آن لاگین کرده
                            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                            if (ImageSetting == "3")
                                Session["UserState"] = 17;//آیدی استانی که کاربر با آن لاگین کرده
                            else if (ImageSetting == "1")
                                Session["UserState"] = 11;//آیدی استانی که کاربر با آن لاگین کرده
                            else if (ImageSetting == "2")
                                Session["UserState"] = 3;//آیدی استانی که کاربر با آن لاگین کرده
                            string user = model.UserName;
                            Session["UserName"] = user;

                            var co = p.sp_SelectUpTreeCountryDivisions(UserLocation.fldID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                            string area = "", offic = "";
                            Session["CountryType"] = 5;
                            Session["CountryCode"] = Mun;
                            var officeexpire = "";
                            foreach (var item in co)
                            {
                                switch (item.fldNodeType)
                                {
                                    case 6:
                                        area = area + " --> " + item.fldNodeName;
                                        Session["CountryType"] = 6;
                                        Session["CountryCode"] = item.fldSourceID;
                                        break;
                                    case 7:
                                        area = area + " --> " + item.fldNodeName;
                                        Session["CountryType"] = 7;
                                        Session["CountryCode"] = item.fldSourceID;
                                        break;
                                    case 8:
                                        offic = " --> " + item.fldNodeName;
                                        Session["CountryType"] = 8;
                                        Session["CountryCode"] = item.fldSourceID;
                                        var off = p.sp_OfficesSelect("fldid", item.fldSourceID.ToString(), 0, 1, "").FirstOrDefault();
                                        officeexpire = off.fldExpire;
                                        break;
                                }
                            }
                            if (officeexpire != "" && MyLib.Shamsi.Shamsi2miladiDateTime(officeexpire) < DateTime.Now)
                            {

                                ModelState.AddModelError("", "شما بدلیل اتمام تاریخ انقضای دفتر مجاز به ورود نمی باشید. لطفا با پشتیبانی تماس حاصل فرمایید");
                                Session.RemoveAll();
                                var ImageSetting1 = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                                if (ImageSetting1 == "3")
                                {
                                    ViewBag.TypeUser = model.UserType;
                                    return View("logonz_setad", model);
                                }
                                return View(model);
                            }
                            Session["area"] = area.Replace("--> ", "");
                            Session["office"] = offic.Replace(" --> ", "");
                            Session["Location"] = area + offic;
                            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                            OnlineUser.AddOnlineUser(q.fldID.ToString(), Request.ServerVariables["REMOTE_ADDR"].ToString(), Mun);
                            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                //return Json(new { redirectToUrl = Url.Action("Index", "First") });
                                return RedirectToAction("Index", "First", new { area = "NewVer" });
                            }
                        }
                        else
                        {
                            //Msg = "شما مجاز به ورود در موقعیت انتخاب شده نمی باشید.";
                            //MsgTitle = "ورود ناموفق";

                            ModelState.AddModelError("", "شما مجاز به ورود در موقعیت انتخاب شده نمی باشید.");
                            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                            if (ImageSetting == "1")
                                return View("logont_setad", model);
                            if (ImageSetting == "2")
                                return View("logonQ_setad", model);
                            if (ImageSetting == "3")
                            {
                                ViewBag.TypeUser = model.UserType;
                                return View("logonz_setad", model);
                            }
                            return View(model);
                        }
                    }
                    else
                    {
                        /*Msg = "نام کاربری یا کلمه عبور صحیح نیست.";
                        MsgTitle = "ورود ناموفق";*/
                        Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                        ModelState.AddModelError("", "نام کاربری یا کلمه عبور صحیح نیست.");

                        var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                        if (ImageSetting == "1")
                            return View("logont_setad", model);
                        if (ImageSetting == "2")
                            return View("logonQ_setad", model);
                        if (ImageSetting == "3")
                        {
                            ViewBag.TypeUser = model.UserType;
                            return View("logonz_setad", model);
                        }

                        return View(model);
                        // return View(model);
                    }
                }
                else
                {
                    Models.cartaxEntities p = new Models.cartaxEntities();
                    if (Mun != "" && Mun != null)
                    {
                        var Exist = p.sp_tblGuestInfoSelect("fldUserName", model.UserName, "", 1).FirstOrDefault();
                        if (Exist != null)
                        {
                            var q = p.sp_tblGuestInfoSelect("cheakPass", model.UserName, model.Password.GetHashCode().ToString(), 1).FirstOrDefault();
                            if (q != null)
                            {
                                if (q.fldMunId.ToString() == Mun)
                                {
                                    Session["GeustId"] = 20;
                                    Session["GuestInfId"] = q.fldId;
                                    Session["CountryType"] = 5;
                                    Session["CountryCode"] = q.fldMunId;
                                    Session["UserPass"] = model.Password.GetHashCode().ToString();
                                    Session["UserMnu"] = Mun;//آیدی شهرداری که کاربر با آن لاگین کرده
                                    var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                                    if (ImageSetting == "3")
                                        Session["UserState"] = 17;//آیدی استانی که کاربر با آن لاگین کرده
                                    else if (ImageSetting == "1")
                                        Session["UserState"] = 11;//آیدی استانی که کاربر با آن لاگین کرده
                                    else if (ImageSetting == "2")
                                        Session["UserState"] = 3;//آیدی استانی که کاربر با آن لاگین کرده

                                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                                    {
                                        return Redirect(returnUrl);
                                    }
                                    else
                                    {
                                        return RedirectToAction("Index", "First", new { area = "NewVer" });
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "شما مجاز به ورود در موقعیت انتخاب شده نمی باشید.");
                                    var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                                    if (ImageSetting == "1")
                                        return View("logont_setad", model);
                                    if (ImageSetting == "2")
                                        return View("logonQ_setad", model);
                                    if (ImageSetting == "3")
                                    {
                                        ViewBag.TypeUser = model.UserType;
                                        return View("logonz_setad", model);
                                    }
                                    return View(model);
                                }
                            }
                            else
                            {
                                Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                                ModelState.AddModelError("", "نام کاربری یا کلمه عبور صحیح نیست.");

                                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                                if (ImageSetting == "1")
                                    return View("logont_setad", model);
                                if (ImageSetting == "2")
                                    return View("logonQ_setad", model);
                                if (ImageSetting == "3")
                                {
                                    ViewBag.TypeUser = model.UserType;
                                    return View("logonz_setad", model);
                                }

                                return View(model);
                            }
                        }
                        else
                        {
                            Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                            ModelState.AddModelError("", "کاربری با مشخصات وارد شده در سامانه یافت نشد.");

                            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                            if (ImageSetting == "1")
                                return View("logont_setad", model);
                            if (ImageSetting == "2")
                                return View("logonQ_setad", model);
                            if (ImageSetting == "3")
                            {
                                ViewBag.TypeUser = model.UserType;
                                return View("logonz_setad", model);
                            }

                            return View(model);
                        }
                    }
                    else
                    {
                        Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                        ModelState.AddModelError("", "انتخاب شهرداری ضروری است.");

                        var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                        if (ImageSetting == "1")
                            return View("logont_setad", model);
                        if (ImageSetting == "2")
                            return View("logonQ_setad", model);
                        if (ImageSetting == "3")
                        {
                            ViewBag.TypeUser = model.UserType;
                            return View("logonz_setad", model);
                        }

                        return View(model);
                    }
                }
                
            }           
            else
            {
                //return Json(new { redirectToUrl = Url.Action("Index", "First") });
                return RedirectToAction("Index", "Register_New", new { area = "NewVer", @ImageSetting = "1" });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                flag = flag,
                captchahave = Session["captchahave"]
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RegisterGuest(){        
            return View();
        }
        public ActionResult LogOff()
        {
            if (Session["UserId"] != null)
            {
                Models.OnlineUser.RemoveOnlineUser(Session["UserId"].ToString());
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                
            }
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "First");
        }

        public void captchahave(string count)
        {
            Session["captchahave"] = count;
        }
        public ActionResult Akhbar(string id)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.id = id;
            return PartialView;
        }
        public ActionResult MatnKabar(string idKhabar)
        {
            Avarez.Models.cartaxEntities m = new cartaxEntities();
            var q = m.sp_NewsSelect("fldId", idKhabar, 1, null, null).FirstOrDefault();
            string fldMemo = "";
            if (q != null)
                fldMemo = q.fldMemo;
            return Json(new
            {
                fldMemo = fldMemo
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
