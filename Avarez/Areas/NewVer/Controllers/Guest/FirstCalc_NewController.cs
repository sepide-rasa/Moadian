using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using System.Configuration;

namespace Avarez.Areas.NewVer.Controllers.Guest
{
    public class FirstCalc_NewController : Controller
    {
        //
        // GET: /NewVer/FirstCalc_New/

        public ActionResult Index()
        {//باز شدن پرونده جدید
            if (Session["UserState"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public JsonResult GetCascadeMake()
        {
            List<SelectListItem> No = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "شمسی";
            item.Value = "1";
            No.Add(item);
            item = new SelectListItem();
            item.Text = "میلادی";
            item.Value = "2";
            No.Add(item);
            return Json(No.Select(c => new { fldID = c.Value, fldName = c.Text }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeAccount(int cboCarMake)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarAccountTypeSelect("fldCarMakeID", cboCarMake.ToString(), 0, 1, "");
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeCabin(int cboCarAccountTypes)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CabinTypeSelect("fldCarAccountTypeID", cboCarAccountTypes.ToString(), 0, 1, "");
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeSystem(int cboCarCabin)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarSystemSelect("fldCabinTypeID", cboCarCabin.ToString(), 0, 1, "");
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeModel(int cboSystem)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarModelSelect("fldCarSystemID", cboSystem.ToString(), 0, 1, "");
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeClass(int cboModel)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarClassSelect("fldCarModelID", cboModel.ToString(), 0, 1, "");
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModel(int Noo)
        {
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Noo == 1)
            {
                for (int i = 1350; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = 1950; i <= DateTime.Now.Year + 1; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.OrderByDescending(l => l.Text).Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFromYear(int Noo, int value)
        {
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Noo == 1)
            {
                for (int i = value; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(value.ToString() + "/01/01")).Substring(0, 4)); i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetToYear(int Noo, int value)
        {
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Noo == 1)
            {
                for (int i = value; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = value; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillDateText(string year)
        {
            if (Convert.ToInt32(year) < 1900)
                return Json(new { date = year + "/01/01" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { date = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(year + "/01/01")) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Calc(int carCode, string fromYear, string toYear, string model, string Date)
        {
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";
            Models.cartaxEntities m = new Models.cartaxEntities();
            //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            //var q = m.sp_jCalcSingleBaze(6, carCode, 5, Convert.ToInt32(Session["UserMnu"]),
            //    MyLib.Shamsi.Shamsi2miladiDateTime(fromYear + "/01/01"),
            //    MyLib.Shamsi.Shamsi2miladiDateTime(date),
            //    MyLib.Shamsi.Shamsi2miladiDateTime(Date), DateTime.Now, Convert.ToInt32(model), _year).OrderBy(h => h.fldyear).ToList();
            string _year = "";
            var q = m.prs_newCarCalc(DateTime.Now, Convert.ToInt32(Session["CountryType"]),
                Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["UserId"]), Convert.ToInt32(fromYear),
                Convert.ToInt32(toYear), Convert.ToInt32(model), MyLib.Shamsi.Shamsi2miladiDateTime(Date), carCode).ToList();
            var years = q.Where(k => k.fldPrice == null).ToList();
            foreach (var item in years)
            {
                _year += item.fldYear;
            } 
            if (_year.ToString() == "")
                return Json(new { data = q, flag = 0 }, JsonRequestBehavior.AllowGet);
            else
            {
                string s = "", msg = "";
                for (int i = 0; i < _year.ToString().Length; i += 4)
                {
                    if (i < _year.ToString().Length - 4)
                        s += _year.ToString().Substring(i, 4) + " و ";
                    else
                        s += _year.ToString().Substring(i, 4);
                }
                // msg = "تعرفه سالهای " + s + " تعریف نشده است لطفا به مدیر سیستم گزارش دهید.";
                msg = "تعرفه سالهای " + s + " تعریف نشده است لطفا جهت اعلام به پشتیبان دکمه ارسال به پشتیبان را انتخاب و تا زمانی که نرخ توسط پشتیبان ثبت شود، منتظر بمانید، سپس دکمه دریافت از سرور را انتخاب کنید و پس از دریافت پیغام تایید، دکمه محاسبه مجدد را از قسمت صورت حساب انتخاب کنید.";
                object q1 = null;
                return Json(new { data = q1, flag = 1, msg = msg, Year = s.Replace(" و ", ",") }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintMohasebat(int carCode, string fromYear, string toYear, string model, string Date, string CarMake
            , string CarAccountTypes, string CarCabin, string System, string Model1, string Class, string ModelNum, string AzYear, string Tasal, string DateBime)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.carCode = carCode;
            PartialView.ViewBag.fromYear = fromYear;
            PartialView.ViewBag.toYear = toYear;
            PartialView.ViewBag.model = model;
            PartialView.ViewBag.Date = Date;
            PartialView.ViewBag.CarMake = CarMake;
            PartialView.ViewBag.CarAccountTypes = CarAccountTypes;
            PartialView.ViewBag.CarCabin = CarCabin;
            PartialView.ViewBag.System = System;
            PartialView.ViewBag.Model1 = Model1;
            PartialView.ViewBag.Class = Class;
            PartialView.ViewBag.ModelNum = ModelNum;
            PartialView.ViewBag.AzYear = AzYear;
            PartialView.ViewBag.Tasal = Tasal;
            PartialView.ViewBag.DateBime = DateBime;
            return PartialView;
        }
        public ActionResult GeneratePDFMohasebat(int carCode, string fromYear, string toYear, string model, string Date, string CarMake
            , string CarAccountTypes, string CarCabin, string System, string Model1, string Class, string ModelNum, string AzYear, string Tasal, string DateBime)
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
            Avarez.DataSet.DataSet1TableAdapters.prs_newCarCalcTableAdapter jCalcSingle = new Avarez.DataSet.DataSet1TableAdapters.prs_newCarCalcTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, Session["UserPass"].ToString());
            String _year = "";
            //jCalcSingle.Fill(dt.sp_jCalcSingleBaze, 6,carCode, 5, Convert.ToInt32(Session["UserMnu"]),
            //    MyLib.Shamsi.Shamsi2miladiDateTime(fromYear + "/01/01"),
            //    MyLib.Shamsi.Shamsi2miladiDateTime(date),
            //    MyLib.Shamsi.Shamsi2miladiDateTime(Date), DateTime.Now, Convert.ToInt32(model), ref _year);
            jCalcSingle.Fill(dt.prs_newCarCalc, DateTime.Now, Convert.ToInt32(Session["CountryType"]),
                Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["UserId"]), Convert.ToInt32(fromYear),
                Convert.ToInt32(toYear), Convert.ToInt32(model), MyLib.Shamsi.Shamsi2miladiDateTime(DateBime), carCode);

            var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMohasebatSarAngoshti1.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", "");
            Report.SetParameterValue("OfficeName", "");
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
            Report.SetParameterValue("TypeModel", CarMake);
            Report.SetParameterValue("NoeKhodro", CarAccountTypes);
            Report.SetParameterValue("NoeCabin", CarCabin);
            Report.SetParameterValue("SystemKhodro", System);
            Report.SetParameterValue("TipKhodro", Model1);
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
