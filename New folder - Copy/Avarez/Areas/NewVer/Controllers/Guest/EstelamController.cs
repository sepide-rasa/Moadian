using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.NewVer.Controllers.Guest
{
    public class EstelamController : Controller
    {
        //
        // GET: /NewVer/Estelam/

        public ActionResult Index()
        {
            ViewData.Model = new Avarez.Models.clsEstelam();

            return View();
        }
        public ActionResult ReadPay()
        {

            return null;
        }
        public ActionResult getcar(string vin, string captcha)
        {
            try
            {
                if (Session["captchaLogin"].ToString().ToLower() != captcha.ToLower())
                    return Json(new { Err = "1", msg = "کد امنیتی وارد شده صحیح نمی باشد." }, JsonRequestBehavior.AllowGet);
                Estelam es = new Estelam();
                var c = es.GetCar(vin, "admin", "@DM!N");
                if (c != null)
                    return Json(new { car = c, Err = "0" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Err = "1", msg = "اطلاعات مربوط به VIN وارد شده در سامانه وجود ندارد." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return null;
            }
        }
    }
}
