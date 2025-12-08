using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Avarez.Models;
using System.Net.NetworkInformation;
using System.IO;
using DotNetOpenAuth.Messaging;
using System.Net;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.RelyingParty;
using System.Configuration;
using System.Web.Configuration;
namespace Avarez.Controllers
{

    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn
        public FileContentResult generateCaptcha()
        {
            System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
            CaptchaImage img = new CaptchaImage(90, 40, family);
            string text = img.CreateRandomText(5);
            img.SetText(text);
            img.GenerateImage();
            MemoryStream stream = new MemoryStream();
            img.Image.Save(stream,
            System.Drawing.Imaging.ImageFormat.Png);
            Session["captchaText"] = text;
            return File(stream.ToArray(), "jpg");
        }

        public ActionResult LogOn()
        {
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")//تهران
            {
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            }
            else
            {
                return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
            }
            /*if (Session["HaveCaptcha"] == null)
                Session["HaveCaptcha"] = 0;
            return View();*/
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


        public ActionResult DetailNews(int id)
        {
            try
            {
                Models.cartaxEntities car = new cartaxEntities();
                var News = car.sp_NewsSelect("fldID", id.ToString(), 0, 1, "").FirstOrDefault();

                return Json(new
                {
                    title = News.fldSubject,
                    body = News.fldMemo,
                    date = News.fldDate
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public String MnuNews(int id)
        {
            ViewBag.MunicipalityID = id;
            Models.cartaxEntities car = new cartaxEntities();
            var News = car.sp_NewsSelect("fldMunicipalityID", id.ToString(), 0, 1, "").ToList();
            if (News.Count == 0)
                News = car.sp_NewsSelect("ISNULL", "", 0, 1, "").ToList();
            string news_titles = "";
            foreach (var item in News)
            {
                news_titles += "<span style='font-size:8px;'>></span><a href='#' id='" + item.fldID + "' class='NewsDetail'>" + item.fldSubject + "</a><br />";
            }
            return (news_titles);
        }
        public String MnuPic(int id)
        {
            Models.cartaxEntities car = new cartaxEntities();
            var picmnu = car.sp_PictureMunicipalitySelect("fldMunicipalityID", id.ToString(), 0, 1, "").ToList();
            if (picmnu.Count == 0)
                picmnu = car.sp_PictureMunicipalitySelect("ISNULL", "", 0, 1, "").ToList();
            string Pics = "";
            string start = "<div id='sliderFrame' dir='rtl'><div id='slider'>";
            string end = "</div></div>";
            foreach (var item in picmnu)
            {
                string p = Url.Content("~/Account/MunImage/" + item.fldID);
                Pics += "<img src='" + p + "' alt='" + item.fldName + "' />";
            }
            return (start + Pics + end);
        }
        public ActionResult Login(int id)
        {
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")//تهران
            {
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            }
            else
            {
                return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
            }
            cartaxEntities car = new cartaxEntities();
            string State = "";
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
            var q = car.sp_StateSelect("fldName", State, 1, 1, "").FirstOrDefault();
            ViewBag.StateId = q.fldID;
            return View("Logon");
        }

        //
        // POST: /Account/LogOn        

        public ActionResult OfficesLogin(string Office, string ver_code, string nc)
        {
            int office = -1;
            if (Office != null)
                if (!int.TryParse(Office, out office))
                    return null;
            if (office != -1 && Session["FirstCall"] == null)
            {
                // Handles relying party's request
                HandleRelyingPartyRequest(office);
                Session["FirstCall"] = 1;
            }
            else
            {
                // Handles a redirect from OpenID Provider
                // Handles a first time load of this page
                Session["ver_code"] = ver_code;
                HandleOpneIdProviderResponse();
                Session.Remove("FirstCall");
            }

            if (Session["FirstCall"] == null)
            {
                if (Session["Error"] != null)
                {
                    string error = Session["Error"].ToString();
                    Session.Remove("Error");
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Session.Remove("Error");
                    return RedirectToAction("home", "home");
                }
            }
            else
                return null;
        }
        private void HandleOpneIdProviderResponse()
        {
            // Define a new instance of OpenIdRelyingParty class
            using (var openid = new OpenIdRelyingParty())
            {
                // Get authentication response from OpenId Provider Create IAuthenticationResponse instance to be used
                // to retreive the response from OP
                var response = openid.GetResponse();

                // No authentication request was sent
                if (response == null) return;

                switch (response.Status)
                {
                    // If user was authenticated
                    case AuthenticationStatus.Authenticated:
                        // This is where you would look for any OpenID extension responses included
                        // in the authentication assertion.
                        var fetchResponse = response.GetExtension<FetchResponse>();

                        // Use FormsAuthentication to tell ASP.NET that the user is now logged in,
                        // with the OpenID Claimed Identifier as their username.
                        string userkey = fetchResponse.GetAttributeValue("userkey");
                        Session["userkey"] = userkey;

                        //epishkhan.devpishkhannwsv1 epishkhan = new Avarez.epishkhan.devpishkhannwsv1();
                        epishkhan_pos.devpishkhanposws epishkhan = new epishkhan_pos.devpishkhanposws();
                        //validate
                        //var result = epishkhan.validate("atJ5+$J1RtFpj", Session["ver_code"].ToString(), Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ServiceId"]));
                        int serviceid = 0;
                        Models.cartaxEntities p = new Models.cartaxEntities();
                        var q1 = p.sp_UserSelect("fldOfficeUserKey", userkey, 1, "", 1, "").FirstOrDefault();
                        if (q1 == null)
                        {
                            string[] res = epishkhan.info("7sFi[8r-${R5{B", "autotax4", Convert.ToInt32(userkey));
                            if (Convert.ToInt32(res[0]) > 0)
                            {
                                var q2 = p.sp_UserSelect("fldOfficeUserName", res[0], 1, "", 1, "").FirstOrDefault();
                                if (q2 != null)
                                    p.sp_UserUpdate_OfficeUserKey((int)q2.fldID, Convert.ToInt32(userkey));
                                else
                                {
                                    var m = p.sp_MunicipalitySelect("fldDesc", res[8], 0, 1, "").FirstOrDefault();
                                    if (m != null)
                                    {
                                        System.Data.Entity.Core.Objects.ObjectParameter _id=new System.Data.Entity.Core.Objects.ObjectParameter("fldid",typeof(int));
                                        p.Sp_RegisterInsert(_id, Convert.ToInt32(res[0]), res[9], res[5], m.fldID, null, null, false, res[2], res[3], res[4], "", "", true, res[13],"");
                                        var u = p.sp_UserSelect("fldId", "1", 0, "", 1, "").FirstOrDefault();
                                        var Office = p.Sp_RegisterSelect("fldId", _id.Value.ToString(), 30, 1, "").FirstOrDefault();
                                        WebTransaction.TransactionWebService trn = new WebTransaction.TransactionWebService();
                                        if (trn.Register(Office.fldCodeDaftar.ToString(), 8, u.fldDesc, Office.fldModirDaftar
                                            , Office.fldmodirFamily, Office.fldcodeMeli, Office.fldTel, Office.fldAddress))
                                        {
                                            var o = p.sp_OfficesSelect("fldName", "کد" + Office.fldCodeDaftar, 0, 1, "").FirstOrDefault();
                                            if (o == null)
                                            {
                                                Supporter.SendToSuporter s = new Supporter.SendToSuporter();
                                                s.insertOffice(Office.fldCodeDaftar.ToString(), Office.fldAddress, Office.fldMunId, Office.fldLocalId, Office.fldAreaId, Office.fldTel,"");
                                                if (WebConfigurationManager.AppSettings["IsBase"].ToString() == "false")
                                                {
                                                    p.sp_OfficesInsert("کد" + Office.fldCodeDaftar, Office.fldAddress, 1, Office.fldMunId,
                                                        Office.fldLocalId, Office.fldAreaId, 1, "", Office.fldTel, "", Office.fldExpireDate);
                                                }
                                                var ofice = p.sp_OfficesSelect("fldName", "کد" + Office.fldCodeDaftar, 0, 1, "").FirstOrDefault();
                                                System.Data.Entity.Core.Objects.ObjectParameter user_id = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(Int64));

                                                p.sp_UserInsert(user_id, Office.fldModirDaftar, Office.fldmodirFamily, true, Office.fldCodeDaftar.ToString().GetHashCode().ToString(), Office.fldCodeDaftar.ToString(),
                                                    Office.fldcodeMeli, "-", "0", Office.fldTel, Office.fldMobile, DateTime.Now, 8, (int)ofice.fldID, 1, "", null, userkey, "", false);

                                                p.sp_User_GroupInsert(2, Convert.ToInt64(user_id.Value), 1, "","");

                                                p.sp_TransactionInfInsert(Office.fldCodeDaftar.ToString(), 8, (int)ofice.fldID, CodeDecode.stringcode(Office.fldCodeDaftar.ToString()),1, "", false);

                                                p.Sp_RegisterUpdate(Office.fldId, true);
                                            }
                                            
                                        }
                                    }
                                }
                                q2 = p.sp_UserSelect("fldOfficeUserName", res[0], 1, "", 1, "").FirstOrDefault();
                                var ss = p.sp_UpPishkhanServiceSelect(9, (int)q2.fldID).FirstOrDefault();
                                if (ss != null)
                                    serviceid = ss.fldServiceId;
                            }
                        }
                        else
                        {
                            var s = p.sp_UpPishkhanServiceSelect(9, (int)q1.fldID).FirstOrDefault();
                            if (s != null)
                                serviceid = s.fldServiceId;
                        }
                        Session["ServiceId"] = serviceid;
                        var result = epishkhan.validate("7sFi[8r-${R5{B", "autotax4", Session["ver_code"].ToString(), Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ServiceId"]));
                        switch (result)
                        {
                            case 1:

                                var q = p.sp_UserSelect("fldOfficeUserKey", userkey, 1, "", 1, "").FirstOrDefault();
                                bool isUserInCurrentTree = false;
                                if (q != null)
                                {
                                    var UserLocation = p.sp_SelectTreeNodeID(Convert.ToInt32(q.fldID)).FirstOrDefault();
                                    var mnu = p.sp_SelectUpTreeCountryDivisions(UserLocation.fldID, 1, "").Where(k => k.fldNodeType == 5).FirstOrDefault();
                                    var state = p.sp_SelectUpTreeCountryDivisions(UserLocation.fldID, 1, "").Where(k => k.fldNodeType == 1).FirstOrDefault();
                                    //var mnu = p.sp_TableTreeSelect("fldSourceID", "2", 0, 0, 0).Where(h => h.fldNodeType == 5).FirstOrDefault();
                                    var user_Down = p.sp_SelectDownTreeCountryDivisions(mnu.fldID, 1, "").Where(h => h.fldSourceID == UserLocation.fldSourceID);
                                    var user_Up = p.sp_SelectUpTreeCountryDivisions(mnu.fldID, 1, "").Where(h => h.fldSourceID == UserLocation.fldSourceID);

                                    if (user_Down.Any())
                                        isUserInCurrentTree = true;
                                    else if (user_Up.Any())
                                        isUserInCurrentTree = true;

                                    if (isUserInCurrentTree && q.fldStatus == true)
                                    {
                                        Session["UserId"] = q.fldID;
                                        Session["UserPass"] = q.fldPassword;
                                        Session["UserMnu"] = mnu.fldSourceID;//آیدی شهرداری که کاربر با آن لاگین کرده
                                        Session["UserState"] = state.fldSourceID;//آیدی استانی که کاربر با آن لاگین کرده
                                        Session["IsOfficeUser"] = true;
                                        Session["UserName"] = q.fldUserName;

                                        var co = p.sp_SelectUpTreeCountryDivisions(UserLocation.fldID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                                        string area = "", offic = "";
                                        Session["CountryType"] = 5;
                                        Session["CountryCode"] = mnu.fldSourceID;
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
                                        FormsAuthentication.SetAuthCookie(q.fldUserName, true);
                                        OnlineUser.AddOnlineUser(q.fldID.ToString(), Request.ServerVariables["REMOTE_ADDR"].ToString(), mnu.fldSourceID.ToString());
                                    }
                                }
                                break;
                            case -3:
                                Session["Error"] = "مهلت تراکنش به پایان رسیده است.";
                                break;
                            case -7:
                                Session["Error"] = "مجوز استفاده از این سرویس را ندارید.";
                                break;
                            case -8:
                                Session["Error"] = "کاربر شما غیر فعال شده است.";
                                break;
                            case -10:
                                Session["Error"] = "اعتبار شما کافی نیست.";
                                break;
                        }

                        break;
                    // User has cancelled the OpenID Dance
                    case AuthenticationStatus.Canceled:
                        //this.loginCanceledLabel.Visible = true;
                        break;
                    // Authentication failed
                    case AuthenticationStatus.Failed:
                        //this.loginFailedLabel.Visible = true;
                        break;
                }

            }
        }


        private void HandleRelyingPartyRequest(int office)
        {
            try
            {
                // Create a new instance of OpenIdRelyingParty
                using (var openid = new OpenIdRelyingParty())
                {
                    // Create IAuthenticationRequest instance representing the Relying Party sending an authentication request
                    var request = openid.CreateRequest("http://auth.epishkhan.ir/identity/" + office);

                    // This is where you would add any OpenID extensions you wanted
                    // to include in the authentication request. In this case, we are making use of OpenID Attribute Exchange 1.0
                    // to fetch additional data fields from the OpenID Provider 
                    var fetchRequest = new FetchRequest();
                    fetchRequest.Attributes.AddRequired("http://axschema.org/userkey");
                    request.AddExtension(fetchRequest);

                    //this.request.Text = WellKnownAttributes.Name.Alias;
                    // Issue request to OP
                    request.RedirectToProvider();
                }
            }
            catch (ProtocolException pExp)
            {
                // The user probably entered an Identifier that was not a valid OpenID endpoint.
            }
            catch (WebException ex)
            {
                // The user probably entered an Identifier that was not a valid OpenID endpoint.
            }
        }
        [HttpPost]
        public ActionResult LogOn(Models.LogOnModel model, string returnUrl)
        {
            //if (ModelState.IsValid)
            //{
            //model.cboState = "11";//براي استانداري تهران
            if (model.cboState == null)
            {
                ModelState.AddModelError("", "لطفا استان را انتخاب نمایید.");
                return View(model);
            }
            //ViewBag.StateId = model.cboState;
            if (model.cboMnu == null)
            {
                ModelState.AddModelError("", "لطفا شهرداری را انتخاب نمایید.");
                return View(model);
            }
            if (model.Captcha != Session["captchaText"].ToString() && Convert.ToInt32(Session["HaveCaptcha"]) > 1)
            {
                ModelState.AddModelError("", "لطفا کد امنیتی را صحیح وارد نمایید.");
                return View(model);
            }
            if (model.UserType == 1)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    Models.cartaxEntities p = new Models.cartaxEntities();
                    var q = p.sp_UserSelect("cheakPass", model.UserName, 1, model.Password.GetHashCode().ToString(), 1, "").FirstOrDefault();
                    var UserLocation = p.sp_SelectTreeNodeID(Convert.ToInt32(q.fldID)).FirstOrDefault();
                    var mnu = p.sp_TableTreeSelect("fldSourceID", model.cboMnu, 0, 0, 0).Where(h => h.fldNodeType == 5).FirstOrDefault();
                    var user_Down = p.sp_SelectDownTreeCountryDivisions(mnu.fldID, 1, "").Where(h => h.fldSourceID == UserLocation.fldSourceID && h.fldNodeType != 9);
                    var user_Up = p.sp_SelectUpTreeCountryDivisions(mnu.fldID, 1, "").Where(h => h.fldSourceID == UserLocation.fldSourceID && h.fldNodeType != 9);
                    bool isUserInCurrentTree = false;
                    if (user_Down.Any())
                        isUserInCurrentTree = true;
                    else if (user_Up.Any())
                        isUserInCurrentTree = true;
                    //string userid = q.fldUserID.ToString();
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
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        OnlineUser.AddOnlineUser(q.fldID.ToString(), Request.ServerVariables["REMOTE_ADDR"].ToString(), model.cboMnu);
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Home", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "شما مجاز به ورود در موقعیت انتخاب شده نمی باشید.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "نام کاربری یا کلمه عبور صحیح نیست.");
                    Session["HaveCaptcha"] = Convert.ToInt32(Session["HaveCaptcha"]) + 1;
                    return View(model);
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
                return RedirectToAction("Guest", "Home");
            }
            //}

            // If we got this far, something failed, redisplay form
            //return View(model);
        }
        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            if (Session["UserId"] != null)
            {
                Models.OnlineUser.RemoveOnlineUser(Session["UserId"].ToString());
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                Session.RemoveAll();
                //Session.Remove("UserId");
                //Session.Remove("UserPass");
                //Session.Remove("UserMnu");
                //Session.Remove("UserState");
                //Session.Remove("UserName");
                //Session.Remove("Location");
                //Session.Remove("area");
                //Session.Remove("office");
            }
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(Models.RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            return PartialView();
        }

        //
        // POST: /Account/ChangePassword


        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(Models.ChangePasswordModel model)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            try
            {
                if (model.OldPassword.GetHashCode().ToString() == Session["UserPass"].ToString())
                {
                    if (model.NewPassword == model.ConfirmPassword)
                    {
                        Models.cartaxEntities p = new cartaxEntities();
                        p.sp_UserPassUpdate(Convert.ToInt32(Session["UserId"]), model.NewPassword.GetHashCode().ToString());
                        Session["UserPass"] = model.NewPassword.GetHashCode().ToString();
                        return Json(new { data = "تغییر رمز با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        return Json(new { data = "رمز جدید با تکرار آن برابر نیست.", state = 1 });
                    }
                }
                else
                {
                    return Json(new { data = "رمز قدیم نادرست است.", state = 1 });
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }
        public ActionResult ForgetPass()
        {
            return View();
        }
        public ActionResult SendSMS(string UserName, string Mobile)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            string Msg = "";
            var q = m.sp_UserSelect("fldUserName", UserName, 0, "", 1, "").Where(k => k.fldMobile == Mobile).FirstOrDefault();
            if (q != null)
            {
                System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
                CaptchaImage img = new CaptchaImage(90, 40, family);
                string text = img.CreateRandomText(5);

                m.sp_ForgetPass_VerifyInsert(text, false, q.fldID, "");

                SmsSender sendsms = new SmsSender();
                Msg = sendsms.SendVerifyMessage(q.fldCountryDivisionsID.ToString(), Mobile, text);
                //Msg = "پیامکی حاوی کد بازسازی رمزعبور برای شما ارسال می گردد.";
            }
            else
                Msg = "کاربری با این اطلاعات در سیستم ثبت نشده است.";

            return Json(new { Msg = Msg }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResetPass(string UserName, string Mobile, string VerifyCode)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            string Msg = "";
            var q = m.sp_UserSelect("fldUserName", UserName, 0, "", 1, "").Where(k => k.fldMobile == Mobile).FirstOrDefault();
            if (q != null)
            {
                var f = m.sp_ForgetPass_VerifySelect("fldUserId", q.fldID.ToString(), 0).Where(k => k.fldVerifyCode == VerifyCode && k.fldActive_Deactive == false).FirstOrDefault();
                if (f != null)
                {
                    m.sp_ForgetPass_VerifyUpdate(f.fldId);
                    m.sp_UserPassUpdate(Convert.ToInt32(q.fldID), q.fldUserName.GetHashCode().ToString());
                    Msg = "رمز عبور شما به نام کاربری تغییر پیدا کرد.";
                }
                else
                    Msg = "کد فعالسازی اشتباه است.";
            }
            else
                Msg = "کاربری با این اطلاعات در سیستم ثبت نشده است.";


            return Json(new { Msg = Msg }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
