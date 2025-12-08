using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Web.Configuration;
using System.IO;
namespace Avarez.Controllers.Users
{
    public class TaidSabtenameController : Controller
    {
        //
        // GET: /TaidSabtename/

        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.Sp_RegisterSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldState == false).ToList().ToDataSourceResult(request);
            return Json(q);
        }      
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldId", "fldCodeDaftar", "MunName", "fldmodirFamily" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.Sp_RegisterSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Reload(string field, string value, int top, int searchtype)
        //{//جستجو
        //    Models.cartaxEntities m = new Models.cartaxEntities();
        //    var q = m.Sp_RegisterSelect().Where(k => k.fldState == false).ToList();
        //    return Json(q, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Save(int id)
        {
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var Office = Car.Sp_RegisterSelect("fldId", id.ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (Office != null)
            {//ثبت رکورد جدید
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 49))
                {
                    if (Office.fldState == false)
                    {
                        var u = Car.sp_UserSelect("fldId", Session["UserId"].ToString(), 0, "", 1, "").FirstOrDefault();

                        WebTransaction.TransactionWebService trn = new WebTransaction.TransactionWebService();
                        if (trn.Register(Office.fldCodeDaftar.ToString(), 8, u.fldDesc,Office.fldModirDaftar
                            ,Office.fldmodirFamily,Office.fldcodeMeli,Office.fldTel,Office.fldAddress))
                        {
                            var o = Car.sp_OfficesSelect("fldName", "کد" + Office.fldCodeDaftar, 0, 1, "").FirstOrDefault();
                            if (o == null)
                            {
                                Supporter.SendToSuporter s = new Supporter.SendToSuporter();

                                s.insertOffice(Office.fldCodeDaftar.ToString(), Office.fldAddress, Office.fldMunId, Office.fldLocalId, Office.fldAreaId, Office.fldTel,"");
                                if (WebConfigurationManager.AppSettings["IsBase"].ToString() == "false")
                                {
                                    Car.sp_OfficesInsert("کد" + Office.fldCodeDaftar, Office.fldAddress, 1, Office.fldMunId,
                                        Office.fldLocalId, Office.fldAreaId, Convert.ToInt32(Session["UserId"]), ""
                                        , Office.fldTel, Session["UserPass"].ToString(), Office.fldExpireDate);
                                }
                                var ofice = Car.sp_OfficesSelect("fldName", "کد" + Office.fldCodeDaftar, 0, 1, "").FirstOrDefault();
                                System.Data.Entity.Core.Objects.ObjectParameter user_id = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(Int64));

                                Car.sp_UserInsert(user_id,Office.fldModirDaftar,Office.fldmodirFamily, true, Office.fldCodeDaftar.ToString().GetHashCode().ToString(), Office.fldCodeDaftar.ToString(),
                                    Office.fldcodeMeli, "-", "0", Office.fldTel, "0", DateTime.Now, 8, (int)ofice.fldID, Convert.ToInt32(Session["UserId"]), "", null,u.fldOfficeUserKey, "",false);

                                Car.sp_User_GroupInsert(2, Convert.ToInt64(user_id.Value), Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());

                                Car.sp_TransactionInfInsert(Office.fldCodeDaftar.ToString(), 8, (int)ofice.fldID, CodeDecode.stringcode(Office.fldCodeDaftar.ToString()), Convert.ToInt32(Session["UserId"]), "", false);
                                
                                Car.Sp_RegisterUpdate(Office.fldId, true);
                                return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                            }
                            else
                            {
                                Car.Sp_RegisterUpdate(Office.fldId, true);
                                return Json(new { data = "دفتر انتخاب شده قبلا در سامانه ثبت شده است", state = 1 });
                            }
                        }
                        else
                        {
                            return Json(new { data = "شما مجاز به تایید نمی باشید.", state = 1 });
                        }
                    }
                    else
                    {
                        return Json(new { data = "شما مجاز به تایید نمی باشید.", state = 1 });
                    }
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
                }
            }
            else
                return null;
        }
        //public ActionResult PrintReport()
        //{
        //    if (Session["UserId"] == null)
        //        return RedirectToAction("logon", "Account");
        //    //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 251))
        //    //{
        //    Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "افراد ثبت شده");
        //    SignalrHub hub = new SignalrHub();
        //    hub.ReloadOnlineUser();
        //    return PartialView();
        //    //}
        //    //else
        //    //{
        //    //    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
        //    //    return RedirectToAction("error", "Metro");
        //    //}
        //}
        public ActionResult PrintReport()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 325))
            //{
            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.Sp_RegisterSelectTableAdapter sp_Register = new Avarez.DataSet.DataSet1TableAdapters.Sp_RegisterSelectTableAdapter();
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            sp_Register.Fill(dt.Sp_RegisterSelect, "", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Rpt_Register.frx");
            Report.RegisterData(dt, "complicationsCarDBDataSet1");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            //}
            //else
            //{
            //    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
            //    return RedirectToAction("error", "Metro");
            //}
        }
    }
}
