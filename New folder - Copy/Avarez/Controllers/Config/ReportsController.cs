using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.IO;
using System.Xml;

using Avarez.Controllers.Users;

namespace Avarez.Controllers.Config
{
 
    public class ReportsController : Controller
    {
        public ActionResult Index(int id)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 170))
            {
                Models.cartaxEntities car = new Models.cartaxEntities();
                var q = car.sp_ReportTypeSelect("fldID", id.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().FirstOrDefault();
                ViewBag.typeId = id;
                ViewBag.typeName = q.fldName;
                Session["typeId"] = id;
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Upload()
        {
            var file = Request.Files["Filedata"];
            string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
            file.SaveAs(savePath);
            Session["savePath"] = savePath;
            return Content(Url.Content(@"~\Uploaded\" + file.FileName));
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_ReportsSelect("fldReportTypeId", Session["typeId"].ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            Session.Remove("typeId");
            return Json(q);

        }
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldReportTypeId" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_ReportsSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_ReportsSelect("fldID", id.ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldReportPic != null)
                {
                    return File((byte[])pic.fldReportPic, "jpg");
                }
            }
            return null;

        }
        public FileContentResult FileReport(int id)
        {
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var reports = Car.sp_ReportsSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            return File((byte[])reports.fldReport, "frx");
        }
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 172))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_ReportsDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public ActionResult Save(Models.Reports Reports)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Reports.fldDesc == null)
                    Reports.fldDesc = "";
                if (Reports.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 171))
                    {
                        byte[] image_report = null;
                        byte[] report_file = null;
                        if (Session["savePath"] != null)
                        {
                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                            System.IO.File.Delete(Session["savePath"].ToString());
                            Session.Remove("savePath");
                            report_file = stream.ToArray();

                            if (Reports.fldReportPic != null)
                                image_report = Avarez.Helper.ClsCommon.Base64ToImage(Reports.fldReportPic);
                            Car.sp_ReportsInsert(Reports.fldReportTypeId, image_report, report_file,
                            Convert.ToInt32(Session["UserId"]), Reports.fldDesc, Session["UserPass"].ToString());
                            return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                        }
                        else
                            return Json(new { data = "لطفا فایل گزارش را وارد کنید.", state = 1 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 173))
                    {
                        byte[] image_report = null;
                        byte[] report_file = null;
                        if (Session["savePath"] != null)
                        {
                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                            System.IO.File.Delete(Session["savePath"].ToString());
                            Session.Remove("savePath");
                            report_file = stream.ToArray();

                            if (Reports.fldReportPic != null)
                                image_report = Avarez.Helper.ClsCommon.Base64ToImage(Reports.fldReportPic);

                            Car.sp_ReportsUpdate(Reports.fldID, Reports.fldReportTypeId, image_report,
                                report_file,
                               Convert.ToInt32(Session["UserId"]), Reports.fldDesc, Session["UserPass"].ToString());
                            return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                        }
                        else
                            return Json(new { data = "لطفا فایل گزارش را وارد کنید.", state = 1 });
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
                Models.cartaxEntities Car = new Models.cartaxEntities();
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
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_ReportsSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldID,
                    fldDesc = q.fldDesc
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
        public static string XmlUnescape(string escaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerXml = escaped;
            return node.InnerText;
        }
        public ActionResult SaveDesignedReport(string reportUUID, string reportID)
        {
            var s = System.IO.File.ReadAllText(Server.MapPath(@"\App_Data\" + reportUUID));
            s = s.Replace("Border.Color=\"0, 0, 0, 0\"", "Border.Color=\"#000000\"");
            byte[] toEncodeAsBytes = System.Text.UTF8Encoding.UTF8.GetBytes(s);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            FastReport.Report a = new FastReport.Report();
            a.ReportResourceString = returnValue;
            MemoryStream stream = new MemoryStream();
            a.Save(stream);
            Models.cartaxEntities p = new Models.cartaxEntities();
            var Report = p.sp_ReportsSelect("fldId", reportID, 0, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();

            p.sp_ReportsUpdate(Report.fldID, Report.fldReportTypeId, Report.fldReportPic, stream.ToArray(), Convert.ToInt32(Session["UserId"]), Report.fldDesc, "");
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\Uploaded\" + Report.fldID + ".frx");
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\" + reportUUID);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReportIndex(int Id)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var q = p.sp_ReportsSelect("fldId", Id.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            //string path = Server.MapPath(@"\Reports\" + q.fldName + ".frx");
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Uploaded\" + q.fldID + ".frx";
            System.IO.File.WriteAllBytes(path, q.fldReport);
            ViewBag.Path = path;
            ViewBag.RId = q.fldID.ToString();
            return View();
        }
    }
}
