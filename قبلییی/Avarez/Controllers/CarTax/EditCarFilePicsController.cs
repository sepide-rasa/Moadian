using Avarez.Controllers.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.CarTax
{
    [Authorize]
    public class EditCarFilePicsController : Controller
    {
        //
        // GET: /EditCarFilePics/

        public ActionResult Index(int carid)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            ViewBag.carid = carid;
            return PartialView();
        }

        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var car = Car.sp_SelectCarDetils(id).FirstOrDefault();
                var file = Car.sp_CarFileSelect("fldID", car.fldID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                return Json(new
                {
                    fldId = file.fldID,
                    fldBargSabzFileId = file.fldBargSabzFileId,
                    fldCartFileId = file.fldCartFileId,
                    fldSanadForoshFileId = file.fldSanadForoshFileId,
                    fldCartBackFileId = file.fldCartBackFileId
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
        public ActionResult Save(Models.CarFile car)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (car.fldDesc == null)
                    car.fldDesc = "";
                int? Bargsabzfileid = null;
                int? Cartfileid = null;
                int? Sanadfileid = null;
                int? CartBack = null;
                var care = Car.sp_CarFileSelect("fldid", car.fldID.ToString(), 0, 1, "").FirstOrDefault();
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

                            Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]),null,null,null);
                            System.IO.File.Delete(savePath);


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

                            Car.Sp_FilesInsert(b, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
                            System.IO.File.Delete(savePath);


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

                            Car.Sp_FilesInsert(c, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
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

                            Car.Sp_FilesInsert(d, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
                            System.IO.File.Delete(savePath);

                            if (d.Value != null)
                                CartBack = Convert.ToInt32(d.Value);
                        }
                        else if (care.fldCartBackFileId != null)
                        {
                            CartBack = care.fldCartBackFileId;
                        }
                    }
                    else if (care.fldBargSabzFileId != null || care.fldCartFileId != null || care.fldSanadForoshFileId != null)
                    {
                        Bargsabzfileid = care.fldBargSabzFileId;
                        Cartfileid = care.fldCartFileId;
                        Sanadfileid = care.fldSanadForoshFileId;
                        CartBack = care.fldCartBackFileId;
                    }
                    else
                        return Json(new { data = "لطفا فایل مدرک را آپلود کنید.", state = 1 });
                    Car.sp_CarFileUpdate(care.fldID, care.fldCarID, care.fldCarPlaqueID, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque),
                        Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Bargsabzfileid, Cartfileid, Sanadfileid,CartBack,null);
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
