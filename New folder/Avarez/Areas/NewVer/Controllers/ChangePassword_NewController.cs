using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
using Ext.Net;
using Ext.Net.MVC;
using Avarez.Models;

namespace Avarez.Areas.NewVer.Controllers
{
    public class ChangePassword_NewController : Controller
    {
        //
        // GET: /NewVer/ChangePassword_New/

        public ActionResult Index(string UserType)
        {
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "تغییر رمز عبور");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            if (UserType == "1")
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            }
            else if (UserType == "3")
            {
                if (Session["UserGeust"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            }

            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.UserType = UserType;
            return PartialView;

        }

        public ActionResult changePass()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }

        public ActionResult Help()
        {//باز شدن پنجره

            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();

            return PartialView;
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(Models.ChangePasswordModel model, string UserType)
        {
            
            try
            {
                if (UserType == "1")
                {
                    if (Session["UserId"] == null)
                        return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                }
                else if (UserType == "3")
                {
                    if (Session["UserGeust"] == null)
                        return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                }
                    
                if (Session["UserId"] != null)
                {
                    if (model.OldPassword.GetHashCode().ToString() == Session["UserPass"].ToString())
                    {
                        if (model.NewPassword == model.ConfirmPassword)
                        {
                            if (model.OldPassword.GetHashCode().ToString() != model.NewPassword.GetHashCode().ToString())
                            {
                                Models.cartaxEntities p = new cartaxEntities();
                                p.sp_UserPassUpdate(Convert.ToInt32(Session["UserId"]), model.NewPassword.GetHashCode().ToString());
                                Session["UserPass"] = model.NewPassword.GetHashCode().ToString();
                                return Json(new { Msg = "تغییر رمز با موفقیت انجام شد.", MsgTitle = "عملیات موفق", Er = 0 });
                            }
                            else
                            {
                                return Json(new { Msg = "رمز جدید با رمز قدیم یکسان است.", MsgTitle = "خطا", Er = 1 });
                            }
                        }
                        else
                        {
                            return Json(new { Msg = "رمز جدید با تکرار آن برابر نیست.", MsgTitle = "خطا", Er = 1 });
                        }
                    }
                    else
                    {
                        return Json(new { Msg = "رمز قدیم نادرست است.", MsgTitle = "خطا", Er = 1 });
                    }
                }
                else if (Session["UserGeust"] != null)
                {
                    if (model.OldPassword.GetHashCode().ToString() == Session["UserPassGuest"].ToString())
                    {
                        if (model.NewPassword == model.ConfirmPassword)
                        {
                            Models.cartaxEntities p = new cartaxEntities();
                            p.sp_UpdatePassGuestUser(model.NewPassword.GetHashCode().ToString(), Convert.ToInt32(Session["UserGeust"]));
                            Session["UserPassGuest"] = model.NewPassword.GetHashCode().ToString();
                            return Json(new { Msg = "تغییر رمز با موفقیت انجام شد.", MsgTitle = "عملیات موفق", Er = 0 });
                        }
                        else
                        {
                            return Json(new { Msg = "رمز جدید با تکرار آن برابر نیست.", MsgTitle = "خطا", Er = 1 });
                        }
                    }
                    else
                    {
                        return Json(new { Msg = "رمز قدیم نادرست است.", MsgTitle = "خطا", Er = 1 });
                    }
                }
                return null;
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }
    }
}
