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
    public class ParvandeController : Controller
    {
        //
        // GET: /NewVer/Parvande/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
               // return RedirectToAction("logon", "Account");
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var Div = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                
                //var TransactionInf = p.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                //Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
                //string Mojodi = "0";
                //bool haveSharj = false;
                //if (TransactionInf != null)
                //{
                //    var y = h.CheckAccountCharge(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName);
                //    if (y != null)
                //    {
                //        Mojodi = y.Mojodi;
                //        haveSharj = y.HaveCharge;
                //    }
                //}
                //if (Mojodi != "" && haveSharj)
                //{
                    OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض");
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
                //}
                //else
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.ERROR,
                //        Title = "خطا",
                //        Message = "جهت استفاده از امکانات نرم افزار موجودی حساب خود را افزایش دهید. لطفا به آدرس ذیل مراجعه فرمایید: http://trn.ecartax.ir"
                //    });
                //    DirectResult result = new DirectResult();
                //    return result;
                //    /*Session["ER"] = "جهت استفاده از امکانات نرم افزار موجودی حساب خود را افزایش دهید. لطفا به آدرس ذیل مراجعه فرمایید: http://trn.ecartax.ir";
                //    return RedirectToAction("error", "Metro");*/
                //}
            }
            else
            {
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
        public ActionResult ShouldMobileVerify(int CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                cartaxEntities c = new cartaxEntities();
                var countrydiv = c.sp_GET_IDCountryDivisions(5, Convert.ToInt32(Session["UserMnu"])).FirstOrDefault();
                var subSetting = c.sp_SubSettingSelect("fldCountryDivisionsID", countrydiv.CountryDivisionId.ToString(), 0, 1, "").FirstOrDefault();
                int Verifyed = 1;
                if (subSetting.fldMobileVerify)
                {
                    var q = c.sp_CarFileSelect("fldId", CarFileId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    var p = c.sp_CarPlaqueSelect("fldId", q.fldCarPlaqueID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    var owner = c.sp_OwnerSelect("fldId", p.fldOwnerID.ToString(), 0, 1, "").FirstOrDefault();
                    //if (Convert.ToInt32(Session["UserMnu"]) != 1)
                    Verifyed = Convert.ToInt32(owner.fldMobileVerify);
                }
                return Json(new
                {
                    Verifyed = Verifyed
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult MobileVerify(int CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                cartaxEntities c = new cartaxEntities();
                var q = c.sp_CarFileSelect("fldId", CarFileId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var p = c.sp_CarPlaqueSelect("fldId", q.fldCarPlaqueID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                PartialView.ViewBag.OwnerId = p.fldOwnerID;
                PartialView.ViewBag.carid = q.fldCarID;
                PartialView.ViewBag.carfileid = CarFileId;

                return PartialView;
            }
        }
        public ActionResult SendSms(int CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                
                cartaxEntities c = new cartaxEntities();
                var q = c.sp_CarFileSelect("fldId", CarFileId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var p = c.sp_CarPlaqueSelect("fldId", q.fldCarPlaqueID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                SmsSender sms = new SmsSender();
                sms.SendMobileVerify(Convert.ToInt32(Session["UserMnu"]), p.fldOwnerID);

                return Json(new
                {
                    Msg = "ارسال با موفقیت انجام شد.",
                    MsgTitle = "تایید موفق",
                    Err = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult MobileVerification(int OwnerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                
                cartaxEntities c = new cartaxEntities();
                c.sp_OwnerMobileVerify(OwnerId, true, Convert.ToInt32(Session["UserId"]));

                return Json(new {
                    Msg = "شماره موبایل شخص با موفقیت تایید شد. اکنون میتوانید ادامه دهید.",
                    MsgTitle = "تایید موفق",
                    Err = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult HelpParvande()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }
        }

        public ActionResult DeleteCached(string CarFileId)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var h = p.sp_CachedSearchCarFileSelect("fldCarFileId", CarFileId, 0).FirstOrDefault();
                if (h != null)
                {
                    p.sp_CachedSearchCarFileDelete(h.fldCode, Convert.ToInt32(Session["UserId"]));
                }
                return Json(new
                {
                    Msg = "حذف با موفقیت انجام شد.",
                    MsgTitle = "حذف موفق",
                    Err = 0
                }, JsonRequestBehavior.AllowGet);
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

        public ActionResult DeleteListSiyah(string CarId)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q=p.sp_ListeSiyahSelect("fldCarId", CarId.ToString(), 0).FirstOrDefault();
                p.sp_ListeSiyahDelete(q.fldId, Convert.ToInt32(Session["UserId"]));
                return Json(new
                {
                    Msg = "عملیات با موفقیت انجام شد.",
                    MsgTitle = "عملیات موفق",
                    Err = 0
                }, JsonRequestBehavior.AllowGet);
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
        public ActionResult HelpPersonal()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }
        }

        public ActionResult HelpPelak()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }
        }

        public ActionResult HelpParvandeC()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            List<Avarez.Models.sp_CachedSearchCarFileSelect> data = null;
            int userid = Convert.ToInt32(Session["UserId"]);
            data = p.sp_CachedSearchCarFileSelect("fldUserId", userid.ToString(), 20).ToList();
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Avarez.Models.sp_CachedSearchCarFileSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Search(int SearchField, string Value1,string Value2, int SearchType)
        {//جستجو
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            string[] _fiald = new string[] { "fldVIN", "fldShasiAndMotorNumber", "fldMotor", "fldShasi", "fldOwnerName", "fldCodeMeli", "fldPelak" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[SearchType], Value1);
            string searchtext2 = string.Format(searchType[SearchType], Value2);
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (SearchField == 0)
                Value2 = "";
            var q = m.sp_CarUserGuestSelect(_fiald[SearchField], searchtext, searchtext2, 100).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveSearch(Models.sp_CachedSearchCarFileSelect car)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (car.fldDesc == null)
                    car.fldDesc = "";
                var h=Car.sp_CachedSearchCarFileSelect("fldCarFileId", car.fldCarFileId.ToString(), 0).FirstOrDefault();
                if (h != null)
                    Car.sp_CachedSearchCarFileDelete(h.fldCode, Convert.ToInt32(Session["UserId"]));
                Car.sp_CachedSearchCarFileInsert(car.fldID, car.fldMotorNumber, car.fldShasiNumber, car.fldVIN, car.fldModel, car.fldCarModelName,
                    car.fldCarClassName, car.fldCarFileId, Convert.ToInt64(Session["UserId"]), car.fldDesc,car.fldName,car.fldMelli_EconomicCode,car.fldPlaqueNumber,
                    car.fldCarAccountName, car.fldCarsystemName, car.fldColor, car.fldFuleTypeName, car.fldAccept, car.fldAcceptName, car.fldIsBalckList, car.fldAccountTypeId);

                    return Json(new
                    {
                        Msg = "ذخیره با موفقیت انجام شد.",
                        MsgTitle = "ذخیره موفق",
                        Err = 0
                    }, JsonRequestBehavior.AllowGet);
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
        public ActionResult NewParvande(string containerId)
        {//باز شدن پرونده جدید
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            ViewData.Model = new Avarez.Models.Parvande();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult GetTypeP()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PlaqueTypeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(l => l.Name), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCityP(string cboTypeP)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            if (cboTypeP == "ملی")
                return Json(car.sp_PlaqueCitySelect("fldName", "ایران", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(l => l.Name), JsonRequestBehavior.AllowGet);
            else
                return Json(car.sp_PlaqueCitySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(p => p.fldName != "ایران").Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(l => l.Name), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetCityP()
        //{
        //    Models.cartaxEntities car = new Models.cartaxEntities();
        //    return Json(car.sp_PlaqueCitySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(p => p.fldName), JsonRequestBehavior.AllowGet);
        //}
        public ActionResult GetCharP()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CharacterPersianPlaqueSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStatusP()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StatusPlaqueSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { ID = c.fldID, Name = c.fldName }).OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSerialP()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PlaqueSerialSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { ID = c.fldID, Name = c.fldSerial }).OrderBy(p => p.Name), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetShort(string cboCarMake)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            if (cboCarMake == "داخلی")
                return Json(car.sp_ShortTermCountrySelect("fldSymbol", "IR", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { ID = c.fldID, Name = c.fldSymbol }), JsonRequestBehavior.AllowGet);
            else
                return Json(car.sp_ShortTermCountrySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(p => p.fldSymbol != "IR").Select(c => new { ID = c.fldID, Name = c.fldSymbol }), JsonRequestBehavior.AllowGet);


        }
        public ActionResult GetColor()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_ColorCarSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { ID = c.fldID, Name = c.fldColor }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPattern()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CarPatternModelSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { ID = c.fldID, Name = c.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMake()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CarMakeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { ID = c.fldID, Name = c.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAccount(int? cboCarMake)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var AccountType = car.sp_CarAccountTypeSelect("fldCarMakeID", cboCarMake.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(AccountType.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCabin(int? cboCarAccountTypes)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var CabinType = car.sp_CabinTypeSelect("fldCarAccountTypeID", cboCarAccountTypes.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CabinType.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSystem(int? cboCarCabin)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var CarSystem = car.sp_CarSystemSelect("fldCabinTypeID", cboCarCabin.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CarSystem.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetModel(int? cboSystem)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var CarModel = car.sp_CarModelSelect("fldCarSystemID", cboSystem.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CarModel.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClass(int? cboModel)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var CarClass = car.sp_CarClassSelect("fldCarModelID", cboModel.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CarClass.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetYear(int? Noo)
        {
            if (Noo == null)
                Noo = 1;
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Noo == 1)
            {
                for (int i = 1340; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = 1950; i <= DateTime.Now.Year + 1; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.OrderByDescending(l => l.Text).Select(p1 => new { ID = p1.Value, Name = p1.Text }), JsonRequestBehavior.AllowGet);
        }
        int per = 0;
        public ActionResult NewMalek(int id, int State)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            if (id == 0)
                per = 227;
            else
                per = 228;
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), per))
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.Id = id;
                PartialView.ViewBag.State = State;
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعریف مالک");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                }
            );
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult NewPelak(int id, int Malekid, int state)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            if (id == 0)
                per = 230;
            else
                per = 232;
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), per))
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.Id = id;
                PartialView.ViewBag.Malekid = Malekid;
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعریف پلاک");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                Models.cartaxEntities m = new Models.cartaxEntities();
                PartialView.ViewBag.state = state;
                return PartialView;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                }
            );
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult NewParvandeKhodro(int id, int Pelakid, int state)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            // return RedirectToAction("logon", "Account");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var subSett = p.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
            var ForceScan = "1";
            if (subSett != null)
            {
                if (subSett.fldHaveScan == false)
                    ForceScan = "0";
            }
            if (id == 0)
                per = 238;
            else
                per = 240;
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), per))
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعریف پرونده خودرو");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                if (id == 0)//جدید
                {
                    if (state == 2)
                    {
                        Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                        PartialView.ViewBag.state = state;
                        PartialView.ViewBag.Id = id;
                        PartialView.ViewBag.ForceScan = ForceScan;
                        return PartialView;
                    }
                    else
                    {
                        var q = p.sp_CarPlaqueSelect("fldID", Pelakid.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).First();
                        Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                        PartialView.ViewBag.state = state;
                        PartialView.ViewBag.fldCarPlaqueID = Pelakid;
                        PartialView.ViewBag.Id = id;
                        PartialView.ViewBag.fldPlaqueTypeName = q.fldPlaqueTypeName;
                        PartialView.ViewBag.ForceScan = ForceScan;
                        return PartialView;
                    }
                }
                else//ویرایش
                {
                    var file = Car.sp_CarFileSelect("fldId", id.ToString(), 1, 1, "").FirstOrDefault();
                    var edit = Car.sp_CheckEditCarFile(Convert.ToInt32(file.fldCarID)).FirstOrDefault().falg;
                    if (edit == false || Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 428))
                    {
                        if (state == 2)
                        {
                            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                            PartialView.ViewBag.state = state;
                            PartialView.ViewBag.Id = id;
                            PartialView.ViewBag.ForceScan = ForceScan;
                            return PartialView;
                        }
                        else
                        {
                            var q = p.sp_CarPlaqueSelect("fldID", Pelakid.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).First();
                            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                            PartialView.ViewBag.state = state;
                            PartialView.ViewBag.fldCarPlaqueID = Pelakid;
                            PartialView.ViewBag.Id = id;
                            PartialView.ViewBag.fldPlaqueTypeName = q.fldPlaqueTypeName;
                            PartialView.ViewBag.ForceScan = ForceScan;
                            return PartialView;
                        }
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "کاربر گرامی شما قادر به ویرایش پرونده نمی باشید"
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                /*if (edit == false)
                {
                    Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعریف پرونده خودرو");
                    SignalrHub hub = new SignalrHub();
                    hub.ReloadOnlineUser();

                    if (state == 2)
                    {
                        Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                        PartialView.ViewBag.state = state;
                        PartialView.ViewBag.Id = id;
                        PartialView.ViewBag.ForceScan = ForceScan;
                        return PartialView;
                    }
                    else
                    {
                        var q = p.sp_CarPlaqueSelect("fldID", Pelakid.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).First();
                        Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                        PartialView.ViewBag.state = state;
                        PartialView.ViewBag.fldCarPlaqueID = Pelakid;
                        PartialView.ViewBag.Id = id;
                        PartialView.ViewBag.fldPlaqueTypeName = q.fldPlaqueTypeName;
                        PartialView.ViewBag.ForceScan = ForceScan;
                        return PartialView;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "کاربر گرامی شما قادر به ویرایش پرونده نمی باشید"
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }*/
                
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                }
            );
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult ReloadPelak(int MalekId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            List<Models.sp_CarPlaqueSelect> data = null;
            data = car.sp_CarPlaqueSelect("fldOwnerID", MalekId.ToString(), 200, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Models.sp_SelectCar_TypeEntezami care,string carFileID,string carMake,bool Match)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
                
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var file = Car.sp_CarFileSelect("fldID", carFileID, 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var subSett = Car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), Car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                var edit = Car.sp_CheckEditCarFile(Convert.ToInt32(file.fldCarID)).FirstOrDefault().falg;
                bool? ForceScan = true;
                if (subSett != null)
                {
                    ForceScan = subSett.fldHaveScan;
                }
                string Datesanand = "";
                if (carMake == "1")
                {
                    Datesanand = care.fldModel + "/01/01";
                }
                else
                {
                    var datee = Convert.ToDateTime(care.fldModel + "-01-01");
                    Datesanand = MyLib.Shamsi.Miladi2ShamsiString(datee);
                }
                if (care.fldDesc == null)
                    care.fldDesc = "";
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 240))
                {
                    if (edit == false || Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 428))
                    {
                        if (file.fldAccept == false || (file.fldAccept == true && Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 434)))
                        {
                            if (care.fldID != 0)
                            {
                                if (ForceScan == true && care.fldStartDateInsurance != Datesanand && file.fldSanadForoshFileId == null)
                                {
                                    return Json(new
                                    {
                                        Msg = "لطفا ابتدا فایل سند فروش را ذخیره کرده و سپس اقدام به ویرایش مشخصات خودرو نمایید.",
                                        MsgTitle = "خطا",
                                        Er = 1
                                    }, JsonRequestBehavior.AllowGet);
                                }

                                Car.sp_UpdateCar_TypeEntezami(Convert.ToInt64(carFileID), care.fldMotorNumber, care.fldShasiNumber, care.fldVIN, care.fldCarModelID, care.fldCarClassID,
                                    care.fldCarColorID, care.fldModel, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance), care.fldTypeEntezami,Match, Convert.ToInt32(Session["UserId"]), care.fldDesc, Session["UserPass"].ToString());

                                return Json(new { MsgTitle = "ویرایش موفق", Msg = "ویرایش با موفقیت انجام شد.", Er = 0 });
                            }
                            else
                            {
                                return Json(new { MsgTitle = "ویرایش ناموفق", Msg = "ویرایش با موفقیت انجام نشد.", Er = 1 });
                            }
                        }
                        else
                        {
                            return Json(new { MsgTitle = "خطا", Msg = "به علت تأیید پرونده شما قادر به ویرایش آن نمی باشید.", Er = 1 });
                        }
                    }
                    else
                    {
                        return Json(new { MsgTitle = "خطا", Msg = "کاربر گرامی شما قادر به ویرایش پرونده نمی باشید.", Er = 1 });
                    }
                }
                else
                {
                    return Json(new { MsgTitle = "خطا", Msg = "شما مجاز به دسترسی نمی باشید.", Er = 1 });
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
                return Json(new { MsgTitle="خطا",Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }

        public ActionResult ReloadParvande(int PelakId, int MalekId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            List<Models.sp_CheckOwner_CarFile> data = null;
            data=car.sp_CheckOwner_CarFile(PelakId, MalekId).ToList();
            /*data = car.sp_CarFileSelect("fldCarPlaqueID", PelakId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();*/
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadMalek(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities car = new Models.cartaxEntities();

            List<Models.sp_OwnerSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.sp_OwnerSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldID";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldMelli_EconomicCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMelli_EconomicCode";
                            break;
                        case "fldMobile":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMobile";
                            break;
                        case "fldAddress":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAddress";
                            break;

                    }
                    if (data != null)
                        data1 = car.sp_OwnerSelect(field, searchtext,100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = car.sp_OwnerSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = car.sp_OwnerSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Models.sp_OwnerSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Upload()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

                if (Session["P_savePath"] != null)
                {
                    System.IO.File.Delete(Session["P_savePath"].ToString());
                    Session.Remove("P_savePath");
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                {
                    if (Request.Files[0].ContentLength <= 716800)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                        var Name = Guid.NewGuid();
                        string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                        file.SaveAs(savePath);
                        Session["P_savePath"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult Upload1()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

                if (Session["P_savePath1"] != null)
                {
                    System.IO.File.Delete(Session["P_savePath1"].ToString());
                    Session.Remove("P_savePath1");
                }
                var extension = Path.GetExtension(Request.Files[1].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                {
                    if (Request.Files[1].ContentLength <= 716800)
                    {
                        HttpPostedFileBase file = Request.Files[1];
                        //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                        var Name = Guid.NewGuid();
                        string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                        file.SaveAs(savePath);
                        Session["P_savePath1"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[1].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult Upload2()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

                if (Session["P_savePath2"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["P_savePath2"].ToString());
                    System.IO.File.Delete(Session["P_savePath2"].ToString());
                    Session.Remove("P_savePath2");
                }
                var extension = Path.GetExtension(Request.Files[3].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                {
                    if (Request.Files[3].ContentLength <= 716800)
                    {
                        HttpPostedFileBase file = Request.Files[3];
                        //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                        var Name = Guid.NewGuid();
                        string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                        file.SaveAs(savePath);
                        Session["P_savePath2"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[3].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult Upload3()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

                if (Session["P_savePath3"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["P_savePath3"].ToString());
                    System.IO.File.Delete(Session["P_savePath3"].ToString());
                    Session.Remove("P_savePath3");
                }
                var extension = Path.GetExtension(Request.Files[2].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                {
                    if (Request.Files[2].ContentLength <= 716800)
                    {
                        HttpPostedFileBase file = Request.Files[2];
                        //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                        var Name = Guid.NewGuid();
                        string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                        file.SaveAs(savePath);
                        Session["P_savePath3"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[2].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public FileContentResult Download(int FileId)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q = car.Sp_FilesSelect(FileId).FirstOrDefault();

            if (q != null)
            {
                MemoryStream st = new MemoryStream(q.fldImage);
                return File(st.ToArray(), MimeType.Get(".jpg"), "DownloadFile.jpg");
            }
            return null;
        }
        public ActionResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var file = Car.sp_CarFileSelect("fldID", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var care = Car.sp_CarSelect("fldId", file.fldCarID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var plaq = Car.sp_CarPlaqueSelect("fldID", file.fldCarPlaqueID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                int CarMake = 0, CarAccount = 0, CabinType = 0, CarSystem = 0, CarModel = 0;
                var c_class = Car.sp_CarClassSelect("fldId", care.fldCarClassID.ToString(), 1, 1, "").FirstOrDefault();
                var c_model = Car.sp_CarModelSelect("fldId", c_class.fldCarModelID.ToString(), 1, 1, "").FirstOrDefault();
                var c_system = Car.sp_CarSystemSelect("fldId", c_model.fldCarSystemID.ToString(), 1, 1, "").FirstOrDefault();
                var c_CabinType = Car.sp_CabinTypeSelect("fldId", c_system.fldCabinTypeID.ToString(), 1, 1, "").FirstOrDefault();
                var c_Account = Car.sp_CarAccountTypeSelect("fldId", c_CabinType.fldCarAccountTypeID.ToString(), 1, 1, "").FirstOrDefault();
                var c_Make = Car.sp_CarMakeSelect("fldId", c_Account.fldCarMakeID.ToString(), 0, 1, "").FirstOrDefault();

                var Symbol = Car.sp_ShortTermCountrySelect("fldSymbol", "IR", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldSymbol });
                if (c_Make.fldName != "داخلی")
                    Symbol = Car.sp_ShortTermCountrySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(p1 => p1.fldSymbol != "IR").Select(c => new { fldID = c.fldID, fldName = c.fldSymbol });


                CarMake = c_Account.fldCarMakeID;
                CarAccount = c_CabinType.fldCarAccountTypeID;
                CabinType = (int)c_system.fldCabinTypeID;
                CarSystem = c_model.fldCarSystemID;
                CarModel = c_class.fldCarModelID;

                var AccountType = Car.sp_CarAccountTypeSelect("fldCarMakeID", CarMake.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CabinType = Car.sp_CabinTypeSelect("fldCarAccountTypeID", CarAccount.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarSystem = Car.sp_CarSystemSelect("fldCabinTypeID", CabinType.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarModel = Car.sp_CarModelSelect("fldCarSystemID", CarSystem.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarClass = Car.sp_CarClassSelect("fldCarModelID", CarModel.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });

                string fldVIN = care.fldVIN.ToString();
                if (fldVIN.Length == 17)
                    fldVIN = fldVIN.Substring(0, 2);
                else
                    fldVIN = "..";
                if (fldVIN == "..")
                {
                    fldVIN = "IR";

                }
                var q = Car.sp_ShortTermCountrySelect("fldSymbol", fldVIN, 1, 1, "").FirstOrDefault();
                int sumbolid = 0;
                var ShortIcon = "";
                if (q != null)
                {
                    sumbolid = q.fldID;
                    ShortIcon = Convert.ToBase64String(q.fldIcon);
                }
                var fldTypeEntezami = "0";
                if (file.fldTypeEntezami == false)
                    fldTypeEntezami = "1";
                return Json(new
                {
                    fldPlaqueTypeName=plaq.fldPlaqueTypeName,
                    sumbolid = sumbolid.ToString(),
                    ShortIcon=ShortIcon,
                    symbol = fldVIN,
                    fldMotorNumber = care.fldMotorNumber,
                    fldShasiNumber = care.fldShasiNumber,
                    VIN = care.fldVIN,
                    fldCarModelID = care.fldCarModelID,
                    fldCarClassID = care.fldCarClassID,
                    fldCarColorID = care.fldCarColorID.ToString(),
                    fldColorName = care.fldColor,
                    fldModel = care.fldModel.ToString(),
                    fldStartDateInsurance = care.fldStartDateInsurance,
                    fldCarID = care.fldID,
                    fldCarPlaqueID = file.fldCarPlaqueID,
                    fldCarPlaquenum = plaq.fldPlaqueNumber,
                    fldDatePlaque = file.fldDatePlaque,
                    fldId = file.fldID,
                    CarMake = CarMake.ToString(),
                    CarAccount = AccountType,
                    CabinType = _CabinType,
                    CarSystem = _CarSystem,
                    CarModel = _CarModel,
                    CarClass = _CarClass,
                    CarAccountId = CarAccount.ToString(),
                    CabinTypeId = CabinType.ToString(),
                    CarSystemId = CarSystem.ToString(),
                    CarModelId = CarModel.ToString(),
                    CarClassId = care.fldCarClassID.ToString(),
                    Symbol = Symbol,
                    fldDesc = care.fldDesc,
                    fldBargSabzFileId = file.fldBargSabzFileId,
                    fldCartFileId = file.fldCartFileId,
                    fldSanadForoshFileId = file.fldSanadForoshFileId,
                    fldCartBackFileId = file.fldCartBackFileId,
                    fldTypeEntezami = fldTypeEntezami,
                    fldMotabeghBaShasi=care.fldMotabeghBaShasi,
                    Err=0
                }, JsonRequestBehavior.AllowGet);
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
        public ActionResult Save(Models.CarFile care, string CarMake,bool Match)
        {
            string Msg = ""; string MsgTitle = ""; var Err = 0;
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

                Models.cartaxEntities Car = new Models.cartaxEntities();
                var plaq = Car.sp_CarPlaqueSelect("fldid", care.fldCarPlaqueID.ToString(), 0, 1, "").FirstOrDefault();
                var owner = Car.sp_OwnerSelect("fldid", plaq.fldOwnerID.ToString(), 0, 1, "").FirstOrDefault();
                var mobile = Car.prs_MobileCount(owner.fldMobile).FirstOrDefault();
                if (mobile.MobileCount > 5)
                {
                    return Json(new
                    {
                        Msg = "شما به دلیل محدودیت تعداد پرونده برای یک شماره موبایل مجاز به ثبت پلاک برای مالک مورد نظر نمی باشید.",
                        MsgTitle = "ذخیره ناموفق",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                if (care.fldDesc == null)
                    care.fldDesc = "";
                int? Bargsabzfileid = null;
                int? Cartfileid = null;
                int? Sanadfileid = null;
                int? CartBack = null;
                string Datesanand = "";
                if (CarMake == "1")
                {
                    Datesanand=care.fldModel + "/01/01";
                }
                else
                {
                    var datee = Convert.ToDateTime(care.fldModel + "-01-01");
                    Datesanand=MyLib.Shamsi.Miladi2ShamsiString(datee);
                }

                var fileee=Car.sp_CarFileSelect("fldid",care.fldID.ToString(),1,1,"").FirstOrDefault();
                
                var subSett = Car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), Car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                bool? ForceScan = true;
                if (subSett != null)
                {
                    ForceScan = subSett.fldHaveScan;
                }

                if (care.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 238))
                    {
                        bool isupload = false;
                        if (Session["P_savePath"] != null)
                            isupload = true;
                        else if (Session["P_savePath1"] != null && Session["P_savePath3"] != null)
                            isupload = true;
                        else if (Session["P_savePath2"] != null)
                            isupload = true;

                        if (ForceScan == true && ((Session["P_savePath1"] != null && Session["P_savePath3"] == null) || (Session["P_savePath1"] == null && Session["P_savePath3"] != null)))
                        {
                            if (Session["P_savePath1"] != null)
                            {
                                System.IO.File.Delete(Session["P_savePath1"].ToString());
                                Session.Remove("P_savePath1");
                            }
                            if (Session["P_savePath3"] != null)
                            {
                                System.IO.File.Delete(Session["P_savePath3"].ToString());
                                Session.Remove("P_savePath3");
                            }
                            if (Session["P_savePath2"] != null)
                            {
                                System.IO.File.Delete(Session["P_savePath2"].ToString());
                                Session.Remove("P_savePath2");
                            }
                            if (Session["P_savePath"] != null)
                            {
                                System.IO.File.Delete(Session["P_savePath"].ToString());
                                Session.Remove("P_savePath");
                            }
                            return Json(new
                            {
                                Msg = "صفحات اول و دوم کارت خودرو باید همزمان آپلود شوند.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else if (ForceScan == true && isupload == false)
                        {
                            return Json(new
                            {
                                Msg = "لطفا فایل مدرک را آپلود کنید.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else if (ForceScan == true && care.fldStartDateInsurance != Datesanand && Session["P_savePath2"] == null)
                        {
                            if (Session["P_savePath1"] != null)
                            {
                                System.IO.File.Delete(Session["P_savePath1"].ToString());
                                Session.Remove("P_savePath1");
                            }
                            if (Session["P_savePath3"] != null)
                            {
                                System.IO.File.Delete(Session["P_savePath3"].ToString());
                                Session.Remove("P_savePath3");
                            }
                            if (Session["P_savePath"] != null)
                            {
                                System.IO.File.Delete(Session["P_savePath"].ToString());
                                Session.Remove("P_savePath");
                            }
                            return Json(new
                            {
                                Msg = "لطفا فایل سند فروش را آپلود کنید.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            //if (ForceScan == true && (Session["P_savePath"] != null || Session["P_savePath1"] != null || Session["P_savePath2"] != null || Session["P_savePath3"] != null))
                            //{
                            System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter b = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter c = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter d = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            if (Session["P_savePath"] != null)
                            {
                                string savePath = Session["P_savePath"].ToString();

                                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                var Ex = Path.GetExtension(savePath);
                                if (Ex == ".tiff" || Ex == ".tif")
                                {
                                    using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                    {
                                        image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                    }
                                }
                                byte[] _File = stream.ToArray();

                                Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                                System.IO.File.Delete(savePath);
                                Session.Remove("P_savePath");

                                if (a.Value != null)
                                    Bargsabzfileid = Convert.ToInt32(a.Value);
                            }
                            if (Session["P_savePath1"] != null)
                            {
                                string savePath = Session["P_savePath1"].ToString();

                                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                var Ex = Path.GetExtension(savePath);
                                if (Ex == ".tiff" || Ex == ".tif")
                                {
                                    using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                    {
                                        image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                    }
                                }
                                byte[] _File = stream.ToArray();

                                Car.Sp_FilesInsert(b, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                                System.IO.File.Delete(savePath);
                                Session.Remove("P_savePath1");

                                if (b.Value != null)
                                    Cartfileid = Convert.ToInt32(b.Value);
                            }
                            if (Session["P_savePath2"] != null)
                            {
                                string savePath = Session["P_savePath2"].ToString();

                                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                var Ex = Path.GetExtension(savePath);
                                if (Ex == ".tiff" || Ex == ".tif")
                                {
                                    using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                    {
                                        image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                    }
                                }
                                byte[] _File = stream.ToArray();

                                Car.Sp_FilesInsert(c, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                                System.IO.File.Delete(savePath);
                                Session.Remove("P_savePath2");

                                if (c.Value != null)
                                    Sanadfileid = Convert.ToInt32(c.Value);
                            }
                            if (Session["P_savePath3"] != null)
                            {
                                string savePath = Session["P_savePath3"].ToString();

                                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                var Ex = Path.GetExtension(savePath);
                                if (Ex == ".tiff" || Ex == ".tif")
                                {
                                    using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                    {
                                        image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                    }
                                }
                                byte[] _File = stream.ToArray();

                                Car.Sp_FilesInsert(d, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                                System.IO.File.Delete(savePath);
                                Session.Remove("P_savePath3");

                                if (d.Value != null)
                                    CartBack = Convert.ToInt32(d.Value);
                            }
                            System.Data.Entity.Core.Objects.ObjectParameter _Carid = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(long));
                            System.Data.Entity.Core.Objects.ObjectParameter carfileid = new System.Data.Entity.Core.Objects.ObjectParameter("carfileid", sizeof(int));

                            Car.sp_InsertCar_CarFile(_Carid, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN, care.fldCarModelID,
                            care.fldCarClassID, care.fldCarColorID, care.fldModel, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance),
                            "", care.fldCarPlaqueID, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque), Convert.ToInt64(Session["UserId"]),
                            "", Session["UserPass"].ToString(), Bargsabzfileid, Cartfileid, Sanadfileid, CartBack, false, null, null, care.fldTypeEntezami,Match, carfileid);
                            /*Car.sp_CarInsert(_Carid, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN, care.fldCarModelID,
                                care.fldCarClassID, care.fldCarColorID, care.fldModel,
                                MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance),
                                Convert.ToInt32(Session["UserId"]), care.fldDesc, Session["UserPass"].ToString());
                            //var q = Car.sp_CarSelect("fldVIN", care.fldVIN, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().First();
                            Car.sp_CarFileInsert(CarFileid, Convert.ToInt64(_Carid.Value), care.fldCarPlaqueID,
                                MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque),
                                Convert.ToInt32(Session["UserId"]), care.fldDesc,
                                Session["UserPass"].ToString(), Bargsabzfileid, Cartfileid,
                                Sanadfileid, CartBack, false, null, null,care.fldTypeEntezami);*/
                            SmsSender sendsms = new SmsSender();
                            //sendsms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 2, Convert.ToInt32(CarFileid.Value), "", "", "", "");
                            /*return Json(new
                            {*/
                            Msg = "ذخیره با موفقیت انجام شد. کد پرونده: " + carfileid.Value;
                                MsgTitle = "ذخیره موفق";
                                Err = 0;
                            /*}, JsonRequestBehavior.AllowGet);*/
                        }
                        //else
                        //    return Json(new
                        //    {
                        //        Msg = "لطفا فایل مدرک را آپلود کنید.",
                        //        MsgTitle = "خطا",
                        //        Err = 1
                        //    }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (Session["P_savePath1"] != null)
                        {
                            System.IO.File.Delete(Session["P_savePath1"].ToString());
                            Session.Remove("P_savePath1");
                        }
                        if (Session["P_savePath3"] != null)
                        {
                            System.IO.File.Delete(Session["P_savePath3"].ToString());
                            Session.Remove("P_savePath3");
                        }
                        if (Session["P_savePath2"] != null)
                        {
                            System.IO.File.Delete(Session["P_savePath2"].ToString());
                            Session.Remove("P_savePath2");
                        }
                        if (Session["P_savePath"] != null)
                        {
                            System.IO.File.Delete(Session["P_savePath"].ToString());
                            Session.Remove("P_savePath");
                        }
                       /* return Json(new
                        {*/
                            Msg ="شما مجاز به دسترسی نمی باشید.";
                            MsgTitle = "خطا";
                            Err = 1;
                        /*}, JsonRequestBehavior.AllowGet);*/
                        /*Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");*/
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 240))
                    {
                        if (fileee.fldAccept == true && Convert.ToInt32(Session["UserId"]) != 1)
                        {
                            return Json(new { MsgTitle = "خطا", Msg = "پرونده تأیید شده و شما قادر به ویرایش نمی باشید.", Err = 1 });
                        }

                        if (ForceScan == true && care.fldStartDateInsurance != Datesanand && care.fldSanadForoshFileId == null)
                        {
                            if (ForceScan == true && care.fldStartDateInsurance != Datesanand && Session["P_savePath2"] == null)
                            {
                                if (Session["P_savePath1"] != null)
                                {
                                    System.IO.File.Delete(Session["P_savePath1"].ToString());
                                    Session.Remove("P_savePath1");
                                }
                                if (Session["P_savePath3"] != null)
                                {
                                    System.IO.File.Delete(Session["P_savePath3"].ToString());
                                    Session.Remove("P_savePath3");
                                }
                                if (Session["P_savePath"] != null)
                                {
                                    System.IO.File.Delete(Session["P_savePath"].ToString());
                                    Session.Remove("P_savePath");
                                }
                                return Json(new
                                {
                                    Msg = "لطفا فایل سند فروش را آپلود کنید.",
                                    MsgTitle = "خطا",
                                    Err = 1
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        if (Session["P_savePath"] != null || Session["P_savePath1"] != null || Session["P_savePath2"] != null || Session["P_savePath3"] != null)
                        {
                            System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter b = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter c = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter d = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            //int? Bargsabzfileid = null;
                            //int? Cartfileid = null;
                            //int? Sanadfileid = null;
                            if ((Session["P_savePath1"] != null && Session["P_savePath3"] == null) || (Session["P_savePath1"] == null && Session["P_savePath3"]!=null))
                            {

                                if (care.fldCartFileId == null && care.fldCartBackFileId == null)
                                {
                                    if (Session["P_savePath1"] != null)
                                    {
                                        System.IO.File.Delete(Session["P_savePath1"].ToString());
                                        Session.Remove("P_savePath1");
                                    }
                                    if (Session["P_savePath3"] != null)
                                    {
                                        System.IO.File.Delete(Session["P_savePath3"].ToString());
                                        Session.Remove("P_savePath3");
                                    }
                                    if (Session["P_savePath2"] != null)
                                    {
                                        System.IO.File.Delete(Session["P_savePath2"].ToString());
                                        Session.Remove("P_savePath2");
                                    }
                                    if (Session["P_savePath"] != null)
                                    {
                                        System.IO.File.Delete(Session["P_savePath"].ToString());
                                        Session.Remove("P_savePath");
                                    }
                                    return Json(new
                                    {
                                        Msg = "صحفه اول و دوم کارت خودرو باید همزمان آپلود شوند.",
                                        MsgTitle = "خطا",
                                        Err = 1
                                    }, JsonRequestBehavior.AllowGet);
                                }
                            }

                            if (Session["P_savePath"] != null)
                            {
                                string savePath = Session["P_savePath"].ToString();

                                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                var Ex = Path.GetExtension(savePath);
                                if (Ex == ".tiff" || Ex == ".tif")
                                {
                                    using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                    {
                                        image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                    }
                                }
                                byte[] _File = stream.ToArray();

                                Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                                System.IO.File.Delete(savePath);
                                Session.Remove("P_savePath");

                                if (a.Value != null)
                                    Bargsabzfileid = Convert.ToInt32(a.Value);
                            }
                            else if (care.fldBargSabzFileId != null)
                            {
                                Bargsabzfileid = care.fldBargSabzFileId;
                            }
                            if (Session["P_savePath1"] != null)
                            {
                                string savePath = Session["P_savePath1"].ToString();

                                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                var Ex = Path.GetExtension(savePath);
                                if (Ex == ".tiff" || Ex == ".tif")
                                {
                                    using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                    {
                                        image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                    }
                                }
                                byte[] _File = stream.ToArray();

                                Car.Sp_FilesInsert(b, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                                System.IO.File.Delete(savePath);
                                Session.Remove("P_savePath1");

                                if (b.Value != null)
                                    Cartfileid = Convert.ToInt32(b.Value);
                            }
                            else if (care.fldCartFileId != null)
                            {
                                Cartfileid = care.fldCartFileId;
                            }
                            if (Session["P_savePath2"] != null)
                            {
                                string savePath = Session["P_savePath2"].ToString();

                                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                var Ex = Path.GetExtension(savePath);
                                if (Ex == ".tiff" || Ex == ".tif")
                                {
                                    using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                    {
                                        image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                    }
                                }
                                byte[] _File = stream.ToArray();

                                Car.Sp_FilesInsert(c, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                                System.IO.File.Delete(savePath);
                                Session.Remove("P_savePath2");
                                if (c.Value != null)
                                    Sanadfileid = Convert.ToInt32(c.Value);
                            }
                            else if (care.fldSanadForoshFileId != null)
                            {
                                Sanadfileid = care.fldSanadForoshFileId;
                            }
                            if (Session["P_savePath3"] != null)
                            {
                                string savePath = Session["P_savePath3"].ToString();

                                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                var Ex = Path.GetExtension(savePath);
                                if (Ex == ".tiff" || Ex == ".tif")
                                {
                                    using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                    {
                                        image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                    }
                                }
                                byte[] _File = stream.ToArray();

                                Car.Sp_FilesInsert(d, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                                System.IO.File.Delete(savePath);
                                Session.Remove("P_savePath3");
                                if (d.Value != null)
                                    CartBack = Convert.ToInt32(d.Value);
                            }
                            else if (care.fldCartBackFileId != null)
                            {
                                CartBack = care.fldCartBackFileId;
                            }
                        }
                        else if (care.fldBargSabzFileId != null || care.fldCartFileId != null || care.fldSanadForoshFileId != null || care.fldCartBackFileId != null)
                        {
                            if (care.fldCartFileId != null && care.fldCartBackFileId == null)
                            {
                                if (Session["P_savePath1"] != null)
                                {
                                    System.IO.File.Delete(Session["P_savePath1"].ToString());
                                    Session.Remove("P_savePath1");
                                }
                                if (Session["P_savePath3"] != null)
                                {
                                    System.IO.File.Delete(Session["P_savePath3"].ToString());
                                    Session.Remove("P_savePath3");
                                }
                                if (Session["P_savePath"] != null)
                                {
                                    System.IO.File.Delete(Session["P_savePath"].ToString());
                                    Session.Remove("P_savePath");
                                }
                                if (Session["P_savePath2"] != null)
                                {
                                    System.IO.File.Delete(Session["P_savePath2"].ToString());
                                    Session.Remove("P_savePath2");
                                }
                                return Json(new
                                {
                                    Msg = "لطفا صفحه2 کارت خودرو را آپلود کنید.",
                                    MsgTitle = "خطا",
                                    Err = 1
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else if (care.fldCartFileId == null && care.fldCartBackFileId != null)
                            {
                                if (Session["P_savePath1"] != null)
                                {
                                    System.IO.File.Delete(Session["P_savePath1"].ToString());
                                    Session.Remove("P_savePath1");
                                }
                                if (Session["P_savePath3"] != null)
                                {
                                    System.IO.File.Delete(Session["P_savePath3"].ToString());
                                    Session.Remove("P_savePath3");
                                }
                                if (Session["P_savePath"] != null)
                                {
                                    System.IO.File.Delete(Session["P_savePath"].ToString());
                                    Session.Remove("P_savePath");
                                }
                                if (Session["P_savePath2"] != null)
                                {
                                    System.IO.File.Delete(Session["P_savePath2"].ToString());
                                    Session.Remove("P_savePath2");
                                }
                                return Json(new
                                {
                                    Msg = "لطفا تصویر کارت خودرو را آپلود کنید.",
                                    MsgTitle = "خطا",
                                    Err = 1
                                }, JsonRequestBehavior.AllowGet);
                            }
                            Bargsabzfileid = care.fldBargSabzFileId;
                            Cartfileid = care.fldCartFileId;
                            Sanadfileid = care.fldSanadForoshFileId;
                            CartBack = care.fldCartBackFileId;
                        }
                        else if (ForceScan == true)
                        /*if (ForceScan == true && (Bargsabzfileid == null || Cartfileid == null || Sanadfileid == null || CartBack == null))*/
                        {
                            return Json(new
                            {
                                Msg = "لطفا فایل مدرک را آپلود کنید.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        /*else
                        {*/
                            Car.sp_CarUpdate(care.fldCarID, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN,
                                care.fldCarModelID, care.fldCarClassID,
                                care.fldCarColorID, care.fldModel, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance)
                                ,Match, Convert.ToInt32(Session["UserId"]), care.fldDesc, Session["UserPass"].ToString());
                            Car.sp_CarFileUpdate(care.fldID, care.fldCarID, care.fldCarPlaqueID,
                                MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque),
                                Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(),
                                Bargsabzfileid, Cartfileid, Sanadfileid, CartBack,care.fldTypeEntezami);
                            if (care.fldBargSabzFileId != null && Session["P_savePath"] != null)
                                Car.Sp_FilesDelete(care.fldBargSabzFileId);
                            if (care.fldCartFileId != null && Session["P_savePath1"] != null)
                                Car.Sp_FilesDelete(care.fldCartFileId);
                            if (care.fldSanadForoshFileId != null && Session["P_savePath2"] != null)
                                Car.Sp_FilesDelete(care.fldSanadForoshFileId);
                            if (care.fldCartBackFileId != null && Session["P_savePath3"] != null)
                                Car.Sp_FilesDelete(care.fldCartBackFileId);

                            /*Session.Remove("P_savePath");
                            Session.Remove("P_savePath1");
                            Session.Remove("P_savePath2");
                            Session.Remove("P_savePath3");*/
                            /*return Json(new
                            {*/
                            //var caruser = Car.sp_SelectCarDetils(Convert.ToInt32(care.fldCarID)).FirstOrDefault();
                            var file = Car.sp_CarFileSelect("fldId",care.fldID.ToString(),1,1,"").FirstOrDefault();
                            var caruser = Car.sp_CarUserGuestSelect("fldCarFileId", care.fldCartFileId.ToString(), "", 1).FirstOrDefault();
                            if (caruser != null)
                            {
                                Car.sp_CachedSearchCarFileUpdate(caruser.fldMotorNumber, caruser.fldShasiNumber, caruser.fldVIN, caruser.fldModel, caruser.fldCarModelName, caruser.fldCarClassName,
                               caruser.fldCarFileId, Convert.ToInt64(Session["UserId"]), "", caruser.fldName, caruser.fldMelli_EconomicCode, caruser.fldPlaqueNumber,
                               caruser.fldCarAccountName, caruser.fldCarsystemName, caruser.fldColor, caruser.fldFuleTypeName, caruser.fldAccept, caruser.fldAcceptName, caruser.fldAccountTypeId);
                            }
                            //Car.sp_CachedSearchCarFileUpdate(caruser.fldMotorNumber, caruser.fldShasiNumber, caruser.fldVIN, caruser.fldModel, caruser.fldCarModel, caruser.fldCarClassName,
                            //    care.fldID, Convert.ToInt64(Session["UserId"]), "", caruser.fldOwnerName, caruser.fldMelli_EconomicCode, caruser.fldPlaquNumber,
                            //    caruser.fldCarAccountName, caruser.fldCarSystemName, caruser.fldColor, caruser.fldFuelType,Convert.ToByte(file.fldAccept), ""
                            //    , caruser.fldCarAccountTypeID);


                                Msg = "ویرایش با موفقیت انجام شد.";
                                MsgTitle = "ویرایش موفق";
                                Err = 0;
                            /*}, JsonRequestBehavior.AllowGet);*/
                        /*}*/
                    }

                    else
                    {
                        /*return Json(new
                        {*/
                            Msg = "شما مجاز به دسترسی نمی باشید.";
                            MsgTitle = "خطا";
                            Err = 1;
                       /* }, JsonRequestBehavior.AllowGet);*/
                    }
                    if (Session["P_savePath"] != null)
                    {
                        System.IO.File.Delete(Session["P_savePath"].ToString());
                        Session.Remove("P_savePath");
                    }
                    if (Session["P_savePath1"] != null)
                    {
                        System.IO.File.Delete(Session["P_savePath1"].ToString());
                        Session.Remove("P_savePath1");
                    }
                    if (Session["P_savePath2"] != null)
                    {
                        System.IO.File.Delete(Session["P_savePath2"].ToString());
                        Session.Remove("P_savePath2");
                    }
                    if (Session["P_savePath3"] != null)
                    {
                        System.IO.File.Delete(Session["P_savePath3"].ToString());
                        Session.Remove("P_savePath3");
                    }
                }
                return Json(new
                {
                    Msg =Msg,
                    MsgTitle =MsgTitle,
                    Err = Err
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                if (Session["P_savePath"] != null)
                {
                    System.IO.File.Delete(Session["P_savePath"].ToString());
                    Session.Remove("P_savePath");
                }
                if (Session["P_savePath1"] != null)
                {
                    System.IO.File.Delete(Session["P_savePath1"].ToString());
                    Session.Remove("P_savePath1");
                }
                if (Session["P_savePath2"] != null)
                {
                    System.IO.File.Delete(Session["P_savePath2"].ToString());
                    Session.Remove("P_savePath2");
                }
                if (Session["P_savePath3"] != null)
                {
                    System.IO.File.Delete(Session["P_savePath3"].ToString());
                    Session.Remove("P_savePath3");
                }
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
        public FileContentResult Image(int id)
        {//برگرداندن عکس  
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_ShortTermCountrySelect("fldId", id.ToString(), 30, 1,"").FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldIcon != null)
                {
                    return File((byte[])pic.fldIcon, "jpg");
                }
            }
            return null;

        }
        public FileContentResult CountryImage(string symbol)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_ShortTermCountrySelect("fldSymbol", symbol, 30, 1, "").FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldIcon != null)
                {
                    return File((byte[])pic.fldIcon, "jpg");
                }
            }
            var filee = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/pargham.jpg"));
            return File(filee, "jpg");
        }
        public ActionResult DeleteParvande(string id,string state)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 239))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    var file = Car.sp_CarFileSelect("fldid", id, 0, 1, "").FirstOrDefault();                   
                    
                    if (state == "1")//حذف عمومی
                    {
                        if (file.fldAccept == true && Convert.ToInt32(Session["UserId"]) != 1)
                        {
                            return Json(new { MsgTitle = "خطا", Msg = "پرونده تأیید شده و شما قادر به حذف نمی باشید.", Er = 1 });
                        }
                        if (Convert.ToInt32(id) != 0)
                        {
                            var carfile=Car.sp_CarFileSelect("fldId", id, 1, 1, "").FirstOrDefault();
                            var q = Car.sp_CarFileSelect("fldCarID", carfile.fldCarID.ToString(), 0, 1, "").ToList();

                            if (q.Count == 1)//اگر ماشین یک پرونده داشت
                            {
                                Car.sp_CarFileDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                                Car.sp_CarDelete(carfile.fldCarID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                            }
                            else
                            {
                                Car.sp_CarFileDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                            }
                            
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
                                Msg = "رکوردی برای حذف انتخاب نشده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var carExperience = Car.sp_CarExperienceSelect("fldCarFileID", id, 0, 1, "").ToList();
                        for (int i = 0; i < carExperience.Count; i++)
                        {
                            Car.sp_CarExperienceDelete(carExperience[i].fldID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        }
                        var collection = Car.sp_CollectionSelect("fldCarFileID",id, 0, 1, "").ToList();
                        for (int i = 0; i < collection.Count; i++)
                        {
                            Car.sp_CollectionDelete(collection[i].fldID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        }
                        var peacockery = Car.sp_PeacockerySelect("fldCarFileID", id, 0, 1, "").ToList();
                        for (int i = 0; i < peacockery.Count; i++)
                        {
                            var Peacockery_Copy=Car.Sp_Peacockery_CopySelect(peacockery[i].fldID).FirstOrDefault();
                            if (Peacockery_Copy != null)
                            {
                                Car.sp_DeletePeacockery_Copy(Peacockery_Copy.fldId);
                            }
                            Car.sp_PeacockeryDelete(peacockery[i].fldID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        }
                        var onlinepay = Car.sp_OnlinePaymentsSelect("fldPeacockeryID", id, 0, 1, "").ToList();
                        for (int i = 0; i < onlinepay.Count; i++)
                        {
                            Car.sp_OnlinePaymentsDelete(onlinepay[i].fldID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        }

                        var carfile = Car.sp_CarFileSelect("fldId", id, 1, 1, "").FirstOrDefault();
                        var q = Car.sp_CarFileSelect("fldCarID", carfile.fldCarID.ToString(), 0, 1, "").ToList();
                        if (q.Count == 1)//اگر ماشین یک پرونده داشت
                        {
                            var mafasa=Car.Sp_MafasaSelect(carfile.fldCarID).ToList();
                            for (int i = 0; i < mafasa.Count; i++)
                            {
                                Car.sp_DeleteMafasa(mafasa[0].fldCarId);
                            }
                            var transcation = Car.sp_CalcTransactionSelect("fldCarId", carfile.fldCarID.ToString(), 0).ToList();
                            for (int i = 0; i < transcation.Count; i++)
                            {
                                Car.Sp_DeleteCalcTransaction(transcation[0].fldId,Convert.ToInt32(Session["UserId"]));
                            }
                            Car.sp_CarFileDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                            Car.sp_CarDelete(carfile.fldCarID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        }
                        else
                        {
                            Car.sp_CarFileDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        }
                        return Json(new
                        {
                            Msg = "حذف با موفقیت انجام شد.",
                            MsgTitle = "حذف موفق",
                            Err = 0
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                return null;
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
        public ActionResult FillDateText(string year)
        {
            if (Convert.ToInt32(year) < 1900)
                return Json(new { date = year + "/01/01" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { date = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(year + "/01/01")) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFillDate(string year,string Make)
        {
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Make != "1")
            {
                if (Convert.ToInt32(year) > 1900)
                {
                    var date = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(year + "/01/01"));
                    var s = date.Substring(0, 4);
                    for (int i = 0; i < 2; i++)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = s;
                        item.Value = s;
                        sal.Add(item);
                        s = (Convert.ToInt32(s) + 1).ToString();
                    }
                }
            }
            else
            {
                var date = year + "/01/01";
                var s = date.Substring(0, 4);
                for (int i = 0; i < 2; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = s;
                    item.Value = s;
                    sal.Add(item);
                    s = (Convert.ToInt32(s) + 1).ToString();
                }
            }
            return Json(sal.OrderByDescending(l => l.Text).Select(p1 => new { ID = p1.Value, Name = p1.Text }), JsonRequestBehavior.AllowGet);
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
        public ActionResult SaveMalek(Models.sp_OwnerSelect Owner, string Melli_EconomicCodeOld)
        {
            var Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
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
                               Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(l => l.fldID != Owner.fldID).FirstOrDefault();
                    if (Owner.fldID == 0)
                    {//ثبت رکورد جدید
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 227))
                        {

                            if (ow == null)
                            {
                                var mobile = Car.prs_MobileCount(Owner.fldMobile).FirstOrDefault();
                                if (mobile.MobileCount <= 5)
                                {
                                    if (Owner.fldDateShamsi != null && Owner.fldDateShamsi != "")
                                    {
                                        Car.sp_OwnerInsert(Owner.fldName, Owner.fldMelli_EconomicCode, Owner.fldOwnerType,
                                       Owner.fldEmail, Owner.fldMobile, Owner.fldAddress, Owner.fldPostalCode,
                                       Convert.ToInt32(Session["UserId"]), Owner.fldDesc, Session["UserPass"].ToString(),
                                       Owner.fldType, MyLib.Shamsi.Shamsi2miladiDateTime(Owner.fldDateShamsi), false);
                                    }
                                    else
                                    {
                                        Car.sp_OwnerInsert(Owner.fldName, Owner.fldMelli_EconomicCode, Owner.fldOwnerType,
                                       Owner.fldEmail, Owner.fldMobile, Owner.fldAddress, Owner.fldPostalCode,
                                       Convert.ToInt32(Session["UserId"]), Owner.fldDesc, Session["UserPass"].ToString(),
                                       Owner.fldType, null, false);
                                    }



                                    var q = Car.sp_OwnerSelect("fldMelli_EconomicCode", Owner.fldMelli_EconomicCode, 30,
                                    Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                                    SmsSender sms = new SmsSender();
                                    sms.SendMobileVerify(Convert.ToInt32(Session["UserMnu"]), q.fldID);
                                    return Json(new
                                    {
                                        Msg = "ذخیره با موفقیت انجام شد.",
                                        MsgTitle = "ذخیره موفق",
                                        Err = 0,
                                        id = q.fldID
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
                        }
                        else
                        {
                            return Json(new
                            {
                                MsgTitle = "خطا",
                                Msg = "شما مجاز به دسترسی نمی باشید.",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {//ویرایش رکورد ارسالی
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 228))
                        {
                            if (Owner.IsAccept == 1 && Convert.ToInt32(Session["UserId"]) != 1)
                            {
                                return Json(new { MsgTitle = "خطا", Msg = "مالک مورد نظر دارای پرونده تأیید شده است و لذا شما قادر به ویرایش آن نمی باشید.", Err = 1 });
                            }
                            if (ow != null)
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
                                var mobile = Car.prs_MobileCount(Owner.fldMobile).FirstOrDefault();
                                if (mobile.MobileCount <= 5)
                                {
                                    if (Owner.fldDateShamsi != null && Owner.fldDateShamsi != "")
                                    {
                                        Car.sp_OwnerUpdate(Owner.fldID, Owner.fldName, Owner.fldMelli_EconomicCode, Owner.fldOwnerType,
                                        Owner.fldEmail, Owner.fldMobile, Owner.fldAddress, Owner.fldPostalCode,
                                        Convert.ToInt32(Session["UserId"]), Owner.fldDesc, Session["UserPass"].ToString(), MyLib.Shamsi.Shamsi2miladiDateTime(Owner.fldDateShamsi), Owner.fldType);
                                    }
                                    else
                                    {
                                        Car.sp_OwnerUpdate(Owner.fldID, Owner.fldName, Owner.fldMelli_EconomicCode, Owner.fldOwnerType,
                                        Owner.fldEmail, Owner.fldMobile, Owner.fldAddress, Owner.fldPostalCode,
                                        Convert.ToInt32(Session["UserId"]), Owner.fldDesc, Session["UserPass"].ToString(), null, Owner.fldType);
                                    }
                                    Car.sp_CachedSearchCarFile_MalekUpdate(Owner.fldName, Owner.fldMelli_EconomicCode, Melli_EconomicCodeOld);

                                    return Json(new
                                    {
                                        Msg = "ویرایش با موفقیت انجام شد.",
                                        MsgTitle = "ویرایش موفق",
                                        Err = 0,
                                        id = Owner.fldID
                                    }, JsonRequestBehavior.AllowGet);
                                }else
                                {
                                    return Json(new
                                    {
                                        Msg = "شماره موبایل درج شده تکراری بوده و قبلا استفاده شده است.",
                                        MsgTitle = "ذخیره ناموفق",
                                        Err = 1
                                    }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                MsgTitle = "خطا",
                                Msg = "شما مجاز به دسترسی نمی باشید.",
                                Err = 1
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
                            id = 0
                        }, JsonRequestBehavior.AllowGet);
                    else
                    {
                        return Json(new
                        {
                            Msg = "کدملی وارد شده معتبر نمی باشد.",
                            MsgTitle = "خطا",
                            Err = 1,
                            id = 0
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
                    Err = 1
                }, JsonRequestBehavior.AllowGet);
            }
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
        public ActionResult DetailsMalek(string id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_OwnerSelect("fldID", id.ToString(), 1,
                    Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (q != null)
                {
                    /*var fldOwnerType = "0";
                    if (q.fldOwnerType)
                        fldOwnerType = "1";*/
                    var fldDateShamsi = q.fldDateShamsi;
                    if (q.fldDateShamsi == null)
                    {
                        fldDateShamsi = "";
                    }
                    return Json(new
                    {
                        fldId = q.fldID,
                        fldName = q.fldName,
                        fldMelli_EconomicCode = q.fldMelli_EconomicCode,
                        fldType=q.fldType,
                        fldOwnerType = q.fldOwnerType.ToString(),
                        fldEmail = q.fldEmail,
                        fldMobile = q.fldMobile,
                        fldAddress = q.fldAddress,
                        fldPostalCode = q.fldPostalCode,
                        fldDesc = q.fldDesc,
                        fldDateShamsi = fldDateShamsi,
                        Err=0
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        fldId = 0,
                        fldName = "",
                        fldMelli_EconomicCode = id,
                        fldOwnerType = "1",
                        fldEmail = "",
                        fldMobile = "",
                        fldAddress = "",
                        fldPostalCode = "",
                        fldDesc = "",
                        fldDateShamsi = "",
                        Err = 0
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
        public ActionResult EstelamSabt(string CodeMeli,string Tarikhtavalod)
        {
            try
            {
                RasaSabt.Estelam h = new RasaSabt.Estelam();
                string Err = "";
                var k = h.CodeMeli(CodeMeli, Tarikhtavalod, false, "admin", "40BD001563085FC35165329EA1FF5C5ECBDBBEEF",out Err);
                
                //  InSabt.InternalSabtWebServise a = new InSabt.InternalSabtWebServise();
                //SabtWs.SabtWebService s = new SabtWs.SabtWebService();
                //string p = a.GetData(CodeMeli, Tarikhtavalod, "CArtaXWebSrv");
                // var p = s.GetData(CodeMeli, Tarikhtavalod);

                if (k != null)
                {
                    var serial = k.shenasnameSerial.Substring(0, 1);
                    for (var i = 0; i < k.shenasnameSerial.Length - 1; i++)
                    {
                        serial = serial + "*";
                    }
                    return Json(new
                    {
                        state = "0",
                        FirstName = k.name,
                        LastName = k.family,
                        Name = k.name + " " + k.family,
                        fatherName = k.fatherName,
                        shenasnameNo = k.shenasnameNo,
                        shenasnameSerial = serial + "|" + k.shenasnameSeri,
                        officeName = k.officeName,
                        Error = Err
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { state = "1", Error = Err }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = "1" }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult DeletePelak(string id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 231))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_CarPlaqueDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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
                            Msg = "رکوردی برای حذف انتخاب نشده است.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
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
                return Json(new
                {
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    MsgTitle = "خطا",
                    Err = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteMalek(string id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Convert.ToInt32(id) != 0)
                {
                    Car.sp_OwnerDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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
                        Msg = "رکوردی برای حذف انتخاب نشده است.",
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
                return Json(new
                {
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    MsgTitle = "خطا",
                    Err = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SavePelak(Models.sp_CarPlaqueSelect CarPlaque, string PlaqueNumberOld)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var owner = Car.sp_OwnerSelect("fldid", CarPlaque.fldOwnerID.ToString(), 0, 1, "").FirstOrDefault();
                var mobile = Car.prs_MobileCount(owner.fldMobile).FirstOrDefault();
                if (mobile.MobileCount > 5)
                {
                    return Json(new
                    {
                        Msg = "شما به دلیل محدودیت تعداد پرونده برای یک شماره موبایل مجاز به ثبت پلاک برای مالک مورد نظر نمی باشید.",
                        MsgTitle = "ذخیره ناموفق",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                if (CarPlaque.fldDesc == null)
                    CarPlaque.fldDesc = "";
                System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                int? CharacterPersianPlaqueID = null;
                if (CarPlaque.fldCharacterPersianPlaqueID != 0)
                   CharacterPersianPlaqueID= CarPlaque.fldCharacterPersianPlaqueID;
                if (CarPlaque.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 230))
                    {
                            Car.sp_CarPlaqueInsert(id, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,
                            CarPlaque.fldPlaqueTypeID, CarPlaque.fldOwnerID, CharacterPersianPlaqueID,
                            CarPlaque.fldStatusPlaqeID, Convert.ToInt32(Session["UserId"]), CarPlaque.fldDesc, Session["UserPass"].ToString());
                           
                        return Json(new
                        {
                            Msg = "ذخیره با موفقیت انجام شد.",
                            MsgTitle = "ذخیره موفق",
                            Err = 0,
                            id = id.Value
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 232))
                    {
                        if (CarPlaque.IsAccept == 1 && Convert.ToInt32(Session["UserId"]) != 1)
                        {
                            return Json(new { MsgTitle = "خطا", Msg = "پلاک مورد نظر دارای پرونده تأیید شده است و لذا شما قادر به ویرایش آن نمی باشید.", Err = 1 });
                        }
                        Car.sp_CarPlaqueUpdate(CarPlaque.fldID, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,
                                        CarPlaque.fldPlaqueTypeID, CarPlaque.fldOwnerID, CarPlaque.fldCharacterPersianPlaqueID,
                                        CarPlaque.fldStatusPlaqeID, Convert.ToInt32(Session["UserId"]), CarPlaque.fldDesc, Session["UserPass"].ToString());
                        Car.sp_CachedSearchCarFile_PluqeUpdate(CarPlaque.fldPlaqueNumber, PlaqueNumberOld);
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
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Err = 1
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
                    Err = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DetailsPelak(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var CarPlaque = Car.sp_CarPlaqueSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                        fldId = CarPlaque.fldID,
                        fldStatusPlaqeID = CarPlaque.fldStatusPlaqeID.ToString(),
                        fldPlaqueNumber = CarPlaque.fldPlaqueNumber.ToString(),
                        fldPlaqueCityID = CarPlaque.fldPlaqueCityID.ToString(),
                        fldPlaqueSerialID = CarPlaque.fldPlaqueSerialID.ToString(),
                        fldPlaqueTypeID = CarPlaque.fldPlaqueTypeID.ToString(),
                        fldOwnerID = CarPlaque.fldOwnerID.ToString(),
                        fldCharacterPersianPlaqueID = CarPlaque.fldPlaqueCityID.ToString(),
                        fldDesc = CarPlaque.fldDesc,
                        Err=0
                    }, JsonRequestBehavior.AllowGet);

                }
                else if (type.fldName != "ملی")
                {
                    return Json(new
                    {
                        Three = CarPlaque.fldPlaqueNumber,
                        fldId = CarPlaque.fldID,
                        fldStatusPlaqeID = CarPlaque.fldStatusPlaqeID.ToString(),
                        fldPlaqueNumber = CarPlaque.fldPlaqueNumber.ToString(),
                        fldPlaqueCityID = CarPlaque.fldPlaqueCityID.ToString(),
                        fldPlaqueSerialID = CarPlaque.fldPlaqueSerialID.ToString(),
                        fldPlaqueTypeID = CarPlaque.fldPlaqueTypeID.ToString(),
                        fldOwnerID = CarPlaque.fldOwnerID.ToString(),
                        fldCharacterPersianPlaqueID = CarPlaque.fldPlaqueCityID.ToString(),
                        fldDesc = CarPlaque.fldDesc,
                        Err = 0
                    }, JsonRequestBehavior.AllowGet);

                }
                return Json("");
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Err = 1 });
            }
        }
        public ActionResult DetailSubSetting()
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var subSett = Car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), Car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                int? fldDefaultPelakSerial = 0;
                int? fldDefaultPelakChar = 0;
                if (subSett != null)
                {
                    if (subSett.fldDefaultPelakSerial != null)
                        fldDefaultPelakSerial = subSett.fldDefaultPelakSerial;
                    if (subSett.fldDefaultPelakChar != null)
                        fldDefaultPelakChar = subSett.fldDefaultPelakChar;
                }
                return Json(new
                {
                    fldDefaultPelakSerial = fldDefaultPelakSerial,
                    fldDefaultPelakChar = fldDefaultPelakChar
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public ActionResult Malek_Pelak(string CarID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            ViewData.Model = new Avarez.Models.Parvande();
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 700))
            {
                //Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                var result = new Ext.Net.MVC.PartialViewResult
                {
                   
                    ViewData = this.ViewData
                };
                result.ViewBag.CarID = CarID;
                return result;
            }
            else
            {
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "شما مجاز به دسترسی نمی باشید.",
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DetailSearchSetting()
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var subSett = Car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), Car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                int? fldDefaultSearch = 0;
                if (subSett != null)
                {
                    fldDefaultSearch = subSett.fldDefaultSearch;
                }
                return Json(new
                {
                    fldDefaultSearch = fldDefaultSearch.ToString()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }
        public ActionResult FastParvande(string containerId)
        {//باز شدن پرونده جدید
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->ثبت پرونده سریع");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            ViewData.Model = new Avarez.Models.Parvande();            
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult GetPelakHa(int? MalekId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var CarPlaque = car.sp_CarPlaqueSelect("fldOwnerID", MalekId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CarPlaque.Select(p1 => new { ID = p1.fldID, Name = p1.fldPlaqueNumber }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMalekInfo(string MeliCode)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var m = Car.sp_OwnerSelect("fldMelli_EconomicCode", MeliCode, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                
                bool IsMalek = false;
                var MalekId = 0.0;
                string fldMobile = "";
                string fldName = "";
                string fldTarikh = "";
                bool HavePelak = false;
                if (m != null)
                {
                    var p = Car.sp_CarPlaqueSelect("fldOwnerID", m.fldID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    IsMalek = true;
                    MalekId = m.fldID;
                    fldName=m.fldName;
                    fldMobile=m.fldMobile;
                    fldTarikh = m.fldDateShamsi;
                    if (p != null)
                    HavePelak = true;
                }
                
                return Json(new
                {
                    MalekId = MalekId,
                    IsMalek = IsMalek,
                    fldName = fldName,
                    fldMobile = fldMobile,
                    fldTarikh = fldTarikh,
                    HavePelak = HavePelak
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }
        public ActionResult PelakType(string PelakId)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var p = Car.sp_CarPlaqueSelect("fldId", PelakId, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                
                return Json(new
                {
                    pelakType = p.fldPlaqueTypeName
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }
        public ActionResult F_Upload()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

                if (Session["F_savePath"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["F_savePath"].ToString());
                    System.IO.File.Delete(Session["F_savePath"].ToString());
                    Session.Remove("F_savePath");
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                {
                    if (Request.Files[0].ContentLength <= 716800)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        var Name = Path.GetFileNameWithoutExtension(file.FileName);
                        
                        string savePath = Server.MapPath(@"~\Uploaded\" + Guid.NewGuid().ToString() + extension);
                        file.SaveAs(savePath);
                        Session["F_savePath"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult F_Upload1()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

                if (Session["F_savePath1"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["F_savePath1"].ToString());
                    System.IO.File.Delete(Session["F_savePath1"].ToString());
                    Session.Remove("F_savePath1");
                }
                var extension = Path.GetExtension(Request.Files[1].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                {
                    if (Request.Files[1].ContentLength <= 716800)
                    {
                        HttpPostedFileBase file = Request.Files[1];
                        var Name = Path.GetFileNameWithoutExtension(file.FileName);
                        string savePath = Server.MapPath(@"~\Uploaded\" + Guid.NewGuid().ToString() + extension);
                        file.SaveAs(savePath);
                        Session["F_savePath1"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[1].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult F_Upload2()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

                if (Session["F_savePath2"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["F_savePath2"].ToString());                    
                    System.IO.File.Delete(Session["F_savePath2"].ToString());
                    Session.Remove("F_savePath2");
                }
                var extension = Path.GetExtension(Request.Files[3].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                {
                    if (Request.Files[3].ContentLength <= 716800)
                    {
                        HttpPostedFileBase file = Request.Files[3];
                        var Name = Path.GetFileNameWithoutExtension(file.FileName);
                        string savePath = Server.MapPath(@"~\Uploaded\" + Guid.NewGuid().ToString() + extension);
                        file.SaveAs(savePath);
                        Session["F_savePath2"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[3].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult F_Upload3()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

                if (Session["F_savePath3"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["F_savePath3"].ToString());
                    System.IO.File.Delete(Session["F_savePath3"].ToString());
                    Session.Remove("F_savePath3");
                }
                var extension = Path.GetExtension(Request.Files[2].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                {
                    if (Request.Files[2].ContentLength <= 716800)
                    {
                        HttpPostedFileBase file = Request.Files[2];
                        var Name = Path.GetFileNameWithoutExtension(file.FileName);
                        string savePath = Server.MapPath(@"~\Uploaded\" + Guid.NewGuid().ToString() + extension);
                        file.SaveAs(savePath);
                        Session["F_savePath3"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[2].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult SaveFastParvande(Models.sp_OwnerSelect Owner, Models.CarFile care, Models.sp_CarPlaqueSelect CarPlaque,string carmake,bool Match)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 227) && Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 230) && Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 238))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    var ow1 = Car.sp_OwnerSelect("fldMelli_EconomicCode", Owner.fldMelli_EconomicCode, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    if (ow1 == null)
                    {
                        var mobile = Car.prs_MobileCount(Owner.fldMobile).FirstOrDefault();
                        if (mobile.MobileCount > 5)
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
                        var mobile = Car.prs_MobileCount(ow1.fldMobile).FirstOrDefault();
                        if (mobile.MobileCount > 5)
                        {
                            return Json(new
                            {
                                Msg = "شماره موبایل درج شده تکراری بوده و قبلا استفاده شده است.",
                                MsgTitle = "ذخیره ناموفق",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    System.Data.Entity.Core.Objects.ObjectParameter _Pid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    System.Data.Entity.Core.Objects.ObjectParameter _Carid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(long));
                    System.Data.Entity.Core.Objects.ObjectParameter carfileid = new System.Data.Entity.Core.Objects.ObjectParameter("carfileid", sizeof(int));

                    int? Bargsabzfileid = null;
                    int? Cartfileid = null;
                    int? Sanadfileid = null;
                    int? CartBack = null;
                    string Datesanand = "";

                    if (carmake == "1")
                    {
                        Datesanand = care.fldModel + "/01/01";
                    }
                    else
                    {
                        var datee = Convert.ToDateTime(care.fldModel + "-01-01");
                        Datesanand = MyLib.Shamsi.Miladi2ShamsiString(datee);
                    }

                    var subSett = Car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), Car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                    bool? ForceScan = true;
                    if (subSett != null)
                    {
                        ForceScan = subSett.fldHaveScan;
                    }

                    bool isupload = false;
                    if (Session["F_savePath"] != null)
                        isupload = true;
                    else if (Session["F_savePath1"] != null && Session["F_savePath3"] != null)
                        isupload = true;
                    else if (Session["F_savePath2"] != null)
                        isupload = true;

                    if (ForceScan == true && ((Session["F_savePath1"] != null && Session["F_savePath3"] == null) || (Session["F_savePath1"] == null && Session["F_savePath3"] != null)))
                    {
                        if (Session["F_savePath1"] != null)
                        {
                            System.IO.File.Delete(Session["F_savePath1"].ToString());
                            Session.Remove("F_savePath1");
                        }
                        if (Session["F_savePath"] != null)
                        {
                            System.IO.File.Delete(Session["F_savePath"].ToString());
                            Session.Remove("F_savePath");
                        }
                        if (Session["F_savePath2"] != null)
                        {
                            System.IO.File.Delete(Session["F_savePath2"].ToString());
                            Session.Remove("F_savePath2");
                        }
                        if (Session["F_savePath3"] != null)
                        {
                            System.IO.File.Delete(Session["F_savePath3"].ToString());
                            Session.Remove("F_savePath3");
                        }
                        return Json(new
                        {
                            Msg = "صفحات اول و دوم کارت خودرو باید همزمان آپلود شوند.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else if (ForceScan == true && isupload == false)
                    {
                        return Json(new
                        {
                            Msg = "لطفا فایل مدرک را آپلود کنید.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else if (ForceScan == true && care.fldStartDateInsurance != Datesanand && Session["F_savePath2"] == null)
                    {
                        if (Session["F_savePath1"] != null)
                        {
                            System.IO.File.Delete(Session["F_savePath1"].ToString());
                            Session.Remove("F_savePath1");
                        }
                        if (Session["F_savePath"] != null)
                        {
                            System.IO.File.Delete(Session["F_savePath"].ToString());
                            Session.Remove("F_savePath");
                        }
                        if (Session["F_savePath3"] != null)
                        {
                            System.IO.File.Delete(Session["F_savePath3"].ToString());
                            Session.Remove("F_savePath3");
                        }

                        return Json(new
                        {
                            Msg = "لطفا فایل سند فروش را آپلود کنید.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    }

                    int chck = 1;
                    if (Owner.fldOwnerType == 1)
                        chck = checks(Owner.fldMelli_EconomicCode);
                    if (chck == 1)
                    {
                        if (Owner.fldID == 0)
                        {//ثبت مالک جدید
                            var ow = Car.sp_OwnerSelect("fldMelli_EconomicCode", Owner.fldMelli_EconomicCode, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            if (ow == null)
                            {
                                var mobile = Car.prs_MobileCount(Owner.fldMobile).FirstOrDefault();
                                if (mobile.MobileCount <= 5)
                                {
                                    if (Owner.fldDateShamsi != null && Owner.fldDateShamsi != "")
                                    {
                                        Car.sp_OwnerInsert(Owner.fldName, Owner.fldMelli_EconomicCode, Owner.fldOwnerType, "", Owner.fldMobile, "", "",
                                            Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Owner.fldType, MyLib.Shamsi.Shamsi2miladiDateTime(Owner.fldDateShamsi), false);
                                    }
                                    else
                                    {
                                        Car.sp_OwnerInsert(Owner.fldName, Owner.fldMelli_EconomicCode, Owner.fldOwnerType, "", Owner.fldMobile, "", "",
                                            Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Owner.fldType, null, false);
                                    }

                                    Owner.fldID = Car.sp_OwnerSelect("fldMelli_EconomicCode", Owner.fldMelli_EconomicCode, 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldID;

                                    SmsSender sms = new SmsSender();
                                    sms.SendMobileVerify(Convert.ToInt32(Session["UserMnu"]), Owner.fldID);
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
                                    Msg = "کد ملی وارد شده تکراری است.",
                                    MsgTitle = "خطا",
                                    Err = 1,
                                    id = 0
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        if (care.fldCarPlaqueID == 0)
                        {//ثبت پلاک جدید
                            if (CarPlaque.fldCharacterPersianPlaqueID == 0)
                                Car.sp_CarPlaqueInsert(_Pid, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,CarPlaque.fldPlaqueTypeID, 
                                    Owner.fldID, null, CarPlaque.fldStatusPlaqeID, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                            else
                                Car.sp_CarPlaqueInsert(_Pid, CarPlaque.fldPlaqueNumber, CarPlaque.fldPlaqueCityID, CarPlaque.fldPlaqueSerialID,CarPlaque.fldPlaqueTypeID, Owner.fldID, 
                                CarPlaque.fldCharacterPersianPlaqueID,CarPlaque.fldStatusPlaqeID, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());

                            care.fldCarPlaqueID = Convert.ToInt32(_Pid.Value);
                        }
                        //ثبت پرونده

                        //if (ForceScan == true && (Session["F_savePath"] != null || Session["F_savePath1"] != null || Session["F_savePath2"] != null || Session["F_savePath3"] != null))
                        //{
                        System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                        System.Data.Entity.Core.Objects.ObjectParameter b = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                        System.Data.Entity.Core.Objects.ObjectParameter c = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                        System.Data.Entity.Core.Objects.ObjectParameter d = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                        if (Session["F_savePath"] != null)
                        {
                            string savePath = Session["F_savePath"].ToString();

                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                            var Ex = Path.GetExtension(savePath);
                            if (Ex == ".tiff" || Ex == ".tif")
                            {
                                using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                {
                                    image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                }
                            }
                            byte[] _File = stream.ToArray();

                            Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                            System.IO.File.Delete(savePath);
                            Session.Remove("F_savePath");

                            if (a.Value != null)
                                Bargsabzfileid = Convert.ToInt32(a.Value);
                        }
                        if (Session["F_savePath1"] != null)
                        {
                            string savePath = Session["F_savePath1"].ToString();

                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                            var Ex = Path.GetExtension(savePath);
                            if (Ex == ".tiff" || Ex == ".tif")
                            {
                                using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                {
                                    image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                }
                            }
                            byte[] _File = stream.ToArray();

                            Car.Sp_FilesInsert(b, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                            System.IO.File.Delete(savePath);
                            Session.Remove("F_savePath1");

                            if (b.Value != null)
                                Cartfileid = Convert.ToInt32(b.Value);
                        }
                        if (Session["F_savePath2"] != null)
                        {
                            string savePath = Session["F_savePath2"].ToString();

                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                            var Ex = Path.GetExtension(savePath);
                            if (Ex == ".tiff" || Ex == ".tif")
                            {
                                using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                {
                                    image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                }
                            }
                            byte[] _File = stream.ToArray();

                            Car.Sp_FilesInsert(c, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                            System.IO.File.Delete(savePath);
                            Session.Remove("F_savePath2");

                            if (c.Value != null)
                                Sanadfileid = Convert.ToInt32(c.Value);
                        }
                        if (Session["F_savePath3"] != null)
                        {
                            string savePath = Session["F_savePath3"].ToString();

                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                            var Ex = Path.GetExtension(savePath);
                            if (Ex == ".tiff" || Ex == ".tif")
                            {
                                using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                {
                                    image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                }
                            }
                            byte[] _File = stream.ToArray();

                            Car.Sp_FilesInsert(d, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                            System.IO.File.Delete(savePath);
                            Session.Remove("F_savePath3"); 

                            if (d.Value != null)
                                CartBack = Convert.ToInt32(d.Value);
                        }
                        Car.sp_InsertCar_CarFile(_Carid, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN, care.fldCarModelID,
                            care.fldCarClassID, care.fldCarColorID, care.fldModel, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance), 
                            "", care.fldCarPlaqueID, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque), Convert.ToInt64(Session["UserId"]),
                            "", Session["UserPass"].ToString(), Bargsabzfileid, Cartfileid, Sanadfileid, CartBack, false, null, null, care.fldTypeEntezami,Match, carfileid);
                        /*Car.sp_CarInsert(_Carid, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN, care.fldCarModelID,
                            care.fldCarClassID, care.fldCarColorID, care.fldModel, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance),
                            Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                        //var q = Car.sp_CarSelect("fldVIN", care.fldVIN, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().First();
                        Car.sp_CarFileInsert(CarFileid, Convert.ToInt64(_Carid.Value), care.fldCarPlaqueID, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque),
                            Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Bargsabzfileid, Cartfileid, Sanadfileid, CartBack, false, null, null, care.fldTypeEntezami);*/

                        return Json(new
                        {
                            Msg = "ذخیره با موفقیت انجام شد. کد پرونده: " + carfileid.Value,
                            MsgTitle = "ذخیره موفق",
                            Err = 0,
                            carId=_Carid.Value.ToString(),
                            carFileId = carfileid.Value.ToString()
                        }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        if (Session["F_savePath"] != null)
                        {
                            System.IO.File.Delete(Session["F_savePath"].ToString());
                            Session.Remove("F_savePath");
                        }
                        if (Session["F_savePath2"] != null)
                        {
                            System.IO.File.Delete(Session["F_savePath2"].ToString());
                            Session.Remove("F_savePath2");
                        }
                        if (Session["F_savePath3"] != null)
                        {
                            System.IO.File.Delete(Session["F_savePath3"].ToString());
                            Session.Remove("F_savePath3");
                        }
                        if (Session["F_savePath1"] != null)
                        {
                            System.IO.File.Delete(Session["F_savePath1"].ToString());
                            Session.Remove("F_savePath1");
                        }
                        return Json(new
                        {
                            Msg = "کد ملی وارد شده معتبر نمی باشد.",
                            MsgTitle = "خطا",
                            Err = 1,
                            id = 0
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    if (Session["F_savePath"] != null)
                    {
                        System.IO.File.Delete(Session["F_savePath"].ToString());
                        Session.Remove("F_savePath");
                    }
                    if (Session["F_savePath2"] != null)
                    {
                        System.IO.File.Delete(Session["F_savePath2"].ToString());
                        Session.Remove("F_savePath2");
                    }
                    if (Session["F_savePath3"] != null)
                    {
                        System.IO.File.Delete(Session["F_savePath3"].ToString());
                        Session.Remove("F_savePath3");
                    }
                    if (Session["F_savePath1"] != null)
                    {
                        System.IO.File.Delete(Session["F_savePath1"].ToString());
                        Session.Remove("F_savePath1");
                    }
                    return Json(new
                    {
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        MsgTitle = "خطا",
                        Err = 1,
                        id = 0
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                if (Session["F_savePath"] != null)
                {
                    System.IO.File.Delete(Session["F_savePath"].ToString());
                    Session.Remove("F_savePath");
                }
                if (Session["F_savePath2"] != null)
                {
                    System.IO.File.Delete(Session["F_savePath2"].ToString());
                    Session.Remove("F_savePath2");
                }
                if (Session["F_savePath3"] != null)
                {
                    System.IO.File.Delete(Session["F_savePath3"].ToString());
                    Session.Remove("F_savePath3");
                }
                if (Session["F_savePath1"] != null)
                {
                    System.IO.File.Delete(Session["F_savePath1"].ToString());
                    Session.Remove("F_savePath1");
                }

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
        public ActionResult CheckVIN(string Id,string VIN)
        {
            string Msg = ""; string MsgTitle = ""; string Er = "0";
            if (Session["UserId"] == null && Session["GeustId"] == null && Session["UserGeust"] == null)
            {
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            }
            var UserId = "20";/*مهمان*/
            if (Session["GeustId"] == null && Session["UserGeust"] == null && Session["UserId"]!=null)/*کاربر نرم افزار*/
                UserId = Session["UserId"].ToString();
            else if (Session["GeustId"] == null && Session["UserId"] == null && Session["UserGeust"] != null)/*کاربر خوداظهاری*/
                UserId = Session["UserGeust"].ToString();
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (VIN != "") { 
                var q=Car.sp_CarSelect("fldVIN",VIN,0,1,"").Where(l => l.fldID.ToString() != Id).FirstOrDefault();
                if(q!=null)
                {
                    Msg = "VIN وارد شده تکراری می باشد.";
                    MsgTitle = "خطا";
                    Er = "1";
                }
                    }
                //else if(Id!="" && q!=null && q.fldID.ToString()!=Id)
                //{
                //    Msg = "VIN وارد شده تکراری می باشد.";
                //    MsgTitle = "خطا";
                //    Er = "1";
                //}
                return Json(new { Msg=Msg,MsgTitle=MsgTitle, Er =Er });
            }
            catch (Exception x)
            {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    string InnerException = "";
                    if (x.InnerException != null)
                        InnerException = x.InnerException.Message;
                    Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(UserId), x.Message, DateTime.Now, Session["UserPass"].ToString());
                    return Json(new {MsgTitle="خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
                }
        }

        public ActionResult CheckMotorNumber(string Id, int CarSystem, string fldMotorNumber)
        {
            string Msg = ""; string MsgTitle = ""; string Er = "0";
            if (Session["UserId"] == null && Session["GeustId"] == null && Session["UserGeust"] == null)
            {
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            }
            var UserId = "20";/*مهمان*/
            if (Session["GeustId"] == null && Session["UserGeust"] == null && Session["UserId"] != null)/*کاربر نرم افزار*/
                UserId = Session["UserId"].ToString();
            else if (Session["GeustId"] == null && Session["UserId"] == null && Session["UserGeust"] != null)/*کاربر خوداظهاری*/
                UserId = Session["UserGeust"].ToString();
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_CarSelect("CarSystem_MotorNumber",fldMotorNumber,0, CarSystem,  "").Where(l=>l.fldID.ToString() != Id).FirstOrDefault();
                if (q != null)
                {
                    Msg = "شماره موتور وارد شده تکراری می باشد.";
                    MsgTitle = "خطا";
                    Er = "1";
                }
                //else if (Id != "" && q != null && )
                //{
                //    Msg = "شماره موتور وارد شده تکراری می باشد.";
                //    MsgTitle = "خطا";
                //    Er = "1";
                //}
                return Json(new { Msg = Msg, MsgTitle = MsgTitle, Er = Er });
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }

        public ActionResult changeOwnerofRPlaque(string PlaqueId, string NewOwnerId, bool Accept)
        {
            byte Er = 0;
            if (Session["UserId"] == null)
            return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities Car = new Models.cartaxEntities();
            var q = Car.sp_CarPlaqueSelect("fldId", PlaqueId, 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var owner = Car.sp_OwnerSelect("fldId", q.fldOwnerID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var NewOwnerr = Car.sp_OwnerSelect("fldId", NewOwnerId, 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.NameFamily = owner.fldName;
            PartialView.ViewBag.Mobile = owner.fldMobile;
            PartialView.ViewBag.CodeMeli = owner.fldMelli_EconomicCode;
            PartialView.ViewBag.NameFamilyNew = NewOwnerr.fldName;
            PartialView.ViewBag.MobileNew = NewOwnerr.fldMobile;
            PartialView.ViewBag.CodeMeliNew = NewOwnerr.fldMelli_EconomicCode;
            PartialView.ViewBag.PlaqueId = PlaqueId;
            PartialView.ViewBag.NewOwnerId = NewOwnerId;
            PartialView.ViewBag.Accept = Accept;
            return PartialView;
        }
        public ActionResult CheckPelakNumber(string Id, int fldPlaqueCityID, int fldPlaqueSerialID, string fldPelakNumber, string fldPlaqueTypeName)
        {
            string Msg = ""; string MsgTitle = ""; string Er = "0"; bool Edit = true; int plaqeID = 0; bool acc = true;
            if (Session["UserId"] == null && Session["GeustId"] == null && Session["UserGeust"] == null)
            {
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            }
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_CarPlaqueSelect("fldPlaqueNumber", fldPelakNumber, 0, 1, "").Where(l => l.fldID.ToString() != Id && l.fldPlaqueCityID == fldPlaqueCityID && l.fldPlaqueSerialID == fldPlaqueSerialID && l.fldOwnerID == 0).FirstOrDefault();
                if (q != null)
                {
                    var carFile = Car.sp_CarFileSelect("fldCarPlaqueID", q.fldID.ToString(), 1, 1, "").FirstOrDefault();
                    /*if (carFile.fldAccept == false)
                    {*/
                    if (Session["UserId"] == null && Session["GeustId"] == null && Session["UserGeust"] != null)
                    {
                        Msg = "پلاک وارد شده تکراری می باشد.";
                        MsgTitle = "خطا";
                        Er = "1";
                        Edit = false;
                    }
                    else if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 411))
                    {
                        if (carFile != null)
                        {
                            if (carFile.fldAccept == false)
                            {
                                Msg = "پلاک وارد شده تکراری می باشد. آیا مایلید مالک پلاک مورد نظر ویرایش شود؟";
                                MsgTitle = "خطا";
                                Er = "1";
                                plaqeID = q.fldID;
                                acc = false;
                            }
                            else
                            {
                                Msg = "پلاک وارد شده تکراری می باشد. آیا مایلید مالک پلاک مورد نظر ویرایش شود؟";
                                MsgTitle = "خطا";
                                Er = "1";
                                plaqeID = q.fldID;
                                if (Convert.ToInt32(Session["UserId"]) == 1)
                                {
                                    acc = false;
                                }
                            }
                        }
                        else
                        {
                            Msg = "پلاک وارد شده تکراری می باشد. آیا مایلید مالک پلاک مورد نظر ویرایش شود؟";
                            MsgTitle = "خطا";
                            Er = "1";
                            plaqeID = q.fldID;
                            acc = false;
                        }
                    }
                    else
                    {
                        Msg = "پلاک وارد شده تکراری می باشد.";
                        MsgTitle = "خطا";
                        Er = "1";
                        Edit = false;
                    }
                   /* }
                    else
                    {//پرونده تایید شده است
                        Msg = "پلاک وارد شده تکراری می باشد";
                        MsgTitle = "خطا";
                        Er = "1";
                        Edit = false;
                    }*/
                }
                //else if (Id != "" && q != null && q.fldID.ToString() != Id)
                //{
                //    Msg = "پلاک وارد شده تکراری می باشد.";
                //    MsgTitle = "خطا";
                //    Er = "1";
                //}
                return Json(new {acc=acc, Edit = Edit, Msg = Msg, MsgTitle = MsgTitle, Er = Er, plaqeID = plaqeID });
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }

        public ActionResult SaveNewOwnerP(string NewOwnerId, string PlaqueId)
        {
            byte Er = 0;
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                Car.Sp_UpdateCarPlaque_Owner(Convert.ToInt32(NewOwnerId), Convert.ToInt32(PlaqueId));
                return Json(new
                {
                    Msg = "عملیات با موفقیت انجام شد.",
                    MsgTitle = "عملیات موفق",
                    Er = Er
                }, JsonRequestBehavior.AllowGet);
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
    }
}
