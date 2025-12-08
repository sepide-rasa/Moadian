using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.Config
{
    public class PcPosParam_DetailController : Controller
    {
        //
        // GET: /PcPosParam_Detail/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 339))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public JsonResult GetPcPosInfo()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PcPosInfoSelect("", "",0,0, 0).Select(c => new { fldID = c.fldId, fldName = c.fldBankName + "(" + c.fldCountryDivisionName + ")" }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }


        public ActionResult Save(List<Models.sp_PcPosParam_DetailSelect> Value, int PcPosInfoId)
        {
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();


                //ثبت رکورد جدید
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 340))
                {
                    p.sp_PcPosParam_Detail_InfoIdDelete(PcPosInfoId, Convert.ToInt32(Session["UserId"]));
                    foreach (var item in Value)
                        p.sp_PcPosParam_DetailInsert(item.fldParamId, item.fldValue, Convert.ToInt32(Session["UserId"]), "",item.fldPcPosInfoId);

                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public ActionResult Reload(int value)
        {//جستجو
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_tblPcPos_Param_Value(value).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

       
    }
}
