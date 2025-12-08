
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
using System.Configuration;

namespace Avarez.Areas.NewVer.Controllers.Guest
{
    public class KhodEzhariController : Controller
    {
        //
        // GET: /NewVer/KhodEzhari/

        public ActionResult Index(long? MalekId)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var u = Car.sp_GuestUserSelect("fldId", Session["UserGeust"].ToString(), "", 0).FirstOrDefault();
            if (MalekId == 0 && Session["UserGeust"] != null)
            {
                var ow = Car.sp_OwnerSelect("fldMelli_EconomicCode", u.fldNationalCode, 0, null, null).FirstOrDefault();
                if (ow != null)
                    MalekId = ow.fldID;
                else
                {
                    PartialView.ViewBag.MelliCode = u.fldNationalCode;
                    PartialView.ViewBag.Mobile = u.fldMobile;
                    PartialView.ViewBag.Type = u.fldType;
                    PartialView.ViewBag.fldTarikhTavalod = u.fldTarikhTavalod;
                }
            }
            PartialView.ViewBag.Malekid = MalekId;
            PartialView.ViewBag.Type = u.fldType;
            PartialView.ViewBag.fldTarikhTavalod = u.fldTarikhTavalod;
            return PartialView;
        }
        public ActionResult SaveMalek(Models.sp_OwnerSelect Owner)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            var Msg = "";
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                int chck = 1;
                bool checkE = true;
                if (Owner.fldDesc == null)
                    Owner.fldDesc = "";
                if (Owner.fldEmail == null)
                    Owner.fldEmail = "";
                if (Owner.fldOwnerType == 1)
                    chck = checks(Owner.fldMelli_EconomicCode);
                if (Owner.fldEmail != "")
                    checkE = checkEmail(Owner.fldEmail);
                if (chck == 1 && checkE)
                {
                    var ow = Car.sp_OwnerSelect("fldMelli_EconomicCode", Owner.fldMelli_EconomicCode, 0,
                              null, null).FirstOrDefault();
                    if (Owner.fldID == 0)
                    {//ثبت رکورد جدید
                           
                            if (ow == null)
                            {
                                Car.sp_OwnerInsert(Owner.fldName, Owner.fldMelli_EconomicCode, Owner.fldOwnerType,
                                    Owner.fldEmail, Owner.fldMobile, Owner.fldAddress, Owner.fldPostalCode,
                                    null, Owner.fldDesc, null, Owner.fldType, MyLib.Shamsi.Shamsi2miladiDateTime(Owner.fldDateShamsi), false);

                                var q = Car.sp_OwnerSelect("fldMelli_EconomicCode", Owner.fldMelli_EconomicCode, 30,
                                null, null).FirstOrDefault();

                                SmsSender sms = new SmsSender();
                                sms.SendMobileVerify(Convert.ToInt32(Session["UserMnu"]), q.fldID);

                                return Json(new
                                {
                                    Msg = "ذخیره با موفقیت انجام شد.",
                                    MsgTitle = "ذخیره موفق",
                                    Err = 0,
                                    MalekID = q.fldID
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                Msg = "کد ملی وارد شده تکراری است.";
                                if (Owner.fldOwnerType == 0)
                                    Msg = "شناسه ملی وارد شده تکراری است.";
                                return Json(new
                                {
                                    Msg = Msg,
                                    MsgTitle = "خطا",
                                    Err = 1,
                                    MalekID = 0
                                }, JsonRequestBehavior.AllowGet);
                            }
                    }
                    else
                    {//ویرایش رکورد ارسالی
                        if (ow != null && Owner.fldID != ow.fldID)
                        {
                            Msg = "کد ملی وارد شده تکراری است.";
                            if (Owner.fldOwnerType == 0)
                                Msg = "شناسه ملی وارد شده تکراری است.";
                            return Json(new
                            {
                                Msg = Msg,
                                MsgTitle = "خطا",
                                Err = 1,
                                id = 0
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            Car.sp_OwnerUpdate(Owner.fldID, Owner.fldName, Owner.fldMelli_EconomicCode, Owner.fldOwnerType,
                                Owner.fldEmail, Owner.fldMobile, Owner.fldAddress, Owner.fldPostalCode,
                                null, Owner.fldDesc,null, MyLib.Shamsi.Shamsi2miladiDateTime(Owner.fldDateShamsi),Owner.fldType);
                            return Json(new
                            {
                                Msg = "ویرایش با موفقیت انجام شد.",
                                MsgTitle = "ویرایش موفق",
                                Err = 0,
                                MalekID = Owner.fldID
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    if (!checkE)
                        return Json(new
                        {
                            Msg = "ایمیل وارد شده معتبر نمی باشد.",
                            MsgTitle = "خطا",
                            Err = 1,
                            MalekID = 0
                        }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new
                        {
                            Msg = "کد ملی وارد شده معتبر نمی باشد.",
                            MsgTitle = "خطا",
                            Err = 1,
                            MalekID = 0
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException!= null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, null, x.Message, DateTime.Now, null);
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }

        bool invalid = false;
        public bool checkEmail(string Email)
        {

            if (String.IsNullOrEmpty(Email))
                invalid = false;

            else
            {
                Email = Regex.Replace(Email, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);

                invalid = Regex.IsMatch(Email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);
            }
            return invalid;
        }
        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
        public int checks(string codec)
        {
            char[] chArray = codec.ToCharArray();
            int[] numArray = new int[chArray.Length];
            string x = codec;
            switch (x)
            {
                case "0000000000":
                case "1111111111":
                case "2222222222":
                case "3333333333":
                case "4444444444":
                case "5555555555":
                case "6666666666":
                case "7777777777":
                case "8888888888":
                case "9999999999":
                case "0123456789":
                case "9876543210":

                    return 0;
                    break;
            }
            for (int i = 0; i < chArray.Length; i++)
            {
                numArray[i] = (int)char.GetNumericValue(chArray[i]);
            }
            int num2 = numArray[9];

            int num3 = ((((((((numArray[0] * 10) + (numArray[1] * 9)) + (numArray[2] * 8)) + (numArray[3] * 7)) + (numArray[4] * 6)) + (numArray[5] * 5)) + (numArray[6] * 4)) + (numArray[7] * 3)) + (numArray[8] * 2);
            int num4 = num3 - ((num3 / 11) * 11);
            if ((((num4 == 0) && (num2 == num4)) || ((num4 == 1) && (num2 == 1))) || ((num4 > 1) && (num2 == Math.Abs((int)(num4 - 11)))))
            {
                return 1;
            }
            else
            {
                return 0;

            }
        }
        public JsonResult DetailsMalek(string id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_OwnerSelect("fldID", id.ToString(), 1,null, null).FirstOrDefault();
                var IsAccept = false;
                if (q != null)
                {
                    /*var fldOwnerType = "0";
                    if (q.fldOwnerType)
                        fldOwnerType = "1";*/
                    if (q.fldUserID != null)
                        IsAccept = true;
                    return Json(new
                    {
                        fldId = q.fldID,
                        fldName = q.fldName,
                        fldMelli_EconomicCode = q.fldMelli_EconomicCode,
                        fldType = q.fldType,
                        fldOwnerType = q.fldOwnerType.ToString(),
                        fldEmail = q.fldEmail,
                        fldMobile = q.fldMobile,
                        fldAddress = q.fldAddress,
                        fldPostalCode = q.fldPostalCode,
                        fldDesc = q.fldDesc,
                        fldDateShamsi = q.fldDateShamsi,
                        IsAccept = IsAccept
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        fldId = 0,
                        fldName = "",
                        fldMelli_EconomicCode = id,
                        fldOwnerType = 1,
                        fldEmail = "",
                        fldMobile = "",
                        fldAddress = "",
                        fldPostalCode = "",
                        fldDesc = "",
                        fldDateShamsi = "",
                        IsAccept = IsAccept
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, null, x.Message, DateTime.Now, null);
                return Json(new
                {
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    MsgTitle = "خطا",
                    Err = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult KhodEzhariDetail()
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_GuestUserSelect("fldId", Session["UserGeust"].ToString(),"", 0).FirstOrDefault();
                var state = "1";
                if (q.fldName != "")
                    state = "0";
                    return Json(new
                    {
                        state=state,
                        fldId = q.fldId,
                        Name = q.fldName,
                        fatherName = q.fldFatherName,
                        shenasnameSerial = q.fldSerialShenasname,
                        shenasnameNo = q.fldSh_Shenasname,
                        officeName = q.fldMahalSodoor
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, null, x.Message, DateTime.Now, null);
                return Json(new
                {
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    MsgTitle = "خطا",
                    Err = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Archive()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult ReloadArchive()
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var u = p.sp_GuestUserSelect("fldId", Session["UserGeust"].ToString(), "", 0).FirstOrDefault();
            var owner=p.sp_OwnerSelect("fldMelli_EconomicCode", u.fldNationalCode, 1, null, "").FirstOrDefault();
            var data = p.sp_CarFileSelect("OwnerId", owner.fldID.ToString(), 0, null, null).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult calc(int carid)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            int State = 1;
            Models.cartaxEntities m = new Models.cartaxEntities();
            var u = m.sp_GuestUserSelect("fldId", Session["UserGeust"].ToString(), "", 0).FirstOrDefault();
            var DateTime = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
            var car = m.sp_CarFileSelect("fldCarId", carid.ToString(), 1, null,null).FirstOrDefault();
            int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime).Substring(0, 4));
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
                var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), 0);
                string msg1 = "";

                switch (Result)
                {
                    case Transaction.TransactionResult.Fail:
                        msg1 = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1,
                            State = State
                        }, JsonRequestBehavior.AllowGet);

                    case Transaction.TransactionResult.NotSharj:
                        msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1,
                            State = State
                        }, JsonRequestBehavior.AllowGet);

                }
            }
            //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]),Convert.ToInt32( Session["CountryCode"]), null,
            //    null, DateTime, null, _year, _Bed).ToList();
            //Session["year"] = _year.Value.ToString();
            //Session["bed"] = _Bed.Value.ToString();

            var bedehi = m.prs_newCarFileCalc(DateTime, Convert.ToInt32(Session["CountryType"]),
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
                Session["Year"] = Years;
                //mablagh += Convert.ToInt32(_Bed.Value);
                //fldPrice += Convert.ToInt32(_Bed.Value);
                // Session["mablagh"] = mablagh;
                //Session["Fine"] = fldFine;
                // Session["ValueAddPrice"] = fldValueAddPrice;
                //Session["Price"] = fldPrice;
                //
                //Session["Bed"] = Convert.ToInt32(_Bed.Value);
                //Session["OtherPrice"] = fldOtherPrice;
                //Session["fldMainDiscount"] = fldMainDiscount;
                //Session["fldFineDiscount"] = fldFineDiscount;
                //Session["fldValueAddDiscount"] = fldValueAddDiscount;
                //Session["fldOtherDiscount"] = fldOtherDiscount;
                //Session["Joziyat"] = a;
                if (mablagh < 1000)
                {
                    mablagh = 0;
                    //bedehi = null;
                }

                string shGhabz = "", ShParvande = "",
                    shPardakht = "",
                    barcode = "";
                return Json(new
                {
                    bedehi = bedehi,
                    mablagh = mablagh,
                    shGhabz = shGhabz,
                    shPardakht = shPardakht,
                    barcode = barcode,
                    msg = "",
                    State = State,
                    fldFine = fldFine,
                    fldValueAddPrice = fldValueAddPrice,
                    fldPrice = fldPrice,
                    Years = Years,
                    //Bed = Convert.ToInt32(_Bed.Value),
                    fldOtherPrice = fldOtherPrice,
                    fldMainDiscount = fldMainDiscount,
                    fldFineDiscount = fldFineDiscount,
                    fldValueAddDiscount = fldValueAddDiscount,
                    fldOtherDiscount = fldOtherDiscount//,
                    // Joziyat=a
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                State = 2;
                string s = "", msg = "", year = _year.ToString();
                for (int i = 0; i < year.Length; i += 4)
                {
                    if (i < year.Length - 4)
                        s += year.Substring(i, 4) + " و ";
                    else
                        s += year.Substring(i, 4);
                }
                msg = "تعرفه سالهای " + s + " تعریف نشده است لطفا جهت اعلام به پشتیبان دکمه ارسال به پشتیبان را انتخاب و تا زمانی که نرخ توسط پشتیبان ثبت شود، منتظر بمانید، سپس دکمه دریافت از سرور را انتخاب کنید و پس از دریافت پیغام تایید، دکمه محاسبه مجدد را از قسمت صورت حساب انتخاب کنید.";
                return Json(new
                {

                    msg = msg,
                    Year = s.Replace(" و ", ","),
                    State = State
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult InsertInSuporter(string Year, int? fldCarClassId, int? fldCarModelId, int? fldCarSystemId, int? fldCabinTypeId, int? fldCarAccountTypeId, int? fldCarMakeId)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var u = p.sp_GuestUserSelect("fldId", Session["UserGeust"].ToString(), "", 0).FirstOrDefault();
            var Path = ""; ArrayList ar = new ArrayList();
            //var car = p.sp_SelectCarDetils(Convert.ToInt32(Session["fldCarID3"])).FirstOrDefault();
            /*var Div = p.sp_CountryDivName(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();*/
            var LastNodeId = p.sp_TableTreeSelect("fldSourceID", Session["CountryCode"].ToString(), 0, 0, 0).Where(l => l.fldNodeType == Convert.ToInt32(Session["CountryCode"])).FirstOrDefault().fldID;
            var nodes = p.sp_SelectUpTreeCountryDivisions(LastNodeId, 1, "").ToList();
            foreach (var item in nodes)
            {
                ar.Add(item.fldNodeName);
            }
            for (int i = 0; i < ar.Count; i++)
            {
                if (i < ar.Count - 1)
                    Path += ar[i].ToString() + "-->";
                else
                    Path += ar[i].ToString();
            }
            Supporter.SendToSuporter S = new Supporter.SendToSuporter();
            var Code = S.InsertInSupport(Year, fldCarClassId, fldCarModelId, fldCarSystemId, fldCabinTypeId, fldCarAccountTypeId, fldCarMakeId, Path);

            return Json(new
            {
                msg = "درخواست با کد رهگیری " + Code + " به پشتیبان ارسال شد."
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendToSupporter(string msg, string Year, int CarClassId, int carid)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            string fldCarClassName = "";
            string fldCarModelName = "";
            string fldCarSystemName = "";
            string fldCabinTypeName = "";
            string fldCarAccountTypeName = "";
            string fldCarMakeName = "";

            if (CarClassId == 0)
            {
                var car = p.sp_SelectCarDetils(carid).FirstOrDefault();
                fldCarClassName = car.fldCarClassName;
                fldCarModelName = car.fldCarModel;
                fldCarSystemName = car.fldCarSystemName;
                fldCabinTypeName = car.fldCarCabinName;
                fldCarAccountTypeName = car.fldCarAccountName;
                fldCarMakeName = car.fldCarMakeName;
            }
            else
            {
                var q = p.sp_CarClassSelect("fldId", CarClassId.ToString(), 1, 1, "").FirstOrDefault();
                fldCarClassName = q.fldName;
                fldCarModelName = q.fldCarModelName;
                var q1 = p.sp_CarModelSelect("fldId", q.fldCarModelID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                fldCarSystemName = q1.fldCarSystemName;
                var k = p.sp_CarSystemSelect("fldId", q1.fldCarSystemID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                fldCabinTypeName = k.fldCabinTypeName;
                fldCarAccountTypeName = k.fldCarAccountType;
                fldCarMakeName = k.fldCarMake;
            }
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.msg = msg;
            PartialView.ViewBag.fldCarClassId = fldCarClassName;
            PartialView.ViewBag.fldCarModelId = fldCarModelName;
            PartialView.ViewBag.fldCarSystemId = fldCarSystemName;
            PartialView.ViewBag.fldCabinTypeId = fldCabinTypeName;
            PartialView.ViewBag.fldCarAccountTypeId = fldCarAccountTypeName;
            PartialView.ViewBag.fldCarMakeId = fldCarMakeName;
            return PartialView;
        }
        public ActionResult Update(string FromYear, string ToYear, string CarMakeType, string CarAccountType, string CarCabin, string CarSystem, string CarTip, string CarClass)
        {
            try
            {
                if (Session["UserGeust"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                
                var Year2 = ToYear;
                var Year = FromYear.Split(',');
                var Loop = Year.Count();

                Models.cartaxEntities m = new Models.cartaxEntities();
                var u = m.sp_GuestUserSelect("fldId", Session["UserGeust"].ToString(), "", 0).FirstOrDefault();
                var Mun = m.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, 0, "").FirstOrDefault();
                string make = CarMakeType, acc = CarAccountType, cabin = CarCabin, sys = CarSystem, tip = CarTip, cls = CarClass;
                //make = m.sp_CarMakeSelect("fldid", CarMakeType, 0, 1, "").FirstOrDefault().fldName;
                //acc = m.sp_CarAccountTypeSelect("fldid", CarAccountType, 0, 1, "").FirstOrDefault().fldName;
                //cabin = m.sp_CabinTypeSelect("fldid", CarCabin, 0, 1, "").FirstOrDefault().fldName;
                //sys = m.sp_CarSystemSelect("fldid", CarSystem, 0, 1, "").FirstOrDefault().fldName;
                //tip = m.sp_CarModelSelect("fldid", CarTip, 0, 1, "").FirstOrDefault().fldName;
                //cls = m.sp_CarClassSelect("fldid", CarTip, 0, 1, "").FirstOrDefault().fldName;

                RateWebService.Rate WebRate = new RateWebService.Rate();
                var Check = WebRate.CheckAccountCharge(Mun.fldRWUserName, Mun.fldRWPass, Mun.fldName);
                if (Check == true)
                {
                    for (int i = 0; i < Loop; i++)
                    {
                        if (Year2 == "")
                        {
                            FromYear = ToYear = Year[i];
                        }
                        var Rate = WebRate.GetRate(Mun.fldRWUserName, Mun.fldRWPass, Mun.fldName, Convert.ToInt32(FromYear), Convert.ToInt32(ToYear), make, acc, cabin, sys, tip, cls);
                        var Type = 1;
                        var Code = 0;
                        Models.sp_CarAccountTypeSelect Acc = null;
                        Models.sp_CabinTypeSelect Cabin = null;
                        Models.sp_CarSystemSelect Sys = null;
                        Models.sp_CarModelSelect Tip = null;
                        Models.sp_CarClassSelect Class = null;

                        if (Rate != null)
                            foreach (var item in Rate)
                            {
                                var Name = item.fldName.Split('|');

                                var Make = m.sp_CarMakeSelect("fldName", Name[0], 0, 0, "").FirstOrDefault();
                                if (Make != null && Name.Count() > 1)
                                    Acc = m.sp_CarAccountTypeSelect("fldName", Name[1], 0, 0, "").Where(k => k.fldCarMakeID == Make.fldID).FirstOrDefault();
                                if (Acc != null && Name.Count() > 2)
                                    Cabin = m.sp_CabinTypeSelect("fldName", Name[2], 0, 0, "").Where(k => k.fldCarAccountTypeID == Acc.fldID).FirstOrDefault();
                                if (Cabin != null && Name.Count() > 3)
                                    Sys = m.sp_CarSystemSelect("fldName", Name[3], 0, 0, "").Where(k => k.fldCabinTypeID == Cabin.fldID).FirstOrDefault();
                                if (Sys != null && Name.Count() > 4)
                                    Tip = m.sp_CarModelSelect("fldName", Name[4], 0, 0, "").Where(k => k.fldCarSystemID == Sys.fldID).FirstOrDefault();
                                if (Tip != null && Name.Count() > 5)
                                    Class = m.sp_CarClassSelect("fldName", Name[5], 0, 0, "").Where(k => k.fldCarModelID == Tip.fldID).FirstOrDefault();

                                switch (Name.Count() - 1)
                                {
                                    case 1:
                                        Type = 1;
                                        Code = Make.fldID;
                                        break;
                                    case 2:
                                        Type = 2;
                                        Code = Acc.fldID;
                                        break;
                                    case 3:
                                        Type = 3;
                                        Code = Cabin.fldID;
                                        break;
                                    case 4:
                                        Type = 4;
                                        Code = Sys.fldID;
                                        break;
                                    case 5:
                                        Type = 5;
                                        Code = Tip.fldID;
                                        break;
                                    case 6:
                                        Type = 6;
                                        Code = Class.fldID;
                                        break;
                                }

                                var CarSeries = m.sp_GET_IDCarSeries(Type, Code).FirstOrDefault();
                                short? fromModel = null, ToModel = null;
                                byte? ToWheel = null, FromWheel = null, ToCylinder = null, FromCylinder = null;
                                if (item.fldFromModel != null)
                                    fromModel = (short)item.fldFromModel;
                                if (item.fldToModel != null)
                                    ToModel = (short)item.fldToModel;
                                if (item.fldToWheel != null)
                                    ToWheel = (byte)item.fldToWheel;
                                if (item.fldFromWheel != null)
                                    FromWheel = (byte)item.fldFromWheel;
                                if (item.fldToCylinder != null)
                                    ToCylinder = (byte)item.fldToCylinder;
                                if (item.fldFromCylinder != null)
                                    FromCylinder = (byte)item.fldFromCylinder;

                                var CRate = m.sp_ComplicationsRateSelect("fldYear", item.fldYear.ToString(), 0,
                                    0, "")
                                    .Where(k => k.fldCarSeriesID == CarSeries.CarSeriesId && k.fldCountryDivisions == 1
                                        && k.fldFromModel == fromModel && k.fldToModel == ToModel
                                        && k.fldFromWheel == FromWheel && k.fldToWheel == ToWheel
                                        && k.fldFromCylinder == FromCylinder && k.fldToCylinder == ToCylinder
                                        && k.fldFromContentMotor == item.fldFromContentMotor &&
                                        k.fldToContentMotor == item.fldToContentMotor).FirstOrDefault();
                                if (CRate != null)
                                    m.sp_ComplicationsRateUpdate(CRate.fldID, Type, Code, 0, 0, Convert.ToInt16(item.fldYear), FromCylinder, ToCylinder, FromWheel, ToWheel, fromModel, ToModel, item.fldFromContentMotor, item.fldToContentMotor, item.fldPrice, 0, "", "");
                                else
                                    m.sp_ComplicationsRateInsert(Type, Code, 0, 0, Convert.ToInt16(item.fldYear), FromCylinder, ToCylinder, FromWheel, ToWheel, fromModel, ToModel, item.fldFromContentMotor, item.fldToContentMotor, item.fldPrice, 0, "", "");
                            }

                    }
                    return Json(new { data = "بارگذاری با موفقیت انجام شد.", state = 0 });
                }
                else
                {
                    return Json(new { data = "شما مجاز به استفاده از خدمات پشتیبانی نمی باشید، لطفا با واحد پشتیبانی تماس بگیرید.", state = 1 });
                }

            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, null, x.Message, DateTime.Now, null);
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }
        public ActionResult Pardakht(string CarID, string CarFileID)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.CarID = CarID;
            PartialView.ViewBag.CarFileID = CarFileID;
            return PartialView;
        }
        public FileContentResult ImageBank(int id)
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
        public ActionResult GenerateFishReport(int id)
        {
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
                    MyLib.Shamsi.Shamsi2miladiDateTime(car.fldStartDateInsurance), datetime, null,
                    "","", fldMainDiscount, fldValueAddDiscount, fldOtherDiscount, ShGhabz, ShPardakht, _id, fldFineDiscount);
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
                            , Convert.ToInt32(mablagh), sal, mah);
                        ShGhabz = gh.ShGhabz;
                        ShPardakht = gh.ShPardakht;
                        BarcodeText = gh.BarcodeText;
                    }
                }
                //}
                Avarez.DataSet.DataSet1TableAdapters.QueriesTableAdapter Queries = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                Queries.PeacokeryUpdate(ShGhabz, ShPardakht, Convert.ToInt64(_id.Value));
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter sp_pic1 = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter();
                sp_pic1.Fill(dt.sp_PictureSelect1, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                sp_pic.Fill(dt.sp_PictureSelect, "fldBankPic", "1", 1, 1, "");

                fish.Fill(dt.sp_PeacockerySelect, "fldId", Session["FishId"].ToString(), 1, 1, "");

                var mnu1 = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();

                var UpReportSelect = p.sp_UpReportSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var FishReport = p.sp_ReportsSelect("fldId", UpReportSelect.fldReportsID.ToString(), 1, null, "").FirstOrDefault();
                System.IO.MemoryStream Stream = new System.IO.MemoryStream(FishReport.fldReport);
                FastReport.Report Report = new FastReport.Report();
                Report.Load(Stream);
                Report.RegisterData(dt, "complicationsCarDBDataSet");
                Report.SetParameterValue("MunicipalityName", mnu1.fldName);
                Report.SetParameterValue("Barcode", Session["Barcode"].ToString());
                Report.SetParameterValue("ShGhabz", Session["ShGhabz"].ToString());
                Report.SetParameterValue("ShPardakht", Session["ShPardakht"].ToString());
                Report.Prepare();
                Session.Remove("FishId");
                Session.Remove("ShGhabz");
                Session.Remove("ShPardakht");
                Session.Remove("Barcode");
                Session.Remove("SalAvarez");
                Session.Remove("Mohlat");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            return null;

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
                    shGabz = shGabz.PadLeft(13, '0');
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
            long peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
            if (peackokeryid == 0)
            {
                FishReport(CarId);
                peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
            }
            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
            if (BankId != 17)
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "",null, "", "", Amount, Convert.ToInt32(Session["UserMnu"]), Convert.ToInt32(Session["UserGeust"]));

            if (BankId != 15)
            {
                Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                Session["OnlinefishId"] = fish.fldID;
            }
            else
            {
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "",null, "", "", Amount, Convert.ToInt32(Session["UserMnu"]), Convert.ToInt32(Session["UserGeust"]));
                var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, null, "").FirstOrDefault();
                Tax = q.fldTemporaryCode;
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                Session["OnlinefishId"] = fish.fldID;
            }
            if (BankId == 17)
            {                
                if (peackokeryid != 0)
                {
                    var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                    if (fish.fldShGhabz != "" && fish.fldShPardakht != "")
                    {
                        p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "",null, fish.fldShGhabz + "|" + fish.fldShPardakht, "", Amount, Convert.ToInt32(Session["UserMnu"]), Convert.ToInt32(Session["UserGeust"]));
                        var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, null, "").FirstOrDefault();
                        Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
                        Session["shGhabz"] = fish.fldShGhabz;
                        Session["shPardakht"] = fish.fldShPardakht;
                        Session["OnlinefishId"] = fish.fldID;
                    }
                    else
                        return null;
                }
            }

            p.sp_OnlineTemporaryCodePaymentsUpdate(Convert.ToInt32(_id.Value), Tax, Amount,null, "");
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
            return Json(new { url = "~/" + URL + "/Index", Msg = "", MsgTitle = "", Er = 0 }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShowMafasa(string CarId)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.CarId = CarId;
            return PartialView;
        }
        public ActionResult Mafasa(int id)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_SelectCarDetils(id).FirstOrDefault();
            if (car != null)
            {
                var Cdate = p.sp_GetDate().FirstOrDefault();
                int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Cdate.CurrentDateTime).Substring(0, 4));
                string date = toYear + "/12/29";
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                    date = toYear + "/12/30";
                //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                
                var datetime = p.sp_GetDate().FirstOrDefault().CurrentDateTime;
                //var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                //    null, datetime,null, _year, _Bed).ToList();
                var bedehi = p.prs_newCarFileCalc(datetime, Convert.ToInt32(Session["CountryType"]),
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
                int? mablagh = 0;
                foreach (var item in bedehi)
                {
                    int? jam = (item.fldFinalPrice + item.fldMashmol + item.fldNoMashmol + item.fldMablaghJarime + item.fldOtherPrice) -
                        (item.fldDiscontJarimePrice + item.fldDiscontMoaserPrice + item.fldDiscontOtherPrice + item.fldDiscontValueAddPrice);
                    mablagh += jam;
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
                        fish1.Fill(dt.rpt_RecoupmentAccount1, car.fldID, DateTime.Now);
                    }
                    else
                    {
                        fish.Fill(dt.rpt_RecoupmentAccount, car.fldID, DateTime.Now);
                    }
                    dt.EnforceConstraints = false;
                    exp.Fill(dt.sp_CarExperienceSelect, "fldCarFileID", car.fldID.ToString(), 0, Convert.ToInt32(Session["UserMnu"].ToString()), "");
                    Avarez.Models.cartaxEntities Car = new Avarez.Models.cartaxEntities();
                    Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                    var mnu = Car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    var State = Car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();

                    System.Data.Entity.Core.Objects.ObjectParameter mafasaId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterDate = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterDate", typeof(DateTime));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterNum = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterNum", typeof(string));

                    p.Sp_MafasaInsert(mafasaId, car.fldCarID, Convert.ToInt32(Session["UserMnu"]), null, null, LetterDate, LetterNum);
                    string barcode = WebConfigurationManager.AppSettings["SiteURL"] + "/QR_Mafasa/Get/" + mafasaId.Value;

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
                    Report.SetParameterValue("AreaName", ""/*Session["area"].ToString()*/);
                    Report.SetParameterValue("OfficeName", "" /*Session["office"].ToString()*/);
                    Report.SetParameterValue("sal", toYear.ToString().Substring(0, 4));
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
                    //return File(stream.ToArray(), "application/pdf");
                }
                else
                {
                    this.GetCmp<Window>("MafasaWin").Destroy();
                    //return Json(new { Er = 1 }, JsonRequestBehavior.AllowGet);
                    return Json(new { Er = 1, Msg = "کاربر گرامی شما در حال انجام عملیات غیر قانونی می باشید و در صورت تکرار آی پی شما به پلیس فتا اعلام خواهد شد. باتشکر", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);

                    /*X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "کاربر گرامی شما در حال انجام عملیات غیر قانونی می باشید و در صورت تکرار آی پی شما به پلیس فتا اعلام خواهد شد. باتشکر"
                    });
                    DirectResult result = new DirectResult();
                    return result;*/
                }
            }
            else
                return null;

        }
        public ActionResult HelpMalek()
        {//باز شدن پنجره
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
        }
    }
}
