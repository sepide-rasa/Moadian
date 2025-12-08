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

namespace Avarez.Areas.NewVer.Controllers.Guest
{
    public class KhodEzhari_ParvandeController : Controller
    {
        //
        // GET: /NewVer/KhodEzhari_Parvande/

        public ActionResult Index(long PelakId, long MalekId)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q = car.sp_CarPlaqueSelect("fldID", PelakId.ToString(), 1, null, null).First();
            PartialView.ViewBag.fldPelakName = q.fldPlaqueTypeName;
            PartialView.ViewBag.PelakId = PelakId;
            PartialView.ViewBag.MalekId = MalekId;
            return PartialView;
        }
        public ActionResult HelpParvande()
        {//باز شدن پنجره
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
        }
        public FileContentResult Image(int id)
        {//برگرداندن عکس  
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_ShortTermCountrySelect("fldId", id.ToString(), 30,null,null).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldIcon != null)
                {
                    return File((byte[])pic.fldIcon, "jpg");
                }
            }
            return null;

        }
        public ActionResult FillDateText(string year)
        {
            if (Convert.ToInt32(year) < 1900)
                return Json(new { date = year + "/01/01" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { date = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(year + "/01/01")) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveFile(Models.CarFile care, string CarMake, bool Match)
        {
             string Msg = ""; string MsgTitle = ""; var Err = 0;
             try
             {
                 Models.cartaxEntities Car = new Models.cartaxEntities();
                 if (care.fldDesc == null)
                     care.fldDesc = "";
                 int? Bargsabzfileid = null;
                 int? Cartfileid = null;
                 int? Sanadfileid = null;
                 int? CartBack = null;
                 string Datesanand = "";
                 if (CarMake == "1")
                 {
                     Datesanand = care.fldModel + "/01/01";
                 }
                 else
                 {
                     var datee = Convert.ToDateTime(care.fldModel + "-01-01");
                     Datesanand = MyLib.Shamsi.Miladi2ShamsiString(datee);
                 }

                 var fileee = Car.sp_CarFileSelect("fldid", care.fldID.ToString(), 1, 1, "").FirstOrDefault();

                 if (care.fldID == 0)
                 {//ثبت رکورد جدید

                     bool isupload = false;
                     if (Session["P_savePath"] != null)
                         isupload = true;
                     else if (Session["P_savePath1"] != null && Session["P_savePath3"] != null)
                         isupload = true;
                     else if (Session["P_savePath2"] != null)
                         isupload = true;

                     if (((Session["P_savePath1"] != null && Session["P_savePath3"] == null) || (Session["P_savePath1"] == null && Session["P_savePath3"] != null)))
                     {
                         if (Session["P_savePath1"] != null)
                         {
                             System.IO.File.Delete(Session["P_savePath1"].ToString());
                             Session.Remove("P_savePath1");
                         }
                         if (Session["P_savePath3"] != null)
                         {
                             System.IO.File.Delete(Session["P_savePath3"].ToString());
                             Session.Remove("P_savePath3");
                         }
                         if (Session["P_savePath2"] != null)
                         {
                             System.IO.File.Delete(Session["P_savePath2"].ToString());
                             Session.Remove("P_savePath2");
                         }
                         if (Session["P_savePath"] != null)
                         {
                             System.IO.File.Delete(Session["P_savePath"].ToString());
                             Session.Remove("P_savePath");
                         }
                         return Json(new
                         {
                             Msg = "صفحات اول و دوم کارت خودرو باید همزمان آپلود شوند.",
                             MsgTitle = "خطا",
                             Err = 1
                         }, JsonRequestBehavior.AllowGet);
                     }
                     else if (isupload == false)
                     {
                         return Json(new
                         {
                             Msg = "لطفا فایل مدرک را آپلود کنید.",
                             MsgTitle = "خطا",
                             Err = 1
                         }, JsonRequestBehavior.AllowGet);
                     }
                     else if (care.fldStartDateInsurance != Datesanand && Session["P_savePath2"] == null)
                     {
                         if (Session["P_savePath1"] != null)
                         {
                             System.IO.File.Delete(Session["P_savePath1"].ToString());
                             Session.Remove("P_savePath1");
                         }
                         if (Session["P_savePath3"] != null)
                         {
                             System.IO.File.Delete(Session["P_savePath3"].ToString());
                             Session.Remove("P_savePath3");
                         }
                         if (Session["P_savePath"] != null)
                         {
                             System.IO.File.Delete(Session["P_savePath"].ToString());
                             Session.Remove("P_savePath");
                         }
                         return Json(new
                         {
                             Msg = "لطفا فایل سند فروش را آپلود کنید.",
                             MsgTitle = "خطا",
                             Err = 1
                         }, JsonRequestBehavior.AllowGet);
                     }
                     else
                     {
                         //if (ForceScan == true && (Session["P_savePath"] != null || Session["P_savePath1"] != null || Session["P_savePath2"] != null || Session["P_savePath3"] != null))
                         //{
                         System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                         System.Data.Entity.Core.Objects.ObjectParameter b = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                         System.Data.Entity.Core.Objects.ObjectParameter c = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                         System.Data.Entity.Core.Objects.ObjectParameter d = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                         if (Session["P_savePath"] != null)
                         {
                             string savePath = Session["P_savePath"].ToString();

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

                             Car.Sp_FilesInsert(a, _File,null, null, null, null);

                             System.IO.File.Delete(savePath);
                             Session.Remove("P_savePath");

                             if (a.Value != null)
                                 Bargsabzfileid = Convert.ToInt32(a.Value);
                         }
                         if (Session["P_savePath1"] != null)
                         {
                             string savePath = Session["P_savePath1"].ToString();

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

                             Car.Sp_FilesInsert(b, _File, null, null, null, null);

                             System.IO.File.Delete(savePath);
                             Session.Remove("P_savePath1");

                             if (b.Value != null)
                                 Cartfileid = Convert.ToInt32(b.Value);
                         }
                         if (Session["P_savePath2"] != null)
                         {
                             string savePath = Session["P_savePath2"].ToString();

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

                             Car.Sp_FilesInsert(c, _File, null, null, null, null);

                             System.IO.File.Delete(savePath);
                             Session.Remove("P_savePath2");

                             if (c.Value != null)
                                 Sanadfileid = Convert.ToInt32(c.Value);
                         }
                         if (Session["P_savePath3"] != null)
                         {
                             string savePath = Session["P_savePath3"].ToString();

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

                             Car.Sp_FilesInsert(d, _File, null, null, null, null);

                             System.IO.File.Delete(savePath);
                             Session.Remove("P_savePath3");

                             if (d.Value != null)
                                 CartBack = Convert.ToInt32(d.Value);
                         }
                         System.Data.Entity.Core.Objects.ObjectParameter _Carid = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(long));
                         System.Data.Entity.Core.Objects.ObjectParameter CarFileid = new System.Data.Entity.Core.Objects.ObjectParameter("carfileid", sizeof(int));

                         /*Car.sp_CarInsert(_Carid, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN, care.fldCarModelID,
                             care.fldCarClassID, care.fldCarColorID, care.fldModel,
                             MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance),
                             null, care.fldDesc, null);
                         //var q = Car.sp_CarSelect("fldVIN", care.fldVIN, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().First();
                         Car.sp_CarFileInsert(CarFileid, Convert.ToInt64(_Carid.Value), care.fldCarPlaqueID,
                             MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque),null, care.fldDesc,null, Bargsabzfileid, Cartfileid,Sanadfileid, CartBack, false, null, null,null);*/
                         Car.sp_InsertCar_CarFile(_Carid, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN, care.fldCarModelID,
                                   care.fldCarClassID, care.fldCarColorID, care.fldModel, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance),
                                   "", care.fldCarPlaqueID, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque), null,
                                   "", "", Bargsabzfileid, Cartfileid, Sanadfileid, CartBack, false, null, null, care.fldTypeEntezami, Match, CarFileid);
                         SmsSender sendsms = new SmsSender();
                         //sendsms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 2, Convert.ToInt32(CarFileid.Value), "", "", "", "");
                         Msg = "ذخیره با موفقیت انجام شد. کد پرونده: " + CarFileid.Value;
                         MsgTitle = "ذخیره موفق";
                         Err = 0;

                         //return Json(new
                         //{
                         //    Msg = "ذخیره با موفقیت انجام شد. کد پرونده: " + CarFileid.Value,
                         //    MsgTitle = "ذخیره موفق",
                         //    Err = 0
                         //}, JsonRequestBehavior.AllowGet);
                         //else
                         //    return Json(new
                         //    {
                         //        Msg = "لطفا فایل مدرک را آپلود کنید.",
                         //        MsgTitle = "خطا",
                         //        Err = 1
                         //    }, JsonRequestBehavior.AllowGet);
                     }

                 }
                 else
                 {//ویرایش رکورد ارسالی

                     if (fileee.fldAccept == true)
                     {
                         return Json(new { MsgTitle = "خطا", Msg = "پرونده تأیید شده و شما قادر به ویرایش نمی باشید.", Err = 1 });
                     }

                     if (care.fldStartDateInsurance != Datesanand && care.fldSanadForoshFileId == null)
                     {
                         if (care.fldStartDateInsurance != Datesanand && Session["P_savePath2"] == null)
                         {
                             if (Session["P_savePath1"] != null)
                             {
                                 System.IO.File.Delete(Session["P_savePath1"].ToString());
                                 Session.Remove("P_savePath1");
                             }
                             if (Session["P_savePath3"] != null)
                             {
                                 System.IO.File.Delete(Session["P_savePath3"].ToString());
                                 Session.Remove("P_savePath3");
                             }
                             if (Session["P_savePath"] != null)
                             {
                                 System.IO.File.Delete(Session["P_savePath"].ToString());
                                 Session.Remove("P_savePath");
                             }
                             return Json(new
                             {
                                 Msg = "لطفا فایل سند فروش را آپلود کنید.",
                                 MsgTitle = "خطا",
                                 Err = 1
                             }, JsonRequestBehavior.AllowGet);
                         }
                     }

                     if (Session["P_savePath"] != null || Session["P_savePath1"] != null || Session["P_savePath2"] != null || Session["P_savePath3"] != null)
                     {
                         System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                         System.Data.Entity.Core.Objects.ObjectParameter b = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                         System.Data.Entity.Core.Objects.ObjectParameter c = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                         System.Data.Entity.Core.Objects.ObjectParameter d = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                         //int? Bargsabzfileid = null;
                         //int? Cartfileid = null;
                         //int? Sanadfileid = null;
                         if ((Session["P_savePath1"] != null && Session["P_savePath3"] == null) || (Session["P_savePath1"] == null && Session["P_savePath3"] != null))
                         {

                             if (care.fldCartFileId == null && care.fldCartBackFileId == null)
                             {
                                 if (Session["P_savePath1"] != null)
                                 {
                                     System.IO.File.Delete(Session["P_savePath1"].ToString());
                                     Session.Remove("P_savePath1");
                                 }
                                 if (Session["P_savePath3"] != null)
                                 {
                                     System.IO.File.Delete(Session["P_savePath3"].ToString());
                                     Session.Remove("P_savePath3");
                                 }
                                 if (Session["P_savePath2"] != null)
                                 {
                                     System.IO.File.Delete(Session["P_savePath2"].ToString());
                                     Session.Remove("P_savePath2");
                                 }
                                 if (Session["P_savePath"] != null)
                                 {
                                     System.IO.File.Delete(Session["P_savePath"].ToString());
                                     Session.Remove("P_savePath");
                                 }
                                 return Json(new
                                 {
                                     Msg = "صحفه اول و دوم کارت خودرو باید همزمان آپلود شوند.",
                                     MsgTitle = "خطا",
                                     Err = 1
                                 }, JsonRequestBehavior.AllowGet);
                             }
                         }

                         if (Session["P_savePath"] != null)
                         {
                             string savePath = Session["P_savePath"].ToString();

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
                             Session.Remove("P_savePath");

                             if (a.Value != null)
                                 Bargsabzfileid = Convert.ToInt32(a.Value);
                         }
                         else if (care.fldBargSabzFileId != null)
                         {
                             Bargsabzfileid = care.fldBargSabzFileId;
                         }
                         if (Session["P_savePath1"] != null)
                         {
                             string savePath = Session["P_savePath1"].ToString();

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

                             Car.Sp_FilesInsert(b, _File, null, null, null, null);

                             System.IO.File.Delete(savePath);
                             Session.Remove("P_savePath1");

                             if (b.Value != null)
                                 Cartfileid = Convert.ToInt32(b.Value);
                         }
                         else if (care.fldCartFileId != null)
                         {
                             Cartfileid = care.fldCartFileId;
                         }
                         if (Session["P_savePath2"] != null)
                         {
                             string savePath = Session["P_savePath2"].ToString();

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

                             Car.Sp_FilesInsert(c, _File, null, null, null, null);

                             System.IO.File.Delete(savePath);
                             Session.Remove("P_savePath2");
                             if (c.Value != null)
                                 Sanadfileid = Convert.ToInt32(c.Value);
                         }
                         else if (care.fldSanadForoshFileId != null)
                         {
                             Sanadfileid = care.fldSanadForoshFileId;
                         }
                         if (Session["P_savePath3"] != null)
                         {
                             string savePath = Session["P_savePath3"].ToString();

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

                             Car.Sp_FilesInsert(d, _File, null, null, null, null);

                             System.IO.File.Delete(savePath);
                             Session.Remove("P_savePath3");
                             if (d.Value != null)
                                 CartBack = Convert.ToInt32(d.Value);
                         }
                         else if (care.fldCartBackFileId != null)
                         {
                             CartBack = care.fldCartBackFileId;
                         }
                     }
                     else if (care.fldBargSabzFileId != null || care.fldCartFileId != null || care.fldSanadForoshFileId != null || care.fldCartBackFileId != null)
                     {
                         if (care.fldCartFileId != null && care.fldCartBackFileId == null)
                         {
                             if (Session["P_savePath1"] != null)
                             {
                                 System.IO.File.Delete(Session["P_savePath1"].ToString());
                                 Session.Remove("P_savePath1");
                             }
                             if (Session["P_savePath3"] != null)
                             {
                                 System.IO.File.Delete(Session["P_savePath3"].ToString());
                                 Session.Remove("P_savePath3");
                             }
                             if (Session["P_savePath"] != null)
                             {
                                 System.IO.File.Delete(Session["P_savePath"].ToString());
                                 Session.Remove("P_savePath");
                             }
                             if (Session["P_savePath2"] != null)
                             {
                                 System.IO.File.Delete(Session["P_savePath2"].ToString());
                                 Session.Remove("P_savePath2");
                             }
                             return Json(new
                             {
                                 Msg = "لطفا صفحه2 کارت خودرو را آپلود کنید.",
                                 MsgTitle = "خطا",
                                 Err = 1
                             }, JsonRequestBehavior.AllowGet);
                         }
                         else if (care.fldCartFileId == null && care.fldCartBackFileId != null)
                         {
                             if (Session["P_savePath1"] != null)
                             {
                                 System.IO.File.Delete(Session["P_savePath1"].ToString());
                                 Session.Remove("P_savePath1");
                             }
                             if (Session["P_savePath3"] != null)
                             {
                                 System.IO.File.Delete(Session["P_savePath3"].ToString());
                                 Session.Remove("P_savePath3");
                             }
                             if (Session["P_savePath"] != null)
                             {
                                 System.IO.File.Delete(Session["P_savePath"].ToString());
                                 Session.Remove("P_savePath");
                             }
                             if (Session["P_savePath2"] != null)
                             {
                                 System.IO.File.Delete(Session["P_savePath2"].ToString());
                                 Session.Remove("P_savePath2");
                             }
                             return Json(new
                             {
                                 Msg = "لطفا تصویر کارت خودرو را آپلود کنید.",
                                 MsgTitle = "خطا",
                                 Err = 1
                             }, JsonRequestBehavior.AllowGet);
                         }
                         Bargsabzfileid = care.fldBargSabzFileId;
                         Cartfileid = care.fldCartFileId;
                         Sanadfileid = care.fldSanadForoshFileId;
                         CartBack = care.fldCartBackFileId;
                     }
                     else
                     /*if (ForceScan == true && (Bargsabzfileid == null || Cartfileid == null || Sanadfileid == null || CartBack == null))*/
                     {
                         return Json(new
                         {
                             Msg = "لطفا فایل مدرک را آپلود کنید.",
                             MsgTitle = "خطا",
                             Err = 1
                         }, JsonRequestBehavior.AllowGet);
                     }

                     Car.sp_CarUpdate(care.fldCarID, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN,
                         care.fldCarModelID, care.fldCarClassID,
                         care.fldCarColorID, care.fldModel, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance)
                         , Match, null, care.fldDesc, null);
                     Car.sp_CarFileUpdate(care.fldID, care.fldCarID, care.fldCarPlaqueID,
                         MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque), null, "", null, Bargsabzfileid, Cartfileid, Sanadfileid, CartBack, null);

                     if (care.fldBargSabzFileId != null && Session["P_savePath"] != null)
                         Car.Sp_FilesDelete(care.fldBargSabzFileId);
                     if (care.fldCartFileId != null && Session["P_savePath1"] != null)
                         Car.Sp_FilesDelete(care.fldCartFileId);
                     if (care.fldSanadForoshFileId != null && Session["P_savePath2"] != null)
                         Car.Sp_FilesDelete(care.fldSanadForoshFileId);
                     if (care.fldCartBackFileId != null && Session["P_savePath3"] != null)
                         Car.Sp_FilesDelete(care.fldCartBackFileId);

                   


                     Msg = "ویرایش با موفقیت انجام شد.";
                     MsgTitle = "ویرایش موفق";
                     Err = 0;
                     //return Json(new
                     //{
                     //    Msg = "ویرایش با موفقیت انجام شد.",
                     //    MsgTitle = "ویرایش موفق",
                     //    Err = 0
                     //}, JsonRequestBehavior.AllowGet);
                 }
                 if (Session["P_savePath"] != null)
                 {
                     System.IO.File.Delete(Session["P_savePath"].ToString());
                     Session.Remove("P_savePath");
                 }
                 if (Session["P_savePath1"] != null)
                 {
                     System.IO.File.Delete(Session["P_savePath1"].ToString());
                     Session.Remove("P_savePath1");
                 }
                 if (Session["P_savePath2"] != null)
                 {
                     System.IO.File.Delete(Session["P_savePath2"].ToString());
                     Session.Remove("P_savePath2");
                 }
                 if (Session["P_savePath3"] != null)
                 {
                     System.IO.File.Delete(Session["P_savePath3"].ToString());
                     Session.Remove("P_savePath3");
                 }
                 return Json(new
                {
                    Msg = Msg,
                    MsgTitle = MsgTitle,
                    Err = Err
                }, JsonRequestBehavior.AllowGet);
             }
             catch (Exception x)
             {
                 if (Session["P_savePath"] != null)
                 {
                     System.IO.File.Delete(Session["P_savePath"].ToString());
                     Session.Remove("P_savePath");
                 }
                 if (Session["P_savePath1"] != null)
                 {
                     System.IO.File.Delete(Session["P_savePath1"].ToString());
                     Session.Remove("P_savePath1");
                 }
                 if (Session["P_savePath2"] != null)
                 {
                     System.IO.File.Delete(Session["P_savePath2"].ToString());
                     Session.Remove("P_savePath2");
                 }
                 if (Session["P_savePath3"] != null)
                 {
                     System.IO.File.Delete(Session["P_savePath3"].ToString());
                     Session.Remove("P_savePath3");
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

        public ActionResult ReloadFile(int PelakId)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            List<Models.sp_CarFileSelect> data = null;
            data = car.sp_CarFileSelect("fldCarPlaqueID", PelakId.ToString(), 30, null, null).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadParvande(StoreRequestParameters parameters, string Pelakid)
        {

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_CarFileSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_CarFileSelect> data1 = null;
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

                        data1 = m.sp_CarFileSelect(field, searchtext, 200, null, null).ToList();
                    else
                        data = m.sp_CarFileSelect(field, searchtext, 200, null, null).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_CarFileSelect("fldCarPlaqueID", Pelakid, 200, null, null).ToList();
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

            List<Avarez.Models.sp_CarFileSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
            public JsonResult DetailsFile(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var file = Car.sp_CarFileSelect("fldID", id.ToString(), 1, null,null).FirstOrDefault();
                var care = Car.sp_CarSelect("fldId", file.fldCarID.ToString(), 1,null,null).FirstOrDefault();
                var plaq = Car.sp_CarPlaqueSelect("fldID", file.fldCarPlaqueID.ToString(), 1, null,null).FirstOrDefault();
                int CarMake = 0, CarAccount = 0, CabinType = 0, CarSystem = 0, CarModel = 0;
                var c_class = Car.sp_CarClassSelect("fldId", care.fldCarClassID.ToString(), 1, 1, "").FirstOrDefault();
                var c_model = Car.sp_CarModelSelect("fldId", c_class.fldCarModelID.ToString(), 1, 1, "").FirstOrDefault();
                var c_system = Car.sp_CarSystemSelect("fldId", c_model.fldCarSystemID.ToString(), 1, 1, "").FirstOrDefault();
                var c_CabinType = Car.sp_CabinTypeSelect("fldId", c_system.fldCabinTypeID.ToString(), 1, 1, "").FirstOrDefault();
                var c_Account = Car.sp_CarAccountTypeSelect("fldId", c_CabinType.fldCarAccountTypeID.ToString(), 1, 1, "").FirstOrDefault();
                var c_Make = Car.sp_CarMakeSelect("fldId", c_Account.fldCarMakeID.ToString(), 0, 1, "").FirstOrDefault();

                var Symbol = Car.sp_ShortTermCountrySelect("fldSymbol", "IR", 0, null, null).Select(c => new { fldID = c.fldID, fldName = c.fldSymbol });
                if (c_Make.fldName != "داخلی")
                    Symbol = Car.sp_ShortTermCountrySelect("", "", 0, null, null).Where(p1 => p1.fldSymbol != "IR").Select(c => new { fldID = c.fldID, fldName = c.fldSymbol });

                var IsAccept = false;
                if (file.fldUserID != null)
                    IsAccept = true;

                CarMake = c_Account.fldCarMakeID;
                CarAccount = c_CabinType.fldCarAccountTypeID;
                CabinType = (int)c_system.fldCabinTypeID;
                CarSystem = c_model.fldCarSystemID;
                CarModel = c_class.fldCarModelID;

                var AccountType = Car.sp_CarAccountTypeSelect("fldCarMakeID", CarMake.ToString(), 0, null, null).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CabinType = Car.sp_CabinTypeSelect("fldCarAccountTypeID", CarAccount.ToString(), 0, null, null).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarSystem = Car.sp_CarSystemSelect("fldCabinTypeID", CabinType.ToString(), 0, null, null).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarModel = Car.sp_CarModelSelect("fldCarSystemID", CarSystem.ToString(), 0, null, null).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });
                var _CarClass = Car.sp_CarClassSelect("fldCarModelID", CarModel.ToString(), 0, null, null).Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName });

                string fldVIN = care.fldVIN.ToString();
                if (fldVIN.Length == 17)
                    fldVIN = fldVIN.Substring(0, 2);
                else
                    fldVIN = "..";
                if (fldVIN == "..")
                {
                    fldVIN = "IR";

                }
                var q = Car.sp_ShortTermCountrySelect("fldSymbol", fldVIN, 1, 1, "").FirstOrDefault();
                int sumbolid = 0;
                var ShortIcon="";
                if (q != null)
                {
                    sumbolid = q.fldID;
                    ShortIcon = Convert.ToBase64String(q.fldIcon);
                }          

                return Json(new
                {
                    sumbolid = sumbolid.ToString(),
                    ShortIcon=ShortIcon,
                    symbol = fldVIN,
                    fldMotorNumber = care.fldMotorNumber,
                    fldShasiNumber = care.fldShasiNumber,
                    fldVIN = care.fldVIN,
                    fldCarModelID = care.fldCarModelID,
                    fldCarClassID = care.fldCarClassID,
                    fldCarColorID = care.fldCarColorID.ToString(),
                    fldColorName = care.fldColor,
                    fldModel = care.fldModel.ToString(),
                    fldStartDateInsurance = care.fldStartDateInsurance,
                    fldCarID = care.fldID,
                    fldCarPlaqueID = file.fldCarPlaqueID,
                    fldCarPlaquenum = plaq.fldPlaqueNumber,
                    fldDatePlaque = file.fldDatePlaque,
                    fldId = file.fldID,
                    CarMake = CarMake.ToString(),
                    CarAccount = AccountType,
                    CabinType = _CabinType,
                    CarSystem = _CarSystem,
                    CarModel = _CarModel,
                    CarClass = _CarClass,
                    CarAccountId = CarAccount.ToString(),
                    CabinTypeId = CabinType.ToString(),
                    CarSystemId = CarSystem.ToString(),
                    CarModelId = CarModel.ToString(),
                    CarClassId = care.fldCarClassID.ToString(),
                    Symbol = Symbol,
                    fldDesc = care.fldDesc,
                    fldBargSabzFileId = file.fldBargSabzFileId,
                    fldCartFileId = file.fldCartFileId,
                    fldSanadForoshFileId = file.fldSanadForoshFileId,
                    fldCartBackFileId = file.fldCartBackFileId,
                    IsAccept = IsAccept,
                    fldMotabeghBaShasi=care.fldMotabeghBaShasi
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
             return Json(new
                {
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    MsgTitle = "خطا",
                    Err = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
            public JsonResult GetShort(string cboCarMake)
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                if (cboCarMake == "داخلی")
                    return Json(car.sp_ShortTermCountrySelect("fldSymbol", "IR", 0, null, null).Select(c => new { ID = c.fldID, Name = c.fldSymbol }), JsonRequestBehavior.AllowGet);
                else
                    return Json(car.sp_ShortTermCountrySelect("", "", 0, null, null).Where(p => p.fldSymbol != "IR").Select(c => new { ID = c.fldID, Name = c.fldSymbol }), JsonRequestBehavior.AllowGet);


            }
            public JsonResult GetColor()
            {

                Models.cartaxEntities car = new Models.cartaxEntities();
                return Json(car.sp_ColorCarSelect("", "", 0, null, null).Select(c => new { ID = c.fldID, Name = c.fldColor }), JsonRequestBehavior.AllowGet);
            }
            public JsonResult GetPattern()
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                return Json(car.sp_CarPatternModelSelect("", "", 0, null, null).Select(c => new { ID = c.fldID, Name = c.fldName }), JsonRequestBehavior.AllowGet);
            }
            public JsonResult GetMake()
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                return Json(car.sp_CarMakeSelect("", "", 0, null, null).Select(c => new { ID = c.fldID, Name = c.fldName }), JsonRequestBehavior.AllowGet);
            }
            public JsonResult GetAccount(int? cboCarMake)
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                var AccountType = car.sp_CarAccountTypeSelect("fldCarMakeID", cboCarMake.ToString(), 0, null, null);
                return Json(AccountType.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
            }
            public JsonResult GetCabin(int? cboCarAccountTypes)
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                var CabinType = car.sp_CabinTypeSelect("fldCarAccountTypeID", cboCarAccountTypes.ToString(), 0, null, null);
                return Json(CabinType.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
            }
            public JsonResult GetSystem(int? cboCarCabin)
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                var CarSystem = car.sp_CarSystemSelect("fldCabinTypeID", cboCarCabin.ToString(), 0, null, null);
                return Json(CarSystem.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
            }
            public JsonResult GetModel(int? cboSystem)
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                var CarModel = car.sp_CarModelSelect("fldCarSystemID", cboSystem.ToString(), 0, null, null);
                return Json(CarModel.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
            }

            public JsonResult GetClass(int? cboModel)
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                var CarClass = car.sp_CarClassSelect("fldCarModelID", cboModel.ToString(), 0, null, null);
                return Json(CarClass.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
            }
            public JsonResult GetYear(int? Noo)
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
                return Json(sal.OrderByDescending(l => l.Text).Select(p1 => new { ID = p1.Value, Name = p1.Text }), JsonRequestBehavior.AllowGet);
            }
            public ActionResult DeleteParvande(string id)
            {//حذف یک رکورد
                try
                {
                        Models.cartaxEntities Car = new Models.cartaxEntities();
                        if (Convert.ToInt32(id) != 0)
                        {
                            Car.sp_CarFileDelete(Convert.ToInt32(id),null, null);
                            return Json(new
                            {
                                Msg = "حذف با موفقیت انجام شد.",
                                MsgTitle = "حذف موفق",
                                Err = 0
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                Msg = "رکوردی برای حذف انتخاب نشده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
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
            public ActionResult Upload()
            {
                string Msg = "";
                try
                {

                    if (Session["P_savePath"] != null)
                    {
                        System.IO.File.Delete(Session["P_savePath"].ToString());
                        Session.Remove("P_savePath");
                    }
                    var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                    {
                        if (Request.Files[0].ContentLength <= 716800)
                        {
                            HttpPostedFileBase file = Request.Files[0];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["P_savePath"] = savePath;
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
                                Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
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
            public ActionResult Upload1()
            {
                string Msg = "";
                try
                {

                    if (Session["P_savePath1"] != null)
                    {
                        System.IO.File.Delete(Session["P_savePath1"].ToString());
                        Session.Remove("P_savePath1");
                    }
                    var extension = Path.GetExtension(Request.Files[1].FileName).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                    {
                        if (Request.Files[1].ContentLength <= 716800)
                        {
                            HttpPostedFileBase file = Request.Files[1];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["P_savePath1"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[1].FileName
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
                                Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
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
            public ActionResult Upload2()
            {
                string Msg = "";
                try
                {

                    if (Session["P_savePath2"] != null)
                    {
                        //string physicalPath = System.IO.Path.Combine(Session["P_savePath2"].ToString());
                        System.IO.File.Delete(Session["P_savePath2"].ToString());
                        Session.Remove("P_savePath2");
                    }
                    var extension = Path.GetExtension(Request.Files[3].FileName).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                    {
                        if (Request.Files[3].ContentLength <= 716800)
                        {
                            HttpPostedFileBase file = Request.Files[3];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["P_savePath2"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[3].FileName
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
                                Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
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
            public ActionResult Upload3()
            {
                string Msg = "";
                try
                {

                    if (Session["P_savePath3"] != null)
                    {
                        //string physicalPath = System.IO.Path.Combine(Session["P_savePath3"].ToString());
                        System.IO.File.Delete(Session["P_savePath3"].ToString());
                        Session.Remove("P_savePath3");
                    }
                    var extension = Path.GetExtension(Request.Files[2].FileName).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tiff" || extension == ".tif")
                    {
                        if (Request.Files[2].ContentLength <= 716800)
                        {
                            HttpPostedFileBase file = Request.Files[2];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["P_savePath3"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[2].FileName
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
                                Message = "حجم فایل انتخابی می بایست کمتر از 700کیلوبایت باشد."
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
            public FileContentResult Download(int FileId)
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                var q = car.Sp_FilesSelect(FileId).FirstOrDefault();

                if (q != null)
                {
                    MemoryStream st = new MemoryStream(q.fldImage);
                    return File(st.ToArray(), MimeType.Get(".jpg"), "DownloadFile.jpg");
                }
                return null;
            }
    }
}
