using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections;
namespace Avarez.Controllers.Users
{
    [Authorize]
    public class UserController : Controller
    {
        //
        // GET: /User/
        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 264))
            {
                return PartialView();
            }  
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public JsonResult _Tree(int? id)
        {
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            //پروسیجر تشخیص کاربر در تقسیمات کشوری برای
            //لود درخت همان ناحیه ای کاربر در سطح آن تعریف شده 
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (id != null)
            {
                var rols = (from k in p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0)

                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldPID", id.ToString(), 0, 0, 0).Any()
                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var rols = (from k in p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0)

                            select new
                            {
                                id = k.fldID,
                                fldNodeType = k.fldNodeType,
                                fldSid = k.fldSourceID,
                                Name = k.fldNodeName,
                                image = url + k.fldNodeType.ToString() + ".png",
                                hasChildren = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }

        }        

        public JsonResult Position(int id)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var nodes = car.sp_SelectUpTreeCountryDivisions(id, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            ArrayList ar = new ArrayList();
            
            foreach (var item in nodes)
            {
                ar.Add(item.fldNodeName);
            }
            string nodeNames = "";
            for (int i = 0; i < ar.Count; i++)
            {
                if (i < ar.Count - 1)
                    nodeNames += ar[i].ToString() + "-->";
                else
                    nodeNames += ar[i].ToString();
            }
            
            return Json(new { Position = nodeNames });
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_UserSelect("", "", 30, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldName", "fldFamily", "fldCountryDivisionsName", "fldUserName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_UserSelect(_fiald[Convert.ToInt32(field)], searchtext, top, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_PictureSelect("fldUserPic", id.ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldPic != null)
                {
                    return File((byte[])pic.fldPic, "jpg");
                }
            }
            return null;

        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 266))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_UserDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
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

        public ActionResult Save(Models.sp_UserSelect User, string[] _checked, string fldImage)
        {
            try
            {
                
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (User.fldDesc == null)
                    User.fldDesc = "";
                bool fldStatus = true;
                
                if (User.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 265))
                    {
                        DateTime Sdate = MyLib.Shamsi.Shamsi2miladiDateTime(User.fldStartDate);
                        byte[] image = null;
                        if (fldImage != null)
                            image = Avarez.Helper.ClsCommon.Base64ToImage(fldImage);

                        System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                        Car.sp_UserInsert(id, User.fldName, User.fldFamily, User.fldStatus, User.fldPassword.GetHashCode().ToString(), User.fldUserName, User.fldMelliCode,
                            User.fldEmail, User.fldNumberAgoTel, User.fldTel, User.fldMobile, Sdate, User.CountryType, User.CountryCode,
                           Convert.ToInt32(Session["UserId"]), User.fldDesc, image, User.fldOfficeUserKey, Session["UserPass"].ToString(),false);
                        if (_checked != null)
                        {
                            for (int i = 0; i < _checked.Count(); i++)
                            {
                                Car.sp_User_GroupInsert(Convert.ToInt32(_checked[i]), Convert.ToInt64(id.Value), Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                            }
                        }
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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 267))
                    {
                        DateTime Sdate = MyLib.Shamsi.Shamsi2miladiDateTime(User.fldStartDate);
                        Car.sp_UserUpdate(User.fldID, User.fldName, User.fldFamily, User.fldStatus, "", "", User.fldMelliCode,
                            User.fldEmail, User.fldNumberAgoTel, User.fldTel, User.fldMobile, Sdate, User.CountryType, User.CountryCode,
                            Convert.ToInt32(Session["UserId"]), User.fldDesc, Avarez.Helper.ClsCommon.Base64ToImage(fldImage), User.fldOfficeUserKey, Session["UserPass"].ToString(),false);
                        Car.sp_User_GroupDelete(Convert.ToInt32(User.fldID), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        if (_checked != null)
                        {
                            for (int i = 0; i < _checked.Count(); i++)
                            {
                                Car.sp_User_GroupInsert(Convert.ToInt32(_checked[i]), User.fldID, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                            }
                        }
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
                var User = Car.sp_UserSelect("fldId", id.ToString(), 1, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var countryId = Car.sp_TableTreeSelect("fldSourceID", User.CountryCode.ToString(), 0, 0, 0).Where(h => h.fldNodeType == User.CountryType).FirstOrDefault();
                var _checked = Car.sp_User_GroupSelect("fldUserSelectID", User.fldID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                int[] checkedNodes = new int[_checked.Count];
                for (int i = 0; i < _checked.Count; i++)
                {
                    checkedNodes[i] = Convert.ToInt32(_checked[i].fldUserGroupID);
                }
                string status = "1";
                if (User.fldStatus)
                    status = "0";

                return Json(new
                {
                    fldID = User.fldID,
                    fldName = User.fldName,
                    fldFamily = User.fldFamily,
                    fldStatus = status,
                    //fldPassword=User.fldPassword,
                    //fldUserName = User.fldUserName,
                    fldMelliCode = User.fldMelliCode,
                    fldEmail = User.fldEmail,
                    fldSDate = User.fldStartDate,
                    fldNumberAgoTel = User.fldNumberAgoTel,
                    fldTel = User.fldTel,
                    fldMobile = User.fldMobile,
                    fldStartDate = User.fldStartDate,
                    fldType = User.CountryType,
                    fldCode = User.CountryCode,
                    fldCountryId = countryId.fldID,
                    fldDesc = User.fldDesc,
                    checkedNodes = checkedNodes
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
        public JsonResult ResetPass(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var User = Car.sp_UserSelect("fldId", id.ToString(), 1, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                Car.sp_UserPassUpdate(id, User.fldUserName.GetHashCode().ToString());
                return Json(new { data = "کلمه عبور به نام کاربری تغییر کرد."}, JsonRequestBehavior.AllowGet);
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
