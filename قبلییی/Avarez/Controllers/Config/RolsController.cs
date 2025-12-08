using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.Config
{
     [Authorize]
    public class RolsController : Controller
    {
        //
        // GET: /Rols/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            return View();
        }

        public JsonResult _RolsTree(int? id)
        {            
            var p = new Models.cartaxEntities();
            
            if (id != null)
            {
                var rols = (from k in p.sp_ApplicationPartSelect("fldPID", id.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString())
                            select new
                            {
                                id = k.fldID,
                                Name = k.fldTitle,
                                hasChildren = p.sp_ApplicationPartSelect("fldPID", id.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var rols = (from k in p.sp_ApplicationPartSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString())
                            select new
                            {
                                id = k.fldID,
                                Name = k.fldTitle,
                                hasChildren = p.sp_ApplicationPartSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Save(Models.sp_ApplicationPartSelect Rols)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Rols.fldDesc == null)
                    Rols.fldDesc = "";
                if (Rols.fldID == 0)
                {//ثبت رکورد جدید
                    Car.sp_ApplicationPartInsert(Rols.fldTitle, Rols.fldPID, Convert.ToInt32(Session["UserId"]), Rols.fldDesc, DateTime.Now, Session["UserPass"].ToString());

                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                }
                else
                {//ویرایش رکورد ارسالی
                   
                    return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
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
    }
}
