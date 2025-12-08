using Avarez.Controllers.Users;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class ChCarFile_Controller : Controller
    {
        //
        // GET: /NewVer/ChCarFile_/

        public ActionResult Index(int PlaquId, int CarID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 300))
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.PlaquId = PlaquId;
                PartialView.ViewBag.CarID = CarID;
                Models.cartaxEntities p = new Models.cartaxEntities();
                PartialView.ViewBag.Date = MyLib.Shamsi.Miladi2ShamsiString(p.sp_GetDate().FirstOrDefault().CurrentDateTime);
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعویض مالک");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }

        public ActionResult Save(int PlaquId, int CarID, string Date)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 301))
                {
                    System.Data.Entity.Core.Objects.ObjectParameter CarFileid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                    Car.sp_CarFileInsert(CarFileid, CarID, PlaquId,
                             MyLib.Shamsi.Shamsi2miladiDateTime(Date),
                             Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(),
                             null, null, null, null, false, null, null, null);
                    return Json(new { MsgTitle="ذخیره موفق",Msg = "ذخیره با موفقیت انجام شد. کد پرونده: " + CarFileid.Value, Er = 0 });
                }
                else
                {
                    return Json(new { MsgTitle = "خطا", Msg = "شما مجاز به دسترسی نمی باشید.", Er = 1 });
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }
        public ActionResult Reload(string PlaquId)
        {//جستجو
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_CarFileSelect("fldCarPlaqueID", PlaquId, 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().OrderBy(p => p.fldID);
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult Fill(int PlaquId, long CarID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarSelect("fldId", CarID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var Plaqu = p.sp_CarPlaqueSelect("fldId", PlaquId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var car1 = p.sp_SelectCarDetils(CarID).FirstOrDefault();
            return Json(new
            {
                plaq = Plaqu.fldPlaqueCityName + "|" + Plaqu.fldPlaqueSerial + "|" + Plaqu.fldPlaqueNumber,
                classs = car.fldCarClassName,
                modell = car.fldCarModelName,
                fldCarPlaqueID=car1.fldCarPlaqueID,
                syst = car1.fldCarSystemName,
                cabin = car1.fldCarCabinName,
                account = car1.fldCarAccountName,
                make = car1.fldCarMakeName,
                Malek = Plaqu.fldOwnerName,
                CodeMelli = Plaqu.fldOwnerMelli_EconomicCode,
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
