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
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.Configuration;
using Aspose.Cells;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Configuration;

namespace Avarez.Areas.NewVer.Controllers.Users 
{
    public class TaidSabtename_NewController : Controller
    {
        //
        // GET: /NewVer/TaidSabtename_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "مدیریت کاربران-> تایید ثبت نام");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }


        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
           var q = m.Sp_RegisterSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldState == false).ToList();
           List<Avarez.Models.Sp_RegisterSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.Sp_RegisterSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldCodeDaftar":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodeDaftar";
                            break;
                        case "MunName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "MunName";
                            break;
                        case "fldModirDaftar":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldModirDaftar";
                            break;
                        case "fldmodirFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldmodirFamily";
                            break;
                        case "fldcodeMeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldcodeMeli";
                            break;
                        case "fldTel":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTel";
                            break;
                        case "fldAddress":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAddress";
                            break;

                    }
                    if (data != null)

                        data = m.Sp_RegisterSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.Sp_RegisterSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.Sp_RegisterSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.Sp_RegisterSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------
            return this.Store(rangeData, data.Count);
          
          //  return this.Store(q);
        }

        public ActionResult Save(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var Office = Car.Sp_RegisterSelect("fldId", id.ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            try
            {
                if (Office != null)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 391))
                    {
                        if (Office.fldState == false)
                        {
                            var u = Car.sp_UserSelect("fldId", Session["UserId"].ToString(), 0, "", 1, "").FirstOrDefault();

                            WebTransaction.TransactionWebService trn = new WebTransaction.TransactionWebService();
                            if (trn.Register(Office.fldCodeDaftar.ToString(), 8, u.fldDesc, Office.fldModirDaftar
                                , Office.fldmodirFamily, Office.fldcodeMeli, Office.fldTel, Office.fldAddress))
                            {
                                var o = Car.sp_OfficesSelect("fldName", "کد" + Office.fldCodeDaftar, 0, 1, "").FirstOrDefault();
                                if (o == null)
                                {
                                    Supporter.SendToSuporter s = new Supporter.SendToSuporter();

                                    s.insertOffice(Office.fldCodeDaftar.ToString(), Office.fldAddress, Office.fldMunId, Office.fldLocalId, Office.fldAreaId, Office.fldTel, "");
                                    if (WebConfigurationManager.AppSettings["IsBase"].ToString() == "false")
                                    {
                                        Car.sp_OfficesInsert("کد" + Office.fldCodeDaftar, Office.fldAddress, 1, Office.fldMunId,
                                            Office.fldLocalId, Office.fldAreaId, Convert.ToInt32(Session["UserId"]),
                                            "", Office.fldTel, Session["UserPass"].ToString(), Office.fldExpireDate);
                                    }
                                    var ofice = Car.sp_OfficesSelect("fldName", "کد" + Office.fldCodeDaftar, 0, 1, "").FirstOrDefault();
                                    System.Data.Entity.Core.Objects.ObjectParameter user_id = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(Int64));

                                    Car.sp_UserInsert(user_id, Office.fldModirDaftar, Office.fldmodirFamily, true, Office.fldCodeDaftar.ToString().GetHashCode().ToString(), Office.fldCodeDaftar.ToString(),
                                        Office.fldcodeMeli, "-", "0", Office.fldTel, Office.fldMobile, DateTime.Now, 8, (int)ofice.fldID, Convert.ToInt32(Session["UserId"]), "", null, u.fldOfficeUserKey, "",false);

                                    Car.sp_User_GroupInsert(2, Convert.ToInt64(user_id.Value), Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());

                                    Car.sp_TransactionInfInsert(Office.fldCodeDaftar.ToString(), 8, (int)ofice.fldID, CodeDecode.stringcode(Office.fldCodeDaftar.ToString()), Convert.ToInt32(Session["UserId"]), "", false);

                                    Car.Sp_RegisterUpdate(Office.fldId, true);

                                    return Json(new { Msg = "تایید ثبت نام با موفقیت انجام شد.", MsgTitle = "تایید موفق", Er = 0 });
                                }
                                else
                                {
                                    Car.Sp_RegisterUpdate(Office.fldId, true);
                                    return Json(new { Msg = "دفتر انتخاب شده قبلا در سامانه ثبت شده است", MsgTitle = "ثبت نام شده  ", Er = 1 });

                                }
                            }
                            else
                            {
                                return Json(new { Msg = "شما مجاز به تایید نمی باشید.", MsgTitle = "عدم دسترسی ", Er = 1 });
                            }
                        }
                        else
                        {
                            return Json(new { Msg = "شما مجاز به تایید نمی باشید.", MsgTitle = "عدم دسترسی ", Er = 1 });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        });
                    }
                }
                else
                    return null;
            }
            catch (Exception x)
            {
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult GeneratePDFReport()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");

            try
            {
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
        public ActionResult PrintReport(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }


    }
}
