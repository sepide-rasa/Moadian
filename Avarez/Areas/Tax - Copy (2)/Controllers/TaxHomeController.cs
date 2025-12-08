using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Avarez.Areas.Tax.Controllers
{
    public class TaxHomeController : Controller
    {
        //
        // GET: /Tax/TaxHome/

        public ActionResult Index()
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
             ViewBag.time = DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Second.ToString().PadLeft(2, '0');
            return View();
        }
        public ActionResult GetDate()
        {
            return Json(DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Second.ToString().PadLeft(2, '0'), JsonRequestBehavior.AllowGet);
        }
        public ActionResult LogOffTax()
        {
            if (Session["TaxUserId"] != null)
            {
                // TIBmodel.sp_tblLoggingInsert(int.Parse(Session["UserId"].ToString()), false, "");
                //model.sp_tblInputInfoInsert(model.sp_GetDate().FirstOrDefault().fldDateTime, Request.ServerVariables["REMOTE_HOST"].ToString(), "", false, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                //UserLoginCount.RemoveOnlineUser(Session["UserId"].ToString());
                Session.RemoveAll();
            }
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
        }
        public ActionResult ChangePassTax()
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult SaveChangePass(string fldPass, string fldNewPass, string fldConfirmPass)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            string Msg = "", MsgTitle = "";
            try
            {
                Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();

                var q = p.prs_User_GharardadSelect("fldId", Session["TaxUserId"].ToString(),0 ,"", 1,"").FirstOrDefault();

                var pass = CodeDecode.GenerateHash(fldPass);
                if (q.fldPassword == pass)
                {
                    if (fldNewPass == fldConfirmPass)
                    {
                        p.prs_User_GharardadResetPass(q.fldID, CodeDecode.GenerateHash(fldNewPass) );
                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";

                    }
                    else
                    {
                        Msg = "رمز جدید با تکرار آن یکسان نیست.";
                        MsgTitle = "اخطار";
                    }
                }
                else
                {
                    Msg = "رمز فعلی وارد شده معتبر نیست.";
                    MsgTitle = "اخطار";
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
