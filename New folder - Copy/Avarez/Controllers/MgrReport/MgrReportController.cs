using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avarez.Controllers.Users;
using System.IO;

namespace Avarez.Controllers.MgrReport
{
    public class MgrReportController : Controller
    {
        //
        // GET: /MgrReport/
        
        public ActionResult Chart()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 304))
            {
            return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult RptChart(string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_RptChartTableAdapter Collection = new Avarez.DataSet.DataSet1TableAdapters.sp_RptChartTableAdapter();

            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");


            Collection.Fill(dt.sp_RptChart, MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), Convert.ToInt32(Session["UserMnu"]));
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\rptChart.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("OfficeName", Session["office"].ToString());
            Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
            

        }

        public ActionResult PersentChart()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 305))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult RptPersentChart(string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_RptChartTableAdapter Collection = new Avarez.DataSet.DataSet1TableAdapters.sp_RptChartTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");


            Collection.Fill(dt.sp_RptChart, MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), Convert.ToInt32(Session["UserMnu"]));
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\rptPersentChart.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("OfficeName", Session["office"].ToString());
            Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");

        }

        public ActionResult TafkikDaramad()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 306))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult RptTafkikDaramad(int Sal)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyChartTableAdapter MonthlyChart = new Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyChartTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


            MonthlyChart.Fill(dt.sp_RptMonthlyChart, Convert.ToInt32(Sal), Convert.ToInt32(Session["UserMnu"]));
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMonthlyChart.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("sal", Sal.ToString());
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("OfficeName", Session["office"].ToString());
            Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");

        }

        public ActionResult CountFish()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 307))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult RptCountFish(int sal)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyChart_CountTableAdapter MonthlyChart_Count = new Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyChart_CountTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


            MonthlyChart_Count.Fill(dt.sp_RptMonthlyChart_Count, Convert.ToInt32(sal), Convert.ToInt32(Session["UserMnu"]));
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptCountFish.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("sal", sal.ToString());
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("OfficeName", Session["office"].ToString());
            Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }

        public ActionResult YearBudget_Tafriq()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            
                return PartialView();
            
        }
        public ActionResult CheckBudget(string Sal)
        {
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            var Budget = m.sp_BudgetByMonthSelect("fldMun_Year", Session["UserMnu"].ToString(), Sal.ToString(), 0).FirstOrDefault();
            return Json(new { Budget = Budget }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RptYearBudget_Tafriq(int Sal)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 365))
            {
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_YearBudget_TafriqSelectTableAdapter Budget_Tafriq = new Avarez.DataSet.DataSet1TableAdapters.sp_YearBudget_TafriqSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_BudgetByMonthSelectTableAdapter BudgetByMonth = new Avarez.DataSet.DataSet1TableAdapters.sp_BudgetByMonthSelectTableAdapter();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


                Budget_Tafriq.Fill(dt.sp_YearBudget_TafriqSelect, Convert.ToInt32(Sal), Convert.ToInt32(Session["UserMnu"]));
                BudgetByMonth.Fill(dt.sp_BudgetByMonthSelect, "fldMun_Year", Session["UserMnu"].ToString(), Sal.ToString(), 0);

                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptYearBudget_Tafriq.frx");
                Report.RegisterData(dt, "CarTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);


                return File(stream.ToArray(), "application/pdf");

            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult MounthlyTipCollection()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 366))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult RptMounthlyTipCollection(int Sal)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_MonthlyCollectionTipChartTableAdapter Collection = new Avarez.DataSet.DataSet1TableAdapters.sp_MonthlyCollectionTipChartTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");


            Collection.Fill(dt.sp_MonthlyCollectionTipChart, Convert.ToInt32(Sal), Convert.ToInt32(Session["UserMnu"]));
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMonthlyTipChart.frx");
            Report.RegisterData(dt, "dataSet1");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("OfficeName", Session["office"].ToString());
            Report.SetParameterValue("year", Sal.ToString());
            Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
    }
}
