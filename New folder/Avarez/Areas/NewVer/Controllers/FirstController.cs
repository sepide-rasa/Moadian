using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using System.Collections;
using System.Web.Security;
using System.Configuration;
using System.Drawing;
using System.Net;

namespace Avarez.Areas.NewVer.Controllers
{
    public class FirstController : Controller
    {
        //
        // GET: /NewVer/First/
        public ActionResult DirectLogin(int Mun)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var m = p.sp_MunicipalitySelect("fldid", Mun.ToString(), 0, 1, "").FirstOrDefault();
            if (m != null)
            {
                var city = p.sp_CitySelect("fldid", m.fldCityID.ToString(), 0, 1, "").FirstOrDefault();
                if (city != null)
                {
                    var zone = p.sp_ZoneSelect("fldid", city.fldZoneID.ToString(), 0, 1, "").FirstOrDefault();
                    if (zone != null)
                    {
                        var conty = p.sp_CountySelect("fldid", zone.fldCountyID.ToString(), 0, 1, "").FirstOrDefault();
                        if (conty != null)
                        {

                            Session["UserMnu"] = Mun;//آیدی شهرداری که کاربر با آن لاگین کرده
                            Session["UserState"] = conty.fldStateID;//آیدی استانی که کاربر با آن لاگین کرده
                            Session["GeustId"] = 20;//آیدی کاربر مهمان
                            Session["UserPass"] = "";
                            Session["CountryType"] = 5;
                            Session["CountryCode"] = Mun;
                        }
                    }
                }
            }
            
            return RedirectToAction("Index", "First", new { area = "NewVer" });
        }
        public ActionResult Index()
        {
                     
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (Session["UserId"] == null && Session["GeustId"] == null && Session["UserGeust"] == null)
            {
                //   return RedirectToAction("logon", "Account");
                if (ImageSetting == "1")//تهران
                {
                    //return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                }
                else
                {
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                }
            }
            DateTime d1, d2;
            d1 = DateTime.Now;
            d2 = Convert.ToDateTime("2021/08/28");
            //string mm="406";//خمینی شهر
            string mm = "";// "41";// "55";//اردکان
            //string mm = "114";//اندیشه

            if (Session["UserMnu"].ToString() == mm && DateTime.Compare(d1, d2) > 0)
                return null;
            if (Session["UserId"] != null)
            {
                if (Session["UserId"].ToString() == "")
                    Session["UserId"] = null;
            }
            var UserType="1";
            if ((Session["UserId"] == null) && Session["GeustId"] != null)
                UserType = "2";
            if (Session["UserId"] == null && Session["GeustId"] == null && Session["UserGeust"] != null)
                UserType = "3";
            //if (UserType == "2")
            //    return PartialView("Guest");
            Models.cartaxEntities m = new Models.cartaxEntities();
            /*List<Models.sp_GetNotVerifiedCarExperience> data = null;
            data = m.sp_GetNotVerifiedCarExperience(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).ToList();*/

            var q = m.sp_GetDate().FirstOrDefault();
            var time = q.CurrentDateTime;
            ViewBag.time = time.Hour.ToString().PadLeft(2, '0') + ":" +
                time.Minute.ToString().PadLeft(2, '0') + ":" +
                time.Second.ToString().PadLeft(2, '0');
            ViewBag.UserType = UserType;
            if (UserType == "1")
            {
                var tempid = m.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
                var Etelaie = m.sp_AnnouncementManagerSelect("fldCountryDivisonTempID", tempid.fldID.ToString()
                    , 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                var EtelaieText = "";
                var EtelaieID = "";
                foreach (var item in Etelaie)
                {
                    EtelaieText = EtelaieText + item.fldSubject + ";";
                    EtelaieID = EtelaieID + item.fldID + ";";
                }
                ViewBag.EtelaieText = EtelaieText;
                ViewBag.EtelaieID = EtelaieID;
                m.sp_LogInProgramInsert(Convert.ToInt32(Session["UserId"]), "", GetIp(), Session["UserPass"].ToString());
            }
            ViewBag.ImageSetting = ImageSetting;
            /*ViewBag.NotVerifyCarExp = data.Count;*/
            return View();
        }
        public string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }
        public FileContentResult DownloadChrome()
        {
            string savePath = Server.MapPath(@"~\Uploaded\Google.Chrome.zip");
            MemoryStream st = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
            return File(st.ToArray(), MimeType.Get(".zip"), "Google.Chrome.zip");
        }
        public ActionResult GetDate()
        {
            return Json(DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Second.ToString().PadLeft(2, '0'), JsonRequestBehavior.AllowGet);
        }
        public ActionResult changeMogheiat()
        {
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")//تهران
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
            }
            else
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
            }
            Models.cartaxEntities p = new Models.cartaxEntities();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            if (Session["UserId"] != null && Session["GeustId"] == null) {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "تغییر موقعیت");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
            }
            //var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            //string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            //var q = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).FirstOrDefault();
            //PartialView.ViewBag.RootNameTree = q.fldNodeName;
            //PartialView.ViewBag.NodeId = q.fldID;
            return PartialView;
        }
        public ActionResult Logout()
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
        public ActionResult MohtavaAbout()
        {

            Models.cartaxEntities m = new Models.cartaxEntities();
            var PageHtml = m.sp_PageHtmlSelect("fldId", "1", 0, "", 1).FirstOrDefault();
            var MohtavaAbout = "";
            ////ussd.UssdService h = new ussd.UssdService();
            ////MohtavaAbout=h.GetMablagh("IRFC901V7D2201001");
            if (PageHtml != null)
            {
                MohtavaAbout = PageHtml.fldMatnHtml;
            }
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1" || ImageSetting == "2")
            {
                return Json(new { MohtavaAbout = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { MohtavaAbout = MohtavaAbout }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult About()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult ForgetPass(byte UserType)
        {
            ViewBag.UserType = UserType;
            return View();
        }
        public ActionResult HelpForgetPass()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult SendSMS(string UserName, string Mobile,byte UserType)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            string Msg = ""; string MsgTitle = ""; int Er = 0;
            if (UserType == 1)
            {
                var q = m.sp_UserSelect("fldUserName", UserName, 0, "", 1, "").Where(k => k.fldMobile == Mobile).FirstOrDefault();
                if (q != null)
                {
                    System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
                    CaptchaImage img = new CaptchaImage(90, 40, family);
                    string text = img.CreateRandomText(5);

                    m.sp_ForgetPass_VerifyInsert(text, false, q.fldID, "");

                    SmsSender sendsms = new SmsSender();
                    if (CheckMobileNumber(Mobile))
                    {
                        Msg = sendsms.SendVerifyMessage(q.fldCountryDivisionsID.ToString(), Mobile, text);
                        MsgTitle = "عملیات موفق";
                        //Msg = "پیامکی حاوی کد بازسازی رمزعبور برای شما ارسال می گردد.";
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "موبایل وارد شده نامعتبر است",
                            MsgTitle = "خطا",
                            Er = 1
                        }
                        , JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    Msg = "کاربری با این اطلاعات در سیستم ثبت نشده است.";
                    MsgTitle = "خطا";
                    Er = 1;
                }
            }
            else
            {
                var q = m.sp_tblGuestInfoSelect("fldUserName", UserName, "", 1).Where(k => k.fldMobile == Mobile).FirstOrDefault();
                if (q != null)
                {
                    System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
                    CaptchaImage img = new CaptchaImage(90, 40, family);
                    string text = img.CreateRandomText(5);

                    m.sp_ForgetPass_VerifyGuestInsert(text, false, q.fldId);

                    SmsSender sendsms = new SmsSender();
                    if (CheckMobileNumber(Mobile))
                    {
                        Msg = sendsms.SendVerifyMessage(q.fldCountryDivTempId.ToString(), Mobile, text);
                        MsgTitle = "عملیات موفق";
                        //Msg = "پیامکی حاوی کد بازسازی رمزعبور برای شما ارسال می گردد.";
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "موبایل وارد شده نامعتبر است",
                            MsgTitle = "خطا",
                            Er = 1
                        }
                        , JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    Msg = "کاربری با این اطلاعات در سیستم ثبت نشده است.";
                    MsgTitle = "خطا";
                    Er = 1;
                }
            }

            return Json(new { Msg = Msg, MsgTitle = MsgTitle, Er = Er }, JsonRequestBehavior.AllowGet);
        }
        public bool CheckMobileNumber(string MobileNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(MobileNumber, "(^(09|9)[0-9][0-9]\\d{7}$)");
        }
        public ActionResult ResetPass(string UserName, string Mobile, string VerifyCode, byte UserType)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            string Msg = ""; string MsgTitle = ""; int Er = 0;
            if (UserType == 1)
            {
                var q = m.sp_UserSelect("fldUserName", UserName, 0, "", 1, "").Where(k => k.fldMobile == Mobile).FirstOrDefault();
                if (q != null)
                {
                    var f = m.sp_ForgetPass_VerifySelect("fldUserId", q.fldID.ToString(), 0).Where(k => k.fldVerifyCode == VerifyCode && k.fldActive_Deactive == false).FirstOrDefault();
                    if (f != null)
                    {
                        m.sp_ForgetPass_VerifyUpdate(f.fldId);
                        m.sp_UserPassUpdate(Convert.ToInt32(q.fldID), q.fldUserName.GetHashCode().ToString());
                        Msg = "رمز عبور شما به نام کاربری تغییر پیدا کرد.";
                        MsgTitle = "عملیات موفق";
                    }
                    else
                    {
                        Msg = "کد فعالسازی اشتباه است.";
                        MsgTitle = "خطا";
                        Er = 1;
                    }

                }
                else
                {
                    Msg = "کاربری با این اطلاعات در سیستم ثبت نشده است.";
                    MsgTitle = "خطا";
                    Er = 1;
                }
            }
            else
            {
                var q = m.sp_tblGuestInfoSelect("fldUserName", UserName, "", 1).Where(k => k.fldMobile == Mobile).FirstOrDefault();
                if (q != null)
                {
                    var f = m.sp_ForgetPass_VerifyGuestSelect("fldGuestInfId", q.fldId.ToString(), 0).Where(k => k.fldVerifyCode == VerifyCode && k.fldActive_Deactive == false).FirstOrDefault();
                    if (f != null)
                    {
                        m.sp_ForgetPass_VerifyGuestUpdate(f.fldId);
                        m.sp_UserPassGuestUpdate(Convert.ToInt32(q.fldId), q.fldUserName.GetHashCode().ToString());
                        Msg = "رمز عبور شما به نام کاربری تغییر پیدا کرد.";
                        MsgTitle = "عملیات موفق";
                    }
                    else
                    {
                        Msg = "کد فعالسازی اشتباه است.";
                        MsgTitle = "خطا";
                        Er = 1;
                    }
                }
                else
                {
                    Msg = "کاربری با این اطلاعات در سیستم ثبت نشده است.";
                    MsgTitle = "خطا";
                    Er = 1;
                }
            }
            return Json(new { Msg = Msg, MsgTitle = MsgTitle, Er = Er }, JsonRequestBehavior.AllowGet);
        }
        /*public ActionResult NodeLoadTreeStructure(string nod)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (nod == "0" || nod==null)
            {
                var child = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldNodeName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.IconFile = url + ch.fldNodeType + ".png";
                    childNode.DataPath = ch.fldNodeType.ToString();
                    childNode.Cls = ch.fldSourceID.ToString();
                    nodes.Add(childNode);
                }
            }
            else
            {

                    var child = p.sp_TableTreeSelect("fldPId", nod, 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldNodeName;
                        childNode.NodeID = ch.fldID.ToString();
                        childNode.IconFile = url + ch.fldNodeType + ".png";
                        childNode.DataPath = ch.fldNodeType.ToString();
                        childNode.Cls = ch.fldSourceID.ToString();
                        nodes.Add(childNode);
                    }
               
            }
            return this.Direct(nodes);
        }*/

        public ActionResult NodeLoadTreeStructure(string nod)
        {
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (Session["UserId"] == null)
            {
                //   return RedirectToAction("logon", "Account");
                if (ImageSetting == "1")//تهران
                {
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                }
                else
                {
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                }
            }
            NodeCollection nodes = new Ext.Net.NodeCollection();
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (nod == "0" || nod == null)
            {
                var child = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldNodeName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.IconFile = url + ch.fldNodeType + ".png";
                    childNode.DataPath = ch.fldNodeType.ToString();
                    childNode.Cls = ch.fldSourceID.ToString();
                    nodes.Add(childNode);
                }
            }
            else
            {

                var child = p.sp_TableTreeSelect("fldPId", nod, 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();
                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldNodeName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.IconFile = url + ch.fldNodeType + ".png";
                    childNode.DataPath = ch.fldNodeType.ToString();
                    childNode.Cls = ch.fldSourceID.ToString();
                    nodes.Add(childNode);
                }

            }
            return this.Direct(nodes);
        }
        public ActionResult SelectLocation(int id)
        {
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (Session["UserId"] == null)
            {
                //   return RedirectToAction("logon", "Account");
                if (ImageSetting == "1")//تهران
                {
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                }
                else
                {
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                }
            }
            var Location = "";
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var Selected_Up_mnu = p.sp_SelectUpTreeCountryDivisions(id, 1, "");
                var Selected_Up_state = p.sp_SelectUpTreeCountryDivisions(id, 1, ""); 
                var mun = Selected_Up_mnu.Where(h => h.fldNodeType == 5).FirstOrDefault();
                var state = Selected_Up_state.Where(h => h.fldNodeType == 1).FirstOrDefault();
                Session["UserMnu"] = mun.fldSourceID;//آیدی شهرداری که کاربر با آن لاگین کرده
                Session["UserState"] = state.fldSourceID;//آیدی استانی که کاربر با آن لاگین کرده
                Session["CountryType"] = 5;
                Session["CountryCode"] = mun.fldSourceID;
                var co = p.sp_SelectUpTreeCountryDivisions(id, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                string area = "", offic = "";
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
                            break;
                    }
                }
                Session["area"] = area.Replace("--> ", "");
                Session["office"] = offic.Replace(" --> ", "");
                Session["Location"] = area + offic;

                var s = p.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var Mnu = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                Location = s.fldName + " --> " + Mnu.fldName + Session["Location"];
                return Json(new { Location = Location,Er=0 });

            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }
        public ActionResult CountryPosition(int id)
        {
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (Session["UserId"] == null)
            {
                //   return RedirectToAction("logon", "Account");
                if (ImageSetting == "1")//تهران
                {
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                }
                else
                {
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                }
            }
            Models.cartaxEntities car = new Models.cartaxEntities();
            var nodes = car.sp_SelectUpTreeCountryDivisions(id, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            ArrayList ar = new ArrayList();

            foreach (var item in nodes)
            {
                ar.Add(item.fldNodeName);
            }
            string nodeNames = "";
            for (int i = 0; i < ar.Count; i++)
            {
                if (i < ar.Count - 1)
                    nodeNames += ar[i].ToString() + "-->";
                else
                    nodeNames += ar[i].ToString();
            }

            return Json(new { Position = nodeNames });
        }
        public ActionResult HelpGuest()
        {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
        }

        public ActionResult HelpMehman()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public FileContentResult ShowPic()
        {//برگرداندن عکس 
            if (Session["Username"] != null)
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                var user = car.rpt_UserSelect("fldId", Session["UserId"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                //var user = p.sp_UserSelect("fldUserName", User.Identity.Name, 1, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (user != null)
                {
                    var pic = car.sp_PictureSelect("fldUserPic", user.fldID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    if (pic != null)
                    {
                        if (pic.fldPic != null)
                        {
                            return File((byte[])pic.fldPic, "jpg");
                        }
                    }
                }
                return null;
            }
            return null;

        }
        public ActionResult GotoNExtEtelaiie(string ID)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ID = ID;
            return PartialView;
        }
        public ActionResult DetailsInfoPage(int Id)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.sp_AnnouncementManagerSelect("fldId", Id.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q1 = p.sp_AnnouncementManagerAttachmentSelect("fldAnnouncementID", q.fldID.ToString(), 0,
               Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                int[] attach = null;
                if (q1.Count > 0)
                {
                    int i = 0;
                    attach = new int[q1.Count];
                    foreach (var item in q1)
                    {
                        attach[i] = item.fldID;
                        i++;
                    }
                }
                return Json(new
                {
                    fldId = q.fldID,
                    fldMemo = q.fldMemo,
                    fldDate=q.fldDate,
                    fldSubject = q.fldSubject,
                    attach = attach
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }
        public FileContentResult DownloadAnnouncementAttach(int Id)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var f = p.sp_AnnouncementManagerAttachmentSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (f != null)
            {
                MemoryStream st = new MemoryStream(f.fldAttachment);
                return File(st.ToArray(), MimeType.Get(System.IO.Path.GetExtension(f.fldFileName)), f.fldFileName);
            }
            return null;
        }
    }
}
