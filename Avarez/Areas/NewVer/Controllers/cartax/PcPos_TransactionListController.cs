using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Collections;
using Avarez.Controllers.Users;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class PcPos_TransactionListController : Controller
    {
        //
        // GET: /NewVer/PcPos_TransactionList/

        public ActionResult Index(string containerId, string CarId, string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 342))
            {
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };
                result.ViewBag.CarId = CarId;
                result.ViewBag.CarFileId = CarFileId;
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return result;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }

        public ActionResult Read(StoreRequestParameters parameters, string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities p = new Models.cartaxEntities();
            List<Models.sp_PcPosTransactionSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.sp_PcPosTransactionSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldPrice":
                            searchtext ="%"+ ConditionValue.Value.ToString()+ "%";
                            field = "fldPrice";
                            break;
                        case "fldTrackingCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTrackingCode";
                            break;
                        case "fldStatusName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStatusName";
                            break;
                    }
                    if (data != null)
                        data1 = p.sp_PcPosTransactionSelect(field, searchtext, 100).Where(l => l.fldCarFileId == Convert.ToInt32(CarFileId)).ToList();
                    else
                        data = p.sp_PcPosTransactionSelect(field, searchtext, 100).Where(l => l.fldCarFileId == Convert.ToInt32(CarFileId)).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.sp_PcPosTransactionSelect("fldCarFileId", CarFileId, 100).ToList();
            }

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            return !oValue.ToString().Contains(value.ToString());
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Models.sp_PcPosTransactionSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Save(int TransactionId, string CarFileId)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

                Models.cartaxEntities m = new Models.cartaxEntities();

                System.Data.Entity.Core.Objects.ObjectParameter _Cid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));

                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 343))
                {
                    var s = m.sp_PcPosTransactionSelect("fldId", TransactionId.ToString(), 0).FirstOrDefault();
                    var t = m.sp_PcPosTransactionUpdate_Status(TransactionId, "", "پرداخت با PcPos از طریق تایید دستی.");
                    m.sp_CollectionInsert(_Cid, Convert.ToInt64(CarFileId), m.sp_GetDate().FirstOrDefault().CurrentDateTime, s.fldPrice, 9, null, null, "",
                        Convert.ToInt32(Session["UserId"]), "پرداخت با PcPos از طریق تایید دستی.", "", "", null, "", null, null, true, 1, DateTime.Now);
                    SendToSamie.Send(Convert.ToInt32(_Cid.Value), Convert.ToInt32(Session["UserMnu"]));
                    return Json(new { MsgTitle = "عملیات موفق", Msg = "تایید پرداخت با موفقیت انجام شد.", Er = 0 });
                }
                else
                {
                    return Json(new { MsgTitle = "خطا", Msg = "شما مجاز به دسترسی نمی باشید.", Er = 1 });
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle="خطا",Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }

    }
}
