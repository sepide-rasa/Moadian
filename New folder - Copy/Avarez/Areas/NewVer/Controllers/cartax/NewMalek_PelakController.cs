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

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class NewMalek_PelakController : Controller
    {
        //
        // GET: /NewVer/NewMalek_Pelak/
        bool invalidNew = false;
        public ActionResult Index(string NationalCode, string State, bool Malek, string CarFileID, byte TypeMalek)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q=car.sp_OwnerSelect("fldMelli_EconomicCode", NationalCode, 0, 1, "").FirstOrDefault();
            var subSett = car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
            int? fldDefaultPelakSerial = 0;
            int? fldDefaultPelakChar = 0;
            if (subSett != null)
            {
                if (subSett.fldDefaultPelakSerial != null)
                    fldDefaultPelakSerial = subSett.fldDefaultPelakSerial;
                if (subSett.fldDefaultPelakChar != null)
                    fldDefaultPelakChar = subSett.fldDefaultPelakChar;
            }

            var Accept = 0;
            if (State == "1" && Malek == true)/*ویرایش مالک*/
            {
                if (q.IsAccept == 1 && Convert.ToInt32(Session["UserId"])!=1)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "به علت تأیید پرونده شما قادر به ویرایش مالک نمی باشید."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            PartialView.ViewBag.NationalCode = NationalCode;
            PartialView.ViewBag.Malek = Convert.ToString(Malek);
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.CarFileID = CarFileID;
            PartialView.ViewBag.TypeMalek = TypeMalek.ToString();
            PartialView.ViewBag.fldDefaultPelakChar = fldDefaultPelakChar;
            PartialView.ViewBag.fldDefaultPelakSerial = fldDefaultPelakSerial;
            PartialView.ViewBag.Accept = Accept.ToString();
            return PartialView;
        }

        public ActionResult GetTypeP()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PlaqueTypeSelect("", "", 0, 0, "").Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(l => l.Name), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCityP(string cboTypeP)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            if (cboTypeP == "ملی")
                return Json(car.sp_PlaqueCitySelect("fldName", "ایران", 0, 0, "").Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(l => l.Name), JsonRequestBehavior.AllowGet);
            else
                return Json(car.sp_PlaqueCitySelect("", "", 0, 0, "").Where(p => p.fldName != "ایران").Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(l => l.Name), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSerialP()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PlaqueSerialSelect("", "", 0, 0, "").Select(c => new { ID = c.fldID, Name = c.fldSerial }).OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStatusP()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StatusPlaqueSelect("", "", 0, 0, "").Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCharP()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CharacterPersianPlaqueSelect("", "", 0,0, "").Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadPelak(StoreRequestParameters parameters, string NationalCode)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            List<Models.sp_CarPlaqueSelect> data = null;
            var q = p.sp_OwnerSelect("fldMelli_EconomicCode", NationalCode, 1, 0, "").FirstOrDefault();
            if (q != null)
            {
                data = p.sp_CarPlaqueSelect("fldOwnerID", q.fldID.ToString(), 200, 0, "").ToList();
            }        

            return this.Store(data);
        }

        public ActionResult DetailsPelak(int Id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var CarPlaque = Car.sp_CarPlaqueSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (CarPlaque.IsAccept == 1 && Convert.ToInt32(Session["UserId"])!=1)
                {
                    return Json(new
                    {
                        Msg = "به علت تأیید پرونده شما قادر به ویرایش پلاک نمی باشید.",
                        Er = 1,
                        MsgTitle = "خطا"
                    });
                }
                else
                {
                    var type = Car.sp_PlaqueTypeSelect("fldId", CarPlaque.fldPlaqueTypeID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    int charId = 0;
                    string two = "";
                    string three = "";
                    string charp = "";
                    string plaqu = CarPlaque.fldPlaqueNumber; ;
                    if (type.fldName == "ملی")
                    {
                        two = plaqu.Substring(4, 2);
                        three = plaqu.Substring(0, 3);
                        charp = plaqu.Substring(3, 1);
                        var type1 = Car.sp_CharacterPersianPlaqueSelect("fldName", charp, 1, 1, "").FirstOrDefault();
                        charId = type1.fldID;
                        return Json(new
                        {
                            charId = charId.ToString(),
                            Two = two,
                            Three = three,
                            Er=0,
                            fldId = CarPlaque.fldID,
                            fldStatusPlaqeID = CarPlaque.fldStatusPlaqeID.ToString(),
                            fldPlaqueNumber = CarPlaque.fldPlaqueNumber.ToString(),
                            fldPlaqueCityID = CarPlaque.fldPlaqueCityID.ToString(),
                            fldPlaqueSerialID = CarPlaque.fldPlaqueSerialID.ToString(),
                            fldPlaqueTypeID = CarPlaque.fldPlaqueTypeID.ToString(),
                            fldOwnerID = CarPlaque.fldOwnerID.ToString(),
                            fldCharacterPersianPlaqueID = CarPlaque.fldCharacterPersianPlaqueID.ToString(),
                            fldDesc = CarPlaque.fldDesc
                        }, JsonRequestBehavior.AllowGet);

                    }
                    else if (type.fldName != "ملی")
                    {
                        return Json(new
                        {
                            Three = CarPlaque.fldPlaqueNumber,
                            fldId = CarPlaque.fldID,
                            Er = 0,
                            fldStatusPlaqeID = CarPlaque.fldStatusPlaqeID.ToString(),
                            fldPlaqueNumber = CarPlaque.fldPlaqueNumber.ToString(),
                            fldPlaqueCityID = CarPlaque.fldPlaqueCityID.ToString(),
                            fldPlaqueSerialID = CarPlaque.fldPlaqueSerialID.ToString(),
                            fldPlaqueTypeID = CarPlaque.fldPlaqueTypeID.ToString(),
                            fldOwnerID = CarPlaque.fldOwnerID.ToString(),
                            fldCharacterPersianPlaqueID = CarPlaque.fldPlaqueCityID.ToString(),
                            fldDesc = CarPlaque.fldDesc
                        }, JsonRequestBehavior.AllowGet);

                    }
                    return Json("");
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle="خطا" ,Msg= "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }

        public ActionResult DetailsMalek(string NationalCode)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_OwnerSelect("fldMelli_EconomicCode", NationalCode, 1, 0, "").FirstOrDefault();
                if (q != null)
                {
                    /*var fldOwnerType = "0";
                    if (q.fldOwnerType)
                        fldOwnerType = "1";*/
                    return Json(new
                    {
                        fldId = q.fldID,
                        fldName = q.fldName,
                        fldMelli_EconomicCode = q.fldMelli_EconomicCode,
                        fldType = q.fldType,/*واکشی از ثبت*/
                        fldAccept = q.IsAccept,/*تایید پرونده*/
                        fldOwnerType = q.fldOwnerType.ToString(),
                        fldEmail = q.fldEmail,
                        fldMobile = q.fldMobile,
                        Err = 0,
                        fldAddress = q.fldAddress,
                        fldPostalCode = q.fldPostalCode,
                        fldDesc = q.fldDesc,
                        fldDateShamsi = q.fldDateShamsi
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        fldId = 0,
                        fldName = "",
                        fldMelli_EconomicCode = NationalCode,
                        fldOwnerType = 1,
                        fldEmail = "",
                        fldMobile = "",
                        fldAddress = "",
                        fldPostalCode = "",
                        Err=0,
                        fldDesc = "",
                        fldDateShamsi = ""
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    MsgTitle = "خطا",
                    Err = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeletePelak(string Id)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 231))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    var q=Car.sp_CarFileSelect("fldCarPlaqueID", Id, 0, 0, "").ToList();
                    if (q.Count==0)
                    {
                        Car.sp_CarPlaqueDelete(Convert.ToInt32(Id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        return Json(new
                        {
                            Msg = "حذف با موفقیت انجام شد.",
                            MsgTitle = "حذف موفق",
                            Err = 0
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "برای پلاک مورد نظر پرونده ثبت شده است. جهت حذف پلاک ابتدا پرونده را حذف نمایید.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Err = 1,
                        MsgTitle = "خطا"
                    });
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    MsgTitle = "خطا",
                    Err = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SavePelak(Models.sp_CarPlaqueSelect CarPlaque)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();

                var q = Car.sp_CarPlaqueSelect("fldPlaqueNumber", CarPlaque.fldPlaqueNumber, 0, 0, "")
                    .Where(l => l.fldID != CarPlaque.fldID && l.fldPlaqueCityID == CarPlaque.fldPlaqueCityID &&
                        l.fldPlaqueSerialID == CarPlaque.fldPlaqueSerialID && l.fldOwnerID == CarPlaque.fldOwnerID).FirstOrDefault();
                if (CarPlaque.fldDesc == null)
                    CarPlaque.fldDesc = "";

                System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                if (CarPlaque.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 230))
                    {
                        if (q != null)
                        {
                            return Json(new
                            {
                                Msg = "پلاک وارد شده تکراری می باشد.",
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }

                        if (CarPlaque.fldCharacterPersianPlaqueID == 0 || CarPlaque.fldCharacterPersianPlaqueID == null)
                            Car.sp_CarPlaqueInsert(id, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,
                            CarPlaque.fldPlaqueTypeID, CarPlaque.fldOwnerID, null,
                            CarPlaque.fldStatusPlaqeID, Convert.ToInt32(Session["UserId"]), CarPlaque.fldDesc, Session["UserPass"].ToString());
                        else
                            Car.sp_CarPlaqueInsert(id, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,
                            CarPlaque.fldPlaqueTypeID, CarPlaque.fldOwnerID, CarPlaque.fldCharacterPersianPlaqueID,
                            CarPlaque.fldStatusPlaqeID, Convert.ToInt32(Session["UserId"]), CarPlaque.fldDesc, Session["UserPass"].ToString());

                        return Json(new
                        {
                            Msg = "ذخیره با موفقیت انجام شد.",
                            MsgTitle = "ذخیره موفق",
                            Er = 0,
                            id = id.Value
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 232))
                    {
                        if (q != null)
                        {
                            return Json(new
                            {
                                Msg = "پلاک وارد شده تکراری می باشد.",
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        if (CarPlaque.fldCharacterPersianPlaqueID == 0 )
                        {
                            CarPlaque.fldCharacterPersianPlaqueID = null;
                        }
                        Car.sp_CarPlaqueUpdate(CarPlaque.fldID, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,
                                        CarPlaque.fldPlaqueTypeID, CarPlaque.fldOwnerID, CarPlaque.fldCharacterPersianPlaqueID,
                                        CarPlaque.fldStatusPlaqeID, Convert.ToInt32(Session["UserId"]), CarPlaque.fldDesc, Session["UserPass"].ToString());
                        return Json(new
                             {
                                 Msg = "ویرایش با موفقیت انجام شد.",
                                 MsgTitle = "ویرایش موفق",
                                 Err = 0,
                                 id = CarPlaque.fldID
                             }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    MsgTitle = "خطا",
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public bool checkEmail(string Email)
        {
            if (Session["UserId"] == null)
                return false;
            if (String.IsNullOrEmpty(Email))
                invalidNew = false;
            else
            {
                Email =Regex.Replace(Email, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);

                invalidNew = Regex.IsMatch(Email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);
            }
            return invalidNew;
        }

        private string DomainMapper(Match match)
        {
            if (Session["UserId"] == null)
                return "";
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalidNew = true;
            }
            return match.Groups[1].Value + domainName;
        }
        public ActionResult SaveMalek_Pelak(Models.sp_OwnerSelect Owner, string fldIdPelak, string fldPlaqueNumber,
            int? fldPlaqueCityID, int? fldPlaqueSerialID, string fldPlaqueTypeID,
            int? fldCharacterPersianPlaqueID, string fldStatusPlaqeID, string fldDescPelak)
        {

            var Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));

                bool checkE = true;

                if (Owner.fldDesc == null)
                    Owner.fldDesc = "";

                if (Owner.fldEmail == null)
                    Owner.fldEmail = "";

                if (Owner.fldEmail != "")
                    checkE = checkEmail(Owner.fldEmail);

                byte? ownertype=0;
                if (Owner.fldMelli_EconomicCode.Length == 10)//حقیقی
                {
                    ownertype = 1;
                }

                if (checkE)
                {
                    if (Owner.fldID == 0)
                    {//ثبت رکورد جدید
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 227) && Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 230))
                        {
                            /*Car.sp_OwnerInsert(Owner.fldName, Owner.fldMelli_EconomicCode, ownertype,
                                Owner.fldEmail, Owner.fldMobile, Owner.fldAddress, Owner.fldPostalCode,
                                Convert.ToInt64(Session["UserId"]), Owner.fldDesc, Session["UserPass"].ToString(), Owner.fldType, MyLib.Shamsi.Shamsi2miladiDateTime(Owner.fldDateShamsi));

                            var q = Car.sp_OwnerSelect("fldMelli_EconomicCode", Owner.fldMelli_EconomicCode, 1,
                            Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();*/
                            var mobile = Car.prs_MobileCount(Owner.fldMobile).FirstOrDefault();
                            if (mobile.MobileCount <= 5)
                            {
                                if (Owner.fldDateShamsi != null && Owner.fldDateShamsi != "")
                                {

                                    Car.sp_InsertOwner_CarPlaque(Owner.fldName, Owner.fldMelli_EconomicCode, ownertype, Owner.fldEmail, Owner.fldMobile,
                                    Owner.fldAddress, Owner.fldPostalCode, Owner.fldType, MyLib.Shamsi.Shamsi2miladiDateTime(Owner.fldDateShamsi),
                                    fldPlaqueNumber, fldPlaqueCityID, fldPlaqueSerialID, Convert.ToInt32(fldPlaqueTypeID),
                                    fldCharacterPersianPlaqueID, Convert.ToInt32(fldStatusPlaqeID), Owner.fldDesc, Convert.ToInt64(Session["UserId"]),
                                    Session["UserPass"].ToString(), fldDescPelak);
                                }
                                else
                                {
                                    Car.sp_InsertOwner_CarPlaque(Owner.fldName, Owner.fldMelli_EconomicCode, ownertype, Owner.fldEmail, Owner.fldMobile,
                                    Owner.fldAddress, Owner.fldPostalCode, Owner.fldType, null,
                                    fldPlaqueNumber, fldPlaqueCityID, fldPlaqueSerialID, Convert.ToInt32(fldPlaqueTypeID),
                                    fldCharacterPersianPlaqueID, Convert.ToInt32(fldStatusPlaqeID), Owner.fldDesc, Convert.ToInt64(Session["UserId"]),
                                    Session["UserPass"].ToString(), fldDescPelak);
                                }
                                var q = Car.sp_OwnerSelect("fldMelli_EconomicCode", Owner.fldMelli_EconomicCode, 30,
                                   null, null).FirstOrDefault();
                                SmsSender sms = new SmsSender();
                                sms.SendMobileVerify(Convert.ToInt32(Session["UserMnu"]), q.fldID);
                                return Json(new
                                {
                                    Msg = "ذخیره با موفقیت انجام شد.",
                                    MsgTitle = "ذخیره موفق",
                                    Err = 0
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new
                                {
                                    Msg = "شماره موبایل درج شده تکراری بوده و قبلا استفاده شده است.",
                                    MsgTitle = "ذخیره ناموفق",
                                    Err = 1
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                Msg = "شما مجاز به ذخیره نمی باشید.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {//ویرایش رکورد ارسالی
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 228))
                        {
                            if (Owner.fldDateShamsi != null && Owner.fldDateShamsi != "")
                            {
                                Car.sp_OwnerUpdate(Owner.fldID, Owner.fldName, Owner.fldMelli_EconomicCode, ownertype,
                                    Owner.fldEmail, Owner.fldMobile, Owner.fldAddress, Owner.fldPostalCode,
                                    Convert.ToInt64(Session["UserId"]), Owner.fldDesc, Session["UserPass"].ToString(), MyLib.Shamsi.Shamsi2miladiDateTime(Owner.fldDateShamsi), Owner.fldType);
                            }
                            else
                            {
                                Car.sp_OwnerUpdate(Owner.fldID, Owner.fldName, Owner.fldMelli_EconomicCode, ownertype,
                                    Owner.fldEmail, Owner.fldMobile, Owner.fldAddress, Owner.fldPostalCode,
                                    Convert.ToInt64(Session["UserId"]), Owner.fldDesc, Session["UserPass"].ToString(), null, Owner.fldType);
                            }

                            if (fldPlaqueNumber != "")
                            {
                                var q2 = Car.sp_CarPlaqueSelect("fldPlaqueNumber", fldPlaqueNumber, 0, 0, "").Where(l => l.fldID != Convert.ToInt32(fldIdPelak) && l.fldPlaqueCityID == Convert.ToInt32(fldPlaqueCityID) && l.fldPlaqueSerialID == Convert.ToInt32(fldPlaqueSerialID)).FirstOrDefault();

                                if (fldIdPelak == "0")
                                {
                                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 230))
                                    {
                                        if (q2 == null)
                                        {
                                            Car.sp_CarPlaqueInsert(id, fldPlaqueNumber, Convert.ToInt32(fldPlaqueCityID), Convert.ToInt32(fldPlaqueSerialID),
                                            Convert.ToInt32(fldPlaqueTypeID), Owner.fldID, fldCharacterPersianPlaqueID,
                                            Convert.ToInt32(fldStatusPlaqeID), Convert.ToInt64(Session["UserId"]), fldDescPelak, Session["UserPass"].ToString());
                                            return Json(new
                                            {
                                                Msg = "ویرایش با موفقیت انجام شد.",
                                                MsgTitle = "ویرایش موفق",
                                                Err = 0
                                            }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            return Json(new
                                            {
                                                Msg = "پلاک وارد شده تکراری است.",
                                                MsgTitle = "خطا",
                                                Err = 1
                                            }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        return Json(new
                                        {
                                            Msg = "شما مجاز به دسترسی نمی باشید.",
                                            MsgTitle = "خطا",
                                            Err = 1
                                        }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 232))
                                    {
                                        if (q2 == null)
                                        {
                                            Car.sp_CarPlaqueUpdate(Convert.ToInt32(fldIdPelak), fldPlaqueNumber, Convert.ToInt32(fldPlaqueCityID), Convert.ToInt32(fldPlaqueSerialID),
                                                Convert.ToInt32(fldPlaqueTypeID), Owner.fldID, fldCharacterPersianPlaqueID,
                                                Convert.ToInt32(fldStatusPlaqeID), Convert.ToInt64(Session["UserId"]), fldDescPelak, Session["UserPass"].ToString());
                                            return Json(new
                                            {
                                                Msg = "ویرایش با موفقیت انجام شد.",
                                                MsgTitle = "ویرایش موفق",
                                                Err = 0
                                            }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            return Json(new
                                            {
                                                Msg = "پلاک وارد شده تکراری است.",
                                                MsgTitle = "خطا",
                                                Err = 1
                                            }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        return Json(new
                                        {
                                            Msg = "شما مجاز به دسترسی نمی باشید.",
                                            MsgTitle = "خطا",
                                            Err = 1
                                        }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            return Json(new
                            {
                                Msg = "ویرایش با موفقیت انجام شد.",
                                MsgTitle = "ویرایش موفق",
                                Err = 0
                            }, JsonRequestBehavior.AllowGet);
                            
                        }
                        else
                        {
                            return Json(new
                            {
                                Msg = "شما مجاز به دسترسی نمی باشید.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json(new
                    {
                        Msg = "ایمیل وارد شده معتبر نمی باشد.",
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Err = 1});
            } 
        }
    }
}
