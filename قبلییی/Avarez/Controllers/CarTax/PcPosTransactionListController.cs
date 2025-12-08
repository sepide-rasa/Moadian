using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.CarTax
{
    public class PcPosTransactionListController : Controller
    {
        //
        // GET: /PcPosTransactionList/

        public ActionResult Index(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 342))
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                var car = m.sp_SelectCarDetils(id).FirstOrDefault();
                Session["CarFileId"] = car.fldID;
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
            var q = m.sp_PcPosTransactionSelect("fldCarFileId", Session["CarFileId"].ToString(), 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(int TransactionId)
        {
            try
            {
                Models.cartaxEntities m = new Models.cartaxEntities();

                System.Data.Entity.Core.Objects.ObjectParameter _Cid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
             
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 343))
                    {
                        var s = m.sp_PcPosTransactionSelect("fldId", TransactionId.ToString(), 0).FirstOrDefault();
                        var t = m.sp_PcPosTransactionUpdate_Status(TransactionId, "", "پرداخت با PcPos از طریق تایید دستی.");
                        m.sp_CollectionInsert(_Cid, Convert.ToInt64(Session["CarFileId"]), m.sp_GetDate().FirstOrDefault().CurrentDateTime, s.fldPrice, 9, null, null, "",
                            Convert.ToInt32(Session["UserId"]), "پرداخت با PcPos از طریق تایید دستی.", "", "", null, "", null, null, true, 1, DateTime.Now);

                        return Json(new { data = "تایید پرداخت با موفقیت انجام شد."});
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

        public ActionResult Reload()
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_PcPosTransactionSelect("fldCarFileId", Session["CarFileId"].ToString(), 30).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        

    }
}
