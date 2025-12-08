using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;
using System.IO;

namespace Avarez.Areas.NewVer.Controllers.BasicInf
{
    public class Municipality_NewController : Controller
    {
        //
        // GET: /NewVer/Municipality_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->تعریف شهرداری");
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
        public ActionResult loadFromWebServiceWin()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Help()
        {
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
        public ActionResult Upload()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            string Msg = "";
            try
            {
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    Session.Remove("FileName");
                    System.IO.File.Delete(physicalPath);
                }

                if (Request.Files[0].ContentType == "image/jpeg" || Request.Files[0].ContentType == "image/png")
                {
                    if (Request.Files[0].ContentLength <= 25600)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                        file.SaveAs(savePath);
                        Session["FileName"] = file.FileName;
                        Session["savePath"] = savePath;
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
                            Message = "حجم فایل انتخابی می بایست کمتر از 25 کیلوبایت باشد."
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

        public ActionResult ShowPic(string dc)
        {//برگرداندن عکس 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            byte[] file = null;
            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
            file = stream.ToArray();
            var image = Convert.ToBase64String(file);
            return Json(new { image = image });
        }
        public ActionResult Save(Models.sp_MunicipalitySelect Municipality)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            byte[] file = null; string FileName = "";
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter _MunId = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                if (Municipality.fldDesc == null)
                    Municipality.fldDesc = "";
                if (Municipality.fldID == 0)
                {//ثبت رکورد جدید
                    if (Session["savePath"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                        file = stream.ToArray();
                        FileName = Session["FileName"].ToString();
                    }
                    else
                    {
                        var Image = Server.MapPath("~/Content/Blank.jpg");
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
                        file = stream.ToArray();
                    }
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 33))
                    {
                        MsgTitle = "ذخیره موفق";
                        Msg = "ذخیره با موفقیت انجام شد.";
                        Car.sp_MunicipalityInsert(_MunId, Municipality.fldName, Municipality.fldCityID,
                            Municipality.fldInformaticesCode, Municipality.fldServiceCode, Municipality.fldRWUserName, CodeDecode.stringcode(Municipality.fldRWPass),
                            Convert.ToInt32(Session["UserId"]), Municipality.fldDesc, file, Session["UserPass"].ToString(), Municipality.fldSamieUser, Municipality.fldSamiePass, Municipality.fldSamieGUID);
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
                    if (Session["savePath"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                        file = stream.ToArray();
                        FileName = Session["FileName"].ToString();
                    }
                    else
                    {
                        var pic = Car.sp_PictureSelect("fldMunicipalityPic", Municipality.fldID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (pic != null && pic.fldPic != null)
                        {
                            file = pic.fldPic;
                        }
                        else
                        {
                            var Image = Server.MapPath("~/Content/Blank.jpg");
                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
                            file = stream.ToArray();
                        }
                    }
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 35))
                    {
                        MsgTitle = "ویرایش موفق";
                        Msg = "ویرایش با موفقیت انجام شد.";
                        Car.sp_MunicipalityUpdate(Municipality.fldID, Municipality.fldName,
                            Municipality.fldCityID, Municipality.fldInformaticesCode, Municipality.fldServiceCode,
                            Municipality.fldRWUserName, CodeDecode.stringcode(Municipality.fldRWPass),
                            Convert.ToInt32(Session["UserId"]), Municipality.fldDesc, file , Session["UserPass"].ToString(),
                            Municipality.fldSamieUser,Municipality.fldSamiePass,Municipality.fldSamieGUID);
                    }
                    else
                    {
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به ویرایش اطلاعات نمی باشید.";
                        Er = 1;
                    }
                }
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    Session.Remove("FileName");
                    System.IO.File.Delete(physicalPath);
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
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 34))
                {

                    MsgTitle = "حذف موفق";
                    Msg = "حذف با موفقیت انجام شد.";
                    Car.sp_MunicipalityDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());

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
                if (x.InnerException.Message != null)
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
            var Img = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New");
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_MunicipalitySelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q1 = Car.sp_CitySelect("fldId", q.fldCityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q2 = Car.sp_ZoneSelect("fldId", q1.fldZoneID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q3 = Car.sp_CountySelect("fldId", q2.fldCountyID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var pic = Car.sp_PictureSelect("fldMunicipalityPic", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                var StateId = 0; var CountyId = 0; var ZoneId = 0;
                StateId = q3.fldStateID;
                CountyId = q2.fldCountyID;
                ZoneId = q1.fldZoneID;

                var County = Car.sp_CountySelect("fldStateID", StateId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var Zone = Car.sp_ZoneSelect("fldCountyID", CountyId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var City = Car.sp_CitySelect("fldZoneID", ZoneId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                if (pic != null)
                {
                    Img = Convert.ToBase64String(pic.fldPic);
                }
                return Json(new
                {
                    fldID = q.fldID,
                    fldName = q.fldName,
                    fldCityID = q.fldCityID.ToString(),
                    fldInformaticesCode = q.fldInformaticesCode,
                    fldServiceCode = q.fldServiceCode.ToString(),
                    fldRWUserName = q.fldRWUserName,
                    fldRWPass = CodeDecode.stringDecode(q.fldRWPass),
                    fldDesc = q.fldDesc,
                    fldStateID = q3.fldStateID.ToString(),
                    fldCountyID = q2.fldCountyID.ToString(),
                    fldZoneID = q1.fldZoneID.ToString(),
                    fldSamieUser = q.fldSamieUser,
                    fldSamiePass = q.fldSamiePass,
                    fldSamieGUID = q.fldSamieGUID,
                    County = County,
                    Zone = Zone,
                    City = City,
                    fldImage = Img,
                    Er = 0
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
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New");
            Models.cartaxEntities m = new Models.cartaxEntities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Avarez.Models.sp_MunicipalitySelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_MunicipalitySelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldCityName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCityName";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldStateName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStateName";
                            break;
                        case "fldCountyName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCountyName";
                            break;
                        case "fldZoneName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldZoneName";
                            break;
                        case "fldInformaticesCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldInformaticesCode";
                            break;
                        case "fldServiceCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldServiceCode";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = m.sp_MunicipalitySelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_MunicipalitySelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_MunicipalitySelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_MunicipalitySelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult loadFromWebService(string StateName, string CountyName, string ZoneName, string CityName, string MunName)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
                RateWebService.Rate a = new RateWebService.Rate();

                System.Data.Entity.Core.Objects.ObjectParameter _StateId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter _CountyId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter _ZoneId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter _CityId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter _MunId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter _LocalId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter _AreaId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));

                Models.cartaxEntities p = new Models.cartaxEntities();

                var mun = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                var CheckAccountCharge = a.CheckAccountCharge(mun.fldRWUserName, mun.fldRWPass, mun.fldName);
                int stateId = 0, countyId = 0, zoneId = 0, cityId = 0, MunId = 0;
                long? localId = null, areaId = null;
                byte[] image = null;
                if (CheckAccountCharge == false)
                {
                    return Json(new { MsgTitle = "خطا", Msg = "شما مجاز به استفاده از خدمات پشتیبانی نمی باشید، لطفا با واحد پشتیبانی تماس بگیرید.", Er = 1 });
                }
                else
                {
                    var GetCountry = a.GetCountry(mun.fldRWUserName, mun.fldRWPass, mun.fldName, StateName, CountyName, ZoneName, CityName, MunName).ToList();
                    foreach (var item in GetCountry)
                    {
                        var m = item.fldPath.Split(';');
                        foreach (var _item in m)
                        {
                            var k = _item.Split('|');
                            if (k[0] == "")
                                break;
                            else if (k[1] == "1")
                            {
                                var state = p.sp_StateSelect("fldName", k[0], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (state == null)
                                {
                                    p.sp_StateInsert(_StateId, k[0], Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                                    stateId = Convert.ToInt32(_StateId.Value);
                                }
                                else
                                {
                                    stateId = state.fldID;
                                }
                            }

                            else if (k[1] == "2")
                            {
                                var county = p.sp_CountySelect("fldName", k[0], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(w => w.fldStateID == stateId).FirstOrDefault();
                                if (county == null)
                                {
                                    p.sp_CountyInsert(_CountyId, k[0], stateId, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                                    stateId = Convert.ToInt32(_CountyId.Value);
                                }
                                else
                                {
                                    countyId = county.fldID;
                                }
                            }
                            else if (k[1] == "3")
                            {
                                var zone = p.sp_ZoneSelect("fldName", k[0], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(w => w.fldCountyID == countyId).FirstOrDefault();
                                if (zone == null)
                                {
                                    p.sp_ZoneInsert(_ZoneId, k[0], countyId, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                                    zoneId = Convert.ToInt32(_ZoneId.Value);
                                }
                                else
                                {
                                    zoneId = zone.fldID;
                                }
                            }
                            else if (k[1] == "4")
                            {
                                var city = p.sp_CitySelect("fldName", k[0], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(w => w.fldZoneID == zoneId).FirstOrDefault();
                                if (city == null)
                                {
                                    p.sp_CityInsert(_CityId, k[0], zoneId, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                                    cityId = Convert.ToInt32(_CityId.Value);
                                }
                                else
                                {
                                    cityId = city.fldID;
                                }
                            }
                            else if (k[1] == "5")
                            {
                                var municipality = p.sp_MunicipalitySelect("fldName", k[0], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(w => w.fldCityID == cityId).FirstOrDefault();
                                MunName = k[0];
                                if (municipality == null)
                                {
                                    var GetMunInf = a.GetMunInf(mun.fldRWUserName, mun.fldRWPass, mun.fldName, k[0]);
                                    p.sp_MunicipalityInsert(_MunId, k[0], cityId, GetMunInf.fldInformaticesCode, GetMunInf.fldServiceCode,
                                        mun.fldRWUserName, mun.fldRWPass, Convert.ToInt32(Session["UserId"]), GetMunInf.fldDesc, image, Session["UserPass"].ToString()
                                        ,mun.fldSamieUser,mun.fldSamiePass,mun.fldSamieGUID);
                                    MunId = Convert.ToInt32(_MunId.Value);
                                }
                                else
                                {
                                    MunId = municipality.fldID;
                                }
                            }
                            else if (k[1] == "6")
                            {
                                var local = p.sp_LocalSelect("fldName", k[0], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(w => w.fldMunicipalityID == MunId).FirstOrDefault();
                                if (local == null)
                                {
                                    var GetLocalInf = a.GetLocalInf(mun.fldRWUserName, mun.fldRWPass, mun.fldName, MunName, k[0]);
                                    p.sp_LocalInsert(_LocalId, k[0], MunId, Convert.ToInt32(Session["UserId"]), GetLocalInf.fldDesc, Session["UserPass"].ToString(), GetLocalInf.fldServiceCode, GetLocalInf.fldSourceInformatics);
                                    localId = Convert.ToInt32(_LocalId.Value);
                                }
                                else
                                {
                                    localId = local.fldID;
                                }
                            }
                            else if (k[1] == "7")
                            {
                                var area = p.sp_AreaSelect("fldName", k[0], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(w => w.fldMunicipalityID == MunId && w.fldLocalID == localId).FirstOrDefault();
                                if (area == null)
                                {
                                    p.sp_AreaInsert(_AreaId, k[0], localId, MunId, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                                    areaId = Convert.ToInt32(_AreaId.Value);
                                }
                                else
                                {
                                    areaId = area.fldID;
                                }
                            }
                            else if (k[1] == "8")
                            {
                                var office = p.sp_OfficesSelect("fldName", k[0], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(w => w.fldMunicipalityID == MunId && w.fldLocalID == localId && w.fldAreaID == areaId).FirstOrDefault();
                                if (office == null)
                                {
                                    var GetOfficeInf = a.GetOfficeInf(mun.fldRWUserName, mun.fldRWPass, mun.fldName, MunName, k[0]);
                                    p.sp_OfficesInsert(k[0], GetOfficeInf.fldAddress, GetOfficeInf.fldOfficesTypeID, MunId, localId, areaId, Convert.ToInt32(Session["UserId"]), GetOfficeInf.fldDesc, GetOfficeInf.fldTel, Session["UserPass"].ToString(), "");
                                }
                            }
                        }
                        stateId = 0; countyId = 0; zoneId = 0; cityId = 0; MunId = 0;
                        localId = null; areaId = null;
                    }
                    return Json(new { MsgTitle = "بارگذاری موفق", Msg = "بارگذاری با موفقیت انجام شد.", Er = 0 });
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
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }
    }
}
