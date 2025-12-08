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
    public class CarFileController : Controller
    {
        public ActionResult Index(int id,int state)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 237))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعریف پرونده خودرو");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
                var q = p.sp_CarPlaqueSelect("fldID", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).First();
                ViewBag.fldCarPlaqueID = q.fldID;
                ViewBag.State = state;
                ViewBag.fldPlaqueTypeName = q.fldPlaqueTypeName;
                ViewBag.Plaque = q.fldPlaqueCityName + " | " + q.fldPlaqueSerial + " | " + q.fldPlaqueNumber;
                Session["fldCarPlaqueID"] = q.fldID;
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
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
        public ActionResult UploadContent1(HttpPostedFileBase UptContent1)
        {
            if (UptContent1 != null)
            {
                if (UptContent1.ContentLength <= 5242880)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(UptContent1.FileName);
                    string savePath = Server.MapPath(@"~\Uploaded\" + fileName);

                    Session["savePath1"] = savePath;
                    // The files are not actually saved in this demo
                    UptContent1.SaveAs(savePath);
                }
                else
                {
                    Session["er"] = "حجم فایل بزرگتر از حد مجاز است. ";
                    return Content("");
                }
            }
            return Content("");
        }
        public ActionResult RemoveContent1(string fileNames)
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
                Session.Remove("savePath1");
            }
            return Content("");
        }
        public ActionResult UploadContent2(HttpPostedFileBase UptContent2)
        {
            if (UptContent2 != null)
            {
                if (UptContent2.ContentLength <= 5242880)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(UptContent2.FileName);
                    string savePath = Server.MapPath(@"~\Uploaded\" + fileName);

                    Session["savePath2"] = savePath;
                    // The files are not actually saved in this demo
                    UptContent2.SaveAs(savePath);
                }
                else
                {
                    Session["er"] = "حجم فایل بزرگتر از حد مجاز است. ";
                    return Content("");
                }
            }
            return Content("");
        }
        public ActionResult RemoveContent2(string fileNames)
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
                Session.Remove("savePath2");
            }
            return Content("");
        }
        public ActionResult UploadContent3(HttpPostedFileBase UptContent3)
        {
            if (UptContent3 != null)
            {
                if (UptContent3.ContentLength <= 5242880)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(UptContent3.FileName);
                    string savePath = Server.MapPath(@"~\Uploaded\" + fileName);

                    Session["savePath3"] = savePath;
                    // The files are not actually saved in this demo
                    UptContent3.SaveAs(savePath);
                }
                else
                {
                    Session["er"] = "حجم فایل بزرگتر از حد مجاز است. ";
                    return Content("");
                }
            }
            return Content("");
        }
        public ActionResult RemoveContent3(string fileNames)
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
                Session.Remove("savePath3");
            }
            return Content("");
        }
        public ActionResult FillDateText(string year)
        {
            if (Convert.ToInt32(year) < 1900)
                return Json(new { date = year + "/01/01" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { date = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(year + "/03/21")) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModel(int? Noo)
        {
            if (Noo == null)
                Noo = 1;
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Noo == 1)
            {
                for (int i = 1340; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = 1950; i <= DateTime.Now.Year + 1; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.OrderByDescending(l => l.Text).Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_CarFileSelect("fldCarPlaqueID", Session["fldCarPlaqueID"].ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            Session.Remove("fldCarPlaqueID");
            return Json(q);

        }
        public FileContentResult Image(int id)
        {//برگرداندن عکس  
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            var pic = p.sp_ShortTermCountrySelect("fldId", id.ToString(), 30,Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldIcon != null)
                {
                    return File((byte[])pic.fldIcon, "jpg");
                }
            }
            return null;

        }
        
        public JsonResult GetCascadeShort(string cboCarMake)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            if (cboCarMake == "داخلی")
                return Json(car.sp_ShortTermCountrySelect("fldSymbol", "IR", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldSymbol }), JsonRequestBehavior.AllowGet);
            else
                return Json(car.sp_ShortTermCountrySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(p => p.fldSymbol != "IR").Select(c => new { fldID = c.fldID, fldName = c.fldSymbol }), JsonRequestBehavior.AllowGet);
           

        }
        public JsonResult GetCascadeColor()
        {

            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            return Json(car.sp_ColorCarSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldColor }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadePattern()
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            return Json(car.sp_CarPatternModelSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeMake()
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            return Json(car.sp_CarMakeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeAccount(int? cboCarMake)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var AccountType = car.sp_CarAccountTypeSelect("fldCarMakeID", cboCarMake.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(AccountType.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeCabin(int? cboCarAccountTypes)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var CabinType = car.sp_CabinTypeSelect("fldCarAccountTypeID", cboCarAccountTypes.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CabinType.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeSystem(int? cboCarCabin)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var CarSystem = car.sp_CarSystemSelect("fldCabinTypeID", cboCarCabin.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CarSystem.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeModel(int? cboSystem)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var CarModel = car.sp_CarModelSelect("fldCarSystemID", cboSystem.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CarModel.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeClass(int? cboModel)
        {
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var CarClass = car.sp_CarClassSelect("fldCarModelID", cboModel.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CarClass.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldCarPlaqueID", "fldVIN", "fldShasiNumber", "fldMotorNumber" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_CarFileSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().OrderBy(p => p.fldID);
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 239))
                {
                    Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_CarFileDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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

        

        public ActionResult Save(Models.CarFile care)
        {
            try
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                if (care.fldDesc == null)
                    care.fldDesc = "";
                int? Bargsabzfileid = null;
                int? Cartfileid = null;
                int? Sanadfileid = null;
                int? CartBack = null;

                if (care.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 238))
                    {
                        if (Session["savePath"] != null || Session["savePath1"] != null || Session["savePath2"] != null || Session["savePath3"] != null)
                        {
                            System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter b = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter c = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter d = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
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

                                Car.Sp_FilesInsert(a, _File);
                                System.IO.File.Delete(savePath);

                                Session.Remove("savePath");

                                if (a.Value != null)
                                    Bargsabzfileid = Convert.ToInt32(a.Value);
                            }
                            if (Session["savePath1"] != null)
                            {
                                string savePath = Session["savePath1"].ToString();

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

                                Car.Sp_FilesInsert(b, _File);
                                System.IO.File.Delete(savePath);

                                Session.Remove("savePath1");

                                if (b.Value != null)
                                    Cartfileid = Convert.ToInt32(b.Value);
                            }
                            if (Session["savePath2"] != null)
                            {
                                string savePath = Session["savePath2"].ToString();

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

                                Car.Sp_FilesInsert(c, _File);
                                System.IO.File.Delete(savePath);

                                Session.Remove("savePath2");

                                if (c.Value != null)
                                    Sanadfileid = Convert.ToInt32(c.Value);
                            }
                            if (Session["savePath3"] != null)
                            {
                                string savePath = Session["savePath3"].ToString();

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

                                Car.Sp_FilesInsert(d, _File);
                                System.IO.File.Delete(savePath);

                                Session.Remove("savePath3");

                                if (d.Value != null)
                                    CartBack = Convert.ToInt32(d.Value);
                            }
                            System.Data.Entity.Core.Objects.ObjectParameter _Carid = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(long));
                            System.Data.Entity.Core.Objects.ObjectParameter CarFileid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                            Car.sp_CarInsert(_Carid, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN, care.fldCarModelID,
                                care.fldCarClassID, care.fldCarColorID, care.fldModel,
                                MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance),
                                Convert.ToInt32(Session["UserId"]), care.fldDesc, Session["UserPass"].ToString());
                            //var q = Car.sp_CarSelect("fldVIN", care.fldVIN, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().First();
                            Car.sp_CarFileInsert(CarFileid, Convert.ToInt64(_Carid.Value), care.fldCarPlaqueID,
                                MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque),
                                Convert.ToInt32(Session["UserId"]), care.fldDesc,
                                Session["UserPass"].ToString(), Bargsabzfileid, Cartfileid, Sanadfileid, CartBack);
                            SmsSender sendsms = new SmsSender();
                            //sendsms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 2, Convert.ToInt32(CarFileid.Value), "", "", "", "");
                            return Json(new { data = "ذخیره با موفقیت انجام شد. کد پرونده: " + CarFileid.Value, state = 0 });
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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 240))
                    {
                        if (Session["savePath"] != null || Session["savePath1"] != null || Session["savePath2"] != null || Session["savePath3"] != null)
                        {
                            System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter b = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter c = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            System.Data.Entity.Core.Objects.ObjectParameter d = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            //int? Bargsabzfileid = null;
                            //int? Cartfileid = null;
                            //int? Sanadfileid = null;
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

                                Car.Sp_FilesInsert(a, _File);
                                System.IO.File.Delete(savePath);

                                Session.Remove("savePath");

                                if (a.Value != null)
                                    Bargsabzfileid = Convert.ToInt32(a.Value);
                            }
                            else if (care.fldBargSabzFileId != null)
                            {
                                Bargsabzfileid = care.fldBargSabzFileId;
                            }
                            if (Session["savePath1"] != null)
                            {
                                string savePath = Session["savePath1"].ToString();

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

                                Car.Sp_FilesInsert(b, _File);
                                System.IO.File.Delete(savePath);

                                Session.Remove("savePath1");

                                if (b.Value != null)
                                    Cartfileid = Convert.ToInt32(b.Value);
                            }
                            else if (care.fldCartFileId != null)
                            {
                                Cartfileid = care.fldCartFileId;
                            }
                            if (Session["savePath2"] != null)
                            {
                                string savePath = Session["savePath2"].ToString();

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

                                Car.Sp_FilesInsert(c, _File);
                                System.IO.File.Delete(savePath);

                                if (c.Value != null)
                                    Sanadfileid = Convert.ToInt32(c.Value);
                            }
                            else if (care.fldSanadForoshFileId != null)
                            {
                                Sanadfileid = care.fldSanadForoshFileId;
                            }
                            if (Session["savePath3"] != null)
                            {
                                string savePath = Session["savePath3"].ToString();

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

                                Car.Sp_FilesInsert(d, _File);
                                System.IO.File.Delete(savePath);

                                if (d.Value != null)
                                    CartBack = Convert.ToInt32(d.Value);
                            }
                            else if (care.fldSanadForoshFileId != null)
                            {
                                CartBack = care.fldCartBackFileId;
                            }
                        }
                        else if (care.fldBargSabzFileId != null || care.fldCartFileId != null || care.fldSanadForoshFileId != null || care.fldCartBackFileId != null)
                        {
                            Bargsabzfileid = care.fldBargSabzFileId;
                            Cartfileid = care.fldCartFileId;
                            Sanadfileid = care.fldSanadForoshFileId;
                            CartBack = care.fldCartBackFileId;
                        }
                        else
                            return Json(new { data = "لطفا فایل مدرک را آپلود کنید.", state = 1 });
                        Car.sp_CarUpdate(care.fldCarID, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN,
                            care.fldCarModelID, care.fldCarClassID,
                            care.fldCarColorID, care.fldModel, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance)
                            , Convert.ToInt32(Session["UserId"]), care.fldDesc, Session["UserPass"].ToString());
                        Car.sp_CarFileUpdate(care.fldID, care.fldCarID, care.fldCarPlaqueID,
                            MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque),
                            Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(),
                            Bargsabzfileid, Cartfileid, Sanadfileid, CartBack);
                        if (care.fldBargSabzFileId != null && Session["savePath"] != null)
                            Car.Sp_FilesDelete(care.fldBargSabzFileId);
                        if (care.fldCartFileId != null && Session["savePath1"] != null)
                            Car.Sp_FilesDelete(care.fldCartFileId);
                        if (care.fldSanadForoshFileId != null && Session["savePath2"] != null)
                            Car.Sp_FilesDelete(care.fldSanadForoshFileId);
                        if (care.fldCartBackFileId != null && Session["savePath3"] != null)
                            Car.Sp_FilesDelete(care.fldCartBackFileId);
                        Session.Remove("savePath");
                        Session.Remove("savePath1");
                        Session.Remove("savePath2");
                        Session.Remove("savePath3");
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
        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                var file = Car.sp_CarFileSelect("fldID", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var care = Car.sp_CarSelect("fldId", file.fldCarID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var plaq = Car.sp_CarPlaqueSelect("fldID", file.fldCarPlaqueID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                int CarMake = 0, CarAccount = 0, CabinType = 0, CarSystem = 0, CarModel = 0;
                var c_class = Car.sp_CarClassSelect("fldId", care.fldCarClassID.ToString(), 1, 1, "").FirstOrDefault();
                var c_model = Car.sp_CarModelSelect("fldId", c_class.fldCarModelID.ToString(), 1, 1, "").FirstOrDefault();
                var c_system = Car.sp_CarSystemSelect("fldId", c_model.fldCarSystemID.ToString(), 1, 1, "").FirstOrDefault();
                var c_CabinType = Car.sp_CabinTypeSelect("fldId", c_system.fldCabinTypeID.ToString(), 1, 1, "").FirstOrDefault();
                var c_Account = Car.sp_CarAccountTypeSelect("fldId", c_CabinType.fldCarAccountTypeID.ToString(), 1, 1, "").FirstOrDefault();
                var c_Make = Car.sp_CarMakeSelect("fldId", c_Account.fldCarMakeID.ToString(), 0, 1, "").FirstOrDefault();

                var Symbol = Car.sp_ShortTermCountrySelect("fldSymbol", "IR", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldSymbol });
                if (c_Make.fldName != "داخلی")
                    Symbol = Car.sp_ShortTermCountrySelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(p1 => p1.fldSymbol != "IR").Select(c => new { fldID = c.fldID, fldName = c.fldSymbol });


                CarMake = c_Account.fldCarMakeID;
                CarAccount = c_CabinType.fldCarAccountTypeID;
                CabinType = (int)c_system.fldCabinTypeID;
                CarSystem = c_model.fldCarSystemID;
                CarModel = c_class.fldCarModelID;

                var AccountType = Car.sp_CarAccountTypeSelect("fldCarMakeID", CarMake.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CabinType = Car.sp_CabinTypeSelect("fldCarAccountTypeID", CarAccount.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarSystem = Car.sp_CarSystemSelect("fldCabinTypeID", CabinType.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarModel = Car.sp_CarModelSelect("fldCarSystemID", CarSystem.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarClass = Car.sp_CarClassSelect("fldCarModelID", CarModel.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });

                string fldVIN = care.fldVIN.ToString();
                if (fldVIN.Length == 17)
                    fldVIN = fldVIN.Substring(0, 2);
                else
                    fldVIN = "..";
                if (fldVIN == "..")
                {
                    fldVIN = "IR";

                }
                var q = Car.sp_ShortTermCountrySelect("fldSymbol", fldVIN, 1, 1, "").First();

                return Json(new
                {
                    sumbolid = q.fldID,
                    symbol = fldVIN,
                    fldMotorNumber = care.fldMotorNumber,
                    fldShasiNumber = care.fldShasiNumber,
                    fldVIN = care.fldVIN,
                    fldCarModelID = care.fldCarModelID,
                    fldCarClassID = care.fldCarClassID,
                    fldCarColorID = care.fldCarColorID,
                    fldColorName = care.fldColor,
                    fldModel = care.fldModel,
                    fldStartDateInsurance = care.fldStartDateInsurance,
                    fldCarID = care.fldID,
                    fldCarPlaqueID = file.fldCarPlaqueID,
                    fldCarPlaquenum = plaq.fldPlaqueNumber,
                    fldDatePlaque = file.fldDatePlaque,
                    fldId = file.fldID,
                    CarMake = CarMake,
                    CarAccount = AccountType,
                    CabinType = _CabinType,
                    CarSystem = _CarSystem,
                    CarModel = _CarModel,
                    CarClass = _CarClass,
                    CarAccountId = CarAccount,
                    CabinTypeId = CabinType,
                    CarSystemId = CarSystem,
                    CarModelId = CarModel,
                    CarClassId = care.fldCarClassID,
                    Symbol = Symbol,
                    fldDesc = care.fldDesc,
                    fldBargSabzFileId = file.fldBargSabzFileId,
                    fldCartFileId = file.fldCartFileId,
                    fldSanadForoshFileId = file.fldSanadForoshFileId,
                    fldCartBackFileId = file.fldCartBackFileId
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
        //public JsonResult sub(string id)
        //{//نمایش اطلاعات جهت رویت کاربر
        //    return Json(new
        //    {
                
        //        model= id.Substring(1,2)
        //    }, JsonRequestBehavior.AllowGet);

        //}

    }
}
