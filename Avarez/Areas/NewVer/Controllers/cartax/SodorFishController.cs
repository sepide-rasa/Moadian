using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using System.Web.Configuration;
using Avarez.Controllers.Users;
using System.Collections;
using System.Configuration;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class SodorFishController : Controller
    {
        //
        // GET: /NewVer/SodorFish/
         
        public ActionResult Index(int CarId,int CarFileId, string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });

            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 246))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->صورتحساب");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                Session["fldCarID"] = CarId;
                Session["fldCarID1"] = CarId;
                Session["fldCarID2"] = CarId;
                Session["fldCarID3"] = CarId;
                Session["mablagh"] = null;
                Session["Fine"] = null;
                Session["ValueAddPrice"] = null;
                Session["Price"] = null;
                Session["Year"] = null;

                Models.cartaxEntities m = new Models.cartaxEntities();
                var Tree = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();

                //var UpTree = m.sp_SelectUpTreeCountryDivisions(Convert.ToInt32(Tree.fldID), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                var PosIPId = 0;

                //foreach (var item in UpTree.OrderByDescending(l=>l.fldNodeType))
                //{
                //var c = m.sp_GET_IDCountryDivisions(item.fldNodeType, item.fldSourceID).FirstOrDefault();
                var Pos = m.sp_PcPosInfoSelect("fldTreeId","", Convert.ToInt32(Session["CountryType"]),Convert.ToInt32(Session["CountryCode"]), 0).FirstOrDefault();
                if (Pos != null)
                {
                    var UserPos = m.sp_PcPosUserSelect("fldIdUser", Session["UserId"].ToString(), 0).FirstOrDefault();
                    if (UserPos != null)
                    {
                        var PosIp = m.sp_PcPosIPSelect("fldId", UserPos.fldPosIPId.ToString(), 0).FirstOrDefault();
                        if (PosIp != null)
                        {
                            PosIPId = PosIp.fldId;
                            //break;
                        }
                    }
                }
                //}
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };

                result.ViewBag.PosIPId = PosIPId;
                result.ViewBag.CarId = CarId;
                result.ViewBag.CarFileId = CarFileId;
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return result;
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

        public ActionResult HelpSodorFish()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }
        }
        public ActionResult ShowMafasa(string containerId, string CarId, string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.id = CarId;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult Mafasa(long id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 249))
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var car = p.sp_SelectCarDetils(id).FirstOrDefault();
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
                        string msg1 = ""; string MsgTitle = ""; int Err = 0;
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
                        Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                        Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter();
                        Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter Receipt = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
                        sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                        Receipt.Fill(dt.rpt_Receipt, Convert.ToInt32(car.fldCarID), 2);
                        Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter exp = new Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter();
                        fish.Fill(dt.rpt_RecoupmentAccount, car.fldID,DateTime.Now);
                        exp.Fill(dt.sp_CarExperienceSelect, "fldCarFileID", car.fldID.ToString(), 0, Convert.ToInt32(Session["UserMnu"].ToString()), "");
                        Avarez.Models.cartaxEntities Car = new Avarez.Models.cartaxEntities();
                        Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                        var mnu = Car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                        var State = Car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();

                        System.Data.Entity.Core.Objects.ObjectParameter mafasaId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(string));
                        System.Data.Entity.Core.Objects.ObjectParameter LetterDate = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterDate", typeof(DateTime));
                        System.Data.Entity.Core.Objects.ObjectParameter LetterNum = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterNum", typeof(string));

                        p.Sp_MafasaInsert(mafasaId, car.fldCarID, Convert.ToInt32(Session["UserMnu"]), null, Convert.ToInt32(Session["UserId"]), LetterDate, LetterNum);
                        string barcode = WebConfigurationManager.AppSettings["SiteURL"] + "/QR_Mafasa/Get/" + mafasaId.Value;

                        FastReport.Report Report = new FastReport.Report();
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_Mafasa.frx");
                        Report.RegisterData(dt, "complicationsCarDBDataSet");
                        Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(LetterDate.Value)));
                        var time = Convert.ToDateTime(LetterDate.Value);
                        Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
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
                            Report.SetParameterValue("MyTablighat",  "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات");
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

        public FileResult ShowMafasaPdf(string id)
        {
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q=p.Sp_MafasaSelect_Image(id).FirstOrDefault();
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
        public ActionResult Receipt(int id, int Type)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ResidId = id;
            PartialView.ViewBag.Type = Type;
            return PartialView;
        }
        public ActionResult GeneratPdfReceipt(int ResidId,int Type)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter resid = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter carDitail = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.rpt_Receipt(ResidId, Type).FirstOrDefault();
            if (q == null)
                return null;
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();
            carDitail.Fill(dt.sp_SelectCarDetils, Convert.ToInt64(q.fldCarId));
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
            resid.Fill(dt.rpt_Receipt, ResidId, Type);
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
            Report.Prepare();
            Report.Export(pdf, stream);
            return File(stream.ToArray(), "application/pdf");
        }
        public ActionResult GetPicStatus(long id)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var car = m.sp_SelectCarDetils(id).FirstOrDefault();
            var file = m.sp_CarFileSelect("fldid", car.fldID.ToString(), 0, 1, "").FirstOrDefault();
            bool status = true; int carfile = 0; int carext = 0; int coll = 0;


            if (file.fldSanadForoshFileId == null && file.fldCartFileId == null && file.fldBargSabzFileId == null)
            {
                status = false;
                carfile = 1;
            }
            var carex = m.sp_CarExperienceSelect("fldCarFileID", car.fldID.ToString(), 0, 1, "").ToList();
            if (carex != null)
            {
                foreach (var item in carex)
                {
                    DateTime dt1 = MyLib.Shamsi.Shamsi2miladiDateTime(item.fldDate);
                    DateTime dt2 = DateTime.Parse("2017/02/19");

                    if (dt1.Date > dt2.Date)
                        if (item.fldFileId == null)
                        {
                            status = false;
                            carext = 1;
                        }
                }
            }
            var collection = m.sp_CollectionSelect("fldCarFileID", car.fldID.ToString(), 0, 1, "").Where(k => k.fldMunId != null).ToList();
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
            return Json(new { status = status, carfile = carfile, carext = carext, coll = coll }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintResidDaftar(int carid)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.carid = carid;
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
                    Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(LetterDate.Value)));
                    var time = Convert.ToDateTime(LetterDate.Value);
                    Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
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
        public ActionResult PrintJoziyatAvarez(int carid)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.carid = carid;
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
            Avarez.DataSet.DataSet1TableAdapters.sp_jCalcCarFileTableAdapter jCalcCarFile = new Avarez.DataSet.DataSet1TableAdapters.sp_jCalcCarFileTableAdapter();
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
            jCalcCarFile.Fill(dt.sp_jCalcCarFile, (int)car.fldID, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null, null, DateTime.Now, Convert.ToInt32(Session["UserId"]), ref _year, ref _Bed);
            CarDetils.Fill(dt.sp_SelectCarDetils, carid);
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptJoziatAvarez.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("Parameter", _Bed.Value);
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
        public ActionResult CheckBlackList(long carid)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_ListeSiyahSelect("fldCarId", carid.ToString(), 30).FirstOrDefault();
            var Type = 0;
            string Msg = "";
            if (q != null)
            {
                Type = q.fldType;
                if (Type == 2)
                {
                    Msg = q.fldMsg;
                }
                else
                {
                    Msg = "";
                }
            }
            return Json(new { Msg = Msg }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult CheckExistFish(long carid, int showmoney)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
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


            showmoney = Convert.ToInt32(Math.Floor(showmoney / Rounded) * Rounded);//گرد به پایین  
            var q = p.sp_SelectExistPeacockery(carid, showmoney).FirstOrDefault();
            if (q != null)
                if (q.PeacockeryId != null)
                {
                    var t = p.sp_PeacockerySelect("fldId", q.PeacockeryId.ToString(), 1, 1, "").FirstOrDefault();
                    var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    if (bankid!=null)
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
        
        public ActionResult _FishReport(int PeacockeryId, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
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
        public ActionResult _GenerateFishReport(int PeacockeryId, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)//المثنی
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var peacokery = p.sp_PeacockerySelect("fldid", PeacockeryId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var peacokery_copy = p.Sp_Peacockery_CopySelect(PeacockeryId).FirstOrDefault();
                if (peacokery_copy != null)
                {
                    return File(peacokery_copy.fldCopy, "application/pdf");
                }
                var carfile = p.sp_CarFileSelect("fldid", peacokery.fldCarFileID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                   /* ArrayList */Years = (ArrayList)Session["Year"];
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
                    joziat = (DataSet.DataSet1.sp_jCalcCarFileDataTable)Session["Joziyat"];
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
                    var FishReport = p.sp_ReportsSelect("fldId", UpReportSelect.fldReportsID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    System.IO.MemoryStream Stream = new System.IO.MemoryStream(FishReport.fldReport);
                    Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                    var mnu = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    var State = p.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro", new { area = "" });
            }
        }
        public ActionResult FishReport(int carid, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.carid = carid;
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
        public ActionResult GenerateFishReport(long carid, double mablagh, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var car = p.sp_SelectCarDetils(carid).FirstOrDefault();
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
                    Years = (ArrayList)Session["Year"];

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
                        //X.Msg.Show(new MessageBoxConfig
                        //{
                        //    Buttons = MessageBox.Button.OK,
                        //    Icon = MessageBox.Icon.ERROR,
                        //    Title = "خطا",
                        //    Message = "پرونده انتخابی بدهکار نیست."
                        //});
                        //DirectResult result = new DirectResult();
                        //return result;
                        Session["ER"] = "پرونده انتخابی بدهکار نیست.";
                        return RedirectToAction("error", "Metro", new { area = "" });
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
                        var mnu = p.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (mnu.fldInformaticesCode == "")
                            mnu.fldInformaticesCode = "0";
                        if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                        {
                            var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                            if (Divisions != null)
                            {
                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                        var local = p.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (local.fldSourceInformatics == "")
                            local.fldSourceInformatics = "0";
                        if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                        {
                            var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                            if (Divisions != null)
                            {
                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                        var area = p.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (area != null)
                        {
                            if (area.fldLocalID != null)
                            {
                                var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                                if (local.fldSourceInformatics == "")
                                    local.fldSourceInformatics = "0";
                                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                                var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (mnu.fldInformaticesCode == "")
                                    mnu.fldInformaticesCode = "0";
                                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                        var office = p.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (office != null)
                        {
                            if (office.fldAreaID != null)
                            {
                                var area = p.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (area != null)
                                {
                                    if (area.fldLocalID != null)
                                    {
                                        var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                                        if (local.fldSourceInformatics == "")
                                            local.fldSourceInformatics = "0";
                                        if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                        {
                                            var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                            if (Divisions != null)
                                            {
                                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                                        var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                        if (mnu.fldInformaticesCode == "")
                                            mnu.fldInformaticesCode = "0";
                                        if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                        {
                                            var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                            if (Divisions != null)
                                            {
                                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                                var local = p.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (local != null)
                                {
                                    if (local.fldSourceInformatics == "")
                                        local.fldSourceInformatics = "0";
                                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                                    var mnu = p.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                    if (mnu.fldInformaticesCode == "")
                                        mnu.fldInformaticesCode = "0";
                                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                    joziat = (DataSet.DataSet1.sp_jCalcCarFileDataTable)Session["Joziyat"];
                    var k = p.sp_CarFileSelect("fldCarId", carid.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

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
                    var FishReport = p.sp_ReportsSelect("fldId", UpReportSelect.fldReportsID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    System.IO.MemoryStream Stream = new System.IO.MemoryStream(FishReport.fldReport);
                    //int _Bedehkar = Convert.ToInt32(Session["Bed"]);
                    int _Bedehkar = Bed;
                    var State = p.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                    return File(stream.ToArray(), "application/pdf");
                }
                return null;
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
                    result += "-" + a[i].ToString();
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
                    result += "-" + a[i].ToString();
                }
                start = a[i];
            }
            return result;
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
        public ActionResult GoToOnlinePay(decimal Amount, long CarId, int BankId, int fldFine, int fldValueAddPrice, int fldPrice, ArrayList Years, int Bed, int fldOtherPrice, int fldMainDiscount, int fldFineDiscount, int fldValueAddDiscount, int fldOtherDiscount, DataSet.DataSet1.sp_jCalcCarFileDataTable Joziyat)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            //Amount = 1000;
            Models.cartaxEntities p = new Models.cartaxEntities();
            string Tax = "";
            long peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
            if (peackokeryid == 0)
            {
                GenerateFishReport(CarId, Convert.ToDouble(Amount), fldFine, fldValueAddPrice, fldPrice, Years, Bed, fldOtherPrice, fldMainDiscount, fldFineDiscount, fldValueAddDiscount, fldOtherDiscount);
                peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
            }
            var car = p.sp_SelectCarDetils(CarId).FirstOrDefault();

            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
            if (BankId != 17 && BankId != 15)
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);

            if (BankId != 15)
            {
                Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                Session["OnlinefishId"] = fish.fldID;
            }
            else
            {
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);
                var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                Tax = q.fldTemporaryCode;
                var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                Session["OnlinefishId"] = fish.fldID;
            }
            if (BankId == 17)
            {                
                if (peackokeryid != 0)
                {
                    var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                    if (fish.fldShGhabz != "" && fish.fldShPardakht != "")
                    {
                        p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), fish.fldShGhabz + "|" + fish.fldShPardakht, Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]), null);
                        var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
                        Session["shGhabz"] = fish.fldShGhabz;
                        Session["shPardakht"] = fish.fldShPardakht;
                        Session["OnlinefishId"] = fish.fldID;
                    }
                    else
                        return null;
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
            else if (BankId == 31)
            {
                URL = "NewVer/Samaan_New";
            }
            //return RedirectToAction("Index", URL, new { area = "" });
           return Json("~/"+URL + "/Index", JsonRequestBehavior.AllowGet);
        }
        public ActionResult calc(int carid)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            int State = 1;
            Models.cartaxEntities m = new Models.cartaxEntities();
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
                string msg1 = "";

                switch (Result)
                {
                    case Transaction.TransactionResult.Fail:
                        msg1 = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1,
                            State = State
                        }, JsonRequestBehavior.AllowGet);

                    case Transaction.TransactionResult.NotSharj:
                        msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1,
                            State = State
                        }, JsonRequestBehavior.AllowGet);

                }
            }
            //var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
            //    null, DateTime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
            //Session["year"] = _year.Value.ToString();
            //Session["bed"] = _Bed.Value.ToString();

            var bedehi = m.prs_newCarFileCalc(DateTime, Convert.ToInt32(Session["CountryType"]),
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
                if (mablagh < 1000)
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
                    //Bed=Convert.ToInt32(_Bed.Value),
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
        }
        public ActionResult InsertInSuporter(string Year, int? fldCarClassId, int? fldCarModelId, int? fldCarSystemId, int? fldCabinTypeId, int? fldCarAccountTypeId, int? fldCarMakeId)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var Path = ""; ArrayList ar = new ArrayList();
            //var car = p.sp_SelectCarDetils(Convert.ToInt32(Session["fldCarID3"])).FirstOrDefault();
            /*var Div = p.sp_CountryDivName(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();*/
            var LastNodeId = p.sp_TableTreeSelect("fldSourceID", Session["CountryCode"].ToString(),0, 0, 0).Where(l => l.fldNodeType == Convert.ToInt32(Session["CountryType"])).FirstOrDefault().fldID;
            var nodes = p.sp_SelectUpTreeCountryDivisions(LastNodeId, 1, "").ToList();
            foreach (var item in nodes)
            {
                ar.Add(item.fldNodeName);
            }
            for (int i = 0; i < ar.Count; i++)
            {
                if (i < ar.Count - 1)
                    Path += ar[i].ToString() + "-->";
                else
                    Path += ar[i].ToString();
            }
            Supporter.SendToSuporter S = new Supporter.SendToSuporter();
            var Code = S.InsertInSupport(Year, fldCarClassId, fldCarModelId, fldCarSystemId, fldCabinTypeId, fldCarAccountTypeId, fldCarMakeId, Path);

            return Json(new
            {
                msg = "درخواست با کد رهگیری " + Code + " به پشتیبان ارسال شد."
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Update(string FromYear, string ToYear, string CarMakeType, string CarAccountType, string CarCabin, string CarSystem, string CarTip, string CarClass)
        {
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 120))
                {
                    var Year2 = ToYear;
                    var Year = FromYear.Split(',');
                    var Loop = Year.Count();

                    Models.cartaxEntities m = new Models.cartaxEntities();
                    var Mun = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    string make = CarMakeType, acc = CarAccountType, cabin = CarCabin, sys = CarSystem, tip = CarTip, cls = CarClass;
                    //make = m.sp_CarMakeSelect("fldid", CarMakeType, 0, 1, "").FirstOrDefault().fldName;
                    //acc = m.sp_CarAccountTypeSelect("fldid", CarAccountType, 0, 1, "").FirstOrDefault().fldName;
                    //cabin = m.sp_CabinTypeSelect("fldid", CarCabin, 0, 1, "").FirstOrDefault().fldName;
                    //sys = m.sp_CarSystemSelect("fldid", CarSystem, 0, 1, "").FirstOrDefault().fldName;
                    //tip = m.sp_CarModelSelect("fldid", CarTip, 0, 1, "").FirstOrDefault().fldName;
                    //cls = m.sp_CarClassSelect("fldid", CarTip, 0, 1, "").FirstOrDefault().fldName;

                    RateWebService.Rate WebRate = new RateWebService.Rate();
                    var Check = WebRate.CheckAccountCharge(Mun.fldRWUserName, Mun.fldRWPass, Mun.fldName);
                    if (Check == true)
                    {
                        int UserId = Convert.ToInt32(Session["UserId"]);

                        for (int i = 0; i < Loop; i++)
                        {
                            if (Year2 == "")
                            {
                                FromYear = ToYear = Year[i];
                            }
                            var Rate = WebRate.GetRate(Mun.fldRWUserName, Mun.fldRWPass, Mun.fldName, Convert.ToInt32(FromYear), Convert.ToInt32(ToYear), make, acc, cabin, sys, tip, cls);
                            var Type = 1;
                            var Code = 0;
                            Models.sp_CarAccountTypeSelect Acc = null;
                            Models.sp_CabinTypeSelect Cabin = null;
                            Models.sp_CarSystemSelect Sys = null;
                            Models.sp_CarModelSelect Tip = null;
                            Models.sp_CarClassSelect Class = null;

                            if (Rate != null)
                                foreach (var item in Rate)
                                {
                                    var Name = item.fldName.Split('|');

                                    var Make = m.sp_CarMakeSelect("fldName", Name[0], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                    if (Make != null && Name.Count() > 1)
                                        Acc = m.sp_CarAccountTypeSelect("fldName", Name[1], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldCarMakeID == Make.fldID).FirstOrDefault();
                                    if (Acc != null && Name.Count() > 2)
                                        Cabin = m.sp_CabinTypeSelect("fldName", Name[2], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldCarAccountTypeID == Acc.fldID).FirstOrDefault();
                                    if (Cabin != null && Name.Count() > 3)
                                        Sys = m.sp_CarSystemSelect("fldName", Name[3], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldCabinTypeID == Cabin.fldID).FirstOrDefault();
                                    if (Sys != null && Name.Count() > 4)
                                        Tip = m.sp_CarModelSelect("fldName", Name[4], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldCarSystemID == Sys.fldID).FirstOrDefault();
                                    if (Tip != null && Name.Count() > 5)
                                        Class = m.sp_CarClassSelect("fldName", Name[5], 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(k => k.fldCarModelID == Tip.fldID).FirstOrDefault();

                                    switch (Name.Count() - 1)
                                    {
                                        case 1:
                                            Type = 1;
                                            Code = Make.fldID;
                                            break;
                                        case 2:
                                            Type = 2;
                                            Code = Acc.fldID;
                                            break;
                                        case 3:
                                            Type = 3;
                                            Code = Cabin.fldID;
                                            break;
                                        case 4:
                                            Type = 4;
                                            Code = Sys.fldID;
                                            break;
                                        case 5:
                                            Type = 5;
                                            Code = Tip.fldID;
                                            break;
                                        case 6:
                                            Type = 6;
                                            Code = Class.fldID;
                                            break;
                                    }

                                    var CarSeries = m.sp_GET_IDCarSeries(Type, Code).FirstOrDefault();
                                    short? fromModel = null, ToModel = null;
                                    byte? ToWheel = null, FromWheel = null, ToCylinder = null, FromCylinder = null;
                                    if (item.fldFromModel != null)
                                        fromModel = (short)item.fldFromModel;
                                    if (item.fldToModel != null)
                                        ToModel = (short)item.fldToModel;
                                    if (item.fldToWheel != null)
                                        ToWheel = (byte)item.fldToWheel;
                                    if (item.fldFromWheel != null)
                                        FromWheel = (byte)item.fldFromWheel;
                                    if (item.fldToCylinder != null)
                                        ToCylinder = (byte)item.fldToCylinder;
                                    if (item.fldFromCylinder != null)
                                        FromCylinder = (byte)item.fldFromCylinder;

                                    var CRate = m.sp_ComplicationsRateSelect("fldYear", item.fldYear.ToString(), 0,
                                        Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString())
                                        .Where(k => k.fldCarSeriesID == CarSeries.CarSeriesId && k.fldCountryDivisions == 1
                                            && k.fldFromModel == fromModel && k.fldToModel == ToModel
                                            && k.fldFromWheel == FromWheel && k.fldToWheel == ToWheel
                                            && k.fldFromCylinder == FromCylinder && k.fldToCylinder == ToCylinder
                                            && k.fldFromContentMotor == item.fldFromContentMotor &&
                                            k.fldToContentMotor == item.fldToContentMotor).FirstOrDefault();
                                    if (CRate != null)
                                        m.sp_ComplicationsRateUpdate(CRate.fldID, Type, Code, 0, 0, Convert.ToInt16(item.fldYear), FromCylinder, ToCylinder, FromWheel, ToWheel, fromModel, ToModel, item.fldFromContentMotor, item.fldToContentMotor, item.fldPrice, Convert.ToInt64(Session["UserId"]), "", Session["UserPass"].ToString());
                                    else
                                        m.sp_ComplicationsRateInsert(Type, Code, 0, 0, Convert.ToInt16(item.fldYear), FromCylinder, ToCylinder, FromWheel, ToWheel, fromModel, ToModel, item.fldFromContentMotor, item.fldToContentMotor, item.fldPrice, Convert.ToInt64(Session["UserId"]), "", Session["UserPass"].ToString());
                                }

                        }
                        return Json(new { Msg = "بارگذاری با موفقیت انجام شد.",MsgTitle="عملیات موفق" ,Er = 0 });
                    }
                    else
                    {
                        return Json(new {MsgTitle="خطا", Msg = "شما مجاز به استفاده از خدمات پشتیبانی نمی باشید، لطفا با واحد پشتیبانی تماس بگیرید.", Er = 1 });
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
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }
        public ActionResult SendToSupporter(string msg, string Year, int CarClassId, long carid)
        {
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (Session["UserId"] == null && Session["GeustId"] == null && Session["UserGeust"] == null)
            {
                //   return RedirectToAction("logon", "Account");
                if (ImageSetting == "1")//تهران
                {
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                }
                else
                {
                    return RedirectToAction("Index", "Account_New", new { area = "NewVer" });
                }
            }

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
                var car = p.sp_SelectCarDetils(carid).FirstOrDefault();
                fldCarClassName = car.fldCarClassName;
                fldCarModelName = car.fldCarModel;
                fldCarSystemName = car.fldCarSystemName;
                fldCabinTypeName = car.fldCarCabinName;
                fldCarAccountTypeName = car.fldCarAccountName;
                fldCarMakeName = car.fldCarMakeName;
            }
            else
            {
                var q = p.sp_CarClassSelect("fldId", CarClassId.ToString(), 1, 1, "").FirstOrDefault();
                fldCarClassName = q.fldName;
                fldCarModelName = q.fldCarModelName;
                var q1 = p.sp_CarModelSelect("fldId", q.fldCarModelID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
    }
}
