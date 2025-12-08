using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Areas.Tax.Models;
using System.IO;

namespace Avarez.Areas.Tax.Controllers
{
    public class SooratHesabExcelTemplateController : Controller
    {
        //
        // GET: /Tax/SooratHesabExcelTemplate/
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        public ActionResult Index()
        {//باز شدن تب جدید
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            return new Ext.Net.MVC.PartialViewResult();


        }
        public ActionResult New(int id)
        {//باز شدن پنجره
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }
        public ActionResult Details(int Id)
        {
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var q = p.prs_tblSooratHesabExcelTemplateSelect("fldId", Id.ToString(), 0).FirstOrDefault();
           
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldStartRowNumber = q.fldStartRowNumber,
                fldDesc = q.fldDesc

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.prs_tblSooratHesabExcelTemplateSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.prs_tblSooratHesabExcelTemplateSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                        case "fldStartRowNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStartRowNumber";
                            break;
                    }
                    if (data != null)
                        data1 = p.prs_tblSooratHesabExcelTemplateSelect(field, searchtext, 100).ToList();
                    else
                        data = p.prs_tblSooratHesabExcelTemplateSelect(field, searchtext, 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.prs_tblSooratHesabExcelTemplateSelect("", "", 100).ToList();
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

            List<Models.prs_tblSooratHesabExcelTemplateSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Save(Models.prs_tblSooratHesabExcelTemplateSelect tem,List<Models.prs_tblSooratHesabExcelField_TemplateSelect> ListFields)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                Models.cartaxtest2Entities m = new Models.cartaxtest2Entities();


                if (tem.fldDesc == null)
                    tem.fldDesc = "";

                var q = m.prs_tblSooratHesabExcelTemplateSelect("fldTitle", tem.fldTitle, 0).FirstOrDefault();
                if (tem.fldId == 0)
                { //ذخیره
                    if (q != null)
                    {
                        Er = 1;
                        Msg = "الگو با این نام قبلا ثبت شده است.";
                        MsgTitle = "خطا";
                    }
                    else
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter UId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                        m.prs_tblSooratHesabExcelTemplateInsert(UId, tem.fldTitle, tem.fldStartRowNumber,Convert.ToInt64(Session["TaxUserId"]), tem.fldDesc,IP);
                        foreach (var item in ListFields)
                        {
                            m.prs_tblSooratHesabExcelField_TemplateInsert(Convert.ToInt32(UId.Value), item.fldParametrId, item.fldColumnNum, Convert.ToInt64(Session["TaxUserId"]), "",IP);
                        }
                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";
                    }

                }
                else
                { //ویرایش
                    if (q != null && q.fldId != tem.fldId)
                    {
                        Er = 1;
                        Msg = "الگو با این نام قبلا ثبت شده است.";
                        MsgTitle = "خطا";
                    }
                    else
                    {
                        m.prs_tblSooratHesabExcelTemplateUpdate(tem.fldId, tem.fldTitle, tem.fldStartRowNumber, Convert.ToInt64(Session["TaxUserId"]), tem.fldDesc,IP);
                        m.prs_tblSooratHesabExcelField_TemplateDelete(tem.fldId, Convert.ToInt64(Session["TaxUserId"]),IP);
                        foreach (var item in ListFields)
                        {
                            m.prs_tblSooratHesabExcelField_TemplateInsert(tem.fldId, item.fldParametrId, item.fldColumnNum, Convert.ToInt64(Session["TaxUserId"]), "",IP);
                        }

                        Msg = "ویرایش با موفقیت انجام شد.";
                        MsgTitle = "ویرایش موفق";
                    }
                }

            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;
                Er = 1;
                MsgTitle = "خطا";
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int Id)
        {//حذف یک رکورد
            try
            {
                if (Session["TaxUserId"] == null)
                    return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

                Models.cartaxtest2Entities m = new Models.cartaxtest2Entities();
                m.prs_tblSooratHesabExcelTemplateDelete(Convert.ToInt32(Id), Convert.ToInt64(Session["TaxUserId"]),IP);
                return Json(new
                {
                    MsgTitle = "حذف موفق",
                    Msg = "حذف با موفقیت انجام شد.",
                    Er = 0
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception x)
            {
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = InnerException,
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ReadFields( int TempId)
        {
            Models.cartaxtest2Entities m = new Models.cartaxtest2Entities();
            List<prs_SelectParametr_Field_Template> data = null;
            data = m.prs_SelectParametr_Field_Template(TempId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
