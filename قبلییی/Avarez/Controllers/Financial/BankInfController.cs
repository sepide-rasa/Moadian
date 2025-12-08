using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
namespace Avarez.Controllers.Financial
{
    [Authorize]
    public class BankInfController : Controller
    {
        //
        // GET: /BankInf/

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 165))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 166))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    var sp=id.Split('-');
                    if (sp.Count() >= 2)
                    {
                        Car.sp_BankInformationDelete(Convert.ToInt32(sp[0]), Convert.ToInt32(sp[1].Split('|')[0]),
                            Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString(), Convert.ToInt32(sp[1].Split('|')[1]));
                        return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 });
                    }
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
        public JsonResult GetCascadeState()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StateSelect("", "", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeCounty(int cboState)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(1, cboState, 5).ToList();
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeLocal(int cboMnu)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(5, cboMnu, 6).ToList();
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeBank()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_BankSelect("", "", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            return Json(null);

        }

        public ActionResult Reload(int value,int type)
        {//جستجو
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_SelectNameBankAndMunForBankInformation(value, type).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        
    }
}
