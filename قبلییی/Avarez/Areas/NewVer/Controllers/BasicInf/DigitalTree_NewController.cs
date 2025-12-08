using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;
using System.Collections;

namespace Avarez.Areas.NewVer.Controllers.BasicInf
{
    public class DigitalTree_NewController : Controller
    {
        //
        // GET: /NewVer/DigitalTree_New/

        public ActionResult Index(string containerId)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات مالی->ساختار درختی بایگانی دیجیتال");
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
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }

        public ActionResult nodeLoadTreeArchives(string nod)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                string url = Url.Content("~/Content/images/");
                NodeCollection nodes = new Ext.Net.NodeCollection();
                var p = new Models.cartaxEntities();

                if (nod == "0" || nod == null)
                {
                    var q = p.sp_tblDigitalTreeSelect("", "", 0).ToList();

                    foreach (var item in q)
                    {
                        Node asyncNode = new Node();
                        asyncNode.Text = item.fldName;
                        asyncNode.NodeID = item.fldId.ToString();
                        //asyncNode.DataPath = item.fldNodeType.ToString();
                        //asyncNode.Cls = item.fldSourceID.ToString();
                        var child = p.sp_tblDigitalTreeSelect("PId", item.fldId.ToString(), 0).ToList();
                        foreach (var ch in child)
                        {
                            Node childNode = new Node();
                            childNode.Text = ch.fldName;
                            childNode.NodeID = ch.fldId.ToString();
                            //childNode.IconFile = url + ch.fldNodeType.ToString() + ".png";
                            //childNode.DataPath = ch.fldNodeType.ToString();
                            //childNode.Cls = ch.fldSourceID.ToString();
                            asyncNode.Children.Add(childNode);
                        }
                        nodes.Add(asyncNode);
                    }
                }
                else
                {
                    var child = p.sp_tblDigitalTreeSelect("PId", nod, 0).ToList();

                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldName;
                        childNode.NodeID = ch.fldId.ToString();
                        //childNode.IconFile = url + ch.fldNodeType.ToString() + ".png";
                        //childNode.DataPath = ch.fldNodeType.ToString();
                        //childNode.Cls = ch.fldSourceID.ToString();
                        nodes.Add(childNode);
                    }
                }
                return this.Direct(nodes);
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

        public ActionResult Save(Models.sp_tblDigitalTreeSelect Tree)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                if (Tree.fldDesc == null)
                    Tree.fldDesc = "";
                if (Tree.fldId == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 346))
                    {
                        p.sp_tblDigitalTreeInsert(Tree.fldName, Tree.fldAddable, Tree.PId, Convert.ToInt32(Session["UserId"]), Tree.fldDesc);
                        return Json(new
                        {
                            MsgTitle = "ذخیره موفق",
                            Msg = "ذخیره با موفقیت انجام شد.",
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
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 347))
                    {
                        p.sp_tblDigitalTreeUpdate(Tree.fldId, Tree.fldName, Tree.fldAddable, Tree.PId, Convert.ToInt32(Session["UserId"]), Tree.fldDesc);
                        return Json(new
                        {
                            MsgTitle = "ویرایش موفق",
                            Msg = "ویرایش با موفقیت انجام شد.",
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
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 348))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                      var p = Car.sp_tblDigitalTreeSelect("PID", id.ToString(), 1).FirstOrDefault();
                    if (p != null)
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "ابتدا زیر شاخه ها را پاک کنید.",
                            Er = 0
                        });
                        
                    }
                   
                        Car.sp_tblDigitalTreeDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                Models.cartaxEntities m = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                m.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }

       
        public ActionResult Details(int id)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.sp_tblDigitalTreeSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldName = q.fldName,
                    fldId = q.fldId,
                    fldPId = q.PId,
                    fldAddable = q.fldAddable,
                    Er=0
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
    }
}
