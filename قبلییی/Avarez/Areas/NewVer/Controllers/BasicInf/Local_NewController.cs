using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;

namespace Avarez.Areas.NewVer.Controllers.BasicInf
{
    public class Local_NewController : Controller
    {
        //
        // GET: /NewVer/Local_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->تعریف منطقه");
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
        }
        public ActionResult New(int id)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }

        public ActionResult Help()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult loadFromWebServiceWin()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetState()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StateSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeCounty(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CountySelect("fldStateID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeZone(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Zone = car.sp_ZoneSelect("fldCountyID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Zone.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeCity(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var City = car.sp_CitySelect("fldZoneID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(City.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeMunicipality(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Municipality = car.sp_MunicipalitySelect("fldCityID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Municipality.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(Models.sp_LocalSelect local)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter _LocalId = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                if (local.fldDesc == null)
                    local.fldDesc = "";
                if (local.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 37))
                    {
                        MsgTitle = "ذخیره موفق";
                        Msg = "ذخیره با موفقیت انجام شد.";
                        Car.sp_LocalInsert(_LocalId, local.fldName, local.fldMunicipalityID,
                            Convert.ToInt32(Session["UserId"]), local.fldDesc, Session["UserPass"].ToString(),
                            local.fldServiceCode, local.fldSourceInformatics);
                        Car.SaveChanges();
                    }
                    else
                    {
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به ذخیره اطلاعات نمی باشید.";
                        Er = 1;
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 39))
                    {
                        MsgTitle = "ویرایش موفق";
                        Msg = "ویرایش با موفقیت انجام شد.";
                        Car.sp_LocalUpdate(local.fldID, local.fldName, local.fldMunicipalityID,
                            Convert.ToInt32(Session["UserId"]), local.fldDesc, Session["UserPass"].ToString(),
                            local.fldServiceCode, local.fldSourceInformatics);
                    }
                    else
                    {
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به ویرایش اطلاعات نمی باشید.";
                        Er = 1;
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
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            try
            {
                //حذف
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 38))
                {

                    MsgTitle = "حذف موفق";
                    Msg = "حذف با موفقیت انجام شد.";
                    Car.sp_LocalDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());

                }
                else
                {
                    MsgTitle = "خطا";
                    Msg = "شما مجاز به حذف اطلاعات نمی باشد.";
                    Er = 1;
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
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int id)
        {
            try
            {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var q = Car.sp_LocalSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var q1 = Car.sp_MunicipalitySelect("fldId", q.fldMunicipalityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var q2 = Car.sp_CitySelect("fldId", q1.fldCityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var q3 = Car.sp_ZoneSelect("fldId", q2.fldZoneID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var q4 = Car.sp_CountySelect("fldId", q3.fldCountyID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            var StateId = 0; var CountyId = 0; var ZoneId = 0; var CityId = 0;

            StateId = q4.fldStateID;
            CountyId = q3.fldCountyID;
            ZoneId = q2.fldZoneID;
            CityId = q1.fldCityID;

            var County = Car.sp_CountySelect("fldStateID", StateId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
            var Zone = Car.sp_ZoneSelect("fldCountyID", CountyId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
            var City = Car.sp_CitySelect("fldZoneID", ZoneId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
            var Municipality = Car.sp_MunicipalitySelect("fldCityID", CityId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });

            return Json(new
            {
                fldID = q.fldID,
                fldName = q.fldName,
                fldMunicipalityID = q.fldMunicipalityID.ToString(),
                fldDesc = q.fldDesc,
                fldStateID = q4.fldStateID.ToString(),
                fldCountyID = q3.fldCountyID.ToString(),
                fldZoneID = q2.fldZoneID.ToString(),
                fldCityID = q1.fldCityID.ToString(),
                fldSourceInformatics = q.fldSourceInformatics,
                fldServiceCode = q.fldServiceCode.ToString(),
                County = County,
                Zone = Zone,
                City = City,
                Municipality = Municipality,
                Er=0
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
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Models.cartaxEntities m = new Models.cartaxEntities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Avarez.Models.sp_LocalSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_LocalSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldMunicipalityName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMunicipalityName";
                            break;
                        case "fldStateName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStateName";
                            break;
                        case "fldCountyName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMunicipalityName";
                            break;
                        case "fldZoneName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldZoneName";
                            break;
                        case "fldCityName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCityName";
                            break;
                        case "fldSourceInformatics":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSourceInformatics";
                            break;
                        case "fldServiceCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldServiceCode";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = m.sp_LocalSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_LocalSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_LocalSelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_LocalSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
