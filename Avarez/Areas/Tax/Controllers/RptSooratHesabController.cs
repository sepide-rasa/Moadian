using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
using Ext.Net;
using Ext.Net.MVC;
using Avarez.Models;
using Aspose.Cells;
using System.IO;
using FastReport;

namespace Avarez.Areas.Tax.Controllers
{
    public class RptSooratHesabController : Controller
    {
        //
        // GET: /Tax/RptSooratHesab/

        public ActionResult Index(int state)
        {//باز شدن تب جدید
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            //var res=new Ext.Net.MVC.PartialViewResult();
            Avarez.Models.cartaxEntities m = new cartaxEntities();
            var q = m.sp_GetDate().FirstOrDefault();
            
            PartialView.ViewBag.Tarikh = q.DateShamsi;
            return PartialView;

        }
        public FileResult CreateExcel(string Checked, string DateStart, string DateEnd)
        {
            if (Session["TaxUserId"] == null)
                return null;
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
            int index = 0;
            var StatusCheck = Checked.Split(';');
            var Check = "";
            var fldIndatim = ""; var fldShomareFish = ""; var fldkh_name = ""; var fldkh_fldNationalCode = ""; var fldkh_CodeEghtesadi = ""; var SumSooratHesab = ""; var fldStatusName = "";var RaveshTasviye = "";
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            List<Models.prs_tblSooratHesab_HeaderSelect> data = null;

            

            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];

            foreach (var item in StatusCheck)
            {
                
                        data = p.prs_tblSooratHesab_HeaderSelect("AzTarikh_TaTarikh", Session["TarafGharardadId"].ToString(), DateStart, DateEnd, 0).ToList();

                // "fldIndatim" + ";"+"fldShomareFish" + ";" + "fldkh_name" + ";"+ "fldkh_fldNationalCode" + ";"+ "fldkh_CodeEghtesadi" + ";" + "SumSooratHesab" + ";" + "fldStatusName" + ";"
                switch (item)
                {
                    case "fldIndatim":
                        Check = "تاریخ صدور";
                        Cell cell = sheet.Cells[alpha[index] + "1"];
                        cell.PutValue(Check);
                        int i = 0;
                        foreach (var _item in data)
                        {
                            fldIndatim = _item.fldIndatim;
                            Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                            Cell.PutValue(fldIndatim);
                            i++;
                        }
                        index++;
                        break;
                    case "fldShomareFish":
                        Check = "شماره فیش";
                        Cell cell1 = sheet.Cells[alpha[index] + "1"];
                        cell1.PutValue(Check);
                        int j = 0;
                        foreach (var _item in data)
                        {
                            fldShomareFish = _item.fldShomareFish;
                            Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                            Cell.PutValue(fldShomareFish);
                            j++;
                        }
                        index++;
                        break;
                    case "fldkh_name":
                        Check = "نام خریدار";
                        Cell cell2 = sheet.Cells[alpha[index] + "1"];
                        cell2.PutValue(Check);
                        int k = 0;
                        foreach (var _item in data)
                        {
                            fldkh_name = _item.fldkh_name;
                            Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                            Cell.PutValue(fldkh_name);
                            k++;
                        }
                        index++;
                        break;
                    case "fldkh_fldNationalCode":
                        Check = "شناسه ملی";
                        Cell cell3 = sheet.Cells[alpha[index] + "1"];
                        cell3.PutValue(Check);
                        int q = 0;
                        foreach (var _item in data)
                        {
                            fldkh_fldNationalCode = _item.fldkh_fldNationalCode;
                            Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                            Cell.PutValue(fldkh_fldNationalCode);
                            q++;
                        }
                        index++;
                        break;
                    case "fldkh_CodeEghtesadi":
                        Check = "کداقتصادی";
                        Cell cell4 = sheet.Cells[alpha[index] + "1"];
                        cell4.PutValue(Check);
                        int w = 0;
                        foreach (var _item in data)
                        {
                            fldkh_CodeEghtesadi = _item.fldkh_CodeEghtesadi;
                            Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                            Cell.PutValue(fldkh_CodeEghtesadi);
                            w++;
                        }
                        index++;
                        break;
                    case "SumSooratHesab":
                        Check = "جمع مبلغ";
                        Cell cell5 = sheet.Cells[alpha[index] + "1"];
                        cell5.PutValue(Check);
                        int d = 0;
                        foreach (var _item in data)
                        {
                            SumSooratHesab = _item.SumSooratHesab;
                            Cell Cell = sheet.Cells[alpha[index] + (d + 2)];
                            Cell.PutValue(SumSooratHesab);
                            d++;
                        }
                        index++;
                        break;
                    case "RaveshTasviye":
                        Check = "روش تسویه";
                        Cell cell7 = sheet.Cells[alpha[index] + "1"];
                        cell7.PutValue(Check);
                        int e = 0;
                        foreach (var _item in data)
                        {
                            RaveshTasviye = _item.fldRaveshTasviye;
                            Cell Cell = sheet.Cells[alpha[index] + (e + 2)];
                            Cell.PutValue(RaveshTasviye);
                            e++;
                        }
                        index++;
                        break;
                    case "fldStatusName":
                        Check = "وضعیت";
                        Cell cell6 = sheet.Cells[alpha[index] + "1"];
                        cell6.PutValue(Check);
                        int f = 0;
                        foreach (var _item in data)
                        {
                            fldStatusName = _item.fldStatusName;
                            Cell Cell = sheet.Cells[alpha[index] + (f + 2)];
                            Cell.PutValue(fldStatusName);
                            f++;
                        }
                        index++;
                        break;
                }
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            return File(stream.ToArray(), "xls", "SooratHesab.xls");
        }
        public ActionResult GeneratePdf(string DateStart, string DateEnd)
        {
            {
                if (Session["TaxUserId"] == null)
                    return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
                try
                {
                    Models.cartaxtest2Entities m = new Models.cartaxtest2Entities();
                    Avarez.DataSet.DataSet1 dt = new DataSet.DataSet1();

                    Avarez.DataSet.DataSet1TableAdapters.sp_GetDateTableAdapter up = new Avarez.DataSet.DataSet1TableAdapters.sp_GetDateTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.prs_tblSooratHesab_HeaderSelectTableAdapter SooratHesab = new Avarez.DataSet.DataSet1TableAdapters.prs_tblSooratHesab_HeaderSelectTableAdapter();
                    up.Fill(dt.sp_GetDate);
                    SooratHesab.Fill(dt.prs_tblSooratHesab_HeaderSelect, "AzTarikh_TaTarikh", Session["TarafGharardadId"].ToString(), DateStart, DateEnd, 0);

                    FastReport.Report Report = new FastReport.Report();
                    dt.EnforceConstraints = false;
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Tax\Soorathesab.frx");

                    Report.RegisterData(dt, "moadianDataSet");
                    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                    pdf.EmbeddingFonts = true;
                    MemoryStream stream = new MemoryStream();
                    var u = m.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault();
                    var s = m.prs_tblSooratHesab_HeaderSelect("AzTarikh_TaTarikh", Session["TarafGharardadId"].ToString(), DateStart, DateEnd, 0).FirstOrDefault();
                    Report.SetParameterValue("UserName", u.fldName + " " + u.fldFamily);
                    Report.SetParameterValue("AzTarikh", DateStart);
                    Report.SetParameterValue("TaTarikh", DateEnd);
                    Report.SetParameterValue("Forushande", s.fldf_Name);
                    Report.Prepare();
                    Report.Export(pdf, stream);
                    return File(stream.ToArray(), "application/pdf");
                }
                catch (Exception x)
                {
                    return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
                }

            }
        }
    }
}
