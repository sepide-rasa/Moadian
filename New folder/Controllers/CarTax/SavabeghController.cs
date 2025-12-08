using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
using System.IO;

namespace Avarez.Controllers.CarTax
{
    [Authorize]
    public class SavabeghController : Controller
    {
        //
        // GET: /Savabegh/

        public ActionResult Index(int id,int state)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 242))
            {
                Session.Remove("savePath");
                Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
                var q = p.sp_SelectCarDetils(id).FirstOrDefault();
                Session["carId"] = q.fldCarID;
                Session["CarfileId"] = q.fldID;
                ViewBag.CarId = q.fldCarID;
                ViewBag.CarFileId = q.fldID;
                ViewBag.State = state;
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->انتقال سوابق");
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
        public ActionResult EditSavabegh()
        {//بارگذاری صفحه اصلی 
            
            return PartialView();

        }
        
        public ActionResult UploadContent(HttpPostedFileBase UptContent)
        {
            if (UptContent != null)
            {
                if (UptContent.ContentLength <= 5242880)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(UptContent.FileName);
                    string savePath = Server.MapPath(@"~\Uploaded\" + fileName);
                    
                    Session["savePath"] = savePath;
                    // The files are not actually saved in this demo
                    UptContent.SaveAs(savePath);
                }
                else
                {
                    Session["er"] = "حجم فایل بزرگتر از حد مجاز است. ";
                    return Content("");
                }
            }
            return Content("");
        }
        public ActionResult RemoveContent(string fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                string physicalPath = Server.MapPath(@"~\Uploaded\" + fileNames);
                if (System.IO.File.Exists(physicalPath))
                {
                    // The files are not actually removed in this demo
                    System.IO.File.Delete(physicalPath);
                }
                Session.Remove("savePath");
            }
            return Content("");
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();

            var q = m.sp_CarExperienceSelect("fldCarID", Session["carId"].ToString(), 50, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            Session.Remove("carId");
            return Json(q);

        }
        
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldCarID" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_CarExperienceSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public string GetFromDate(int? FromYear)
        {
            string date = FromYear + "/01/01";
            return date;
        }
        public string GetToDate(int ToYear)
        {
            string date = ToYear + "/12/29";
            if(MyLib.Shamsi.Iskabise(ToYear))
                date = ToYear + "/12/30";
            return date;
        }
        public JsonResult GetFromYear(int carid)
        {
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            var car = p.sp_CarSelect("fldid", carid.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            List<SelectListItem> sal = new List<SelectListItem>();
            if (car.fldModel <1900)
            {
                for (int i = car.fldModel; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(car.fldModel.ToString() + "/05/21")).Substring(0, 4)); i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetToYear(int value)
        {
            List<SelectListItem> sal = new List<SelectListItem>();

            for (int i = value; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)) - 1; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString();
                item.Value = i.ToString();
                sal.Add(item);
            }

            return Json(sal.Select(p1 => new { fldID = p1.Value, fldName = p1.Text }).OrderByDescending(k => k.fldID), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 244))
                {
                    Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        //var q = Car.sp_CarExperienceSelect("fldId", id.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        //if (q.fldUserID == 1 && Convert.ToInt32(Session["UserId"]) != 1)
                        //{
                        //    return Json(new { data = "شما مجوز ویرایش را ندارید.", state = 1 });
                        //}
                        Car.sp_CarExperienceDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public ActionResult Save(Models.CarExp CarExp)
        {
            try
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                if (CarExp.fldDesc == null)
                    CarExp.fldDesc = "";

                CarExp.fldStartDate = GetFromDate(CarExp.fldFromYear);
                CarExp.fldEndDate = GetToDate(CarExp.fldToYear);

                if (CarExp.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 243))
                    {
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

                            Car.Sp_FilesInsert(a, _File);
                            System.IO.File.Delete(savePath);

                            Session.Remove("savePath");
                            int? fileid = null;
                            if (a.Value != null)
                                fileid = Convert.ToInt32(a.Value);
                            Car.sp_CarExperienceInsert(CarExp.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldStartDate),
                                MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldEndDate),
                                CarExp.fldMunicipalityID, CarExp.fldLetterNumber, Convert.ToInt32(Session["UserId"]),
                                CarExp.fldDesc, Session["UserPass"].ToString(), fileid);
                            Car.SaveChanges();
                            return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                        }
                        else
                            return Json(new { data = "لطفا فایل مدرک را آپلود کنید.", state = 1 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 245))
                    {
                        var q = Car.sp_CarExperienceSelect("fldId", CarExp.fldID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (q.fldUserID == 1 && Convert.ToInt32(Session["UserId"]) != 1)
                        {
                            return Json(new { data = "شما مجوز ویرایش را ندارید.", state = 1 });
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

                            Car.Sp_FilesInsert(a, _File);
                            System.IO.File.Delete(savePath);

                            if (a.Value != null)
                                fileid = Convert.ToInt32(a.Value);
                        }
                        else if (CarExp.fldFileId != null)
                            fileid = CarExp.fldFileId;
                        else
                            return Json(new { data = "لطفا فایل مدرک را آپلود کنید.", state = 1 });
                        Car.sp_CarExperienceUpdate(CarExp.fldID, CarExp.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldStartDate)
                            , MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldEndDate),
                            CarExp.fldMunicipalityID, CarExp.fldLetterNumber, Convert.ToInt32(Session["UserId"]),
                            CarExp.fldDesc, Session["UserPass"].ToString(), fileid);
                        if (CarExp.fldFileId != null && Session["savePath"] != null)
                            Car.Sp_FilesDelete(CarExp.fldFileId);
                        Session.Remove("savePath");
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
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
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
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                var q = Car.sp_CarExperienceSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q1 = Car.sp_MunicipalitySelect("fldId", q.fldMunicipalityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q2 = Car.sp_CitySelect("fldId", q1.fldCityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q3 = Car.sp_ZoneSelect("fldId", q2.fldZoneID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q4 = Car.sp_CountySelect("fldId", q3.fldCountyID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldID,
                    fldDesc = q.fldDesc,
                    fldFromYear = q.fldStartDate.Substring(0,4),
                    fldToYear = q.fldEndDate.Substring(0, 4),
                    fldLetterNumber = q.fldLetterNumber,
                    fldMunID = q.fldMunicipalityID,
                    fldStateId = q4.fldStateID,
                    fldFileId = q.fldFileId
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }
        public FileContentResult showFile(int id)
        {//برگرداندن عکس 
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            
            var image = p.Sp_FilesSelect(id).FirstOrDefault();
            if (image != null)
            {
                if (image.fldImage != null)
                {
                    return File((byte[])image.fldImage, "jpg");
                }
            }
            return null;
        }
        public JsonResult DateFill(int carID)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                var q = Car.sp_CarSelect("fldID", carID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                string FromDate = "";
                string EndDate = "";
                string modelDate = "";
                var timenow=MyLib.Shamsi.Miladi2ShamsiString(Car.sp_GetDate().FirstOrDefault().CurrentDateTime);
               
                if(q.fldModel>=1900)
                {
                    modelDate = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(q.fldModel + "-01-01"));
                    FromDate = Convert.ToInt32(modelDate.Substring(0, 4)) + 1 + "/01/01";
                }
                else
                {
                    
                    FromDate = q.fldModel + "/01/01";
                }
                if(MyLib.Shamsi.Iskabise(Convert.ToInt32(timenow.Substring(0,4)))==true)
                     EndDate = Convert.ToInt32(timenow.Substring(0, 4))-1+"/12/30";
                else
                    EndDate = Convert.ToInt32(timenow.Substring(0, 4)) - 1 + "/12/29";
                return Json(new
                {

                    fldStartDate =FromDate,
                    fldEndDate = EndDate
                    
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
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
