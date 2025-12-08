using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using System.IO;
using Kendo.Mvc.Extensions;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data.Entity;
using Avarez.Controllers.Users;


namespace Avarez.Controllers.Tools
{
    public class NameTableController : Controller
    {

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 281))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }


        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_NameTablesSelect("","",0).ToList().ToDataSourceResult(request);
            return Json(q);
        }


        public ActionResult FileExport(int id,string start,string end)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            GridView gv = new GridView();
            if (id == 1)
            {
                gv.DataSource = car.sp_State_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();

            }
            else if (id == 2)
            {
                gv.DataSource = car.sp_County_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 3)
            {
                gv.DataSource = car.sp_Zone_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 4)
            {
                //gv.DataSource = car.sp_City_LogSelect(start,
                //    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 5)
            {
                gv.DataSource = car.sp_Municipality_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 6)
            {
                gv.DataSource = car.sp_Local_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 7)
            {
                gv.DataSource = car.sp_Area_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 8)
            {
                gv.DataSource = car.sp_OfficesType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 9)
            {
                gv.DataSource = car.sp_Offices_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 10)
            {
                gv.DataSource = car.sp_DegreeMunicipality_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 11)
            {
                gv.DataSource = car.sp_PlaqueCity_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 12)
            {
                gv.DataSource = car.sp_PlaqueSerial_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 13)
            {
                gv.DataSource = car.sp_StatusPlaque_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 14)
            {
                gv.DataSource = car.sp_ColorCar_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 15)
            {
                gv.DataSource = car.sp_Bank_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }

            else if (id == 16)
            {
                gv.DataSource = car.sp_BankType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();

            }
            else if (id == 17)
            {
                gv.DataSource = car.sp_BankBranch_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 18)
            {
                gv.DataSource = car.sp_AccountBank_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 19)
            {
                gv.DataSource = car.sp_User_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 20)
            {
                gv.DataSource = car.sp_CarMake_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 21)
            {
                gv.DataSource = car.sp_CarAccountType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 22)
            {
                gv.DataSource = car.sp_CabinType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 23)
            {
                gv.DataSource = car.sp_CarSystem_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 24)
            {
                gv.DataSource = car.sp_CarPatternModel_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 25)
            {
                gv.DataSource = car.sp_CarModel_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 26)
            {
                gv.DataSource = car.sp_CarClass_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 27)
            {
                gv.DataSource = car.sp_FuelType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 28)
            {
                gv.DataSource = car.sp_CharacterPersianPlaque_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 29)
            {
                gv.DataSource = car.sp_Cost_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 30)
            {
                gv.DataSource = car.sp_AmountCost_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }

            else if (id == 31)
            {
                gv.DataSource = car.sp_Contact_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();

            }
            else if (id == 32)
            {
                gv.DataSource = car.sp_AccostType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 33)
            {
                gv.DataSource = car.sp_MainSetting_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 34)
            {
                gv.DataSource = car.sp_SubSetting_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 35)
            {
                gv.DataSource = car.sp_SendLetter_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 36)
            {
                gv.DataSource = car.sp_PlaqueType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 37)
            {
                gv.DataSource = car.sp_Organization_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 38)
            {
                gv.DataSource = car.sp_CarPlaque_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 39)
            {
                gv.DataSource = car.sp_Owner_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 40)
            {
                gv.DataSource = car.sp_Car_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 41)
            {
                gv.DataSource = car.sp_CarFile_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 42)
            {
                gv.DataSource = car.sp_Post_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 43)
            {
                gv.DataSource = car.sp_SignerEmployee_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 44)
            {
                gv.DataSource = car.sp_CarExperience_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 45)
            {
                gv.DataSource = car.sp_Peacockery_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 46)
            {
                gv.DataSource = car.sp_Collection_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 47)
            {
                gv.DataSource = car.sp_OftenInvolved_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 48)
            {
                gv.DataSource = car.sp_ComplicationsRate_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 49)
            {
                //gv.DataSource = car.sp_FinesRule_LogSelect(start,
                //    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 50)
            {
                gv.DataSource = car.sp_ImplementationFinesRule_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 51)
            {
                gv.DataSource = car.sp_SettleType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 52)
            {
                gv.DataSource = car.sp_Discount_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 53)
            {
                gv.DataSource = car.sp_News_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 54)
            {
                gv.DataSource = car.sp_OnlinePayments_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 55)
            {
                gv.DataSource = car.sp_BankInformation_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 56)
            {
                gv.DataSource = car.sp_Reports_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 57)
            {
                gv.DataSource = car.sp_SystemReport_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 58)
            {
                gv.DataSource = car.sp_ApplicationPart_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 59)
            {
                gv.DataSource = car.sp_ShortTermCountry_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 60)
            {
                gv.DataSource = car.sp_UserGroup_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 61)
            {
                gv.DataSource = car.sp_User_Group_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 62)
            {
                gv.DataSource = car.sp_Permission_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 63)
            {
                gv.DataSource = car.sp_Round_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 64)
            {
                gv.DataSource = car.sp_Friends_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=پیش نمایش.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View();
        }


        //public ActionResult Reload(string field, string value, int top, int searchtype)
        //{//جستجو
        //    string[] _fiald = new string[] { "fldName" };
        //    string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
        //    string searchtext = string.Format(searchType[searchtype], value);
        //    Models.cartaxEntities m = new Models.cartaxEntities();
        //    var q = m.sp_BankSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
        //    return Json(q, JsonRequestBehavior.AllowGet);
        //}

        //public FileContentResult Image(int id)
        //{//برگرداندن عکس 
        //    Models.cartaxEntities p = new Models.cartaxEntities();
        //    var pic = p.sp_PictureSelect("fldBankPic", id.ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
        //    if (pic != null)
        //    {
        //        if (pic.fldPic != null)
        //        {
        //            return File((byte[])pic.fldPic, "jpg");
        //        }
        //    }
        //    return null;

        //}

        //public JsonResult Delete(string id)
        //{//حذف یک رکورد
        //    try
        //    {
        //        Models.cartaxEntities Car = new Models.cartaxEntities();
        //        if (Convert.ToInt32(id) != 0)
        //        {
        //            Car.sp_BankDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
        //            return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
        //        }
        //        else
        //        {
        //            return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 });
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        Models.cartaxEntities Car = new Models.cartaxEntities();
        //        System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
        //        string InnerException = "";
        //        if (x.InnerException.Message != null)
        //            InnerException = x.InnerException.Message;
        //        Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
        //        return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
        //    }
        //}

        //public JsonResult Save(Models.Bank Bank)
        //{
        //    try
        //    {
        //        Models.cartaxEntities Car = new Models.cartaxEntities();
        //        if (Bank.fldDesc == null)
        //            Bank.fldDesc = "";
        //        if (Bank.fldID == 0)
        //        {//ثبت رکورد جدید
        //            byte[] image = null;
        //            if (Bank.fldImage != null)
        //                image = Avarez.Helper.ClsCommon.Base64ToImage(Bank.fldImage);
        //            Car.sp_BankInsert(Bank.fldName, Bank.fldBankTypeID, Bank.fldCentralBankCode,
        //                Bank.fldInfinitiveBank, Convert.ToInt32(Session["UserId"]), Bank.fldDesc, image, Session["UserPass"].ToString());

        //            return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
        //        }
        //        else
        //        {//ویرایش رکورد ارسالی
        //            Car.sp_BankUpdate(Bank.fldID, Bank.fldName, Bank.fldBankTypeID, Bank.fldCentralBankCode,
        //                Bank.fldInfinitiveBank, Convert.ToInt32(Session["UserId"]), Bank.fldDesc, Avarez.Helper.ClsCommon.Base64ToImage(Bank.fldImage), Session["UserPass"].ToString());
        //            return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        Models.cartaxEntities Car = new Models.cartaxEntities();
        //        System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
        //        string InnerException = "";
        //        if (x.InnerException.Message != null)
        //            InnerException = x.InnerException.Message;
        //        Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
        //        return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
        //    }
        //}

        //public JsonResult Details(int id)
        //{//نمایش اطلاعات جهت رویت کاربر
        //    try
        //    {
        //        Models.cartaxEntities Car = new Models.cartaxEntities();
        //        var q = Car.sp_BankSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
        //        return Json(new
        //        {
        //            fldId = q.fldID,
        //            fldName = q.fldName,
        //            fldBankType = q.fldBankTypeID,
        //            fldInfinitiveBank = q.fldInfinitiveBank,
        //            fldCentralBankCode = q.fldCentralBankCode,
        //            fldDesc = q.fldDesc
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception x)
        //    {
        //        Models.cartaxEntities Car = new Models.cartaxEntities();
        //        System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
        //        string InnerException = "";
        //        if (x.InnerException.Message != null)
        //            InnerException = x.InnerException.Message;
        //        Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
        //        return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
        //    }
        //}

    }
}
