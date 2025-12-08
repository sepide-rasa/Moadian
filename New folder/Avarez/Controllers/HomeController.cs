using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avarez.Models;
using System.Collections;
using Avarez.Controllers.Users;
using System.Web.Security;
using System.Web.Configuration;
using System.Configuration;

namespace Avarez.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult PreviewRptPDFBox()
        {
            return PartialView();
        }
        [Authorize]
        public ActionResult PreviewFile()
        {
            return PartialView();
        }
        [Authorize]
        public ActionResult Chat()
        {
            if (OnlineUser.userObj.Where(item => item.sessionId == System.Web.HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value.ToString()).Count() > 0)
                OnlineUser.userObj.Remove(OnlineUser.userObj.Where(item => item.sessionId == System.Web.HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value.ToString()).FirstOrDefault());
            OnlineUser.AddOnlineUser("", Session["UserName"].ToString(), Session["UserId"].ToString(), System.Web.HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value.ToString());
            return PartialView();
        }

        [Authorize]
        public ActionResult AppReport()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            return View();
        }
        public ActionResult DirectLogin(int Mnu, int State)
        {
            Session["UserMnu"] = Mnu;//آیدی شهرداری که کاربر با آن لاگین کرده
            Session["UserState"] = State;//آیدی استانی که کاربر با آن لاگین کرده
            Session["GeustId"] = 20;//آیدی کاربر مهمان
            Session["UserPass"] = "";
            Session["CountryType"] = 5;
            Session["CountryCode"] = Mnu;
            return RedirectToAction("Index", "First", new { area = "NewVer" });
        }
        [Authorize]
        public ActionResult MgrReport()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات مدیریتی");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            return View();
        }

        [Authorize]
        public ActionResult ChangeLocation()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            OnlineUser.UpdateUrl(Session["UserId"].ToString(), "تغییر موقعیت");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            return View();
        }

        [Authorize]
        public ActionResult ChLocation()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            OnlineUser.UpdateUrl(Session["UserId"].ToString(), "تغییر موقعیت");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            return PartialView();
        }

        [Authorize]
        public ActionResult SelectLocation(int id)
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
            return Json(new { data = "" });
        }

        [Authorize]
        public JsonResult CountryPosition(int id)
        {
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

        [Authorize]
        public JsonResult _CountryTree(int? id)
        {
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            //پروسیجر تشخیص کاربر در تقسیمات کشوری برای
            //لود درخت همان ناحیه ای کاربر در سطح آن تعریف شده 
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (id != null)
            {
                var rols = (from k in p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var rols = (from k in p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Index()
        {
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")//تهران
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                else
                    return RedirectToAction("Index", "First", new { area = "NewVer" });
            }
            else
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                else
                    return RedirectToAction("Index", "First", new { area = "NewVer" });
            }
            /*if (Session["HaveCaptcha2"] == null)
                Session["HaveCaptcha2"] = 0;
            return View();*/
        }

        [Authorize]
        public ActionResult BasicInf()
        {
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 1))
            {
                OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return View();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        [Authorize]
        public ActionResult Home()
        {
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")//تهران
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                else
                    return RedirectToAction("Index", "First", new { area = "NewVer" });
            }
            else
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                else
                    return RedirectToAction("Index", "First", new { area = "NewVer" });
            }
            //if (Session["UserId"] == null)
            //    return RedirectToAction("logon", "Account");
            Models.cartaxEntities p = new cartaxEntities();
            p.sp_LogInProgramInsert(Convert.ToInt32(Session["UserId"]), "", Request.ServerVariables["REMOTE_ADDR"].ToString(), Session["UserPass"].ToString());
            var LastLogin = p.sp_LogInProgramSelect("fldUserID", Session["UserId"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
           
            if (LastLogin.Count > 0)
            {
                var time = DateTime.Now;
                OnlineUser.UpdateUrl(Session["UserId"].ToString(), "صفحه اصلی");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                ViewBag.time = time.Hour.ToString().PadLeft(2, '0') + ":" +
                    time.Minute.ToString().PadLeft(2, '0') + ":" +
                    time.Second.ToString().PadLeft(2, '0');
                return View();
            }
            else
                return RedirectToAction("ChangePassword", "Account");
        }

        [Authorize]
        public ActionResult FinancialInf()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 114))
            {
                OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات مالی");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return View();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        [Authorize]
        public ActionResult Config()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 168))
            {
                OnlineUser.UpdateUrl(Session["UserId"].ToString(), "پیکربندی سیستم");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return View();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        [Authorize]
        public ActionResult CarTax()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 222))
            {
                
                Avarez.Models.cartaxEntities p = new Avarez.Models.cartaxEntities();

                var Div = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
                {
                    var TransactionInf = p.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                    Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
                    string Mojodi = "0";
                    bool haveSharj = false;
                    if (TransactionInf != null)
                    {
                        var y = h.CheckAccountCharge(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName);
                        if (y != null)
                        {
                            Mojodi = y.Mojodi;
                            haveSharj = y.HaveCharge;
                        }
                    }
                    if (Mojodi != "" && haveSharj)
                    {
                        OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض");
                        SignalrHub hub = new SignalrHub();
                        hub.ReloadOnlineUser();
                        return View();
                    }
                    else
                    {
                        Session["ER"] = "جهت استفاده از امکانات نرم افزار موجودی حساب خود را افزایش دهید. لطفا به آدرس ذیل مراجعه فرمایید: http://trn.ecartax.ir";
                        return RedirectToAction("error", "Metro");
                    }
                }
                else
                    return View();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        [Authorize]
        public ActionResult Tools()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 269))
            {
                OnlineUser.UpdateUrl(Session["UserId"].ToString(), "ابزارهای سیستم");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return View();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        [Authorize]
        public ActionResult UsersMgr()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 255))
            {
                OnlineUser.UpdateUrl(Session["UserId"].ToString(), "مدیریت کاربران");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return View();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        [Authorize]
        public ActionResult CurrentUser()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            OnlineUser.UpdateUrl(Session["UserId"].ToString(), "تنظیمات کاربر جاری");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            return View();                        
        }

        [Authorize]
        public FileContentResult Image()
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();
            var user = p.sp_UserSelect("fldUserName", User.Identity.Name, 1, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (user != null)
            {
                var pic = p.sp_PictureSelect("fldUserPic", user.fldID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
        [Authorize]
        public FileContentResult showFile(int id,string type)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();
            int fileid=0;
            switch (type)
            {
                case "CarEx":
                    var carEx = p.sp_CarExperienceSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
                    fileid = (int)carEx.fldFileId;
                    break;
                case "Collection":
                    var Fish = p.sp_CollectionSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
                    fileid = (int)Fish.fldFileId;
                    break;
                case "Bargsabz":
                    var car = p.sp_SelectCarDetils(id).FirstOrDefault();
                    var file = p.sp_CarFileSelect("fldid", car.fldID.ToString(), 0, 1, "").FirstOrDefault();
                    fileid = (int)file.fldBargSabzFileId;
                    break;
                case "Cart":
                    var car1 = p.sp_SelectCarDetils(id).FirstOrDefault();
                    var file1 = p.sp_CarFileSelect("fldid", car1.fldID.ToString(), 0, 1, "").FirstOrDefault();
                    fileid = (int)file1.fldCartFileId;
                    break;
                case "CartBack":
                    var car3 = p.sp_SelectCarDetils(id).FirstOrDefault();
                    var file3 = p.sp_CarFileSelect("fldid", car3.fldID.ToString(), 0, 1, "").FirstOrDefault();
                    fileid = (int)file3.fldCartBackFileId;
                    break;
                case "Sanad":
                    var car2 = p.sp_SelectCarDetils(id).FirstOrDefault();
                    var file2 = p.sp_CarFileSelect("fldid", car2.fldID.ToString(), 0, 1, "").FirstOrDefault();
                    fileid = (int)file2.fldSanadForoshFileId;
                    break;
            }
            var image = p.Sp_FilesSelect(fileid).FirstOrDefault();
            if (image != null)
            {
                if (image.fldImage != null)
                {
                    return File((byte[])image.fldImage, "jpg");
                }
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

        public JsonResult GetCascadeState()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StateSelect("", "", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeCounty(int cboState)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(1, cboState, 5).ToList().OrderBy(h => h.NodeName);
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Guest()
        {
            if (Session["UserState"] == null)
                return RedirectToAction("logon", "Account");
            return View();
        }
        public ActionResult LogOnDafater(string UserName, string Password, string Captcha)
        {
            if (Captcha != Session["captchaText"].ToString() && Convert.ToInt32(Session["HaveCaptcha2"]) > 1)
            {
                return Json(new { Err = "لطفا کد امنیتی را صحیح وارد نمایید.", state = "0", HaveCaptcha2 = Session["HaveCaptcha2"] });
            }
            if (Membership.ValidateUser(UserName, Password))
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.sp_UserSelect("cheakPass", UserName, 1, Password.GetHashCode().ToString(), 1, "").FirstOrDefault();
                var UserLocation = p.sp_SelectTreeNodeID(Convert.ToInt32(q.fldID)).FirstOrDefault();
                if (q.fldStatus == false)
                {
                    return Json(new { Err = "شما مجاز به ورود در موقعیت انتخاب شده نمی باشید.", state = "0", HaveCaptcha2 = Session["HaveCaptcha2"] });
                }
                Session["UserId"] = q.fldID;
                Session["UserPass"] = Password.GetHashCode().ToString();
                Session["UserName"] = UserName;
                var MunID = "";

                switch(q.CountryType){
                    case 5:
                        Session["CountryType"] = 5;
                        Session["CountryCode"] =q.CountryCode ;
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
                        break;
                }
                if (MunID == "")
                    return Json(new { Err = "", state = "1", HaveCaptcha2 = Session["HaveCaptcha2"] });

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

                return Json(new { Err = "", state = "0", HaveCaptcha2 = Session["HaveCaptcha2"] });
               
            }
            else
            {
                Session["HaveCaptcha2"] = Convert.ToInt32(Session["HaveCaptcha2"]) + 1;
                return Json(new { Err = "نام کاربری یا کلمه عبور صحیح نیست.", state = "0", HaveCaptcha2 = Session["HaveCaptcha2"] });
            }


        }
    }
}
