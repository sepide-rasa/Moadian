using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.CarTax
{
    public class SearchCarFileController : Controller
    {
        //
        // GET: /SearchFile/

        public ActionResult Index(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            var Per = 0;
            if (id == 1)
                Per = 242;
            else if (id == 2)
                Per = 290;
            else if (id == 3)
                Per = 300;
            else if (id == 4)
                Per = 353;
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), Per))
            {
            //id=1 > جستجوی برای انتقال سوابق
            ViewBag.SearchState = id;
            //id=2 > جستجو برای ثبت فیش
            //id=2 > جستجو برای تعویض مالک
            return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult Reload(int field, string value1, string value2, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldVIN", "fldShasiAndMotorNumber", "fldMotor", "fldShasi", "fldOwnerName", "fldCodeMeli", "fldPelak" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value1);
            string searchtext2 = string.Format(searchType[searchtype], value2);
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            if (field == 0)
                value2 = "";
            var q = m.sp_CarUserGuestSelect(_fiald[field], searchtext, searchtext2, 30).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
    }
}
