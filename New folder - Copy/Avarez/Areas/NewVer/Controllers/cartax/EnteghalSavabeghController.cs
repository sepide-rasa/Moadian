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
using System.Configuration;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class EnteghalSavabeghController : Controller
    {
        //
        // GET: /NewVer/EnteghalSavabegh/
        string ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
        public ActionResult Index(string containerId, string CarId, string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 242))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->انتقال سوابق");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };
                result.ViewBag.CarId = CarId;
                result.ViewBag.CarFileId = CarFileId;
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return result;
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

        public ActionResult HelpEnteghalSavabegh()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }
        }
        public ActionResult WindowSavabegh(string CarId, string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 242))
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.CarId = CarId;
                PartialView.ViewBag.CarFileId = CarFileId;
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
        public ActionResult GetSal(string CarId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarSelect("fldid", CarId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var Tasal = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)) - 1;
            if (ImageSetting == "3")
                Tasal = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4));
            List<SelectListItem> sal = new List<SelectListItem>();
            if (car.fldModel < 1900)
            {
                for (int i = car.fldModel; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4))-1; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(car.fldModel.ToString() + "/05/21")).Substring(0, 4))-1; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSal2(int StartYear)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Models.Sal> sh = new List<Models.Sal>();
            var q = m.sp_GetDate().FirstOrDefault();
            int fldSal = Convert.ToInt32(q.DateShamsi.Substring(0, 4));
            var Tasal = fldSal - 1;
            if (ImageSetting == "3")
                Tasal = fldSal;
            for (int i = StartYear; i <= Tasal; i++)
            {
                Models.Sal CboSal = new Models.Sal();

                CboSal.fldSal = i;
                sh.Add(CboSal);

            }
            var q2 = sh.Where(l => l.fldSal >= StartYear);

            return Json(q2, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Upload()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    System.IO.File.Delete(physicalPath);
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg")
                {
                    if (Request.Files[0].ContentLength <= 307200)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                        file.SaveAs(savePath);
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
                            Message = "حجم فایل انتخابی می بایست کمتر از 300کیلوبایت باشد."
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
        public JsonResult GetCascadeState()
        {
            if (Session["UserId"] == null)
                return null;
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StateSelect("", "", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeCounty(int cboState)
        {
            if (Session["UserId"] == null)
                return null;
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(1, cboState, 5).ToList().OrderBy(h => h.NodeName);
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }), JsonRequestBehavior.AllowGet);
        }
        public string GetFromDate(int? FromYear)
        {
            if (Session["UserId"] == null)
                return null;
            string date = FromYear + "/01/01";
            return date;
        }
        public string GetToDate(int ToYear)
        {
            if (Session["UserId"] == null)
                return null;
            string date = ToYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(ToYear))
                date = ToYear + "/12/30";
            return date;
        }
        public ActionResult Save(Models.CarExp CarExp)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (CarExp.fldDesc == null)
                    CarExp.fldDesc = "";

                CarExp.fldStartDate = GetFromDate(CarExp.fldFromYear);
                CarExp.fldEndDate = GetToDate(CarExp.fldToYear);
                var subSett = Car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), Car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                bool? ForceScan = true;
                if (subSett != null)
                {
                    ForceScan = subSett.fldExpScan;
                }
                if (CarExp.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 243))
                    {
                        if (ForceScan == true && Session["savePath"] == null)
                        {
                            return Json(new { Msg = "لطفا فایل مدرک را آپلود کنید.", MsgTitle = "خطا", Err = 1 });
                        }
                        else
                        {
                            System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            int? fileid = null;
                            if (Session["savePath"] != null)
                            {
                                string savePath = Session["savePath"].ToString();

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

                                Session.Remove("savePath");
                                
                                if (a.Value != null)
                                    fileid = Convert.ToInt32(a.Value);
                            }
                            Car.sp_CarExperienceInsert(CarExp.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldStartDate),
                                MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldEndDate),
                                CarExp.fldMunicipalityID, CarExp.fldLetterNumber, Convert.ToInt32(Session["UserId"]),
                                CarExp.fldDesc, Session["UserPass"].ToString(), fileid, false, null, null);
                            Car.SaveChanges();
                            return Json(new { Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = "ذخیره موفق", Err = 0 });
                        }
                        //else
                        //    return Json(new { Msg = "لطفا فایل مدرک را آپلود کنید.", MsgTitle = "خطا", Err = 1 });
                    }
                    else
                    {
                        //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        //return RedirectToAction("error", "Metro");
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "شما مجاز به دسترسی نمی باشید."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 245))
                    {
                        var q = Car.sp_CarExperienceSelect("fldId", CarExp.fldID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (q.fldUserID == 1 && Convert.ToInt32(Session["UserId"]) != 1)
                        {
                            return Json(new { Msg = "شما مجوز ویرایش را ندارید.", MsgTitle = "خطا", Err = 1 });
                        }
                        int? fileid = null;
                        if (ForceScan == true && Session["savePath"] != null)
                        {
                            System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            string savePath = Session["savePath"].ToString();

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

                            if (a.Value != null)
                                fileid = Convert.ToInt32(a.Value);
                        }
                        else if (CarExp.fldFileId != null)
                            fileid = CarExp.fldFileId;
                        else if (ForceScan == true && CarExp.fldFileId == null)
                            return Json(new { Msg = "لطفا فایل مدرک را آپلود کنید.", MsgTitle = "خطا", Err = 1 });
                        Car.sp_CarExperienceUpdate(CarExp.fldID, CarExp.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldStartDate)
                            , MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldEndDate),
                            CarExp.fldMunicipalityID, CarExp.fldLetterNumber, Convert.ToInt32(Session["UserId"]),
                            CarExp.fldDesc, Session["UserPass"].ToString(), fileid);
                        if (CarExp.fldFileId != null && Session["savePath"] != null)
                            Car.Sp_FilesDelete(CarExp.fldFileId);
                        Session.Remove("savePath");
                        return Json(new { Msg = "ویرایش با موفقیت انجام شد.", MsgTitle = "ویرایش موفق", Err = 0 });
                    }
                    else
                    {
                        //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        //return RedirectToAction("error", "Metro");
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "شما مجاز به دسترسی نمی باشید."
                        });
                        DirectResult result = new DirectResult();
                        return result;
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Err = 1 });
            }
        }
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 244))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();

                    //var q = Car.sp_CarExperienceSelect("fldId", id.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //if (q.fldUserID == 1 && Convert.ToInt32(Session["UserId"]) != 1)
                    //{
                    //    return Json(new { data = "شما مجوز ویرایش را ندارید.", state = 1 });
                    //}
                    Car.sp_CarExperienceDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                    return Json(new { Msg = "حذف با موفقیت انجام شد.", MsgTitle = "حذف موفق", Err = 0 });

                }
                else
                {
                    //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                   // return RedirectToAction("error", "Metro");
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "شما مجاز به دسترسی نمی باشید."
                    });
                    DirectResult result = new DirectResult();
                    return result;
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Err = 1 });
            }
        }
        public ActionResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_CarExperienceSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q1 = Car.sp_MunicipalitySelect("fldId", q.fldMunicipalityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q2 = Car.sp_CitySelect("fldId", q1.fldCityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q3 = Car.sp_ZoneSelect("fldId", q2.fldZoneID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q4 = Car.sp_CountySelect("fldId", q3.fldCountyID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldID,
                    fldDesc = q.fldDesc,
                    fldFromYear = q.fldStartDate.Substring(0, 4),
                    fldToYear = q.fldEndDate.Substring(0, 4),
                    fldLetterNumber = q.fldLetterNumber,
                    fldMunID = q.fldMunicipalityID.ToString(),
                    fldStateId = q4.fldStateID.ToString(),
                    fldFileId = q.fldFileId,
                    Er = 0
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }
        public ActionResult DateFill(int carID)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_CarSelect("fldID", carID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                string FromDate = "";
                string EndDate = "";
                string modelDate = "";
                var timenow = MyLib.Shamsi.Miladi2ShamsiString(Car.sp_GetDate().FirstOrDefault().CurrentDateTime);

                if (q.fldModel >= 1900)
                {
                    modelDate = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(q.fldModel + "-01-01"));
                    FromDate = Convert.ToInt32(modelDate.Substring(0, 4)) + 1 + "/01/01";
                }
                else
                {

                    FromDate = q.fldModel + "/01/01";
                }
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(timenow.Substring(0, 4))) == true)
                    EndDate = Convert.ToInt32(timenow.Substring(0, 4)) - 1 + "/12/30";
                else
                    EndDate = Convert.ToInt32(timenow.Substring(0, 4)) - 1 + "/12/29";
                return Json(new
                {

                    fldStartDate = FromDate,
                    fldEndDate = EndDate

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
        public ActionResult Read(StoreRequestParameters parameters, string CarId)
        {
            //if (Session["UserId"] == null)
            //    return RedirectToAction("LogOn", "Account");
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities p = new Models.cartaxEntities();
            List<Models.sp_CarExperienceSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.sp_CarExperienceSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldStartDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStartDate_S";
                            break;
                        case "fldEndDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEndDate_S";
                            break;
                        case "fldLetterNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldLetterNumber";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldUserName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldUserName";
                            break;
                    }
                    if (data != null)
                        data1 = p.sp_CarExperienceSelect(field, searchtext, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(l => l.fldCarID == Convert.ToInt32(CarId)).ToList();
                    else
                        data = p.sp_CarExperienceSelect(field, searchtext, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(l => l.fldCarID == Convert.ToInt32(CarId)).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.sp_CarExperienceSelect("fldCarID", CarId, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Models.sp_CarExperienceSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult CheckTaiidSavabegh(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carEx = p.sp_CarExperienceSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
            //var f = p.Sp_FilesSelect(carEx.fldFileId).FirstOrDefault();
            return Json(new
            {
                HaveTaiid = carEx.fldAccept,
                userId = Session["UserId"].ToString()
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
