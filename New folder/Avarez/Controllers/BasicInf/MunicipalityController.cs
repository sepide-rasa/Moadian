using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Avarez.Controllers.Users;
namespace Avarez.Controllers.BasicInf
{
    [Authorize]
    public class MunicipalityController : Controller
    {
        //
        // GET: /Municipality/

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 32))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->شهرداری");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
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
            var q = m.sp_MunicipalitySelect("", "", 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);
        }
        public JsonResult GetCascadeState()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StateSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeCounty(int cboState)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CountySelect("fldStateID", cboState.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetCascadeZone(int cboCounty)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Zone = car.sp_ZoneSelect("fldCountyID", cboCounty.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Zone.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeCity(int cboZone)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var City = car.sp_CitySelect("fldZoneID", cboZone.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(City.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldName", "fldCityName", "fldCityID" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_MunicipalitySelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_PictureSelect("fldMunicipalityPic", id.ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldPic != null)
                {
                    return File((byte[])pic.fldPic, "jpg");
                }
            }
            return null;

        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 34))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_MunicipalityDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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

        public ActionResult Save(Models.sp_MunicipalitySelect Municipality, string fldImage)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter _MunId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                if (Municipality.fldDesc == null)
                    Municipality.fldDesc = "";
                if (Municipality.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 33))
                    {
                        byte[] image = null;
                        if (fldImage != null)
                            image = Avarez.Helper.ClsCommon.Base64ToImage(fldImage);
                        Car.sp_MunicipalityInsert(_MunId, Municipality.fldName, Municipality.fldCityID,
                            Municipality.fldInformaticesCode, Municipality.fldServiceCode, Municipality.fldRWUserName, CodeDecode.stringcode(Municipality.fldRWPass),
                            Convert.ToInt32(Session["UserId"]), Municipality.fldDesc, image, Session["UserPass"].ToString(),Municipality.fldSamieUser,Municipality.fldSamiePass,Municipality.fldSamieGUID);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 35))
                    {
                        Car.sp_MunicipalityUpdate(Municipality.fldID, Municipality.fldName,
                            Municipality.fldCityID, Municipality.fldInformaticesCode, Municipality.fldServiceCode,
                            Municipality.fldRWUserName, CodeDecode.stringcode(Municipality.fldRWPass), Convert.ToInt32(Session["UserId"]),
                            Municipality.fldDesc, Avarez.Helper.ClsCommon.Base64ToImage(fldImage)
                            , Session["UserPass"].ToString(),Municipality.fldSamieUser,Municipality.fldSamiePass,Municipality.fldSamieGUID);
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

        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_MunicipalitySelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q1 = Car.sp_CitySelect("fldId", q.fldCityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q2 = Car.sp_ZoneSelect("fldId", q1.fldZoneID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q3 = Car.sp_CountySelect("fldId", q2.fldCountyID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                var StateId = 0; var CountyId = 0; var ZoneId = 0;
                StateId = q3.fldStateID;
                CountyId = q2.fldCountyID;
                ZoneId = q1.fldZoneID;

                var County = Car.sp_CountySelect("fldStateID", StateId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var Zone = Car.sp_ZoneSelect("fldCountyID", CountyId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var City = Car.sp_CitySelect("fldZoneID", ZoneId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });

                return Json(new
                {
                    fldId = q.fldID,
                    fldName = q.fldName,
                    fldCityID = q.fldCityID.ToString(),
                    fldInformaticesCode = q.fldInformaticesCode,
                    fldServiceCode = q.fldServiceCode,
                    fldRWUserName = q.fldRWUserName,
                    fldRWPass = CodeDecode.stringDecode(q.fldRWPass),
                    fldDesc = q.fldDesc,
                    fldStateID = q3.fldStateID.ToString(),
                    fldCountyID = q2.fldCountyID.ToString(),
                    fldZoneID = q1.fldZoneID.ToString(),
                    County = County,
                    Zone = Zone,
                    City = City
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

        public ActionResult loadFromWebService(string StateName, string CountyName, string ZoneName, string CityName, string MunName)
        {

            try
            {
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
                    return Json(new { data = "شما مجاز به استفاده از خدمات پشتیبانی نمی باشید، لطفا با واحد پشتیبانی تماس بگیرید.", state = 1 });
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
                                    p.sp_MunicipalityInsert(_MunId, k[0], cityId, GetMunInf.fldInformaticesCode,
                                        GetMunInf.fldServiceCode, mun.fldRWUserName, mun.fldRWPass, Convert.ToInt32(Session["UserId"]),
                                        GetMunInf.fldDesc, image, Session["UserPass"].ToString(),mun.fldSamieUser,mun.fldSamiePass ,mun.fldSamieGUID);
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
                                    p.sp_OfficesInsert(k[0], GetOfficeInf.fldAddress, GetOfficeInf.fldOfficesTypeID, MunId, localId, areaId,
                                        Convert.ToInt32(Session["UserId"]), GetOfficeInf.fldDesc, GetOfficeInf.fldTel, Session["UserPass"].ToString(),"");
                                }
                            }
                        }
                        stateId = 0; countyId = 0; zoneId = 0; cityId = 0; MunId = 0;
                        localId = null; areaId = null;
                    }
                    return Json(new { data = "بارگذاری با موفقیت انجام شد.", state = 0 });
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

    }
}
