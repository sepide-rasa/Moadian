using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Collections;
using System.IO;
using System.Web.Configuration;
using System.Configuration;
using System.Xml;

namespace Avarez.Areas.NewVer.Controllers.Guest
{
    public class Facture_NewController : Controller
    {
        //
        // GET: /NewVer/Facture_New/

        public ActionResult Index(string containerId, long id, string carFileId)
        {
            if (Session["UserState"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            ViewData.Model = new Avarez.Models.Facture_Guest();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };
            result.ViewBag.carFileId = carFileId;
            this.GetCmp<TabPanel>("GuestTab").SetActiveTab(0);
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Carid = id;
            return result;
        }
        public ActionResult Fill(string carFileId)
        { 
            if (Session["UserState"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_SelectCarDetilsByCarFileID(Convert.ToInt32(carFileId)).FirstOrDefault();
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
        public ActionResult calc(int CarID)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldCarId", CarID.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
            int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4));
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";
            //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                Transaction_Guest Tr = new Transaction_Guest();
                var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["GeustId"]));
                string msg1 = "";
                switch (Result)
                {
                    case Transaction_Guest.TransactionResult.Fail:
                        msg1 = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.";
                        return Json(new
                        {
                            Msg = msg1
                        }, JsonRequestBehavior.AllowGet);

                    case Transaction_Guest.TransactionResult.NotSharj:
                        msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
                        return Json(new
                        {
                            Msg = msg1
                        }, JsonRequestBehavior.AllowGet);

                }
            }
            //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
            //    null, DateTime.Now, Convert.ToInt32(Session["GeustId"]), _year, _Bed).ToList();

            //Session["year"] = _year.Value.ToString();
            //Session["bed"] = _Bed.Value.ToString();
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
                foreach (var item in bedehi)
                {
                    mablagh += item.fldDept;
                    fldFine += (int)item.fldMablaghJarime;
                    fldValueAddPrice += (int)item.fldValueAdded;
                    fldPrice += (int)((item.fldFinalPrice - item.fldValueAdded) + item.fldMashmol + item.fldNoMashmol); 
                    Years.Add(item.fldYear);
                    fldOtherPrice += (int)item.fldOtherPrice;
                    fldMainDiscount += (int)item.fldDiscontMoaserPrice;
                    fldFineDiscount += (int)item.fldDiscontJarimePrice;
                    fldValueAddDiscount += (int)item.fldDiscontValueAddPrice;
                    fldOtherDiscount += (int)item.fldDiscontOtherPrice;
                }

                int sal = 0, mah = 0;
                //mablagh += Convert.ToInt32(_Bed.Value);
                //fldPrice += Convert.ToInt32(_Bed.Value);
                /*Session["mablagh"] = mablagh;
                Session["Fine"] = fldFine;
                Session["ValueAddPrice"] = fldValueAddPrice;
                Session["Price"] = fldPrice;
                Session["Year"] = Years;
                Session["OtherPrice"] = fldOtherPrice;
                Session["fldMainDiscount"] = fldMainDiscount;
                Session["fldFineDiscount"] = fldFineDiscount;
                Session["fldValueAddDiscount"] = fldValueAddDiscount;
                Session["fldOtherDiscount"] = fldOtherDiscount;*/
                if (mablagh < 10000)
                {
                    mablagh = 0;
                    //bedehi = null;
                }
                /*var Year = "";
                if (Years != null)
                {
                    for (int i = 0; i < Years.Count; i++)
                    {
                        Year = Year + Years[i]+',';
                    }
                }*/
                string shGhabz = "", ShParvande = "",
                    shPardakht = "",
                    barcode = "";
                if (Convert.ToInt32(Session["CountryType"]) == 5)
                {
                    var mnu = m.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                    {
                        var Divisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                    var local = m.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                    {
                        var Divisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                    var area = m.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (area != null)
                    {
                        if (area.fldLocalID != null)
                        {
                            var local = m.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();

                            if (local.fldSourceInformatics == "")
                                local.fldSourceInformatics = "0";
                            if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                            {
                                var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                            var mnu = m.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                            if (mnu.fldInformaticesCode == "")
                                mnu.fldInformaticesCode = "0";
                            if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                            {
                                var Divisions = m.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                    var office = m.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (office != null)
                    {
                        if (office.fldAreaID != null)
                        {
                            var area = m.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                            if (area != null)
                            {
                                if (area.fldLocalID != null)
                                {
                                    var local = m.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();

                                    if (local.fldSourceInformatics == "")
                                        local.fldSourceInformatics = "0";
                                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                    {
                                        var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                                    var mnu = m.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                    if (mnu.fldInformaticesCode == "")
                                        mnu.fldInformaticesCode = "0";
                                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                    {
                                        var Divisions = m.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                            var local = m.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                            if (local != null)
                            {
                                if (local.fldSourceInformatics == "")
                                    local.fldSourceInformatics = "0";
                                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                {
                                    var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                                var mnu = m.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                if (mnu.fldInformaticesCode == "")
                                    mnu.fldInformaticesCode = "0";
                                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                {
                                    var Divisions = m.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                    fldFine = fldFine,
                    fldValueAddPrice = fldValueAddPrice,
                    fldPrice = fldPrice,
                    Years = Years,
                    fldOtherPrice = fldOtherPrice,
                    fldMainDiscount = fldMainDiscount,
                    fldFineDiscount = fldFineDiscount,
                    fldValueAddDiscount = fldValueAddDiscount,
                    fldOtherDiscount = fldOtherDiscount,
                    //Bed = Convert.ToInt32(_Bed.Value),
                    Msg = ""
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string s = "", msg = "", year = _year.ToString();
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
                    Msg = msg
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult calcBill(int CarID)
        {
            if (Session["GuestInfId"] == null)
            return Json(new
            {
                Er=1,
                Msg = "جهت مشاهده صورتحساب ابتدا باید وارد سایت شوید."
            }, JsonRequestBehavior.AllowGet);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldCarId", CarID.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
            int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4));
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";

            var Guest = m.sp_tblGuestInfoSelect("fldId", Session["GuestInfId"].ToString(), "", 1).FirstOrDefault();
            //var Division = m.sp_CountryDivisionsSelect("fldMunicipalityID", Guest.fldMunId.ToString(), 1, 1, "").FirstOrDefault();
            var transaction = m.sp_TransactionInfSelect("fldDivId", Guest.fldCountryDivTempId.ToString(), 0).OrderBy(l => l.fldId).FirstOrDefault();
            WebTransaction.TransactionWebService trn = new WebTransaction.TransactionWebService();
            //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                Transaction_Guest Tr = new Transaction_Guest();                
                //var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var TransactionInf = m.sp_TransactionInfSelect("fldUserName", Guest.fldCodeMeli, 0).FirstOrDefault();
                var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["GeustId"]));

                switch (Result)
                {
                    case Transaction_Guest.TransactionResult.Fail:
                        return Json(new
                        {
                            Msg = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.",
                            Er=3
                        }, JsonRequestBehavior.AllowGet);

                    case Transaction_Guest.TransactionResult.NotSharj:
                        var Mablagh = trn.GetPayablePrice(transaction.fldUserName, transaction.fldPass, transaction.fldCountryDivisionsName);
                        var msg1="خدمت مورد نظر مبلغ " + Mablagh.ToString("N0") + " ریال هزینه دارد. در صورت تمایل به پرداخت روی لینک ذیل کلیک نمایید.</br></br><div style='text-align:right;height:25px;'><a href='http://trn.ecartax.ir/_account/LoginFromPrg?username=" +
                            TransactionInf.fldUserName + "&pass=" + TransactionInf.fldPass + "&UserType=2" + "'>لینک پرداخت</a></div>";
                        return Json(new
                        {
                            Msg = msg1,
                            Er=2
                        }, JsonRequestBehavior.AllowGet);

                }
            }
            //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
            //    null, DateTime.Now, Convert.ToInt32(Session["GeustId"]), _year, _Bed).ToList();

            //Session["year"] = _year.Value.ToString();
            //Session["bed"] = _Bed.Value.ToString();
            var bedehi = m.prs_newCarFileCalc(DateTime.Now, Convert.ToInt32(Session["CountryType"]),
                Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["GeustId"])).Where(k => k.fldCollectionId == 0).ToList();
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
                foreach (var item in bedehi)
                {
                    mablagh += item.fldDept;
                    fldFine += (int)item.fldMablaghJarime;
                    fldValueAddPrice += (int)item.fldValueAdded;
                    fldPrice += (int)((item.fldFinalPrice - item.fldValueAdded) + item.fldMashmol + item.fldNoMashmol);
                    Years.Add(item.fldYear);
                    fldOtherPrice += (int)item.fldOtherPrice;
                    fldMainDiscount += (int)item.fldDiscontMoaserPrice;
                    fldFineDiscount += (int)item.fldDiscontJarimePrice;
                    fldValueAddDiscount += (int)item.fldDiscontValueAddPrice;
                    fldOtherDiscount += (int)item.fldDiscontOtherPrice;
                }

                int sal = 0, mah = 0;
                //mablagh += Convert.ToInt32(_Bed.Value);
                //fldPrice += Convert.ToInt32(_Bed.Value);
                /*Session["mablagh"] = mablagh;
                Session["Fine"] = fldFine;
                Session["ValueAddPrice"] = fldValueAddPrice;
                Session["Price"] = fldPrice;
                Session["Year"] = Years;
                Session["OtherPrice"] = fldOtherPrice;
                Session["fldMainDiscount"] = fldMainDiscount;
                Session["fldFineDiscount"] = fldFineDiscount;
                Session["fldValueAddDiscount"] = fldValueAddDiscount;
                Session["fldOtherDiscount"] = fldOtherDiscount;*/
                if (mablagh < 10000)
                {
                    mablagh = 0;
                    //bedehi = null;
                }
                /*var Year = "";
                if (Years != null)
                {
                    for (int i = 0; i < Years.Count; i++)
                    {
                        Year = Year + Years[i]+',';
                    }
                }*/
                string shGhabz = "", ShParvande = "",
                    shPardakht = "",
                    barcode = "";
                if (Convert.ToInt32(Session["CountryType"]) == 5)
                {
                    var mnu = m.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                    {
                        var Divisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                    var local = m.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                    {
                        var Divisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                    var area = m.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (area != null)
                    {
                        if (area.fldLocalID != null)
                        {
                            var local = m.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();

                            if (local.fldSourceInformatics == "")
                                local.fldSourceInformatics = "0";
                            if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                            {
                                var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                            var mnu = m.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                            if (mnu.fldInformaticesCode == "")
                                mnu.fldInformaticesCode = "0";
                            if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                            {
                                var Divisions = m.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                    var office = m.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (office != null)
                    {
                        if (office.fldAreaID != null)
                        {
                            var area = m.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                            if (area != null)
                            {
                                if (area.fldLocalID != null)
                                {
                                    var local = m.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();

                                    if (local.fldSourceInformatics == "")
                                        local.fldSourceInformatics = "0";
                                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                    {
                                        var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                                    var mnu = m.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                    if (mnu.fldInformaticesCode == "")
                                        mnu.fldInformaticesCode = "0";
                                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                    {
                                        var Divisions = m.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                            var local = m.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                            if (local != null)
                            {
                                if (local.fldSourceInformatics == "")
                                    local.fldSourceInformatics = "0";
                                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                {
                                    var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                                var mnu = m.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                if (mnu.fldInformaticesCode == "")
                                    mnu.fldInformaticesCode = "0";
                                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                {
                                    var Divisions = m.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                    fldFine = fldFine,
                    fldValueAddPrice = fldValueAddPrice,
                    fldPrice = fldPrice,
                    Years = Years,
                    fldOtherPrice = fldOtherPrice,
                    fldMainDiscount = fldMainDiscount,
                    fldFineDiscount = fldFineDiscount,
                    fldValueAddDiscount = fldValueAddDiscount,
                    fldOtherDiscount = fldOtherDiscount,
                    //Bed = Convert.ToInt32(_Bed.Value),
                    Msg = "",
                    Er=0
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string s = "", msg = "", year = _year.ToString();
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
                    Er=3,
                    Msg = msg
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Read_Savabegh(StoreRequestParameters parameters, int CarID)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            List<Avarez.Models.sp_CarExperienceSelect> data = null;
            data = p.sp_CarExperienceSelect("fldCarID", CarID.ToString(), 30, Convert.ToInt32(Session["GeustId"]), "").ToList();
            /*int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Avarez.Models.sp_CarExperienceSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);*/
            //-- end paging ------------------------------------------------------------

            return this.Store(data);
        }

        public ActionResult Read_Varizi(StoreRequestParameters parameters, int CarID)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            List<Avarez.Models.rpt_Receipt> data = null;
            data = p.rpt_Receipt(CarID, 2).ToList();
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Avarez.Models.rpt_Receipt> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult PrintReceipt(string id)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Session["ResidId"] = id;
            PartialView.ViewBag.id = id;
            return PartialView;
        }
        public ActionResult GeneratePDFResid(string id)
        {
            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter resid = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter carDitail = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
            var q = car.rpt_Receipt(Convert.ToInt32(Session["ResidId"]), 1).FirstOrDefault(); ;
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();
            carDitail.Fill(dt.sp_SelectCarDetils, Convert.ToInt32(q.fldCarId));
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
            resid.Fill(dt.rpt_Receipt, Convert.ToInt32(Session["ResidId"]), 1);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_Resid.frx");
            Report.RegisterData(dt, "complicationsCarDBDataSet");
            Report.Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
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
            Session.Remove("ResidId");
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
           
        }
        public ActionResult showMafasa(int id)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.id = id;
            return PartialView;
        }
        public ActionResult Mafasa(int id)
        {
            if (Session["GuestInfId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_SelectCarDetilsByCarFileID(id).FirstOrDefault();
            if (car != null)
            {
                var Cdate = p.sp_GetDate().FirstOrDefault();
                int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Cdate.CurrentDateTime).Substring(0, 4));
                string date = toYear + "/12/29";
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                    date = toYear + "/12/30";
                //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
                {
                    Transaction_Guest Tr = new Transaction_Guest();
                    //var Div = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    //var TransactionInf = p.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                    var Guest = p.sp_tblGuestInfoSelect("fldId", Session["GuestInfId"].ToString(), "", 1).FirstOrDefault();
                    var TransactionInf = p.sp_TransactionInfSelect("fldUserName", Guest.fldCodeMeli, 0).FirstOrDefault();
                    var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["GeustId"]));
                    string msg1 = "";
                    switch (Result)
                    {
                        case Transaction_Guest.TransactionResult.Fail:
                            {
                                this.GetCmp<Window>("Mafasa_Win").Destroy();
                                return Json(new { Er = 1, Msg = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);
                            }

                        //msg1 = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.";
                        //return Json(new
                        //{
                        //    msg = msg1
                        //}, JsonRequestBehavior.AllowGet);

                        case Transaction_Guest.TransactionResult.NotSharj:
                            {
                                this.GetCmp<Window>("Mafasa_Win").Destroy();
                                return Json(new { Er = 1, Msg = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);
                            }
                        //msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
                        //return Json(new
                        //{
                        //    msg = msg1
                        //}, JsonRequestBehavior.AllowGet);

                    }
                }
                //var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                //    null, DateTime.Now, Convert.ToInt32(Session["GeustId"]), _year, _Bed).ToList();
                var bedehi = p.prs_newCarFileCalc(DateTime.Now, Convert.ToInt32(Session["CountryType"]),
                Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["GeustId"])).Where(k => k.fldCollectionId == 0).ToList();
                string _year = "";
                if (bedehi != null)
                {
                    var nullYears = bedehi.Where(k => k.fldPrice == null).ToList();
                    foreach (var item in nullYears)
                    {
                        _year += item.fldYear;
                    }
                }
                int mablagh = 0;
                foreach (var item in bedehi)
                {
                    int? jam = (item.fldFinalPrice + item.fldMashmol + item.fldNoMashmol + item.fldMablaghJarime + item.fldOtherPrice) -
                        (item.fldDiscontJarimePrice + item.fldDiscontMoaserPrice + item.fldDiscontOtherPrice + item.fldDiscontValueAddPrice);
                    mablagh +=(int) jam;
                }
                if (mablagh <= 10000)
                {
                    Session["CarFileId"] = car.fldID;
                    Session["Sal"] = toYear.ToString().Substring(0, 4);
                    Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                    Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccount1TableAdapter fish1 = new Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccount1TableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter Receipt = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
                    sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                    Receipt.Fill(dt.rpt_Receipt, Convert.ToInt32(car.fldCarID), 2);
                    Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter exp = new Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter();
                    var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                    if (ImageSetting == "3")
                    {
                        fish1.Fill(dt.rpt_RecoupmentAccount1, car.fldID,DateTime.Now);
                    }
                    else
                    {
                        fish.Fill(dt.rpt_RecoupmentAccount, car.fldID, DateTime.Now);
                    }
                    exp.Fill(dt.sp_CarExperienceSelect, "fldCarFileID", car.fldID.ToString(), 0, Convert.ToInt32(Session["UserMnu"].ToString()), "");
                    Avarez.Models.cartaxEntities Car = new Avarez.Models.cartaxEntities();
                    Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                    var mnu = Car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    var State = Car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();

                    System.Data.Entity.Core.Objects.ObjectParameter mafasaId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterDate = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterDate", typeof(DateTime));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterNum = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterNum", typeof(string));

                    p.Sp_MafasaInsert(mafasaId, car.fldCarID, Convert.ToInt32(Session["UserMnu"]), null, Convert.ToInt32(Session["GeustId"]), LetterDate, LetterNum);
                    p.sp_UpdateGuestInfoId("Mafasa", mafasaId.Value.ToString(), Convert.ToInt32(Session["GuestInfId"]), Convert.ToInt32(Session["GeustId"]));
                    string barcode = WebConfigurationManager.AppSettings["SiteURL"] + "/NewVer/QR_MafasaNew/Get/" + mafasaId.Value;
                    string Url = WebConfigurationManager.AppSettings["SiteURL"] + "/NewVer/query";
                    Guid mid = Guid.Parse(mafasaId.Value.ToString());
                    var _ref = p.Sp_MafasaSelect(car.fldCarID).Where(k => k.fldId == mid).FirstOrDefault();
                    FastReport.Report Report = new FastReport.Report();
                    if (ImageSetting == "3")//زنجان
                    {
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_MafasaZ.frx");
                    }
                    else
                    {
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_Mafasa.frx");
                    }
                    Report.RegisterData(dt, "carTaxDataSet");
                    Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(LetterDate.Value)));
                    var time = Convert.ToDateTime(LetterDate.Value);
                    Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                    Report.SetParameterValue("Num", LetterNum.Value);
                    Report.SetParameterValue("barcode", barcode);
                    Report.SetParameterValue("MunicipalityName", mnu.fldName);
                    Report.SetParameterValue("StateName", State.fldName);
                    Report.SetParameterValue("ref", _ref.fldRef);
                    Report.SetParameterValue("Url", Url);
                    Report.SetParameterValue("AreaName","" );//Session["area"].ToString()
                    Report.SetParameterValue("OfficeName","" );//Session["office"].ToString()
                    Report.SetParameterValue("sal", Session["Sal"]);
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
                    MemoryStream stream = new MemoryStream();
                    Report.Prepare();
                    Report.Export(pdf, stream);
                    p.Sp_MafasaUpdate(mafasaId.Value.ToString(), stream.ToArray());
                    return Json(new { Er = 0, IdMafasa = mafasaId.Value.ToString() }, JsonRequestBehavior.AllowGet);

                   // return File(stream.ToArray(), "application/pdf");
                }
                else
                {
                    this.GetCmp<Window>("Mafasa_Win").Destroy();
                    return Json(new { Er = 1, Msg = "خودرو مورد نظر بدهکار است و امکان صدور مفاصا وجود ندارد.", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);
                    //return Json(new { Er = 1, Msg = "کاربر گرامی شما در حال انجام عملیات غیر قانونی می باشید و در صورت تکرار آی پی شما به پلیس فتا اعلام خواهد شد. باتشکر", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);
                }
                   // return Json("کاربر گرامی شما در حال انجام عملیات غیر قانونی می باشید و در صورت تکرار آی پی شما به پلیس فتا اعلام خواهد شد. باتشکر", JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }
        public ActionResult FishReport(int id)
        {
            try
            {
                /* Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                 PartialView.ViewBag.carid = carid;
                 PartialView.ViewBag.Mablagh = Mablagh;
                 PartialView.ViewBag.Fine = Fine;
                 PartialView.ViewBag.ValueAddprice = ValueAddprice;
                 PartialView.ViewBag.Price = Price;
                 PartialView.ViewBag.Years = Years;
                 PartialView.ViewBag.OtherPrice = OtherPrice;
                 PartialView.ViewBag.MainDiscount = MainDiscount;
                 PartialView.ViewBag.FineDisCount = FineDisCount;
                 PartialView.ViewBag.ValueAddDiscount = ValueAddDiscount;
                 PartialView.ViewBag.OtherDiscount = OtherDiscount;
                 return PartialView;*/
                Models.cartaxEntities p = new Models.cartaxEntities();
                var car = p.sp_SelectCarDetils(id).FirstOrDefault();
                if (car != null)
                {
                    var ServerDate = p.sp_GetDate().FirstOrDefault();
                    int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime).Substring(0, 4));
                    //string date = toYear + "/12/29";
                    //if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                    //    date = toYear + "/12/30";
                    System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                    //var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                    //null, ServerDate.CurrentDateTime, _year, _Bed).ToList();
                    int sal = 0, mah = 0;
                    double mablagh = 0;
                    int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0,
                        fldMainDiscount = 0, fldFineDiscount = 0, fldValueAddDiscount = 0, fldOtherDiscount = 0;
                    mablagh = Convert.ToInt32(Session["mablagh"]);
                    fldFine = Convert.ToInt32(Session["Fine"]);
                    fldValueAddPrice = Convert.ToInt32(Session["ValueAddPrice"]);
                    fldPrice = Convert.ToInt32(Session["Price"]);
                    fldOtherPrice = Convert.ToInt32(Session["OtherPrice"]);
                    fldMainDiscount = Convert.ToInt32(Session["fldMainDiscount"]);
                    fldFineDiscount = Convert.ToInt32(Session["fldFineDiscount"]);
                    fldValueAddDiscount = Convert.ToInt32(Session["fldValueAddDiscount"]);
                    fldOtherDiscount = Convert.ToInt32(Session["fldOtherDiscount"]);

                    //////////////////////////////////
                    //mablagh = Convert.ToInt32(Mablagh);
                    //fldFine = Convert.ToInt32(Fine);
                    //fldValueAddPrice = Convert.ToInt32(ValueAddprice);
                    //fldPrice =Convert.ToInt32(Price);
                    //fldOtherPrice = Convert.ToInt32(OtherPrice);
                    //fldMainDiscount = Convert.ToInt32(MainDiscount);
                    //fldFineDiscount = Convert.ToInt32(FineDisCount);
                    //fldValueAddDiscount = Convert.ToInt32(ValueAddDiscount);
                    //fldOtherDiscount = Convert.ToInt32(OtherDiscount);
                    /////////////////////////////////

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

                    //string[] Years = Years2.Split(',');
                    //int c=Years.Count();
                    //int[] AvarezSal = new int[c];
                    //for (int i = 0; i < Years.Count() ; i++)
                    //{
                    //    AvarezSal[i] = Convert.ToInt32(Years[i]);
                    //}

                    mablagh += Convert.ToInt32(_Bed.Value);
                    fldPrice += Convert.ToInt32(_Bed.Value);
                    if (mablagh < 10000)
                    {
                        return Json(new
                        {
                            Msg = "پرونده انتخابی بدهکار نیست.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);

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
                            string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
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
                        case 0:
                            Rounded = 1;
                            break;
                    }


                    mablagh = Math.Floor(mablagh / Rounded) * Rounded;//گرد به پایین

                    string ShGhabz = "", ShPardakht = "", BarcodeText = "", ShParvande = "";

                    System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    var datetime = p.sp_GetDate().FirstOrDefault().CurrentDateTime;
                    p.sp_PeacockeryInsert(car.fldID, datetime, bankid.fldID, "", Convert.ToInt32(fldPrice),
                        Convert.ToInt32(fldFine), fldValueAddPrice, fldOtherPrice, Convert.ToInt32(mablagh),
                        MyLib.Shamsi.Shamsi2miladiDateTime(car.fldStartDateInsurance), datetime, Convert.ToInt32(Session["GeustId"]),
                        "", "", fldMainDiscount, fldValueAddDiscount, fldOtherDiscount, ShGhabz, ShPardakht, _id, fldFineDiscount);
                    if (Convert.ToInt32(Session["CountryType"]) == 5)
                    {
                        var mnu = p.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                        if (mnu.fldInformaticesCode == "")
                            mnu.fldInformaticesCode = "0";
                        if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                        {
                            var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                            if (Divisions != null)
                            {
                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                        var local = p.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                        if (local.fldSourceInformatics == "")
                            local.fldSourceInformatics = "0";
                        if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                        {
                            var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                            if (Divisions != null)
                            {
                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                        var area = p.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                        if (area != null)
                        {
                            if (area.fldLocalID != null)
                            {
                                var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();

                                if (local.fldSourceInformatics == "")
                                    local.fldSourceInformatics = "0";
                                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                                var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                if (mnu.fldInformaticesCode == "")
                                    mnu.fldInformaticesCode = "0";
                                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                        var office = p.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                        if (office != null)
                        {
                            if (office.fldAreaID != null)
                            {
                                var area = p.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                if (area != null)
                                {
                                    if (area.fldLocalID != null)
                                    {
                                        var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();

                                        if (local.fldSourceInformatics == "")
                                            local.fldSourceInformatics = "0";
                                        if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                        {
                                            var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                            if (Divisions != null)
                                            {
                                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                                        var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]),"").FirstOrDefault();
                                        if (mnu.fldInformaticesCode == "")
                                            mnu.fldInformaticesCode = "0";
                                        if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                        {
                                            var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                            if (Divisions != null)
                                            {
                                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                                var local = p.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                if (local != null)
                                {
                                    if (local.fldSourceInformatics == "")
                                        local.fldSourceInformatics = "0";
                                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
                                    var mnu = p.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                    if (mnu.fldInformaticesCode == "")
                                        mnu.fldInformaticesCode = "0";
                                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
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
            catch (Exception ex)
            {
                return Json(new { state = "1" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GenerateFishReport(int id, double Mablagh, string Fine, string ValueAddprice, string Price, string Year1, string OtherPrice, string MainDiscount, string FineDisCount, string ValueAddDiscount, string OtherDiscount)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var file = p.sp_CarFileSelect("fldId", id.ToString(), 1, null, "").FirstOrDefault();

            var car = p.sp_SelectCarDetils(file.fldCarID).FirstOrDefault();
            if (car != null)
            {
                var ServerDate = p.sp_GetDate().FirstOrDefault();
                int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime).Substring(0, 4));
                //string date = toYear + "/12/29";
                //if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                //    date = toYear + "/12/30";
                System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                //var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                //null, ServerDate.CurrentDateTime, _year, _Bed).ToList();
                int sal = 0, mah = 0;
                string[] Year2;
                Year2 = Year1.Split(',');

                ArrayList Years3 = new ArrayList();
                for (int i = 0; i < Year2.Count(); i++)
                {
                    Years3.Add(Year2[i]);
                }

                for (int i = 0; i < Years3.Count; i++)
                {
                    if (Convert.ToInt32(Years3[i]) == 0)
                    {
                        Years3.Remove(Years3[i]);
                        break;
                    }
                }

                int[] AvarezSal = new int[0];
                if (Years3 != null)
                {
                    AvarezSal = new int[Years3.Count];
                    for (int i = 0; i < Years3.Count; i++)
                    {
                        AvarezSal[i] = Convert.ToInt32(Years3[i]);
                    }
                }

                Mablagh += Convert.ToInt32(_Bed.Value);
                Price += Convert.ToInt32(_Bed.Value);
                if (Mablagh < 10000)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "پرونده انتخابی بدهکار نیست."
                    });
                    DirectResult result = new DirectResult();
                    return result;
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
                        string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                        int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                        int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                        int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                        if (Years3.Count > 1)
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
                        else if (Years3.Count == 1)
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
                    case 0:
                        Rounded = 1;
                        break;
                }


                Mablagh = Math.Floor(Mablagh / Rounded) * Rounded;//گرد به پایین

                string ShGhabz = "", ShPardakht = "", BarcodeText = "", ShParvande = "";

                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                var datetime = p.sp_GetDate().FirstOrDefault().CurrentDateTime;
                p.sp_PeacockeryInsert(car.fldID, datetime, bankid.fldID, "", Convert.ToInt32(Price),
                    Convert.ToInt32(Fine), Convert.ToInt32(ValueAddprice), Convert.ToInt32(OtherPrice), Convert.ToInt32(Mablagh),
                    MyLib.Shamsi.Shamsi2miladiDateTime(car.fldStartDateInsurance), datetime, Convert.ToInt32(Session["GeustId"]),
                    "", "", Convert.ToInt32(MainDiscount), Convert.ToInt32(ValueAddDiscount), Convert.ToInt32(OtherDiscount), ShGhabz, ShPardakht, _id, Convert.ToInt32(FineDisCount));
                p.sp_UpdateGuestInfoId("Peacockery", _id.Value.ToString(), Convert.ToInt32(Session["GuestInfId"]), Convert.ToInt32(Session["GeustId"]));
                if (Convert.ToInt32(Session["CountryType"]) == 5)
                {
                    var mnu = p.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (mnu.fldInformaticesCode == "")
                        mnu.fldInformaticesCode = "0";
                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                    {
                        var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                            sal = ShParvande.Length - 2;
                            if (ShParvande.Length > 8)
                            {
                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                ShParvande = ShParvande.Substring(0, 8);
                                mah = Convert.ToInt32(s);
                            }
                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                , Convert.ToInt32(Mablagh), sal, mah);
                            ShGhabz = gh.ShGhabz;
                            ShPardakht = gh.ShPardakht;
                            BarcodeText = gh.BarcodeText;
                        }
                    }
                }
                else if (Convert.ToInt32(Session["CountryType"]) == 6)
                {
                    var local = p.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (local.fldSourceInformatics == "")
                        local.fldSourceInformatics = "0";
                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                    {
                        var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                            sal = ShParvande.Length - 2;
                            if (ShParvande.Length > 8)
                            {
                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                ShParvande = ShParvande.Substring(0, 8);
                                mah = Convert.ToInt32(s);
                            }
                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                , Convert.ToInt32(Mablagh), sal, mah);
                            ShGhabz = gh.ShGhabz;
                            ShPardakht = gh.ShPardakht;
                            BarcodeText = gh.BarcodeText;
                        }
                    }
                }
                else if (Convert.ToInt32(Session["CountryType"]) == 7)
                {
                    var area = p.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (area != null)
                    {
                        if (area.fldLocalID != null)
                        {
                            var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();

                            if (local.fldSourceInformatics == "")
                                local.fldSourceInformatics = "0";
                            if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                            {
                                var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                    sal = ShParvande.Length - 2;
                                    if (ShParvande.Length > 8)
                                    {
                                        string s = ShParvande.Substring(8).PadRight(2, '0');
                                        ShParvande = ShParvande.Substring(0, 8);
                                        mah = Convert.ToInt32(s);
                                    }
                                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                        , Convert.ToInt32(Mablagh), sal, mah);
                                    ShGhabz = gh.ShGhabz;
                                    ShPardakht = gh.ShPardakht;
                                    BarcodeText = gh.BarcodeText;
                                }
                            }
                        }
                        else
                        {
                            var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                            if (mnu.fldInformaticesCode == "")
                                mnu.fldInformaticesCode = "0";
                            if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                            {
                                var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                    sal = ShParvande.Length - 2;
                                    if (ShParvande.Length > 8)
                                    {
                                        string s = ShParvande.Substring(8).PadRight(2, '0');
                                        ShParvande = ShParvande.Substring(0, 8);
                                        mah = Convert.ToInt32(s);
                                    }
                                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                        , Convert.ToInt32(Mablagh), sal, mah);
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
                    var office = p.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                    if (office != null)
                    {
                        if (office.fldAreaID != null)
                        {
                            var area = p.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                            if (area != null)
                            {
                                if (area.fldLocalID != null)
                                {
                                    var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();

                                    if (local.fldSourceInformatics == "")
                                        local.fldSourceInformatics = "0";
                                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                                , Convert.ToInt32(Mablagh), sal, mah);
                                            ShGhabz = gh.ShGhabz;
                                            ShPardakht = gh.ShPardakht;
                                            BarcodeText = gh.BarcodeText;
                                        }
                                    }
                                }
                                else
                                {
                                    var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                    if (mnu.fldInformaticesCode == "")
                                        mnu.fldInformaticesCode = "0";
                                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                                , Convert.ToInt32(Mablagh), sal, mah);
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
                            var local = p.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                            if (local != null)
                            {
                                if (local.fldSourceInformatics == "")
                                    local.fldSourceInformatics = "0";
                                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                            , Convert.ToInt32(Mablagh), sal, mah);
                                        ShGhabz = gh.ShGhabz;
                                        ShPardakht = gh.ShPardakht;
                                        BarcodeText = gh.BarcodeText;
                                    }
                                }
                            }
                            else
                            {
                                var mnu = p.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                if (mnu.fldInformaticesCode == "")
                                    mnu.fldInformaticesCode = "0";
                                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                            , Convert.ToInt32(Mablagh), sal, mah);
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

                if (Convert.ToInt32(Session["UserMnu"]) == 1)
                {
                    XmlDocument XDoc1 = new XmlDocument();
                    // Create root node.
                    XmlElement XElemRoot1 = XDoc1.CreateElement("FicheDetailByFisyear");
                    XDoc1.AppendChild(XElemRoot1);
                    XmlDocument XDoc2 = new XmlDocument();
                    // Create root node.
                    XmlElement XElemRoot2 = XDoc2.CreateElement("FicheAddAndSub");
                    XDoc2.AppendChild(XElemRoot2);
                    XmlDocument XDoc = new XmlDocument();
                    // Create root node.
                    XmlElement XElemRoot = XDoc.CreateElement("FicheDetail");
                    XDoc.AppendChild(XElemRoot);
                    int discount = 0;
                    Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                    string hesabid = "";
                    System.Xml.XmlReader xmlReader = System.Xml.XmlReader
                        .Create((Stream)new MemoryStream(System.Text.Encoding.UTF8.GetBytes("<hesabs>" + new Avarez.Hesabrayan.ServiseToRevenueSystems().AccountListRevenue(1).InnerXml.Replace("\"", "'") + "</hesabs>")));
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Node")
                        {
                            if (bankid.fldAccountNumber == xmlReader["AccountNum"].ToString())
                                hesabid = xmlReader["ID"].ToString();
                        }
                    }

                    if (hesabid != "")
                    {
                        int avarez = 0, jarime = 0, sayer = 0;
                        string takhfif = "0";
                        avarez = Convert.ToInt32(Price) + Convert.ToInt32(ValueAddprice) - Convert.ToInt32(OtherPrice);
                        jarime = Convert.ToInt32(Fine);
                        sayer = Convert.ToInt32(OtherPrice);

                        int bed = 0;
                        if (Mablagh - (avarez + jarime + sayer) != 0)
                        {
                            bed = (int)Mablagh - (avarez + jarime + sayer);
                        }
                        takhfif = MainDiscount;
                        XmlElement Xsource = XDoc.CreateElement("Node");
                        Xsource.SetAttribute("RevenueID", "198");//کد درامدی عوارض خودرو 
                        Xsource.SetAttribute("RevenueCost", avarez.ToString());
                        Xsource.SetAttribute("RevenueTaxCost", "0");
                        Xsource.SetAttribute("RevenueAvarezCost", "0");
                        Xsource.SetAttribute("RevenueTaxAvarezCost", "0");
                        Xsource.SetAttribute("RevenueBed", bed.ToString());
                        Xsource.SetAttribute("RevenueBes", "0");
                        Xsource.SetAttribute("AmountSavingBand", "0");
                        Xsource.SetAttribute("Discount", "0");
                        Xsource.SetAttribute("Discount", takhfif.ToString());
                        XElemRoot.AppendChild(Xsource);

                        if (jarime != 0)
                        {
                            XmlElement Xsource1 = XDoc.CreateElement("Node");
                            Xsource1.SetAttribute("RevenueID", "158");//کد در امدی جرائم
                            Xsource1.SetAttribute("RevenueCost", jarime.ToString());
                            Xsource1.SetAttribute("RevenueTaxCost", "0");
                            Xsource1.SetAttribute("RevenueAvarezCost", "0");
                            Xsource1.SetAttribute("RevenueTaxAvarezCost", "0");
                            Xsource1.SetAttribute("RevenueBed", "0");
                            Xsource1.SetAttribute("RevenueBes", "0");
                            Xsource1.SetAttribute("AmountSavingBand", "0");
                            Xsource1.SetAttribute("Discount", "0");

                            XElemRoot.AppendChild(Xsource1);
                        }
                        if (sayer != 0)
                        {
                            XmlElement Xsource1 = XDoc.CreateElement("Node");
                            Xsource1.SetAttribute("RevenueID", "197");//کد در امدی سایر
                            Xsource1.SetAttribute("RevenueCost", sayer.ToString());
                            Xsource1.SetAttribute("RevenueTaxCost", "0");
                            Xsource1.SetAttribute("RevenueAvarezCost", "0");
                            Xsource1.SetAttribute("RevenueTaxAvarezCost", "0");
                            Xsource1.SetAttribute("RevenueBed", "0");
                            Xsource1.SetAttribute("RevenueBes", "0");
                            Xsource1.SetAttribute("AmountSavingBand", "0");
                            Xsource1.SetAttribute("Discount", "0");

                            XElemRoot.AppendChild(Xsource1);
                        }

                        var k1 = toRevenueSystems.RegisterNewFicheByAccYearCostAndDiscount(3, 1, _id.Value.ToString(), car.fldOwnerName.ToString(),
                                 datetime, hesabid, _id.Value.ToString(),
                                 "کد ملی:" + car.fldMelli_EconomicCode.ToString() + " پلاک:" + car.fldCarPlaqueNumber.ToString() + " بابت عوارض " + arrang(AvarezSal),
                                  "", "", "", "", "", 8, 2, car.fldCarAccountName + " " + car.fldCarSystemName + " " + car.fldCarModel + " " + car.fldCarClassName, car.fldVIN.ToString(),
                                 (int)Mablagh, 0, discount, XDoc, XDoc1, XDoc2);
                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal avarez: "
                            + k1 + "-" + _id.Value.ToString() + "\n");
                    }
                }
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
                case 0:
                    Rounded = 1;
                    break;
            }


            Session["showmoney"] = Convert.ToInt32(Math.Floor(showmoney / Rounded) * Rounded);//گرد به پایین  
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
        public ActionResult GoToOnlinePay1(decimal Amount, int CarId,string CarFileId, int fldFine, int fldValueAddPrice, int fldPrice, string Years, int Bed, int fldOtherPrice,
            int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            long peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
            string shGabz = "", Shpardakht = "";
            if (peackokeryid == 0)
            {
                GenerateFishReport(Convert.ToInt32(CarFileId), Convert.ToDouble(Amount), fldFine.ToString(), fldValueAddPrice.ToString(), fldPrice.ToString(), Years, fldOtherPrice.ToString(), fldMainDiscount.ToString(), fldFineDiscount.ToString(), fldValueAddDiscount.ToString(), fldOtherDiscount.ToString());               
                peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
            }
            if (peackokeryid != 0)
            {
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                if (fish.fldShGhabz != "" && fish.fldShPardakht != "")
                {
                    shGabz = fish.fldShGhabz;
                    shGabz = shGabz.PadLeft(13, '0');
                    Shpardakht = fish.fldShPardakht;
                }
            }
            return Json(new { shGabz = shGabz, Shpardakht = Shpardakht }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GoToOnlinePay(decimal Amount, int CarId, long CarFileId, int BankId, int fldFine, int fldValueAddPrice, int fldPrice, string Years, int? Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            //Amount = 1000;
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_SelectCarDetils(CarId).FirstOrDefault();
            string Tax = "";
            long peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));

            if (peackokeryid == 0)
            {
                GenerateFishReport(Convert.ToInt32(CarFileId), Convert.ToDouble(Amount), fldFine.ToString(), fldValueAddPrice.ToString(), fldPrice.ToString(), Years, fldOtherPrice.ToString(), fldMainDiscount.ToString(), fldFineDiscount.ToString(), fldValueAddDiscount.ToString(), fldOtherDiscount.ToString());
                peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
            }
            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
            if (BankId != 17 && BankId != 15 && BankId != 30 && BankId != 20)
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["GeustId"]), "", "", Amount, Convert.ToInt32(Session["UserMnu"]), null);

            if (BankId != 15 && BankId != 30)
            {
                Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                Session["OnlinefishId"] = fish.fldID;
                Amount = fish.fldShowMoney;
            }
            else//اگر بانک پارسیان بود
            {
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["GeustId"]), "", "", Amount, Convert.ToInt32(Session["UserMnu"]), null);
                var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                Tax = q.fldTemporaryCode;
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                Session["OnlinefishId"] = fish.fldID;
                Amount = fish.fldShowMoney;
            }
            if (BankId == 17 || BankId == 20)
            {

                if (peackokeryid != 0)
                {
                    var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                    if (fish.fldShGhabz != "" && fish.fldShPardakht != "")
                    {
                        Amount = fish.fldShowMoney;
                        p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["GeustId"]), fish.fldShGhabz + "|" + fish.fldShPardakht, "", Amount, Convert.ToInt32(Session["UserMnu"]), null);
                        var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["GeustId"]), "").FirstOrDefault();
                        Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
                        Session["shGhabz"] = fish.fldShGhabz;
                        Session["shPardakht"] = fish.fldShPardakht;
                        Session["OnlinefishId"] = fish.fldID;

                    }
                    else
                    {
                        Session["shGhabz"] = null;
                        Session["shPardakht"] = null;
                        Session["OnlinefishId"] = null;
                        return null;
                    }
                }
            }
            else if (Convert.ToInt32(Session["UserMnu"]) == 1)
            {
                XmlDocument XDoc1 = new XmlDocument();
                // Create root node.
                XmlElement XElemRoot1 = XDoc1.CreateElement("FicheDetailByFisyear");
                XDoc1.AppendChild(XElemRoot1);
                XmlDocument XDoc2 = new XmlDocument();
                // Create root node.
                XmlElement XElemRoot2 = XDoc2.CreateElement("FicheAddAndSub");
                XDoc2.AppendChild(XElemRoot2);
                XmlDocument XDoc = new XmlDocument();
                // Create root node.
                XmlElement XElemRoot = XDoc.CreateElement("FicheDetail");
                XDoc.AppendChild(XElemRoot);
                int discount = 0;
                Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                string hesabid = "";
                System.Xml.XmlReader xmlReader = System.Xml.XmlReader
                    .Create((Stream)new MemoryStream(System.Text.Encoding.UTF8.GetBytes("<hesabs>" + new Avarez.Hesabrayan.ServiseToRevenueSystems().AccountListRevenue(1).InnerXml.Replace("\"", "'") + "</hesabs>")));
                var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Node")
                    {
                        if (bankid.fldAccountNumber == xmlReader["AccountNum"].ToString())
                            hesabid = xmlReader["ID"].ToString();
                    }
                }

                if (hesabid != "")
                {
                    int avarez = 0, jarime = 0, sayer = 0;
                    string takhfif = "0";
                    avarez = fldPrice + fldValueAddPrice - fldOtherPrice;
                    jarime = Convert.ToInt32(fldFine);
                    sayer = fldOtherPrice;
                    takhfif = fldMainDiscount.ToString();
                    XmlElement Xsource = XDoc.CreateElement("Node");
                    Xsource.SetAttribute("RevenueID", "198");//کد درامدی عوارض خودرو 
                    Xsource.SetAttribute("RevenueCost", avarez.ToString());
                    Xsource.SetAttribute("RevenueTaxCost", "0");
                    Xsource.SetAttribute("RevenueAvarezCost", "0");
                    Xsource.SetAttribute("RevenueTaxAvarezCost", "0");
                    Xsource.SetAttribute("RevenueBed", "0");
                    Xsource.SetAttribute("RevenueBes", "0");
                    Xsource.SetAttribute("AmountSavingBand", "0");
                    Xsource.SetAttribute("Discount", takhfif.ToString());
                    XElemRoot.AppendChild(Xsource);

                    if (jarime != 0)
                    {
                        XmlElement Xsource1 = XDoc.CreateElement("Node");
                        Xsource1.SetAttribute("RevenueID", "158");//کد در امدی جرائم
                        Xsource1.SetAttribute("RevenueCost", jarime.ToString());
                        Xsource1.SetAttribute("RevenueTaxCost", "0");
                        Xsource1.SetAttribute("RevenueAvarezCost", "0");
                        Xsource1.SetAttribute("RevenueTaxAvarezCost", "0");
                        Xsource1.SetAttribute("RevenueBed", "0");
                        Xsource1.SetAttribute("RevenueBes", "0");
                        Xsource1.SetAttribute("AmountSavingBand", "0");
                        Xsource1.SetAttribute("Discount", "0");

                        XElemRoot.AppendChild(Xsource1);
                    }
                    if (sayer != 0)
                    {
                        XmlElement Xsource1 = XDoc.CreateElement("Node");
                        Xsource1.SetAttribute("RevenueID", "197");//کد در امدی سایر
                        Xsource1.SetAttribute("RevenueCost", sayer.ToString());
                        Xsource1.SetAttribute("RevenueTaxCost", "0");
                        Xsource1.SetAttribute("RevenueAvarezCost", "0");
                        Xsource1.SetAttribute("RevenueTaxAvarezCost", "0");
                        Xsource1.SetAttribute("RevenueBed", "0");
                        Xsource1.SetAttribute("RevenueBes", "0");
                        Xsource1.SetAttribute("AmountSavingBand", "0");
                        Xsource1.SetAttribute("Discount", "0");

                        XElemRoot.AppendChild(Xsource1);
                    }
                    var splityear = Years.Split(',');
                    ArrayList Yearss = new ArrayList();
                    for (int i = 0; i < splityear.Count(); i++)
                    {
                        Yearss.Add(splityear[i]);
                    }

                    for (int i = 0; i < Yearss.Count; i++)
                    {
                        if (Convert.ToInt32(Yearss[i]) == 0)
                        {
                            Yearss.Remove(Yearss[i]);
                            break;
                        }
                    }

                    int[] AvarezSal = new int[0];
                    if (Yearss != null)
                    {
                        AvarezSal = new int[Yearss.Count];
                        for (int i = 0; i < Yearss.Count; i++)
                        {
                            AvarezSal[i] = Convert.ToInt32(Yearss[i]);
                        }
                    }

                    var k1 = toRevenueSystems.RegisterNewFicheByAccYearCostAndDiscount(3, 1, Session["OnlinefishId"].ToString(), car.fldOwnerName.ToString(),
                             p.sp_GetDate().FirstOrDefault().CurrentDateTime, hesabid, Session["OnlinefishId"].ToString(),
                             "کد ملی:" + car.fldMelli_EconomicCode.ToString() + " پلاک:" + car.fldCarPlaqueNumber.ToString() + " بابت عوارض " + arrang(AvarezSal),
                              "", "", "", "", "", 8, 2, car.fldCarAccountName + " " + car.fldCarSystemName + " " + car.fldCarModel + " " + car.fldCarClassName, car.fldVIN.ToString(),
                             (int)Amount, 0, discount, XDoc, XDoc1, XDoc2);
                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal avarez: "
                        + k1 + "-" + Session["OnlinefishId"].ToString() + "\n");
                }
            }
            p.sp_UpdateGuestInfoId("OnlinePayments", _id.Value.ToString(), Convert.ToInt32(Session["GuestInfId"]), Convert.ToInt32(Session["GeustId"]));
            p.sp_OnlineTemporaryCodePaymentsUpdate(Convert.ToInt32(_id.Value), Tax, Amount, Convert.ToInt32(Session["GeustId"]), "");
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
            //                return Json(new{Msg="مهلت تراکنش به پایان رسیده است.",MsgTitle ="خطا", Er = 1}, JsonRequestBehavior.AllowGet);
            //            case -7:
            //                return Json(new{Msg="مجوز استفاده از این سرویس را ندارید.",MsgTitle ="خطا", Er = 1}, JsonRequestBehavior.AllowGet);
            //            case -8:
            //                return Json(new{Msg="کاربر شما غیر فعال شده است.",MsgTitle ="خطا", Er = 1}, JsonRequestBehavior.AllowGet);
            //            case -10:
            //                return Json(new { Msg = "اعتبار شما کافی نیست.", MsgTitle = "خطا", Er = 1 }, JsonRequestBehavior.AllowGet);
            //        }
            //    }
            //}
            Session["Amount"] =  Amount;
            Session["Tax"] = Tax;
            Session["ReturnUrl"] = "/NewVer/First/Index";
            string URL = "";

            if (BankId == 20)
            {
                URL = "NewVer/CityBank_New";
            }
            else if (BankId == 1)
            {
                URL = "NewVer/MeliBank_New";
            }
            else if (BankId == 2)
            {
                URL = "NewVer/TejaratBank_New";
            }
            else if (BankId == 15)
            {
                URL = "NewVer/Parsian_New";
            }
            else if (BankId == 17)
            {
                URL = "NewVer/Saman_New";
            }
            else if (BankId == 30)
            {
                URL = "NewVer/Parsiaan_New";
            }
            else if (BankId == 31)
            {
                URL = "NewVer/Samaan_New";
            }
            //return RedirectToAction("Index", URL);
            return Json("~/" + URL + "/Index", JsonRequestBehavior.AllowGet);
        }
        private void sort(int[] a)
        {
            if (a.Length == 0) return;
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = i + 1; j < a.Length; j++)
                {
                    if (a[i] > a[j])
                    {
                        int temp = a[i];
                        a[i] = a[j];
                        a[j] = temp;
                    }
                }
            }
        }
        private string arrang(int[] a)
        {
            sort(a);
            int i;
            if (a.Length == 0) return "()";
            int start = a[0];
            string result = a[0].ToString();
            for (i = 1; i < a.Length - 1; i++)
            {

                if (a[i] - start > 1)
                {
                    result += "," + a[i].ToString();
                }
                else if (a[i + 1] - a[i] > 1)
                {
                    result += "-" + a[i].ToString();
                }
                start = a[i];
            }
            if (a.Length > 1)
            {
                if (a[i] - a[i - 1] > 1)
                {
                    result += "," + a[i].ToString();
                }
                else
                {
                    result += "-" + a[i].ToString();
                }
                start = a[i];
            }
            return result;
        }
        public FileContentResult DownloadReceipt(int id)
        {
            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter resid = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter carDitail = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
            var q = car.rpt_Receipt(id, 1).FirstOrDefault();
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();
            carDitail.Fill(dt.sp_SelectCarDetils, Convert.ToInt32(q.fldCarId));
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
            resid.Fill(dt.rpt_Receipt, id, 1);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_Resid.frx");
            Report.RegisterData(dt, "complicationsCarDBDataSet");
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
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);
            return File(stream.ToArray(), MimeType.Get(".pdf"), "Resid.pdf");
        }
    }
}
