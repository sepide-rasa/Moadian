using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;

namespace Avarez.Areas.NewVer.Controllers.Users
{
    public class Permission_NewController : Controller
    {
        //
        // GET: /NewVer/Permission_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "مدیریت کاربران->سطوح دسترسی");
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
        public ActionResult GetUserGroup()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_UserGroupSelect("ByUserId", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldTitle }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReloadDrp()
        {//نمایش اطلاعات جهت رویت کاربر
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q = car.sp_UserGroupSelect("ByUserId", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            return Json(new
            {
                fldUserGroupId = q.fldID.ToString(),

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SavePermission(List<Models.Permissions> Permission, int UserGroupID)
        {
            string Msg = "", MsgTitle = "";
            var Er = 0;
            try
            {
                /*if (c.Permission(22, Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                {*/
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_PermissionSelect("fldGroupId", UserGroupID,Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                if (q.Count() != 0)
                {
                    Car.sp_PermissionDelete(UserGroupID, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                }

                if (Permission != null)
                {
                    for (int i = 0; i < Permission.Count(); i++)
                    {
                        Car.sp_PermissionInsert(Permission[i].GroupId, Permission[i].RolId, Convert.ToInt32(Session["UserId"]),
                            "", Session["UserPass"].ToString());
                    }
                }
                MsgTitle = "ذخیره موفق";
                Msg = "ذخیره با موفقیت انجام شد.";
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
        public ActionResult NodeLoad(string node, string GrohKarbari)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var child = p.sp_ApplicationPartSelect("fldPId", node, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();

            foreach (var ch in child)
            {
                Node childNode = new Node();
                childNode.Text = ch.fldTitle;
                childNode.NodeID = ch.fldID.ToString();
                childNode.Icon = Ext.Net.Icon.Building;
                childNode.Checked = false;
                var f = p.sp_PermissionSelect("fldGroupId", Convert.ToInt32(GrohKarbari), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                if (f.Count() != 0)
                {
                    foreach (var _item in f)
                    {
                        if (_item.fldApplicationPartID == ch.fldID)
                        {
                            childNode.Checked = true;
                        }
                    }
                }
                nodes.Add(childNode);
            }
            return this.Direct(nodes);
        }
    }
}
