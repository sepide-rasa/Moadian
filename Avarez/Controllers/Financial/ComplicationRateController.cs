using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Kendo.Mvc.UI;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.Financial
{
    [Authorize]
    public class ComplicationRateController : Controller
    {
        //
        // GET: /ComplicationRate/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 116))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
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
                var rols = (from k in p.sp_TableTreeCarSelect("fldPID", id.ToString(), 0, 0, 0).OrderBy(l=>l.fldNodeName)
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
                var rols = (from k in p.sp_TableTreeCarSelect("fldId", "1", 0, 0, 0).OrderBy(l => l.fldNodeName)
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
            var nodes = car.sp_SelectUpTreeCountryDivisions(id, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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
            var nodes = car.sp_SelectUpTreeCarSeries(id, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            ArrayList ar = new ArrayList();
            string nodeNames = ""; bool error = false;
            foreach (var item in nodes)
            {
                ar.Add(item.fldNodeName);
            }
            
            for (int i = 0; i < ar.Count; i++)
            {

                if (i < ar.Count - 1)
                    nodeNames += ar[i].ToString() + "-->";
                else
                    nodeNames += ar[i].ToString();
            }
            if (Convert.ToInt32(Session["UserId"]) != 1 && (nodeNames.ToString().Contains("سواری") || nodeNames.ToString().Contains("آمبولانس") || nodeNames.ToString().Contains("وانت دوکابین")))
            {
                nodeNames = "شما مجاز به انتخاب این گزینه نمی باشید.";
                error = true;
                return Json(new { Position = nodeNames, error = error });
            }
            return Json(new { Position = nodeNames });
        }

        public ActionResult Reload(int type, int value, bool check, string year, int CountryCode, int CountryType)
        {//جستجو            
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_SelectFullCarForSetMony(type, value, check, year, CountryType, CountryCode).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NotNullReload(int type, int value, string year, int CountryCode, int CountryType)
        {//جستجو            
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_SelectFullCarForSetMonyFullNotNull(type, value, year, CountryType, CountryCode, Convert.ToInt32(Session["UserId"])).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

   
        [HttpPost]
        public ActionResult Grid_Save(List<Models.Rate> ArrayL)
        {
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 117))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    foreach (var item in ArrayL)
                    {
                        if (item.fldDesc == null)
                            item.fldDesc = "";
                        Car.sp_ComplicationsRateInsert_Update(item.fldTypeCar, item.fldCodeCar, item.fldTypeCountryDivisions, item.fldCodeCountryDivisions
                            , item.fldYear, item.fldPrice,
                            Convert.ToInt32(Session["UserId"]), item.fldDesc, Session["UserPass"].ToString());
                    }
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
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

        [HttpPost]
        public ActionResult Save(Models.Rate rate)
        {
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 118))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (rate.fldDesc == null)
                        rate.fldDesc = "";
                    if (rate.fldId == 0)
                    {
                        Car.sp_ComplicationsRateInsert(rate.fldTypeCar, rate.fldCodeCar, rate.fldTypeCountryDivisions,
                            rate.fldCodeCountryDivisions, rate.fldYear, rate.fldFromCylinder, rate.fldToCylinder,
                            rate.fldFromWheel, rate.fldToWheel, rate.fldFromModel, rate.fldToModel, rate.fldFromContentMotor,
                            rate.fldToContentMotor, rate.fldPrice, Convert.ToInt32(Session["UserId"]), rate.fldDesc, Session["UserPass"].ToString());
                        return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        Car.sp_ComplicationsRateUpdate(rate.fldId, rate.fldTypeCar, rate.fldCodeCar, rate.fldTypeCountryDivisions,
                            rate.fldCodeCountryDivisions, rate.fldYear, rate.fldFromCylinder, rate.fldToCylinder,
                            rate.fldFromWheel, rate.fldToWheel, rate.fldFromModel, rate.fldToModel, rate.fldFromContentMotor,
                            rate.fldToContentMotor, rate.fldPrice, Convert.ToInt32(Session["UserId"]), rate.fldDesc, Session["UserPass"].ToString());
                        return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
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

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 244))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        //var q = Car.sp_CarExperienceSelect("fldId", id.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        //if (q.fldUserID == 1 && Convert.ToInt32(Session["UserId"]) != 1)
                        //{
                        //    return Json(new { data = "شما مجوز ویرایش را ندارید.", state = 1 });
                        //}
                        Car.sp_ComplicationsRateDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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
                var q = Car.sp_ComplicationsRateSelect("fldId", id.ToString(),1, 1,"").FirstOrDefault();
                var p = Car.sp_CarSeriesSelect("fldId", q.fldCarSeriesID.ToString(), 1, 1, "").FirstOrDefault();
                var c = Car.sp_CountryDivisionsSelect("fldId", q.fldCarSeriesID.ToString(), 1, 1, "").FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldID,
                    fldCabinTypeID=q.fldCabinTypeID,
                    fldCarAccountTypeID=q.fldCarAccountTypeID,
                    fldCarAccountTypeName=q.fldCarAccountTypeName,
                    fldCarCabinTypeName=q.fldCarCabinTypeName,
                    fldCarClassID=q.fldCarClassID,
                    fldCarClassName=q.fldCarClassName,
                    fldCarMakeID=q.fldCarMakeID,
                    fldCarMakeName=q.fldCarMakeName,
                    fldCarModelID=q.fldCarModelID,
                    fldCarModelName=q.fldCarModelName,
                    fldCarSeriesID=q.fldCarSeriesID,
                    fldCarSeriesName=q.fldCarSeriesName,
                    fldCarSystemID=q.fldCarSystemID,
                    fldCarSystemName=q.fldCarSystemName,
                    fldCountryDivisions=q.fldCountryDivisions,
                    fldCountryDivisionsName=q.fldCountryDivisionsName,
                    fldFromContentMotor=q.fldFromContentMotor,
                    fldFromCylinder=q.fldFromCylinder,
                    fldFromModel=q.fldFromModel,
                    fldFromWheel=q.fldFromWheel,
                    fldPrice=q.fldPrice,
                    fldToContentMotor=q.fldToContentMotor,
                    fldToCylinder=q.fldToCylinder,
                    fldToModel=q.fldToModel,
                    fldToWheel=q.fldToWheel,
                    fldUserName=q.fldUserName,
                    fldYear = q.fldYear
                    //,
                    //CarId = q.fldCarId,
                    //fldType = q.fldType,
                    //fldMsg = q.fldMsg
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
