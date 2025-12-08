using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avarez.Controllers.Users;
using System.IO;
using Microsoft.Reporting.WebForms;
using System.Xml;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.Configuration;
using Aspose.Cells;

namespace Avarez.Controllers.AppReport
{
    [Authorize]
    public class AppReportsController : Controller
    {
        //
        // GET: /Reports/
        public ActionResult MicrosoftReport_Fish() 
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter fish = new DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter();
            fish.Fill(dt.sp_PeacockerySelect, "fldid", "100", 1, 1, "");
            using (StreamReader rdlcSR = new StreamReader(Request.MapPath(Request.ApplicationPath) + @"Reports\rptfish.rdlc"))
            {
                reportViewer.LocalReport.LoadReportDefinition(rdlcSR);
                
            }
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("sp_PeacockerySelect", dt.Tables["sp_PeacockerySelect"]));
reportViewer.LocalReport.Refresh();
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        public ActionResult PelaqueSerial()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 250))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->سریال پلاک ها");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult CarColor()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 251))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->رنگ های خودرو");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult User()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 252))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->کاربرهای تعریف شده");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult IDCountryDivisions(string Code, string NType)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var IdCountryDivisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(NType), Convert.ToInt32(Code)).FirstOrDefault();
            return Json(new { IdCountryDivisions = IdCountryDivisions.CountryDivisionId }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RptUser(string IdCountryDivisions)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 252))
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                dt.EnforceConstraints = false;
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.rpt_UserSelectTableAdapter rpt_user = new Avarez.DataSet.DataSet1TableAdapters.rpt_UserSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                rpt_user.Fill(dt.rpt_UserSelect, "fldCountryDivisionsID", IdCountryDivisions, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\User.frx");
                Report.RegisterData(dt, "complicationsCarDBDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.Prepare();
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult CarSystem()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 253))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->سیستم های خودرو");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult CarSystemCom()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 355))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->سیستم های کامل خودرو");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Organization()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 356))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->سازمان ها");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Cost()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 357))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->هزینه ها");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult PlaqueType()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 358))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->انواع پلاک ها");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult PlaqueStatus()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 359))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->انواع وضعیت پلاک ها");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Paid()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 298))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->فیش های صادر شده و پرداخت شده");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult RptPaid(string SDate, string EDate,string User)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (User == "")
                User = "0";
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 298))
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptPeacockeryTableAdapter Peacockery = new Avarez.DataSet.DataSet1TableAdapters.sp_RptPeacockeryTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Peacockery.Fill(dt.sp_RptPeacockery, "Paid", Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate.ToString()), MyLib.Shamsi.Shamsi2miladiDateTime(EDate.ToString()), Convert.ToInt32(User));
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_Paid.frx");
                Report.RegisterData(dt, "CarTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.SetParameterValue("TitleGozaresh", "فیش های صادر شده و پرداخت شده");
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
        public ActionResult CollectionLog()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 325))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->تاریخچه پرداخت ها");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult RptCollectionLog(string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 325))
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_Collection_LogSelectTableAdapter CollectionLog = new DataSet.DataSet1TableAdapters.sp_Collection_LogSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                CollectionLog.Fill(dt.sp_Collection_LogSelect, SDate, EDate, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTabligh = new Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\CollectionLog.frx");
                Report.RegisterData(dt, "CarTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("MyTablighat", MyTabligh.Matn);
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
        public ActionResult NotPaid()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 299))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->فیش های صادر شده و پرداخت نشده");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult RptNotPaid(string SDate, string EDate, string User)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (User == "")
                User = "0";
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 299))
            {
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptPeacockeryTableAdapter Peacockery = new Avarez.DataSet.DataSet1TableAdapters.sp_RptPeacockeryTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                Peacockery.Fill(dt.sp_RptPeacockery, "NotPaid", Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate.ToString()), MyLib.Shamsi.Shamsi2miladiDateTime(EDate.ToString()),Convert.ToInt32(User));
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_Paid.frx");
                Report.RegisterData(dt, "CarTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.SetParameterValue("TitleGozaresh", "فیش های صادر شده و پرداخت نشده");
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

        public ActionResult CarFile()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 360))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->پرونده های خودرو");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return View();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult Owner()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 361))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->مالکان خودرو");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return View();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult CharacterPersianPlaque()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 362))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->کاراکترهای پلاک");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return View();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult CarPlaque()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 363))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->شماره پلاک ها");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return View();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult ComplicationsRate()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 364))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->نرخ عوارض سالیانه");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult rptComplicationsRate(int Sal)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_ComplicationsRateSelectTableAdapter complication = new Avarez.DataSet.DataSet1TableAdapters.sp_ComplicationsRateSelectTableAdapter();
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            complication.Fill(dt.sp_ComplicationsRateSelect, "fldYear", Sal.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

           FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\ComplicationsRate.frx");
            Report.RegisterData(dt, "complicationsCarDBDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("Year", Sal.ToString());
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
        public JsonResult _Tree(int? id)
        {
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            //پروسیجر تشخیص کاربر در تقسیمات کشوری برای
            //لود درخت همان ناحیه ای کاربر در سطح آن تعریف شده 
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (id != null)
            {
                var rols = (from k in p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0).Any()
                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var rols = (from k in p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult userCount()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 297))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->فیش وصولی به تفکیک کاربران");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult rptuserCount(int Sal)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountTableAdapter MonthlyChart = new Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


            MonthlyChart.Fill(dt.sp_RptMonthlyUser_Count, Convert.ToInt32(Sal), Convert.ToInt32(Session["UserMnu"]));
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMonthlyUser.frx");
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
        public ActionResult userCount_Date()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 297))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->فیش وصولی به تفکیک کاربران(بازه زمانی)");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult rptuserCount_Date(string startDate,string EndDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountWithDateTableAdapter MonthlyChart = new Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountWithDateTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


            MonthlyChart.Fill(dt.sp_RptMonthlyUser_CountWithDate, MyLib.Shamsi.Shamsi2miladiDateTime(startDate),
                MyLib.Shamsi.Shamsi2miladiDateTime(EndDate), Convert.ToInt32(Session["UserMnu"]));
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMonthlyUser_date.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("sal", "از تاریخ: " + startDate + " تا تاریخ:" + EndDate);
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
        public ActionResult rptuserCount_Date_notpay(string startDate, string EndDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountWithDate_NotPayTableAdapter MonthlyChart = new Avarez.DataSet.DataSet1TableAdapters.sp_RptMonthlyUser_CountWithDate_NotPayTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());


            MonthlyChart.Fill(dt.sp_RptMonthlyUser_CountWithDate_NotPay, MyLib.Shamsi.Shamsi2miladiDateTime(startDate),
                MyLib.Shamsi.Shamsi2miladiDateTime(EndDate), Convert.ToInt32(Session["UserMnu"]));
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMonthlyUser_date_notpay.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("sal", "از تاریخ: " + startDate + " تا تاریخ:" + EndDate);
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
        public ActionResult Collection()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 297))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->جدول وصول");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public JsonResult GetUsers()
        {

            Models.cartaxEntities car = new Models.cartaxEntities();
            var q = car.sp_SelectCountryDivTemp(5, Convert.ToInt32(Session["UserMnu"])).FirstOrDefault();
            return Json(car.sp_SelectDownTreeCountryDivisions(q.fldID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString())
                .Where(k=>k.fldNodeType==9).Select(c => new { fldID = c.fldSourceID, fldName = c.fldNodeName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        } 
        public JsonResult GetSettleType()
        {

            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_SettleTypeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult RptCollection(string SDate, string EDate, int ReportType, int treeid, string SettleTypeId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 297))
            {
                if (SettleTypeId == "")
                    SettleTypeId = 0.ToString();
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                dt.EnforceConstraints = false;
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_RptCollectionTableAdapter Collection = new Avarez.DataSet.DataSet1TableAdapters.sp_RptCollectionTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                Collection.Fill(dt.sp_RptCollection, Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), ReportType, treeid, Convert.ToInt32(SettleTypeId));
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rptCollection.frx");
                Report.RegisterData(dt, "CarTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.SetParameterValue("SettleTypeId", SettleTypeId);
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
        public ActionResult CollectionExcel(string SDate, string EDate, int ReportType, int treeid, string SettleTypeId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 297))
            {
                if (SettleTypeId == "")
                    SettleTypeId = 0.ToString();
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                
                GridView gv = new GridView();
                gv.DataSource = car.sp_RptCollection(Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate), ReportType, treeid, Convert.ToInt32(SettleTypeId));
                gv.DataBind();
                
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=پیش نمایش.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                return View();

            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult RptJoziyatAvarez(int carid)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->جزئیات محاسبات");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            Models.cartaxEntities m = new Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldCarId", carid.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_jCalcCarFileTableAdapter jCalcCarFile = new Avarez.DataSet.DataSet1TableAdapters.sp_jCalcCarFileTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter CarDetils = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter();

            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());

            Nullable<int> _Bed = new int();
            String _year = "";
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                Transaction Tr = new Transaction();
                var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["UserId"]));
                string msg1 = "";
                switch (Result)
                {
                    case Transaction.TransactionResult.Fail:
                        msg1 = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1
                        }, JsonRequestBehavior.AllowGet);

                    case Transaction.TransactionResult.NotSharj:
                        msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1
                        }, JsonRequestBehavior.AllowGet);

                }
            }
            jCalcCarFile.Fill(dt.sp_jCalcCarFile, (int)car.fldID, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null, null, DateTime.Now, Convert.ToInt32(Session["UserId"]), ref _year, ref _Bed);
            CarDetils.Fill(dt.sp_SelectCarDetils, carid);
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptJoziatAvarez.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("Parameter", _Bed.Value);
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
        public ActionResult RptMohasebat(int carCode, string fromYear, string toYear, string model, string Date, string CarMake
            , string CarAccountTypes, string CarCabin, string System, string Model, string Class, string ModelNum, string AzYear, string Tasal, string DateBime)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (Tasal == "تا سال...")
                Tasal = "";
            if (Tasal == "")
            {
               DateTime TaSal=  m.sp_GetDate().FirstOrDefault().CurrentDateTime;
               Tasal = MyLib.Shamsi.Miladi2ShamsiString(TaSal).Substring(0, 4);
            }
            if (toYear == "")
            {
                DateTime Sal = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
                toYear = MyLib.Shamsi.Miladi2ShamsiString(Sal).Substring(0, 4);
            }   
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";
            
            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_jCalcSingleBazeTableAdapter jCalcSingle = new Avarez.DataSet.DataSet1TableAdapters.sp_jCalcSingleBazeTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            String _year = "";
            jCalcSingle.Fill(dt.sp_jCalcSingleBaze, 6, carCode, 5, Convert.ToInt32(Session["UserMnu"]),
                MyLib.Shamsi.Shamsi2miladiDateTime(fromYear + "/01/01"),
                MyLib.Shamsi.Shamsi2miladiDateTime(date),
                MyLib.Shamsi.Shamsi2miladiDateTime(Date), DateTime.Now, Convert.ToInt32(model),ref _year);

            var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMohasebatSarAngoshti.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("OfficeName", Session["office"].ToString());
            Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            Report.SetParameterValue("TypeModel", CarMake);
            Report.SetParameterValue("NoeKhodro", CarAccountTypes);
            Report.SetParameterValue("NoeCabin", CarCabin);
            Report.SetParameterValue("SystemKhodro", System);
            Report.SetParameterValue("TipKhodro", Model);
            Report.SetParameterValue("ClassKhodro", Class);
            Report.SetParameterValue("Model", ModelNum);
            Report.SetParameterValue("TarikhBime", DateBime);
            Report.SetParameterValue("AzSal", AzYear);
            Report.SetParameterValue("TaSal", Tasal);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");

        }
        public ActionResult RptMohasebatKhareji(int carCode, string fromYear, string toYear, string model, string Date, string CarMake
            , string CarAccountTypes, string CarCabin, string System, string Model, string Class, string ModelNum, string AzYear, string Tasal, string DateBime)
        {
            
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (Tasal == "تا سال...")
                Tasal = "";
            if (Tasal == "")
            {
                DateTime TaSal = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
                Tasal = MyLib.Shamsi.Miladi2ShamsiString(TaSal).Substring(0, 4);
            }
            if (toYear == "")
            {
                DateTime Sal = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
                toYear = MyLib.Shamsi.Miladi2ShamsiString(Sal).Substring(0, 4);
            }   
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_jCalcSingleBazeTableAdapter jCalcSingle = new Avarez.DataSet.DataSet1TableAdapters.sp_jCalcSingleBazeTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            String _year = "";
            jCalcSingle.Fill(dt.sp_jCalcSingleBaze, 6, carCode, 5, Convert.ToInt32(Session["UserMnu"]),
                MyLib.Shamsi.Shamsi2miladiDateTime(fromYear + "/01/01"),
                MyLib.Shamsi.Shamsi2miladiDateTime(date),
                MyLib.Shamsi.Shamsi2miladiDateTime(Date), DateTime.Now, Convert.ToInt32(model), ref _year);

            var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMohasebatSarAngoshti.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", "");
            Report.SetParameterValue("OfficeName", "");
            Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            Report.SetParameterValue("TypeModel", CarMake);
            Report.SetParameterValue("NoeKhodro", CarAccountTypes);
            Report.SetParameterValue("NoeCabin", CarCabin);
            Report.SetParameterValue("SystemKhodro", System);
            Report.SetParameterValue("TipKhodro", Model);
            Report.SetParameterValue("ClassKhodro", Class);
            Report.SetParameterValue("Model", ModelNum);
            Report.SetParameterValue("TarikhBime", DateBime);
            Report.SetParameterValue("AzSal", AzYear);
            Report.SetParameterValue("TaSal", Tasal);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");

        }
    }
}
