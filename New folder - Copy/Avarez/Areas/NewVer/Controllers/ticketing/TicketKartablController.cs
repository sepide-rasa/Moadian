using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Models;
using System.IO;

namespace Avarez.Areas.NewVer.Controllers.Ticketing
{
    public class TicketKartablController : Controller
    {
        //
        // GET: /TicketKartabl/
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" }); 
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult New(int SetadUserId, string Type)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.SetadUserId = SetadUserId;
            PartialView.ViewBag.Type = Type;                           
            return PartialView;
        }
        public ActionResult GetCategory(string TypeMsg)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.prs_tblTicketCategorySelect("fldType", TypeMsg,Session["UserId"].ToString(), 0).ToList().Select(n => new { ID = n.fldId, Name = n.fldTitle });
            return this.Store(q);
        }
        public ActionResult LoadChat(int Id, int CategoryId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities m = new Models.cartaxEntities();

            var FieldName = "Admin";
            //if (TypePer == "1")
            //    FieldName = "Admin_Comite";

            var q = m.prs_tblTicketSelect("fldSetadUserId", Id.ToString(), CategoryId.ToString(), 0).ToList();
            var q1 = m.prs_tblTicketSelect("fldSetadUser_NotSeen", Session["UserId"].ToString(), "", 0).Where(k => k.fldUserId != null).ToList();
            var q2 = m.prs_tblTicketSelect("Permmision", Session["UserId"].ToString(), "", 0).Where(k => k.fldUserId == null && k.fldSeen == false && k.fldSetadUserId != Convert.ToInt32(Session["UserId"])).ToList();
            var type = "";
            var matn = "";
            var att = "";
            var time = "";
            var seen = "";

            bool? ReplyPermission = true;
            bool? ReadPermission = true;
           
                ReadPermission = m.prs_CheckTicketPermission("See", Convert.ToInt32(Session["UserId"]), CategoryId).FirstOrDefault().fldPermission;
                ReplyPermission = m.prs_CheckTicketPermission("Answer", Convert.ToInt32(Session["UserId"]), CategoryId).FirstOrDefault().fldPermission;
            
            if(ReadPermission==true)
                m.prs_tblTicketUpdateBySetadUserId(Id, CategoryId,true, FieldName);

            foreach (var item in q)
            {
                int? haveAtt = 0;
                //var q2 = servic.GetTicketFileWithFilter("fldTicketId", Item.fldId.ToString(), 0, Session["Username"].ToString(), (Session["Password"].ToString()), out Err);
                if (item.fldFileId != null)
                    haveAtt = item.fldFileId;
                if (item.fldSeen)
                    seen = seen + "1" + "|";
                else
                    seen = seen + "0" + "|";
                var t = 2;
                if (item.fldUserId != null)
                    t = 1;
                type = type + t + "|";
                matn = matn + item.fldHTML + "|";
                att = att + haveAtt + "|";
                time = time + item.fldTarikh + "|";
            }
            return Json(new
            {
                type = type,
                matn = matn,
                att = att,
                time = time,
                seen = seen,
                ReplyPermission = ReplyPermission,
                ReadPermission = ReadPermission,
                CountUser=q1.Count,
                CountAdmin = q2.Count,
                Err = 0
            }, JsonRequestBehavior.AllowGet);
        }
        public FileContentResult DownloadAttach(int Id)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var f = m.Sp_FilesSelect(Id).FirstOrDefault();
            if (f != null)
            {
                MemoryStream st = new MemoryStream(f.fldImage);
                return File(st.ToArray(), MimeType.Get(System.IO.Path.GetExtension("jpg")), "File.jpg");
            }
            return null;
        }
        public ActionResult Read(StoreRequestParameters parameters, string TypeMsg)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            var fieldName = "Setadi";
            if (TypeMsg == "0")
                fieldName = "Navahi";
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Models.prs_tblTicketSelect> data = null;            
            data = m.prs_tblTicketSelect(fieldName, Session["UserId"].ToString(), "", 0).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Models.prs_tblTicketSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Upload()
        {

            if (Session["savePath"] != null)
            {
                string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                Session.Remove("savePath");
                Session.Remove("FileName");
                Session.Remove("Pasvand");
                System.IO.File.Delete(physicalPath);
            }
            HttpPostedFileBase file = Request.Files[0];
            string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
            string e = Path.GetExtension(savePath);
            //file.FileName.Substring(file.FileName.IndexOf('.'));
            //if ((file.ContentType) == "application/pdf" || e == ".ppsx" || e == ".xlsx" || e == ".pptx" || e == ".docx")
            //{

            file.SaveAs(savePath);
            Session["FileName"] = file.FileName;
            Session["savePath"] = savePath;
            Session["Pasvand"] = file.FileName.Split('.').Last();
            object r = new
            {
                success = true,
                name = Request.Files[0].FileName
            };
            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
            //}
            //else
            //{
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.INFO,
            //        Title = "خطا",
            //        Message = "فایل غیرمجاز"
            //    });
            //    DirectResult result = new DirectResult();
            //    result.IsUpload = true;
            //    return result;
            //}

        }
        public ActionResult Save(Avarez.Models.prs_tblTicketSelect Ticket, bool? HaveFile)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            string Msg = "ارسال پیام با موفقیت انجام شد.", MsgTitle = ""; var Er = 0;
            var s = m.sp_GetDate().FirstOrDefault();
            var d = s.DateShamsi + "-" + s.CurrentDateTime.TimeOfDay.ToString();
            string Pasvand = "";
            System.Data.Entity.Core.Objects.ObjectParameter FileId = new System.Data.Entity.Core.Objects.ObjectParameter("FileId", typeof(int));
            try
            {
                byte[] report_file = null;
                if (HaveFile == true && Session["savePath"] != null)
                {
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                    report_file = stream.ToArray();
                    Pasvand = Session["Pasvand"].ToString();
                }
                if (Ticket.fldHTML == null)
                    Ticket.fldHTML = "";

                //ذخیره
                //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 3))
                //{
                //var haveSmsPanel = servic.GetSMSSettingWithFilter("", "", 1, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).FirstOrDefault();
                //RaiSms.Service w = new RaiSms.Service();
                //var Matn = "به تیکت شما پاسخ داده شد.";
                //var r = servic.GetRequestRankingWithFilter("fldId", Ticket.fldRequestId.ToString(), 0, out Err).FirstOrDefault();

                //if (Ticket.fldRequestId != null)
                //{
                //    var q = servic.GetFirstRegisterWithFilter("fldId", r.fldFirstRegisterId.ToString(), "", 1, out Err).FirstOrDefault();
                //    var k = servic.GetTicketWithFilter("fldRequestId", Ticket.fldRequestId.ToString(), Ticket.fldTicketCategoryId.ToString(), 0, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).OrderByDescending(l => l.fldId).FirstOrDefault();
                //    if (k.fldUserId == null)
                //    {
                //        var returnCode = w.SendSms(haveSmsPanel.fldUserName, haveSmsPanel.fldPassword, Matn, q.fldMobile, null, 1, 2, null, "RailWay", null, 0, 0, "", "");

                //        if (returnCode.Length < 3)
                //        {
                //            MsgTitle = "خطا";
                //            Msg = w.ShowError(returnCode, "FA");
                //            Er = 1;
                //        }
                //    }
                //}
                MsgTitle = "ذخیره موفق";
                m.prs_tblTicketInsert(FileId, Ticket.fldHTML, false, report_file, Pasvand, IP, Convert.ToInt32(Session["UserId"]),"", Ticket.fldTicketCategoryId, Ticket.fldSetadUserId);

                //}
                //else
                //{
                //    return null;
                //}
            }
            catch (Exception x)
            {
                System.Data.Entity.Core.Objects.ObjectParameter ErrorId = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                else
                    InnerException = x.Message;
                m.sp_ErrorProgramInsert(ErrorId, InnerException, Convert.ToInt32(Session["UserId"]), "", DateTime.Now, "");
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + ErrorId.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er,
                time = d,
                FileId = FileId.Value
            }, JsonRequestBehavior.AllowGet);
        }
        public FileContentResult ShowPic(int id, string Type)
        {//برگرداندن عکس 
            if (Session["Username"] != null)
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                int? FileId = null;
                if(id!=0)
                {
                    var pic = m.sp_PictureSelect("fldUserPic", id.ToString(), 0, 1, "").FirstOrDefault();
                    if (pic == null)
                    {
                        var Image = Server.MapPath("~/Content/Blank.jpg");
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
                        return File(stream.ToArray(), "jpg");
                    }
                    else
                    {
                        if (pic.fldPic != null)
                        {
                            //return File(PDF(q.fldPic),".pdf");
                            return File((byte[])pic.fldPic, "jpg");
                        }
                    }
                }
                else
                {
                    var pic = m.sp_PictureSelect("fldUserPic", id.ToString(), 0, 1, "").FirstOrDefault();
                    if (pic == null)
                    {
                        var Image = Server.MapPath("~/Content/support.png");
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
                        return File(stream.ToArray(), "jpg");
                    }
                    else
                    {
                        if (pic.fldPic != null)
                        {
                            //return File(PDF(q.fldPic),".pdf");
                            return File((byte[])pic.fldPic, "jpg");
                        }
                    }
                }
            }
            return null;
        }
        public ActionResult LoadChatAdmin(int Id, int CategoryId, string PerType)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities m = new Models.cartaxEntities();
            //var FieldName = "Admin";
            //if (PerType == "1")
            //    FieldName = "User";
            bool? ReadPermission = true;

            ReadPermission = m.prs_CheckTicketPermission("See", Convert.ToInt32(Session["UserId"]), CategoryId).FirstOrDefault().fldPermission;
            if (ReadPermission == true)
                m.prs_tblTicketUpdateBySetadUserId(Id, CategoryId,true, "Admin");
            return Json( JsonRequestBehavior.AllowGet);
        }
        //public ActionResult LoadChatAdminComite(int Id, int CategoryId)
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("Login", "Admin", new { area = "faces" });
        //    servic.TicketUpdateByRequestId(Id, CategoryId, true, "Admin_Comite", Session["Username"].ToString(), Session["Password"].ToString(), out Err);
        //    return Json(new
        //    {
        //        Err = Err.ErrorMsg
        //    }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult RealodFormAdmin()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            SignalrHub h = new SignalrHub();
            h.Send("");//صدا زدن Clients.All.broadcastMessage
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCountPM()
        {
            if (Session["UserId"] != null)
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                var q = m.prs_tblTicketSelect("fldSetadUser_NotSeen", Session["UserId"].ToString(), "", 0).ToList();//.Where(k => k.fldUserId != null).ToList();
                var q2 = m.prs_tblTicketSelect("Permmision", Session["UserId"].ToString(), "", 0).ToList();//.Where(k => k.fldUserId == null && k.fldSeen == false && k.fldSetadUserId != Convert.ToInt32(Session["UserId"])).ToList();
                return Json(new
                {
                    CountUser = q.Count,
                    CountAdmin = q2.Count
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    CountUser = 0,
                    CountAdmin = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CheckHavePm()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            //Models.RaiKMEntities m = new Models.RaiKMEntities();
            //Boolean Have = false;
            //string txt = "";
            //var q = m.prs_tblTicketSelect("fldSetadUser_NotSeen", Session["UserId"].ToString(), "", 0).Where(k => k.fldUserId != null).ToList();
            //if (q.Count() != 0)
            //{
            //    Have = true;
            //    txt = "<li>" + q.Count() + " پیام خوانده نشده" + "</li>";
            //}

            //if (RaiKM.Models.Permission.haveAccess(Convert.ToInt32(Session["UserId"]),53))
            //{

            //    //var q2 = m.prs_tblTicketSelect("", "", "", 0).Where(k => k.fldUserId == null && k.fldSeen == false).ToList();
            //    var q2 = m.prs_tblTicketSelect("Permmision", Session["UserId"].ToString(), "", 0).Where(k => k.fldUserId == null && k.fldSeen == false && k.fldSetadUserId != Convert.ToInt32(Session["UserId"])).ToList();
            //    var cc = 0;
            //    foreach (var item in q2)
            //    {
            //        var q3 = m.prs_tblTicketPermissionSelect("fldCategoryId_User", item.fldTicketCategoryId.ToString(), Session["UserId"].ToString(), 0).Where(l => l.fldSee || l.fldAnswer).ToList();
            //        if (q3.Count != 0)
            //            cc++;
            //    }
            //    if (cc != 0)
            //    {
            //        Have = true;
            //        txt = txt + "<li>" + q2.Count() + " پیام خوانده نشده در کارتابل" + "</li>";
            //    }

            //    //var q2 = m.prs_tblTicketSelect("", "", "", 0).Where(k => k.fldUserId == null && k.fldSeen == false).ToList();
            //    //if (q2.Count() != 0)
            //    //{
            //    //    Have = true;
            //    //    txt = txt + "<li>" + q2.Count() + " پیام خوانده نشده در کارتابل" + "</li>";
            //    //}
            //}

            int UserLoginCount = 0;// RaiKM.UserLoginCount.userObj.Count();

            return Json(new
            {
                //Have = Have,
                //txt = txt,
                UserOnline = UserLoginCount
            }, JsonRequestBehavior.AllowGet);

        }
    }
}
