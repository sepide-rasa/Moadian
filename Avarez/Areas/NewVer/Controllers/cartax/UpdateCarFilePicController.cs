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

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class UpdateCarFilePicController : Controller
    {
        //
        // GET: /NewVer/UpdateCarFilePic/

        public ActionResult Index(int CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var CarId = Car.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarID;
            PartialView.ViewBag.CarId = CarId;
            return PartialView;
        }
        public ActionResult UploadBargSabz()
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
                    System.IO.File.Delete(physicalPath);
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg")
                {
                    if (Request.Files[0].ContentLength <= 5242880)
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
                            Message = "حجم فایل انتخابی می بایست کمتر از 5 مگابایت باشد."
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
        public ActionResult UploadCartKhodro()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            string Msg = "";
            try
            {
                if (Session["savePath1"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath1"].ToString());
                    Session.Remove("savePath1");
                    System.IO.File.Delete(physicalPath);
                }
                var extension = Path.GetExtension(Request.Files[1].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg")
                {
                    if (Request.Files[1].ContentLength <= 5242880)
                    {
                        HttpPostedFileBase file = Request.Files[1];
                        string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                        file.SaveAs(savePath);
                        Session["savePath1"] = savePath;
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
                            Message = "حجم فایل انتخابی می بایست کمتر از 5 مگابایت باشد."
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
        public ActionResult UploadCartKhodro_P()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            string Msg = "";
            try
            {
                if (Session["savePath3"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath3"].ToString());
                    Session.Remove("savePath3");
                    System.IO.File.Delete(physicalPath);
                }
                var extension = Path.GetExtension(Request.Files[2].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg")
                {
                    if (Request.Files[2].ContentLength <= 5242880)
                    {
                        HttpPostedFileBase file = Request.Files[2];
                        string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                        file.SaveAs(savePath);
                        Session["savePath3"] = savePath;
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
                            Message = "حجم فایل انتخابی می بایست کمتر از 5 مگابایت باشد."
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
        public ActionResult UploadSanad()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            string Msg = "";
            try
            {
                if (Session["savePath2"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath2"].ToString());
                    Session.Remove("savePath2");
                    System.IO.File.Delete(physicalPath);
                }
                var extension = Path.GetExtension(Request.Files[3].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg")
                {
                    if (Request.Files[3].ContentLength <= 5242880)
                    {
                        HttpPostedFileBase file = Request.Files[3];
                        string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                        file.SaveAs(savePath);
                        Session["savePath2"] = savePath;
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
                            Message = "حجم فایل انتخابی می بایست کمتر از 5 مگابایت باشد."
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
        public ActionResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
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
                    fldCartBackFileId = file.fldCartBackFileId,
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
        public FileContentResult showFile(string dc, int id)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();

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
        public ActionResult Save(Models.CarFile car)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
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

                            Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
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
                        return Json(new { Msg = "لطفا فایل مدرک را آپلود کنید.", MsgTitle = "خطا", Err = 1 });
                    Car.sp_CarFileUpdate(care.fldID, care.fldCarID, care.fldCarPlaqueID, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque),
                        Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Bargsabzfileid, Cartfileid, Sanadfileid, CartBack,null);
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
                    return Json(new { Msg = "ویرایش با موفقیت انجام شد.", MsgTitle = "ویرایش موفق", Err = 0 });
                }

                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "شما مجاز به دسترسی نمی باشید."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                    //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    //return RedirectToAction("error", "Metro");
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
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید."
                });
                DirectResult result = new DirectResult();
                return result;
               // return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }
    }
}
