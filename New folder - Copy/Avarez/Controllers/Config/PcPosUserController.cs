using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.Config
{
    public class PcPosUserController : Controller
    {
        //
        // GET: /PcPosUser/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 331))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public JsonResult GetPcPosInfo()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_PcPosInfoSelect("", "",0,0, 0).Select(c => new { fldID = c.fldId, fldName = c.fldBankName + "(" + c.fldCountryDivisionName + ")" }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_PcPosIPSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.sp_PcPosIPSelect Pos,string UserName)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Pos.fldDesc == null)
                    Pos.fldDesc = "";
                var UserId = UserName.Split(';');
                if (Pos.fldId == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 332))
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                        Car.sp_PcPosIPInsert(_id, Pos.fldPcPosId, Pos.fldIP, Pos.fldSerialNum,Convert.ToInt32(Session["UserId"]), Pos.fldDesc);
                        for (int i = 0; i < UserId.Length-1; i++)
                            Car.sp_PcPosUserInsert(Convert.ToInt32(_id.Value), Convert.ToInt64(UserId[i]), Convert.ToInt32(Session["UserId"]), "");   

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 333))
                    {
                        Car.sp_PcPosIPUpdate(Pos.fldId, Pos.fldPcPosId, Pos.fldIP,Pos.fldSerialNum, Convert.ToInt32(Session["UserId"]), Pos.fldDesc);
                        Car.sp_PcPosUserPosIPIdDelete(Pos.fldId, Convert.ToInt32(Session["UserId"]));
                        for (int i = 0; i < UserId.Length-1; i++)
                            Car.sp_PcPosUserInsert(Pos.fldId, Convert.ToInt64(UserId[i]), Convert.ToInt32(Session["UserId"]), "");  
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
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldCountryDivisionName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_PcPosIPSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 334))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_PcPosIPDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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

        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var Pos = Car.sp_PcPosIPSelect("fldId", id.ToString(), 1).FirstOrDefault();
                var PosUser = Car.sp_PcPosUserSelect("fldPosIPId", Pos.fldId.ToString(), 0).ToList();
                var UserName = "";
                var UserId = "";
                foreach (var item in PosUser)
                {
                    UserName = UserName + item.fldNameFamilyUser + ";";
                    UserId = UserId + item.fldIdUser + ";";
                }
                return Json(new
                {
                    fldId = Pos.fldId,
                    fldSerialNum = Pos.fldSerialNum,
                    UserName=UserName,
                    UserId=UserId,
                    fldIP=Pos.fldIP,
                    fldPcPosId = Pos.fldPcPosId
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

    }
}
