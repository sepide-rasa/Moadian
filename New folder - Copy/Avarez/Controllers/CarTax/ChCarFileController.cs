using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.CarTax
{
    public class ChCarFileController : Controller
    {
        //
        // GET: /ChCarFile/

        public ActionResult Index(int PlaquId,int CarID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 300))
            {
                ViewBag.PlaquId = PlaquId;
                ViewBag.CarID = CarID;
                Models.cartaxEntities p = new Models.cartaxEntities();
                ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(p.sp_GetDate().FirstOrDefault().CurrentDateTime);
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعویض مالک");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Save(int PlaquId, int CarID,string Date)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 301))
                {
                    System.Data.Entity.Core.Objects.ObjectParameter CarFileid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    Car.sp_CarFileInsert(CarFileid, CarID, PlaquId,
                             MyLib.Shamsi.Shamsi2miladiDateTime(Date),
                             Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), null, null, null, null, false, null, null,null);
                    return Json(new { data = "ذخیره با موفقیت انجام شد. کد پرونده: " + CarFileid.Value, state = 0 });
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
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldCarPlaqueID" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_CarFileSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().OrderBy(p => p.fldID);
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Fill(int PlaquId, int CarID)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarSelect("fldId", CarID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var Plaqu = p.sp_CarPlaqueSelect("fldId", PlaquId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var car1 = p.sp_SelectCarDetils(CarID).FirstOrDefault();
            return Json(new
            {
                plaq =Plaqu.fldPlaqueCityName+"|"+Plaqu.fldPlaqueSerial+"|"+ Plaqu.fldPlaqueNumber,
                classs = car.fldCarClassName,
                modell = car.fldCarModelName,
                syst = car1.fldCarSystemName,
                cabin = car1.fldCarCabinName,
                account = car1.fldCarAccountName,
                make = car1.fldCarMakeName,
                Malek = Plaqu.fldOwnerName,
                motor = car.fldMotorNumber,
                shasi = car.fldShasiNumber,
                vin = car.fldVIN,
                color = car.fldColor,
                date = car.fldStartDateInsurance,
                year = car.fldModel

            }, JsonRequestBehavior.AllowGet);
        }
    }
}
