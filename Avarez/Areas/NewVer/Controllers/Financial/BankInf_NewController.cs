using Avarez.Controllers.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Models;

namespace Avarez.Areas.NewVer.Controllers.Financial
{
    public class BankInf_NewController : Controller
    {
        //
        // GET: /NewVer/BankInf_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات مالی->تعریف اطلاعات پرداخت آنلاین");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                ViewData.Model = new Avarez.Models.ModelsofBankInf();

                var result = new Ext.Net.MVC.PartialViewResult
                {
                    ViewData = this.ViewData,
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };

                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return result;
                //}
                //else
                //{
                //    return null;
                //}
        }
        public ActionResult New(int? MunId, int? LocalId)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var fldType = 5;
            if (LocalId != null)
            {
                MunId = LocalId;
                fldType = 6;
            }
            PartialView.ViewBag.fldType = fldType;
            PartialView.ViewBag.MunId = MunId;
            return PartialView;
        }

        
        public JsonResult GetCascadeBank()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_BankSelect("", "", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeState()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StateSelect("", "", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeCounty(int cboState)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(1, cboState, 5).ToList();
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeLocal(int cboMnu)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(5, cboMnu, 6).ToList();
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Delete(int? DivisionID, int? BankId)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                int UserId = Convert.ToInt32(Session["UserId"]);
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 166))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                        Car.sp_BankInformationDelete(BankId, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString(), DivisionID);
                    
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
                if (x.InnerException.Message != null)
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
        public ActionResult Reload(string value, int type)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_SelectNameBankAndMunForBankInformation(Convert.ToInt32(value), type).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadDetailGrid(int BankId, int? MunId, int? LocalId)
        {
             int Type= 5;
             if (LocalId != 0)
             {
                 Type = 6;
                 MunId = LocalId;
             }
            
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_BankParameterSelect("fldBankID", BankId.ToString(), 100,Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString(), MunId, Type).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Grid_Save(List<Models.BankInf> ArrayL)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
        
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 167))
                    {
                        MsgTitle = "ذخیره موفق";
                        Msg = "ذخیره با موفقیت انجام شد.";

                       // System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("id", typeof(int));
                        //Car.sp_INSERT_IDCountryDivisions(ArrayL[0].fldType, ArrayL[0].fldMunID, Convert.ToInt32(Session["UserId"]), id);

                        Car.sp_BankInformationDelete(ArrayL[0].fldBankId,0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString(), ArrayL[0].fldDivisionID);
                        foreach (var item in ArrayL)
                        {
                            if (item.fldDesc == null)
                                item.fldDesc = "";

                            Car.sp_BankInformationInsert(null, item.fldValue, item.fldParametrID,
                                Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), ArrayL[0].fldDivisionID);
                        }
                    }
                    else
                    {
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به ذخیره اطلاعات نمی باشید.";
                        Er = 1;
                    }
                
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save_Bank(int? fldBankId, int? fldMunID, int? fldType)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();

                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 167))
                {
                    MsgTitle = "ذخیره موفق";
                    Msg = "ذخیره با موفقیت انجام شد.";

                    System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("id", typeof(int));
                    Car.sp_INSERT_IDCountryDivisions(fldType, fldMunID, Convert.ToInt32(Session["UserId"]), id);

                    Car.sp_BankInformationDelete(fldBankId,fldMunID,Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString(), Convert.ToInt32(id.Value));

                    var q = Car.sp_BankParameterSelect("fldBankID", fldBankId.ToString(), 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString(), fldMunID, fldType).ToList();
                    foreach (var item in q)
                    {
                        Car.sp_BankInformationInsert(null, "", item.fldID,Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Convert.ToInt32(id.Value));
                    }
                }
                else
                {
                    MsgTitle = "خطا";
                    Msg = "شما مجاز به ذخیره اطلاعات نمی باشید.";
                    Er = 1;
                }

            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckBank(int? Bank, int value, int type)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                var q = m.sp_SelectNameBankAndMunForBankInformation(value, type).Where(l => l.BankId == Bank).ToList();
                if (q.Count != 0)
                {
                    Msg = "بانک مورد نظر قبلا انتخاب شده است.";
                    MsgTitle = "خطا";
                    Er = 1;
                }
                else
                {
                    var b = m.sp_BankParameterSelect("fldBankID", Bank.ToString(), 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString(), value, type).ToList();
                    if (b.Count == 0)
                    {
                        Msg = "برای بانک مورد نظر پارامتری تعریف نشده است.";
                        MsgTitle = "خطا";
                        Er = 1;
                    }
                }
                return Json(new
                {
                    Er = Er,
                    Msg = Msg,
                    MsgTitle = MsgTitle
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
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
