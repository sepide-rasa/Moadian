using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;
using System.Collections;
using System.IO;
using System.Configuration;

namespace Avarez.Areas.NewVer.Controllers.AppReport
{
    public class MgrReport_NewController : Controller
    {
        //
        // GET: /NewVer/MgrReport_New/

        public ActionResult FromDateToDate(string containerId, int state) 
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات مدیریتی->گزارش آماری(ریالی)");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.stateFD = state;
            if (Session["UserMnu"].ToString() == "612")
            {
                result.ViewBag.StartDate = DateTime.Now.Date.AddDays(-30);
            }
            else
            {
                result.ViewBag.StartDate = "0";
            }
            return result;
        }
        public ActionResult FromDateToDateDarsadi(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات مدیریتی->گزارش آماری(درصدی)");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            if (Session["UserMnu"].ToString() == "612")
            {
                result.ViewBag.StartDate = DateTime.Now.Date.AddDays(-30);
            }
            else
            {
                result.ViewBag.StartDate = "0";
            }
            return result;
        }
        public ActionResult FromYear(string containerId, int state)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (state == 1)
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات مدیریتی->گزارش تفکیکی درآمد به ماه و سال ");
            }
            else if (state == 5)
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات مدیریتی->فیش های صادر شده به تفکیک ماه ");
            }
            else if (state == 6)
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات مدیریتی->فیش های صادر شده به تفکیک توابع ");
            }
            else if (state == 7)
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات مدیریتی->فیش های صادر شده به تفکیک ماه و توابع ");
            }
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var k=car.sp_GetDate().FirstOrDefault().DateShamsi;
            var sal=k.Substring(0, 4);
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.stateFY = state.ToString();
            result.ViewBag.salFY = sal;
            return result;
        }
        public ActionResult FromYearFish(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات مدیریتی->گزارش تعداد فیش به تفکیک ماه وسال");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var k = car.sp_GetDate().FirstOrDefault().DateShamsi;
            var sal = k.Substring(0, 4);
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.salFish = sal;
            return result;
        }
        public ActionResult FromYearBudje(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات مدیریتی->تفریغ بودجه در سال");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var k = car.sp_GetDate().FirstOrDefault().DateShamsi;
            var sal = k.Substring(0, 4);
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.salB = sal;
            return result;
        }
        public ActionResult CheckBudget(string Sal)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            var total = 0;
            var Budget = m.sp_BudgetByMonthSelect("fldMun_Year", Session["UserMnu"].ToString(), Sal.ToString(), 0).FirstOrDefault();
            if (Budget != null)
            {
                total = Budget.fldTotalBudget;
            }
            return Json(new { total = total,Budget = Budget }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FromYearSaliyane(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات مدیریتی->درآمد سالیانه به تفکیک ماه و نوع خودرو");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var k = car.sp_GetDate().FirstOrDefault().DateShamsi;
            var sal = k.Substring(0, 4);
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.salYS = sal;
            return result;
        }
        public JsonResult GetYear()
        {
            if (Session["UserId"] == null)
                return null;
            List<SelectListItem> sal = new List<SelectListItem>();

            for (int i = 1390; i <= 1405; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString();
                item.Value = i.ToString();
                sal.Add(item);
            }

            return Json(sal.OrderByDescending(k => k.Text).Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintChart(string containerId, string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.SDate = SDate;
            result.ViewBag.EDate = EDate;
            return result;
        }
        public ActionResult GeneratePDFChart(string SDate, string EDate)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptChartTableAdapter Collection = new Avarez.DataSet.DataSet1TableAdapters.sp_RptChartTableAdapter();
                dt.EnforceConstraints = false;

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
                Report.SetParameterValue("AzTarikh", SDate);
                Report.SetParameterValue("TaTarikh", EDate);
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);


                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintPersentChart(string containerId, string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.SDate = SDate;
            result.ViewBag.EDate = EDate;
            return result;
        }
        public ActionResult GeneratePDFPersentChart(string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptChartTableAdapter Collection = new Avarez.DataSet.DataSet1TableAdapters.sp_RptChartTableAdapter();
                dt.EnforceConstraints = false;

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
                
                Report.SetParameterValue("AzTarikh", SDate);
                Report.SetParameterValue("TaTarikh",EDate);
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);

                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintTafkikDaramad(string containerId, string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            return result;
        }
        public ActionResult GeneratePDFTafkikDaramad(string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyChartTableAdapter MonthlyChart = new Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyChartTableAdapter();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


                MonthlyChart.Fill(dt.sp_RptMonthlyChart, Convert.ToInt32(Year), Convert.ToInt32(Session["UserMnu"]));
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
                Report.SetParameterValue("sal", Year.ToString());
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);

                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintCountFish(string containerId, string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            return result;
        }
        public ActionResult GeneratePDFCountFish(string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyChart_CountTableAdapter MonthlyChart_Count = new Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyChart_CountTableAdapter();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


                MonthlyChart_Count.Fill(dt.sp_RptMonthlyChart_Count, Convert.ToInt32(Year), Convert.ToInt32(Session["UserMnu"]));
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
                Report.SetParameterValue("sal", Year.ToString());
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);

                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintYearBudget_Tafriq(string containerId, string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            return result;
        }
        public ActionResult GeneratePDFYearBudget_Tafriq(string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_YearBudget_TafriqSelectTableAdapter Budget_Tafriq = new Avarez.DataSet.DataSet1TableAdapters.sp_YearBudget_TafriqSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_BudgetByMonthSelectTableAdapter BudgetByMonth = new Avarez.DataSet.DataSet1TableAdapters.sp_BudgetByMonthSelectTableAdapter();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


                Budget_Tafriq.Fill(dt.sp_YearBudget_TafriqSelect, Convert.ToInt32(Year), Convert.ToInt32(Session["UserMnu"]));
                BudgetByMonth.Fill(dt.sp_BudgetByMonthSelect, "fldMun_Year", Session["UserMnu"].ToString(), Year.ToString(), 0);

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
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                var Msg = x.Message;
                if (x.InnerException != null)
                {
                    Msg = x.InnerException.Message;
                }
                return Json(new { Msg = Msg }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintMounthlyTipCollection(string containerId, string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            return result;
        }
        public ActionResult GeneratePDFMounthlyTipCollection(string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_MonthlyCollectionTipChartTableAdapter Collection = new Avarez.DataSet.DataSet1TableAdapters.sp_MonthlyCollectionTipChartTableAdapter();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");


                Collection.Fill(dt.sp_MonthlyCollectionTipChart, Convert.ToInt32(Year), Convert.ToInt32(Session["UserMnu"]));
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
                Report.SetParameterValue("year", Year.ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);

                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrintFishWithMonth(string containerId, string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            return result;
        }

        public ActionResult GeneratePDFFishWithMonth(string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptCountPaid_NotPaidTableAdapter NotPaid = new Avarez.DataSet.DataSet1TableAdapters.sp_RptCountPaid_NotPaidTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptCountPaid_NotPaid1TableAdapter Paid = new Avarez.DataSet.DataSet1TableAdapters.sp_RptCountPaid_NotPaid1TableAdapter();

                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");

                Paid.Fill(dt.sp_RptCountPaid_NotPaid1, Year, "Paid");
                NotPaid.Fill(dt.sp_RptCountPaid_NotPaid, Year, "NotPaid");

                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\CountPaid_NotPaid.frx");
                Report.RegisterData(dt, "carTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("year", Year.ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);

                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrintFishWithTavabe(string containerId, string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            return result;
        }

        public ActionResult GeneratePDFFishWithTavabe(string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_CountPaid_NotPaid_TafkikTavabeTableAdapter NotPaid = new Avarez.DataSet.DataSet1TableAdapters.sp_CountPaid_NotPaid_TafkikTavabeTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_CountPaid_NotPaid_TafkikTavabe1TableAdapter Paid = new Avarez.DataSet.DataSet1TableAdapters.sp_CountPaid_NotPaid_TafkikTavabe1TableAdapter();

                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");


                Paid.Fill(dt.sp_CountPaid_NotPaid_TafkikTavabe1,"Paid", Year);
                NotPaid.Fill(dt.sp_CountPaid_NotPaid_TafkikTavabe,"NotPaid", Year);

                
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\CountPaid_TafkikTavabe.frx");
                Report.RegisterData(dt, "carTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("year", Year.ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);

                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrintFishWithTavabe_Month(string containerId, string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            return result;
        }

        public ActionResult GeneratePDFFishWithTavabe_Month(string Year) 
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.Sp_RptPaid_NotPaid_Month1TableAdapter NotPaid = new Avarez.DataSet.DataSet1TableAdapters.Sp_RptPaid_NotPaid_Month1TableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.Sp_RptPaid_NotPaid_MonthTableAdapter Paid = new Avarez.DataSet.DataSet1TableAdapters.Sp_RptPaid_NotPaid_MonthTableAdapter();

                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                Paid.Fill(dt.Sp_RptPaid_NotPaid_Month, "Paid",Year);
                NotPaid.Fill(dt.Sp_RptPaid_NotPaid_Month1, "NotPaid",Year);

                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptPaid_NotPaid_Month.frx");
                Report.RegisterData(dt, "carTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("Sal", Year.ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
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
