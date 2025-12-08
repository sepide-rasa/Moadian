using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.Tax.Controllers
{
    public class AccountTaxController : Controller
    {
        //
        // GET: /Tax/AccountTax/

        public ActionResult Login()
        {
            if (Session["captchahaveTax"] == null)
                Session["captchahaveTax"] = 0;
            ViewBag.Title = "ورود به سامانه";
            ViewBag.captchahave = Convert.ToInt32(Session["captchahaveTax"]);
            return View();
        }
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
            Session["captchaLoginTax"] = text;
            return File(stream.ToArray(), "jpg");
        }
        public ActionResult Vorod(string UserName, string Password, string Capthalogin)
        {
            //if (ModelState.IsValid)
            //{
            string Msg = ""; var Er = 0; var flag = false; string MsgTitle = "";
            if (Convert.ToInt32(Session["captchahaveTax"]) > 1)
            {
                if (Capthalogin == "")
                {
                    Session["captchaLoginTax"] = "Error";
                    MsgTitle = "خطا";
                    Msg = "لطفا کد امنیتی را وارد نمایید.";
                    Er = 1;
                    Session["captchahaveTax"] = Convert.ToInt32(Session["captchahaveTax"]) + 1;
                    return Json(new
                    {
                        Msg = Msg,
                        MsgTitle = MsgTitle,
                        flag = flag,
                        captchahave = Session["captchahaveTax"]
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    if (Capthalogin.ToLower() != Session["captchaLoginTax"].ToString().ToLower())
                    {
                        Session["captchaLoginTax"] = "Error";
                        MsgTitle = "خطا";
                        Msg = "لطفا کد امنیتی را صحیح وارد نمایید.";
                        Er = 1;
                        Session["captchahaveTax"] = Convert.ToInt32(Session["captchahaveTax"]) + 1;
                        return Json(new
                        {
                            Msg = Msg,
                            MsgTitle = MsgTitle,
                            flag = flag,
                            captchahave = Session["captchahaveTax"]
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            Avarez.Areas.Tax.Models.cartaxtest2Entities p = new Avarez.Areas.Tax.Models.cartaxtest2Entities();

            var qq = p.prs_User_GharardadSelect("cheakPass", UserName, 1, CodeDecode.GenerateHash(Password), 1, "").FirstOrDefault();
            if (qq != null)
            {
               // var q = p.prs_tblTarfGharardadSelect("fldId", qq.fldTarfGharardadId.ToString(), 0).FirstOrDefault();
                if (qq.fldTarfGharardadId != null)
                {
                    Session["TaxUserId"] = qq.fldID;
                    Session["TarafGharardadId"] = qq.fldTarfGharardadId;

                    flag = true;
                    MsgTitle = "ورود موفق";
                }
                else
                {
                    Msg = "شما مجاز به ورود به سامانه مودیان نمی باشید.";
                    MsgTitle = "ورود ناموفق";
                    Session["captchahaveTax"] = Convert.ToInt32(Session["captchahaveTax"]) + 1;
                }
            }
            else
            {
                Msg = "نام کاربری یا رمزعبور صحیح نمی باشد.";
                MsgTitle = "ورود ناموفق";
                Session["captchahaveTax"] = Convert.ToInt32(Session["captchahaveTax"]) + 1;
            }





            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                flag = flag,
                captchahave = Session["captchahaveTax"]
            }, JsonRequestBehavior.AllowGet);
        }
      
    }
}
