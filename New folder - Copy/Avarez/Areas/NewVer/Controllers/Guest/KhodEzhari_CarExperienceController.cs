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
using System.Configuration;

namespace Avarez.Areas.NewVer.Controllers.Guest
{
    public class KhodEzhari_CarExperienceController : Controller
    {
        //
        // GET: /NewVer/KhodEzhari_CarExperience/
        string ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
        public ActionResult Index(long? CarId, long PelakId, long MalekId, string CarFileId)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Models.cartaxEntities car = new Models.cartaxEntities(); 
            PartialView.ViewBag.CarId = CarId;
            PartialView.ViewBag.PelakId = PelakId;
            PartialView.ViewBag.MalekId = MalekId;
            PartialView.ViewBag.CarFileId = CarFileId;
            PartialView.ViewBag.ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            return PartialView;
        }
        public ActionResult HelpSavabegh()
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public JsonResult GetCascadeState()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StateSelect("", "", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeCounty(int State)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(1, State, 5).ToList().OrderBy(h => h.NodeName);
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSal(string CarFileId)
        {
            if (Session["UserGeust"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var file = p.sp_CarFileSelect("fldID", CarFileId.ToString(), 1,1, "").FirstOrDefault();
            var car = p.sp_CarSelect("fldid", file.fldCarID.ToString(), 1, 1, "").FirstOrDefault();

            List<SelectListItem> sal = new List<SelectListItem>();
            if (car.fldModel < 1900)
            {
                for (int i = car.fldModel; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)) - 1; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(car.fldModel.ToString() + "/05/21")).Substring(0, 4)) - 1; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
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
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Models.Sal> sh = new List<Models.Sal>();
            var q = m.sp_GetDate().FirstOrDefault();
            int fldSal = Convert.ToInt32(q.DateShamsi.Substring(0, 4));

            for (int i = StartYear; i <= fldSal - 1; i++)
            {
                Models.Sal CboSal = new Models.Sal();

                CboSal.fldSal = i;
                sh.Add(CboSal);

            }
            var q2 = sh.Where(l => l.fldSal >= StartYear);

            return Json(q2, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadCarExperience(StoreRequestParameters parameters, string CarFileId)
        {

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_CarExperienceSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_CarExperienceSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_CarExperienceSelect(field, searchtext, 200, null, null).ToList();
                    else
                        data = m.sp_CarExperienceSelect(field, searchtext, 200, null, null).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_CarExperienceSelect("fldCarFileID", CarFileId, 200, null, null).ToList();
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

            List<Avarez.Models.sp_CarExperienceSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Upload()
        {
            string Msg = "";
            try
            {

                if (Session["savePath"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    System.IO.File.Delete(Session["savePath"].ToString());
                    Session.Remove("savePath");
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tif"
                || extension == ".tiff")
                {
                    if (ImageSetting == "4")
                    {
                        if (Request.Files[0].ContentLength > 716800)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else if (Request.Files[0].ContentLength < 51200)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست بیشتر از 50 کیلو بایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else
                        {
                            HttpPostedFileBase file = Request.Files[0];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[0].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                    }
                    else
                    {
                        if (Request.Files[0].ContentLength <= 716800)
                        {
                            HttpPostedFileBase file = Request.Files[0];
                            //var Name=Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
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
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
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
        public string GetFromDate(int? FromYear)
        {
            string date = FromYear + "/01/01";
            return date;
        }
        public string GetToDate(int ToYear)
        {
            string date = ToYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(ToYear))
                date = ToYear + "/12/30";
            return date;
        }
        public ActionResult Save(Models.CarExp CarExp)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (CarExp.fldDesc == null)
                    CarExp.fldDesc = "";

                CarExp.fldStartDate = GetFromDate(CarExp.fldFromYear);
                CarExp.fldEndDate = GetToDate(CarExp.fldToYear);
               
                if (CarExp.fldID == 0)
                {//ثبت رکورد جدید
                        if (Session["savePath"] == null)
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

                                Car.Sp_FilesInsert(a, _File, null, null, null, null);

                                System.IO.File.Delete(savePath);
                                Session.Remove("savePath");

                                if (a.Value != null)
                                    fileid = Convert.ToInt32(a.Value);
                            }
                            Car.sp_CarExperienceInsert(CarExp.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldStartDate),
                                MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldEndDate),
                                CarExp.fldMunicipalityID, CarExp.fldLetterNumber, null,
                                CarExp.fldDesc, "", fileid, false, null, null);
                            /*SignalrHub r = new SignalrHub();
                            r.ReloadCarExperience();*/
                            Car.SaveChanges();
                            return Json(new { Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = "ذخیره موفق", Err = 0 });
                        }
                    
                }
                else
                {//ویرایش رکورد ارسالی
                        var q = Car.sp_CarExperienceSelect("fldId", CarExp.fldID.ToString(), 0, null, "").FirstOrDefault();
                        if (q.fldAccept == true )
                        {
                            return Json(new { MsgTitle = "خطا", Msg = "سابقه مورد نظر تأیید شده و شما قادر به ویرایش آن نمی باشید.", Err = 1 });
                        }

                        if (q.fldUserID == 1)
                        {
                            return Json(new { Msg = "شما مجوز ویرایش را ندارید.", MsgTitle = "خطا", Err = 1 });
                        }
                        int? fileid = null;
                        if (Session["savePath"] != null)
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

                            Car.Sp_FilesInsert(a, _File, null, null, null, null);
                            System.IO.File.Delete(savePath);

                            if (a.Value != null)
                                fileid = Convert.ToInt32(a.Value);
                        }
                        else if (CarExp.fldFileId != null)
                            fileid = CarExp.fldFileId;
                        else if (CarExp.fldFileId == null)
                            return Json(new { Msg = "لطفا فایل مدرک را آپلود کنید.", MsgTitle = "خطا", Err = 1 });
                        Car.sp_CarExperienceUpdate(CarExp.fldID, CarExp.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldStartDate)
                            , MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldEndDate),
                            CarExp.fldMunicipalityID, CarExp.fldLetterNumber, null,
                            CarExp.fldDesc, "", fileid);
                        /*SignalrHub r = new SignalrHub();
                        r.ReloadCarExperience();*/
                        if (CarExp.fldFileId != null && Session["savePath"] != null)
                            Car.Sp_FilesDelete(CarExp.fldFileId);
                        Session.Remove("savePath");
                        return Json(new { Msg = "ویرایش با موفقیت انجام شد.", MsgTitle = "ویرایش موفق", Err = 0 });
                }
            }
            catch (Exception x)
            {
                if (Session["savePath"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["P_savePath"].ToString());
                    System.IO.File.Delete(Session["savePath"].ToString());
                    Session.Remove("savePath");
                }
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, null, x.Message, DateTime.Now, null);
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    var q = Car.sp_CarExperienceSelect("fldId", id.ToString(), 0, null, "").FirstOrDefault();
                    if (q.fldAccept == true)
                    {
                        return Json(new { Msg = "به دلیل تأیید شدن امکان حذف وجود ندارد", MsgTitle = "خطا", Err = 1 });
                    }
                    Car.sp_CarExperienceDelete(Convert.ToInt32(id),null, "");
                    return Json(new { Msg = "حذف با موفقیت انجام شد.", MsgTitle = "حذف موفق", Err = 0 });

            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException =x.Message;
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, null, x.Message, DateTime.Now, null);
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Err = 1 });
                //X.Msg.Show(new MessageBoxConfig
                //{
                //    Buttons = MessageBox.Button.OK,
                //    Icon = MessageBox.Icon.ERROR,
                //    Title = "خطا",
                //    Message = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید."
                //});
                //DirectResult result = new DirectResult();
                //return result;
            }
        }
        public ActionResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_CarExperienceSelect("fldId", id.ToString(), 1, null, "").FirstOrDefault();
                var q1 = Car.sp_MunicipalitySelect("fldId", q.fldMunicipalityID.ToString(), 1, null, "").FirstOrDefault();
                var q2 = Car.sp_CitySelect("fldId", q1.fldCityID.ToString(), 1, null, "").FirstOrDefault();
                var q3 = Car.sp_ZoneSelect("fldId", q2.fldZoneID.ToString(), 1, null, "").FirstOrDefault();
                var q4 = Car.sp_CountySelect("fldId", q3.fldCountyID.ToString(), 1, null, "").FirstOrDefault();
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
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, null, x.Message, DateTime.Now, null);
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
    }
}
