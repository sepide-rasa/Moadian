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
using Aspose.Cells;
using System.Configuration;

namespace Avarez.Areas.NewVer.Controllers.Config
{
    public class SendSmsController : Controller
    {
        //
        // GET: /NewVer/SendSms/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "پیکربندی سیستم->ارسال پیامک");
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
        public ActionResult Excel(string Checked)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
               
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", 
                                     "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
                int index = 0;
                var Check = "";
                var fldCarFileID = ""; var fldName = ""; var fldMelli_EconomicCode = ""; var fldMotorNumber = ""; var fldShasiNumber = "";
                var fldMobile = ""; var fldPlaqueNumber = ""; var fldModel = ""; var fldClassName = ""; var fldSystemName = "";var fldMablagh = "0";
                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                var StatusCheck = Checked.Split(';');
                Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
                var List = car.sp_tblBedehiSelect("", "", 0);
                foreach (var item in StatusCheck)
                {
                    var coll = car.sp_tblBedehiSelect("", "", 0).ToList();
                    switch (item)
                    {
                        case "fldCarFileID":
                            Check = "شماره پرونده";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int i = 0;
                            foreach (var _item in coll)
                            {
                                fldCarFileID = _item.fldCarFileID.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                                Cell.PutValue(fldCarFileID);
                                i++;
                            }
                            index++;
                            break;
                        case "fldName":
                            Check = "نام مالک";
                            Cell cell1 = sheet.Cells[alpha[index] + "1"];
                            cell1.PutValue(Check);
                            int j = 0;
                            foreach (var _item in coll)
                            {
                                fldName = _item.fldName;
                                Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                                Cell.PutValue(fldName);
                                j++;
                            }
                            index++;
                            break;
                        case "fldMelli_EconomicCode":
                            Check = "کدملی";
                            Cell cell2 = sheet.Cells[alpha[index] + "1"];
                            cell2.PutValue(Check);
                            int k = 0;
                            foreach (var _item in coll)
                            {
                                fldMelli_EconomicCode = _item.fldMelli_EconomicCode;
                                Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                                Cell.PutValue(fldMelli_EconomicCode);
                                k++;
                            }
                            index++;
                            break;
                        case "fldMotorNumber":
                            Check = "شماره موتور";
                            Cell cell3 = sheet.Cells[alpha[index] + "1"];
                            cell3.PutValue(Check);
                            int q = 0;
                            foreach (var _item in coll)
                            {
                                fldMotorNumber = _item.fldMotorNumber;
                                Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                                Cell.PutValue(fldMotorNumber);
                                q++;
                            }
                            index++;
                            break;
                        case "fldShasiNumber":
                            Check = "شماره شاسی";
                            Cell cell4 = sheet.Cells[alpha[index] + "1"];
                            cell4.PutValue(Check);
                            int w = 0;
                            foreach (var _item in coll)
                            {
                                fldShasiNumber = _item.fldShasiNumber;
                                Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                                Cell.PutValue(fldShasiNumber);
                                w++;
                            }
                            index++;
                            break;
                        case "fldMobile":
                            Check = "موبایل";
                            Cell cell5 = sheet.Cells[alpha[index] + "1"];
                            cell5.PutValue(Check);
                            int a = 0;
                            foreach (var _item in coll)
                            {
                                fldMobile = _item.fldMobile;
                                Cell Cell = sheet.Cells[alpha[index] + (a + 2)];
                                Cell.PutValue(fldMobile);
                                a++;
                            }
                            index++;
                            break;
                        case "fldPlaqueNumber":
                            Check = "پلاک";
                            Cell cell6 = sheet.Cells[alpha[index] + "1"];
                            cell6.PutValue(Check);
                            int b = 0;
                            foreach (var _item in coll)
                            {
                                fldPlaqueNumber = _item.fldPlaqueNumber;
                                Cell Cell = sheet.Cells[alpha[index] + (b + 2)];
                                Cell.PutValue(fldPlaqueNumber);
                                b++;
                            }
                            index++;
                            break;
                        case "fldModelName":
                            Check = "مدل خودرو";
                            Cell cell7 = sheet.Cells[alpha[index] + "1"];
                            cell7.PutValue(Check);
                            int c = 0;
                            foreach (var _item in coll)
                            {
                                fldModel = _item.fldModelName;
                                Cell Cell = sheet.Cells[alpha[index] + (c + 2)];
                                Cell.PutValue(fldModel);
                                c++;
                            }
                            index++;
                            break;
                        case "fldClassName":
                            Check = "کلاس خودرو";
                            Cell cell8 = sheet.Cells[alpha[index] + "1"];
                            cell8.PutValue(Check);
                            int d = 0;
                            foreach (var _item in coll)
                            {
                                fldClassName = _item.fldClassName;
                                Cell Cell = sheet.Cells[alpha[index] + (d + 2)];
                                Cell.PutValue(fldClassName);
                                d++;
                            }
                            index++;
                            break;
                        case "fldSystemName":
                            Check = "سیستم خودرو";
                            Cell cell9 = sheet.Cells[alpha[index] + "1"];
                            cell9.PutValue(Check);
                            int e = 0;
                            foreach (var _item in coll)
                            {
                                fldSystemName = _item.fldSystemName;
                                Cell Cell = sheet.Cells[alpha[index] + (e + 2)];
                                Cell.PutValue(fldSystemName);
                                e++;
                            }
                            index++;
                            break;
                        case "fldMablagh":
                            Check = "مبلغ";
                            Cell cell10 = sheet.Cells[alpha[index] + "1"];
                            cell10.PutValue(Check);
                            int f = 0;
                            foreach (var _item in coll)
                            {
                                fldMablagh = _item.fldMablagh.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (f + 2)];
                                Cell.PutValue(fldMablagh);
                                f++;
                            }
                            index++;
                            break;
                            
                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "xls", "Bedehkaran.xls");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Print(string containerId)
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
            return result;
        }
        public ActionResult GeneratePDF()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                dt.EnforceConstraints = false;
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_tblBedehiSelectTableAdapter rpt_Alluser = new Avarez.DataSet.DataSet1TableAdapters.sp_tblBedehiSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                rpt_Alluser.Fill(dt.sp_tblBedehiSelect, "", "", 0);

                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Bedehkaran.frx");
                Report.RegisterData(dt, "dataSet1");
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
                Report.Prepare();
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult reload()
        {
            
                Models.cartaxEntities m = new Models.cartaxEntities();
                var t = m.sp_TableTreeSelect("TypeAndCode", "", 0, 5, Convert.ToInt32(Session["UserMnu"])).FirstOrDefault();
                var q = m.sp_getBedehkaranList(5, Convert.ToInt32(Session["UserMnu"]), t.fldID).ToList();
                foreach (var item1 in q)
                {
                    var file = m.sp_CarFileSelect("fldid", item1.fldCarFileID.ToString(), 0, 1, "").FirstOrDefault();

                    var DateTime = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
                    var car = m.sp_CarFileSelect("fldCarId", file.fldCarID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime).Substring(0, 4));
                    string date = toYear + "/12/29";
                    if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                        date = toYear + "/12/30";

                    //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                    //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));

                    //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                    //    null, DateTime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
                    var bedehi = m.prs_newCarFileCalc(DateTime.Now, Convert.ToInt32(Session["CountryType"]),
                Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"])).Where(k => k.fldCollectionId == 0).ToList();
                    string _year = "";
                    if (bedehi != null)
                    {
                        var nullYears = bedehi.Where(k => k.fldPrice == null).ToList();
                        foreach (var item in nullYears)
                        {
                            _year += item.fldYear;
                        }
                    }
                    if (_year.ToString() == "")
                    {
                        int? mablagh = 0;
                        int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0, fldMainDiscount = 0, fldFineDiscount = 0,
                            fldValueAddDiscount = 0, fldOtherDiscount = 0;
                        ArrayList Years = new ArrayList();
                        DataSet.DataSet1.sp_jCalcCarFileDataTable a = new DataSet.DataSet1.sp_jCalcCarFileDataTable();
                        foreach (var item in bedehi)
                        {
                            int? jam = (item.fldFinalPrice + item.fldMashmol + item.fldNoMashmol + item.fldMablaghJarime + item.fldOtherPrice) -
                         (item.fldDiscontJarimePrice + item.fldDiscontMoaserPrice + item.fldDiscontOtherPrice + item.fldDiscontValueAddPrice);
                            mablagh += jam;
                            fldFine += (int)item.fldMablaghJarime;
                            fldValueAddPrice += (int)item.fldValueAdded;
                            fldPrice += (int)((item.fldFinalPrice - item.fldValueAdded) + item.fldMashmol + item.fldNoMashmol);
                            Years.Add(item.fldYear);
                            fldOtherPrice += (int)item.fldOtherPrice;
                            fldMainDiscount += (int)item.fldDiscontMoaserPrice;
                            fldFineDiscount += (int)item.fldDiscontJarimePrice;
                            fldValueAddDiscount += (int)item.fldDiscontValueAddPrice;
                            fldOtherDiscount += (int)item.fldDiscontOtherPrice;
                            a.Addsp_jCalcCarFileRow((int)item.fldYear, (int)item.fldPrice, (int)item.fldMablaghMoaser, (int)item.fldValueAdded, (int)item.fldFinalPrice,
                                (int)item.fldMablaghJarime, (int)item.fldTedadJarime, (int)item.fldDiscontMoaserPrice, (int)jam, item.fldCalcDate, (int)item.fldOtherPrice, (int)item.fldDiscontValueAddPrice,
                                (int)item.fldDiscontJarimePrice, (int)item.fldDiscontOtherPrice);
                        }
                        int sal = 0, mah = 0;
                        //mablagh += Convert.ToInt32(_Bed.Value);
                        //fldPrice += Convert.ToInt32(_Bed.Value);
                        if (mablagh > 10000)
                        {
                            m.sp_tblBedehiInsert(mablagh, item1.fldCarFileID, item1.fldName, item1.fldMotorNumber, item1.fldMobile,
                                item1.fldPlaqueNumber, item1.fldModelName, item1.fldClassName, item1.fldSystemName, item1.fldMelli_EconomicCode,
                                item1.fldShasiNumber, item1.fldCarID, null, null);
                        }
                    }
                }
                return Json(new { Er = 0, MsgTitle = "بارگذاری موفق", Msg = "بارگذاری با موفقیت انجام شد." });
            
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_tblBedehiSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_tblBedehiSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldMotorNumber":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldMotorNumber";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldMobile":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMobile";
                            break;
                        case "fldMablagh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablagh";
                            break;
                        case "fldCarFileID":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCarFileID";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_tblBedehiSelect(field, searchtext, 100).ToList();
                    else
                        data = m.sp_tblBedehiSelect(field, searchtext, 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_tblBedehiSelect("", "", 100).ToList();
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
                            //return !oValue.ToString().Contains(value.ToString());
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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

            List<Avarez.Models.sp_tblBedehiSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public bool CheckMobileNumber(string MobileNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(MobileNumber, "(^(09|9)[0-9][0-9]\\d{7}$)");
        }
        public ActionResult Send()
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            SmsPanel.RasaSMSPanel_Send Sms = new SmsPanel.RasaSMSPanel_Send();
            try
            {
                Sms.Timeout = 500000000;
                var haveSmsPanel = p.sp_SMSSettingSelect("", "", 1,1,"").FirstOrDefault();
                var q = p.sp_tblBedehiSelect("", "", 0).ToList();

                foreach (var item in q)
                {
                    var text = "مالک محترم خودرو،لطفا نسبت به پرداخت عوارض نقلیه سالیانه از طریق دفاتر پیشخوان دولت اقدام نمایید. در صورت عدم پرداخت،علاوه بر عوارض،سالیانه 24درصد جریمه دیرکرد به خودرو شما تعلق می‌گیرد."+Environment.NewLine+"شهرداری زنجان";
                    
                    if (CheckMobileNumber(item.fldMobile))
                    {
                        var k = Sms.SendMessage(haveSmsPanel.fldUserName, haveSmsPanel.fldPassword,
                            new string[] { item.fldMobile }, text, 1, haveSmsPanel.fldLineNumber);
                    }
                }
                return Json(new { Msg = "ارسال پیامک با موفقیت انجام شد.", Er = 0,MsgTitle="ارسال موفق" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception x)
            {
                var Msg=x.Message;
                if(x.InnerException !=null){
                    Msg=x.InnerException.Message;
                }
                return Json(new { Msg =Msg, Er = 1,MsgTitle="خطا" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
