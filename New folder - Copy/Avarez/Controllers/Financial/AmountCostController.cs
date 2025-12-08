using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Collections;
using Kendo.Mvc.UI;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.Financial
{
    [Authorize]
    public class AmountCostController : Controller
    {

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 126))
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
            var q = m.sp_AmountCostSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);

        }
        public JsonResult GetCascadeRound()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CostSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(Models.AmountCost AmountCost)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (AmountCost.fldDesc == null)
                    AmountCost.fldDesc = "";
                if (AmountCost.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 127))
                    {
                        Car.sp_AmountCostInsert(AmountCost.fldAmount, MyLib.Shamsi.Shamsi2miladiDateTime(AmountCost.fldDateAmount), AmountCost.fldCostID,
                            AmountCost.fldTypeCountryDivisions, AmountCost.fldCodeCountryDivisions,
                            Convert.ToInt32(Session["UserId"]), AmountCost.fldDesc, AmountCost.fldCountryDivisionsTreeApply,
                            AmountCost.fldTypeCar, AmountCost.fldCodeCar,
                            AmountCost.fldCarSeriesTreeApply, Session["UserPass"].ToString(), AmountCost.fldEffectiveUser,
                            AmountCost.fldEffectiveOffice, AmountCost.fldEffectiveMunicipality);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 129))
                    {
                        Car.sp_AmountCostUpdate(AmountCost.fldID, AmountCost.fldAmount, MyLib.Shamsi.Shamsi2miladiDateTime(AmountCost.fldDateAmount), AmountCost.fldCostID,
                            AmountCost.fldTypeCountryDivisions, AmountCost.fldCodeCountryDivisions,
                            Convert.ToInt32(Session["UserId"]), AmountCost.fldDesc, AmountCost.fldCountryDivisionsTreeApply,
                            AmountCost.fldTypeCar, AmountCost.fldCodeCar,
                            AmountCost.fldCarSeriesTreeApply, Session["UserPass"].ToString(), AmountCost.fldEffectiveUser,
                            AmountCost.fldEffectiveOffice, AmountCost.fldEffectiveMunicipality);
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
            string[] _fiald = new string[] { "fldCountryDivisionsName", "fldCarSeriesName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_AmountCostSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 128))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_AmountCostDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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
              
                var AmountCost = Car.sp_AmountCostSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var countryId = Car.sp_TableTreeSelect("fldSourceID", AmountCost.CountryCode.ToString(), 0, 0, 0).Where(h => h.fldNodeType == AmountCost.CountryType).FirstOrDefault();
                var CarId = Car.sp_TableTreeCarSelect("fldSourceID", AmountCost.CarCode.ToString(), 0, 0, 0).Where(h => h.fldNodeType == AmountCost.CarType).FirstOrDefault();
                return Json(new
                {
                    fldId = AmountCost.fldID,
                    fldDateAmount = AmountCost.fldDateAmount,
                    fldAmount = AmountCost.fldAmount,
                    fldCostID = AmountCost.fldCostID,
                    fldCountryDivisionsTreeApply = AmountCost.fldCountryDivisionsTreeApply,
                    fldCarSeriesTreeApply = AmountCost.fldCarSeriesTreeApply,
                    fldDesc = AmountCost.fldDesc,
                    CountryType = AmountCost.CountryType,
                    countryCode = AmountCost.CountryCode,
                    carType = AmountCost.CarType,
                    carCode = AmountCost.CarCode,
                    carid = CarId.fldID,
                    countryId = countryId.fldID,
                    fldEffectiveMunicipality = AmountCost.fldEffectiveMunicipality,
                    fldEffectiveOffice = AmountCost.fldEffectiveOffice,
                    fldEffectiveUser = AmountCost.fldEffectiveUser
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
