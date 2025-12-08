using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Avarez
{
    /// <summary>
    /// Summary description for UssdService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UssdService : System.Web.Services.WebService
    {
        int CountryCode = 2;
        [WebMethod]
        public string GetMablagh(string Vin)
        {
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var Car = p.sp_CarSelect("fldVIN", Vin, 0, 0, "").FirstOrDefault();
                if (Car == null)
                    return "مودی گرامی، اطلاعات شما در سامانه موجود نمی باشد، برای تکمیل پرونده به دفاتر پیشخوان دولت مراجعه فرمایید.";
                else
                {
                    if (calc(Car.fldID) == null)
                        return "تعرفه عوارض سال های مورد نظر تعریف نشده است.";
                    else
                    {
                        return "مودی گرامی مبلغ عوارض خودرو شما " + calc(Car.fldID) + " ریال می باشد، برای کسب اطلاعات بیشتر به دفاتر پیشخوان دولت مراجعه فرمایید.";
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //[WebMethod]
        //public string GetShenaseGhabz(string Vin)
        //{
        //    try
        //    {
        //        Models.cartaxEntities p = new Models.cartaxEntities();
        //        var Car = p.sp_CarSelect("fldVIN", Vin, 0, 0, "").FirstOrDefault();
        //        if (Car == null)
        //            return "اطلاعات خودروی موردنظر یافت نشد.";
        //        else
        //        {
        //            long CarId = Car.fldID;
        //            if (calc(CarId) == null)
        //                return "تعرفه عوارض سال های مورد نظر تعریف نشده است.";
        //            else
        //            {
        //                long peackokeryid = FindPeackokery(CarId, Convert.ToInt32(calc(CarId)));
        //                string shGabz = "", Shpardakht = "";
        //                if (peackokeryid != 0)
        //                {
        //                    var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
        //                    if (fish.fldShGhabz != "" && fish.fldShPardakht != "")
        //                    {
        //                        shGabz = fish.fldShGhabz;
        //                        Shpardakht = fish.fldShPardakht;
        //                    }
        //                }
        //                return "شناسه قبض: " + shGabz + " و شناسه پرداخت: " + Shpardakht;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        public long FindPeackokery(long carid, int showmoney)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var SubSetting = p.sp_UpSubSettingSelect(5, CountryCode).FirstOrDefault();
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


            var money = Convert.ToInt32(Math.Floor(showmoney / Rounded) * Rounded);//گرد به پایین  
            var q = p.sp_SelectExistPeacockery(carid, money).FirstOrDefault();
            if (q != null)
                if (q.PeacockeryId != null)
                {
                    var t = p.sp_PeacockerySelect("fldId", q.PeacockeryId.ToString(), 1, 1, "").FirstOrDefault();
                    var bankid = p.sp_UpAccountBankSelect(5, CountryCode).FirstOrDefault();
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
        public int? calc(long CarId)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldCarId", CarId.ToString(), 1, 1,"").FirstOrDefault();
            int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4));
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";
            //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));

            //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), 5, CountryCode, null,
            //    null, DateTime.Now, 1, _year, _Bed).ToList();
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
                //mablagh += Convert.ToInt32(_Bed.Value);
                //fldPrice += Convert.ToInt32(_Bed.Value);
                if (mablagh < 10000)
                {
                    mablagh = 0;
                    //bedehi = null;
                }

                string shGhabz = "", ShParvande = "",
                    shPardakht = "",
                    barcode = "";
                //if (Convert.ToInt32(Session["CountryType"]) == 5)
                //{
                var mnu = m.sp_MunicipalitySelect("fldId", CountryCode.ToString(), 0, 1, "").FirstOrDefault();
                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                    {
                        var Divisions = m.sp_GET_IDCountryDivisions(5, CountryCode).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
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
                //}


                return mablagh;
            }
            else
            {
                return null;
            }
        }
    }
}
