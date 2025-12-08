using Avarez.Controllers.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.CarTax
{
    public class Mafasa_TarikhController : Controller
    {
        //
        // GET: /Mafasa_Tarikh/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 371))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->مشاهده مفاصا در بازه زمانی");
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
        public ActionResult RptMafasa(string SDate, string EDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_Car_MafasaTableAdapter Car_Mafasa = new Avarez.DataSet.DataSet1TableAdapters.sp_Car_MafasaTableAdapter();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");


            Car_Mafasa.Fill(dt.sp_Car_Mafasa, MyLib.Shamsi.Shamsi2miladiDateTime(SDate), MyLib.Shamsi.Shamsi2miladiDateTime(EDate),Convert.ToInt32(Session["UserMnu"]));
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Mafasa_Tarikh.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("OfficeName", Session["office"].ToString());
            Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            Report.SetParameterValue("UserName", car.sp_UserSelect("fldid", Session["UserId"].ToString(), 0, ""
                , Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldUserName);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");

        }
    }
}
