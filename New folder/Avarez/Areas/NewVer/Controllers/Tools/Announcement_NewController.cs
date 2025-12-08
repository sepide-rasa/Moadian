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

namespace Avarez.Areas.NewVer.Controllers.Tools
{
    public class Announcement_NewController : Controller
    {
        //
        // GET: /NewVer/Announcement_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "ابزارهای سیستم->ثبت اطلاعیه");
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
        public ActionResult New(int Id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }

        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(Models.sp_AnnouncementManagerSelect Announcement)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New");
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Announcement.fldDesc == null)
                    Announcement.fldDesc = "";
                if (Announcement.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 283))
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                        Car.sp_AnnouncementManagerInsert(id, Announcement.fldSubject, Announcement.fldMemo, Announcement.fldShowStatus,
                            Announcement.fldCountryDivisonTempID, Convert.ToInt32(Session["UserId"]),
                            Announcement.fldDesc, Session["UserPass"].ToString(), MyLib.Shamsi.Shamsi2miladiDateTime(Announcement.fldDateInsert));
                        return Json(new { Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = "ذخیره موفق", Er = 0 });
                    }
                    else
                    {
                        return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Er = 1 });
                        //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        //return RedirectToAction("error", "Metro");
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 285))
                    {
                        Car.sp_AnnouncementManagerUpdate(Announcement.fldID, Announcement.fldSubject, Announcement.fldMemo, Announcement.fldShowStatus,
                            Announcement.fldCountryDivisonTempID, Convert.ToInt32(Session["UserId"]),
                            Announcement.fldDesc, Session["UserPass"].ToString(), MyLib.Shamsi.Shamsi2miladiDateTime(Announcement.fldDateInsert));
                        return Json(new { Msg = "ویرایش با موفقیت انجام شد.", MsgTitle = "ویرایش موفق", Er = 0 });
                    }
                    else
                    {
                        return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Er = 1 });
                        //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        //return RedirectToAction("error", "Metro");
                    }
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New");
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 284))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    Car.sp_AnnouncementManagerDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                    return Json(new { Msg = "حذف با موفقیت انجام شد.", MsgTitle = "حذف موفق", Er = 0 });
                }
                else
                {
                    return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Er = 1 });
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }
        public ActionResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New");
                string Path = "/";
                int treeid = 0;
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter type1 = new System.Data.Entity.Core.Objects.ObjectParameter("Type", sizeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter code1 = new System.Data.Entity.Core.Objects.ObjectParameter("Code", sizeof(int));
                var Announcement = Car.sp_AnnouncementManagerSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var Userlog = Car.sp_UserSelect("fldId", Session["UserId"].ToString(), 1, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (Userlog.fldCountryDivisionsID != 1)
                {
                    var rec = Car.sp_GET_TypeAndCodeCountryDivisions(Userlog.fldCountryDivisionsID, type1, code1);
                    treeid = Car.sp_TableTreeSelect("fldSourceID", code1.Value.ToString(), 0, 0, 0).Where(l => l.fldNodeType == Convert.ToInt32(type1.Value)).FirstOrDefault().fldID;
                }
                var ShowStatus = "0";
                if (Announcement.fldShowStatus == true)
                    ShowStatus = "1";

                var nodes = Car.sp_SelectUpTreeCountryDivisions(Announcement.fldCountryDivisonTempID, 1, "").ToList();
                foreach (var item in nodes)
                {
                    Path = Path + item.fldID + "/";
                }
                Path = Path.Substring(0, Path.Length - 1);
                

                return Json(new
                {
                    Er = 0,
                    fldId = Announcement.fldID,
                    fldSubject = Announcement.fldSubject,
                    fldMemo = Announcement.fldMemo,
                    fldShowStatus = ShowStatus,
                    fldCountryDivisonTempID = Announcement.fldCountryDivisonTempID,
                    Path = Path,
                    treeid = treeid,
                    fldDateInsert = Announcement.fldDateInsert,
                    fldDesc = Announcement.fldDesc
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
        public ActionResult LoadPath(string Path, byte jj, string treeid)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            List<string> a = null;
            if (jj == 0)
            {
                var ind = Path.IndexOf(treeid);
                if (ind != -1)
                {
                    a = Path.Substring(ind).Split('/').ToList();
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "لطفا با پشتیبان تماس بگیرید."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            else
            {
                a = Path.Split('/').Skip(1).ToList();
            }

            NodeCollection nodes = new Ext.Net.NodeCollection();
            var node = new Ext.Net.Node();
            var node2 = new Ext.Net.Node();

            string url = Url.Content("~/Content/images/");
            int m = 0;
            for (var i = 0; i < a.Count - 1; i++)
            {
                var p = new Models.cartaxEntities();
                var child = p.sp_TableTreeSelect("fldPId", a[i].ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();
                if (i == 0)
                {
                    for (int j = 0; j < child.Count; j++)
                    {
                        Node childNode = new Node();
                        if (child[j].fldID == Convert.ToInt32(a[i + 1])) { node = childNode; }
                        childNode.Text = child[j].fldNodeName;
                        childNode.NodeID = child[j].fldID.ToString();
                        childNode.IconFile = url + child[j].fldNodeType + ".png";
                        childNode.DataPath = child[j].fldNodeType.ToString();
                        childNode.Cls = child[j].fldSourceID.ToString();
                        nodes.Add(childNode);
                    }
                }
                else
                {
                    for (int j = 0; j < child.Count; j++)
                    {
                        Node childNode = new Node();
                        if (child[j].fldID == Convert.ToInt32(a[i + 1])) { node2 = childNode; }
                        childNode.Text = child[j].fldNodeName;
                        childNode.NodeID = child[j].fldID.ToString();
                        childNode.IconFile = url + child[j].fldNodeType + ".png";
                        childNode.DataPath = child[j].fldNodeType.ToString();
                        childNode.Cls = child[j].fldSourceID.ToString();
                        node.Children.Add(childNode);
                    };
                    node = node2;
                }
            }
            return this.Direct(nodes);
        }
        public ActionResult NodeLoadTreeCountry(string nod)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            string url = Url.Content("~/Content/images/");
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var p = new Models.cartaxEntities();
           var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (nod == "0" || nod == null)
            {
                var child = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9).ToList();
                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldNodeName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.IconFile = url + ch.fldNodeType.ToString() + ".png";
                    childNode.DataPath = ch.fldNodeType.ToString();
                    childNode.Cls = ch.fldSourceID.ToString();
                    nodes.Add(childNode);
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
                    childNode.IconFile = url + ch.fldNodeType.ToString() + ".png";
                    childNode.DataPath = ch.fldNodeType.ToString();
                    childNode.Cls = ch.fldSourceID.ToString();
                    nodes.Add(childNode);
                }
            }
            return this.Direct(nodes);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_AnnouncementManagerSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_AnnouncementManagerSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldCountryDivisonTempName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCountryDivisonTempName";
                            break;
                        case "fldDateInsert":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDateInsert";
                            break;
                        case "fldSubject":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSubject";
                            break;
                        case "fldShowStatusName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShowStatusName";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_AnnouncementManagerSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_AnnouncementManagerSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_AnnouncementManagerSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_AnnouncementManagerSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult AnnouncementAttach(int Id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.fldAnnouncementID = Id;
           // Session["fldAnnouncementID"] = Id;
            return PartialView;
        }
        public ActionResult Read_AnnouncementAttach(StoreRequestParameters parameters, string fldAnnouncementID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_AnnouncementManagerAttachmentSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_AnnouncementManagerAttachmentSelect> data1 = null;
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

                        data1 = m.sp_AnnouncementManagerAttachmentSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_AnnouncementManagerAttachmentSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_AnnouncementManagerAttachmentSelect("fldAnnouncementID", fldAnnouncementID, 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_AnnouncementManagerAttachmentSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
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
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    System.IO.File.Delete(physicalPath);
                }

                //if (Request.Files[0].ContentType == "image/jpeg" || Request.Files[0].ContentType == "image/png")
                //{
                if (Request.Files[0].ContentLength < 204800)
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
                        Message = "حجم فایل انتخابی می بایست کمتر از 200 کیلوبایت باشد."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
                //}
                //else
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.ERROR,
                //        Title = "خطا",
                //        Message = "فایل انتخاب شده غیر مجاز است."
                //    });
                //    DirectResult result = new DirectResult();
                //    return result;
                //}
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
        public JsonResult SaveAttach(Models.sp_AnnouncementManagerAttachmentSelect Announcement)
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
                    return Json(new { Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = "ذخیره موفق", Er = 0 });
                }
                else
                    return Json(new { Msg = "لطفا فایل ضمیمه را وارد کنید.", MsgTitle = "خطا", Er = 1 });

            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "", Er = 1 });
            }
        }
        public JsonResult DeleteAttach(string id)
        {//حذف یک رکورد
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                Car.sp_AnnouncementManagerAttachmentDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                return Json(new { Msg = "حذف با موفقیت انجام شد.", MsgTitle = "حذف موفق", Er = 0 });

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
        public FileContentResult Download(int Id)
        {
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var q = Car.sp_AnnouncementManagerAttachmentSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            if (q != null && q.fldAttachment != null)
            {
                string Pasvand = Path.GetExtension(q.fldFileName.ToString());
                MemoryStream st = new MemoryStream(q.fldAttachment);
                return File(st.ToArray(), MimeType.Get(Pasvand), "DownloadFile" + Pasvand);
            }
            return null;
        }
    }
}
