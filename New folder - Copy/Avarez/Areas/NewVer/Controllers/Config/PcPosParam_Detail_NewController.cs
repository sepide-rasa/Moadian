using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;
using System.IO;
namespace Avarez.Areas.NewVer.Controllers.Config
{
    public class PcPosParam_Detail_NewController : Controller
    {
        //
        // GET: /NewVer/PcPosParam_Detail_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "پیکربندی سیستم-> مقدار دهی پارامتر PcPos");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }

        public ActionResult Reload(int value)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var data = m.sp_tblPcPos_Param_Value(value).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPcPosInfo()
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q = car.sp_PcPosInfoSelect("", "",0,0, 0).ToList().Select(c => new { fldId = c.fldId, fldBankName = c.fldBankName + "(" + c.fldCountryDivisionName + ")" });
            return this.Store(q);
        }
        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(List<Models.sp_PcPosParam_DetailSelect> Value, int PcPosInfoId)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New");
                Models.cartaxEntities p = new Models.cartaxEntities();
                //ثبت رکورد جدید
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 340))
                {
                    p.sp_PcPosParam_Detail_InfoIdDelete(PcPosInfoId, Convert.ToInt32(Session["UserId"]));
                    foreach (var item in Value)
                        p.sp_PcPosParam_DetailInsert(item.fldParamId, item.fldValue, Convert.ToInt32(Session["UserId"]), "", PcPosInfoId);

                    return Json(new { Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = "ذخیره موفق", Er = 0 });
                }
                else
                {
                    return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Er = 1 });
                    //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    //return RedirectToAction("error", "Metro");
                }
            }
            catch (Exception x)
            {
               // return Json(new { data = x.InnerException.Message, state = 1 });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }
    }
}
