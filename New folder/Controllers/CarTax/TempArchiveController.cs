using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace Avarez.Controllers.CarTax
{
    public class TempArchiveController : Controller
    {
        //
        // GET: /TempArchive/

        public ActionResult Index(int id, int state)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            ViewBag.CarfileID = id;
            Models.ComplicationsCarDBEntities car = new Models.ComplicationsCarDBEntities();
            var q = car.sp_CarFileSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
            ViewBag.carId = q.fldCarID;
            ViewBag.State = state;
            Session["CarfileId"] = id;
            ViewBag.SiteURL = "http://" + Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"];
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
                    string savePath = Server.MapPath(@"~\Uploaded\" + Session["CarfileId"]);
                    if (Directory.Exists(savePath) == false)
                    {
                        DirectoryInfo di = Directory.CreateDirectory(savePath);
                    }
                    Session["savePath"] = savePath;
                    // The files are not actually saved in this demo
                    UptContent.SaveAs(savePath + "\\" + fileName);
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
        public ActionResult SaveToFile(int carfile)
        {
            String strImageName;
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            HttpPostedFile uploadfile = files["RemoteFile"];
            strImageName = uploadfile.FileName;
            if (uploadfile.ContentLength > 0 && uploadfile.ContentLength <= 5242880)
            {

                Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
                string savePath = Server.MapPath(@"~\Uploaded\" + Session["CarfileId"] + strImageName + ".jpg");
                uploadfile.SaveAs(savePath);

                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                byte[] _File = stream.ToArray();

                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                p.sp_TempArchiveInsert(carfile, _File, Convert.ToInt32(Session["UserId"]), "");
                System.IO.File.Delete(savePath);
            }
            return null;
        }
        //public ActionResult ScanedUpload(long id)
        //{
        //    if (Request.Files.Count > 0)
        //    {
        //        Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
        //        string savePath = Server.MapPath(@"~\Uploaded\" + Session["CarfileId"] + ".pdf");
        //        Request.Files[0].SaveAs(savePath);
        //        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
        //        byte[] _File = stream.ToArray();
        //        var q = p.sp_TempArchiveSelect("fldCarFileId", id.ToString(), 0).FirstOrDefault();
        //        if (q == null)
        //        {
        //            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
        //            p.sp_TempArchiveInsert(id, _File, Convert.ToInt32(Session["UserId"]), "");
        //            System.IO.File.Delete(savePath);
        //            return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
        //        }
        //        else
        //        {
        //            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
        //            p.sp_TempArchiveUpdate(q.fldId, id, _File, Convert.ToInt32(Session["UserId"]), "");
        //            System.IO.File.Delete(savePath);
        //            return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
        //        }
        //    }
        //    return Json("", JsonRequestBehavior.AllowGet);
        //}
        public ActionResult ContentSave(int id)
        {
            if (id > 0)
            {
                Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
                if (Session["savePath"] != null)
                {
                    string savePath = Session["savePath"].ToString();
                    string[] files = System.IO.Directory.GetFiles(savePath);

                    //var q = p.sp_TempArchiveSelect("fldCarFileId", id.ToString(), 0).FirstOrDefault();
                    //if (q == null)
                    //{
                    foreach (string fileName in files)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(fileName));
                        var Ex = Path.GetExtension(fileName);
                        if (Ex == ".tiff" || Ex == ".tif")
                        {
                            using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(fileName))
                            {
                                image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                            }
                        }
                        byte[] _File = stream.ToArray();
                        p.sp_TempArchiveInsert(id, _File, Convert.ToInt32(Session["UserId"]), "");
                        System.IO.File.Delete(fileName);
                    }
                    //}
                    //else
                    //{
                    //    foreach (string fileName in files)
                    //    {
                    //        //p.sp_TempArchiveDelete(q.fldId, Convert.ToInt32(Session["UserId"]));
                    //        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(fileName));
                    //        byte[] _File = stream.ToArray();
                    //        p.sp_TempArchiveInsert(id, _File, Convert.ToInt32(Session["UserId"]), "");
                    //        System.IO.File.Delete(fileName);
                    //    }

                    //}
                    Session.Remove("savePath");
                    Directory.Delete(savePath, true);

                    return Json(new { data = "آپلود با موفقیت انجام شد.", state = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { data = Session["er"].ToString(), state = 1 }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { data = "آپلود با موفقیت انجام نشد.", state = 1 }, JsonRequestBehavior.AllowGet);
        }
        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            var pic = p.sp_TempArchiveSelect("fldId", id.ToString(),0).FirstOrDefault();
            ViewBag.tempId = id;
            if (pic != null)
            {
                if (pic.fldPic != null)
                {
                    return File((byte[])pic.fldPic, "jpg");
                }
            }
            return null;
        }
        public string listImage(int id)
        {
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            var q = p.sp_TempArchiveSelect("fldCarFileId", id.ToString(), 0).ToList();
            string Pics = "";
            for (int i = 0; i < q.Count; i++)
            {
                 string p1 = Url.Content("~/TempArchive/Image/" + q[i].fldId);
                Pics += "<input type='checkbox' value='" + q[i].fldId + "' id='" + q[i].fldId + "' /> <img src='" + p1 + "' alt='' width='120px'/>";
                if ( (i+1) % 3 == 0) 
                    Pics += "</br>";
            }
            //foreach (var item in q)
            //{
            //    string p1 = Url.Content("~/TempArchive/Image/" + item.fldId);

            //    Pics += "<input type='checkbox' value='" + item.fldId + "' id='" + item.fldId + "' /> <img src='" + p1 + "' alt='' width='120px'/></br>";
            //}
            return (Pics);
          
        }
        public string ArchivelistImage(long carid, int Pid)
        {
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            var carfiles = p.sp_CarFileSelect("fldCarId", carid.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            string Pics = "";
            foreach (var item1 in carfiles)
            {
                var q = p.sp_DigitalArchiveSelect("fldDigitalTreeId", Pid.ToString(), 0).Where(l => l.fldCarFileId == item1.fldID).ToList();
                foreach (var item2 in q)
                {
                    var m = p.sp_DigitalArchive_DetailSelect("fldDigitalArchiveId", item2.fldID.ToString(), 0).ToList();
                    //foreach (var item in m)
                        for (int i = 0; i < m.Count; i++)
                    {
                        string p1 = Url.Content("~/ListImageInTree/Image/" + m[i].fldId);
                        Pics += "<img class='preview' id='img" + m[i].fldId + "' src='" + p1 + "' alt='' width='120px'/>";
                        if ((i + 1) % 5 == 0)
                            Pics += "</br>";
                    }
                }
            }
            return (Pics);
        }
        public JsonResult _ProductTree(int? id)
        {
            try
            {
                var p = new Models.ComplicationsCarDBEntities();

                if (id != null)
                {
                    var rols = (from k in p.sp_tblDigitalTreeSelect("PId", id.ToString(), 0)
                                select new
                                {
                                    id = k.fldId,
                                    Name = k.fldName,
                                    hasChildren = p.sp_tblDigitalTreeSelect("PId", id.ToString(), 0).Any()

                                });
                    return Json(rols, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var rols = (from k in p.sp_tblDigitalTreeSelect("", "", 0)
                                select new
                                {
                                    id = k.fldId,
                                    Name = k.fldName,
                                    hasChildren = p.sp_tblDigitalTreeSelect("", "", 0).Any()

                                });
                    return Json(rols, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                return null;
            }
        }
        
        public ActionResult Save(List<Models.digtalArchive> digtalArchive, Models.digital digital)
        {
            try
            {
                Models.ComplicationsCarDBEntities Car = new Models.ComplicationsCarDBEntities();
                var Tree = Car.sp_tblDigitalTreeSelect("fldId", digital.PId.ToString(), 0).FirstOrDefault();
                var q = Car.sp_DigitalArchiveSelect("fldCarFileId", digital.fldCarFileId.ToString(), 0).Where(h => h.fldDigitalTreeId == digital.fldDigitalTreeId).FirstOrDefault();
                if (q == null && Tree.PId!=null)
                {//ثبت رکورد جدید

                    System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    Car.sp_DigitalArchiveInsert(_id, digital.fldCarFileId, digital.fldDigitalTreeId, Convert.ToInt32(Session["UserId"]), "");
                    foreach (var item in digtalArchive)
                    {
                        var s = Car.sp_TempArchiveSelect("fldId", item.PicId.ToString(), 0).FirstOrDefault();
                        if (s != null)
                        {

                            Car.sp_DigitalArchive_DetailInsert(Convert.ToInt32(_id.Value), s.fldPic, Convert.ToInt32(Session["UserId"]), "");
                            Car.sp_TempArchiveDelete(s.fldId, Convert.ToInt32(Session["UserId"]));

                        }


                    }
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                }
                else 
                {//ویرایش رکورد ارسالی
 
                    if (Tree.PId != null)
                    {
                        //Car.sp_DigitalArchiveUpdate(digital.fldID, digital.fldCarFileId, digital.fldDigitalTreeId, Convert.ToInt32(Session["UserId"]), "");
                        foreach (var item in digtalArchive)
                        {
                            var s = Car.sp_TempArchiveSelect("fldId", item.PicId.ToString(), 0).FirstOrDefault();
                            if (s != null)
                            {

                                //Car.sp_DigitalArchive_DetailUpdate(item.fldId, digital.fldID, s.fldPic, Convert.ToInt32(Session["UserId"]), "");
                                Car.sp_DigitalArchive_DetailInsert(q.fldID, s.fldPic, Convert.ToInt32(Session["UserId"]), "");
                                Car.sp_TempArchiveDelete(s.fldId, Convert.ToInt32(Session["UserId"]));

                            }

                        }
                        return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                    }
                    
                }
                return Json(new { data = "شما مجازه به ذخیره در این شاخه نمی باشید.", state = 0 });
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
