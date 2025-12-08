using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez
{
    public class SmsSender
    {

        /// <summary>
        /// جهت ارسال پیامک
        /// </summary>
        /// <param name="MunId">آی دی شهرداری جهت ارسال پیامک</param>
        /// <param name="SentType"> نوع ارسال: 1->پاسخ به استعلام   2-> پس از تشکیل پرونده   3-> پس از وصول عوارض 4-> پس از صدور فیش </param>
        /// <param name="carFileId">کد پرونده</param>
        public void SendMessage(int MunId, int SentType, long CarFileid, int Price, string Shgabz, string shPardakht, string BedYear)
        {
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var countrydiv = Car.sp_GET_IDCountryDivisions(5, MunId).FirstOrDefault();
            var haveSmsPanel = Car.sp_SMSSettingSelect("fldCountryDivisionId", countrydiv.CountryDivisionId.ToString(), 0, 1, "").FirstOrDefault();
            if (haveSmsPanel != null)
            {
                var SmsSenderSetting = Car.Sp_SmsSendingSettingSelect("fldCountryDivisionId", countrydiv.CountryDivisionId.ToString(), 0, 1, "").Where(k => k.fldType == SentType && k.fldState == true).FirstOrDefault();
                if (SmsSenderSetting != null)
                {
                    var carfile = Car.sp_CarFileSelect("fldId", CarFileid.ToString(), 1, 1, "").FirstOrDefault();
                    var owner = Car.sp_OwnerSelect("fldId", carfile.fldOwnerID.ToString(), 1, 1, "").FirstOrDefault();
                    if (SmsSenderSetting.fldState && CheckMobileNumber(owner.fldMobile))
                    {
                        //var smsSenderSetting=Car.Sp_SmsSendingSettingSelect(""
                        SmsPanel.RasaSMSPanel_Send Sms = new SmsPanel.RasaSMSPanel_Send();
                        Sms.SendMessage(haveSmsPanel.fldUserName, haveSmsPanel.fldPassword,
                            new string[] { owner.fldMobile }, SmsGenerator(SmsSenderSetting.fldText, CarFileid, Convert.ToInt32(Price), Shgabz, shPardakht, BedYear),
                            1, haveSmsPanel.fldLineNumber);
                    }
                }
            }
        }
        public void SendMobileVerify(int MunId, long OwnerId)
        {
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var countrydiv = Car.sp_GET_IDCountryDivisions(5, MunId).FirstOrDefault();
            var haveSmsPanel = Car.sp_SMSSettingSelect("fldCountryDivisionId", countrydiv.CountryDivisionId.ToString(), 0, 1, "").FirstOrDefault();
            if (haveSmsPanel != null)
            {
                var subSetting = Car.sp_SubSettingSelect("fldCountryDivisionsID", countrydiv.CountryDivisionId.ToString(), 0, 1, "").FirstOrDefault();
                if (subSetting.fldMobileVerify)
                {
                    var mun = Car.sp_MunicipalitySelect("fldid", MunId.ToString(), 0, 1, "").FirstOrDefault();
                    var owner = Car.sp_OwnerSelect("fldId", OwnerId.ToString(), 1, 1, "").FirstOrDefault();
                    if (CheckMobileNumber(owner.fldMobile))
                    {
                        //var smsSenderSetting=Car.Sp_SmsSendingSettingSelect(""
                        SmsPanel.RasaSMSPanel_Send Sms = new SmsPanel.RasaSMSPanel_Send();
                        Sms.SendMessage(haveSmsPanel.fldUserName, haveSmsPanel.fldPassword,
                            new string[] { owner.fldMobile }, "مودی گرامی کد فعال سازی شما:" + OwnerId.ToString() + " می باشد.\n سامانه جامع عوارض خودرو " + mun.fldName,
                            1, haveSmsPanel.fldLineNumber);
                    }
                }
            }
        }
        public string SendVerifyMessage(string fldCountryDivisionsID, string Mobile, string VerifyCode)
        {
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var haveSmsPanel = Car.sp_SMSSettingSelect("fldCountryDivisionId", fldCountryDivisionsID, 0, 1, "").FirstOrDefault();
            if (haveSmsPanel != null)
            {
                if (CheckMobileNumber(Mobile))
                {
                    SmsPanel.RasaSMSPanel_Send Sms = new SmsPanel.RasaSMSPanel_Send();
                    Sms.SendMessage(haveSmsPanel.fldUserName, haveSmsPanel.fldPassword,
                        new string[] { Mobile }, "کد بازسازی کلمه عبور سامانه عوارض خودرو: " + VerifyCode, 1, haveSmsPanel.fldLineNumber);
                    return "پیامکی حاوی کد بازسازی رمزعبور برای شما ارسال می گردد.";
                }
                else
                    return "شماره مورد نظر نامعتبر است.";
            }
            else
                return "لطفا با مدیر سیستم تماس بگیرید.";
        }
        public bool CheckMobileNumber(string MobileNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(MobileNumber, "(^(09|9)[13][0-9]\\d{7}$)");
        }

        string SmsGenerator(String pattern,long CarFileId,int Price,string Shgabz,string shPardakht,string BedYear)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var carfile = car.sp_CarFileSelect("fldId", CarFileId.ToString(), 0, 1, "").FirstOrDefault();

            string s1 = "";
            if (pattern != "")
            {
                string[] p = pattern.Split('*');
                for (var i = 0; i < p.Length; i++)
                {
                    var temp = p[i];
                    switch (temp)
                    {
                        case "CarTip":
                            s1 = s1 + carfile.fldCarModelName;//"داخلی سواری شخصی ";
                            break;
                        case "CarClass":
                            s1 = s1 + carfile.fldCarClassName;//"پژو 206 عادی ";
                            break;
                        case "Owner":
                            s1 = s1 + carfile.fldOwnerName;//"جواد ربیعی ";
                            break;
                        case "Pelaqe":
                            s1 = s1 + carfile.fldPlaqueNumber;//"ایران|44|456~12 ";
                            break;
                        case "Price":
                            s1 = s1 + Price.ToString("#,###") + "ریال ";//"260،000 ریال ";
                            break;
                        case "MotorNum":
                            s1 = s1 + carfile.fldMotorNumber;//"1554654 ";
                            break;
                        case "ShasiNum":
                            s1 = s1 + carfile.fldShasiNumber;//"1654654654 ";
                            break;
                        case "BedYear":
                            s1 = s1 + BedYear;//"90-92 ";
                            break;
                        case "ShGhabz":
                            s1 = s1 + Shgabz;//"51565 ";
                            break;
                        case "ShPardakht":
                            s1 = s1 + shPardakht;//"5584 ";
                            break;
                        case "ShFish":
                            s1 = s1 + shPardakht;//"651235 ";
                            break;
                        default:
                            s1 = s1 + p[i] + "";
                            break;
                    }
                }
            }
            return s1;
        }
    }
}