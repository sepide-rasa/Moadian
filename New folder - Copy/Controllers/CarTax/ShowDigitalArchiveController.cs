using Avarez.Controllers.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.CarTax
{
    public class ShowDigitalArchiveController : Controller
    {
        //
        // GET: /ShowDigitalArchive/

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 370))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->مشاهده بایگانی دیجیتال(ساختار درختی)");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult Reload(string value1, string value2,int Pid)
        {//
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_ShowDigitalArchive(MyLib.Shamsi.Shamsi2miladiDateTime(value1), MyLib.Shamsi.Shamsi2miladiDateTime(value2),Pid).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public JsonResult _ProductTree(int? id)
        {
            try
            {
                var p = new Models.ComplicationsCarDBEntities();

                if (id != null)
                {
                    var rols = (from k in p.sp_tblDigitalTreeSelect("PId", id.ToString(), 0)
                                select new
                                {
                                    id = k.fldId,
                                    Pid = k.PId,
                                    Name = k.fldName,
                                    hasChildren = p.sp_tblDigitalTreeSelect("PId", id.ToString(), 0).Any()

                                });
                    return Json(rols, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var rols = (from k in p.sp_tblDigitalTreeSelect("", "", 0)
                                select new
                                {
                                    id = k.fldId,
                                    Pid = k.PId,
                                    Name = k.fldName,
                                    hasChildren = p.sp_tblDigitalTreeSelect("", "", 0).Any()

                                });
                    return Json(rols, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                return null;
            }
        }
    }
}
