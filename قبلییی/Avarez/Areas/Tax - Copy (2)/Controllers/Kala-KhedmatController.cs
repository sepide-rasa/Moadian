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
using System.Xml;

namespace Avarez.Areas.Tax.Controllers
{
    public class Kala_KhedmatController : Controller
    {
        //
        // GET: /Tax/Kala-Khedmat/

        public ActionResult Index()
        {//باز شدن تب جدید
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            //Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            //var k=p.prs_tblKala_KhedmatSelect("Test", "", 0).ToList();    
            //XmlDocument DOC = new XmlDocument();
            //DOC.Load(Server.MapPath(@"~\Uploaded\testKala.xml"));
            //XmlNode root1 = DOC.DocumentElement;
            //foreach (var item in k)
            //{
            //    XmlNode root = DOC.CreateElement("Table");
            //    root1.AppendChild(root);

            //    XmlNode type = DOC.CreateElement("fldCode");
            //    type.InnerText = item.fldCode;
            //    root.AppendChild(type);

            //    type = DOC.CreateElement("fldTypeShenaseId");
            //    type.InnerText = item.fldTypeShenaseId.ToString();
            //    root.AppendChild(type);

            //    type = DOC.CreateElement("fldTarikh");
            //    type.InnerText = item.fldTarikh;
            //    root.AppendChild(type);

            //    type = DOC.CreateElement("fldRunDate");
            //    type.InnerText = item.fldRunDate;
            //    root.AppendChild(type);

            //    type = DOC.CreateElement("fldExpDate");
            //    type.InnerText = item.fldExpDate;
            //    root.AppendChild(type);

            //    type = DOC.CreateElement("fldSpecialOrGeneral");
            //    type.InnerText = item.fldSpecialOrGeneral.ToString();
            //    root.AppendChild(type);

            //    type = DOC.CreateElement("fldTaxableOrFree");
            //    type.InnerText = item.fldTaxableOrFree.ToString();
            //    root.AppendChild(type);

            //    type = DOC.CreateElement("fldVat");
            //    type.InnerText = item.fldVat.ToString();
            //    root.AppendChild(type);

            //    type = DOC.CreateElement("fldVatCustomPurposes");
            //    type.InnerText = item.fldVatCustomPurposes.ToString();
            //    root.AppendChild(type);

            //    type = DOC.CreateElement("fldDescriptionOfID");
            //    type.InnerText = item.fldDescriptionOfID;
            //    root.AppendChild(type);
            //}
            
            //DOC.Save(Server.MapPath(@"~\Uploaded\testKala.xml"));

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
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.prs_tblKala_KhedmatSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.prs_tblKala_KhedmatSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldDescriptionOfID":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDescriptionOfID";
                            break;
                        case "fldCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCode";
                            break;
                    }
                    if (data != null)
                        data1 = p.prs_tblKala_KhedmatSelect(field, searchtext, 100).ToList();
                    else
                        data = p.prs_tblKala_KhedmatSelect(field, searchtext, 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.prs_tblKala_KhedmatSelect("", "", 100).ToList();
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

            List<Models.prs_tblKala_KhedmatSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Save(Models.prs_tblKala_KhedmatSelect kala,bool typeKala)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                Models.cartaxtest2Entities m = new Models.cartaxtest2Entities();

                kala.fldTypeShenaseId = null;
                kala.fldTarikh = null; 
                kala.fldRunDate = null;
                kala.fldExpDate = null;
                kala.fldSpecialOrGeneral = null;
                kala.fldTaxableOrFree = null;
                kala.fldVat = null; 
                kala.fldVatCustomPurposes = null;


                var q = m.prs_tblKala_KhedmatSelect("fldCode", kala.fldCode.ToString(), 0).FirstOrDefault();
              
                    if (q != null)
                    {
                        Er = 1;
                        Msg = "کالا/خدمت با این کد قبلا ثبت شده است.";
                        MsgTitle = "خطا";
                    }
                    else
                    {
                        m.prs_tblKala_KhedmatInsert(kala.fldCode, kala.fldTypeShenaseId, kala.fldTarikh,kala.fldRunDate, kala.fldExpDate, kala.fldSpecialOrGeneral, kala.fldTaxableOrFree, kala.fldVat, kala.fldVatCustomPurposes, kala.fldDescriptionOfID, typeKala);

                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";
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
    }
}
