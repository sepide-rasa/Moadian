using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.Config
{
    [Authorize]
    public class SubSettingController : Controller
    {

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 181))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_SubSettingSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);

        }
        public JsonResult GetCascadeRound()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_RoundSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldRound }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(Models.SubSetting SubSetting,bool taeed)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (SubSetting.fldDesc == null)
                    SubSetting.fldDesc = "";
                DateTime? fldAzAkharinTarikh = null;
                if (SubSetting.fldCalcFromVariz)
                    fldAzAkharinTarikh= MyLib.Shamsi.Shamsi2miladiDateTime(SubSetting.fldAzAkharinTarikh);
                if (SubSetting.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 182))
                    {
                        Car.sp_SubSettingInsert(SubSetting.fldStartCodeBillIdentity, SubSetting.fldRoundID,
                            SubSetting.fldPrintBill_Payment, SubSetting.fldExemptNewProduction, SubSetting.fldTitleUserReport,
                            SubSetting.fldLastRespitePayment, SubSetting.fldTypeCountryDivisions,
                            SubSetting.fldCodeCountryDivisions, Convert.ToInt32(Session["UserId"]), SubSetting.fldDesc,
                            MyLib.Shamsi.Shamsi2miladiDateTime(SubSetting.fldImplementationDate),
                            SubSetting.fldCountryDivisionsTreeApply, SubSetting.fldTypeCar, SubSetting.fldCodeCar,
                            SubSetting.fldCarSeriesTreeApply, Session["UserPass"].ToString(),
                            SubSetting.fldCalcFromVariz, fldAzAkharinTarikh, SubSetting.fldDefaultPelakSerial,SubSetting.fldDefaultPelakChar,
                            SubSetting.fldDefaultSearch, SubSetting.fldHaveScan, taeed, SubSetting.fldMobileVerify, SubSetting.fldExpScan);

                        return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 184))
                    {
                        Car.sp_SubSettingUpdate(SubSetting.fldID, SubSetting.fldStartCodeBillIdentity, SubSetting.fldRoundID,
                            SubSetting.fldPrintBill_Payment, SubSetting.fldExemptNewProduction, SubSetting.fldTitleUserReport,
                            SubSetting.fldLastRespitePayment, SubSetting.fldTypeCountryDivisions,
                            SubSetting.fldCodeCountryDivisions, Convert.ToInt32(Session["UserId"]), SubSetting.fldDesc,
                            MyLib.Shamsi.Shamsi2miladiDateTime(SubSetting.fldImplementationDate),
                            SubSetting.fldCountryDivisionsTreeApply, SubSetting.fldTypeCar, SubSetting.fldCodeCar,
                            SubSetting.fldCarSeriesTreeApply, Session["UserPass"].ToString(), SubSetting.fldCalcFromVariz,
                            fldAzAkharinTarikh, SubSetting.fldDefaultPelakSerial, SubSetting.fldDefaultPelakChar,
                            SubSetting.fldDefaultSearch, SubSetting.fldHaveScan, taeed,SubSetting.fldMobileVerify,SubSetting.fldExpScan);
                        return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public JsonResult _CountryTree(int? id)
        {
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            //پروسیجر تشخیص کاربر در تقسیمات کشوری برای
            //لود درخت همان ناحیه ای کاربر در سطح آن تعریف شده 
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (id != null)
            {
                var rols = (from k in p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var rols = (from k in p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult _CarTree(int? id)
        {
            string url = Url.Content("~/Content/images/c");
            var p = new Models.cartaxEntities();
            if (id != null)
            {
                var rols = (from k in p.sp_TableTreeCarSelect("fldPID", id.ToString(), 0, 0, 0)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeCarSelect("fldPID", id.ToString(), 0, 0, 0).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var rols = (from k in p.sp_TableTreeCarSelect("fldId", "1", 0, 0, 0)
                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeCarSelect("fldId", "1", 0, 0, 0).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CountryPosition(int id)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var nodes = car.sp_SelectUpTreeCountryDivisions(id, 1, "").ToList();
            ArrayList ar = new ArrayList();

            foreach (var item in nodes)
            {
                ar.Add(item.fldNodeName);
            }
            string nodeNames = "";
            for (int i = 0; i < ar.Count; i++)
            {
                if (i < ar.Count - 1)
                    nodeNames += ar[i].ToString() + "-->";
                else
                    nodeNames += ar[i].ToString();
            }

            return Json(new { Position = nodeNames });
        }

        public JsonResult CarPosition(int id)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var nodes = car.sp_SelectUpTreeCarSeries(id, 1, "").ToList();
            ArrayList ar = new ArrayList();

            foreach (var item in nodes)
            {
                ar.Add(item.fldNodeName);
            }
            string nodeNames = "";
            for (int i = 0; i < ar.Count; i++)
            {
                if (i < ar.Count - 1)
                    nodeNames += ar[i].ToString() + "-->";
                else
                    nodeNames += ar[i].ToString();
            }

            return Json(new { Position = nodeNames });
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldCountryDivisionsName", "fldImplementationDate" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_SubSettingSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 183))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_SubSettingDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 });
                    }
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var SubSetting = Car.sp_SubSettingSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var countryId = Car.sp_TableTreeSelect("fldSourceID", SubSetting.CoutryDivisionCode.ToString(), 0, 0, 0).Where(h => h.fldNodeType == SubSetting.CoutryDivisionType).FirstOrDefault();
                var CarId = Car.sp_TableTreeCarSelect("fldSourceID", SubSetting.CarSeriesCode.ToString(), 0, 0, 0).Where(h => h.fldNodeType == SubSetting.CarSeriesType).FirstOrDefault();
                string fldAzAkharinTarikh = "";
                if (SubSetting.fldAzAkharinTarikh != null)
                    fldAzAkharinTarikh = MyLib.Shamsi.Miladi2ShamsiString((DateTime)SubSetting.fldAzAkharinTarikh);
                return Json(new
                {
                    fldDefaultPelakChar = SubSetting.fldDefaultPelakChar,
                    fldDefaultPelakSerial = SubSetting.fldDefaultPelakSerial,
                    fldDefaultSearch = SubSetting.fldDefaultSearch,
                    fldId = SubSetting.fldID,
                    fldExemptNewProduction = SubSetting.fldExemptNewProduction,
                    fldImplementationDate = SubSetting.fldImplementationDate,
                    fldLastRespitePayment = SubSetting.fldLastRespitePayment,
                    fldPrintBill_Payment = SubSetting.fldPrintBill_Payment,
                    fldRoundID = SubSetting.fldRoundID,
                    fldStartCodeBillIdentity = SubSetting.fldStartCodeBillIdentity,
                    fldTitleUserReport = SubSetting.fldTitleUserReport,
                    fldCountryDivisionsTreeApply = SubSetting.fldCountryDivisionsTreeApply,
                    fldHaveScan=SubSetting.fldHaveScan,
                    fldCarSeriesTreeApply = SubSetting.fldCarSeriesTreeApply,
                    fldDesc = SubSetting.fldDesc,
                    CountryType = SubSetting.CoutryDivisionType,
                    countryCode = SubSetting.CoutryDivisionCode,
                    carType = SubSetting.CarSeriesType,
                    carCode = SubSetting.CarSeriesCode,
                    carid = CarId.fldID,
                    countryId = countryId.fldID,
                    fldCalcFromVariz = SubSetting.fldCalcFromVariz,
                    fldAzAkharinTarikh = fldAzAkharinTarikh
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

    }
}
