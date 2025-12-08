using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;
using System.IO;
using System.Xml;

namespace Avarez.Areas.NewVer.Controllers.Config
{
    public class ReportType_NewController : Controller
    {
        //
        // GET: /NewVer/ReportType_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "پیکربندی سیستم->سفارشی سازی گزارشات");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }


        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_ReportTypeSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_ReportTypeSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldID";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_ReportTypeSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_ReportTypeSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_ReportTypeSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_ReportTypeSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult New(int ReportId)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ReportId = ReportId;
            return PartialView;
        }

        public ActionResult NewReport(int Id, int ReportId)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            PartialView.ViewBag.ReportId = ReportId;
            return PartialView;
        }
        public ActionResult Preview(int ReportId)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ReportId = ReportId;
            return PartialView;
        }
        public ActionResult SelectCountryDivision(int ReportId)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ReportId = ReportId;
            return PartialView;
        }
        public FileContentResult showFile(string dc,int Id)
        {//برگرداندن عکس 

            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_ReportsSelect("fldID", Id.ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldReportPic != null)
                {
                    return File((byte[])pic.fldReportPic, ".jpg");
                }
            }
            return null;
        }
        public FileContentResult Download(int ReportId)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var reports = p.sp_ReportsSelect("fldId", ReportId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (reports != null)
            {
                MemoryStream st = new MemoryStream(reports.fldReport);
                return File(st.ToArray(), MimeType.Get(".frx"), "DownloadFile.frx");
            }
            return null;
        }
        public ActionResult ReadReports(StoreRequestParameters parameters, string ReportId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_ReportsSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_ReportsSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldID";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_ReportsSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_ReportsSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_ReportsSelect("fldReportTypeId", ReportId, 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_ReportsSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
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
        public ActionResult ShowPic(string dc)
        {//برگرداندن عکس 
            byte[] file = null;
            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
            file = stream.ToArray();
            var image = Convert.ToBase64String(file);
            return Json(new { image = image });
        }
        public ActionResult Upload()
        {
            string Msg = "";
            try
            {
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    Session.Remove("FileName");
                    System.IO.File.Delete(physicalPath);
                }

                if (Request.Files[0].ContentType == "image/jpeg" || Request.Files[0].ContentType == "image/png")
                {
                    if (Request.Files[0].ContentLength <= 102400)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                        file.SaveAs(savePath);
                        Session["FileName"] = file.FileName;
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
                            Message = "حجم فایل انتخابی می بایست کمتر از 100 کیلوبایت باشد."
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
                        Message = "فایل انتخاب شده باید پسوند jpeg یا png باشد."
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
        public ActionResult Save(Models.Reports Reports)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            string Msg = "",
            MsgTitle = "";
            var Er = 0;
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Reports.fldDesc == null)
                    Reports.fldDesc = "";
               
                byte[] file = null; string FileName = "";
                byte[] Rptfile = null; string RptFileName = "";
                if (Reports.fldID == 0)
                {
                    if (Session["savePath"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                        file = stream.ToArray();
                        FileName = Session["FileName"].ToString();
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "لطفا تصویر گزارش را انتخاب نمایید.",
                            MsgTitle = "خطا",
                            Er = 1,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (Session["savePathFile"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathFile"].ToString()));
                        Rptfile = stream.ToArray();
                        RptFileName = Session["FileNameFile"].ToString();
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "لطفا فایل گزارش را انتخاب نمایید.",
                            MsgTitle = "خطا",
                            Er = 1,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 171))
                    {
                        Car.sp_ReportsInsert(Reports.fldReportTypeId, file, Rptfile,
                            Convert.ToInt32(Session["UserId"]), Reports.fldDesc, Session["UserPass"].ToString());
                        MsgTitle = "ذخیره موفق";
                        Msg = "ذخیره با موفقیت انجام شد.";
                    }
                    else
                    {
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به دسترسی نمی باشید.";
                        Er = 1;
                    }
                }
                else
                {
                    if (Session["savePath"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                        file = stream.ToArray();
                        FileName = Session["FileName"].ToString();
                    }
                    else
                    {
                        var pic = Car.sp_ReportsSelect("fldId", Reports.fldID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        file = pic.fldReportPic;
                    }
                    if (Session["savePathFile"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathFile"].ToString()));
                        Rptfile = stream.ToArray();
                        RptFileName = Session["FileNameFile"].ToString();
                    }
                    else
                    {
                        var pic = Car.sp_ReportsSelect("fldId", Reports.fldID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        Rptfile = pic.fldReport;
                    }
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 173))
                    {
                        Car.sp_ReportsUpdate(Reports.fldID, Reports.fldReportTypeId, file, Rptfile,
                                Convert.ToInt32(Session["UserId"]), Reports.fldDesc, Session["UserPass"].ToString());
                        MsgTitle = "ویرایش موفق";
                        Msg = "ویرایش با موفقیت انجام شد.";
                    }
                    else
                    {
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به دسترسی نمی باشید.";
                        Er = 1;
                    }
                }
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    Session.Remove("FileName");
                    System.IO.File.Delete(physicalPath);
                    string physicalPath2 = System.IO.Path.Combine(Session["savePathFile"].ToString());
                    Session.Remove("savePathFile");
                    Session.Remove("FileNameFile");
                    System.IO.File.Delete(physicalPath2);
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
                MsgTitle = "خطا";
                Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.";
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int Id)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            try
            {
                var Image = "";
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_ReportsSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                Image = Convert.ToBase64String(q.fldReportPic);
                return Json(new
                {
                    fldId = q.fldID,
                    fldDesc = q.fldDesc,
                    Image = Image
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
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }
        public ActionResult Delete(int Id)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 172))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    Car.sp_ReportsDelete(Convert.ToInt32(Id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                    return Json(new
                    {
                        MsgTitle = "حذف موفق",
                        Msg = "حذف با موفقیت انجام شد.",
                        Er = 0
                    });
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    });
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
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }
        public FileContentResult DownloadFile(int Id)
        {
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var q = Car.sp_ReportsSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
           if (q != null)
            {
                MemoryStream st = new MemoryStream(q.fldReport);
                return File(st.ToArray(), MimeType.Get(".frx"), "ReportFile.frx");
            }
            return null;
        }
        public ActionResult UploadFileNew(string docId)
        {
            if (Session["savePathFile"] != null)
            {
                string physicalPath = System.IO.Path.Combine(Session["savePathFile"].ToString());
                Session.Remove("savePathFile");
                Session.Remove("FileNameFile");
                System.IO.File.Delete(physicalPath);
            }
            var e = System.IO.Path.GetExtension(Request.Files[1].FileName);

            if (e.ToLower() == ".frx")
            {
                if (Request.Files[0].ContentLength <= 104857600)
                {
                    HttpPostedFileBase file = Request.Files[1];
                    string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                    file.SaveAs(savePath);
                    Session["FileNameFile"] = file.FileName;
                    Session["savePathFile"] = savePath;
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
                        Message = "حجم فایل انتخابی می بایست کمتر از 100 مگابایت باشد."
                    });
                    DirectResult result = new DirectResult();
                    result.IsUpload = true;
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
                    Message = "فایل انتخاب شده باید با پسوند frx باشد."
                });
                DirectResult result = new DirectResult();
                result.IsUpload = true;
                return result;
            }
        }
        public ActionResult NodeLoadTreeStructure(string nod)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            NodeCollection nodes = new Ext.Net.NodeCollection();
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (nod == "0" || nod == null)
            {
                var q = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldNodeName;
                    asyncNode.NodeID = item.fldID.ToString();
                    asyncNode.DataPath = item.fldNodeType.ToString();
                    asyncNode.Cls = item.fldSourceID.ToString();
                    asyncNode.IconFile = url + item.fldNodeType + ".png";

                    var child = p.sp_TableTreeSelect("fldPId", item.fldID.ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldNodeName;
                        childNode.NodeID = ch.fldID.ToString();
                        childNode.IconFile = url + ch.fldNodeType + ".png";
                        childNode.DataPath = ch.fldNodeType.ToString();
                        childNode.Cls = ch.fldSourceID.ToString();
                        asyncNode.Children.Add(childNode);
                    }
                    nodes.Add(asyncNode);
                }
            }
            else
            {
            var child = p.sp_TableTreeSelect("fldPId", nod, 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldNodeName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.IconFile = url + ch.fldNodeType + ".png";
                    childNode.DataPath = ch.fldNodeType.ToString();
                    childNode.Cls = ch.fldSourceID.ToString();
                    nodes.Add(childNode);
                }
            }
            return this.Direct(nodes);
        }
        public ActionResult DeleteDivision(string id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 175))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                        Car.sp_Report_CountryDivitionDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        return Json(new
                        {
                            MsgTitle = "حذف موفق",
                            Msg = "حذف با موفقیت انجام شد.",
                            Er = 0
                        });
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    });
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
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }
        public ActionResult ReadSelectCountryDivision(StoreRequestParameters parameters, string ReportId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_Report_CountryDivitionSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_Report_CountryDivitionSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldID";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_Report_CountryDivitionSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_Report_CountryDivitionSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_Report_CountryDivitionSelect("fldReportsID", ReportId, 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_Report_CountryDivitionSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveDiv(Models.ReportDiv report)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (report.fldDesc == null)
                    report.fldDesc = "";
                    //ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 174))
                    {
                        var IdCountryDivisions = Car.sp_GET_IDCountryDivisions(Convert.ToInt32(report.fldTypeCountryDivisions), Convert.ToInt32(report.fldCodeCountryDivisions)).FirstOrDefault().CountryDivisionId;
                       // var LastNodeId = Car.sp_TableTreeSelect("fldSourceID", report.fldCodeCountryDivisions.ToString(), 1, 0, 0).Where(l => l.fldNodeType == report.fldTypeCountryDivisions).FirstOrDefault().fldID;
                        var q = Car.sp_Report_CountryDivitionSelect("fldCountryDivisionsID", IdCountryDivisions.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(l => l.fldReportsID == report.fldReportsID).FirstOrDefault();
                        if (q == null)
                        {
                            Car.sp_Report_CountryDivitionInsert(report.fldTypeCountryDivisions, report.fldCodeCountryDivisions,
                            report.fldReportsID, Convert.ToInt32(Session["UserId"]), report.fldDesc,
                            Session["UserPass"].ToString());
                            return Json(new
                            {
                                MsgTitle = "ذخیره موفق",
                                Msg = "ذخیره با موفقیت انجام شد.",
                                Er = 0
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                MsgTitle = "خطا",
                                Msg = "اطلاعات وارد شده تکراری است.",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
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
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
