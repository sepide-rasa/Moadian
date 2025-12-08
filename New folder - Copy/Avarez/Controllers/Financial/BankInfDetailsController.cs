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
    public class BankInfDetailsController : Controller
    {
        //
        // GET: /BankInfDetails/
        public ActionResult Index(int BankId, int MunId,int? LocalId)
        {
            //بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 165))
            {
                Session["BankId"] = BankId;
                Session["MunId"] = MunId;
                Session["fldType"] = 5;
                if (LocalId != null)
                {
                    Session["MunId"] = LocalId;
                    Session["fldType"] = 6;
                }
                ViewBag.MunId = Session["MunId"];
                ViewBag.fldType = Session["fldType"];
                ViewBag.BankId = BankId;
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
            var q = m.sp_BankParameterSelect("fldBankID", Session["BankId"].ToString(), 30,
                Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString(),
                Convert.ToInt32(Session["MunId"]),Convert.ToInt32(Session["fldType"])).ToList().ToDataSourceResult(request);
            Session.Remove("BankId");
            Session.Remove("MunId");
            return Json(q);
        }

        [HttpPost]
        public ActionResult Grid_Save(List<Models.BankInf> ArrayL)
        {
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 167))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();                    
                    
                    System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("id", typeof(int));
                    Car.sp_INSERT_IDCountryDivisions(ArrayL[0].fldType, ArrayL[0].fldMunID, Convert.ToInt32(Session["UserId"]), id);

                    Car.sp_BankInformationDelete(ArrayL[0].fldBankId, ArrayL[0].fldMunID,
                        Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString(), Convert.ToInt32(id.Value));
                    foreach (var item in ArrayL)
                    {
                        if (item.fldDesc == null)
                            item.fldDesc = "";

                        Car.sp_BankInformationInsert(null, item.fldValue, item.fldParametrID,
                            Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Convert.ToInt32(id.Value));
                    }
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
