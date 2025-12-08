using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.Config
{
     [Authorize]
    public class ReportTypeController : Controller
    {
        //
        // GET: /ReportType/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 170))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_ReportTypeSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);
        }
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_ReportTypeSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().OrderBy(p => p.fldID);
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int Id)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New");
                int UserId = Convert.ToInt32(Session["UserId"]);
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 172))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    Car.sp_ReportsDelete(Convert.ToInt32(Id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                    return Json(new
                    {
                        MsgTitle = "حذف موفق",
                        Msg = "حذف با موفقیت انجام شد.",
                        Er = 0
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                m.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
