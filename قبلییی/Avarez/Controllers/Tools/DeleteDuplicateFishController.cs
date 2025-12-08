using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.Tools
{
    public class DeleteDuplicateFishController : Controller
    {
        //
        // GET: /DeleteDuplicateFish/

        public ActionResult Index()
        {
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 302))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult Delete(string From,string To)
        {
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 303))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    Car.sp_DeleteDuplicatedFish(Convert.ToInt32(Session["UserId"]), MyLib.Shamsi.Shamsi2miladiDateTime(From), MyLib.Shamsi.Shamsi2miladiDateTime(To));
                    return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
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
        public ActionResult DeleteByCarFile(int id)
        {
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 303))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    Car.sp_DeleteDuplicatedFishByCarFile(Convert.ToInt32(Session["UserId"]), id);
                    return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 },JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
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
