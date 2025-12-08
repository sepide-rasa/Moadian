using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Avarez.Controllers.Users;
using Avarez.Models;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using System.Web.Configuration;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Text;
using System.Xml;
using Avarez.Filters;


namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class SelectParvandeController : Controller
    {
        //
        // GET: /NewVer/SelectParvande/
        string ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
        public ActionResult Index(string containerId, int CarId, string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            ViewData.Model = new Avarez.Models.Varizi_Savabegh();
            Models.cartaxEntities car = new Models.cartaxEntities();
            //var Tree = car.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();

            //var UpTree = m.sp_SelectUpTreeCountryDivisions(Convert.ToInt32(Tree.fldID), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            //var PosIPId = 0;

            //foreach (var item in UpTree.OrderByDescending(l=>l.fldNodeType))
            //{
            //var c = m.sp_GET_IDCountryDivisions(item.fldNodeType, item.fldSourceID).FirstOrDefault();
            //var Pos = car.sp_PcPosInfoSelect("fldTreeId", Tree.CountryDivisionId.ToString(), 0).FirstOrDefault();
            //if (Pos!= null)
            //{
            //    var UserPos = car.sp_PcPosUserSelect("fldIdUser", Session["UserId"].ToString(), 0).FirstOrDefault();
            //    if (UserPos != null)
            //    {
            //        var PosIp = car.sp_PcPosIPSelect("fldId", UserPos.fldPosIPId.ToString(), 0).FirstOrDefault();
            //        if (PosIp != null)
            //        {
            //            PosIPId = PosIp.fldId;
            //            //break;
            //        }
            //    }
            //}
            var q = car.sp_SelectCarDetilsByCarFileID(Convert.ToInt64(CarFileId)).FirstOrDefault();
            var q1 = car.sp_CarFileSelect("fldid", CarFileId, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            string KhodEzhari = "0";//پرونده تایید شده
            if (q1.fldUserID == null || q1.fldAccept == false)
                KhodEzhari = "1";
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };
            //result.ViewBag.PosIPId = PosIPId;
            OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض-> پرونده "+q.fldOwnerName);
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();

            result.ViewBag.CarId = CarId;
            result.ViewBag.ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            result.ViewBag.CarFileId = CarFileId.ToString();
            result.ViewBag.KhodEzhari = KhodEzhari;
            result.ViewBag.Ownername = q.fldOwnerName.ToString();
            this.GetCmp<TabPanel>("AvarezTab").SetActiveTab(0);
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        //ارتباط با pos پیشخوان
        public ActionResult PishkhanPos(int Mablagh, int CarFileid, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            epishkhan_pos.devpishkhanposws pishkhan = new epishkhan_pos.devpishkhanposws();
            var result = pishkhan.validate("7sFi[8r-${R5{B", "autotax4", Session["ver_code"].ToString(), Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ServiceId"]));
            if (result == -11)
            {
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            }
            Models.cartaxEntities p = new Models.cartaxEntities();
            var q = p.sp_CarFileSelect("fldid", CarFileid.ToString(), 0, 1, "").FirstOrDefault();
            var owner = p.sp_OwnerSelect("fldid", q.fldOwnerID.ToString(), 0, 1, "").FirstOrDefault();
            long peackokeryid = CheckExistFishForPos(q.fldCarID, Convert.ToInt32(Mablagh));
            if (peackokeryid == 0)
            {
                Years = (ArrayList)Session["Year" + CarFileid];
                GenerateFishReport(CarFileid, Convert.ToInt32(Mablagh), fldFine, fldValueAddPrice, fldPrice, Years, Bed, fldOtherPrice, fldMainDiscount, fldFineDiscount, fldValueAddDiscount, fldOtherDiscount);
                peackokeryid = CheckExistFishForPos(q.fldCarID, Convert.ToInt32(Mablagh));
            }
            string res = "";
            string posString = "";
            string posSerial = "";
            int price = 0;
            int bed = Convert.ToInt32(calcu(q.fldCarID));
            int err = 0;
            byte flag = 0;
            int posId = 0;
            if (bed > 10000)
            {
                if (peackokeryid != 0)
                {
                    var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                    var pos = p.sp_PishkhanPosSelect("fldFishId", peackokeryid.ToString()).FirstOrDefault();
                    if (pos != null && pos.fldPrice == fish.fldShowMoney)
                    {

                        Session["PishkhanPosId"] = pos.fldId;
                        posSerial = pos.fldSerial;
                        posString = pos.fldPosString;
                        flag = 1;
                        posId = pos.fldId;
                    }
                    else
                    {
                        price = fish.fldShowMoney;
                        res = pishkhan.servicePayPos("7sFi[8r-${R5{B", "autotax4", Session["ver_code"].ToString(),
                            Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ServiceId"]),
                            price, price, owner.fldMelli_EconomicCode, owner.fldMobile);
                        int a;
                        int.TryParse(res, out a);

                        if (a == 0)
                        {
                            posSerial = res.Split('|')[0];
                            posString = res.Split('|')[1];
                            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                            p.sp_PishkhanPosInsert(_id, fish.fldID, price, 1, posSerial, posString, "", "", "", Convert.ToInt32(Session["UserId"]), DateTime.Now, "");
                            Session["PishkhanPosId"] = _id.Value.ToString();
                        }
                    }
                }
            }
            else
                err = 1;
            return Json(new { posString = posString, err = err, flag = flag, posId = posId }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult PishkhanPosResive()
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            p.sp_PishkhanPosResive(Convert.ToInt32(Session["PishkhanPosId"]), 2, Convert.ToInt32(Session["UserId"]));
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult PishkhanPosVerify(string Trace, string TermId, string RRN)
        {
            epishkhan_pos.devpishkhanposws pishkhan = new epishkhan_pos.devpishkhanposws();
            Models.cartaxEntities p = new Models.cartaxEntities();
            var q = p.sp_PishkhanPosSelect("fldid", Session["PishkhanPosId"].ToString()).FirstOrDefault();
            var res = pishkhan.verify("7sFi[8r-${R5{B", "autotax4", Session["ver_code"].ToString(),
                    Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ServiceId"]),
                    q.fldSerial, q.fldId.ToString(), Trace, TermId, RRN);
            int a;
            int.TryParse(res, out a);
            string Msg = "", MsgTitle = "", MsgType = "";
            if (a > 0)
            {
                p.sp_PishkhanPosVerify(Convert.ToInt32(Session["PishkhanPosId"]), 3, TermId, Trace, RRN, Convert.ToInt32(Session["UserId"]), "کد رهگیری: " + a);
                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                var fish = p.sp_PeacockerySelect("fldid", q.fldFishId.ToString(), 0, 1, "").FirstOrDefault();
                var date = p.sp_GetDate().FirstOrDefault().CurrentDateTime;
                p.sp_CollectionInsert(_id, fish.fldCarFileID, date, q.fldPrice,
                    9, (int)q.fldFishId, null, "", Convert.ToInt32(Session["UserId"]),
                    "پرداخت از طریق pc-pos پیشخوان دولت", "", a.ToString(), null, "", null, null, true, 1, date);
                SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));

                Msg = "عملیات با موفقیت انجام شد.";
                MsgTitle = "عملیات موفق";
                MsgType = "1";
            }
            else
            {
                switch (a)
                {
                    case -44:
                        Msg = "امکان تایید نیست، تراکنش هنوز پرداخت نشده است.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -45:
                        Msg = "خطای پایگاه داده در زمان تایید تراکنش کسر از اعتبار";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -46:
                        Msg = "خطای پایگاه داده در زمان تایید تراکنش کسر از کارت";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -50:
                        Msg = "نحوه پرداخت این سرویس تعریف نشده است";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -51:
                        Msg = "تراکنش پرداخت یافت نشد.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -58:
                        Msg = "خطای اتصال به سرویس تایید تراکنش.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -59:
                        Msg = "خطای دریافت اطلاعات از سرویس تایید تراکنش.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -60:
                        Msg = "تراکنش مورد تایید نیست.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                }
            }
            Session.Remove("PishkhanPosId");
            return Json(new { Msg = Msg, MsgTitle = MsgTitle, MsgType = MsgType }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PishkhanResipt(int CarFileid)
        {
            epishkhan_pos.devpishkhanposws pishkhan = new epishkhan_pos.devpishkhanposws();
            var result = pishkhan.validate("7sFi[8r-${R5{B", "autotax4", Session["ver_code"].ToString(), Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ServiceId"]));
            if (result == -11)
            {
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            }
            Models.cartaxEntities p = new Models.cartaxEntities();
            var q = p.sp_CarFileSelect("fldid", CarFileid.ToString(), 0, 1, "").FirstOrDefault();
            var owner = p.sp_OwnerSelect("fldid", q.fldOwnerID.ToString(), 0, 1, "").FirstOrDefault();
            string res = "";
            string posSerial = "", posString = "";
            var car = p.sp_SelectCarDetils(q.fldCarID).FirstOrDefault();
            int bed = Convert.ToInt32(calcu(q.fldCarID));
            int err = 0;
            string msg = "";
            byte flag = 0;
            int recieptId = 0;
            if (bed > 10000)
            {
                err = 1;
                msg = "شما مجاز به دریافت رسید دفتر پیش از تسویه حساب پرونده نخواهید بود.";
            }
            var resipt = p.Sp_tblPishkhanResiptSelect("fldCarFileIdInCurrentYear", CarFileid.ToString()).FirstOrDefault();
            if (resipt != null)
            {
                err = 1;
                msg = "کاربر گرامی، مبلغ کارمزد قبلا دریافت شده و مجاز به دریافت مجدد نمی باشید.";

            }
            if (car != null && err == 0)
            {
                int? price = 0;
                var nerkh = p.sp_tblOfficeReciptNerkhSelect(6, car.fldCarClassID, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                if (nerkh != null)
                {
                    price = nerkh.fldPrice;
                    Session["ResiptServiceId"] = nerkh.fldServiceId;
                }
                if (price > 0)
                {
                    var pos = p.Sp_tblPishkhanResiptSelect("fldCarFileId", CarFileid.ToString()).FirstOrDefault();
                    if (pos != null && pos.fldPrice == price)
                    {
                        Session["PishkhanResiptId"] = pos.fldId;
                        posSerial = pos.fldSerial;
                        posString = pos.fldPosString;
                        flag = 1;
                        recieptId = pos.fldId;
                    }

                    res = pishkhan.servicePayPos("7sFi[8r-${R5{B", "autotax4", Session["ver_code"].ToString(),
                        Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ResiptServiceId"]),
                        (int)price, (int)price, owner.fldMelli_EconomicCode, owner.fldMobile);
                    int a;
                    int.TryParse(res, out a);
                    if (a == 0)
                    {
                        posSerial = res.Split('|')[0];
                        posString = res.Split('|')[1];
                        System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                        p.Sp_tblPishkhanResiptInsert(_id, CarFileid, (int)price, "", Convert.ToInt32(Session["UserId"]), 1, posSerial, posString, "", "", "", "");
                        Session["PishkhanResiptId"] = _id.Value.ToString();
                    }
                }
            }
            return Json(new { posString = posString, msg = msg, err = err, recieptId = recieptId, flag = flag }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PishkhanResiptResive()
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            p.Sp_tblPishkhanResiptResive(Convert.ToInt32(Session["PishkhanResiptId"]), Convert.ToInt32(Session["UserId"]), 2);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult PishkhanResiptVerify(string Trace, string TermId, string RRN)
        {
            epishkhan_pos.devpishkhanposws pishkhan = new epishkhan_pos.devpishkhanposws();
            Models.cartaxEntities p = new Models.cartaxEntities();
            var q = p.Sp_tblPishkhanResiptSelect("fldid", Session["PishkhanResiptId"].ToString()).FirstOrDefault();
            var res = pishkhan.verify("7sFi[8r-${R5{B", "autotax4", Session["ver_code"].ToString(),
                    Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ResiptServiceId"]),
                    q.fldSerial, q.fldId.ToString(), Trace, TermId, RRN);
            int a;
            int.TryParse(res, out a);
            string Msg = "", MsgTitle = "", MsgType = "";
            if (a > 0)
            {
                p.Sp_tblPishkhanResiptVerify(Convert.ToInt32(Session["PishkhanResiptId"]), "", Convert.ToInt32(Session["UserId"]), 3, TermId, Trace, RRN, a.ToString());
                Msg = "عملیات با موفقیت انجام شد.";
                MsgTitle = "عملیات موفق";
                MsgType = "1";
            }
            else
            {
                switch (a)
                {
                    case -44:
                        Msg = "امکان تایید نیست، تراکنش هنوز پرداخت نشده است.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -45:
                        Msg = "خطای پایگاه داده در زمان تایید تراکنش کسر از اعتبار";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -46:
                        Msg = "خطای پایگاه داده در زمان تایید تراکنش کسر از کارت";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -50:
                        Msg = "نحوه پرداخت این سرویس تعریف نشده است";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -51:
                        Msg = "تراکنش پرداخت یافت نشد.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -58:
                        Msg = "خطای اتصال به سرویس تایید تراکنش.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -59:
                        Msg = "خطای دریافت اطلاعات از سرویس تایید تراکنش.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                    case -60:
                        Msg = "تراکنش مورد تایید نیست.";
                        MsgTitle = "عملیات  ناموفق";
                        MsgType = "0";
                        break;
                }
            }
            Session.Remove("ResiptServiceId");
            return Json(new { Msg = Msg, MsgTitle = MsgTitle, MsgType = MsgType, resiptid = Convert.ToInt32(Session["PishkhanResiptId"]) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintPishkhanResidWin(int resiptId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.resiptId = resiptId;
            return PartialView;
        }
        public FileResult rptpishkhanRecipt(int id)
        {
            Avarez.Models.cartaxEntities Car = new Avarez.Models.cartaxEntities();
            var resid = Car.Sp_tblPishkhanResiptSelect("fldid", id.ToString()).FirstOrDefault();
            if (resid.fldStatus != 3)
                return null;
            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.Sp_tblPishkhanResiptSelectTableAdapter Receipt = new DataSet.DataSet1TableAdapters.Sp_tblPishkhanResiptSelectTableAdapter();
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
            Receipt.Fill(dt.Sp_tblPishkhanResiptSelect, "fldid", id.ToString());

            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            var mnu = Car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = Car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\PishkhanResipt.frx");
            Report.RegisterData(dt, "dataSet1");
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("OfficeName", Session["office"].ToString());
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")
            {
                Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
            }
            else if (ImageSetting == "2")
            {
                Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
            }
            else
                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);

            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);
            return File(stream.ToArray(), "application/pdf");
        }
        public JsonResult GetFillDate(string year, string Make)
        {
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Make != "1")
            {
                if (Convert.ToInt32(year) > 1900)
                {
                    var date = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(year + "/01/01"));
                    var s = date.Substring(0, 4);
                    for (int i = 0; i < 2; i++)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = s;
                        item.Value = s;
                        sal.Add(item);
                        s = (Convert.ToInt32(s) + 1).ToString();
                    }
                }
            }
            else
            {
                var date = year + "/01/01";
                var s = date.Substring(0, 4);
                for (int i = 0; i < 2; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = s;
                    item.Value = s;
                    sal.Add(item);
                    s = (Convert.ToInt32(s) + 1).ToString();
                }
            }
            return Json(sal.OrderByDescending(l => l.Text).Select(p1 => new { ID = p1.Value, Name = p1.Text }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMake()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_CarMakeSelect("", "", 0, 0, "").Select(c => new { ID = c.fldID, Name = c.fldName }), JsonRequestBehavior.AllowGet);
        }

        public FileContentResult DownloadHelpVideo()
        {
            string savePath = Server.MapPath(@"~\NewHelps\avarez.mp4");
            MemoryStream st = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
            return File(st.ToArray(), MimeType.Get(".mp4"), "avarez.mp4");
        }
        public FileContentResult DownloadHelpPdf()
        {
            string savePath = Server.MapPath(@"~\NewHelps\22عوارض.pdf");
            MemoryStream st = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
            return File(st.ToArray(), MimeType.Get(".pdf"), "avarez.pdf");
        }

        public ActionResult HelpHtml()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }
        }
        public ActionResult SaveBlackList(Models.sp_ListeSiyahSelect BlackList, string CarFileID)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (BlackList.fldDesc == null)
                    BlackList.fldDesc = "";
                if (BlackList.fldId == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 309))
                    {
                        var file = Car.sp_CarFileSelect("fldID", CarFileID.ToString(), 1, 0, "").FirstOrDefault();
                        Car.sp_ListeSiyahInsert(file.fldCarID,BlackList.fldType, BlackList.fldMsg,
                             Convert.ToInt32(Session["UserId"]), BlackList.fldDesc);
                        return Json(new { Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = "ذخیره موفق", Err = 0 });
                    }
                    else
                    {
                        return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Err = 1 });
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 310))
                    {
                        Car.sp_ListeSiyahUpdate(BlackList.fldId, BlackList.fldCarId, BlackList.fldType, BlackList.fldMsg,
                              Convert.ToInt32(Session["UserId"]), BlackList.fldDesc);
                        return Json(new { Msg = "ویرایش با موفقیت انجام شد.", MsgTitle = "ویرایش موفق", Err = 0 });
                    }
                    else
                    {
                        return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Err = 1 });
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Err = 1 });
            }
        }

        public ActionResult GetShort(string cboCarMake)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            if (cboCarMake == "داخلی")
                return Json(car.sp_ShortTermCountrySelect("fldSymbol", "IR", 0, 0, "").Select(c => new { ID = c.fldID, Name = c.fldSymbol }), JsonRequestBehavior.AllowGet);
            else
                return Json(car.sp_ShortTermCountrySelect("", "", 0, 0, "").Where(p => p.fldSymbol != "IR").Select(c => new { ID = c.fldID, Name = c.fldSymbol }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPelakofOwner(string NationalCode)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q = car.sp_CarPlaqueSelect("fldOwnerMelli_EconomicCode", NationalCode, 0, 0, "").Select(c => new { ID = c.fldID, Name = c.fldPlaqueCityName + c.fldPlaqueSerial + c.fldPlaqueNumber }).ToList();
            return Json(car.sp_CarPlaqueSelect("fldOwnerMelli_EconomicCode", NationalCode, 0, 0, "").Select(c => new { ID = c.fldID, Name =c.fldPlaqueCityName+c.fldPlaqueSerial+ c.fldPlaqueNumber }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult checks(string codec)
        {
            char[] chArray = codec.ToCharArray();
            int[] numArray = new int[chArray.Length];
            string x = codec;
            switch (x)
            {
                case "0000000000":
                case "1111111111":
                case "2222222222":
                case "3333333333":
                case "4444444444":
                case "5555555555":
                case "6666666666":
                case "7777777777":
                case "8888888888":
                case "9999999999":
                case "0123456789":
                case "9876543210":

                    return Json(new
                    {
                        data = 0
                    }, JsonRequestBehavior.AllowGet);
            }
            for (int i = 0; i < chArray.Length; i++)
            {
                numArray[i] = (int)char.GetNumericValue(chArray[i]);
            }
            int num2 = numArray[9];

            int num3 = ((((((((numArray[0] * 10) + (numArray[1] * 9)) + (numArray[2] * 8)) + (numArray[3] * 7)) + (numArray[4] * 6)) + (numArray[5] * 5)) + (numArray[6] * 4)) + (numArray[7] * 3)) + (numArray[8] * 2);
            int num4 = num3 - ((num3 / 11) * 11);
            if ((((num4 == 0) && (num2 == num4)) || ((num4 == 1) && (num2 == 1))) || ((num4 > 1) && (num2 == Math.Abs((int)(num4 - 11)))))
            {
                return Json(new
                {
                    data = 1
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    data = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckIsOwner(string NationalCode)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var name = "";
            long MalekIDChange = 0;
            byte ownertype = 0;

            var q = car.sp_OwnerSelect("fldMelli_EconomicCode", NationalCode, 1, 0, "").FirstOrDefault();
            if (q!=null)
            {
                name = q.fldName;
                MalekIDChange = q.fldID;
            }
            return Json(new
            {
                name = name,
                MalekIDChange = MalekIDChange
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckBlackList(string CarFileId)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var file = m.sp_CarFileSelect("fldID", CarFileId.ToString(), 1, 0, "").FirstOrDefault();
            var q = m.sp_ListeSiyahSelect("fldCarId", file.fldCarID.ToString(), 1).FirstOrDefault();
            string Msg = "";
            if (q != null && q.fldType==2)
            {
                Msg = q.fldMsg;
            }
            return Json(new { Msg = Msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTrnStatus(string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities m = new Models.cartaxEntities();
            var file = m.sp_CarFileSelect("fldID", CarFileId.ToString(), 1, 0, "").FirstOrDefault();
            var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
                var y = h.CheckAccountCharge(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName);

                if (y != null)
                {
                    if (y.Type == 1)
                        return Json("1", JsonRequestBehavior.AllowGet);
                    if (y.HaveCharge && y.Type == 2)//Type=2 --> کاربر تراکنشی
                    {

                        var Trans = m.sp_CalcTransactionSelect("fldCarId", file.fldCarID.ToString(), 0).FirstOrDefault();
                        if (Trans != null)
                        {
                            if (MyLib.Shamsi.DiffOfShamsiDate(Trans.fldTarikh, MyLib.Shamsi.Miladi2ShamsiString(m.sp_GetDate().FirstOrDefault().CurrentDateTime)) > 30)
                            {
                                return Json("0", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json("1", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json("0", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json("0", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveChangeOwner(int PlaquId, int CarID, string Date)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();

                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 301))
                {
                    var T = MyLib.Shamsi.Shamsi2miladiString(Date);
                    var z = Car.sp_GetDate().FirstOrDefault().Time;
                    string D = T + " " + z;

                    System.Data.Entity.Core.Objects.ObjectParameter CarFileid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    Car.sp_CarFileInsert(CarFileid, CarID, PlaquId,
                             Convert.ToDateTime(D),
                             Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), null, null, null, null, false, null, null, null);
                    var plaqedetail=Car.sp_CarPlaqueSelect("fldId", PlaquId.ToString(), 1, 1, "").FirstOrDefault();
                    return Json(new {carFilechange=CarFileid.Value, plaqecity=plaqedetail.fldPlaqueCityName,plaqenumber=plaqedetail.fldPlaqueNumber,pluqeserial=plaqedetail.fldPlaqueSerial, Msg = "ذخیره با موفقیت انجام شد. کد پرونده: " + CarFileid.Value, MsgtTitle = "ذخیره موفق", Er = 0 });
                }
                else
                {
                    return Json(new { MsgTitle = "خطا", Msg = "شما مجاز به دسترسی نمی باشید.", Er = 1 });
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",MsgTitle="خطا", Er = 1 });
            }
        }
        public ActionResult FillChangeMalek(string CarID, string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarSelect("fldId", CarID.ToString(), 1, 0, "").FirstOrDefault();
            /*var Plaqu = p.sp_CarPlaqueSelect("fldId", PlaquId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();*/
            var car1 = p.sp_SelectCarDetilsByCarFileID(Convert.ToInt64(CarFileId)).FirstOrDefault();
            var currentDate = p.sp_GetDate().FirstOrDefault().DateShamsi;
            return Json(new
            {
                /*plaq = Plaqu.fldPlaqueCityName + "|" + Plaqu.fldPlaqueSerial + "|" + Plaqu.fldPlaqueNumber,*/
                classs = car.fldCarClassName,
                modell = car.fldCarModelName,
                fldCarPlaqueID = car1.fldCarPlaqueID,
                syst = car1.fldCarSystemName,
                cabin = car1.fldCarCabinName,
                account = car1.fldCarAccountName,
                make = car1.fldCarMakeName,
                currentDate=currentDate,
                /*Malek = Plaqu.fldOwnerName,
                CodeMelli = Plaqu.fldOwnerMelli_EconomicCode,*/
                motor = car.fldMotorNumber,
                shasi = car.fldShasiNumber,
                vin = car.fldVIN,
                color = car.fldColor,
                date = car.fldStartDateInsurance,
                year = car.fldModel

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReloadGridCarFileOwner(string PlaquId)
        {//جستجو
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_CarFileSelect("fldCarPlaqueID", PlaquId, 30, 0, "").ToList().OrderBy(p => p.fldID);
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSal(string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var file = p.sp_CarFileSelect("fldID", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var car = p.sp_CarSelect("fldid", file.fldCarID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            List<SelectListItem> sal = new List<SelectListItem>();
            var Tasal = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)) - 1;
            if (ImageSetting == "3")
                Tasal = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4));
            if (car.fldModel < 1900)
            {
                for (int i = car.fldModel; i <= Tasal; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(car.fldModel.ToString() + "/05/21")).Substring(0, 4)) - 1; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSal2(int StartYear)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Models.Sal> sh = new List<Models.Sal>();
            var q = m.sp_GetDate().FirstOrDefault();
            int fldSal = Convert.ToInt32(q.DateShamsi.Substring(0, 4));
            var Tasal = fldSal - 1;
            if (ImageSetting == "3")
                Tasal = fldSal;
            for (int i = StartYear; i <= Tasal ; i++)
            {
                Models.Sal CboSal = new Models.Sal();

                CboSal.fldSal = i;
                sh.Add(CboSal);

            }
            var q2 = sh.Where(l => l.fldSal >= StartYear);

            return Json(q2, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadSavabegh()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

                if (Session["savePath"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    System.IO.File.Delete(Session["savePath"].ToString());
                    Session.Remove("savePath");
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tif"
                || extension == ".tiff")
                {
                    if (ImageSetting == "4")
                    {
                        if (Request.Files[0].ContentLength > 716800)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else if (Request.Files[0].ContentLength < 51200)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست بیشتر از 50 کیلو بایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else
                        {
                            HttpPostedFileBase file = Request.Files[0];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[0].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                    }
                    else
                    {
                        if (Request.Files[0].ContentLength <= 716800)
                        {
                            HttpPostedFileBase file = Request.Files[0];
                            //var Name=Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[0].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                        else
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }

        }

        public ActionResult UploadBargeSabz()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

                if (Session["savePath2"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["savePath2"].ToString());
                    System.IO.File.Delete(Session["savePath2"].ToString());
                    Session.Remove("savePath2");
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tif"
                || extension == ".tiff")
                {
                    if (ImageSetting == "4")
                    {
                        if (Request.Files[0].ContentLength > 716800)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else if (Request.Files[0].ContentLength < 51200)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست بیشتر از 50 کیلو بایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else
                        {
                            HttpPostedFileBase file = Request.Files[0];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath2"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[0].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                    }
                    else
                    {
                        if (Request.Files[0].ContentLength <= 716800)
                        {
                            HttpPostedFileBase file = Request.Files[0];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath2"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[0].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                        else
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult ReadFilesofPelak(StoreRequestParameters parameters, string PlaquId)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            List<Models.sp_CarFileSelect> data = null;
            data = p.sp_CarFileSelect("fldCarPlaqueID", PlaquId, 100,0, "").ToList();
            return this.Store(data);
        }
        public ActionResult ShowPic(int state)
        {//برگرداندن عکس 
            byte[] file = null;
            MemoryStream stream = null;
            if (state == 2)
            {
                stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath2"].ToString()));
            }
            else if (state == 3)
            {
                stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath3"].ToString()));
            }
            else if (state == 4)
            {
                stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath4"].ToString()));
            }
            else if (state == 5)
            {
                stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath5"].ToString()));
            }
            file = stream.ToArray();
            var Image1=System.Drawing.Image.FromStream(new System.IO.MemoryStream(file));
            var image = Convert.ToBase64String(file);

            return Json(new { image = image, width = Image1.Width, height = Image1.Height});
        }

        public ActionResult UploadKart()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

                if (Session["savePath3"] != null)
                {
                    System.IO.File.Delete(Session["savePath3"].ToString());
                    Session.Remove("savePath3");
                }
                var extension = Path.GetExtension(Request.Files[1].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tif"
                || extension == ".tiff")
                {
                    if (ImageSetting == "4")
                    {
                        if (Request.Files[1].ContentLength > 716800)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else if (Request.Files[1].ContentLength < 51200)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست بیشتر از 50 کیلو بایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else
                        {
                            HttpPostedFileBase file = Request.Files[1];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath3"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[1].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                    }
                    else
                    {
                        if (Request.Files[1].ContentLength <= 716800)
                        {
                            HttpPostedFileBase file = Request.Files[1];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath3"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[1].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                        else
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }

        public ActionResult UploadBackKart()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

                if (Session["savePath4"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["savePath4"].ToString());
                    System.IO.File.Delete(Session["savePath4"].ToString());
                    Session.Remove("savePath4");
                }
                var extension = Path.GetExtension(Request.Files[2].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tif"
                || extension == ".tiff")
                {
                    if (ImageSetting == "4")
                    {
                        if (Request.Files[2].ContentLength > 716800)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else if (Request.Files[2].ContentLength < 51200)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست بیشتر از 50 کیلو بایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else
                        {
                            HttpPostedFileBase file = Request.Files[2];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath4"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[2].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                    }
                    else
                    {
                        if (Request.Files[2].ContentLength <= 716800)
                        {
                            HttpPostedFileBase file = Request.Files[2];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath4"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[2].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                        else
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }

        public ActionResult UploadSanad()
        {
            string Msg = "";
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

                if (Session["savePath5"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["savePath5"].ToString());
                    System.IO.File.Delete(Session["savePath5"].ToString());
                    Session.Remove("savePath5");
                }
                var extension = Path.GetExtension(Request.Files[3].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tif"
                || extension == ".tiff")
                {
                    if (ImageSetting == "4")
                    {
                        if (Request.Files[3].ContentLength > 716800)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else if (Request.Files[3].ContentLength < 51200)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست بیشتر از 50 کیلو بایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else
                        {
                            HttpPostedFileBase file = Request.Files[3];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath5"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[3].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                    }
                    else
                    {
                        if (Request.Files[3].ContentLength <= 716800)
                        {
                            HttpPostedFileBase file = Request.Files[3];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath5"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[3].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                        else
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }

        public ActionResult SavePictures(string carFileId,int fileIdB,int fileIdK,int FileIdBK, int fileIdS)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            int Bargsabzfileidd = 0;
            int Cartfileidd = 0;
            int Sanadfileidd = 0;
            int CartBackfileidd = 0;

            try
            {
                string Msg = ""; string MsgTitle = ""; var Er = 0;
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 240))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    var file = Car.sp_CarFileSelect("fldid", carFileId, 0, 1, "").FirstOrDefault();
                    if (file.fldAccept == true && Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 434)==false)
                    {
                        return Json(new { MsgTitle = "خطا", Msg = "پرونده تأیید شده و شما قادر به ویرایش نمی باشید.", Er = 1 });
                    }

                    var subSett = Car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), Car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                    bool? ForceScan = true;
                    if (subSett != null)
                    {
                        ForceScan = subSett.fldHaveScan;
                    }
                    if (Session["savePath2"] != null || Session["savePath3"] != null || Session["savePath4"] != null || Session["savePath5"] != null)
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                        System.Data.Entity.Core.Objects.ObjectParameter b = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                        System.Data.Entity.Core.Objects.ObjectParameter c = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                        System.Data.Entity.Core.Objects.ObjectParameter d = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));

                        if ((Session["savePath3"] != null && Session["savePath4"] == null) || (Session["savePath3"] == null && Session["savePath4"] != null))
                        {

                            if (fileIdK == 0 && FileIdBK == 0)
                            {
                                if (Session["savePath2"] != null)
                                {
                                    System.IO.File.Delete(Session["savePath2"].ToString());
                                    Session.Remove("savePath2");
                                }
                                if (Session["savePath3"] != null)
                                {
                                    System.IO.File.Delete(Session["savePath3"].ToString());
                                    Session.Remove("savePath3");
                                }
                                if (Session["savePath5"] != null)
                                {
                                    System.IO.File.Delete(Session["savePath5"].ToString());
                                    Session.Remove("savePath5");
                                }
                                if (Session["savePath4"] != null)
                                {
                                    System.IO.File.Delete(Session["savePath4"].ToString());
                                    Session.Remove("savePath4");
                                }
                                return Json(new
                                {
                                    Msg = "صحفه اول و دوم کارت خودرو باید همزمان آپلود شوند.",
                                    MsgTitle = "خطا",
                                    Er = 1
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        if (Session["savePath2"] != null)
                        {
                            string savePath2 = Session["savePath2"].ToString();

                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath2));
                            var Ex = Path.GetExtension(savePath2);
                            if (Ex == ".tiff" || Ex == ".tif")
                            {
                                using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath2))
                                {
                                    image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                }
                            }
                            byte[] _File = stream.ToArray();

                            Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
                            System.IO.File.Delete(savePath2);

                            Session.Remove("savePath2");

                            if (a.Value != null)
                                Bargsabzfileidd = Convert.ToInt32(a.Value);
                        }
                        else if (fileIdB != 0)
                        {
                            Bargsabzfileidd = fileIdB;
                        }
                        if (Session["savePath3"] != null)
                        {
                            string savePath3 = Session["savePath3"].ToString();

                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath3));
                            var Ex = Path.GetExtension(savePath3);
                            if (Ex == ".tiff" || Ex == ".tif")
                            {
                                using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath3))
                                {
                                    image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                }
                            }
                            byte[] _File = stream.ToArray();

                            Car.Sp_FilesInsert(b, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
                            System.IO.File.Delete(savePath3);

                            Session.Remove("savePath3");

                            if (b.Value != null)
                                Cartfileidd = Convert.ToInt32(b.Value);
                        }
                        else if (fileIdK != 0)
                        {
                            Cartfileidd = fileIdK;
                        }
                        if (Session["savePath4"] != null)
                        {
                            string savePath4 = Session["savePath4"].ToString();

                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath4));
                            var Ex = Path.GetExtension(savePath4);
                            if (Ex == ".tiff" || Ex == ".tif")
                            {
                                using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath4))
                                {
                                    image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                }
                            }
                            byte[] _File = stream.ToArray();

                            Car.Sp_FilesInsert(c, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
                            System.IO.File.Delete(savePath4);

                            Session.Remove("savePath4");
                            if (c.Value != null)
                                CartBackfileidd = Convert.ToInt32(c.Value);
                        }
                        else if (FileIdBK != 0)
                        {
                            CartBackfileidd = FileIdBK;
                        }
                        if (Session["savePath5"] != null)
                        {
                            string savePath5 = Session["savePath5"].ToString();

                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath5));
                            var Ex = Path.GetExtension(savePath5);
                            if (Ex == ".tiff" || Ex == ".tif")
                            {
                                using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath5))
                                {
                                    image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                }
                            }
                            byte[] _File = stream.ToArray();

                            Car.Sp_FilesInsert(d, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
                            System.IO.File.Delete(savePath5);

                            Session.Remove("savePath5");
                            if (d.Value != null)
                                Sanadfileidd = Convert.ToInt32(d.Value);
                        }
                        else if (fileIdS != 0)
                        {
                            Sanadfileidd = fileIdS;
                        }
                    }
                    else if (fileIdB != 0 || fileIdK != 0 || fileIdS != 0 || FileIdBK != 0)
                    {
                        if (Session["savePath2"] != null)
                        {
                            System.IO.File.Delete(Session["savePath2"].ToString());
                            Session.Remove("savePath2");
                        }
                        if (Session["savePath3"] != null)
                        {
                            System.IO.File.Delete(Session["savePath3"].ToString());
                            Session.Remove("savePath3");
                        }
                        if (Session["savePath5"] != null)
                        {
                            System.IO.File.Delete(Session["savePath5"].ToString());
                            Session.Remove("savePath5");
                        }
                        if (Session["savePath4"] != null)
                        {
                            System.IO.File.Delete(Session["savePath4"].ToString());
                            Session.Remove("savePath4");
                        }
                        if (fileIdK != 0 && FileIdBK == 0)
                        {
                            return Json(new
                            {
                                Msg = "لطفا صفحه2 کارت خودرو را آپلود کنید.",
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else if (fileIdK == 0 && FileIdBK != 0)
                        {
                            if (Session["savePath2"] != null)
                            {
                                System.IO.File.Delete(Session["savePath2"].ToString());
                                Session.Remove("savePath2");
                            }
                            if (Session["savePath3"] != null)
                            {
                                System.IO.File.Delete(Session["savePath3"].ToString());
                                Session.Remove("savePath3");
                            }
                            if (Session["savePath5"] != null)
                            {
                                System.IO.File.Delete(Session["savePath5"].ToString());
                                Session.Remove("savePath5");
                            }
                            if (Session["savePath4"] != null)
                            {
                                System.IO.File.Delete(Session["savePath4"].ToString());
                                Session.Remove("savePath4");
                            }
                            return Json(new
                            {
                                Msg = "لطفا تصویر کارت خودرو را آپلود کنید.",
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        Bargsabzfileidd = fileIdB;
                        Cartfileidd = fileIdK;
                        Sanadfileidd = fileIdS;
                        CartBackfileidd = FileIdBK;
                    }
                    else if (ForceScan == true)
                    {
                        return Json(new
                        {
                            Msg = "لطفا فایل مدرک را آپلود کنید.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }

                        Car.sp_SelectCarFile_UplaodFile(Bargsabzfileidd, Cartfileidd, Sanadfileidd, CartBackfileidd, Convert.ToInt32(carFileId));

                        if (fileIdB != 0 && Session["savePath2"] != null)
                            Car.Sp_FilesDelete(fileIdB);
                        if (fileIdK != 0 && Session["savePath3"] != null)
                            Car.Sp_FilesDelete(fileIdK);
                        if (FileIdBK != 0 && Session["savePath4"] != null)
                            Car.Sp_FilesDelete(FileIdBK);
                        if (fileIdS != 0 && Session["savePath5"] != null)
                            Car.Sp_FilesDelete(fileIdS);

                        //اینجا پروسیجری بنویسم که فقط آی دی فایلها را گرفته و update کند.
                        /* Car.sp_CarUpdate(care.fldCarID, care.fldMotorNumber, care.fldShasiNumber, care.fldVIN,
                             care.fldCarModelID, care.fldCarClassID,
                             care.fldCarColorID, care.fldModel, MyLib.Shamsi.Shamsi2miladiDateTime(care.fldStartDateInsurance)
                             , Convert.ToInt32(Session["UserId"]), care.fldDesc, Session["UserPass"].ToString());
                         Car.sp_CarFileUpdate(care.fldID, care.fldCarID, care.fldCarPlaqueID,
                             MyLib.Shamsi.Shamsi2miladiDateTime(care.fldDatePlaque),
                             Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(),
                             Bargsabzfileid, Cartfileid, Sanadfileid, CartBack);*/

                        Msg = "ویرایش با موفقیت انجام شد.";
                        MsgTitle = "ویرایش موفق";
                        Er = 0;
                }
                else
                {
                    Msg = "شما مجاز به دسترسی نمی باشید.";
                    MsgTitle = "خطا";
                    Er = 1;
                }
                return Json(new
                {
                    Msg = Msg,
                    MsgTitle = MsgTitle,
                    Er = Er,
                    CartBackfileidd = CartBackfileidd,
                    Sanadfileidd = Sanadfileidd,
                    Cartfileidd = Cartfileidd,
                    Bargsabzfileidd = Bargsabzfileidd
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                if (Session["savePath2"] != null)
                {
                    System.IO.File.Delete(Session["savePath2"].ToString());
                    Session.Remove("savePath2");
                }
                if (Session["savePath3"] != null)
                {
                    System.IO.File.Delete(Session["savePath3"].ToString());
                    Session.Remove("savePath3");
                }
                if (Session["savePath4"] != null)
                {
                    System.IO.File.Delete(Session["savePath4"].ToString());
                    Session.Remove("savePath4");
                }
                if (Session["savePath5"] != null)
                {
                    System.IO.File.Delete(Session["savePath5"].ToString());
                    Session.Remove("savePath5");
                }
               Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }
        public JsonResult GetCascadeState()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StateSelect("", "", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeCounty(int cboState)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(1, cboState, 5).ToList().OrderBy(h => h.NodeName);
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }), JsonRequestBehavior.AllowGet);
        }
        public string GetFromDate(int? FromYear)
        {
            string date = FromYear + "/01/01";
            return date;
        }
        public string GetToDate(int ToYear)
        {
            string date = ToYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(ToYear))
                date = ToYear + "/12/30";
            return date;
        }


        public ActionResult GetPicStatus(string CarFileid)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                Models.cartaxEntities m = new Models.cartaxEntities();
                // var car = m.sp_SelectCarDetils(id).FirstOrDefault();
                var file = m.sp_CarFileSelect("fldid", CarFileid, 0, 1, "").FirstOrDefault();
                bool status = true; int carfile = 0; int carext = 0; int coll = 0;
                bool status1 = true;
                DateTime dtf1 = MyLib.Shamsi.Shamsi2miladiDateTime(file.fldDate);
                DateTime dtf2 = DateTime.Parse("2017/08/21");

                var subSett = m.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), m.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                var ForceScan = "1";//اجباري
                var ExpScan = "1";
                if (subSett != null)
                {
                    if (subSett.fldHaveScan == false)//اجباري نيست
                        ForceScan = "0";
                    if (subSett.fldExpScan == false)
                        ExpScan = "0";
                }
                if (ImageSetting == "4")
                {
                    if (file.fldSanadForoshFileId == null && file.fldCartFileId == null && file.fldBargSabzFileId == null && file.fldCartBackFileId == null)
                    {
                        status = false;
                        carfile = 1;
                    }
                }
                else
                {
                    if (dtf1.Date > dtf2.Date)
                    {
                        if (file.fldSanadForoshFileId == null && file.fldCartFileId == null && file.fldBargSabzFileId == null && file.fldCartBackFileId == null)
                        {
                            status = false;
                            carfile = 1;
                        }
                    }
                }
                var carex = m.sp_CarExperienceSelect("fldCarFileID", CarFileid, 0, 1, "").ToList();
                if (carex != null)
                {
                    foreach (var item in carex)
                    {
                        DateTime dt1 = MyLib.Shamsi.Shamsi2miladiDateTime(item.fldDate);
                        //DateTime dt2 = DateTime.Parse("2017/08/21");/*برای شاهرودو سمنان و قزوین*/
                        DateTime dt2 = DateTime.Parse("2020/03/28");/*برای شاهرودو سمنان و قزوین*/
                        if (dt1.Date > dt2.Date)
                            if (item.fldFileId == null)
                            {
                                status1 = false;
                                carext = 1;
                            }
                    }
                }
                var collection = m.sp_CollectionSelect("fldCarFileID", CarFileid, 0, 1, "").Where(k => k.fldMunId != null).ToList();
                if (collection != null)
                {
                    foreach (var item in collection)
                    {
                        if (item.fldFileId == null)
                        {
                            status = false;
                            coll = 1;
                        }
                    }
                }
                return Json(new
                {
                    status = status,
                    status1 = status1,
                    carfile = carfile,
                    carext = carext, 
                    coll = coll, 
                    ForceScan = ForceScan, 
                    ExpScan = ExpScan, 
                    Er = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                var msg = x.Message;
                if (x.InnerException != null)
                {
                    msg = x.InnerException.Message;
                }
                return Json(new { Msg = msg, Er = 1 }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ShowMafasa(string containerId, string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities m = new Models.cartaxEntities();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            var file = m.sp_CarFileSelect("fldid", CarFileId, 0, 1, "").FirstOrDefault();
            result.ViewBag.id = file.fldCarID;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult Mafasa(long id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 249))
                {
                    Models.cartaxEntities p = new Models.cartaxEntities();
                    /*var file = p.sp_CarFileSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
                    var carID = file.fldCarID;*/
                    var car = p.sp_SelectCarDetilsByCarFileID(id).FirstOrDefault();
                    if (car != null)
                    {
                        var Cdate = p.sp_GetDate().FirstOrDefault();
                        int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Cdate.CurrentDateTime).Substring(0, 4));
                        string date = toYear + "/12/29";
                        if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                            date = toYear + "/12/30";
                        //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                        //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                        if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
                        {
                            Transaction Tr = new Transaction();
                            var Div = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                            var TransactionInf = p.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                            var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["UserId"]));
                            //string msg1 = ""; string MsgTitle = ""; int Err = 0;
                            switch (Result)
                            {
                                case Transaction.TransactionResult.Fail:
                                    {
                                        return Json(new { Er = 1, Msg = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);
                                        /*X.Msg.Show(new MessageBoxConfig
                                        {
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Title = "خطا",
                                            Message = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد."
                                        });
                                        DirectResult result = new DirectResult();
                                        return result;*/
                                    }


                                case Transaction.TransactionResult.NotSharj:
                                    {
                                        return Json(new { Er = 1, Msg = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);

                                        /*X.Msg.Show(new MessageBoxConfig
                                        {
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Title = "خطا",
                                            Message = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد."
                                        });
                                        DirectResult result = new DirectResult();
                                        return result;*/
                                    }
                            }
                        }
                        var datetime = p.sp_GetDate().FirstOrDefault().CurrentDateTime;
                        //var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                        //    null, datetime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
                        var bedehi = p.prs_newCarFileCalc(datetime, Convert.ToInt32(Session["CountryType"]),
                         Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"])).Where(k => k.fldCollectionId == 0).ToList();
                        string _year = "";
                        if (bedehi != null)
                        {
                            var nullYears = bedehi.Where(k => k.fldPrice == null).ToList();
                            foreach (var item in nullYears)
                            {
                                _year += item.fldYear;
                            }
                        }
                        int? mablagh = 0;
                        foreach (var item in bedehi)
                        {
                            int? jam = (item.fldFinalPrice + item.fldMashmol + item.fldNoMashmol + item.fldMablaghJarime + item.fldOtherPrice) -
                              (item.fldDiscontJarimePrice + item.fldDiscontMoaserPrice + item.fldDiscontOtherPrice + item.fldDiscontValueAddPrice);
                            mablagh += jam;
                        }

                        if (mablagh <= 10000)
                        {
                            Session["CarFileId"] = car.fldID;
                            Session["Sal"] = toYear.ToString().Substring(0, 4);
                            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                            dt.EnforceConstraints = false;
                            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                            Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter();
                            Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccount1TableAdapter fish1 = new Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccount1TableAdapter();
                            Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter Receipt = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
                            Avarez.DataSet.DataSet1TableAdapters.prs_newCarFileCalcForMafasa_VariziTableAdapter MafasaVarizi = new DataSet.DataSet1TableAdapters.prs_newCarFileCalcForMafasa_VariziTableAdapter();
                            MafasaVarizi.Fill(dt.prs_newCarFileCalcForMafasa_Varizi, DateTime.Now, Convert.ToInt32(Session["CountryType"]),
                                Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"]));

                            foreach (var item in dt.prs_newCarFileCalcForMafasa_Varizi)
                            {
                                var Years = item.Years.Split(';');
                                int[] AvarezSal = new int[Years.Count()];
                                for (int i = 0; i < Years.Count(); i++)
                                {
                                    AvarezSal[i] = Convert.ToInt32(Years[i]);
                                }
                                item.Years = arrang(AvarezSal);
                            }
                            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                            Receipt.Fill(dt.rpt_Receipt, Convert.ToInt32(car.fldCarID), 2);
                            Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter exp = new Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter();
                            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                            //if (ImageSetting == "3")
                            //{
                                fish1.Fill(dt.rpt_RecoupmentAccount1, car.fldID,datetime);
                            //}
                            //else
                            //{
                            //    fish.Fill(dt.rpt_RecoupmentAccount, car.fldID, datetime);
                            //}

                            exp.Fill(dt.sp_CarExperienceSelect, "fldCarID", car.fldCarID.ToString(), 0, Convert.ToInt32(Session["UserId"].ToString()), "");
                            Avarez.Models.cartaxEntities Car = new Avarez.Models.cartaxEntities();
                            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                            var mnu = Car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                            var State = Car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();

                            System.Data.Entity.Core.Objects.ObjectParameter mafasaId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(string));
                            System.Data.Entity.Core.Objects.ObjectParameter LetterDate = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterDate", typeof(DateTime));
                            System.Data.Entity.Core.Objects.ObjectParameter LetterNum = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterNum", typeof(string));

                            p.Sp_MafasaInsert(mafasaId, car.fldCarID, Convert.ToInt32(Session["UserMnu"]), null, Convert.ToInt32(Session["UserId"]), LetterDate, LetterNum);
                            string barcode = WebConfigurationManager.AppSettings["SiteURL"] + "/NewVer/QR_MafasaNew/Get/" + mafasaId.Value;
                            string Url = WebConfigurationManager.AppSettings["SiteURL"] + "/NewVer/query";
                            Guid mid = Guid.Parse(mafasaId.Value.ToString());
                            var _ref = p.Sp_MafasaSelect(car.fldCarID).Where(k => k.fldId == mid).FirstOrDefault();

                            FastReport.Report Report = new FastReport.Report();
                            //if (ImageSetting == "3")//زنجان
                            //{
                                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_MafasaZ.frx");
                            //}
                            //else
                            //{
                            //    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_Mafasa.frx");
                            //}
                            /*Report.RegisterData(dt, "complicationsCarDBDataSet");*/
                            Report.RegisterData(dt, "carTaxDataSet");
                            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(LetterDate.Value)));
                            var time = Convert.ToDateTime(LetterDate.Value);
                            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                            Report.SetParameterValue("Num", LetterNum.Value);
                            Report.SetParameterValue("barcode", barcode);
                            Report.SetParameterValue("Url", Url);
                            Report.SetParameterValue("ref", _ref.fldRef);
                            Report.SetParameterValue("MunicipalityName", mnu.fldName);
                            Report.SetParameterValue("StateName", State.fldName);
                            Report.SetParameterValue("AreaName", Session["area"].ToString());
                            Report.SetParameterValue("OfficeName", Session["office"].ToString());
                            Report.SetParameterValue("sal", toYear.ToString().Substring(0, 4));
                            var us = Car.sp_UserSelect("fldid", Session["UserId"].ToString(), 0, ""
                            , Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            string username = us.fldUserName;
                            if (Session["UserMnu"].ToString() == "114")
                            {
                                username = us.fldName + " " + us.fldFamily;
                            }
                            Report.SetParameterValue("UserName", username);
                            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                            MemoryStream stream = new MemoryStream();
                            if (ImageSetting == "1")
                            {
                                Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                            }
                            else if (ImageSetting == "2")
                            {
                                Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                            }
                            else
                                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                            Report.Prepare();
                            Report.Export(pdf, stream);
                            p.Sp_MafasaUpdate(mafasaId.Value.ToString(), stream.ToArray());
                            return Json(new { Er = 0, IdMafasa = mafasaId.Value.ToString() }, JsonRequestBehavior.AllowGet);
                            //return File(stream.ToArray(), "application/pdf");
                        }
                        else
                        {
                            this.GetCmp<Window>("MafasaWin").Destroy();
                            //return Json(new { Er = 1 }, JsonRequestBehavior.AllowGet);
                            return Json(new { Er = 1, Msg = "کاربر گرامی شما در حال انجام عملیات غیر قانونی می باشید و در صورت تکرار آی پی شما به پلیس فتا اعلام خواهد شد. باتشکر", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);

                            /*X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "کاربر گرامی شما در حال انجام عملیات غیر قانونی می باشید و در صورت تکرار آی پی شما به پلیس فتا اعلام خواهد شد. باتشکر"
                            });
                            DirectResult result = new DirectResult();
                            return result;*/
                        }
                    }
                    else
                        return null;
                }
                else
                {
                    return Json(new { Er = 1, Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);

                    /*X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "شما مجاز به دسترسی نمی باشید."
                    });
                    DirectResult result = new DirectResult();
                    return result;*/
                }
            }
            catch(Exception x)
            {
                var Msg = x.Message;
                if (x.InnerException != null)
                {
                    Msg = x.InnerException.Message;
                }
                return Json(new { Er = 1, Msg = Msg, MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult ShowMafasaPdf(string id)
        {
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.Sp_MafasaSelect_Image(id).FirstOrDefault();
                if (q != null)
                {
                    return File(q.fldimage.ToArray(), "application/pdf");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        public FileResult DownloadMafasaPdf(string id)
        {
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.Sp_MafasaSelect_Image(id).FirstOrDefault();
                if (q != null)
                {
                    return File(q.fldimage.ToArray(), MimeType.Get(".pdf"), "Mafasa.pdf");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        public ActionResult ShowMafasaNewTab(string IdMafasa)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            ViewBag.IdMafasa = IdMafasa;
            return View();
        }
        public ActionResult ShowMafasaNewWin(string IdMafasa)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.IdMafasa = IdMafasa;
            return PartialView;
        }

        public ActionResult ShowMafasaInVarizi(string Id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            cartaxEntities m = new cartaxEntities();
            var col = m.sp_CollectionSelect("fldId", Id.ToString(), 0, 1, "").FirstOrDefault();

            string IdMafasa ="";
            var car = m.sp_SelectCarDetilsByCarFileID(col.fldCarFileID).FirstOrDefault();
            if (car != null)
            {

                int toYear = Convert.ToInt32(col.fldCollectionDate.Substring(0, 4));
                
                //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
                {
                    Transaction Tr = new Transaction();
                    var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                    var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["UserId"]));
                    
                    switch (Result)
                    {
                        case Transaction.TransactionResult.Fail:
                            {
                                return Json(new { Er = 1, Msg = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);
                                
                            }


                        case Transaction.TransactionResult.NotSharj:
                            {
                                return Json(new { Er = 1, Msg = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);

                            }
                    }
                }
                var datetime = MyLib.Shamsi.Shamsi2miladiDateTime(col.fldCollectionDate);
                //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                //    null, datetime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
                var bedehi = m.prs_newCarFileCalc(datetime, Convert.ToInt32(Session["CountryType"]),
               Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"])).Where(k => k.fldCollectionId == 0).ToList();
                string _year = "";
                if (bedehi != null)
                {
                    var nullYears = bedehi.Where(k => k.fldPrice == null).ToList();
                    foreach (var item in nullYears)
                    {
                        _year += item.fldYear;
                    }
                }
                int? mablagh = 0;
                foreach (var item in bedehi)
                {
                    int? jam = (item.fldFinalPrice + item.fldMashmol + item.fldNoMashmol + item.fldMablaghJarime + item.fldOtherPrice) -
                         (item.fldDiscontJarimePrice + item.fldDiscontMoaserPrice + item.fldDiscontOtherPrice + item.fldDiscontValueAddPrice);
                    mablagh += jam;
                }

                if (mablagh <= 10000)
                {
                    Session["CarFileId"] = car.fldID;
                    Session["Sal"] = toYear.ToString().Substring(0, 4);
                    Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                    dt.EnforceConstraints = false;
                    Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccount1TableAdapter fish1 = new Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccount1TableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter Receipt = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.prs_newCarFileCalcForMafasa_VariziTableAdapter MafasaVarizi = new DataSet.DataSet1TableAdapters.prs_newCarFileCalcForMafasa_VariziTableAdapter();
                    MafasaVarizi.Fill(dt.prs_newCarFileCalcForMafasa_Varizi, DateTime.Now, Convert.ToInt32(Session["CountryType"]),
                        Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"]));
                    foreach (var item in dt.prs_newCarFileCalcForMafasa_Varizi)
                    {
                        var Years = item.Years.Split(';');
                        int[] AvarezSal = new int[Years.Count()];
                        for (int i = 0; i < Years.Count(); i++)
                        {
                            AvarezSal[i] = Convert.ToInt32(Years[i]);
                        }
                        item.Years = arrang(AvarezSal);
                    }
                    sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                    Receipt.Fill(dt.rpt_Receipt, Convert.ToInt32(car.fldCarID), 2);
                    Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter exp = new Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter();
                    var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                    //if (ImageSetting == "3")
                    //{
                        fish1.Fill(dt.rpt_RecoupmentAccount1, car.fldID, datetime);
                    //}
                    //else
                    //{
                    //    fish.Fill(dt.rpt_RecoupmentAccount, car.fldID, datetime);
                    //}

                    exp.Fill(dt.sp_CarExperienceSelect, "fldCarFileID", car.fldID.ToString(), 0, Convert.ToInt32(Session["UserId"].ToString()), "");
                    
                    Avarez.Models.cartaxEntities Car = new Avarez.Models.cartaxEntities();
                    Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                    var mnu = Car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    var State = Car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();

                    System.Data.Entity.Core.Objects.ObjectParameter mafasaId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterDate = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterDate", typeof(DateTime));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterNum = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterNum", typeof(string));

                    m.Sp_MafasaInsert(mafasaId, car.fldCarID, Convert.ToInt32(Session["UserMnu"]), null, Convert.ToInt32(Session["UserId"]), LetterDate, LetterNum);
                    string barcode = WebConfigurationManager.AppSettings["SiteURL"] + "/NewVer/QR_MafasaNew/Get/" + mafasaId.Value;
                    string Url = WebConfigurationManager.AppSettings["SiteURL"] + "/NewVer/query";
                    Guid mid = Guid.Parse(mafasaId.Value.ToString());
                    var _ref = m.Sp_MafasaSelect(car.fldCarID).Where(k => k.fldId == mid).FirstOrDefault();

                    FastReport.Report Report = new FastReport.Report();
                    //if (ImageSetting == "3")//زنجان
                    //{
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_MafasaZ.frx");
                    //}
                    //else
                    //{
                    //    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_Mafasa.frx");
                    //}
                    /*Report.RegisterData(dt, "complicationsCarDBDataSet");*/
                    Report.RegisterData(dt, "carTaxDataSet");
                    Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(LetterDate.Value)));
                    var time = Convert.ToDateTime(LetterDate.Value);
                    Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                    Report.SetParameterValue("Num", LetterNum.Value);
                    Report.SetParameterValue("barcode", barcode);
                    Report.SetParameterValue("Url", Url);
                    Report.SetParameterValue("ref", _ref.fldRef);
                    Report.SetParameterValue("MunicipalityName", mnu.fldName);
                    Report.SetParameterValue("StateName", State.fldName);
                    Report.SetParameterValue("AreaName", Session["area"].ToString());
                    Report.SetParameterValue("OfficeName", Session["office"].ToString());
                    Report.SetParameterValue("sal", toYear.ToString().Substring(0, 4));

                    Report.SetParameterValue("UserName", Car.sp_UserSelect("fldid", Session["UserId"].ToString(), 0, ""
            , Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldUserName);
                    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                    MemoryStream stream = new MemoryStream();
                    if (ImageSetting == "1")
                    {
                        Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                    }
                    else if (ImageSetting == "2")
                    {
                        Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                    }
                    else
                        Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                    Report.Prepare();
                    Report.Export(pdf, stream);
                    m.Sp_MafasaUpdate(mafasaId.Value.ToString(), stream.ToArray());
                    IdMafasa = mafasaId.Value.ToString();
                    //return File(stream.ToArray(), "application/pdf");
                }

            }            

            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.IdMafasa = IdMafasa;
            return PartialView;
        }

        public ActionResult ShowMafasaInExp(string Id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            cartaxEntities m = new cartaxEntities();
            var col = m.sp_CarExperienceSelect("fldId", Id.ToString(), 0, 1, "").FirstOrDefault();

            string IdMafasa = "";
            var car = m.sp_SelectCarDetilsByCarFileID(col.fldCarFileID).FirstOrDefault();
            if (car != null&&col.fldMunicipalityID==Convert.ToInt32(Session["UserMnu"]))
            {

                int toYear = Convert.ToInt32(col.fldEndDate.Substring(0, 4));

                //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
                {
                    Transaction Tr = new Transaction();
                    var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                    var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["UserId"]));

                    switch (Result)
                    {
                        case Transaction.TransactionResult.Fail:
                            {
                                return Json(new { Er = 1, Msg = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);

                            }


                        case Transaction.TransactionResult.NotSharj:
                            {
                                return Json(new { Er = 1, Msg = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.", MsgTitle = "خطا" }, JsonRequestBehavior.AllowGet);

                            }
                    }
                }
                var datetime = MyLib.Shamsi.Shamsi2miladiDateTime(col.fldEndDate);
                //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                //    null, datetime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
                var bedehi = m.prs_newCarFileCalc(datetime, Convert.ToInt32(Session["CountryType"]),
              Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"])).Where(k => k.fldCollectionId == 0).ToList();
                string _year = "";
                if (bedehi != null)
                {
                    var nullYears = bedehi.Where(k => k.fldPrice == null).ToList();
                    foreach (var item in nullYears)
                    {
                        _year += item.fldYear;
                    }
                }
                int? mablagh = 0;
                foreach (var item in bedehi)
                {
                    int? jam = (item.fldFinalPrice + item.fldMashmol + item.fldNoMashmol + item.fldMablaghJarime + item.fldOtherPrice) -
                        (item.fldDiscontJarimePrice + item.fldDiscontMoaserPrice + item.fldDiscontOtherPrice + item.fldDiscontValueAddPrice);
                    mablagh += jam;
                }

                if (mablagh <= 10000)
                {
                    Session["CarFileId"] = car.fldID;
                    Session["Sal"] = toYear.ToString().Substring(0, 4);
                    Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                    dt.EnforceConstraints = false;
                    Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccount1TableAdapter fish1 = new Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccount1TableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter Receipt = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.prs_newCarFileCalcForMafasa_VariziTableAdapter MafasaVarizi = new DataSet.DataSet1TableAdapters.prs_newCarFileCalcForMafasa_VariziTableAdapter();
                    MafasaVarizi.Fill(dt.prs_newCarFileCalcForMafasa_Varizi, DateTime.Now, Convert.ToInt32(Session["CountryType"]),
                        Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"]));
                    foreach (var item in dt.prs_newCarFileCalcForMafasa_Varizi)
                    {
                        var Years = item.Years.Split(';');
                        int[] AvarezSal = new int[Years.Count()];
                        for (int i = 0; i < Years.Count(); i++)
                        {
                            AvarezSal[i] = Convert.ToInt32(Years[i]);
                        }
                        item.Years = arrang(AvarezSal);
                    }
                    sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");

                    Receipt.Fill(dt.rpt_Receipt, Convert.ToInt32(car.fldCarID), 2);
                    
                    Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter exp = new Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter();
                    var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                    //if (ImageSetting == "3")
                    //{
                        fish1.Fill(dt.rpt_RecoupmentAccount1, car.fldID, datetime);
                    //}
                    //else
                    //{
                    //    fish.Fill(dt.rpt_RecoupmentAccount, car.fldID, datetime);
                    //}

                    exp.Fill(dt.sp_CarExperienceSelect, "fldCarFileID", car.fldID.ToString(), 0, Convert.ToInt32(Session["UserId"].ToString()), "");

                    Avarez.Models.cartaxEntities Car = new Avarez.Models.cartaxEntities();
                    Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                    var mnu = Car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    var State = Car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();

                    System.Data.Entity.Core.Objects.ObjectParameter mafasaId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterDate = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterDate", typeof(DateTime));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterNum = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterNum", typeof(string));

                    m.Sp_MafasaInsert(mafasaId, car.fldCarID, Convert.ToInt32(Session["UserMnu"]), null, Convert.ToInt32(Session["UserId"]), LetterDate, LetterNum);
                    string barcode = WebConfigurationManager.AppSettings["SiteURL"] + "/NewVer/QR_MafasaNew/Get/" + mafasaId.Value;
                    string Url = WebConfigurationManager.AppSettings["SiteURL"] + "/NewVer/query";
                    Guid mid = Guid.Parse(mafasaId.Value.ToString());
                    var _ref = m.Sp_MafasaSelect(car.fldCarID).Where(k => k.fldId == mid).FirstOrDefault();

                    FastReport.Report Report = new FastReport.Report();
                    //if (ImageSetting == "3")//زنجان
                    //{
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_MafasaZ.frx");
                    //}
                    //else
                    //{
                    //    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_Mafasa.frx");
                    //}
                    /*Report.RegisterData(dt, "complicationsCarDBDataSet");*/
                    Report.RegisterData(dt, "carTaxDataSet");
                    Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(LetterDate.Value)));
                    var time = Convert.ToDateTime(LetterDate.Value);
                    Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                    Report.SetParameterValue("Num", LetterNum.Value);
                    Report.SetParameterValue("barcode", barcode);
                    Report.SetParameterValue("Url", Url);
                    Report.SetParameterValue("ref", _ref.fldRef);
                    Report.SetParameterValue("MunicipalityName", mnu.fldName);
                    Report.SetParameterValue("StateName", State.fldName);
                    Report.SetParameterValue("AreaName", Session["area"].ToString());
                    Report.SetParameterValue("OfficeName", Session["office"].ToString());
                    Report.SetParameterValue("sal", toYear.ToString().Substring(0, 4));

                    Report.SetParameterValue("UserName", Car.sp_UserSelect("fldid", Session["UserId"].ToString(), 0, ""
            , Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldUserName);
                    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                    MemoryStream stream = new MemoryStream();
                    if (ImageSetting == "1")
                    {
                        Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                    }
                    else if (ImageSetting == "2")
                    {
                        Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                    }
                    else
                        Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                    Report.Prepare();
                    Report.Export(pdf, stream);
                    m.Sp_MafasaUpdate(mafasaId.Value.ToString(), stream.ToArray());
                    IdMafasa = mafasaId.Value.ToString();
                    //return File(stream.ToArray(), "application/pdf");
                }

            }

            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.IdMafasa = IdMafasa;
            return PartialView;
        }

        public ActionResult SaveSavabegh(Models.CarExp CarExp)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (CarExp.fldDesc == null)
                    CarExp.fldDesc = "";
                 var file = Car.sp_CarFileSelect("fldid", CarExp.fldCarFileID.ToString(), 0, 1, "").FirstOrDefault();
                CarExp.fldStartDate = GetFromDate(CarExp.fldFromYear);
                CarExp.fldEndDate = GetToDate(CarExp.fldToYear);
                var subSett = Car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), Car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                bool? ForceScan = true;
                if (subSett != null)
                {
                    ForceScan = subSett.fldExpScan;
                }
                if (CarExp.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 243))
                    {
                        if (ForceScan == true && Session["savePath"] == null)
                        {
                            return Json(new { Msg = "لطفا فایل مدرک را آپلود کنید.", MsgTitle = "خطا", Err = 1 });
                        }
                        else
                        {
                            System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            int? fileid = null;
                            if (Session["savePath"] != null)
                            {
                                string savePath = Session["savePath"].ToString();

                                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                var Ex = Path.GetExtension(savePath);
                                if (Ex == ".tiff" || Ex == ".tif")
                                {
                                    using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                    {
                                        image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                    }
                                }
                                byte[] _File = stream.ToArray();

                                Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), null, null, null);

                                System.IO.File.Delete(savePath);
                                Session.Remove("savePath");

                                if (a.Value != null)
                                    fileid = Convert.ToInt32(a.Value);
                            }
                            Car.sp_CarExperienceInsert(CarExp.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldStartDate),
                                MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldEndDate),
                                CarExp.fldMunicipalityID, CarExp.fldLetterNumber, Convert.ToInt32(Session["UserId"]),
                                CarExp.fldDesc, Session["UserPass"].ToString(), fileid, false, null, null);
                            /*SignalrHub r = new SignalrHub();
                            r.ReloadCarExperience();*/
                            Car.SaveChanges();
                            return Json(new { Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = "ذخیره موفق", Err = 0 });
                        }
                        //else
                        //    return Json(new { Msg = "لطفا فایل مدرک را آپلود کنید.", MsgTitle = "خطا", Err = 1 });
                    }
                    else
                    {
                        //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        //return RedirectToAction("error", "Metro");
                        if (Session["savePath"] != null)
                        {
                            //string physicalPath = System.IO.Path.Combine(Session["P_savePath"].ToString());
                            System.IO.File.Delete(Session["savePath"].ToString());
                            Session.Remove("savePath");
                        }
                        return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Err = 1 });
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 245))
                    {
                        var q = Car.sp_CarExperienceSelect("fldId", CarExp.fldID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (q.fldAccept == true && Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 435)==false)
                        {
                            return Json(new { MsgTitle = "خطا", Msg = "سابقه مورد نظر تأیید شده و شما قادر به ویرایش آن نمی باشید.", Err = 1 });
                        }
                        
                        if (q.fldUserID == 1 && Convert.ToInt32(Session["UserId"]) != 1)
                        {
                            return Json(new { Msg = "شما مجوز ویرایش را ندارید.", MsgTitle = "خطا", Err = 1 });
                        }
                        int? fileid = null;
                        if (ForceScan == true && Session["savePath"] != null)
                        {
                            System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                            string savePath = Session["savePath"].ToString();

                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                            var Ex = Path.GetExtension(savePath);
                            if (Ex == ".tiff" || Ex == ".tif")
                            {
                                using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                {
                                    image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                }
                            }
                            byte[] _File = stream.ToArray();

                            Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
                            System.IO.File.Delete(savePath);

                            if (a.Value != null)
                                fileid = Convert.ToInt32(a.Value);
                        }
                        else if (CarExp.fldFileId != null)
                            fileid = CarExp.fldFileId;
                        else if (ForceScan == true && CarExp.fldFileId == null)
                            return Json(new { Msg = "لطفا فایل مدرک را آپلود کنید.", MsgTitle = "خطا", Err = 1 });
                        Car.sp_CarExperienceUpdate(CarExp.fldID, CarExp.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldStartDate)
                            , MyLib.Shamsi.Shamsi2miladiDateTime(CarExp.fldEndDate),
                            CarExp.fldMunicipalityID, CarExp.fldLetterNumber, Convert.ToInt32(Session["UserId"]),
                            CarExp.fldDesc, Session["UserPass"].ToString(), fileid);
                        /*SignalrHub r = new SignalrHub();
                        r.ReloadCarExperience();*/
                        if (CarExp.fldFileId != null && Session["savePath"] != null)
                            Car.Sp_FilesDelete(CarExp.fldFileId);
                        Session.Remove("savePath");
                        return Json(new { Msg = "ویرایش با موفقیت انجام شد.", MsgTitle = "ویرایش موفق", Err = 0 });
                    }
                    else
                    {
                        if (Session["savePath"] != null)
                        {
                            //string physicalPath = System.IO.Path.Combine(Session["P_savePath"].ToString());
                            System.IO.File.Delete(Session["savePath"].ToString());
                            Session.Remove("savePath");
                        }
                        //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        //return RedirectToAction("error", "Metro");
                        return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Err = 1 });
                    }
                }
            }
            catch (Exception x)
            {
                if (Session["savePath"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["P_savePath"].ToString());
                    System.IO.File.Delete(Session["savePath"].ToString());
                    Session.Remove("savePath");
                }
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Err = 1 });
            }
        }

        public ActionResult CheckTaiidSavabegh(int Id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carEx = p.sp_CarExperienceSelect("fldid", Id.ToString(), 0, 1, "").FirstOrDefault();
            return Json(new
            {
                HaveTaiid = carEx.fldAccept,
                userId = Session["UserId"].ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckTaeedSavabeghfromnow(string carFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            var taeed = true;
            bool? fldTaeedSavabegh = false;
            Models.cartaxEntities p = new Models.cartaxEntities();
            var subSett = p.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
            if (subSett != null)
            {
                fldTaeedSavabegh=subSett.fldTaeedSavabegh;
            }
            var carEx = p.sp_CarExperienceSelect("fldCarFileID", carFileId, 0, 1, "").ToList();
            if(carEx.Count!=0){
                var q=carEx.Where(l=>Convert.ToInt32(l.fldDate.Replace("/",""))>13970404 && l.fldAccept==false).ToList();
                if(q.Count>0){
                    taeed=false;
                }
            }
            return Json(new
            {
                taeed = taeed,
                fldTaeedSavabegh = fldTaeedSavabegh
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckTaiidPardakhtha(int Id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carCollection = p.sp_CollectionSelect("fldid", Id.ToString(), 0, 1, "").FirstOrDefault();
            bool? HaveTaiid = true;
            HaveTaiid = carCollection.fldAccept;
            return Json(new
            {
                HaveTaiid = HaveTaiid,
                userId = Session["UserId"].ToString()
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getDetailSavabegh(int Id)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_CarExperienceSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q1 = Car.sp_MunicipalitySelect("fldId", q.fldMunicipalityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q2 = Car.sp_CitySelect("fldId", q1.fldCityID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q3 = Car.sp_ZoneSelect("fldId", q2.fldZoneID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var q4 = Car.sp_CountySelect("fldId", q3.fldCountyID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldID,
                    fldDesc = q.fldDesc,
                    fldFromYear = q.fldStartDate.Substring(0, 4),
                    fldToYear = q.fldEndDate.Substring(0, 4),
                    fldLetterNumber = q.fldLetterNumber,
                    fldMunID = q.fldMunicipalityID.ToString(),
                    fldStateId = q4.fldStateID.ToString(),
                    fldFileId = q.fldFileId,
                    Er = 0
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }

        public ActionResult getDetailsBlackList(int Id)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_ListeSiyahSelect("fldId", Id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldId,
                    CarId = q.fldCarId,
                    fldType = q.fldType.ToString(),
                    fldMsg = q.fldMsg,
                    Er = 0
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }

        public ActionResult getDetailVarizi(int Id)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_CollectionSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var p = Car.sp_MunicipalitySelect("fldId", q.fldMunId.ToString(), 1, 1, "").FirstOrDefault();
                var payType = "2";
                if (q.fldMunId == null)
                    payType = "1";
                int munid = 0;
                string munName = "";
                if (p != null)
                {
                    munid = p.fldID;
                    munName = p.fldName;
                }
                return Json(new
                {
                    fldId = q.fldID,
                    fldDesc = q.fldDesc,
                    fldPeacockeryCode = q.fldPeacockeryCode,
                    fldPrice = q.fldPrice,
                    fldSettleTypeID = q.fldSettleTypeID.ToString(),
                    fldCollectionDate = q.fldCollectionDate,
                    fldSerialBarChasb = q.fldSerialBarChasb,
                    fldPayType = payType,
                    fldSerialFish = q.fldSerialFish,
                    fldName = munName,
                    fldMunId = munid,
                    fldFileId = q.fldFileId,
                    Er = 0
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
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }

        public ActionResult DeleteVarizi(string Id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {

                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 292))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    var Collect=Car.sp_CollectionSelect("fldId", Id, 1, 0, "").FirstOrDefault();
                    //var file = Car.sp_CarFileSelect("fldId", Collect.fldCarFileID.ToString(), 1, 0, "").FirstOrDefault();
                    if (Collect.fldAccept == true && Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 433)==false)
                    {
                        return Json(new { MsgTitle = "خطا", Msg = "واریزی مورد نظر تأیید شده و شما قادر به حذف نمی باشید.", Er = 1 });
                    }
                    if (Convert.ToInt32(Id) != 0)
                    {
                        var q = Car.sp_CollectionSelect("fldId", Id, 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
                        if (q.fldPeacockeryCode == null)
                        {
                            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 320))
                            {
                                Car.sp_CollectionDelete(Convert.ToInt32(Id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                                return Json(new { MsgTitle = "حذف موفق", Msg = "حذف با موفقیت انجام شد.", Er = 0 });
                            }
                            else
                            {
                                return Json(new { MsgTitle = "خطا", Msg = "شما مجاز به دسترسی نمی باشید.", Er = 1 });
                            }
                        }
                        else
                        {
                            Car.sp_CollectionDelete(Convert.ToInt32(Id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                            return Json(new { MsgTitle = "حذف موفق", Msg = "حذف با موفقیت انجام شد.", Er = 0 });
                        }
                    }
                    else
                    {
                        return Json(new { MsgTitle = "خطا", Msg = "رکوردی برای حذف انتخاب نشده است.", Er = 1 });
                    }
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

        public ActionResult DeleteBlackList(string Id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 311))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();

                    Car.sp_ListeSiyahDelete(Convert.ToInt32(Id), Convert.ToInt32(Session["UserId"]));
                    return Json(new { Msg = "حذف با موفقیت انجام شد.", MsgTitle = "حذف موفق", Er = 0 });

                }
                else
                {
                    return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Er = 1 });
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er= 1 });
            }
        }

        public ActionResult DeleteSavabegh(string Id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 244))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    var exp=Car.sp_CarExperienceSelect("fldId", Id, 1, 1, "").FirstOrDefault();
                    //var file = Car.sp_CarFileSelect("fldid", exp.fldCarFileID.ToString(), 0, 1, "").FirstOrDefault();
                    if (exp.fldAccept == true && Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 435)==false)
                    {
                        return Json(new { MsgTitle = "خطا", Msg = "سابقه مورد نظر تأیید شده و شما قادر به حذف نمی باشید.", Er = 1 });
                    }
                    Car.sp_CarExperienceDelete(Convert.ToInt32(Id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                    /*SignalrHub r = new SignalrHub();
                    r.ReloadCarExperience();*/
                    return Json(new { Msg = "حذف با موفقیت انجام شد.", MsgTitle = "حذف موفق", Er = 0 });
                }
                else
                {
                    return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Er = 1 });
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }
        public ActionResult ReadVarizi(StoreRequestParameters parameters,string CarFileId)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var file = car.sp_CarFileSelect("fldID", CarFileId, 1, 0, "").FirstOrDefault();
            List<Models.rpt_Receipt> data = null;
            data = car.rpt_Receipt(Convert.ToInt32(file.fldCarID), 2).ToList();
            return this.Store(data);
        }

        public ActionResult ReadPeacockery(StoreRequestParameters parameters, string CarFileId)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            List<Models.sp_PeacockerySelect> data = null;
            data = car.sp_PeacockerySelect("NotCollection", CarFileId,0,1, "").ToList();
            return this.Store(data);
        }
        public ActionResult ReadBlackList(StoreRequestParameters parameters, string CarFileId)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var file = car.sp_CarFileSelect("fldID", CarFileId.ToString(), 1, 0, "").FirstOrDefault();
            List<Models.sp_ListeSiyahSelect> data = null;
            data = car.sp_ListeSiyahSelect("", "", 0).Where(l => l.fldCarId == Convert.ToInt32(file.fldCarID)).ToList();
            return this.Store(data);
        }

        public ActionResult HelpSelectParvande()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }
        }
        public ActionResult ReadSavabegh(StoreRequestParameters parameters, string CarFileId)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var file = car.sp_CarFileSelect("fldID", CarFileId, 1, 0, "").FirstOrDefault();
            List<Models.sp_CarExperienceSelect> data = null;
            data = car.sp_CarExperienceSelect("fldCarID", file.fldCarID.ToString(), 30, 0, "").ToList();
            return this.Store(data);
        }

        public ActionResult ImageGallery(string CarFileId)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Session["CarFileGalleryId"] = CarFileId;
            PartialView.ViewBag.CarFileId = CarFileId;
            return PartialView;
        }

        public ActionResult ShowMadrak(string Id)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Avarez.Models.cartaxEntities p = new Avarez.Models.cartaxEntities();
            var carEx = p.sp_CarExperienceSelect("fldid", Id.ToString(), 0, 1, "").FirstOrDefault();
            PartialView.ViewBag.fileIdSavabegh = (int)carEx.fldFileId;
            return PartialView;
        }

        public ActionResult ShowMadrakVariz(string Id)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Avarez.Models.cartaxEntities p = new Avarez.Models.cartaxEntities();
            var Fish = p.sp_CollectionSelect("fldid", Id.ToString(), 0, 1, "").FirstOrDefault();
            PartialView.ViewBag.fileIdVariz = (int)Fish.fldFileId;
            return PartialView;
        }

        public ActionResult GetPrice(string Id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            int Money = 0;
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_PeacockerySelect("fldId", Id, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (q != null)
            {
                Money = q.fldShowMoney;
            }

            return Json(Money, JsonRequestBehavior.AllowGet);
        }

        [EncryptedActionParameter]
        public FileContentResult Image(int State, int id)
        {//برگرداندن عکس 
            Avarez.Models.cartaxEntities p = new Avarez.Models.cartaxEntities();

            var pic = p.Sp_FilesSelect(id).FirstOrDefault();
            ViewBag.tempId = id;
            if (pic != null)
            {
                if (pic.fldImage != null)
                {
                    return File((byte[])pic.fldImage, "jpg");
                }
            }
            return null;
        }

        public ActionResult GetSettleType()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Settle = car.sp_SettleTypeSelect("", "", 0, 0, "").ToList();
            return Json(Settle.Select(p1 => new { SettleTypeID = p1.fldID, SettleTypeName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadVarizi()
        {
            string Msg = "";

            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                if (Session["savePath1"] != null)
                {
                    //string physicalPath = System.IO.Path.Combine(Session["savePath1"].ToString());
                    System.IO.File.Delete(Session["savePath1"].ToString());
                    Session.Remove("savePath1");
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tif"
                || extension == ".tiff")
                {
                    if (ImageSetting == "4")
                    {
                        if (Request.Files[0].ContentLength > 716800)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else if (Request.Files[0].ContentLength < 51200)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست بیشتر از 50 کیلو بایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                        else
                        {
                            HttpPostedFileBase file = Request.Files[0];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath1"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[0].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                    }
                    else
                    {
                        if (Request.Files[0].ContentLength <= 716800)
                        {
                            HttpPostedFileBase file = Request.Files[0];
                            //var Name = Path.GetFileNameWithoutExtension(file.FileName);
                            var Name = Guid.NewGuid();
                            string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(savePath);
                            Session["savePath1"] = savePath;
                            object r = new
                            {
                                success = true,
                                name = Request.Files[0].FileName
                            };
                            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                        else
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "حجم فایل انتخابی می بایست کمتر از 700 کیلوبایت باشد."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult Taid(int CarFileId)
        {
            try
            {
                cartaxEntities p = new cartaxEntities();
                string MsgTitle = "", Msg = "", KhodEzhari = "1";
                int Er = 0;
                var carfile = p.sp_CarFileSelect("fldid", CarFileId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (carfile != null)
                {
                    if (carfile.fldAccept == false)
                    {
                        p.sp_CarFileTaid(CarFileId, Convert.ToInt32(Session["UserId"]));
                        MsgTitle = "عملیات موفق";
                        Msg = "پرونده با موفقیت تایید شد.";
                        Er = 0; 
                        KhodEzhari = "0";
                    }
                    else
                    {
                        MsgTitle = "عملیات ناموفق";
                        Msg = "پرونده مورد نظر قبلا تایید شده است.";
                        Er = 1;
                        KhodEzhari = "0";
                    }
                }
                return Json(new { MsgTitle = MsgTitle, Msg = Msg, Er = Er, KhodEzhari = KhodEzhari }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                if (Session["savePath1"] != null)
                {
                    System.IO.File.Delete(Session["savePath1"].ToString());
                    Session.Remove("savePath1");
                }
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 }, JsonRequestBehavior.AllowGet);
            }
            
        }
        public ActionResult SaveVarizi(Models.sp_CollectionSelect Collection)
        {
            string MsgTitle = ""; string Msg = ""; byte Er = 0;
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {

                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Collection.fldDesc == null)
                    Collection.fldDesc = "";
                if (Collection.fldSerialBarChasb == null)
                    Collection.fldSerialBarChasb = "";
                if (Collection.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 291))
                    {
                        /*if (Collection.fldPeacockeryCode != null)*/
                        if (Collection.fldMunId == 0 || Collection.fldMunId == null)//پرداخت عادی
                        {
                            var fish = Car.sp_PeacockerySelect("fldId", Collection.fldPeacockeryCode.ToString(), 1,
                                Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            if (fish != null)
                            {
                                var carIdfish = Car.sp_CarFileSelect("fldId", fish.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarID;
                                var carIdcollection = Car.sp_CarFileSelect("fldId", Collection.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarID;
                                if (carIdfish == carIdcollection)
                                /*if (fish.fldCarFileID == Collection.fldCarFileID)*/
                                {
                                    var q = Car.sp_CollectionSelect("fldPeacockeryCode", Collection.fldPeacockeryCode.ToString(), 1,
                                        Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                    if (q == null)
                                    {
                                        System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                                        Car.sp_CollectionInsert(_id, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                            Collection.fldPrice, Collection.fldSettleTypeID, (int)Collection.fldPeacockeryCode, null,
                                             Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]),
                                             Collection.fldDesc, Session["UserPass"].ToString(), "", null, "", null, null, true, 1, DateTime.Now);
                                        SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));

                                        //ارسال به حسابداری شاهرود
                                        if (Convert.ToInt32(Session["UserMnu"]) == 1)
                                        {
                                            Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                                            var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, Collection.fldPeacockeryCode.ToString(), MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate), "");
                                            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                                    + k1 + "-" + Collection.fldPeacockeryCode.ToString() + "\n");
                                        }
                                        /*Car.SaveChanges();*/
                                        SmsSender sms = new SmsSender();
                                        sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, Collection.fldPrice, "", "", "");
                                        return Json(new { MsgTitle = "ذخیره موفق", Msg = "ذخیره با موفقیت انجام شد.", Er = Er });
                                    }
                                    else
                                    {
                                        return Json(new { MsgTitle = "خطا", Msg = "فیش فعلی قبلا در سیستم ثبت گردیده است.", Er = 1 });
                                    }
                                }
                                else
                                    return Json(new { MsgTitle = "خطا", Msg = "فیش فعلی مربوط به این پرونده نمی باشد.", Er = 1 });
                            }
                            else
                                return Json(new { MsgTitle = "خطا", Msg = "فیش فعلی در سیستم وجود ندارد.", Er = 1 });
                        }
                        else
                        {//پرداخت علی الحساب
                            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 318))
                            {
                                int? fileid = null;
                                var subSett = Car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), Car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                                bool? ForceScan = true;
                                if (subSett != null)
                                {
                                    ForceScan = subSett.fldHaveScan;
                                }
                                if (ForceScan == true && Session["savePath1"] == null)
                                {
                                    return Json(new { Msg = "لطفا فایل مدرک را آپلود کنید.", MsgTitle = "خطا", Er = 1 });
                                }
                                else if (Session["savePath1"] != null)
                                {
                                    System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                                    string savePath = Session["savePath1"].ToString();

                                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                    var Ex = Path.GetExtension(savePath);
                                    byte[] _File = stream.ToArray();

                                    Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
                                    System.IO.File.Delete(savePath);
                                    Session.Remove("savePath1");
                                    if (a.Value != null)
                                        fileid = Convert.ToInt32(a.Value);
                                    if (Collection.fldMunId == 0 || Collection.fldMunId == null)
                                        return Json(new { MsgTitle = "خطا", Msg = "لطفا شهرداری را مشخص کنید.", Er = 1 });
                                    if (Collection.fldSerialFish == "" || Collection.fldSerialFish == "0" || Collection.fldSerialFish == null)
                                        return Json(new { MsgTitle = "خطا", Msg = "لطفا شماره سریال فیش را مشخص کنید.", Er = 1 });
                                }
                                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", sizeof(int));
                                Car.sp_CollectionInsert(_id, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                    Collection.fldPrice, Collection.fldSettleTypeID, null, null,
                                        Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]),
                                        Collection.fldDesc, Session["UserPass"].ToString(), "",
                                        Collection.fldMunId, Collection.fldSerialFish, null, fileid, false, null, null);
                                if (Collection.fldMunId == Convert.ToInt32(Session["UserMnu"]))
                                    SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                                return Json(new { MsgTitle = "ذخیره موفق", Msg = "ذخیره با موفقیت انجام شد.", Er = Er });
                                ///else
                                //{
                                //    return Json(new { MsgTitle = "خطا", Msg = "لطفا فایل مدرک را آپلود کنید.", Er = 1 });
                                //}
                            }
                            else
                            {
                                if (Session["savePath1"] != null)
                                {
                                    System.IO.File.Delete(Session["savePath1"].ToString());
                                    Session.Remove("savePath1");
                                }
                                MsgTitle = "خطا";
                                Msg = "شما مجاز به دسترسی نمی باشید.";
                                Er = 1;
                            }
                        }
                    }
                    else
                    {
                        if (Session["savePath1"] != null)
                        {
                            System.IO.File.Delete(Session["savePath1"].ToString());
                            Session.Remove("savePath1");
                        }
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به دسترسی نمی باشید.";
                        Er = 1;
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 293))
                    {
                        if(Collection.fldAccept==true && Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 433)==false)
                        {
                            return Json(new { MsgTitle = "خطا", Msg = "واریزی مورد نظر تأیید شده و شما قادر به ویرایش آن نمی باشید.", Er = 1 });
                        }

                        if (Collection.fldMunId == 0 || Collection.fldMunId == null)//پرداخت عادی
                        {
                            var fish = Car.sp_PeacockerySelect("fldId", Collection.fldPeacockeryCode.ToString(), 1,
                               Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            if (fish != null)
                            {
                                var carIdfish = Car.sp_CarFileSelect("fldId", fish.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarID;
                                var carIdcollection = Car.sp_CarFileSelect("fldId", Collection.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarID;
                                if (carIdfish == carIdcollection)
                                /*if (fish.fldCarFileID == Collection.fldCarFileID)*/
                                {
                                    Car.sp_CollectionUpdate(Collection.fldID, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                        Collection.fldPrice, Collection.fldSettleTypeID, (int)Collection.fldPeacockeryCode, null,
                                        Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]), Collection.fldDesc,
                                        Session["UserPass"].ToString(), "", null, "", null, null);
                                    if (Convert.ToInt32(Session["UserMnu"]) == 1)
                                    {
                                        Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                                        toRevenueSystems.VitiationRecovrySendedFiche(3, 1, Collection.fldPeacockeryCode.ToString());
                                        var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, Collection.fldPeacockeryCode.ToString(), MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate), "");
                                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                               + k1 + "-" + Collection.fldPeacockeryCode.ToString() + "\n");
                                    }
                                    return Json(new { MsgTitle = "ویرایش موفق", Msg = "ویرایش با موفقیت انجام شد.", Er = Er });
                                }
                                else
                                    return Json(new { MsgTitle = "خطا", Msg = "فیش فعلی مربوط به این پرونده نمی باشد.", Er = 1 });
                            }
                            else if (Collection.fldPeacockeryCode == null && Collection.fldSettleTypeID == 10)
                            {
                                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 433))
                                {
                                    var qq = Car.sp_CollectionSelect("fldId", Collection.fldID.ToString(), 1, 1, "").FirstOrDefault();
                                    Car.sp_CollectionUpdate(Collection.fldID, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                            Collection.fldPrice, Collection.fldSettleTypeID, (int?)Collection.fldPeacockeryCode,Convert.ToInt64(qq.fldOnlinePaymentsId),
                                            Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]), Collection.fldDesc,
                                            Session["UserPass"].ToString(), "", null, "", null, null);

                                    if (Convert.ToInt32(Session["UserMnu"]) == 1)
                                    {
                                        Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                                        toRevenueSystems.VitiationRecovrySendedFiche(3, 1, qq.fldOnlinePaymentsId.ToString());
                                        var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, qq.fldOnlinePaymentsId.ToString(), MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate), "");
                                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                               + k1 + "-" + qq.fldOnlinePaymentsId.ToString() + "\n");
                                    }
                                    return Json(new { MsgTitle = "ویرایش موفق", Msg = "ویرایش با موفقیت انجام شد.", Er = Er });
                                }
                                else
                                {
                                    return Json(new { MsgTitle = "خطا", Msg = "امکان ویرایش وجود ندارد.", Er = 1 });
                                }
                            }
                            else
                                return Json(new { MsgTitle = "خطا", Msg = "فیش فعلی در سیستم وجود ندارد.", Er = 1 });
                        }
                        else
                        {
                            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 319))
                            {
                                int? fileid = null;
                                if (Session["savePath1"] != null)
                                {
                                    System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                                    string savePath = Session["savePath1"].ToString();

                                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));                                    
                                    var Ex = Path.GetExtension(savePath);
                                    byte[] _File = stream.ToArray();

                                    Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
                                    System.IO.File.Delete(savePath);

                                    if (a.Value != null)
                                        fileid = Convert.ToInt32(a.Value);
                                }
                                else if (Collection.fldFileId != null)
                                    fileid = Collection.fldFileId;
                                else
                                    return Json(new { MsgTitle = "خطا", Msg = "لطفا فایل مدرک را آپلود کنید.", Er = 1 });

                                if (Collection.fldMunId == 0 || Collection.fldMunId == null)
                                    return Json(new { MsgTitle = "خطا", Msg = "لطفا شهرداری را مشخص کنید.", Er = 1 });
                                if (Collection.fldSerialFish == "" || Collection.fldSerialFish == "0" || Collection.fldSerialFish == null)
                                    return Json(new { MsgTitle = "خطا", Msg = "لطفا شماره سریال فیش را مشخص کنید.", Er = 1 });
                                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", sizeof(int));
                                Car.sp_CollectionUpdate(Collection.fldID, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                    Collection.fldPrice, Collection.fldSettleTypeID, null, null,
                                     Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"])
                                     , Collection.fldDesc, Session["UserPass"].ToString(), "",
                                     Collection.fldMunId, Collection.fldSerialFish, null, fileid);

                                if (Collection.fldFileId != null && Session["savePath1"] != null)
                                    Car.Sp_FilesDelete(Collection.fldFileId);
                                Session.Remove("savePath1");
                                
                                return Json(new { MsgTitle = "ویرایش موفق", Msg = "ویرایش با موفقیت انجام شد.", Er = Er });
                            }
                            else
                            {
                                if (Session["savePath1"] != null)
                                {
                                    System.IO.File.Delete(Session["savePath1"].ToString());
                                    Session.Remove("savePath1");
                                }
                                MsgTitle = "خطا";
                                Msg = "شما مجاز به دسترسی نمی باشید.";
                                Er = 1;
                            }
                        }
                    }
                    else
                    {
                        if (Session["savePath1"] != null)
                        {
                            System.IO.File.Delete(Session["savePath1"].ToString());
                            Session.Remove("savePath1");
                        }
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به دسترسی نمی باشید.";
                        Er = 1;
                    }
                }
            }
            catch (Exception x)
            {
                if (Session["savePath1"] != null)
                {
                    System.IO.File.Delete(Session["savePath1"].ToString());
                    Session.Remove("savePath1");
                }
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteByCarFile(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                

                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 303))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    Car.sp_DeleteDuplicatedFishByCarFile(Convert.ToInt32(Session["UserId"]), id);
                    return Json(new { Msg = "حذف با موفقیت انجام شد.", MsgTitle = "حذف موفق", Err = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Msg ="شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Err = 1 }, JsonRequestBehavior.AllowGet);
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Err = 1 });
            }
        }
        
        public ActionResult GetAccount(int? cboCarMake)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var AccountType = car.sp_CarAccountTypeSelect("fldCarMakeID", cboCarMake.ToString(), 0, 0, "");
            return Json(AccountType.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCabin(int? cboCarAccountTypes)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities car = new Models.cartaxEntities();
            var CabinType = car.sp_CabinTypeSelect("fldCarAccountTypeID", cboCarAccountTypes.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CabinType.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSystem(int? cboCarCabin)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities car = new Models.cartaxEntities();
            var CarSystem = car.sp_CarSystemSelect("fldCabinTypeID", cboCarCabin.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CarSystem.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetModel(int? cboSystem)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities car = new Models.cartaxEntities();
            var CarModel = car.sp_CarModelSelect("fldCarSystemID", cboSystem.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CarModel.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClass(int? cboModel)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities car = new Models.cartaxEntities();
            var CarClass = car.sp_CarClassSelect("fldCarModelID", cboModel.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(CarClass.Select(p1 => new { ID = p1.fldID, Name = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetYear(int? Noo)
        {
            if (Noo == null)
                Noo = 1;
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Noo == 1)
            {
                for (int i = 1340; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = 1950; i <= DateTime.Now.Year + 1; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.OrderByDescending(l => l.Text).Select(p1 => new { ID = p1.Value, Name = p1.Text }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetColor()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_ColorCarSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { ID = c.fldID, Name = c.fldColor }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult getPictures(string CarFileId)
        {
            var ImgBargeSabz="";var ImageKart="";var ImageBackKart="";var ImageSanad="";
            Models.cartaxEntities car = new Models.cartaxEntities();
            System.Drawing.Image s1 = null;
            System.Drawing.Image s2 = null;
            System.Drawing.Image s3 = null;
            System.Drawing.Image s4 = null; 
            try
            {
                var file = car.sp_CarFileSelect("fldID", CarFileId.ToString(), 1, 0, "").FirstOrDefault();

                if (file.fldBargSabzFileId != null)
                {
                    var Image1=car.Sp_FilesSelect(file.fldBargSabzFileId).FirstOrDefault().fldImage;
                    ImgBargeSabz = Convert.ToBase64String(Image1);
                    s1=System.Drawing.Image.FromStream(new System.IO.MemoryStream(Image1));
                }
                else
                {
                    var Image1 = System.IO.File.ReadAllBytes( Server.MapPath(@"~/Content/Blank3.jpg"));
                    ImgBargeSabz = Convert.ToBase64String(Image1);
                    s1 = System.Drawing.Image.FromStream(new System.IO.MemoryStream(Image1));
                    file.fldBargSabzFileId = 0;
                }

                if (file.fldCartFileId != null)
                {
                    var Image2 = car.Sp_FilesSelect(file.fldCartFileId).FirstOrDefault().fldImage;
                    ImageKart = Convert.ToBase64String(Image2);
                    s2=System.Drawing.Image.FromStream(new System.IO.MemoryStream(Image2));
                }
                else
                {
                    var Image2 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/Blank3.jpg"));
                    ImageKart = Convert.ToBase64String(Image2);
                    s2 = System.Drawing.Image.FromStream(new System.IO.MemoryStream(Image2));
                    file.fldCartFileId = 0;
                }

                if (file.fldCartBackFileId != null)
                {
                    var Image3 = car.Sp_FilesSelect(file.fldCartBackFileId).FirstOrDefault().fldImage;
                    ImageBackKart = Convert.ToBase64String(Image3);
                    s3 = System.Drawing.Image.FromStream(new System.IO.MemoryStream(Image3));
                }
                else
                {
                    var Image3 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/Blank3.jpg"));
                    ImageBackKart = Convert.ToBase64String(Image3);
                    s3 = System.Drawing.Image.FromStream(new System.IO.MemoryStream(Image3));
                    file.fldCartBackFileId = 0;
                }

                if (file.fldSanadForoshFileId != null)
                {
                    var Image4 = car.Sp_FilesSelect(file.fldSanadForoshFileId).FirstOrDefault().fldImage;
                    ImageSanad = Convert.ToBase64String(Image4);
                    s4 = System.Drawing.Image.FromStream(new System.IO.MemoryStream(Image4));
                }
                else
                {
                    var Image4 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/Blank3.jpg"));
                    ImageSanad = Convert.ToBase64String(Image4);
                    s4 = System.Drawing.Image.FromStream(new System.IO.MemoryStream(Image4));
                    file.fldSanadForoshFileId = 0;
                }

                return new JsonResult()
                {
                    Data = new {
                        ImageSanad = ImageSanad,
                        ImageBackKart = ImageBackKart,
                        ImageKart = ImageKart,
                        ImgBargeSabz = ImgBargeSabz,
                        CartBackFileId = file.fldCartBackFileId,
                        SanadForoshFileId = file.fldSanadForoshFileId,
                        CartFileId = file.fldCartFileId,
                        BargSabzFileId = file.fldBargSabzFileId,
                        width1 = s1.Width,
                        height1 = s1.Height,
                        width2 = s2.Width,
                        height2 = s2.Height,
                        width3 = s3.Width,
                        height3 = s3.Height,
                        width4 = s4.Width,
                        height4 = s4.Height,
                        Er = 0
                    },
                    MaxJsonLength = Int32.MaxValue,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                /*return Json(new
                {
                    ImageSanad = ImageSanad,
                    ImageBackKart = ImageBackKart,
                    ImageKart = ImageKart,
                    ImgBargeSabz = ImgBargeSabz,
                    CartBackFileId = file.fldCartBackFileId,
                    SanadForoshFileId = file.fldSanadForoshFileId,
                    CartFileId = file.fldCartFileId,
                    BargSabzFileId = file.fldBargSabzFileId,
                    width1=s1.Width,
                    height1=s1.Height,
                    width2 = s2.Width,
                    height2 = s2.Height,
                    width3 = s3.Width,
                    height3 = s3.Height,
                    width4 = s4.Width,
                    height4 = s4.Height,
                    Er=0
                }, JsonRequestBehavior.AllowGet);*/
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
            
        }
        public ActionResult Fill(string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            try
            {
                //var Tree = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();

                //var UpTree = m.sp_SelectUpTreeCountryDivisions(Convert.ToInt32(Tree.fldID), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                var PosIPId = 0;

                //foreach (var item in UpTree.OrderByDescending(l=>l.fldNodeType))
                //{
                //var c = m.sp_GET_IDCountryDivisions(item.fldNodeType, item.fldSourceID).FirstOrDefault();
                var Pos = p.sp_PcPosInfoSelect("fldTreeId", "", Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), 0).FirstOrDefault();
                if (Pos != null)
                {
                    var UserPos = p.sp_PcPosUserSelect("fldIdUser", Session["UserId"].ToString(), 0).FirstOrDefault();
                    if (UserPos != null)
                    {
                        var PosIp = p.sp_PcPosIPSelect("fldId", UserPos.fldPosIPId.ToString(), 0).FirstOrDefault();
                        if (PosIp != null)
                        {
                            PosIPId = PosIp.fldId;
                            //break;
                        }
                    }
                }

                var file = p.sp_CarFileSelect("fldID", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                var car = p.sp_SelectCarDetilsByCarFileID(Convert.ToInt64(CarFileId)).FirstOrDefault();
                var caruser = p.sp_CarUserGuestSelect("fldCarFileId", CarFileId, "", 1).FirstOrDefault();
                if (caruser != null)
                {
                    p.sp_CachedSearchCarFileUpdate(caruser.fldMotorNumber, caruser.fldShasiNumber, caruser.fldVIN, caruser.fldModel, caruser.fldCarModelName, caruser.fldCarClassName,
                        caruser.fldCarFileId, Convert.ToInt64(Session["UserId"]), "", caruser.fldName, caruser.fldMelli_EconomicCode, caruser.fldPlaqueNumber,
                        caruser.fldCarAccountName, caruser.fldCarsystemName, caruser.fldColor, caruser.fldFuleTypeName, caruser.fldAccept, caruser.fldAcceptName, caruser.fldAccountTypeId);
                }
                var currentyear = p.sp_GetDate().FirstOrDefault().DateShamsi.ToString().Substring(0, 4);
                //p.sp_CachedSearchCarFileUpdate(car.fldMotorNumber, car.fldShasiNumber, car.fldVIN, car.fldModel,car.fldCarModel, car.fldCarClassName,
                //    Convert.ToInt64(CarFileId), Convert.ToInt64(Session["UserId"]), "", car.fldOwnerName, car.fldMelli_EconomicCode, car.fldCarPlaqueNumber,
                //    car.fldCarAccountName, car.fldCarSystemName, car.fldColor, car.fldFuelType, Convert.ToByte(file.fldAccept), ""
                //    , car.fldCarAccountTypeID);
                var owner = p.sp_OwnerSelect("fldid", car.fldOwnerID.ToString(), 0, 1, "").FirstOrDefault();
                return Json(new
                {
                    plaq = car.fldPlaquNumber,
                    fldPlaqueCityName = car.fldPlaqueCityName,
                    fldPlaqueSerial = car.fldPlaqueSerial,
                    fldCarPlaqueNumber = car.fldCarPlaqueNumber,
                    fldFuelType = car.fldFuelType,
                    fldCylinder_Wheel_Pivot = car.fldCylinder_Wheel_Pivot,
                    fldCarPlaqueIDD = car.fldCarPlaqueID,
                    CodeMeli = car.fldMelli_EconomicCode,
                    classs = car.fldCarClassName,
                    modell = car.fldCarModel,
                    syst = car.fldCarSystemName,
                    cabin = car.fldCarCabinName,
                    account = car.fldCarAccountName,
                    make = car.fldCarMakeName,
                    Malek = car.fldOwnerName,
                    mobile = owner.fldMobile,
                    motor = car.fldMotorNumber,
                    shasi = car.fldShasiNumber,
                    PosIPId = PosIPId,
                    vin = car.fldVIN,
                    color = car.fldColor,
                    date = car.fldStartDateInsurance,
                    datep = car.fldDatePlaque,
                    shamsiYear = car.fldStartDateInsurance.Substring(0, 4),
                    year = car.fldModel,
                    currentyear = currentyear,
                    carId = car.fldCarID,//carid       
                    fldBargSabzFileId = file.fldBargSabzFileId,
                    fldCartFileId = file.fldCartFileId,
                    fldSanadForoshFileId = file.fldSanadForoshFileId,
                    fldCartBackFileId = file.fldCartBackFileId,
                    fldOwnerID = car.fldOwnerID
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return null;
            }
            
        }
        public ActionResult ShowResid(int id,int Type)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.Type = Type;
            return PartialView;
        }
        public ActionResult RptResid(int id, int Type)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter resid = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter carDitail = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
            var q = car.rpt_Receipt(id, Type).FirstOrDefault();
            if (q == null)
                return null;
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();
            carDitail.Fill(dt.sp_SelectCarDetils, Convert.ToInt32(q.fldCarId));
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
            resid.Fill(dt.rpt_Receipt, id, Type);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_Resid.frx");
            Report.RegisterData(dt, "complicationsCarDBDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", "");
            Report.SetParameterValue("OfficeName", "");


            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")
            {
                Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
            }
            else if (ImageSetting == "2")
            {
                Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
            }
            else
                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            Report.Prepare();
            Report.Export(pdf, stream);
            return File(stream.ToArray(), "application/pdf");

        }

        public ActionResult CheckTaiid(string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var file = p.sp_CarFileSelect("fldid", CarFileId, 0, 1, "").FirstOrDefault();

            bool? fldIsAcceptSanad = null;
            var fldKarbarTaeedKonandeSanad = "";
            bool? fldIsAcceptCart = null;
            var fldKarbarTaeedKonandeCart = "";
            bool? fldIsAcceptBargSabz = null;
            var fldKarbarTaeedKonandeBargSabz = "";
            bool? fldIsAcceptCartBack = null;
            var fldKarbarTaeedKonandeCartBack = "";
            string Message = "";

            var f = p.Sp_FilesSelect(file.fldSanadForoshFileId).FirstOrDefault();
            if (f != null)
            {
                fldIsAcceptSanad = f.fldIsAccept;
                fldKarbarTaeedKonandeSanad = f.fldNameKarbar;
            }

            f = p.Sp_FilesSelect(file.fldBargSabzFileId).FirstOrDefault();
            if (f != null)
            {
                fldIsAcceptBargSabz = f.fldIsAccept;
                fldKarbarTaeedKonandeBargSabz = f.fldNameKarbar;
            }

            f = p.Sp_FilesSelect(file.fldCartFileId).FirstOrDefault();
            if (f != null)
            {
                fldIsAcceptCart = f.fldIsAccept;
                fldKarbarTaeedKonandeCart = f.fldNameKarbar;
            }

            f = p.Sp_FilesSelect(file.fldCartBackFileId).FirstOrDefault();
            if (f != null)
            {
                fldIsAcceptCartBack = f.fldIsAccept;
                fldKarbarTaeedKonandeCartBack = f.fldNameKarbar;
            }

            var HaveTaiid = false;
            if (fldKarbarTaeedKonandeSanad != "" || fldKarbarTaeedKonandeBargSabz != "" || fldKarbarTaeedKonandeCart != "" || fldKarbarTaeedKonandeCartBack != "")
            {
                HaveTaiid = true;
                Message = "پرونده مورد نظر تأیید شده است.";
            }
            else
            {
                Message = "پرونده مورد نظر تأیید نشده است.";
            }

            var HaveMafasa = false;
            var m = p.Sp_MafasaSelect(file.fldCarID).FirstOrDefault();
            if (m != null)
            {
                HaveMafasa = true;
                Message =Message+ " دارای مفاصا است.";
            }
            else
            {
                Message = Message + " مفاصا ندارد.";
            }

            var HaveSabeghe = false;
            var c = p.sp_CarExperienceSelect("fldCarFileID", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (c != null)
            {
                HaveSabeghe = true;
                Message = Message + " انتقال سابقه دارد.";
            }
            else
            {
                Message = Message + " انتقال سابقه ندارد.";
            }

            var HaveVarizi = false;
            var q = p.sp_CollectionSelect("fldCarFileID", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (q != null)
            {
                HaveVarizi = true;
                Message = Message + " واریزی دارد.";
            }
            else
            {
                Message = Message + " واریزی ندارد.";
            }

            var HaveFish = false;
            var fi = p.sp_PeacockerySelect("fldCarFileID", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (fi != null)
            {
                HaveFish = true;
                Message = Message + " فیش دارد.";
            }
            else
            {
                Message = Message + " فیش ندارد.";
            }
            return Json(new
            {
                HaveTaiid = HaveTaiid,
                HaveMafasa = HaveMafasa,
                HaveSabeghe = HaveSabeghe,
                HaveVarizi = HaveVarizi,
                HaveFish = HaveFish,
                userId = Session["UserId"].ToString(),
                Message = Message
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckBedehi(long CarFileId,string collectionDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities m = new Models.cartaxEntities();
            //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));

            int? mablagh = 0;
            bool allow=false;
            var currentTime = m.sp_GetDate().FirstOrDefault().Time;
            var datemiladi = MyLib.Shamsi.Shamsi2miladiDateTime(collectionDate);
            var DateTax = datemiladi + " " + currentTime;
            var file = m.sp_CarFileSelect("fldid", CarFileId.ToString(), 0, 1, "").FirstOrDefault();
            var car = m.sp_CarFileSelect("fldCarId", file.fldCarID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(CarFileId), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
            //    null, Convert.ToDateTime(DateTax) /*DateTime*/, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
            var bedehi = m.prs_newCarFileCalc(datemiladi, Convert.ToInt32(Session["CountryType"]),
               Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"])).Where(k => k.fldCollectionId == 0).ToList();
            string _year = "";
            if (bedehi != null)
            {
                var nullYears = bedehi.Where(k => k.fldPrice == null).ToList();
                foreach (var item in nullYears)
                {
                    _year += item.fldYear;
                }
            }
            foreach (var item in bedehi)
            {
                int? jam = (item.fldFinalPrice + item.fldMashmol + item.fldNoMashmol + item.fldMablaghJarime + item.fldOtherPrice) -
                        (item.fldDiscontJarimePrice + item.fldDiscontMoaserPrice + item.fldDiscontOtherPrice + item.fldDiscontValueAddPrice);
                mablagh += jam;
            }

            //mablagh += Convert.ToInt32(_Bed.Value);

            if (mablagh < 0 || mablagh == 0)
            {
                allow = true;
            }

            return Json(new
            {
                allow = allow,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult calc(string carFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            int State = 1;
            Models.cartaxEntities m = new Models.cartaxEntities();
            

            var DateTime = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
            var file = m.sp_CarFileSelect("fldid", carFileId, 0, 1, "").FirstOrDefault();
            var car = m.sp_CarFileSelect("fldCarId", file.fldCarID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime).Substring(0, 4));
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";

            //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                Transaction Tr = new Transaction();
                var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["UserId"]));
                string msg1 = "";

                switch (Result)
                {
                    case Transaction.TransactionResult.Fail:
                        msg1 = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.";
                        return Json(new
                        {
                            Er = 1,
                            msg = msg1,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);

                    case Transaction.TransactionResult.NotSharj:
                        msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
                        return Json(new
                        {
                            Er = 1,
                            msg = msg1,
                            MsgTitle = "خطا",
                        }, JsonRequestBehavior.AllowGet);

                }
            }

            //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
            //null, DateTime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
            var bedehi = m.prs_newCarFileCalc(DateTime, Convert.ToInt32(Session["CountryType"]),
                Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"])).Where(k => k.fldCollectionId == 0).ToList();
            string _year = "";
            if(bedehi!=null)
            {
                var nullYears=bedehi.Where(k=>k.fldPrice==null).ToList();
                foreach (var item in nullYears)
                {
                    _year += item.fldYear;
                }
            }
            if (_year.ToString() == "")
            {
                int? mablagh = 0;
                int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0, fldMainDiscount = 0, fldFineDiscount = 0,
                    fldValueAddDiscount = 0, fldOtherDiscount = 0;
                ArrayList Years = new ArrayList();
                DataSet.DataSet1.sp_jCalcCarFileDataTable a = new DataSet.DataSet1.sp_jCalcCarFileDataTable();
                foreach (var item in bedehi)
                {
                    int? jam=(item.fldFinalPrice + item.fldMashmol + item.fldNoMashmol + item.fldMablaghJarime+item.fldOtherPrice) -
                        (item.fldDiscontJarimePrice + item.fldDiscontMoaserPrice + item.fldDiscontOtherPrice + item.fldDiscontValueAddPrice);
                    mablagh += jam;
                    fldFine += (int)item.fldMablaghJarime;
                    fldValueAddPrice += (int)item.fldValueAdded;
                    fldPrice += (int)((item.fldFinalPrice - item.fldValueAdded) + item.fldMashmol + item.fldNoMashmol);
                    if (jam > 0)
                        Years.Add(item.fldYear);
                    fldOtherPrice += (int)item.fldOtherPrice;
                    fldMainDiscount += (int)item.fldDiscontMoaserPrice;
                    fldFineDiscount += (int)item.fldDiscontJarimePrice;
                    fldValueAddDiscount += (int)item.fldDiscontValueAddPrice;
                    fldOtherDiscount += (int)item.fldDiscontOtherPrice;
                    a.Addsp_jCalcCarFileRow((int)item.fldYear, (int)item.fldPrice, (int)item.fldMablaghMoaser, (int)item.fldValueAdded, (int)item.fldFinalPrice,
                       (int)item.fldMablaghJarime, (int)item.fldTedadJarime, (int)item.fldDiscontMoaserPrice, (int)jam, item.fldCalcDate, (int)item.fldOtherPrice, (int)item.fldDiscontValueAddPrice,
                       (int)item.fldDiscontJarimePrice, (int)item.fldDiscontOtherPrice);
                }
                int sal = 0, mah = 0;
                //mablagh += Convert.ToInt32(_Bed.Value);
                //fldPrice += Convert.ToInt32(_Bed.Value);
                /*mablagh += bedOrBes;
                fldPrice += bedOrBes;*/
                // Session["mablagh"] = mablagh;
                //Session["Fine"] = fldFine;
                // Session["ValueAddPrice"] = fldValueAddPrice;
                //Session["Price"] = fldPrice;
                Session["Year" + carFileId] = Years;
                //Session["Bed"] = Convert.ToInt32(_Bed.Value);
                Session["OtherPrice"] = fldOtherPrice;
                Session["fldMainDiscount"] = fldMainDiscount;
                Session["fldFineDiscount"] = fldFineDiscount;
                Session["fldValueAddDiscount"] = fldValueAddDiscount;
                Session["fldOtherDiscount"] = fldOtherDiscount;
                Session["Joziyat" + carFileId] = a;
                if (mablagh < 10000)
                {
                    mablagh = 0;
                    //bedehi = null;
                }
                string shGhabz = "", ShParvande = "",
                    shPardakht = "",
                    barcode = "";
                return Json(new
                {
                    bedehi = bedehi,
                    mablagh = mablagh,
                    shGhabz = shGhabz,
                    shPardakht = shPardakht,
                    barcode = barcode,
                    msg = "",
                    State = State,
                    fldFine = fldFine,
                    fldValueAddPrice = fldValueAddPrice,
                    fldPrice = fldPrice,
                    Years = Years,
                    Bed = 0,
                    fldOtherPrice = fldOtherPrice,
                    fldMainDiscount = fldMainDiscount,
                    fldFineDiscount = fldFineDiscount,
                    fldValueAddDiscount = fldValueAddDiscount,
                    fldOtherDiscount = fldOtherDiscount//,
                    // Joziyat=a
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                State = 2;
                string s = "", msg = "", year = _year.ToString();
                for (int i = 0; i < year.Length; i += 4)
                {
                    if (i < year.Length - 4)
                        s += year.Substring(i, 4) + " و ";
                    else
                        s += year.Substring(i, 4);
                }
                msg = "تعرفه سالهای " + s + " تعریف نشده است لطفا جهت اعلام به پشتیبان دکمه ارسال به پشتیبان را انتخاب و تا زمانی که نرخ توسط پشتیبان ثبت شود، منتظر بمانید، سپس دکمه دریافت از سرور را انتخاب کنید و پس از دریافت پیغام تایید، دکمه محاسبه مجدد را از قسمت صورت حساب انتخاب کنید.";
                return Json(new
                {

                    msg = msg,
                    Year = s.Replace(" و ", ","),
                    State = State
                }, JsonRequestBehavior.AllowGet);
            }
            //Session["year"] = _year.Value.ToString();
            //Session["bed"] = _Bed.Value.ToString();
        }

        //public ActionResult calc(string carFileId, int Yeartax)
        //{
        //    if (Session["UserId"] == null)
        //        return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
        //    int State = 1;
        //    Models.cartaxEntities m = new Models.cartaxEntities();
        //    var file = m.sp_CarFileSelect("fldid", carFileId, 0, 1, "").FirstOrDefault();

        //    var DateTime = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
        //    var car = m.sp_CarFileSelect("fldCarId", file.fldCarID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
        //    int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime).Substring(0, 4));
        //    string date = toYear + "/12/29";
        //    if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
        //        date = toYear + "/12/30";
        //    var DateTax = "";
        //    if (Yeartax != 0)
        //    {
        //        var currentTime = m.sp_GetDate().FirstOrDefault().Time;
        //        //var dattttte = m.sp_GetDate().FirstOrDefault().DateShamsi.Substring(4);
        //        var datemiladi = MyLib.Shamsi.Shamsi2miladiString(Yeartax + "/12/29");
        //        if (MyLib.Shamsi.Iskabise(Convert.ToInt32(Yeartax)))
        //        {
        //            datemiladi = MyLib.Shamsi.Shamsi2miladiString(Yeartax + "/12/30");
        //        }
        //        DateTax = datemiladi + " " + currentTime;
        //    }

        //    System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
        //    System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
        //    if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
        //    {
        //        Transaction Tr = new Transaction();
        //        var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
        //        var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
        //        var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["UserId"]));
        //        string msg1 = "";

        //        switch (Result)
        //        {
        //            case Transaction.TransactionResult.Fail:
        //                msg1 = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.";
        //                return Json(new
        //                {
        //                    Er = 1,
        //                    msg = msg1,
        //                    MsgTitle = "خطا"
        //                }, JsonRequestBehavior.AllowGet);

        //            case Transaction.TransactionResult.NotSharj:
        //                msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
        //                return Json(new
        //                {
        //                    Er = 1,
        //                    msg = msg1,
        //                    MsgTitle = "خطا",
        //                }, JsonRequestBehavior.AllowGet);

        //        }
        //    }
        //    var bedehi = new List<sp_jCalcCarFile_New>();
        //    var bedehi2 = new List<sp_jCalcCarFile_New>();
        //    int bedehi1 = 0;

        //    if (Yeartax == 0)
        //    {
        //        bedehi = m.sp_jCalcCarFile_New(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
        //    null, DateTime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
        //    }
        //    else
        //    {
        //        bedehi = m.sp_jCalcCarFile_New(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
        //    Convert.ToDateTime(DateTax), DateTime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
        //    }
        //    if (_year.Value.ToString() == "")
        //    {
        //        int? mablagh = 0;
        //        int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0, fldMainDiscount = 0, fldFineDiscount = 0,
        //            fldValueAddDiscount = 0, fldOtherDiscount = 0;
        //        ArrayList Years = new ArrayList();
        //        DataSet.DataSet1.sp_jCalcCarFileDataTable a = new DataSet.DataSet1.sp_jCalcCarFileDataTable();
        //        var q1 = bedehi.Where(k => k.fldVarizi != 0).ToList();
        //        if (q1.Count != 0)
        //        {
        //            bedehi1 = Convert.ToInt32(bedehi.Where(k => k.fldVarizi != 0).Max(j => j.fldCalcDate).ToString().Replace("/", ""));
        //            bedehi2 = bedehi.Where(k => Convert.ToInt32(k.fldCalcDate.Replace("/", "")) > bedehi1).ToList();
        //            var bedehi3 = bedehi.Where(k => Convert.ToInt32(k.fldCalcDate.Replace("/", "")) <= bedehi1).ToList();
        //            foreach (var item in bedehi2)
        //            {
        //                mablagh += item.fldDept;
        //                fldFine += (int)item.fldFine;
        //                fldValueAddPrice += (int)item.fldValueAdded;
        //                fldPrice += (int)item.fldCurectPrice;
        //                Years.Add(item.fldyear);
        //                fldOtherPrice += (int)item.OtherPrice;
        //                fldMainDiscount += (int)item.fldDiscount;
        //                fldFineDiscount += (int)item.fldFineDiscount;
        //                fldValueAddDiscount += (int)item.fldValueAddDiscount;
        //                fldOtherDiscount += (int)item.fldOtherDiscount;
        //                a.Addsp_jCalcCarFileRow((int)item.fldyear, (int)item.fldFirstPrice, (int)item.fldCurectPrice, (int)item.fldValueAdded, (int)item.fldFinalPrice,
        //                    (int)item.fldFine, (int)item.fldCountMonth, (int)item.fldDiscount, (int)item.fldDept, item.fldCalcDate, (int)item.OtherPrice, (int)item.fldValueAddDiscount,
        //                    (int)item.fldFineDiscount, (int)item.fldOtherDiscount);
        //            }
        //            int bedOrBes = 0;
        //            foreach (var item in bedehi3)
        //            {
        //                bedOrBes += Convert.ToInt32(item.fldFinalBed - item.fldFinalBes);
        //            }
        //            mablagh += bedOrBes;
        //            fldPrice += bedOrBes;
        //        }
        //        else
        //        {
        //            bedehi2 = bedehi;
        //            foreach (var item in bedehi)
        //            {
        //                mablagh += item.fldDept;
        //                fldFine += (int)item.fldFine;
        //                fldValueAddPrice += (int)item.fldValueAdded;
        //                fldPrice += (int)item.fldCurectPrice;
        //                Years.Add(item.fldyear);
        //                fldOtherPrice += (int)item.OtherPrice;
        //                fldMainDiscount += (int)item.fldDiscount;
        //                fldFineDiscount += (int)item.fldFineDiscount;
        //                fldValueAddDiscount += (int)item.fldValueAddDiscount;
        //                fldOtherDiscount += (int)item.fldOtherDiscount;
        //                a.Addsp_jCalcCarFileRow((int)item.fldyear, (int)item.fldFirstPrice, (int)item.fldCurectPrice, (int)item.fldValueAdded, (int)item.fldFinalPrice,
        //                    (int)item.fldFine, (int)item.fldCountMonth, (int)item.fldDiscount, (int)item.fldDept, item.fldCalcDate, (int)item.OtherPrice, (int)item.fldValueAddDiscount,
        //                    (int)item.fldFineDiscount, (int)item.fldOtherDiscount);
        //            }
        //        }
        //        int sal = 0, mah = 0;
        //        //mablagh += Convert.ToInt32(_Bed.Value);
        //        //fldPrice += Convert.ToInt32(_Bed.Value);
        //        /*mablagh += bedOrBes;
        //        fldPrice += bedOrBes;*/
        //        // Session["mablagh"] = mablagh;
        //        //Session["Fine"] = fldFine;
        //        // Session["ValueAddPrice"] = fldValueAddPrice;
        //        //Session["Price"] = fldPrice;
        //        Session["Year" + carFileId] = Years;
        //        Session["Bed"] = Convert.ToInt32(_Bed.Value);
        //        Session["OtherPrice"] = fldOtherPrice;
        //        Session["fldMainDiscount"] = fldMainDiscount;
        //        Session["fldFineDiscount"] = fldFineDiscount;
        //        Session["fldValueAddDiscount"] = fldValueAddDiscount;
        //        Session["fldOtherDiscount"] = fldOtherDiscount;
        //        Session["Joziyat" + carFileId] = a;
        //        var mablagh1 = mablagh;
        //        if (mablagh < 10000)
        //        {
        //            mablagh = 0;
        //            //bedehi = null;
        //        }
        //        string shGhabz = "", ShParvande = "",
        //            shPardakht = "",
        //            barcode = "";
        //        return Json(new
        //        {
        //            bedehi = bedehi2,
        //            mablagh = mablagh,
        //            mablagh1 = mablagh1,
        //            shGhabz = shGhabz,
        //            shPardakht = shPardakht,
        //            barcode = barcode,
        //            msg = "",
        //            State = State,
        //            fldFine = fldFine,
        //            fldValueAddPrice = fldValueAddPrice,
        //            fldPrice = fldPrice,
        //            Years = Years,
        //            Bed = Convert.ToInt32(_Bed.Value),
        //            fldOtherPrice = fldOtherPrice,
        //            fldMainDiscount = fldMainDiscount,
        //            fldFineDiscount = fldFineDiscount,
        //            fldValueAddDiscount = fldValueAddDiscount,
        //            fldOtherDiscount = fldOtherDiscount//,
        //            // Joziyat=a
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        State = 2;
        //        string s = "", msg = "", year = _year.Value.ToString();
        //        for (int i = 0; i < year.Length; i += 4)
        //        {
        //            if (i < year.Length - 4)
        //                s += year.Substring(i, 4) + " و ";
        //            else
        //                s += year.Substring(i, 4);
        //        }
        //        msg = "تعرفه سالهای " + s + " تعریف نشده است لطفا جهت اعلام به پشتیبان دکمه ارسال به پشتیبان را انتخاب و تا زمانی که نرخ توسط پشتیبان ثبت شود، منتظر بمانید، سپس دکمه دریافت از سرور را انتخاب کنید و پس از دریافت پیغام تایید، دکمه محاسبه مجدد را از قسمت صورت حساب انتخاب کنید.";
        //        return Json(new
        //        {

        //            msg = msg,
        //            Year = s.Replace(" و ", ","),
        //            State = State
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    //Session["year"] = _year.Value.ToString();
        //    //Session["bed"] = _Bed.Value.ToString();
        //}

        public ActionResult SendToSupporter(string msg, string Year, int CarClassId, string carFileID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Models.cartaxEntities p = new Models.cartaxEntities();
            string fldCarClassName = "";
            string fldCarModelName = "";
            string fldCarSystemName = "";
            string fldCabinTypeName = "";
            string fldCarAccountTypeName = "";
            string fldCarMakeName = "";

            if (CarClassId == 0)
            {
                var file = p.sp_CarFileSelect("fldid", carFileID, 0, 1, "").FirstOrDefault();
                var car = p.sp_SelectCarDetils(file.fldCarID).FirstOrDefault();
                fldCarClassName = car.fldCarClassName;
                fldCarModelName = car.fldCarModel;
                fldCarSystemName = car.fldCarSystemName;
                fldCabinTypeName = car.fldCarCabinName;
                fldCarAccountTypeName = car.fldCarAccountName;
                fldCarMakeName = car.fldCarMakeName;
            }
            else
            {
                var q =p.sp_CarClassSelect("fldId",CarClassId.ToString(),1,1,"").FirstOrDefault();
                fldCarClassName = q.fldName;
                fldCarModelName = q.fldCarModelName;
                var q1= p.sp_CarModelSelect("fldId", q.fldCarModelID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                fldCarSystemName = q1.fldCarSystemName;
                var k = p.sp_CarSystemSelect("fldId", q1.fldCarSystemID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                fldCabinTypeName = k.fldCabinTypeName;
                fldCarAccountTypeName = k.fldCarAccountType;
                fldCarMakeName = k.fldCarMake;
            }
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.msg = msg;
            PartialView.ViewBag.fldCarClassId = fldCarClassName;
            PartialView.ViewBag.fldCarModelId = fldCarModelName;
            PartialView.ViewBag.fldCarSystemId = fldCarSystemName;
            PartialView.ViewBag.fldCabinTypeId = fldCabinTypeName;
            PartialView.ViewBag.fldCarAccountTypeId = fldCarAccountTypeName;
            PartialView.ViewBag.fldCarMakeId = fldCarMakeName;
            return PartialView;
        }
        public string calcu(long carid)
        {

            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
            var DateTime = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
            var car = m.sp_CarFileSelect("fldCarId", carid.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime).Substring(0, 4));
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";
            //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            //System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                Transaction Tr = new Transaction();
                var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["UserId"]));
            }
            //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
            //      null, DateTime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
            var bedehi = m.prs_newCarFileCalc(DateTime.Now, Convert.ToInt32(Session["CountryType"]),
                Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"])).Where(k => k.fldCollectionId == 0).ToList();
            string _year = "";
            if (bedehi != null)
            {
                var nullYears = bedehi.Where(k => k.fldPrice == null).ToList();
                foreach (var item in nullYears)
                {
                    _year += item.fldYear;
                }
            }
            if (_year.ToString() == "")
            {
                int? mablagh = 0;
                int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0, fldMainDiscount = 0, fldFineDiscount = 0,
                    fldValueAddDiscount = 0, fldOtherDiscount = 0;
                ArrayList Years = new ArrayList();
                DataSet.DataSet1.sp_jCalcCarFileDataTable a = new DataSet.DataSet1.sp_jCalcCarFileDataTable();

                foreach (var item in bedehi)
                {
                    int? jam = (item.fldFinalPrice + item.fldMashmol + item.fldNoMashmol + item.fldMablaghJarime + item.fldOtherPrice) -
                        (item.fldDiscontJarimePrice + item.fldDiscontMoaserPrice + item.fldDiscontOtherPrice + item.fldDiscontValueAddPrice);
                    mablagh += jam;
                    fldFine += (int)item.fldMablaghJarime;
                    fldValueAddPrice += (int)item.fldValueAdded;
                    fldPrice += (int)((item.fldFinalPrice - item.fldValueAdded) + item.fldMashmol + item.fldNoMashmol); 
                    Years.Add(item.fldYear);
                    fldOtherPrice += (int)item.fldOtherPrice;
                    fldMainDiscount += (int)item.fldDiscontMoaserPrice;
                    fldFineDiscount += (int)item.fldDiscontJarimePrice;
                    fldValueAddDiscount += (int)item.fldDiscontValueAddPrice;
                    fldOtherDiscount += (int)item.fldDiscontOtherPrice;
                    a.Addsp_jCalcCarFileRow((int)item.fldYear, (int)item.fldPrice, (int)item.fldMablaghMoaser, (int)item.fldValueAdded, (int)item.fldFinalPrice,
                        (int)item.fldMablaghJarime, (int)item.fldTedadJarime, (int)item.fldDiscontMoaserPrice, (int)jam, item.fldCalcDate, (int)item.fldOtherPrice, (int)item.fldDiscontValueAddPrice,
                        (int)item.fldDiscontJarimePrice, (int)item.fldDiscontOtherPrice);
                }

                int sal = 0, mah = 0;
                //mablagh += Convert.ToInt32(_Bed.Value);
                //fldPrice += Convert.ToInt32(_Bed.Value);
                // Session["mablagh"] = mablagh;
                //Session["Fine"] = fldFine;
                // Session["ValueAddPrice"] = fldValueAddPrice;
                //Session["Price"] = fldPrice;
                Session["Year"] = Years;
                //Session["Bed"] = Convert.ToInt32(_Bed.Value);
                Session["OtherPrice"] = fldOtherPrice;
                Session["fldMainDiscount"] = fldMainDiscount;
                Session["fldFineDiscount"] = fldFineDiscount;
                Session["fldValueAddDiscount"] = fldValueAddDiscount;
                Session["fldOtherDiscount"] = fldOtherDiscount;
                Session["Joziyat"] = a;
                if (mablagh < 10000)
                {
                    mablagh = 0;
                    //bedehi = null;
                }

                string shGhabz = "", ShParvande = "",
                    shPardakht = "",
                    barcode = "";
                //return Json(new
                //{
                //    bedehi = bedehi,
                //    mablagh = mablagh,
                //    shGhabz = shGhabz,
                //    shPardakht = shPardakht,
                //    barcode = barcode,
                //    msg = "",

                //    fldFine = fldFine,
                //    fldValueAddPrice = fldValueAddPrice,
                //    fldPrice = fldPrice,
                //    Years = Years,
                //    Bed = Convert.ToInt32(_Bed.Value),
                //    fldOtherPrice = fldOtherPrice,
                //    fldMainDiscount = fldMainDiscount,
                //    fldFineDiscount = fldFineDiscount,
                //    fldValueAddDiscount = fldValueAddDiscount,
                //    fldOtherDiscount = fldOtherDiscount//,
                //    // Joziyat=a
                //}, JsonRequestBehavior.AllowGet);
                return mablagh.ToString();
            }
            return "";
        }


        public ActionResult GeneratePDFSooratVaziyat(long carId)
        {
            //if (Session["UserId"] == null)
            //    return RedirectToAction("logon", "Account_New");
            try
            {
                Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter sp_SelectCarDetils = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter sp_Receipt = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter sp_CarExperience = new Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.Sp_AcceptStatusTableAdapter sp_AcceptStatus = new Avarez.DataSet.DataSet1TableAdapters.Sp_AcceptStatusTableAdapter();
                var Accept = m.sp_SelectCarDetils(carId).FirstOrDefault();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                sp_SelectCarDetils.Fill(dt.sp_SelectCarDetils, carId);
                sp_Receipt.Fill(dt.rpt_Receipt, carId, 2);
                sp_AcceptStatus.Fill(dt.Sp_AcceptStatus, Convert.ToInt32(Accept.fldID));
                sp_CarExperience.Fill(dt.sp_CarExperienceSelect, "fldCarID", carId.ToString(), 0, 1, "");
                var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_SooratVaziyat.frx");
                Report.RegisterData(dt, "carTaxDataSet");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                var Mablagh = calcu(carId);
                var mandeHesab = decimal.Parse(Mablagh).ToString("#,#");
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("Mablagh", mandeHesab);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                Report.RegisterData(dt, "DataSet1");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
              
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GeneratePDFPicCar(int FileID)
        {
            try
            {
                Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
                var file = m.Sp_FilesSelect(FileID).FirstOrDefault().fldImage;
                return File(file.ToArray(), ".jpg");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrintSooratVaziyat(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult NewSooratVaziyat(string CarFileId)
        {//باز شدن پنجره
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            //PartialView.ViewBag.Id = car.fldCarID;
            //return PartialView;
            ViewBag.Id = car.fldCarID;
            return View();
        }
        public ActionResult NewSooratVaziyat_Win(string CarFileId)
        {//باز شدن پنجره
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = car.fldCarID;
            return PartialView;
        }
        
        public ActionResult PrintJoziyatAvarezTab(string CarFileId)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            //PartialView.ViewBag.Id = car.fldCarID;
            //return PartialView;
            ViewBag.Id = car.fldCarID;
            return View();
        }

        public ActionResult PrintAllJoziyatAvarezTab(string CarFileId, short Year,int Price)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            //PartialView.ViewBag.Id = car.fldCarID;
            //return PartialView;
            ViewBag.Id = car.fldCarID;
            ViewBag.Year = Year;
            ViewBag.Price = Price;
            return View();
        }
        public ActionResult printJoziyatAvarezWin(string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            //PartialView.ViewBag.Id = car.fldCarID;
            //return PartialView;
            PartialView.ViewBag.Id = car.fldCarID;
            return PartialView;
        }

        public ActionResult PrintAllJoziyatAvarezWin(string CarFileId, short Year,int Price)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            //PartialView.ViewBag.Id = car.fldCarID;
            //return PartialView;
            PartialView.ViewBag.Id = car.fldCarID;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Price = Price;
            return PartialView;
        }
        public ActionResult RptJoziyatAvarez(long carid)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->جزئیات محاسبات");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            Models.cartaxEntities m = new Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldCarId", carid.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.prs_newCarFileCalcTableAdapter jCalcCarFile = new Avarez.DataSet.DataSet1TableAdapters.prs_newCarFileCalcTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter CarDetils = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter();

            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());

            Nullable<int> _Bed = new int();
            String _year = "";
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                Transaction Tr = new Transaction();
                var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(carid), Convert.ToInt32(Session["UserId"]));
                string msg1 = "";
                switch (Result)
                {
                    case Transaction.TransactionResult.Fail:
                        msg1 = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1
                        }, JsonRequestBehavior.AllowGet);

                    case Transaction.TransactionResult.NotSharj:
                        msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1
                        }, JsonRequestBehavior.AllowGet);

                }
            }
            //jCalcCarFile.Fill(dt.sp_jCalcCarFile, (int)car.fldID, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null, null, DateTime.Now, Convert.ToInt32(Session["UserId"]), ref _year, ref _Bed);
            jCalcCarFile.Fill(dt.prs_newCarFileCalc, DateTime.Now, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), car.fldCarID.ToString(), Convert.ToInt32(Session["UserId"]));
            CarDetils.Fill(dt.sp_SelectCarDetils, carid);
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptNewDetails.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("Parameter", decimal.Parse(_Bed.Value.ToString()).ToString("#,#"));
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("OfficeName", Session["office"].ToString());
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")
            {
                Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
            }
            else if (ImageSetting == "2")
            {
                Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
            }
            else
                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);
            return File(stream.ToArray(), "application/pdf");
        }

        public ActionResult RptAllJoziyatAvarez(long carid,short Year,int Price)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "گزارشات کاربردی->جزئیات محاسبات");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            Models.cartaxEntities m = new Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldCarId", carid.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            string Yeartax = Year.ToString();
            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.prs_newCarFileCalcTableAdapter jCalcCarFile_New = new Avarez.DataSet.DataSet1TableAdapters.prs_newCarFileCalcTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter CarDetils = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter();

            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());

            Nullable<int> _Bed = new int();
            String _year = "";
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                Transaction Tr = new Transaction();
                var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["UserId"]));
                string msg1 = "";
                switch (Result)
                {
                    case Transaction.TransactionResult.Fail:
                        msg1 = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1
                        }, JsonRequestBehavior.AllowGet);

                    case Transaction.TransactionResult.NotSharj:
                        msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1
                        }, JsonRequestBehavior.AllowGet);

                }
            }
            if (Year == 0)
            {
                jCalcCarFile_New.Fill(dt.prs_newCarFileCalc, DateTime.Now, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), car.fldID.ToString(), Convert.ToInt32(Session["UserId"]));
                Yeartax = m.sp_GetDate().FirstOrDefault().DateShamsi.Substring(0, 4);
            }
            else
            {
                var currentTime = m.sp_GetDate().FirstOrDefault().Time;
                var datemiladi = MyLib.Shamsi.Shamsi2miladiString(Year + "/12/29");
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(Year)))
                {
                    datemiladi = MyLib.Shamsi.Shamsi2miladiString(Year + "/12/30");
                }
                var DateTax = datemiladi + " " + currentTime;
                //jCalcCarFile_New.Fill(dt.sp_jCalcCarFile_New, (int)car.fldID, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null, Convert.ToDateTime(DateTax), DateTime.Now, Convert.ToInt32(Session["UserId"]), ref _year, ref _Bed);
                jCalcCarFile_New.Fill(dt.prs_newCarFileCalc, DateTime.Now, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), car.fldID.ToString(),Convert.ToInt32(Session["UserId"]));
            }

            CarDetils.Fill(dt.sp_SelectCarDetils, carid);
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            //Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptCompeletedDetils.frx");
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptNewDetails.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("Mablagh1", Price);
            Report.SetParameterValue("Year", Yeartax);
            Report.SetParameterValue("OfficeName", Session["office"].ToString());
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")
            {
                Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
            }
            else if (ImageSetting == "2")
            {
                Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
            }
            else
                Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);
            return File(stream.ToArray(), "application/pdf");
        }
        public ActionResult PrintResidDaftarTab(string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            ViewBag.Id = car.fldCarID;
            return View();
        }
        public ActionResult PrintResidDaftarWin(string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            var car = m.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = car.fldCarID;
            return PartialView;
        }
        public ActionResult OfficeRecipt(long id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 249))
            //{
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_SelectCarDetils(id).FirstOrDefault();
            if (car != null)
            {

                int? mablagh = p.sp_tblOfficeReciptNerkhSelect(6, car.fldCarClassID, 5, Convert.ToInt32(Session["UserMnu"])).FirstOrDefault().fldPrice;

                if (mablagh > 0)
                {
                    var Cdate = p.sp_GetDate().FirstOrDefault();
                    int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Cdate.CurrentDateTime).Substring(0, 4));
                    string date = toYear + "/12/29";
                    if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                        date = toYear + "/12/30";

                    System.Data.Entity.Core.Objects.ObjectParameter residId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterDate = new System.Data.Entity.Core.Objects.ObjectParameter("fldTarikh", typeof(DateTime));
                    System.Data.Entity.Core.Objects.ObjectParameter LetterNum = new System.Data.Entity.Core.Objects.ObjectParameter("fldShomare", typeof(string));
                    p.Sp_tblOfficeReciptInsert(residId, LetterDate, Convert.ToInt32(Session["UserId"]), car.fldCarID, mablagh, null, LetterNum);

                    Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                    Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.Sp_tblOfficeReciptSelectTableAdapter Receipt = new Avarez.DataSet.DataSet1TableAdapters.Sp_tblOfficeReciptSelectTableAdapter();
                    sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                    Receipt.Fill(dt.Sp_tblOfficeReciptSelect, "fldid", residId.Value.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                    Avarez.Models.cartaxEntities Car = new Avarez.Models.cartaxEntities();
                    Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                    var mnu = Car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    var State = Car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();

                    string barcode = WebConfigurationManager.AppSettings["SiteURL"] + "/OfficeRecipt/Get/" + residId.Value;
                    
                    FastReport.Report Report = new FastReport.Report();
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_OfficeRecipt.frx");
                    Report.RegisterData(dt, "dataSet1");
                    Report.SetParameterValue("date", /*MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(*/LetterDate.Value.ToString()/*))*/);
                    //var time = Convert.ToDateTime(LetterDate.Value);
                    var time = Car.sp_GetDate().FirstOrDefault().Time;
                    Report.SetParameterValue("time", time);
                    //Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                    Report.SetParameterValue("Num", LetterNum.Value);
                    Report.SetParameterValue("barcode", barcode);
                    Report.SetParameterValue("MunicipalityName", mnu.fldName);
                    Report.SetParameterValue("StateName", State.fldName);
                    Report.SetParameterValue("AreaName", Session["area"].ToString());
                    Report.SetParameterValue("OfficeName", Session["office"].ToString());
                    Report.SetParameterValue("sal", toYear.ToString().Substring(0, 4));
                    var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                    if (ImageSetting == "1")
                    {
                        Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                    }
                    else if (ImageSetting == "2")
                    {
                        Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                    }
                    else
                        Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                    Report.SetParameterValue("UserName", Car.sp_UserSelect("fldid", Session["UserId"].ToString(), 0, ""
                    , Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldUserName);
                    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                    MemoryStream stream = new MemoryStream();
                    Report.Prepare();
                    Report.Export(pdf, stream);
                    p.Sp_tblOfficeReciptUpdate(residId.Value.ToString(), stream.ToArray());
                    return File(stream.ToArray(), "application/pdf");
                }
                else
                    return Json("کاربر گرامی شما در حال انجام عملیات غیر قانونی می باشید و در صورت تکرار آی پی شما به پلیس فتا اعلام خواهد شد. باتشکر", JsonRequestBehavior.AllowGet);
            }
            else
                return null;
            //}
            //else
            //{
            //    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
            //    return RedirectToAction("error", "Metro");
            //}
        }


        public ActionResult CheckExistFish(long carFileid, int showmoney)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carF = p.sp_CarFileSelect("fldId", carFileid.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var carid = carF.fldCarID;
            
            var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
            string MohlatDate = "";
            byte roundNumber = 0;
            var ServerDate = p.sp_GetDate().FirstOrDefault();
            if (SubSetting != null)
            {
                if (SubSetting.fldLastRespitePayment > 0)
                {
                    MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                }
                else if (SubSetting.fldLastRespitePayment == 0)
                {
                    string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                    int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                    int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                    int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                    if (Mounth <= 6)
                        MohlatDate = Year + "/" + Mounth + "/31";
                    else if (Mounth > 6 && Mounth < 12)
                        MohlatDate = Year + "/" + Mounth + "/30";
                    else if (MyLib.Shamsi.Iskabise(Year) == true)
                        MohlatDate = Year + "/" + Mounth + "/30";
                    else
                        MohlatDate = Year + "/" + Mounth + "/29";
                    //if
                }
                var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                roundNumber = Round.fldRound;
            }


            double Rounded = 10;
            switch (roundNumber)
            {
                case 3:
                    Rounded = 1000;
                    break;
                case 2:
                    Rounded = 100;
                    break;
                case 0:
                    Rounded = 1;
                    break;
            }


            showmoney = Convert.ToInt32(Math.Floor(showmoney / Rounded) * Rounded);//گرد به پایین
            var q = p.sp_SelectExistPeacockery(carid, showmoney).FirstOrDefault();
            if (q != null)
                if (q.PeacockeryId != null)
                {
                    var t = p.sp_PeacockerySelect("fldId", q.PeacockeryId.ToString(), 1, 1, "").FirstOrDefault();
                    var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    if (bankid != null)
                    {
                        if (bankid.fldID == t.fldAccountBankID)
                            return Json(new { PeacockeryId = q.PeacockeryId, state = 1 }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { PeacockeryId = 0, state = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { PeacockeryId = 0, state = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { PeacockeryId = 0, state = 0 }, JsonRequestBehavior.AllowGet);
            return Json(new { PeacockeryId = 0, state = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckExistMafasa(int carId,int carFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            string Year = p.sp_GetDate().FirstOrDefault().DateShamsi.Substring(0, 4);
            var mafasa = p.sp_ExistMafasa(carId, Convert.ToInt32(Session["UserMnu"])).FirstOrDefault();
            if (Session["IsOfficeUser"] != null)
            {
                var resipt = p.Sp_tblPishkhanResiptSelect("fldCarFileIdInCurrentYear", carFileId.ToString()).FirstOrDefault();
                if (resipt == null)
                {
                    return Json(new { state = 2 }, JsonRequestBehavior.AllowGet);/*صدور/عدم صدور رسید*/
                }
            }
            if (mafasa != null)
            {
                return Json(new { MafasaId = mafasa.fldId, state = 1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { MafasaId = 0, state = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult _Mafasa(string mafasaId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var Mafasa = p.Sp_MafasaSelect_Image(mafasaId.ToString()).FirstOrDefault();
            if (Mafasa != null)
            {
                return File(Mafasa.fldimage, "application/pdf");
            }
            return null;
        }
        public ActionResult FishReport(string CarFileId, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            {
                Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
                ViewBag.CarFileId = CarFileId;
                ViewBag.mablagh = mablagh;
                ViewBag.fldFine = fldFine;
                ViewBag.fldValueAddPrice = fldValueAddPrice;
                ViewBag.fldPrice = fldPrice;
                ViewBag.Bed = Bed;
                ViewBag.Years = Years;
                ViewBag.fldOtherPrice = fldOtherPrice;
                ViewBag.fldMainDiscount = fldMainDiscount;
                ViewBag.fldFineDiscount = fldFineDiscount;
                ViewBag.fldValueAddDiscount = fldValueAddDiscount;
                ViewBag.fldOtherDiscount = fldOtherDiscount;
                return View();
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                }
                 );
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult FishReportWin(string CarFileId, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
                PartialView.ViewBag.CarFileId = CarFileId;
                PartialView.ViewBag.mablagh = mablagh;
                PartialView.ViewBag.fldFine = fldFine;
                PartialView.ViewBag.fldValueAddPrice = fldValueAddPrice;
                PartialView.ViewBag.fldPrice = fldPrice;
                PartialView.ViewBag.Bed = Bed;
                PartialView.ViewBag.Years = Years;
                PartialView.ViewBag.fldOtherPrice = fldOtherPrice;
                PartialView.ViewBag.fldMainDiscount = fldMainDiscount;
                PartialView.ViewBag.fldFineDiscount = fldFineDiscount;
                PartialView.ViewBag.fldValueAddDiscount = fldValueAddDiscount;
                PartialView.ViewBag.fldOtherDiscount = fldOtherDiscount;
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
                }
                 );
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult GenerateFishReport(long CarFileId, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            //if (Session["UserId"] == null)
            //    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            //{
            Models.cartaxEntities p = new Models.cartaxEntities();
            var file = p.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            var car = p.sp_SelectCarDetils(file.fldCarID).FirstOrDefault();
            if (car != null)
            {
                var ServerDate = p.sp_GetDate().FirstOrDefault();
                int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime).Substring(0, 4));
                //string date = toYear + "/12/29";
                //if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                //    date = toYear + "/12/30";
                System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));

                //car.fldID, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null, null, DateTime.Now, Convert.ToInt32(Session["UserId"]), ref _year, ref _Bed
                int sal = 0, mah = 0;
                //double mablagh = 0;
                //int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0,
                //    fldMainDiscount = 0, fldFineDiscount = 0, fldValueAddDiscount = 0, fldOtherDiscount = 0;
                //mablagh = Convert.ToInt32(Session["mablagh"]);
                //fldFine = Convert.ToInt32(Session["Fine"]);
                //fldValueAddPrice = Convert.ToInt32(Session["ValueAddPrice"]);
                //fldPrice = Convert.ToInt32(Session["Price"]);
                //fldOtherPrice = Convert.ToInt32(Session["OtherPrice"]);
                //fldMainDiscount = Convert.ToInt32(Session["fldMainDiscount"]);
                //fldFineDiscount = Convert.ToInt32(Session["fldFineDiscount"]);
                //fldValueAddDiscount = Convert.ToInt32(Session["fldValueAddDiscount"]);
                //fldOtherDiscount = Convert.ToInt32(Session["fldOtherDiscount"]);

                //ArrayList Years = new ArrayList();
                //foreach (var item in bedehi)
                //{
                //    fldFine += (int)item.fldFine;
                //    fldValueAddPrice += (int)item.fldValueAdded;
                //    fldPrice += (int)item.fldCurectPrice;
                //    //fldOtherPrice += (int)item.fldOtherPrice;
                //    mablagh += (int)item.fldDept;
                //    Years.Add(item.fldyear);
                //}
                /* ArrayList*/
                Years = (ArrayList)Session["Year" + CarFileId];
                for (int i = 0; i < Years.Count; i++)
                {
                    if (Convert.ToInt32(Years[i]) == 0)
                    {
                        Years.Remove(Years[i]);
                        break;
                    }
                }

                int[] AvarezSal = new int[0];
                if (Years != null)
                {
                    AvarezSal = new int[Years.Count];
                    for (int i = 0; i < Years.Count; i++)
                    {
                        AvarezSal[i] = Convert.ToInt32(Years[i]);
                    }
                }

                mablagh += Convert.ToInt32(_Bed.Value);
                fldPrice += Convert.ToInt32(_Bed.Value);
                if (mablagh < 10000)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "پرونده انتخابی بدهکار نیست."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                    //Session["ER"] = "پرونده انتخابی بدهکار نیست.";
                    //return RedirectToAction("error", "Metro", new { area = "" });
                }

                var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                //p.sp_selectAccountBank(Convert.ToInt32(Session["UserMnu"]), true).FirstOrDefault();

                System.Data.Entity.Core.Objects.ObjectParameter _CountryID = new System.Data.Entity.Core.Objects.ObjectParameter("ID", typeof(long));

                var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
                string MohlatDate = "";
                int roundNumber = 0;
                if (SubSetting != null)
                {
                    if (SubSetting.fldLastRespitePayment > 0)
                    {
                        MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                    }
                    else if (SubSetting.fldLastRespitePayment == 0)
                    {
                        string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                        int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                        int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                        int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                        if (Years.Count > 1)
                        {
                            if (Mounth <= 6)
                                MohlatDate = Year + "/" + Mounth + "/31";
                            else if (Mounth > 6 && Mounth < 12)
                                MohlatDate = Year + "/" + Mounth + "/30";
                            else if (MyLib.Shamsi.Iskabise(Year) == true)
                                MohlatDate = Year + "/" + Mounth + "/30";
                            else
                                MohlatDate = Year + "/" + Mounth + "/29";
                        }
                        else if (Years.Count == 1)
                        {
                            if (MyLib.Shamsi.Iskabise(Year) == true)
                                MohlatDate = Year + "/" + 12 + "/30";
                            else
                                MohlatDate = Year + "/" + 12 + "/29";
                        }
                        //if
                    }
                    var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                    roundNumber = Round.fldRound;
                }

                double Rounded = 10;
                switch (roundNumber)
                {
                    case 3:
                        Rounded = 1000;
                        break;
                    case 2:
                        Rounded = 100;
                        break;
                    case 0:
                        Rounded = 1;
                        break;
                }


                mablagh = Math.Floor(mablagh / Rounded) * Rounded;//گرد به پایین

                string ShGhabz = "", ShPardakht = "", BarcodeText = "", ShParvande = "";

                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                var datetime = p.sp_GetDate().FirstOrDefault().CurrentDateTime;
                p.sp_PeacockeryInsert(car.fldID, datetime, bankid.fldID, "", Convert.ToInt32(fldPrice),
                    Convert.ToInt32(fldFine), fldValueAddPrice, fldOtherPrice, Convert.ToInt32(mablagh),
                    MyLib.Shamsi.Shamsi2miladiDateTime(car.fldStartDateInsurance), datetime, Convert.ToInt32(Session["UserId"]),
                    "", Session["UserPass"].ToString(), fldMainDiscount, fldValueAddDiscount, fldOtherDiscount, ShGhabz, ShPardakht, _id, fldFineDiscount);
                if (Convert.ToInt32(Session["CountryType"]) == 5)
                {
                    var mnu = p.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, 1, "").FirstOrDefault();
                    if (mnu.fldInformaticesCode == "")
                        mnu.fldInformaticesCode = "0";
                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                    {
                        var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                            sal = ShParvande.Length - 2;
                            if (ShParvande.Length > 8)
                            {
                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                ShParvande = ShParvande.Substring(0, 8);
                                mah = Convert.ToInt32(s);
                            }
                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                , Convert.ToInt32(mablagh), sal, mah);
                            ShGhabz = gh.ShGhabz;
                            ShPardakht = gh.ShPardakht;
                            BarcodeText = gh.BarcodeText;
                        }
                    }
                }
                else if (Convert.ToInt32(Session["CountryType"]) == 6)
                {
                    var local = p.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, 1, "").FirstOrDefault();
                    if (local.fldSourceInformatics == "")
                        local.fldSourceInformatics = "0";
                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                    {
                        var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                            sal = ShParvande.Length - 2;
                            if (ShParvande.Length > 8)
                            {
                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                ShParvande = ShParvande.Substring(0, 8);
                                mah = Convert.ToInt32(s);
                            }
                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                , Convert.ToInt32(mablagh), sal, mah);
                            ShGhabz = gh.ShGhabz;
                            ShPardakht = gh.ShPardakht;
                            BarcodeText = gh.BarcodeText;
                        }
                    }
                }
                else if (Convert.ToInt32(Session["CountryType"]) == 7)
                {
                    var area = p.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, 1, "").FirstOrDefault();
                    if (area != null)
                    {
                        if (area.fldLocalID != null)
                        {
                            var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, 1, "").FirstOrDefault();

                            if (local.fldSourceInformatics == "")
                                local.fldSourceInformatics = "0";
                            if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                            {
                                var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                    sal = ShParvande.Length - 2;
                                    if (ShParvande.Length > 8)
                                    {
                                        string s = ShParvande.Substring(8).PadRight(2, '0');
                                        ShParvande = ShParvande.Substring(0, 8);
                                        mah = Convert.ToInt32(s);
                                    }
                                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                        , Convert.ToInt32(mablagh), sal, mah);
                                    ShGhabz = gh.ShGhabz;
                                    ShPardakht = gh.ShPardakht;
                                    BarcodeText = gh.BarcodeText;
                                }
                            }
                        }
                        else
                        {
                            var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, 1, "").FirstOrDefault();
                            if (mnu.fldInformaticesCode == "")
                                mnu.fldInformaticesCode = "0";
                            if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                            {
                                var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                    sal = ShParvande.Length - 2;
                                    if (ShParvande.Length > 8)
                                    {
                                        string s = ShParvande.Substring(8).PadRight(2, '0');
                                        ShParvande = ShParvande.Substring(0, 8);
                                        mah = Convert.ToInt32(s);
                                    }
                                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                        , Convert.ToInt32(mablagh), sal, mah);
                                    ShGhabz = gh.ShGhabz;
                                    ShPardakht = gh.ShPardakht;
                                    BarcodeText = gh.BarcodeText;
                                }
                            }
                        }
                    }
                }
                else if (Convert.ToInt32(Session["CountryType"]) == 8)
                {
                    var office = p.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, 1, "").FirstOrDefault();
                    if (office != null)
                    {
                        if (office.fldAreaID != null)
                        {
                            var area = p.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, 1, "").FirstOrDefault();
                            if (area != null)
                            {
                                if (area.fldLocalID != null)
                                {
                                    var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, 1, "").FirstOrDefault();

                                    if (local.fldSourceInformatics == "")
                                        local.fldSourceInformatics = "0";
                                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                                , Convert.ToInt32(mablagh), sal, mah);
                                            ShGhabz = gh.ShGhabz;
                                            ShPardakht = gh.ShPardakht;
                                            BarcodeText = gh.BarcodeText;
                                        }
                                    }
                                }
                                else
                                {
                                    var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, 1, "").FirstOrDefault();
                                    if (mnu.fldInformaticesCode == "")
                                        mnu.fldInformaticesCode = "0";
                                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                                , Convert.ToInt32(mablagh), sal, mah);
                                            ShGhabz = gh.ShGhabz;
                                            ShPardakht = gh.ShPardakht;
                                            BarcodeText = gh.BarcodeText;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var local = p.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, 1, "").FirstOrDefault();
                            if (local != null)
                            {
                                if (local.fldSourceInformatics == "")
                                    local.fldSourceInformatics = "0";
                                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                            , Convert.ToInt32(mablagh), sal, mah);
                                        ShGhabz = gh.ShGhabz;
                                        ShPardakht = gh.ShPardakht;
                                        BarcodeText = gh.BarcodeText;
                                    }
                                }
                            }
                            else
                            {
                                var mnu = p.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, 1, "").FirstOrDefault();
                                if (mnu.fldInformaticesCode == "")
                                    mnu.fldInformaticesCode = "0";
                                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                            , Convert.ToInt32(mablagh), sal, mah);
                                        ShGhabz = gh.ShGhabz;
                                        ShPardakht = gh.ShPardakht;
                                        BarcodeText = gh.BarcodeText;
                                    }
                                }
                            }
                        }
                    }
                }
                Avarez.DataSet.DataSet1TableAdapters.QueriesTableAdapter Queries = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                Queries.PeacokeryUpdate(ShGhabz, ShPardakht, Convert.ToInt64(_id.Value));

                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();

                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter sp_pic1 = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter();
                DataSet.DataSet1.sp_jCalcCarFileDataTable joziat = new DataSet.DataSet1.sp_jCalcCarFileDataTable();
                joziat = (DataSet.DataSet1.sp_jCalcCarFileDataTable)Session["Joziyat" + CarFileId];
                var k = p.sp_CarFileSelect("fldCarId", file.fldCarID.ToString(), 1, 1, "").FirstOrDefault();

                foreach (var item in joziat)
                {
                    dt.sp_jCalcCarFile.Addsp_jCalcCarFileRow((int)item.fldyear, (int)item.fldFirstPrice, (int)item.fldCurectPrice, (int)item.fldValueAdded, (int)item.fldFinalPrice,
                   (int)item.fldFine, (int)item.fldCountMonth, (int)item.fldDiscount, (int)item.fldDept, item.fldCalcDate, (int)item.OtherPrice, (int)item.fldValueAddDiscount,
                   (int)item.fldFineDiscount, (int)item.fldOtherDiscount);
                }
                SmsSender sendsms = new SmsSender();
                sendsms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 4, car.fldID, 0, "", "", "");

                sp_pic1.Fill(dt.sp_PictureSelect1, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                fish.Fill(dt.sp_PeacockerySelect, "fldId", _id.Value.ToString(), 1, 1, "");
                var _fish = p.sp_PeacockerySelect("fldId", _id.Value.ToString(), 1, 1, "").FirstOrDefault();

                sp_pic.Fill(dt.sp_PictureSelect, "fldBankPic", _fish.fldBankId.ToString(), 1, 1, "");
                var mnu1 = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                var UpReportSelect = p.sp_UpReportSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var FishReport = p.sp_ReportsSelect("fldId", UpReportSelect.fldReportsID.ToString(), 1, 1, "").FirstOrDefault();
                System.IO.MemoryStream Stream = new System.IO.MemoryStream(FishReport.fldReport);
                //int _Bedehkar = Convert.ToInt32(Session["Bed"]);
                int _Bedehkar = Bed;
                var State = p.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                Report.Load(Stream);
                Report.RegisterData(dt, "carTaxDataSet");
                Report.SetParameterValue("MunicipalityName", mnu1.fldName);
                Report.SetParameterValue("Barcode", BarcodeText);
                Report.SetParameterValue("ShGhabz", ShGhabz);
                Report.SetParameterValue("ShPardakht", ShPardakht);
                Report.SetParameterValue("SalAvarez", "(" + arrang(AvarezSal) + ")");

                //var bedeighabl = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                //null, DateTime.Now, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
                //Report.SetParameterValue("Bed", decimal.Parse(_Bed.Value.ToString()).ToString("#,#"));

                Report.SetParameterValue("Mohlat", MohlatDate);
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("Parameter", _Bedehkar);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);

                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);

                p.Sp_Peacockery_CopyInsert(Convert.ToInt64(_id.Value), stream.ToArray());
                //ارسال به حسابداری شاهرود
                if (Convert.ToInt32(Session["UserMnu"]) == 1)
                {
                    XmlDocument XDoc1 = new XmlDocument();
                    // Create root node.
                    XmlElement XElemRoot1 = XDoc1.CreateElement("FicheDetailByFisyear");
                    XDoc1.AppendChild(XElemRoot1);
                    XmlDocument XDoc2 = new XmlDocument();
                    // Create root node.
                    XmlElement XElemRoot2 = XDoc2.CreateElement("FicheAddAndSub");
                    XDoc2.AppendChild(XElemRoot2);
                    XmlDocument XDoc = new XmlDocument();
                    // Create root node.
                    XmlElement XElemRoot = XDoc.CreateElement("FicheDetail");
                    XDoc.AppendChild(XElemRoot);
                    int discount = 0;
                    Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                    string hesabid = "";
                    System.Xml.XmlReader xmlReader = System.Xml.XmlReader
                        .Create((Stream)new MemoryStream(Encoding.UTF8.GetBytes("<hesabs>" + new Avarez.Hesabrayan.ServiseToRevenueSystems().AccountListRevenue(1).InnerXml.Replace("\"", "'") + "</hesabs>")));
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Node")
                        {
                            if (bankid.fldAccountNumber == xmlReader["AccountNum"].ToString())
                                hesabid = xmlReader["ID"].ToString();
                        }
                    }

                    if (hesabid != "")
                    {
                        int avarez = 0, jarime = 0, sayer = 0, takhfif = 0;
                        avarez = fldPrice + fldValueAddPrice;
                        jarime = Convert.ToInt32(fldFine);
                        sayer = fldOtherPrice;
                        takhfif = fldMainDiscount;
                        XmlElement Xsource = XDoc.CreateElement("Node");
                        Xsource.SetAttribute("RevenueID", "198");//کد درامدی عوارض خودرو 
                        Xsource.SetAttribute("RevenueCost", avarez.ToString());
                        Xsource.SetAttribute("RevenueTaxCost", "0");
                        Xsource.SetAttribute("RevenueAvarezCost", "0");
                        Xsource.SetAttribute("RevenueTaxAvarezCost", "0");
                        Xsource.SetAttribute("RevenueBed", "0");
                        Xsource.SetAttribute("RevenueBes", "0");
                        Xsource.SetAttribute("AmountSavingBand", "0");
                        Xsource.SetAttribute("Discount", takhfif.ToString());

                        XElemRoot.AppendChild(Xsource);

                        if (jarime != 0)
                        {
                            XmlElement Xsource1 = XDoc.CreateElement("Node");
                            Xsource1.SetAttribute("RevenueID", "158");//کد در امدی جرائم
                            Xsource1.SetAttribute("RevenueCost", jarime.ToString());
                            Xsource1.SetAttribute("RevenueTaxCost", "0");
                            Xsource1.SetAttribute("RevenueAvarezCost", "0");
                            Xsource1.SetAttribute("RevenueTaxAvarezCost", "0");
                            Xsource1.SetAttribute("RevenueBed", "0");
                            Xsource1.SetAttribute("RevenueBes", "0");
                            Xsource1.SetAttribute("AmountSavingBand", "0");
                            Xsource1.SetAttribute("Discount", "0");

                            XElemRoot.AppendChild(Xsource1);
                        }
                        if (sayer != 0)
                        {
                            XmlElement Xsource1 = XDoc.CreateElement("Node");
                            Xsource1.SetAttribute("RevenueID", "197");//کد در امدی سایر
                            Xsource1.SetAttribute("RevenueCost", sayer.ToString());
                            Xsource1.SetAttribute("RevenueTaxCost", "0");
                            Xsource1.SetAttribute("RevenueAvarezCost", "0");
                            Xsource1.SetAttribute("RevenueTaxAvarezCost", "0");
                            Xsource1.SetAttribute("RevenueBed", "0");
                            Xsource1.SetAttribute("RevenueBes", "0");
                            Xsource1.SetAttribute("AmountSavingBand", "0");
                            Xsource1.SetAttribute("Discount", "0");

                            XElemRoot.AppendChild(Xsource1);
                        }

                        var k1 = toRevenueSystems.RegisterNewFicheByAccYearCostAndDiscount(3, 1, _id.Value.ToString(), car.fldOwnerName.ToString(),
                                 datetime, hesabid, _id.Value.ToString(),
                                 "کد ملی:" + car.fldMelli_EconomicCode.ToString() + " پلاک:" + car.fldCarPlaqueNumber.ToString() + " بابت عوارض " + arrang(AvarezSal),
                                  "", "", "", "", "", 8, 2, car.fldCarAccountName + " " + car.fldCarSystemName + " " + car.fldCarModel + " " + car.fldCarClassName, car.fldVIN.ToString(),
                                 (int)mablagh, 0, discount, XDoc, XDoc1, XDoc2);
                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal avarez: "
                            + k1 + "-" + _id.Value.ToString() + "\n");
                    }
                }
                return File(stream.ToArray(), "application/pdf");
            }
            return null;
            //}
            //else
            //{
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.ERROR,
            //        Title = "خطا",
            //        Message = "شما مجاز به دسترسی نمی باشید."
            //    }
            //     );
            //    DirectResult result = new DirectResult();
            //    return result;
            //}
        }

        public ActionResult DownloadFish(long CarFileId, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            //if (Session["UserId"] == null)
            //    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            //{
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var file = p.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            var car = p.sp_SelectCarDetils(file.fldCarID).FirstOrDefault();
            if (car != null)
            {
                var ServerDate = p.sp_GetDate().FirstOrDefault();
                int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime).Substring(0, 4));
                //string date = toYear + "/12/29";
                //if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                //    date = toYear + "/12/30";
                System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                //var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                //null, ServerDate.CurrentDateTime, _year, _Bed).ToList();
                int sal = 0, mah = 0;
                //double mablagh = 0;
                //int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0,
                //    fldMainDiscount = 0, fldFineDiscount = 0, fldValueAddDiscount = 0, fldOtherDiscount = 0;
                //mablagh = Convert.ToInt32(Session["mablagh"]);
                //fldFine = Convert.ToInt32(Session["Fine"]);
                //fldValueAddPrice = Convert.ToInt32(Session["ValueAddPrice"]);
                //fldPrice = Convert.ToInt32(Session["Price"]);
                //fldOtherPrice = Convert.ToInt32(Session["OtherPrice"]);
                //fldMainDiscount = Convert.ToInt32(Session["fldMainDiscount"]);
                //fldFineDiscount = Convert.ToInt32(Session["fldFineDiscount"]);
                //fldValueAddDiscount = Convert.ToInt32(Session["fldValueAddDiscount"]);
                //fldOtherDiscount = Convert.ToInt32(Session["fldOtherDiscount"]);

                //ArrayList Years = new ArrayList();
                //foreach (var item in bedehi)
                //{
                //    fldFine += (int)item.fldFine;
                //    fldValueAddPrice += (int)item.fldValueAdded;
                //    fldPrice += (int)item.fldCurectPrice;
                //    //fldOtherPrice += (int)item.fldOtherPrice;
                //    mablagh += (int)item.fldDept;
                //    Years.Add(item.fldyear);
                //}
                /* ArrayList*/
                Years = (ArrayList)Session["Year" + CarFileId];
                for (int i = 0; i < Years.Count; i++)
                {
                    if ((int)Years[i] == 0)
                    {
                        Years.Remove(Years[i]);
                        break;
                    }
                }

                int[] AvarezSal = new int[0];
                if (Years != null)
                {
                    AvarezSal = new int[Years.Count];
                    for (int i = 0; i < Years.Count; i++)
                    {
                        AvarezSal[i] = (int)Years[i];
                    }
                }

                mablagh += Convert.ToInt32(_Bed.Value);
                fldPrice += Convert.ToInt32(_Bed.Value);
                if (mablagh < 10000)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "پرونده انتخابی بدهکار نیست."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                    //Session["ER"] = "پرونده انتخابی بدهکار نیست.";
                    //return RedirectToAction("error", "Metro", new { area = "" });
                }

                var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                //p.sp_selectAccountBank(Convert.ToInt32(Session["UserMnu"]), true).FirstOrDefault();

                System.Data.Entity.Core.Objects.ObjectParameter _CountryID = new System.Data.Entity.Core.Objects.ObjectParameter("ID", typeof(long));

                var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
                string MohlatDate = "";
                int roundNumber = 0;
                if (SubSetting != null)
                {
                    if (SubSetting.fldLastRespitePayment > 0)
                    {
                        MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                    }
                    else if (SubSetting.fldLastRespitePayment == 0)
                    {
                        string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                        int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                        int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                        int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                        if (Years.Count > 1)
                        {
                            if (Mounth <= 6)
                                MohlatDate = Year + "/" + Mounth + "/31";
                            else if (Mounth > 6 && Mounth < 12)
                                MohlatDate = Year + "/" + Mounth + "/30";
                            else if (MyLib.Shamsi.Iskabise(Year) == true)
                                MohlatDate = Year + "/" + Mounth + "/30";
                            else
                                MohlatDate = Year + "/" + Mounth + "/29";
                        }
                        else if (Years.Count == 1)
                        {
                            if (MyLib.Shamsi.Iskabise(Year) == true)
                                MohlatDate = Year + "/" + 12 + "/30";
                            else
                                MohlatDate = Year + "/" + 12 + "/29";
                        }
                        //if
                    }
                    var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                    roundNumber = Round.fldRound;
                }

                double Rounded = 10;
                switch (roundNumber)
                {
                    case 3:
                        Rounded = 1000;
                        break;
                    case 2:
                        Rounded = 100;
                        break;
                    case 0:
                        Rounded = 1;
                        break;
                }


                mablagh = Math.Floor(mablagh / Rounded) * Rounded;//گرد به پایین

                string ShGhabz = "", ShPardakht = "", BarcodeText = "", ShParvande = "";

                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                var datetime = p.sp_GetDate().FirstOrDefault().CurrentDateTime;
                p.sp_PeacockeryInsert(car.fldID, datetime, bankid.fldID, "", Convert.ToInt32(fldPrice),
                    Convert.ToInt32(fldFine), fldValueAddPrice, fldOtherPrice, Convert.ToInt32(mablagh),
                    MyLib.Shamsi.Shamsi2miladiDateTime(car.fldStartDateInsurance), datetime, Convert.ToInt32(Session["UserId"]),
                    "", Session["UserPass"].ToString(), fldMainDiscount, fldValueAddDiscount, fldOtherDiscount, ShGhabz, ShPardakht, _id, fldFineDiscount);
                if (Convert.ToInt32(Session["CountryType"]) == 5)
                {
                    var mnu = p.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, 1, "").FirstOrDefault();
                    if (mnu.fldInformaticesCode == "")
                        mnu.fldInformaticesCode = "0";
                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                    {
                        var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                            sal = ShParvande.Length - 2;
                            if (ShParvande.Length > 8)
                            {
                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                ShParvande = ShParvande.Substring(0, 8);
                                mah = Convert.ToInt32(s);
                            }
                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                , Convert.ToInt32(mablagh), sal, mah);
                            ShGhabz = gh.ShGhabz;
                            ShPardakht = gh.ShPardakht;
                            BarcodeText = gh.BarcodeText;
                        }
                    }
                }
                else if (Convert.ToInt32(Session["CountryType"]) == 6)
                {
                    var local = p.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, 1, "").FirstOrDefault();
                    if (local.fldSourceInformatics == "")
                        local.fldSourceInformatics = "0";
                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                    {
                        var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        if (Divisions != null)
                        {
                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                            sal = ShParvande.Length - 2;
                            if (ShParvande.Length > 8)
                            {
                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                ShParvande = ShParvande.Substring(0, 8);
                                mah = Convert.ToInt32(s);
                            }
                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                , Convert.ToInt32(mablagh), sal, mah);
                            ShGhabz = gh.ShGhabz;
                            ShPardakht = gh.ShPardakht;
                            BarcodeText = gh.BarcodeText;
                        }
                    }
                }
                else if (Convert.ToInt32(Session["CountryType"]) == 7)
                {
                    var area = p.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, 1, "").FirstOrDefault();
                    if (area != null)
                    {
                        if (area.fldLocalID != null)
                        {
                            var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, 1, "").FirstOrDefault();

                            if (local.fldSourceInformatics == "")
                                local.fldSourceInformatics = "0";
                            if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                            {
                                var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                    sal = ShParvande.Length - 2;
                                    if (ShParvande.Length > 8)
                                    {
                                        string s = ShParvande.Substring(8).PadRight(2, '0');
                                        ShParvande = ShParvande.Substring(0, 8);
                                        mah = Convert.ToInt32(s);
                                    }
                                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                        , Convert.ToInt32(mablagh), sal, mah);
                                    ShGhabz = gh.ShGhabz;
                                    ShPardakht = gh.ShPardakht;
                                    BarcodeText = gh.BarcodeText;
                                }
                            }
                        }
                        else
                        {
                            var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, 1, "").FirstOrDefault();
                            if (mnu.fldInformaticesCode == "")
                                mnu.fldInformaticesCode = "0";
                            if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                            {
                                var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                if (Divisions != null)
                                {
                                    var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                    sal = ShParvande.Length - 2;
                                    if (ShParvande.Length > 8)
                                    {
                                        string s = ShParvande.Substring(8).PadRight(2, '0');
                                        ShParvande = ShParvande.Substring(0, 8);
                                        mah = Convert.ToInt32(s);
                                    }
                                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                        , Convert.ToInt32(mablagh), sal, mah);
                                    ShGhabz = gh.ShGhabz;
                                    ShPardakht = gh.ShPardakht;
                                    BarcodeText = gh.BarcodeText;
                                }
                            }
                        }
                    }
                }
                else if (Convert.ToInt32(Session["CountryType"]) == 8)
                {
                    var office = p.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, 1, "").FirstOrDefault();
                    if (office != null)
                    {
                        if (office.fldAreaID != null)
                        {
                            var area = p.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, 1, "").FirstOrDefault();
                            if (area != null)
                            {
                                if (area.fldLocalID != null)
                                {
                                    var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, 1, "").FirstOrDefault();

                                    if (local.fldSourceInformatics == "")
                                        local.fldSourceInformatics = "0";
                                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                                , Convert.ToInt32(mablagh), sal, mah);
                                            ShGhabz = gh.ShGhabz;
                                            ShPardakht = gh.ShPardakht;
                                            BarcodeText = gh.BarcodeText;
                                        }
                                    }
                                }
                                else
                                {
                                    var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, 1, "").FirstOrDefault();
                                    if (mnu.fldInformaticesCode == "")
                                        mnu.fldInformaticesCode = "0";
                                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                                , Convert.ToInt32(mablagh), sal, mah);
                                            ShGhabz = gh.ShGhabz;
                                            ShPardakht = gh.ShPardakht;
                                            BarcodeText = gh.BarcodeText;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var local = p.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, 1, "").FirstOrDefault();
                            if (local != null)
                            {
                                if (local.fldSourceInformatics == "")
                                    local.fldSourceInformatics = "0";
                                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                            , Convert.ToInt32(mablagh), sal, mah);
                                        ShGhabz = gh.ShGhabz;
                                        ShPardakht = gh.ShPardakht;
                                        BarcodeText = gh.BarcodeText;
                                    }
                                }
                            }
                            else
                            {
                                var mnu = p.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, 1, "").FirstOrDefault();
                                if (mnu.fldInformaticesCode == "")
                                    mnu.fldInformaticesCode = "0";
                                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, 1, "").FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                            , Convert.ToInt32(mablagh), sal, mah);
                                        ShGhabz = gh.ShGhabz;
                                        ShPardakht = gh.ShPardakht;
                                        BarcodeText = gh.BarcodeText;
                                    }
                                }
                            }
                        }
                    }
                }
                Avarez.DataSet.DataSet1TableAdapters.QueriesTableAdapter Queries = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                Queries.PeacokeryUpdate(ShGhabz, ShPardakht, Convert.ToInt64(_id.Value));

                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();

                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter sp_pic1 = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter();
                DataSet.DataSet1.sp_jCalcCarFileDataTable joziat = new DataSet.DataSet1.sp_jCalcCarFileDataTable();
                joziat = (DataSet.DataSet1.sp_jCalcCarFileDataTable)Session["Joziyat" + CarFileId];
                var k = p.sp_CarFileSelect("fldCarId", file.fldCarID.ToString(), 1, 1, "").FirstOrDefault();

                foreach (var item in joziat)
                {
                    dt.sp_jCalcCarFile.Addsp_jCalcCarFileRow((int)item.fldyear, (int)item.fldFirstPrice, (int)item.fldCurectPrice, (int)item.fldValueAdded, (int)item.fldFinalPrice,
                   (int)item.fldFine, (int)item.fldCountMonth, (int)item.fldDiscount, (int)item.fldDept, item.fldCalcDate, (int)item.OtherPrice, (int)item.fldValueAddDiscount,
                   (int)item.fldFineDiscount, (int)item.fldOtherDiscount);
                }
                SmsSender sendsms = new SmsSender();
                sendsms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 4, car.fldID, 0, "", "", "");

                sp_pic1.Fill(dt.sp_PictureSelect1, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                fish.Fill(dt.sp_PeacockerySelect, "fldId", _id.Value.ToString(), 1, 1, "");
                var _fish = p.sp_PeacockerySelect("fldId", _id.Value.ToString(), 1, 1, "").FirstOrDefault();

                sp_pic.Fill(dt.sp_PictureSelect, "fldBankPic", _fish.fldBankId.ToString(), 1, 1, "");
                var mnu1 = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                var UpReportSelect = p.sp_UpReportSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var FishReport = p.sp_ReportsSelect("fldId", UpReportSelect.fldReportsID.ToString(), 1, 1, "").FirstOrDefault();
                System.IO.MemoryStream Stream = new System.IO.MemoryStream(FishReport.fldReport);
                //int _Bedehkar = Convert.ToInt32(Session["Bed"]);
                int _Bedehkar = Bed;
                var State = p.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                Report.Load(Stream);
                Report.RegisterData(dt, "carTaxDataSet");
                Report.SetParameterValue("MunicipalityName", mnu1.fldName);
                Report.SetParameterValue("Barcode", BarcodeText);
                Report.SetParameterValue("ShGhabz", ShGhabz);
                Report.SetParameterValue("ShPardakht", ShPardakht);
                Report.SetParameterValue("SalAvarez", "(" + arrang(AvarezSal) + ")");
                Report.SetParameterValue("Mohlat", MohlatDate);
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("Parameter", _Bedehkar);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);

                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);

                p.Sp_Peacockery_CopyInsert(Convert.ToInt64(_id.Value), stream.ToArray());
                
                return File(stream.ToArray(), MimeType.Get(".pdf"), "Fish.pdf");
            }
            return null;
            
        }

        private void sort(int[] a)
        {
            if (a.Length == 0) return;
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = i + 1; j < a.Length; j++)
                {
                    if (a[i] > a[j])
                    {
                        int temp = a[i];
                        a[i] = a[j];
                        a[j] = temp;
                    }
                }
            }
        }

        private string arrang(int[] a)
        {
            sort(a);
            int i;
            if (a.Length == 0) return "()";
            int start = a[0];
            string result = a[0].ToString();
            for (i = 1; i < a.Length - 1; i++)
            {

                if (a[i] - start > 1)
                {
                    result += "," + a[i].ToString();
                }
                else if (a[i + 1] - a[i] > 1)
                {
                    result += "الی" + a[i].ToString();
                }
                start = a[i];
            }
            if (a.Length > 1)
            {
                if (a[i] - a[i - 1] > 1)
                {
                    result += "," + a[i].ToString();
                }
                else
                {
                    result += "الی" + a[i].ToString();
                }
                start = a[i];
            }
            return result;
        }
        public ActionResult _FishReport(int PeacockeryId, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            {
                ViewBag.PeacockeryId = PeacockeryId;
                ViewBag.mablagh = mablagh;
                ViewBag.fldFine = fldFine;
                ViewBag.fldValueAddPrice = fldValueAddPrice;
                ViewBag.fldPrice = fldPrice;
                ViewBag.Bed = Bed;
                ViewBag.Years = Years;
                ViewBag.fldOtherPrice = fldOtherPrice;
                ViewBag.fldMainDiscount = fldMainDiscount;
                ViewBag.fldFineDiscount = fldFineDiscount;
                ViewBag.fldValueAddDiscount = fldValueAddDiscount;
                ViewBag.fldOtherDiscount = fldOtherDiscount;
                return View();
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                }
                 );
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult _FishReportWin(int PeacockeryId, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.PeacockeryId = PeacockeryId;
                PartialView.ViewBag.mablagh = mablagh;
                PartialView.ViewBag.fldFine = fldFine;
                PartialView.ViewBag.fldValueAddPrice = fldValueAddPrice;
                PartialView.ViewBag.fldPrice = fldPrice;
                PartialView.ViewBag.Bed = Bed;
                PartialView.ViewBag.Years = Years;
                PartialView.ViewBag.fldOtherPrice = fldOtherPrice;
                PartialView.ViewBag.fldMainDiscount = fldMainDiscount;
                PartialView.ViewBag.fldFineDiscount = fldFineDiscount;
                PartialView.ViewBag.fldValueAddDiscount = fldValueAddDiscount;
                PartialView.ViewBag.fldOtherDiscount = fldOtherDiscount;
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
                }
                 );
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult _GenerateFishReport(int PeacockeryId, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)//المثنی
        {
            //if (Session["UserId"] == null)
            //    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            //{
                Models.cartaxEntities p = new Models.cartaxEntities();
                var peacokery = p.sp_PeacockerySelect("fldid", PeacockeryId.ToString(), 0, 1, "").FirstOrDefault();
                var peacokery_copy = p.Sp_Peacockery_CopySelect(PeacockeryId).FirstOrDefault();
                if (peacokery_copy != null)
                {
                    return File(peacokery_copy.fldCopy, "application/pdf");
                }
                var carfile = p.sp_CarFileSelect("fldid", peacokery.fldCarFileID.ToString(), 0, 1, "").FirstOrDefault();
                var car = p.sp_SelectCarDetils(Convert.ToInt64(carfile.fldCarID)).FirstOrDefault();
                if (car != null)
                {
                    var ServerDate = p.sp_GetDate().FirstOrDefault();
                    int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime).Substring(0, 4));
                    string date = toYear + "/12/29";
                    if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                        date = toYear + "/12/30";
                    System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                    // System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                    //var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                    //null, ServerDate.CurrentDateTime, _year, _Bed).ToList();
                    int sal = 0, mah = 0;
                    //ArrayList Years = new ArrayList();
                    //foreach (var item in bedehi)
                    //{
                    //    Years.Add(item.fldyear);
                    //}
                    /* ArrayList */
                    Years = (ArrayList)Session["Year" + peacokery.fldCarFileID];
                    int[] AvarezSal = new int[Years.Count];
                    for (int i = 0; i < Years.Count; i++)
                    {
                        AvarezSal[i] = (int)Years[i];
                    }

                    var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    //p.sp_selectAccountBank(Convert.ToInt32(Session["UserMnu"]), true).FirstOrDefault();

                    System.Data.Entity.Core.Objects.ObjectParameter _CountryID = new System.Data.Entity.Core.Objects.ObjectParameter("ID", typeof(long));

                    var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
                    string MohlatDate = "";
                    byte roundNumber = 0;
                    if (SubSetting != null)
                    {
                        if (SubSetting.fldLastRespitePayment > 0)
                        {
                            MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                        }
                        else if (SubSetting.fldLastRespitePayment == 0)
                        {
                            string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                            int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                            int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                            int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                            if (Years.Count > 1)
                            {
                                if (Mounth <= 6)
                                    MohlatDate = Year + "/" + Mounth + "/31";
                                else if (Mounth > 6 && Mounth < 12)
                                    MohlatDate = Year + "/" + Mounth + "/30";
                                else if (MyLib.Shamsi.Iskabise(Year) == true)
                                    MohlatDate = Year + "/" + Mounth + "/30";
                                else
                                    MohlatDate = Year + "/" + Mounth + "/29";
                            }
                            else if (Years.Count == 1)
                            {
                                if (MyLib.Shamsi.Iskabise(Year) == true)
                                    MohlatDate = Year + "/" + 12 + "/30";
                                else
                                    MohlatDate = Year + "/" + 12 + "/29";
                            }
                            //if
                        }
                        var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                        roundNumber = Round.fldRound;
                    }


                    double Rounded = 10;
                    switch (roundNumber)
                    {
                        case 3:
                            Rounded = 1000;
                            break;
                        case 2:
                            Rounded = 100;
                            break;
                        case 0:
                            Rounded = 1;
                            break;
                    }


                    mablagh = Convert.ToInt32(Math.Floor(mablagh / Rounded) * Rounded);//گرد به پایین
                    string ShGhabz = peacokery.fldShGhabz, ShPardakht = peacokery.fldShPardakht,
                        BarcodeText = "", ShParvande = "";
                    if (ShGhabz.Length > 0 && ShPardakht.Length > 0)
                        BarcodeText = ShGhabz.PadLeft(13, '0') + ShPardakht.PadLeft(13, '0');


                    Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                    Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter sp_pic1 = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter();

                    sp_pic1.Fill(dt.sp_PictureSelect1, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                    DataSet.DataSet1.sp_jCalcCarFileDataTable joziat = new DataSet.DataSet1.sp_jCalcCarFileDataTable();
                    joziat = (DataSet.DataSet1.sp_jCalcCarFileDataTable)Session["Joziyat" + peacokery.fldCarFileID];
                    foreach (var item in joziat)
                    {
                        dt.sp_jCalcCarFile.Addsp_jCalcCarFileRow((int)item.fldyear, (int)item.fldFirstPrice, (int)item.fldCurectPrice, (int)item.fldValueAdded, (int)item.fldFinalPrice,
                       (int)item.fldFine, (int)item.fldCountMonth, (int)item.fldDiscount, (int)item.fldDept, item.fldCalcDate, (int)item.OtherPrice, (int)item.fldValueAddDiscount,
                       (int)item.fldFineDiscount, (int)item.fldOtherDiscount);
                    }

                    fish.Fill(dt.sp_PeacockerySelect, "fldId", PeacockeryId.ToString(), 1, 1, "");
                    var _fish = p.sp_PeacockerySelect("fldId", PeacockeryId.ToString(), 1, 1, "").FirstOrDefault();
                    //int _Bedehkar = Convert.ToInt32(Session["Bed"]);
                    int _Bedehkar = Bed;
                    sp_pic.Fill(dt.sp_PictureSelect, "fldBankPic", _fish.fldBankId.ToString(), 1, 1, "");
                    var mnu1 = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    FastReport.Report Report = new FastReport.Report();
                    var UpReportSelect = p.sp_UpReportSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    var FishReport = p.sp_ReportsSelect("fldId", UpReportSelect.fldReportsID.ToString(), 1, 1, "").FirstOrDefault();
                    System.IO.MemoryStream Stream = new System.IO.MemoryStream(FishReport.fldReport);
                    Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                    var mnu = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    var State = p.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();
                    Report.Load(Stream);
                    Report.RegisterData(dt, "carTaxDataSet");
                    Report.SetParameterValue("MunicipalityName", mnu1.fldName);
                    Report.SetParameterValue("Barcode", BarcodeText);
                    Report.SetParameterValue("ShGhabz", ShGhabz);
                    Report.SetParameterValue("ShPardakht", ShPardakht);
                    Report.SetParameterValue("SalAvarez", "(" + arrang(AvarezSal) + ")");
                    Report.SetParameterValue("Mohlat", MohlatDate);
                    Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                    var time = DateTime.Now;
                    Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                    Report.SetParameterValue("MunicipalityName", mnu.fldName);
                    Report.SetParameterValue("Parameter", _Bedehkar);
                    Report.SetParameterValue("StateName", State.fldName);
                    Report.SetParameterValue("AreaName", Session["area"].ToString());
                    Report.SetParameterValue("OfficeName", Session["office"].ToString());
                    var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                    if (ImageSetting == "1")
                    {
                        Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                    }
                    else if (ImageSetting == "2")
                    {
                        Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                    }
                    else
                        Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                    MemoryStream stream = new MemoryStream();
                    Report.Prepare();
                    Report.Export(pdf, stream);
                    return File(stream.ToArray(), "application/pdf");
                }
                return null;
            //}
            //else
            //{
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.ERROR,
            //        Title = "خطا",
            //        Message = "شما مجاز به دسترسی نمی باشید."
            //    });
            //    DirectResult result = new DirectResult();
            //    return result;
            //    //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
            //    //return RedirectToAction("error", "Metro", new { area = "" });
            //}
        }

        public ActionResult _DownloadFish(int PeacockeryId, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)//المثنی
        {
            //if (Session["UserId"] == null)
            //    return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            //{
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var peacokery = p.sp_PeacockerySelect("fldid", PeacockeryId.ToString(), 0, 1, "").FirstOrDefault();
            var peacokery_copy = p.Sp_Peacockery_CopySelect(PeacockeryId).FirstOrDefault();
            if (peacokery_copy != null)
            {
                return File(peacokery_copy.fldCopy, MimeType.Get(".pdf"), "_Fish.pdf");
            }
            var carfile = p.sp_CarFileSelect("fldid", peacokery.fldCarFileID.ToString(), 0, 1, "").FirstOrDefault();
            var car = p.sp_SelectCarDetils(Convert.ToInt64(carfile.fldCarID)).FirstOrDefault();
            if (car != null)
            {
                var ServerDate = p.sp_GetDate().FirstOrDefault();
                int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime).Substring(0, 4));
                string date = toYear + "/12/29";
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                    date = toYear + "/12/30";
                System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                // System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                //var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                //null, ServerDate.CurrentDateTime, _year, _Bed).ToList();
                int sal = 0, mah = 0;
                //ArrayList Years = new ArrayList();
                //foreach (var item in bedehi)
                //{
                //    Years.Add(item.fldyear);
                //}
                /* ArrayList */
                Years = (ArrayList)Session["Year" + peacokery.fldCarFileID];
                int[] AvarezSal = new int[Years.Count];
                for (int i = 0; i < Years.Count; i++)
                {
                    AvarezSal[i] = (int)Years[i];
                }

                var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                //p.sp_selectAccountBank(Convert.ToInt32(Session["UserMnu"]), true).FirstOrDefault();

                System.Data.Entity.Core.Objects.ObjectParameter _CountryID = new System.Data.Entity.Core.Objects.ObjectParameter("ID", typeof(long));

                var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
                string MohlatDate = "";
                byte roundNumber = 0;
                if (SubSetting != null)
                {
                    if (SubSetting.fldLastRespitePayment > 0)
                    {
                        MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                    }
                    else if (SubSetting.fldLastRespitePayment == 0)
                    {
                        string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                        int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                        int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                        int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                        if (Years.Count > 1)
                        {
                            if (Mounth <= 6)
                                MohlatDate = Year + "/" + Mounth + "/31";
                            else if (Mounth > 6 && Mounth < 12)
                                MohlatDate = Year + "/" + Mounth + "/30";
                            else if (MyLib.Shamsi.Iskabise(Year) == true)
                                MohlatDate = Year + "/" + Mounth + "/30";
                            else
                                MohlatDate = Year + "/" + Mounth + "/29";
                        }
                        else if (Years.Count == 1)
                        {
                            if (MyLib.Shamsi.Iskabise(Year) == true)
                                MohlatDate = Year + "/" + 12 + "/30";
                            else
                                MohlatDate = Year + "/" + 12 + "/29";
                        }
                        //if
                    }
                    var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                    roundNumber = Round.fldRound;
                }


                double Rounded = 10;
                switch (roundNumber)
                {
                    case 3:
                        Rounded = 1000;
                        break;
                    case 2:
                        Rounded = 100;
                        break;
                    case 0:
                        Rounded = 1;
                        break;
                }


                mablagh = Convert.ToInt32(Math.Floor(mablagh / Rounded) * Rounded);//گرد به پایین
                string ShGhabz = peacokery.fldShGhabz, ShPardakht = peacokery.fldShPardakht,
                    BarcodeText = "", ShParvande = "";
                if (ShGhabz.Length > 0 && ShPardakht.Length > 0)
                    BarcodeText = ShGhabz.PadLeft(13, '0') + ShPardakht.PadLeft(13, '0');


                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter sp_pic1 = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter();

                sp_pic1.Fill(dt.sp_PictureSelect1, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                DataSet.DataSet1.sp_jCalcCarFileDataTable joziat = new DataSet.DataSet1.sp_jCalcCarFileDataTable();
                joziat = (DataSet.DataSet1.sp_jCalcCarFileDataTable)Session["Joziyat" + peacokery.fldCarFileID];
                foreach (var item in joziat)
                {
                    dt.sp_jCalcCarFile.Addsp_jCalcCarFileRow((int)item.fldyear, (int)item.fldFirstPrice, (int)item.fldCurectPrice, (int)item.fldValueAdded, (int)item.fldFinalPrice,
                   (int)item.fldFine, (int)item.fldCountMonth, (int)item.fldDiscount, (int)item.fldDept, item.fldCalcDate, (int)item.OtherPrice, (int)item.fldValueAddDiscount,
                   (int)item.fldFineDiscount, (int)item.fldOtherDiscount);
                }

                fish.Fill(dt.sp_PeacockerySelect, "fldId", PeacockeryId.ToString(), 1, 1, "");
                var _fish = p.sp_PeacockerySelect("fldId", PeacockeryId.ToString(), 1, 1, "").FirstOrDefault();
                //int _Bedehkar = Convert.ToInt32(Session["Bed"]);
                int _Bedehkar = Bed;
                sp_pic.Fill(dt.sp_PictureSelect, "fldBankPic", _fish.fldBankId.ToString(), 1, 1, "");
                var mnu1 = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                var UpReportSelect = p.sp_UpReportSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var FishReport = p.sp_ReportsSelect("fldId", UpReportSelect.fldReportsID.ToString(), 1, 1, "").FirstOrDefault();
                System.IO.MemoryStream Stream = new System.IO.MemoryStream(FishReport.fldReport);
                Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                var State = p.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();
                Report.Load(Stream);
                Report.RegisterData(dt, "carTaxDataSet");
                Report.SetParameterValue("MunicipalityName", mnu1.fldName);
                Report.SetParameterValue("Barcode", BarcodeText);
                Report.SetParameterValue("ShGhabz", ShGhabz);
                Report.SetParameterValue("ShPardakht", ShPardakht);
                Report.SetParameterValue("SalAvarez", "(" + arrang(AvarezSal) + ")");
                Report.SetParameterValue("Mohlat", MohlatDate);
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("Parameter", _Bedehkar);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                if (ImageSetting == "1")
                {
                    Report.SetParameterValue("MyTablighat", "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
                }
                else if (ImageSetting == "2")
                {
                    Report.SetParameterValue("MyTablighat", "سازمان فناوری اطلاعات و ارتباطات شهرداری قزوین");
                }
                else
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);


                return File(stream.ToArray(), MimeType.Get(".pdf"), "_Fish.pdf");
            }
            return null;
            //}
            //else
            //{
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.ERROR,
            //        Title = "خطا",
            //        Message = "شما مجاز به دسترسی نمی باشید."
            //    });
            //    DirectResult result = new DirectResult();
            //    return result;
            //    //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
            //    //return RedirectToAction("error", "Metro", new { area = "" });
            //}
        }
        public ActionResult ParsianPcPosVerify(int fishid, string TerminalID, string ResponseCode,
            string SerialId, string TxnDate, string RRN, string ResponseDescription)
        {
            Models.cartaxEntities m = new cartaxEntities();
            string Msg = ""; string MsgTitle = ""; byte Er = 0; int Id = 0;
            try
            {
                if (ResponseCode == "0")
                {
                    var Currdate = m.sp_GetDate().FirstOrDefault();
                    m.sp_PcPosTransactionUpdate_Status(Convert.ToInt32(Session["PcPosTransActionId"]), RRN, "TerminalID:" + TerminalID + "SerialId:" + SerialId.ToString() + "TxnDate:" + TxnDate.ToString());
                    var fish = m.sp_PeacockerySelect("fldid", fishid.ToString(), 1, 1, "").FirstOrDefault();
                    var carf = m.sp_CarFileSelect("fldId", fish.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    var carid = carf.fldCarID;
                    var car = m.sp_SelectCarDetils(carid).FirstOrDefault();
                    System.Data.Entity.Core.Objects.ObjectParameter _Cid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    m.sp_CollectionInsert(_Cid, car.fldID, m.sp_GetDate().FirstOrDefault().CurrentDateTime, fish.fldShowMoney, 9,
                        (int)fish.fldID, null, "", Convert.ToInt32(Session["UserId"]),
                                    "پرداخت از طریق pcpos با مشخصات: TerminalID:" + TerminalID + " ResponseCode: " + ResponseCode +
                                    " SerialId: " + SerialId + " TxnDate: " + TxnDate + " RRN:" + RRN + "  ResponseDescription: " +
                                    ResponseDescription, "", "", null, "", null, null, true, 1, DateTime.Now);
                    SendToSamie.Send(Convert.ToInt32(_Cid.Value), Convert.ToInt32(Session["UserMnu"]));

                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt", "step1:" + fishid.ToString() + "\n");
                    if (Convert.ToInt32(Session["UserMnu"]) == 1)
                    {
                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt", "step2:" + fishid.ToString() + "\n");
                        Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                        var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, fishid.ToString(), DateTime.Now, "");
                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                               + k1 + "-" + fishid.ToString() + "\n");
                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt", "step3:" + fishid.ToString() + "\n");
                    }
                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt", "step4:" + fishid.ToString() + "\n");

                    SmsSender sms = new SmsSender();
                    sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, Convert.ToInt32(Session["showmoney"]), "", "", "");
                    return Json(new { Msg = "پرداخت با موفقیت انجام شد و واریزی در سیستم ثبت گردید.", Msgtitle = "عملیات موفق", Er = 0 });
                }
                else
                {
                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\dd.txt", "NotPay:" + fishid.ToString() + ResponseCode + ":" + ResponseDescription + "\n");
                    return Json(new { Msg = "پرداخت انجام نشد.(خطا" + ResponseCode + ":" + ResponseDescription + ")", MsgTitle = "عملیات ناموفق", Er = 1 });
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
            }
            return Json(new
            {
                Msg = ResponseDescription,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);

        }  
        public ActionResult ParsianPcPosPay(int fishid, int PosIPId)
        {
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();

            var PayMethodVal = "";
            var pcpos_url = "";
            var ShebaVal = "";
            var PayMethodId = 0;
            var pcpos_urlId = 0;
            var ShebaId = 0;
            Models.cartaxEntities m = new cartaxEntities();
            var Tree = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            var Pos = m.sp_PcPosInfoSelect("fldTreeId", "", Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), 0).FirstOrDefault();
            if (Pos != null)
            {
                var param = m.sp_PcPosParametrSelect("fldBankId", "15", 0).ToList();
                foreach (var item in param)
                {
                    if (item.fldEnName == "PayMethod")
                    {
                        PayMethodId = item.fldId;
                    }
                    else if (item.fldEnName == "pcpos_url")
                    {
                        pcpos_urlId = item.fldId;
                    }
                    else if (item.fldEnName == "Sheba")
                    {
                        ShebaId = item.fldId;
                    }
                }
                PayMethodVal = m.sp_PcPosParam_DetailSelect("fldParamId", PayMethodId.ToString(), 0).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
                pcpos_url = m.sp_PcPosParam_DetailSelect("fldParamId", pcpos_urlId.ToString(), 0).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
                ShebaVal = m.sp_PcPosParam_DetailSelect("fldParamId", ShebaId.ToString(), 0).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
            }
            

            //long[] Mablgh=new long[10];
            //Mablgh[1] = Price;
            var PosIp = m.sp_PcPosIPSelect("fldId", PosIPId.ToString(), 0).FirstOrDefault();
            var pcposinf = m.sp_PcPosInfoSelect("fldid", PosIp.fldPcPosId.ToString(), 0, 0, 0).FirstOrDefault();
            var fish = m.sp_PeacockerySelect("fldid", fishid.ToString(), 1, 1, "").FirstOrDefault();
            var mun = m.sp_MunicipalitySelect("fldid", Session["UserMnu"].ToString(), 0, 1, "").FirstOrDefault();

            result.ViewBag.fishid = fishid;
            result.ViewBag.ip = PosIp.fldIP;
            result.ViewBag.mablagh = fish.fldShowMoney;
            result.ViewBag.ShGhabz = fish.fldShGhabz;
            result.ViewBag.ShPardakht = fish.fldShPardakht;
            result.ViewBag.PayMethodVal = PayMethodVal;
            result.ViewBag.pcpos_url = pcpos_url;
            result.ViewBag.Sheba = ShebaVal;
            //result.ViewBag.state = state;

            return result;
        }
        public ActionResult SamanPcPosPay(int fishid, int PosIPId)
        {
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            Models.cartaxEntities m = new cartaxEntities();
            var Tree = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            var Pos = m.sp_PcPosInfoSelect("fldTreeId","", Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), 0).FirstOrDefault();
            //var Pos = m.sp_PcPosInfoSelect("fldTreeId", Tree.CountryDivisionId.ToString(), 0).FirstOrDefault();
            var PayTypeId=0;
            var radifId=0;
            var AccNumId = 0;
            var PayMethodId = 0;

            var PayTypeVal = "";
            var radifVal = "";
            var AccNumVal = "";
            var PayMethodVal = "";

            if (Pos != null)
            {
                var param = m.sp_PcPosParametrSelect("fldBankId", "17", 0).ToList();
                foreach (var item in param)
	            {
		            if(item.fldEnName=="PayType"){
                        PayTypeId=item.fldId;
                    }
                    else if(item.fldEnName=="radif"){
                        radifId=item.fldId;
                    }
                    else if (item.fldEnName == "AccountNum")
                    {
                        AccNumId = item.fldId;
                    }
                    else if (item.fldEnName == "PayMethod")
                    {
                        PayMethodId = item.fldId;
                    }
	            }
                PayTypeVal = m.sp_PcPosParam_DetailSelect("fldParamId", PayTypeId.ToString(), 0).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
                radifVal = m.sp_PcPosParam_DetailSelect("fldParamId", radifId.ToString(), 0).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
                AccNumVal = m.sp_PcPosParam_DetailSelect("fldParamId", AccNumId.ToString(), 0).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
                PayMethodVal = m.sp_PcPosParam_DetailSelect("fldParamId", PayMethodId.ToString(), 0).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
            }

            var PosIp = m.sp_PcPosIPSelect("fldId", PosIPId.ToString(), 0).FirstOrDefault();
            var pcposinf = m.sp_PcPosInfoSelect("fldid", PosIp.fldPcPosId.ToString(),0,0, 0).FirstOrDefault();
            var fish = m.sp_PeacockerySelect("fldid", fishid.ToString(), 1, 1, "").FirstOrDefault();
            var mun = m.sp_MunicipalitySelect("fldid", Session["UserMnu"].ToString(), 0, 1, "").FirstOrDefault();
            result.ViewBag.fishid = fishid;
            result.ViewBag.ip = PosIp.fldIP;
            result.ViewBag.mablagh = fish.fldShowMoney;
            result.ViewBag.ShGhabz = fish.fldShGhabz;
            result.ViewBag.ShPardakht = fish.fldShPardakht;
            result.ViewBag.mun = mun.fldName;
            result.ViewBag.PayTypeVal = PayTypeVal;
            result.ViewBag.radifVal = radifVal;
            result.ViewBag.AccNumVal = AccNumVal;
            result.ViewBag.PayMethodVal = PayMethodVal;
            return result;
        }

        public ActionResult SamanPosVerify(int fishid, string TerminalID, string ResponseCode,
            string SerialId, string TxnDate, string RRN, string ResponseDescription)
        {
            byte Er = 1;
            if (ResponseCode == "00")
            {
                Er = 0;
                Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
                var fish = m.sp_PeacockerySelect("fldid", fishid.ToString(), 1, 1, "").FirstOrDefault();
                var carf = m.sp_CarFileSelect("fldId", fish.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var carid = carf.fldCarID;
                var car = m.sp_SelectCarDetils(carid).FirstOrDefault();
                System.Data.Entity.Core.Objects.ObjectParameter _Cid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                m.sp_PcPosTransactionUpdate_Status(Convert.ToInt32(Session["PcPosTransActionId"]), RRN, "پرداخت از طریق pcpos با مشخصات: TerminalID:" + TerminalID + " ResponseCode: " + ResponseCode +
                    " SerialId: " + SerialId + " TxnDate: " + TxnDate + " RRN:" + RRN + "  ResponseDescription: " +
                    ResponseDescription);
                m.sp_CollectionInsert(_Cid, car.fldID, m.sp_GetDate().FirstOrDefault().CurrentDateTime, fish.fldShowMoney, 9,
                    (int)fish.fldID, null, "", Convert.ToInt32(Session["UserId"]),
                                "پرداخت از طریق pcpos با مشخصات: TerminalID:" + TerminalID + " ResponseCode: " +ResponseCode+
                                " SerialId: " + SerialId + " TxnDate: " + TxnDate + " RRN:" + RRN + "  ResponseDescription: " +
                                ResponseDescription, "", "", null, "", null, null, true, 1, DateTime.Now);
                SendToSamie.Send(Convert.ToInt32(_Cid.Value), Convert.ToInt32(Session["UserMnu"]));
                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt", "step1:" + fishid.ToString() + "\n");
                if (Convert.ToInt32(Session["UserMnu"]) == 1)
                {
                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt", "step2:" + fishid.ToString() + "\n");
                    Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                    var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, fishid.ToString(), DateTime.Now, "");
                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                           + k1 + "-" + fishid.ToString() + "\n");
                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt", "step3:" + fishid.ToString() + "\n");
                }
                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\cc.txt", "step4:" + fishid.ToString() + "\n");
                        
                SmsSender sms = new SmsSender();
                sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, Convert.ToInt32(Session["showmoney"]), "", "", "");
            }
            return Json(new { Er=Er,msg = ResponseDescription });
        }
        public ActionResult PcPosPay(string serialBarchasb, string Mablagh, string PosIPId, long CarFileId, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.cartaxEntities m = new Avarez.Models.cartaxEntities();
            var carf = m.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var carid = carf.fldCarID;

            
            long peackokeryid = CheckExistFishForPos(carid, Convert.ToInt32(Mablagh));
            if (peackokeryid == 0)
            {
                Years = (ArrayList)Session["Year" + CarFileId];
                GenerateFishReport(CarFileId, Convert.ToInt32(Mablagh), fldFine, fldValueAddPrice, fldPrice, Years, Bed,fldOtherPrice,fldMainDiscount,fldFineDiscount,fldValueAddDiscount,fldOtherDiscount);
                peackokeryid = CheckExistFishForPos(carid, Convert.ToInt32(Mablagh));
            }
            if (peackokeryid != 0)
            {
                var fish = m.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                var car = m.sp_SelectCarDetils(carid).FirstOrDefault();
                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter _Cid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                var k = m.sp_PcPosTransactionInsert(_id, Convert.ToInt32(car.fldID), Convert.ToInt32(Session["showmoney"]), false, "", Convert.ToInt32(Session["UserId"]), "", fish.fldShGhabz, fish.fldShPardakht);
                Session["PcPosTransActionId"] = _id.Value.ToString();
                var PosIp = m.sp_PcPosIPSelect("fldId", PosIPId, 0).FirstOrDefault();
                var pcposinf = m.sp_PcPosInfoSelect("fldid", PosIp.fldPcPosId.ToString(),0,0, 0).FirstOrDefault();
                //var q = m.sp_PcPosParametrSelect("fldPosInfoId", PosIp.fldPcPosId.ToString(), 0).ToList();
                //var id = 0;
                //var id_url = 0;
                //foreach (var item in q)
                //{
                //    if (item.fldEnName == "PIN")
                //    {
                //        id = item.fldId;
                //    }
                //    else if (item.fldEnName == "BackUrl")
                //    {
                //        id_url = item.fldId;
                //    }
                //}
                //var q1 = m.sp_PcPosParam_DetailSelect("fldParamId", id.ToString(), 0).FirstOrDefault();
                if (pcposinf.fldBankId == 20)
                {
                    PAX.PCPOS.ActiveX objDOM = new PAX.PCPOS.ActiveX();
                    objDOM.InitSocket(PosIp.fldIP);
                    JavaScriptSerializer json = new JavaScriptSerializer();
                    var r = objDOM.BillPayment(_id.Value.ToString(), fish.fldShGhabz, fish.fldShPardakht, true);
                    Response res = json.Deserialize<Response>(r);

                    if (res.Success && res.TransactionInfo.ResponseCode == "00")
                    {
                        var t = m.sp_PcPosTransactionUpdate_Status(Convert.ToInt32(res.AdditionalData), res.TransactionInfo.Stan, "پرداخت در تاریخ " + MyLib.Shamsi.Miladi2ShamsiString(m.sp_GetDate().FirstOrDefault().CurrentDateTime) + " توسط دستگاه با کد ترمینال " + res.PosInformation.TerminalId + " و شماره سریال " + PosIp.fldSerialNum + "انجام شد.");
                        string ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
                        if (ImageSetting == "6")
                        {
                            m.sp_CollectionInsert(_Cid, car.fldID, m.sp_GetDate().FirstOrDefault().CurrentDateTime, Convert.ToInt32(Session["showmoney"]), 9, (int)fish.fldID, null, serialBarchasb, Convert.ToInt32(Session["UserId"]),
                                "پرداخت از طریق شناسه قبض و شناسه پرداخت", "", "", null, "", null, null, true, 1, DateTime.Now);
                            SendToSamie.Send(Convert.ToInt32(_Cid.Value), Convert.ToInt32(Session["UserMnu"]));
                        }
                        else
                        {
                            m.sp_CollectionInsert(_Cid, car.fldID, m.sp_GetDate().FirstOrDefault().CurrentDateTime, Convert.ToInt32(Session["showmoney"]), 9, (int)fish.fldID, null, "", Convert.ToInt32(Session["UserId"]),
                                "پرداخت از طریق شناسه قبض و شناسه پرداخت", "", "", null, "", null, null, true, 1, DateTime.Now);
                            SendToSamie.Send(Convert.ToInt32(_Cid.Value), Convert.ToInt32(Session["UserMnu"]));
                        }
                        if (Convert.ToInt32(Session["UserMnu"]) == 1)
                        {
                            Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                            var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, fish.fldID.ToString(), m.sp_GetDate().FirstOrDefault().CurrentDateTime, "");
                            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                    + k1 + "-" + fish.fldID.ToString() + "\n");
                        }

                        SmsSender sms = new SmsSender();
                        sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, Convert.ToInt32(Session["showmoney"]), "", "", "");
                        return Json(new { msg = "پرداخت با موفقیت انجام و واریزی در سیستم ثبت گردید.", Msgtitle = "عملیات موفق", Er = 0, BankId = 20 });
                    }
                    else
                        return Json(new { msg = "پرداخت انجام نشد." + res.ErrorCode, MsgTitle = "خطا", Er = 1, BankId = 20 });
                }
                else if (pcposinf.fldBankId == 17)
                {
                    return Json(new {msg = "", MsgTitle = "", Er = 0, BankId = 17, fishid = fish.fldID, PosIPId = PosIPId });
                    //SmsSender sms = new SmsSender();
                    //sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, Session["showmoney"].ToString(), "", "", "");
                    //return Json(new { msg = "پرداخت با موفقیت انجام و واریزی در سیستم ثبت گردید.", Msgtitle = "عملیات موفق", Er = 0 });
                }
                else if (pcposinf.fldBankId == 15)
                {
                    return Json(new { msg = "", MsgTitle = "", Er = 0, BankId = 15, fishid = fish.fldID, PosIPId = PosIPId });
                    //SmsSender sms = new SmsSender();
                    //sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, Session["showmoney"].ToString(), "", "", "");
                    //return Json(new { msg = "پرداخت با موفقیت انجام و واریزی در سیستم ثبت گردید.", Msgtitle = "عملیات موفق", Er = 0 });
                }
                else
                    return Json(new { msg = "پرداخت انجام نشد." + peackokeryid, MsgTitle = "خطا", Er = 1 });
            }
            else
                return Json(new { msg = "پرداخت انجام نشد." + peackokeryid, MsgTitle = "خطا", Er = 1 });
        }
        public long CheckExistFishForPos(long carid, int showmoney)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
            string MohlatDate = "";
            byte roundNumber = 0;
            var ServerDate = p.sp_GetDate().FirstOrDefault();
            if (SubSetting != null)
            {
                if (SubSetting.fldLastRespitePayment > 0)
                {
                    MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                }
                else if (SubSetting.fldLastRespitePayment == 0)
                {
                    string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                    int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                    int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                    int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                    if (Mounth <= 6)
                        MohlatDate = Year + "/" + Mounth + "/31";
                    else if (Mounth > 6 && Mounth < 12)
                        MohlatDate = Year + "/" + Mounth + "/30";
                    else if (MyLib.Shamsi.Iskabise(Year) == true)
                        MohlatDate = Year + "/" + Mounth + "/30";
                    else
                        MohlatDate = Year + "/" + Mounth + "/29";
                    //if
                }
                var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                roundNumber = Round.fldRound;
            }


            double Rounded = 10;
            switch (roundNumber)
            {
                case 3:
                    Rounded = 1000;
                    break;
                case 2:
                    Rounded = 100;
                    break;
                case 0:
                    Rounded = 1;
                    break;
            }


            Session["showmoney"] = Convert.ToInt32(Math.Floor(showmoney / Rounded) * Rounded);//گرد به پایین  
            var q = p.sp_SelectExistPeacockery(carid, Convert.ToInt32(Session["showmoney"])).FirstOrDefault();
            if (q != null)
                if (q.PeacockeryId != null)
                {
                    var t = p.sp_PeacockerySelect("fldId", q.PeacockeryId.ToString(), 1, 1, "").FirstOrDefault();
                    var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    if (bankid.fldID == t.fldAccountBankID)
                    {
                        return (long)q.PeacockeryId;
                    }
                    else
                        return 0;
                }
                else
                    return 0;
            else
                return 0;
        }
        public ActionResult GoToOnlinePay1(decimal Amount, string CarFileId, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice,
            int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var file = p.sp_CarFileSelect("fldID", CarFileId, 1, 0, "").FirstOrDefault();

            long peackokeryid = CheckExistFishForPos(file.fldCarID, Convert.ToInt32(Amount));
            string shGabz = "", Shpardakht = "";
            if (peackokeryid == 0)
            {
                //FishReport(CarId);
                Years = (ArrayList)Session["Year" + CarFileId];
                GenerateFishReport(Convert.ToInt64(CarFileId), Convert.ToInt32(Amount), fldFine, fldValueAddPrice, fldPrice, Years, Bed, fldOtherPrice, fldMainDiscount, fldFineDiscount, fldValueAddDiscount,fldOtherDiscount);
                peackokeryid = CheckExistFishForPos(file.fldCarID, Convert.ToInt32(Amount));
            }
            if (peackokeryid != 0)
            {
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                if (fish.fldShGhabz != "" && fish.fldShPardakht != "")
                {
                    shGabz = fish.fldShGhabz;
                    shGabz = shGabz.PadLeft(13, '0');
                    Shpardakht = fish.fldShPardakht;
                }
            }
            return Json(new { shGabz = shGabz, Shpardakht = Shpardakht }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GoToOnlinePay(decimal Amount, long CarFileId, int BankId, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount, DataSet.DataSet1.sp_jCalcCarFileDataTable Joziyat)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            //Amount = 1000;
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carf = p.sp_CarFileSelect("fldId", CarFileId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var CarId = carf.fldCarID;

            string Tax = "";
            var car = p.sp_SelectCarDetils(CarId).FirstOrDefault();
            long peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
            if (peackokeryid == 0)
            {
                /*FishReport(carid);*/
                Years = (ArrayList)Session["Year" + CarFileId];
                GenerateFishReport(CarFileId, Convert.ToDouble(Amount), fldFine, fldValueAddPrice, fldPrice, Years, Bed, fldOtherPrice, fldMainDiscount, fldFineDiscount, fldValueAddDiscount, fldOtherDiscount);
                peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
            }
            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));

            if (BankId == 15 || BankId == 30 || BankId==32)//بانک پارسیان
            {
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);
                var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                Tax = q.fldTemporaryCode;
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                Session["OnlinefishId"] = fish.fldID;
                Amount = fish.fldShowMoney;
            }
            else if (BankId == 17 || BankId == 20)//سامان و شهر
            {
                Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
                if (peackokeryid != 0)
                {
                    var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                    if (fish.fldShGhabz != "" && fish.fldShPardakht != "")
                    {
                        Amount = fish.fldShowMoney;
                        p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), fish.fldShGhabz + "|" + fish.fldShPardakht, Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);
                        var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
                        Session["shGhabz"] = fish.fldShGhabz;
                        Session["shPardakht"] = fish.fldShPardakht;
                        Session["OnlinefishId"] = fish.fldID;
                    }
                    else
                    {
                        Session["shGhabz"] = null;
                        Session["shPardakht"] = null;
                        Session["OnlinefishId"] = null;
                        return null;
                    }
                }
            }
            else//سایر بانک ها
            {
                Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');                
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                Session["OnlinefishId"] = fish.fldID;
                Amount = fish.fldShowMoney;
                if (BankId == 33)
                {
                    //CentralOnlinePaymentTransaction CO = new CentralOnlinePaymentTransaction();
                    WebTransaction.TransactionWebService Tr = new WebTransaction.TransactionWebService();
                    var Mun = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //Tax = CO.GetSamanOrderId(Convert.ToInt32(Amount),Mun.fldName).ToString();
                    Tax = Tr.GetSamanOrderId(Convert.ToInt32(Amount), Mun.fldName).ToString();
                }
            }
            //if (BankId != 17 && BankId != 15 && BankId != 30 && BankId != 20)
            //    p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);

            //if (BankId != 15 && BankId != 30)//غیر از پارسیان
            //{
            //    Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
            //    var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
            //    Session["OnlinefishId"] = fish.fldID;
            //    Amount = fish.fldShowMoney;
            //}
            //else//اگر بانک پارسیان بود
            //{
            //    p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);
            //    var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            //    Tax = q.fldTemporaryCode;
            //    var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
            //    Session["OnlinefishId"] = fish.fldID;
            //    Amount = fish.fldShowMoney;
            //}
            //if (BankId == 17 || BankId == 20)
            //{
            //    if (peackokeryid != 0)
            //    {
            //        var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
            //        if (fish.fldShGhabz != "" && fish.fldShPardakht != "")
            //        {
            //            Amount = fish.fldShowMoney;
            //            p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), fish.fldShGhabz + "|" + fish.fldShPardakht, Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);
            //            var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            //            Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
            //            Session["shGhabz"] = fish.fldShGhabz;
            //            Session["shPardakht"] = fish.fldShPardakht;
            //            Session["OnlinefishId"] = fish.fldID;
            //        }
            //        else
            //        {
            //            Session["shGhabz"] = null;
            //            Session["shPardakht"] = null;
            //            Session["OnlinefishId"] = null;
            //            return null;
            //        }
            //    }
            //}

            if (Convert.ToInt32(Session["UserMnu"]) ==1)
            {
                XmlDocument XDoc1 = new XmlDocument();
                // Create root node.
                XmlElement XElemRoot1 = XDoc1.CreateElement("FicheDetailByFisyear");
                XDoc1.AppendChild(XElemRoot1);
                XmlDocument XDoc2 = new XmlDocument();
                // Create root node.
                XmlElement XElemRoot2 = XDoc2.CreateElement("FicheAddAndSub");
                XDoc2.AppendChild(XElemRoot2);
                XmlDocument XDoc = new XmlDocument();
                // Create root node.
                XmlElement XElemRoot = XDoc.CreateElement("FicheDetail");
                XDoc.AppendChild(XElemRoot);
                int discount = 0;
                Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                string hesabid = "";
                System.Xml.XmlReader xmlReader = System.Xml.XmlReader
                    .Create((Stream)new MemoryStream(Encoding.UTF8.GetBytes("<hesabs>" + new Avarez.Hesabrayan.ServiseToRevenueSystems().AccountListRevenue(1).InnerXml.Replace("\"", "'") + "</hesabs>")));
                var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Node")
                    {
                        if (bankid.fldAccountNumber == xmlReader["AccountNum"].ToString())
                            hesabid = xmlReader["ID"].ToString();
                    }
                }

                if (hesabid != "")
                {
                    int avarez = 0, jarime = 0, sayer = 0, takhfif = 0;
                    avarez = fldPrice + fldValueAddPrice;
                    jarime = Convert.ToInt32(fldFine);
                    sayer = fldOtherPrice;
                    takhfif = fldMainDiscount;
                    XmlElement Xsource = XDoc.CreateElement("Node");
                    Xsource.SetAttribute("RevenueID", "198");//کد درامدی عوارض خودرو 
                    Xsource.SetAttribute("RevenueCost", avarez.ToString());
                    Xsource.SetAttribute("RevenueTaxCost", "0");
                    Xsource.SetAttribute("RevenueAvarezCost", "0");
                    Xsource.SetAttribute("RevenueTaxAvarezCost", "0");
                    Xsource.SetAttribute("RevenueBed", "0");
                    Xsource.SetAttribute("RevenueBes", "0");
                    Xsource.SetAttribute("AmountSavingBand", "0");
                    Xsource.SetAttribute("Discount", takhfif.ToString());

                    XElemRoot.AppendChild(Xsource);

                    if (jarime != 0)
                    {
                        XmlElement Xsource1 = XDoc.CreateElement("Node");
                        Xsource1.SetAttribute("RevenueID", "158");//کد در امدی جرائم
                        Xsource1.SetAttribute("RevenueCost", jarime.ToString());
                        Xsource1.SetAttribute("RevenueTaxCost", "0");
                        Xsource1.SetAttribute("RevenueAvarezCost", "0");
                        Xsource1.SetAttribute("RevenueTaxAvarezCost", "0");
                        Xsource1.SetAttribute("RevenueBed", "0");
                        Xsource1.SetAttribute("RevenueBes", "0");
                        Xsource1.SetAttribute("AmountSavingBand", "0");
                        Xsource1.SetAttribute("Discount", "0");

                        XElemRoot.AppendChild(Xsource1);
                    }
                    if (sayer != 0)
                    {
                        XmlElement Xsource1 = XDoc.CreateElement("Node");
                        Xsource1.SetAttribute("RevenueID", "197");//کد در امدی سایر
                        Xsource1.SetAttribute("RevenueCost", sayer.ToString());
                        Xsource1.SetAttribute("RevenueTaxCost", "0");
                        Xsource1.SetAttribute("RevenueAvarezCost", "0");
                        Xsource1.SetAttribute("RevenueTaxAvarezCost", "0");
                        Xsource1.SetAttribute("RevenueBed", "0");
                        Xsource1.SetAttribute("RevenueBes", "0");
                        Xsource1.SetAttribute("AmountSavingBand", "0");
                        Xsource1.SetAttribute("Discount", "0");

                        XElemRoot.AppendChild(Xsource1);
                    }
                    Years = (ArrayList)Session["Year" + CarFileId];
                    for (int i = 0; i < Years.Count; i++)
                    {
                        if (Convert.ToInt32(Years[i]) == 0)
                        {
                            Years.Remove(Years[i]);
                            break;
                        }
                    }

                    int[] AvarezSal = new int[0];
                    if (Years != null)
                    {
                        AvarezSal = new int[Years.Count];
                        for (int i = 0; i < Years.Count; i++)
                        {
                            AvarezSal[i] = Convert.ToInt32(Years[i]);
                        }
                    }
                    var k1 = toRevenueSystems.RegisterNewFicheByAccYearCostAndDiscount(3, 1, Session["OnlinefishId"].ToString(), car.fldOwnerName.ToString(),
                             p.sp_GetDate().FirstOrDefault().CurrentDateTime, hesabid, Session["OnlinefishId"].ToString(),
                             "کد ملی:" + car.fldMelli_EconomicCode.ToString() + " پلاک:" + car.fldCarPlaqueNumber.ToString() + " بابت عوارض " + arrang(AvarezSal),
                              "", "", "", "", "", 8, 2, car.fldCarAccountName + " " + car.fldCarSystemName + " " + car.fldCarModel + " " + car.fldCarClassName, car.fldVIN.ToString(),
                             Convert.ToInt32(Amount), 0, discount, XDoc, XDoc1, XDoc2);
                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal avarez: "
                        + k1 + "-" + Session["OnlinefishId"].ToString() + "\n");
                }
            }
            p.sp_OnlineTemporaryCodePaymentsUpdate(Convert.ToInt32(_id.Value), Tax, Amount, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            //if (Session["IsOfficeUser"] != null)
            //{
            //    epishkhan.devpishkhannwsv1 epishkhan = new epishkhan.devpishkhannwsv1();
            //    var result = epishkhan.servicePay("atJ5+$J1RtFpj", Session["ver_code"].ToString(), Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ServiceId"]), 100);
            //    if (result > 0)
            //        Session["serviceCallSerial"] = result;
            //    else
            //    {
            //        switch (result)
            //        {
            //            case -3:
            //                return Json("مهلت تراکنش به پایان رسیده است.", JsonRequestBehavior.AllowGet);
            //            case -7:
            //                return Json("مجوز استفاده از این سرویس را ندارید.", JsonRequestBehavior.AllowGet);
            //            case -8:
            //                return Json("کاربر شما غیر فعال شده است.", JsonRequestBehavior.AllowGet);
            //            case -10:
            //                return Json("اعتبار شما کافی نیست.", JsonRequestBehavior.AllowGet);
            //        }
            //    }
            //}
            Session["Amount"] = Amount;
            Session["Tax"] = Tax;
            Session["ReturnUrl"] = "/NewVer/First/Index";
            string URL = "";
            if (BankId == 20)
            {
                URL = "NewVer/CityBank_New";
            }
            else if (BankId == 1)
            {
                URL = "NewVer/MeliBank_New";
            }
            else if (BankId == 2)
            {
                URL = "NewVer/TejaratBank_New";
            }
            else if (BankId == 15)
            {
                URL = "NewVer/Parsian_New";
            }
            else if (BankId == 17)
            {
                URL = "NewVer/Saman_New";
            }
            else if (BankId == 30)
            {
                URL = "NewVer/Parsiaan_New";
            }
            else if (BankId == 33)
            {
                URL = "NewVer/SamanKish";
            }
            else if (BankId == 32)
            {
                URL = "NewVer/Sadad";
            }
            //return RedirectToAction("Index", URL, new { area = "" });
            return Json("~/" + URL + "/Index", JsonRequestBehavior.AllowGet);
        }
    }
}
