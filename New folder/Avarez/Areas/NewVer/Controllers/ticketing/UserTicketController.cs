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
    public class UserTicketController : Controller
    {
        //
        // GET: /UserTicket/
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        public ActionResult Index()
        {
            //باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.UserSetadId = Session["UserId"];
                return PartialView;
            }
        }

        public ActionResult GetCategory()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.prs_tblTicketCategorySelect("fldType_UserId", Session["UserId"].ToString(),"", 0).ToList().Select(n => new { ID = n.fldId, Name = n.fldTitle });
            return this.Store(q);
        }

        public ActionResult LoadChat(int Id, int CategoryId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities m = new Models.cartaxEntities();
            m.prs_tblTicketUpdateBySetadUserId(Id, CategoryId, true, "User");
            var q = m.prs_tblTicketSelect("fldSetadUserId", Id.ToString(), CategoryId.ToString(), 0).ToList();
            var q1 = m.prs_tblTicketSelect("fldSetadUser_NotSeen", Session["UserId"].ToString(), "", 0).Where(k => k.fldUserId != null).ToList();
            var q2 = m.prs_tblTicketSelect("Permmision", Session["UserId"].ToString(), "", 0).Where(k => k.fldUserId == null && k.fldSeen == false && k.fldSetadUserId != Convert.ToInt32(Session["UserId"])).ToList();

            var type = "";
            var matn = "";
            var att = "";
            var time = "";
            var seen = "";
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
                var t = 1;
                if (item.fldUserId != null)
                    t = 2;
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
                CountUser=q1.Count,
                CountAdmin = q2.Count
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
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            System.Data.Entity.Core.Objects.ObjectParameter FileId = new System.Data.Entity.Core.Objects.ObjectParameter("FileId", typeof(int));
            Models.cartaxEntities m = new Models.cartaxEntities();
            string Msg = "", MsgTitle = ""; var Er = 0;
            var s = m.sp_GetDate().FirstOrDefault();
            var d = s.DateShamsi + "-" + s.CurrentDateTime.TimeOfDay.ToString();
            string Pasvand = "";
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

                //else if (Ticket.fldFileId == 0 && Session["savePath"] == null)
                //{
                //    MsgTitle = "خطا";
                //    Msg = "لطفا یک فایل انتخاب کنید.";
                //    Er = 1;
                //    return Json(new
                //    {
                //        Msg = Msg,
                //        MsgTitle = MsgTitle,
                //        Err = Er
                //    }, JsonRequestBehavior.AllowGet);
                //}

                //ذخیره
                //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 3))
                //{
                MsgTitle = "ذخیره موفق";
                m.prs_tblTicketInsert(FileId, Ticket.fldHTML, false, report_file, Pasvand,IP,null,"", Ticket.fldTicketCategoryId, Ticket.fldSetadUserId);

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

        public FileContentResult ShowPic(int id)
        {//برگرداندن عکس 

            Models.cartaxEntities m = new Models.cartaxEntities();
            if (Session["UserId"] != null)
            {
                if (id != 0)
                {
                    int? FileId=null;
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
        public ActionResult RealodFormUser()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            SignalrHub h = new SignalrHub();
            h.Send("");
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadChatUser(int Id, int CategoryId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities m = new Models.cartaxEntities();
            m.prs_tblTicketUpdateBySetadUserId(Id, CategoryId, true, "User");
            return Json(JsonRequestBehavior.AllowGet);
        }

    }
}
