using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
using Ext.Net;
using Ext.Net.MVC;


namespace Avarez.Areas.NewVer.Controllers.Financial
{
    public class Signer_NewController : Controller
    {
        //
        // GET: /NewVer/Signer_New/

        public ActionResult Index(string containerId)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات مالی->تعریف اشخاص صاحب امضاء");
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
        public ActionResult New(int Id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }

        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetCascadePost()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PostSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeState()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StateSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeMunicipality(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(1, ID, 5).ToList();
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeLocal(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Local = car.sp_LocalSelect("fldMunicipalityID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Local.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeArea(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Local = car.sp_AreaSelect("fldMunicipalityID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Local.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCascadeOffice(int ID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Office = car.sp_OfficesSelect("fldMunicipalityID", ID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Office.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 163))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();

                    Car.sp_SignerEmployeeDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                    return Json(new
                    {
                        MsgTitle = "حذف موفق",
                        Msg = "حذف با موفقیت انجام شد.",
                        Er = 0
                    });
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    });
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                m.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }

        public ActionResult Save(Models.sp_SignerEmployeeSelect Signer)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Signer.fldDesc == null)
                    Signer.fldDesc = "";
                if (checkCodeMeli(Signer.fldMelliCode) == 0)
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "کدملی وارد شده معتبر نمی باشد.",
                        Er = 1
                    });
                }
                if (Signer.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 162))
                    {
                        Car.sp_SignerEmployeeInsert(Signer.fldName, Signer.fldFamily, Signer.fldMelliCode, Signer.fldPostID, Signer.fldMunicipalityID,
                           Signer.fldLocalID, Signer.fldAreaID, Signer.fldOfficesID, Convert.ToInt32(Session["UserId"]), Signer.fldDesc, Session["UserPass"].ToString());
                        Car.SaveChanges();
                        return Json(new
                        {
                            MsgTitle = "ذخیره موفق",
                            Msg = "ذخیره با موفقیت انجام شد.",
                            Er = 0
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        });
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 164))
                    {
                        Car.sp_SignerEmployeeUpdate(Signer.fldID, Signer.fldName, Signer.fldFamily, Signer.fldMelliCode, Signer.fldPostID,
                             Signer.fldMunicipalityID, Signer.fldLocalID, Signer.fldAreaID, Signer.fldOfficesID, Convert.ToInt32(Session["UserId"]), Signer.fldDesc, Session["UserPass"].ToString());
                        return Json(new
                        {
                            MsgTitle = "ویرایش موفق",
                            Msg = "ویرایش با موفقیت انجام شد.",
                            Er = 0
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        });
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
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }

        public ActionResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New");
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_SignerEmployeeSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q1 = Car.sp_MunicipalitySelect("fldId", q.fldMunicipalityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q2 = Car.sp_CitySelect("fldId", q1.fldCityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q3 = Car.sp_ZoneSelect("fldId", q2.fldZoneID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q4 = Car.sp_CountySelect("fldId", q3.fldCountyID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                return Json(new
                {
                    Er = 0,
                    fldId = q.fldID,
                    fldName = q.fldName,
                    fldFamily = q.fldFamily,
                    fldMelliCode = q.fldMelliCode,
                    fldPostID = q.fldPostID.ToString(),
                    fldMunicipalityID = q.fldMunicipalityID.ToString(),
                    fldLocalID = q.fldLocalID.ToString(),
                    fldAreaID = q.fldAreaID.ToString(),
                    fldOfficesID = q.fldOfficesID.ToString(),
                    fldDesc = q.fldDesc,
                    fldState = q4.fldStateID.ToString()
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
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_SignerEmployeeSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_SignerEmployeeSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamily";
                            break;
                        case "fldMelliCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMelliCode";
                            break;
                        case "fldMunicipalityName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMunicipalityName";
                            break;
                        case "fldLocalName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldLocalName";
                            break;
                        case "fldAreaName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAreaName";
                            break;
                        case "fldOfficeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOfficeName";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_SignerEmployeeSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_SignerEmployeeSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_SignerEmployeeSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_SignerEmployeeSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public int checkCodeMeli(string codec)
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
    }
}
