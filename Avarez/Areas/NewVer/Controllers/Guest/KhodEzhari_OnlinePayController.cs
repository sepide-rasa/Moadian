using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Avarez.Controllers.Users;
using System.Text.RegularExpressions;
using System.Globalization;
using Avarez.Models;
using System.Web.Configuration;
using System.Collections;
using System.Xml;

namespace Avarez.Areas.NewVer.Controllers.Guest
{
    public class KhodEzhari_OnlinePayController : Controller
    {
        //
        // GET: /NewVer/KhodEzhari_OnlinePay/

        public ActionResult Index(long? CarId, long PelakId, long MalekId,string CarFileId)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.CarId = CarId;
            PartialView.ViewBag.PelakId = PelakId;
            PartialView.ViewBag.MalekId = MalekId;
            PartialView.ViewBag.CarFileId = CarFileId;
            return PartialView;
        }
        public ActionResult Fill(int CarId)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_SelectCarDetils(CarId).FirstOrDefault();
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
        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_PictureSelect("fldBankPic", id.ToString(), 30, null, "").FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldPic != null)
                {
                    return File((byte[])pic.fldPic, "jpg");
                }
            }
            return null;

        }
        public ActionResult GoToOnlinePay1(decimal Amount, int CarId, string CarFileId, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice,
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
        public ActionResult GoToOnlinePay(decimal Amount, int CarId, long CarFileId, int BankId, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int? Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
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
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", null, "", "", Amount, Convert.ToInt32(Session["UserMnu"]), Convert.ToInt32(Session["UserGeust"]));

            if (BankId != 15 && BankId != 30)
            {
                Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                Session["OnlinefishId"] = fish.fldID;
                Amount = fish.fldShowMoney;
            }
            else//اگر بانک پارسیان بود
            {
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", null, "", "", Amount, Convert.ToInt32(Session["UserMnu"]), Convert.ToInt32(Session["UserGeust"]));
                var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, null, "").FirstOrDefault();
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
                        p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", null, fish.fldShGhabz + "|" + fish.fldShPardakht, "", Amount, Convert.ToInt32(Session["UserMnu"]), Convert.ToInt32(Session["UserGeust"]));
                        var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, null, "").FirstOrDefault();
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
                    var splityear = (ArrayList)Session["Year"];
                    ArrayList Yearss = new ArrayList();
                    for (int i = 0; i < splityear.Count; i++)
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

            p.sp_OnlineTemporaryCodePaymentsUpdate(Convert.ToInt32(_id.Value), Tax, Amount, null, "");

            Session["Amount"] = Amount;
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
            //return RedirectToAction("Index", URL);
            return Json("~/" + URL + "/Index", JsonRequestBehavior.AllowGet);
        }
        public ActionResult calc(string CarId, string carFileid)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldCarId", CarId,1, 1,"").FirstOrDefault();
            int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4));
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";
            //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
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
                            Er = 1,
                            MsgTitle = "خطا",
                            Msg = msg1
                        }, JsonRequestBehavior.AllowGet);

                    case Transaction.TransactionResult.NotSharj:
                        msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
                        return Json(new
                        {
                            Er = 1,
                            MsgTitle = "خطا",
                            Msg = msg1
                        }, JsonRequestBehavior.AllowGet);

                }
            }
            //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
            //    null, DateTime.Now, null, _year, _Bed).ToList();
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
            //Session["year"] = _year.Value.ToString();
            //Session["bed"] = _Bed.Value.ToString();

           // Session.Remove("fldCarID3");
            if (_year.ToString() == "")
            {
                int? mablagh = 0;
                int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0, fldMainDiscount = 0, fldFineDiscount = 0,
                    fldValueAddDiscount = 0, fldOtherDiscount = 0;
                ArrayList Years = new ArrayList();
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
                }

                int sal = 0, mah = 0;
                Session["Year"] = Years;
                //mablagh += Convert.ToInt32(_Bed.Value);
                //fldPrice += Convert.ToInt32(_Bed.Value);
                /*Session["mablagh"] = mablagh;
                Session["Fine"] = fldFine;
                Session["ValueAddPrice"] = fldValueAddPrice;
                Session["Price"] = fldPrice;
                
                Session["OtherPrice"] = fldOtherPrice;
                Session["fldMainDiscount"] = fldMainDiscount;
                Session["fldFineDiscount"] = fldFineDiscount;
                Session["fldValueAddDiscount"] = fldValueAddDiscount;
                Session["fldOtherDiscount"] = fldOtherDiscount;*/
                if (mablagh < 1000)
                {
                    mablagh = 0;
                    //bedehi = null;
                }

                string shGhabz = "", ShParvande = "",
                    shPardakht = "",
                    barcode = "";
                ////if (Convert.ToInt32(Session["CountryType"]) == 5)
                ////{
                //    var mnu = m.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, null, "").FirstOrDefault();
                //    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                //    {
                //        var Divisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                //        if (Divisions != null)
                //        {
                //            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, null, "").FirstOrDefault();
                //            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                //            sal = ShParvande.Length - 2;
                //            if (ShParvande.Length > 8)
                //            {
                //                string s = ShParvande.Substring(8).PadRight(2, '0');
                //                ShParvande = ShParvande.Substring(0, 8);
                //                mah = Convert.ToInt32(s);
                //            }
                //            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                //                , Convert.ToInt32(mablagh), sal, mah);
                //            shGhabz = gh.ShGhabz;
                //            shPardakht = gh.ShPardakht;
                //            barcode = gh.BarcodeText;
                //        }
                //    }
                ////}
                
                //چک تایید شده و نشده*************************
                var DisableMafasa = 0;
                if (carFileid != null)
                {
                    var c = m.sp_CarFileSelect("fldId", carFileid, 0, null, "").FirstOrDefault();
                    if (c.fldAccept == false)
                        DisableMafasa = 1;
                    else
                    {
                        var cc = m.sp_CarExperienceSelect("fldCarFileID", carFileid, 0, null, "").ToList();
                        foreach (var item in cc)
                        {
                            if (item.fldAccept == false)
                            {
                                DisableMafasa = 1;
                                break;
                            }
                        }
                    }
                }
                return Json(new
                {
                    Er = 0,
                    bedehi = bedehi,
                    mablagh = mablagh,
                    shGhabz = shGhabz,
                    shPardakht = shPardakht,
                    barcode = barcode,
                    Msg = "",
                    fldFine = fldFine,
                    fldValueAddPrice = fldValueAddPrice,
                    fldPrice = fldPrice,
                    Years = Years,
                    //Bed = Convert.ToInt32(_Bed.Value),
                    fldOtherPrice = fldOtherPrice,
                    fldMainDiscount = fldMainDiscount,
                    fldFineDiscount = fldFineDiscount,
                    fldValueAddDiscount = fldValueAddDiscount,
                    fldOtherDiscount = fldOtherDiscount,
                    DisableMafasa = DisableMafasa
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
                    Er = 1,
                    MsgTitle = "خطا",
                    Msg = msg
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult FishReport(int carid)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.carid = carid;
            return PartialView;
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
        public ActionResult GenerateFishReport(int id, double Mablagh, string Fine, string ValueAddprice, string Price, ArrayList Year1, string OtherPrice, string MainDiscount, string FineDisCount, string ValueAddDiscount, string OtherDiscount)
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

                ArrayList Years3 = new ArrayList();
                for (int i = 0; i < Year1.Count; i++)
                {
                    Years3.Add(Year1[i]);
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
                    MyLib.Shamsi.Shamsi2miladiDateTime(car.fldStartDateInsurance), datetime, null,
                    "", "", Convert.ToInt32(MainDiscount), Convert.ToInt32(ValueAddDiscount), Convert.ToInt32(OtherDiscount), ShGhabz, ShPardakht, _id, Convert.ToInt32(FineDisCount));
                //if (Convert.ToInt32(Session["CountryType"]) == 5)
                //{
                var mnu = p.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, null, "").FirstOrDefault();
                if (mnu.fldInformaticesCode == "")
                    mnu.fldInformaticesCode = "0";
                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                {
                    var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    if (Divisions != null)
                    {
                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, null, "").FirstOrDefault();
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
                //}
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
                        avarez = Convert.ToInt32(Price) + Convert.ToInt32(ValueAddprice) - Convert.ToInt32(OtherPrice);
                        jarime = Convert.ToInt32(Fine);
                        sayer = Convert.ToInt32(OtherPrice);

                        int bed = 0;
                        if (Mablagh - (avarez + jarime + sayer) != 0)
                        {
                            bed = (int)Mablagh - (avarez + jarime + sayer);
                        }
                        string takhfif = "0";
                        takhfif = MainDiscount.ToString();
                        XmlElement Xsource = XDoc.CreateElement("Node");
                        Xsource.SetAttribute("RevenueID", "198");//کد درامدی عوارض خودرو 
                        Xsource.SetAttribute("RevenueCost", avarez.ToString());
                        Xsource.SetAttribute("RevenueTaxCost", "0");
                        Xsource.SetAttribute("RevenueAvarezCost", "0");
                        Xsource.SetAttribute("RevenueTaxAvarezCost", "0");
                        Xsource.SetAttribute("RevenueBed", bed.ToString());
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
    }
}
