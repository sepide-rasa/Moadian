using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections;
using System.IO;
using System.Web.Configuration;

namespace Avarez.Controllers.Guest
{
    public class FactureController : Controller
    {
        //
        // GET: /Facture/

        public ActionResult Index(int id)
        {
            if (Session["UserState"] == null)
                return RedirectToAction("logon", "Account");

            Session["fldCarID"] = id;
            Session["fldCarID1"] = id;
            Session["fldCarID2"] = id;
            Session["fldCarID3"] = id;
            return PartialView();
        }

        public ActionResult Receipt(int id)
        {
            Session["ResidId"] = id;
            return View();
        }

        public ActionResult Savabegh([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_CarExperienceSelect("fldCarID", Session["fldCarID"].ToString(), 30, Convert.ToInt32(Session["GeustId"]), "").ToList().ToDataSourceResult(request);
            Session.Remove("fldCarID");
            return Json(q);
        }
        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_PictureSelect("fldBankPic", id.ToString(), 30, 0,"").FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldPic != null)
                {
                    return File((byte[])pic.fldPic, "jpg");
                }
            }
            return null;

        }
        public ActionResult Mafasa(int id)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_SelectCarDetils(id).FirstOrDefault();
            if (car != null)
            {
                var Cdate = p.sp_GetDate().FirstOrDefault();
                int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Cdate.CurrentDateTime).Substring(0, 4));
                string date = toYear + "/12/29";
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                    date = toYear + "/12/30";
                System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                
                if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
                {
                    Transaction Tr = new Transaction();
                    var Div = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    var TransactionInf = p.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                    var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["GeustId"]));
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
                var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                    null, DateTime.Now, Convert.ToInt32(Session["GeustId"]), _year, _Bed).ToList();
                double mablagh = 0;
                foreach (var item in bedehi)
                {
                    mablagh += (int)item.fldDept;
                }
                if (mablagh + Convert.ToInt32(_Bed.Value) <= 10000)
                {
                    Session["CarFileId"] = car.fldID;
                    Session["Sal"] = toYear.ToString().Substring(0, 4);
                    Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                    Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter Receipt = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
                    sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                    Receipt.Fill(dt.rpt_Receipt, Convert.ToInt32(car.fldCarID), 2);
                    Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter exp = new Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter();
                    fish.Fill(dt.rpt_RecoupmentAccount, Convert.ToInt32(Session["CarFileId"]),DateTime.Now);
                    exp.Fill(dt.sp_CarExperienceSelect, "fldCarFileID", Session["CarFileId"].ToString(), 0, Convert.ToInt32(Session["UserMnu"].ToString()), "");
                    Avarez.Models.cartaxEntities Car = new Avarez.Models.cartaxEntities();
                    Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                    var mnu = Car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    var State = Car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();

                    System.Data.Entity.Core.Objects.ObjectParameter mafasaId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterDate = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterDate", typeof(DateTime));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterNum = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterNum", typeof(string));

                    p.Sp_MafasaInsert(mafasaId, car.fldCarID, Convert.ToInt32(Session["UserMnu"]), null, Convert.ToInt32(Session["GeustId"]), LetterDate, LetterNum);
                    string barcode = WebConfigurationManager.AppSettings["SiteURL"] + "/QR_Mafasa/Get/" + mafasaId.Value;

                    FastReport.Report Report = new FastReport.Report();
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_Mafasa.frx");
                    Report.RegisterData(dt, "complicationsCarDBDataSet");
                    Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(LetterDate.Value)));
                    var time = Convert.ToDateTime(LetterDate.Value);
                    Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                    Report.SetParameterValue("Num", LetterNum.Value);
                    Report.SetParameterValue("barcode", barcode);
                    Report.SetParameterValue("MunicipalityName", mnu.fldName);
                    Report.SetParameterValue("StateName", State.fldName);
                    Report.SetParameterValue("AreaName","" );//Session["area"].ToString()
                    Report.SetParameterValue("OfficeName", "");//Session["office"].ToString()
                    Report.SetParameterValue("sal", Session["Sal"]);
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                    MemoryStream stream = new MemoryStream();
                    Report.Prepare();
                    Report.Export(pdf, stream);
                    p.Sp_MafasaUpdate(mafasaId.Value.ToString(), stream.ToArray());


                    return File(stream.ToArray(), "application/pdf");
                }
                else
                    return Json("کاربر گرامی شما در حال انجام عملیات غیر قانونی می باشید و در صورت تکرار آی پی شما به پلیس فتا اعلام خواهد شد. باتشکر", JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }

        public ActionResult Variziha([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.rpt_Receipt(Convert.ToInt32(Session["fldCarID2"]), 2).ToList().ToDataSourceResult(request);
            Session.Remove("fldCarID2");
            return Json(q);
        }
        public ActionResult FishReport(int id)
        {            
                Models.cartaxEntities p = new Models.cartaxEntities();
                var car = p.sp_SelectCarDetils(id).FirstOrDefault();
                if (car != null)
                {
                    var ServerDate=p.sp_GetDate().FirstOrDefault();
                    int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime).Substring(0, 4));
                    //string date = toYear + "/12/29";
                    //if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                    //    date = toYear + "/12/30";
                    System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                    //var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                    //null, ServerDate.CurrentDateTime, _year, _Bed).ToList();
                    int sal=0, mah=0;
                    double mablagh = 0;
                    int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0,
                        fldMainDiscount = 0, fldFineDiscount = 0, fldValueAddDiscount = 0, fldOtherDiscount = 0;
                    mablagh = Convert.ToInt32(Session["mablagh"]);
                    fldFine = Convert.ToInt32(Session["Fine"]);
                    fldValueAddPrice=Convert.ToInt32(Session["ValueAddPrice"]);
                    fldPrice = Convert.ToInt32(Session["Price"]);
                    fldOtherPrice=Convert.ToInt32( Session["OtherPrice"]);
                    fldMainDiscount =  Convert.ToInt32(Session["fldMainDiscount"]);
                    fldFineDiscount =  Convert.ToInt32(Session["fldFineDiscount"]);
                    fldValueAddDiscount =  Convert.ToInt32(Session["fldValueAddDiscount"]);
                    fldOtherDiscount =  Convert.ToInt32(Session["fldOtherDiscount"]);
                    //ArrayList Years = new ArrayList();
                    //foreach (var item in bedehi)
                    //{
                    //    fldFine += (int)item.fldFine;
                    //    fldValueAddPrice += (int)item.fldValueAdded;
                    //    fldPrice += (int)item.fldCurectPrice;
                    //    //fldOtherPrice += (int)item.fldOtherPrice;
                    //    mablagh += (int)item.fldDept;
                    //    Years.Add(item.fldyear);
                    //}
                    ArrayList Years = (ArrayList)Session["Year"];
                    int[] AvarezSal = new int[Years.Count];
                    for (int i = 0; i < Years.Count; i++)
                    {
                        AvarezSal[i] = (int)Years[i];
                    }
                    mablagh += Convert.ToInt32(_Bed.Value);
                    fldPrice += Convert.ToInt32(_Bed.Value);
                    if (mablagh < 10000)
                    {
                        Session["ER"] = "پرونده انتخابی بدهکار نیست.";
                        return RedirectToAction("error", "Metro");
                    }
                    
                    var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    //p.sp_selectAccountBank(Convert.ToInt32(Session["UserMnu"]), true).FirstOrDefault();
                    
                    System.Data.Entity.Core.Objects.ObjectParameter _CountryID = new System.Data.Entity.Core.Objects.ObjectParameter("ID", typeof(long));

                    var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    string MohlatDate = "";
                    int roundNumber = 0;
                    if (SubSetting != null)
                    {
                        if (SubSetting.fldLastRespitePayment > 0)
                        {
                            MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                        }
                        else if (SubSetting.fldLastRespitePayment == 0)
                        {
                            string Cdate=MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                            int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                            int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                            int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                            if (Years.Count > 1)
                            {
                                if (Mounth <= 6)
                                    MohlatDate = Year + "/" + Mounth + "/31";
                                else if (Mounth > 6 && Mounth < 12)
                                    MohlatDate = Year + "/" + Mounth + "/30";
                                else if (MyLib.Shamsi.Iskabise(Year) == true)
                                    MohlatDate = Year + "/" + Mounth + "/30";
                                else
                                    MohlatDate = Year + "/" + Mounth + "/29";
                            }
                            else if (Years.Count == 1)
                            {
                                if (MyLib.Shamsi.Iskabise(Year) == true)
                                    MohlatDate = Year + "/" + 12 + "/30";
                                else
                                    MohlatDate = Year + "/" + 12 + "/29";
                            }
                            //if
                        }
                        var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                        roundNumber = Round.fldRound;
                    }

                    double Rounded = 10;
                    switch (roundNumber)
                    {
                        case 3:
                            Rounded = 1000;
                            break;
                        case 2:
                            Rounded = 100;
                            break;
                    }


                    mablagh = Math.Ceiling(mablagh / Rounded) * Rounded;//گرد به بالا
                    
                    string ShGhabz = "", ShPardakht = "", BarcodeText = "", ShParvande = "";
                    
                    System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    var datetime = p.sp_GetDate().FirstOrDefault().CurrentDateTime;
                    p.sp_PeacockeryInsert(car.fldID, datetime, bankid.fldID, "", Convert.ToInt32(fldPrice),
                        Convert.ToInt32(fldFine), fldValueAddPrice, fldOtherPrice, Convert.ToInt32(mablagh),
                        MyLib.Shamsi.Shamsi2miladiDateTime(car.fldStartDateInsurance), datetime, Convert.ToInt32(Session["GeustId"]),
                        "", Session["UserPass"].ToString(), fldMainDiscount, fldValueAddDiscount, fldOtherDiscount, ShGhabz, ShPardakht, _id, fldFineDiscount);
                    if (Convert.ToInt32(Session["CountryType"]) == 5)
                    {
                        var mnu = p.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (mnu.fldInformaticesCode == "")
                            mnu.fldInformaticesCode = "0";
                        if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                        {
                            var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                            if (Divisions != null)
                            {
                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                sal = ShParvande.Length - 2;
                                if (ShParvande.Length > 8)
                                {
                                    string s = ShParvande.Substring(8).PadRight(2, '0');
                                    ShParvande = ShParvande.Substring(0, 8);
                                    mah = Convert.ToInt32(s);
                                }
                                ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                    , Convert.ToInt32(mablagh), sal, mah);
                                ShGhabz = gh.ShGhabz;
                                ShPardakht = gh.ShPardakht;
                                BarcodeText = gh.BarcodeText;
                            }
                        }
                    }
                    else if (Convert.ToInt32(Session["CountryType"]) == 6)
                    {
                        var local = p.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (local.fldSourceInformatics == "")
                            local.fldSourceInformatics = "0";
                        if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                        {
                            var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                            if (Divisions != null)
                            {
                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                sal = ShParvande.Length - 2;
                                if (ShParvande.Length > 8)
                                {
                                    string s = ShParvande.Substring(8).PadRight(2, '0');
                                    ShParvande = ShParvande.Substring(0, 8);
                                    mah = Convert.ToInt32(s);
                                }
                                ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                    , Convert.ToInt32(mablagh), sal, mah);
                                ShGhabz = gh.ShGhabz;
                                ShPardakht = gh.ShPardakht;
                                BarcodeText = gh.BarcodeText;
                            }
                        }
                    }
                    else if (Convert.ToInt32(Session["CountryType"]) == 7)
                    {
                        var area = p.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (area != null)
                        {
                            if (area.fldLocalID != null)
                            {
                                var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();

                                if (local.fldSourceInformatics == "")
                                    local.fldSourceInformatics = "0";
                                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                            , Convert.ToInt32(mablagh), sal, mah);
                                        ShGhabz = gh.ShGhabz;
                                        ShPardakht = gh.ShPardakht;
                                        BarcodeText = gh.BarcodeText;
                                    }
                                }
                            }
                            else
                            {
                                var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (mnu.fldInformaticesCode == "")
                                    mnu.fldInformaticesCode = "0";
                                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                            , Convert.ToInt32(mablagh), sal, mah);
                                        ShGhabz = gh.ShGhabz;
                                        ShPardakht = gh.ShPardakht;
                                        BarcodeText = gh.BarcodeText;
                                    }
                                }
                            }
                        }
                    }
                    else if (Convert.ToInt32(Session["CountryType"]) == 8)
                    {
                        var office = p.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (office != null)
                        {
                            if (office.fldAreaID != null)
                            {
                                var area = p.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (area != null)
                                {
                                    if (area.fldLocalID != null)
                                    {
                                        var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();

                                        if (local.fldSourceInformatics == "")
                                            local.fldSourceInformatics = "0";
                                        if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                        {
                                            var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                            if (Divisions != null)
                                            {
                                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                                ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                                sal = ShParvande.Length - 2;
                                                if (ShParvande.Length > 8)
                                                {
                                                    string s = ShParvande.Substring(8).PadRight(2, '0');
                                                    ShParvande = ShParvande.Substring(0, 8);
                                                    mah = Convert.ToInt32(s);
                                                }
                                                ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                                    , Convert.ToInt32(mablagh), sal, mah);
                                                ShGhabz = gh.ShGhabz;
                                                ShPardakht = gh.ShPardakht;
                                                BarcodeText = gh.BarcodeText;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                        if (mnu.fldInformaticesCode == "")
                                            mnu.fldInformaticesCode = "0";
                                        if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                        {
                                            var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                            if (Divisions != null)
                                            {
                                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                                ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                                sal = ShParvande.Length - 2;
                                                if (ShParvande.Length > 8)
                                                {
                                                    string s = ShParvande.Substring(8).PadRight(2, '0');
                                                    ShParvande = ShParvande.Substring(0, 8);
                                                    mah = Convert.ToInt32(s);
                                                }
                                                ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                                    , Convert.ToInt32(mablagh), sal, mah);
                                                ShGhabz = gh.ShGhabz;
                                                ShPardakht = gh.ShPardakht;
                                                BarcodeText = gh.BarcodeText;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var local = p.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (local != null)
                                {
                                    if (local.fldSourceInformatics == "")
                                        local.fldSourceInformatics = "0";
                                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                                , Convert.ToInt32(mablagh), sal, mah);
                                            ShGhabz = gh.ShGhabz;
                                            ShPardakht = gh.ShPardakht;
                                            BarcodeText = gh.BarcodeText;
                                        }
                                    }
                                }
                                else
                                {
                                    var mnu = p.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                    if (mnu.fldInformaticesCode == "")
                                        mnu.fldInformaticesCode = "0";
                                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                                , Convert.ToInt32(mablagh), sal, mah);
                                            ShGhabz = gh.ShGhabz;
                                            ShPardakht = gh.ShPardakht;
                                            BarcodeText = gh.BarcodeText;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Avarez.DataSet.DataSet1TableAdapters.QueriesTableAdapter Queries = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                    Queries.PeacokeryUpdate(ShGhabz, ShPardakht, Convert.ToInt64(_id.Value));
                    return null;
                }
                return null;
            
        }
        public long CheckExistFishForPos(long carid, int showmoney)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
            string MohlatDate = "";
            byte roundNumber = 0;
            var ServerDate = p.sp_GetDate().FirstOrDefault();
            if (SubSetting != null)
            {
                if (SubSetting.fldLastRespitePayment > 0)
                {
                    MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                }
                else if (SubSetting.fldLastRespitePayment == 0)
                {
                    string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                    int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                    int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                    int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                    if (Mounth <= 6)
                        MohlatDate = Year + "/" + Mounth + "/31";
                    else if (Mounth > 6 && Mounth < 12)
                        MohlatDate = Year + "/" + Mounth + "/30";
                    else if (MyLib.Shamsi.Iskabise(Year) == true)
                        MohlatDate = Year + "/" + Mounth + "/30";
                    else
                        MohlatDate = Year + "/" + Mounth + "/29";
                    //if
                }
                var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                roundNumber = Round.fldRound;
            }


            double Rounded = 10;
            switch (roundNumber)
            {
                case 3:
                    Rounded = 1000;
                    break;
                case 2:
                    Rounded = 100;
                    break;
            }


            Session["showmoney"] = Convert.ToInt32(Math.Ceiling(showmoney / Rounded) * Rounded);//گرد به بالا  
            var q = p.sp_SelectExistPeacockery(carid, Convert.ToInt32(Session["showmoney"])).FirstOrDefault();
            if (q != null)
                if (q.PeacockeryId != null)
                {
                    var t = p.sp_PeacockerySelect("fldId", q.PeacockeryId.ToString(), 1, 1, "").FirstOrDefault();
                    var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    if (bankid.fldID == t.fldAccountBankID)
                    {
                        return (long)q.PeacockeryId;
                    }
                    else
                        return 0;
                }
                else
                    return 0;
            else
                return 0;
        }
        public ActionResult GoToOnlinePay1(decimal Amount, int CarId)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            long peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
            string shGabz = "", Shpardakht = "";
            if (peackokeryid == 0)
            {
                FishReport(CarId);
                peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
            }
            if (peackokeryid != 0)
            {
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                if (fish.fldShGhabz != "" && fish.fldShPardakht != "")
                {
                    shGabz = fish.fldShGhabz;
                    Shpardakht = fish.fldShPardakht;
                }
            }
            return Json(new { shGabz = shGabz, Shpardakht = Shpardakht }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GoToOnlinePay(decimal Amount, int CarId, int BankId)
        {
            //Amount = 1000;
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_SelectCarDetils(CarId).FirstOrDefault();
            string Tax = "";
            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
            if (BankId != 17)
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["GeustId"]), "", Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);

            if (BankId != 15)
            {
                Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
            }
            else
            {
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["GeustId"]), "", Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);
                var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                Tax = q.fldTemporaryCode;
            }
            if (BankId == 17)
            {

                long peackokeryid = 0;/* CheckExistFishForPos(CarId, Convert.ToInt32(Amount));*/
                if (peackokeryid == 0)
                {
                    FishReport(CarId);
                    peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
                }
                if (peackokeryid != 0)
                {
                    var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                    if (fish.fldShGhabz != "" && fish.fldShPardakht != "")
                    {
                        p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["GeustId"]), fish.fldShGhabz + "|" + fish.fldShPardakht, Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);
                        var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
                        Session["shGhabz"] = fish.fldShGhabz;
                        Session["shPardakht"] = fish.fldShPardakht;
                        Session["OnlinefishId"] = fish.fldID;
                    }
                    else
                        return null;
                }
            }
            
            p.sp_OnlineTemporaryCodePaymentsUpdate(Convert.ToInt32(_id.Value), Tax, Amount, 1, "");
            //if (Session["IsOfficeUser"] != null)
            //{
            //    epishkhan.devpishkhannwsv1 epishkhan = new epishkhan.devpishkhannwsv1();
            //    var result = epishkhan.servicePay("atJ5+$J1RtFpj", Session["ver_code"].ToString(), Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ServiceId"]), 100);
            //    if (result > 0)
            //        Session["serviceCallSerial"] = result;
            //    else
            //    {
            //        switch (result)
            //        {
            //            case -3:
            //                return Json("مهلت تراکنش به پایان رسیده است.", JsonRequestBehavior.AllowGet);
            //            case -7:
            //                return Json("مجوز استفاده از این سرویس را ندارید.", JsonRequestBehavior.AllowGet);
            //            case -8:
            //                return Json("کاربر شما غیر فعال شده است.", JsonRequestBehavior.AllowGet);
            //            case -10:
            //                return Json("اعتبار شما کافی نیست.", JsonRequestBehavior.AllowGet);
            //        }
            //    }
            //}
            Session["Amount"] = Amount;
            Session["Tax"] = Tax;
            Session["ReturnUrl"] = "/Home/Guest";
            string URL = "";
            if (BankId == 20)
            {
                URL = "CityBank";
            }
            else if (BankId == 1)
            {
                URL = "MeliBank";
            }
            else if (BankId == 2)
            {
                URL = "TejaratBank";
            }
            else if (BankId == 15)
            {
                URL = "Parsian";
            }
            else if (BankId == 17)
            {
                URL = "Saman";
            } 
            return RedirectToAction("Index", URL);
        }

        public ActionResult calc()
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldCarId", Session["fldCarID3"].ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
            int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4));
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";
            System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                Transaction Tr = new Transaction();
                var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["GeustId"]));
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
            var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                null, DateTime.Now,Convert.ToInt32(Session["GeustId"]), _year, _Bed).ToList();
            //Session["year"] = _year.Value.ToString();
            //Session["bed"] = _Bed.Value.ToString();

            Session.Remove("fldCarID3");
            if (_year.Value.ToString() == "")
            {
                int? mablagh = 0;
                int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0, fldMainDiscount = 0, fldFineDiscount = 0,
                    fldValueAddDiscount = 0, fldOtherDiscount = 0;
                ArrayList Years = new ArrayList();
                foreach (var item in bedehi)
                {
                    mablagh += item.fldDept;
                    fldFine += (int)item.fldFine;
                    fldValueAddPrice += (int)item.fldValueAdded;
                    fldPrice += (int)item.fldCurectPrice;
                    Years.Add(item.fldyear);
                    fldOtherPrice += (int)item.OtherPrice;
                    fldMainDiscount += (int)item.fldDiscount;
                    fldFineDiscount += (int)item.fldFineDiscount;
                    fldValueAddDiscount += (int)item.fldValueAddDiscount;
                    fldOtherDiscount += (int)item.fldOtherDiscount;
                }

                int sal = 0, mah = 0;
                mablagh += Convert.ToInt32(_Bed.Value);
                fldPrice += Convert.ToInt32(_Bed.Value);
                Session["mablagh"] = mablagh;
                Session["Fine"] = fldFine;
                Session["ValueAddPrice"] = fldValueAddPrice;
                Session["Price"] = fldPrice;
                Session["Year"] = Years;
                Session["OtherPrice"] = fldOtherPrice;
                Session["fldMainDiscount"] = fldMainDiscount;
                Session["fldFineDiscount"] = fldFineDiscount;
                Session["fldValueAddDiscount"] = fldValueAddDiscount;
                Session["fldOtherDiscount"] = fldOtherDiscount;
                if (mablagh < 1000)
                {
                    mablagh = 0;
                    //bedehi = null;
                }

                string shGhabz = "", ShParvande = "",
                    shPardakht = "",
                    barcode = "";
                if (Convert.ToInt32(Session["CountryType"]) == 5)
                {
                    var mnu = m.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                    {
                        var Divisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                            sal = ShParvande.Length - 2;
                            if (ShParvande.Length > 8)
                            {
                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                ShParvande = ShParvande.Substring(0, 8);
                                mah = Convert.ToInt32(s);
                            }
                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                , Convert.ToInt32(mablagh), sal, mah);
                            shGhabz = gh.ShGhabz;
                            shPardakht = gh.ShPardakht;
                            barcode = gh.BarcodeText;
                        }
                    }
                }
                else if (Convert.ToInt32(Session["CountryType"]) == 6)
                {
                    var local = m.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                    {
                        var Divisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                            sal = ShParvande.Length - 2;
                            if (ShParvande.Length > 8)
                            {
                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                ShParvande = ShParvande.Substring(0, 8);
                                mah = Convert.ToInt32(s);
                            }
                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                 , Convert.ToInt32(mablagh), sal, mah);
                            shGhabz = gh.ShGhabz;
                            shPardakht = gh.ShPardakht;
                            barcode = gh.BarcodeText;
                        }
                    }
                }
                else if (Convert.ToInt32(Session["CountryType"]) == 7)
                {
                    var area = m.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    if (area != null)
                    {
                        if (area.fldLocalID != null)
                        {
                            var local = m.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();

                            if (local.fldSourceInformatics == "")
                                local.fldSourceInformatics = "0";
                            if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                            {
                                var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                                    sal = ShParvande.Length - 2;
                                    if (ShParvande.Length > 8)
                                    {
                                        string s = ShParvande.Substring(8).PadRight(2, '0');
                                        ShParvande = ShParvande.Substring(0, 8);
                                        mah = Convert.ToInt32(s);
                                    }
                                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                        , Convert.ToInt32(mablagh), sal, mah);
                                    shGhabz = gh.ShGhabz;
                                    shPardakht = gh.ShPardakht;
                                    barcode = gh.BarcodeText;
                                }
                            }
                        }
                        else
                        {
                            var mnu = m.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            if (mnu.fldInformaticesCode == "")
                                mnu.fldInformaticesCode = "0";
                            if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                            {
                                var Divisions = m.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                                    sal = ShParvande.Length - 2;
                                    if (ShParvande.Length > 8)
                                    {
                                        string s = ShParvande.Substring(8).PadRight(2, '0');
                                        ShParvande = ShParvande.Substring(0, 8);
                                        mah = Convert.ToInt32(s);
                                    }
                                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                        , Convert.ToInt32(mablagh), sal, mah);
                                    shGhabz = gh.ShGhabz;
                                    shPardakht = gh.ShPardakht;
                                    barcode = gh.BarcodeText;
                                }
                            }
                        }
                    }
                }
                else if (Convert.ToInt32(Session["CountryType"]) == 8)
                {
                    var office = m.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    if (office != null)
                    {
                        if (office.fldAreaID != null)
                        {
                            var area = m.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            if (area != null)
                            {
                                if (area.fldLocalID != null)
                                {
                                    var local = m.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();

                                    if (local.fldSourceInformatics == "")
                                        local.fldSourceInformatics = "0";
                                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                    {
                                        var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                                , Convert.ToInt32(mablagh), sal, mah);
                                            shGhabz = gh.ShGhabz;
                                            shPardakht = gh.ShPardakht;
                                            barcode = gh.BarcodeText;
                                        }
                                    }
                                }
                                else
                                {
                                    var mnu = m.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                    if (mnu.fldInformaticesCode == "")
                                        mnu.fldInformaticesCode = "0";
                                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                    {
                                        var Divisions = m.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                                , Convert.ToInt32(mablagh), sal, mah);
                                            shGhabz = gh.ShGhabz;
                                            shPardakht = gh.ShPardakht;
                                            barcode = gh.BarcodeText;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var local = m.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            if (local != null)
                            {
                                if (local.fldSourceInformatics == "")
                                    local.fldSourceInformatics = "0";
                                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                {
                                    var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                            , Convert.ToInt32(mablagh), sal, mah);
                                        shGhabz = gh.ShGhabz;
                                        shPardakht = gh.ShPardakht;
                                        barcode = gh.BarcodeText;
                                    }
                                }
                            }
                            else
                            {
                                var mnu = m.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (mnu.fldInformaticesCode == "")
                                    mnu.fldInformaticesCode = "0";
                                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                {
                                    var Divisions = m.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                            , Convert.ToInt32(mablagh), sal, mah);
                                        shGhabz = gh.ShGhabz;
                                        shPardakht = gh.ShPardakht;
                                        barcode = gh.BarcodeText;
                                    }
                                }
                            }
                        }

                    }
                }

                return Json(new
                {
                    bedehi = bedehi,
                    mablagh = mablagh,
                    shGhabz = shGhabz,
                    shPardakht = shPardakht,
                    barcode = barcode,
                    msg = ""
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string s = "", msg = "", year = _year.Value.ToString();
                for (int i = 0; i < year.Length; i += 4)
                {
                    if (i < year.Length - 4)
                        s += year.Substring(i, 4) + " و ";
                    else
                        s += year.Substring(i, 4);
                }
                msg = "تعرفه سالهای " + s + " تعریف نشده است لطفا به مدیر سیستم گزارش دهید.";
                return Json(new
                {
                    msg = msg
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //public JsonResult Shpardakht(decimal mablagh, int CarId)
        //{


        //}
        public JsonResult Fill()
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_SelectCarDetils(Convert.ToInt32(Session["fldCarID1"])).FirstOrDefault();
            Session.Remove("fldCarID1");
            return Json(new
            {
                plaq = car.fldPlaquNumber,
                classs = car.fldCarClassName,
                modell = car.fldCarModel,
                syst = car.fldCarSystemName,
                cabin = car.fldCarCabinName,
                account = car.fldCarAccountName,
                make = car.fldCarMakeName,
                Malek = car.fldOwnerName,
                motor = car.fldMotorNumber,
                shasi = car.fldShasiNumber,
                vin = car.fldVIN,
                color = car.fldColor,
                date = car.fldStartDateInsurance,
                datep = car.fldDatePlaque,
                year = car.fldModel,
                carId = car.fldCarID//carid

            }, JsonRequestBehavior.AllowGet);
        }
    }
}