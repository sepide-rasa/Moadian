using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;

namespace Avarez.Controllers
{
    public class DigitalTreeController : Controller
    {
        //
        // GET: /DigitalTree/

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 345))
            {
                Session["field"] = "fldTreeId";
                Session["Value"] = "";
                Session["top"] = "30";
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public JsonResult _ProductTree(int? id)
        {
            try
            {
                var p = new Models.cartaxEntities();

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

        public ActionResult Save(Models.sp_tblDigitalTreeSelect Tree)
        {
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
                        return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 347))
                    {
                        p.sp_tblDigitalTreeUpdate(Tree.fldId, Tree.fldName, Tree.fldAddable, Tree.PId, Convert.ToInt32(Session["UserId"]), Tree.fldDesc);
                        return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
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
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 348))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblDigitalTreeDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "PId" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            // Models.ProductionAutomationEntities m = new Models.ProductionAutomationEntities();
            // var q = m.sp_P_tblProductPropertySelect(_fiald[Convert.ToInt32(field)], searchtext,null, top).ToList();
            //  return Json(q, JsonRequestBehavior.AllowGet);
            Session["field"] = _fiald[Convert.ToInt32(field)];
            Session["Value"] = searchtext;
            Session["top"] = top;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult Details(int id)
        {
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.sp_tblDigitalTreeSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldName = q.fldName,
                    fldId = q.fldId,
                    fldPId = q.PId,
                    fldAddable = q.fldAddable
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return null;
            }
        }
    }
}
