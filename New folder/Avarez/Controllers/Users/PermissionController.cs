using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.Users
{
    [Authorize]
    public class PermissionController : Controller
    {
        //
        // GET: /Permission/

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 260))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public JsonResult checkBox(int id)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q1 = car.sp_PermissionSelect("fldGroupId", id, 1, "").ToList();
            int[] checkedNodes = new int[q1.Count];
            for (int i = 0; i < q1.Count; i++)
            {
                checkedNodes[i] = Convert.ToInt32(q1[i].fldApplicationPartID);
            }

            return Json(checkedNodes);
        }

        public JsonResult GetCascadeGroup()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_UserGroupSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldTitle }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult _Rol(int? id)
        {
            var p = new Models.cartaxEntities();
            if (id != null)
            {
                var rols = (from k in p.sp_ApplicationPartSelect("fldPID", id.ToString(),0, 1, "")
                            select new
                            {
                                id = k.fldID,
                                Name = k.fldTitle,
                                hasChildren = p.sp_ApplicationPartSelect("fldPID", id.ToString(), 0, 0, "").Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var rols = (from k in p.sp_ApplicationPartSelect("", "", 0, 0, "")
                            select new
                            {
                                id = k.fldID,
                               Name = k.fldTitle,
                                hasChildren = p.sp_ApplicationPartSelect("", "", 0, 0, "").Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Save(int GroupId, List<Models.Permissions> checkedNodes)
        {
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 261))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    Car.sp_PermissionDelete(GroupId, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                    for (int i = 0; i < checkedNodes.Count(); i++)
                    {
                        Car.sp_PermissionInsert(checkedNodes[i].GroupId, checkedNodes[i].RolId, Convert.ToInt32(Session["UserId"]),
                            "", Session["UserPass"].ToString());
                    }
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
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
    }
}
