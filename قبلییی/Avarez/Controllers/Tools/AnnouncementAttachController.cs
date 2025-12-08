using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.IO;
using System.Collections;
namespace Avarez.Controllers.Tools
{
    public class AnnouncementAttachController : Controller
    {
        //
        // GET: /AnnouncementAttach/
        public ActionResult Index(int id)
        {
            //بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            @ViewBag.fldAnnouncementID = id;
            Session["fldAnnouncementID"] = id;
            return PartialView();
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_AnnouncementManagerAttachmentSelect("fldAnnouncementID", Session["fldAnnouncementID"].ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            Session.Remove("fldAnnouncementID");
            return Json(q);

        }
        public JsonResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Convert.ToInt32(id) != 0)
                {
                    Car.sp_AnnouncementManagerAttachmentDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                    return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
                }
                else
                {
                    return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 });
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
        public ActionResult Upload()
        {
            var file = Request.Files["Filedata"];
            if (file.ContentLength <= 204800)
            {
                string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                file.SaveAs(savePath);
                Session["savePath"] = savePath;
                return Content(Url.Content(@"~\Uploaded\" + file.FileName));
            }
            return null;
        }

        public FileContentResult FileExport(int id)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q = car.sp_AnnouncementManagerAttachmentFile(id).FirstOrDefault();

            return File(q.fldAttachment, MimeType.Get(q.fldFileName.Split('.').Last()), q.fldFileName);
        }
        public JsonResult Detail(int id)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q = car.sp_AnnouncementManagerSelect("fldId", id.ToString(), 1, 
                Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var q1 = car.sp_AnnouncementManagerAttachmentSelect("fldAnnouncementID", q.fldID.ToString(), 0,
                Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            int[] attach = null;
            if (q1.Count > 0)
            {
                int i=0;
                attach = new int[q1.Count];
                foreach (var item in q1)
                {
                    attach[i] = item.fldID;
                    i++;
                }
            }
            return Json(new { body = q.fldMemo, date = q.fldDate, title = q.fldSubject, attach = attach });
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldAnnouncementID" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_AnnouncementManagerAttachmentSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(Models.sp_AnnouncementManagerAttachmentSelect Announcement)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Announcement.fldDesc == null)
                    Announcement.fldDesc = "";
                //ثبت رکورد جدید
                byte[] Attach_file = null;
                if (Session["savePath"] != null)
                {
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                    System.IO.File.Delete(Session["savePath"].ToString());
                    string filename = System.IO.Path.GetFileName(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    Attach_file = stream.ToArray();

                    Car.sp_AnnouncementManagerAttachmentInsert(Announcement.fldAnnouncementID, Attach_file,
                    filename, Convert.ToInt32(Session["UserId"]), Announcement.fldDesc, Session["UserPass"].ToString());
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                }
                else
                    return Json(new { data = "لطفا فایل ضمیمه را وارد کنید.", state = 1 });

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
