using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.NewVer.Controllers
{
    public class queryController : Controller
    {
        //
        // GET: /NewVer/query/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult getMafasa(int id, string captcha)
        {
            if (Session["captchaLogin"].ToString().ToLower() == captcha.ToLower())
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.Sp_MafasaSelect_Image1(id.ToString()).Any();
                if (q == true)
                    return Json(new { haveMafasa = "1" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { haveMafasa = "0" ,msg= "کد رهگیری وارد شده صحیح نمیباشد."}, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { haveMafasa = "0" ,msg="لطفا کد امنیتی را صحیح وارد کنید."}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getMafasaRpt(int id)
        {
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.Sp_MafasaSelect_Image1(id.ToString()).FirstOrDefault();
                if (q != null)
                {
                    return File(q.fldimage.ToArray(), "application/pdf");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}
