using Avarez.Controllers.Users;
using Avarez.Models;
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.NewVer.Controllers.Dashboard
{
    public class Filtering_NewController : Controller
    {
        //
        // GET: /NewVer/Filtering_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "داشبورد->پرونده");
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

        //public ActionResult test(string containerId)
        //{
        //    /*if (Session["UserId"] == null)
        //        return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
        //    var result = new Ext.Net.MVC.PartialViewResult();
        //    return result;*/
        //    return View();
        //}
        public ActionResult SearchCollection(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "داشبورد->جستجو در واریزی ها");
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
        public ActionResult Read(StoreRequestParameters parameters, string startDate, string endDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            long? sid = 0;
            byte type = 0;
            var userr=m.sp_UserSelect("fldId", Session["UserId"].ToString(), 1, Session["UserPass"].ToString(), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var country = m.sp_CountryDivisionsSelect("fldid", userr.fldCountryDivisionsID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            if (country.fldOfficesID != null && country.fldOfficesID == userr.fldID)
            {
                sid = country.fldOfficesID;
                type = 9;
            }
            else if (country.fldAreaID != null)
            {
                sid = country.fldAreaID;
                type = 7;
            }
            else if (country.fldCityID != null)
            {
                sid = country.fldCityID;
                type = 4;
            }
            else if (country.fldCountyID != null)
            {
                sid = country.fldCountyID;
                type = 2;
            }
            else if (country.fldLocalID != null)
            {
                sid = country.fldLocalID;
                type = 6;
            }
            else if (country.fldMunicipalityID != null)
            {
                sid = country.fldMunicipalityID;
                type = 5;
            }
            else if (country.fldOfficesID != null)
            {
                sid = country.fldOfficesID;
                type = 8;
            }
            else if (country.fldStateID != null)
            {
                sid = country.fldStateID;
                type = 1;
            }
            else if (country.fldZoneID != null)
            {
                sid = country.fldZoneID;
                type = 3;
            }
            List<Avarez.Models.sp_KolParvandeHaByUserId> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {

                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_KolParvandeHaByUserId> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldAcceptName":
                            searchtext = "%" + ConditionValue.Value.ToString()+"%" ;
                            field = "fldAcceptName";
                            break;
                        case "fldAcceptCollectionName":
                            searchtext = "%" + ConditionValue.Value.ToString()+"%";
                            field = "fldAcceptCollectionName";
                            break;
                        case "fldAcceptCarExperienceName":
                            searchtext = "%" + ConditionValue.Value.ToString()+"%" ;
                            field = "fldAcceptCarExperienceName";
                            break;
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOwnerName";
                            break;
                        case "fldMotorNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMotorNumber";
                            break;
                        case "fldShasiNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShasiNumber";
                            break;
                        case "fldVIN":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldVIN";
                            break;
                        case "carModel":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "carModel";
                            break;
                        case "CarClass":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "CarClass";
                            break;
                        case "fldModel":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldModel";
                            break;
                        case "fldUserName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldUserName";
                            break;
                        case "fldMobile":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMobile";
                            break;
                    }

                    if (data != null)

                        data1 = m.sp_KolParvandeHaByUserId(MyLib.Shamsi.Shamsi2miladiDateTime(startDate), MyLib.Shamsi.Shamsi2miladiDateTime(endDate),Convert.ToInt32(sid), type, field, searchtext, 0).ToList();
                    else
                        data = m.sp_KolParvandeHaByUserId(MyLib.Shamsi.Shamsi2miladiDateTime(startDate), MyLib.Shamsi.Shamsi2miladiDateTime(endDate), Convert.ToInt32(sid), type, field, searchtext, 0).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_KolParvandeHaByUserId(MyLib.Shamsi.Shamsi2miladiDateTime(startDate), MyLib.Shamsi.Shamsi2miladiDateTime(endDate), Convert.ToInt32(sid), type, "", "", 0).ToList();
            }

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            //return !oValue.ToString().Contains(value.ToString());
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Avarez.Models.sp_KolParvandeHaByUserId> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
            /*var data = p.sp_KolParvandeHa(MyLib.Shamsi.Shamsi2miladiDateTime(start),
                MyLib.Shamsi.Shamsi2miladiDateTime(end)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);*/
        }

        public ActionResult ReadCollection(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_CollectionSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_CollectionSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldCollectionDate":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldCollectionDate";
                            break;
                        case "fldPrice":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPrice";
                            break;
                        case "fldPeacockeryCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPeacockeryCode";
                            break;
                        case "fldTrackCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTrackCode";
                            break;
                        case "fldSettleTypeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSettleTypeName";
                            break;
                        case "fldSerialBarChasb":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSerialBarChasb";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_CollectionSelect(field, searchtext, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_CollectionSelect(field, searchtext, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_CollectionSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            return !oValue.ToString().Contains(value.ToString());
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Avarez.Models.sp_CollectionSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Fishes(string containerId, int CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 378))
            {
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                result.ViewBag.CarFileId = CarFileId;
                return result;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult ReadFishes(int CarfileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.sp_PeacockerySelect("fldCarID", car.fldCarID.ToString(), 0, 1, "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult printFish(int FishId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.FishId = FishId;
            return PartialView;
        }

        //public static byte[] StrToByteArray(string str)
        //{
        //    Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
        //    for (int i = 0; i <= 255; i++)
        //        hexindex.Add(i.ToString("X2"), (byte)i);

        //    List<byte> hexres = new List<byte>();
        //    for (int i = 0; i < str.Length; i += 2)
        //        hexres.Add(hexindex[str.Substring(i, 2)]);

        //    return hexres.ToArray();
        //}
        public ActionResult RptFish(int FishId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var peacokery_copy = p.Sp_Peacockery_CopySelect(FishId).FirstOrDefault();
            if (peacokery_copy != null)
            {
                return File(peacokery_copy.fldCopy, "application/pdf");
            }
            return null;
        }
        public ActionResult PicCar(int CarID, string carFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Models.cartaxEntities p = new Models.cartaxEntities();
            var q=p.sp_CarFileSelect("fldId", carFileId, 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            PartialView.ViewBag.CarID = CarID;
            PartialView.ViewBag.bargsabz = q.fldBargSabzFileId;
            PartialView.ViewBag.sanad = q.fldSanadForoshFileId;
            PartialView.ViewBag.cart = q.fldCartFileId;
            PartialView.ViewBag.backcart = q.fldCartBackFileId;
            PartialView.ViewBag.carFileId = carFileId;
            return PartialView;
        }
        public ActionResult DetailsPicCar(int CarID)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities Car = new Models.cartaxEntities();
            var car = Car.sp_SelectCarDetils(CarID).FirstOrDefault();
            var file = Car.sp_CarFileSelect("fldID", car.fldID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

            return Json(new
            {
                fldId = file.fldID,
                fldBargSabzFileId = file.fldBargSabzFileId,
                fldCartFileId = file.fldCartFileId,
                fldSanadForoshFileId = file.fldSanadForoshFileId,
                fldCartBackFileId = file.fldCartBackFileId,
                Er = 0
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Pardakhtha(string containerId, int CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 379))
            {
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                result.ViewBag.CarFileId = CarFileId;
                return result;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult printResid(int id, int Type)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.Type = Type;
            return PartialView;
        }
        public ActionResult ReadPardakhta(int CarfileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.sp_CollectionSelect("fldCarID", car.fldCarID.ToString(), 0, 1, "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult printResid(string containerId, int CollId)
        //{
        //    if (Session["UserId"] == null)
        //        return RedirectToAction("LogOn", "Account_New");
        //    var result = new Ext.Net.MVC.PartialViewResult
        //    {
        //        WrapByScriptTag = true,
        //        ContainerId = containerId,
        //        RenderMode = RenderMode.AddTo
        //    };
        //    this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
        //    result.ViewBag.CollId = CollId;
        //    return result;
        //}
        public ActionResult RptResid(int CollId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter resid = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter carDitail = new Avarez.DataSet.DataSet1TableAdapters.sp_SelectCarDetilsTableAdapter();
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            var q = car.rpt_Receipt(CollId, 1).FirstOrDefault();
            if (q == null)
                return null;
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();
            carDitail.Fill(dt.sp_SelectCarDetils, Convert.ToInt32(q.fldCarId));
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
            resid.Fill(dt.rpt_Receipt, CollId, 1);
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Rpt_Resid.frx");
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

        public ActionResult Svabegh(string containerId, int CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 380))
            {
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                result.ViewBag.CarFileId = CarFileId;
                return result;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult ReadSavabegh(int CarfileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.sp_CarExperienceSelect("fldCarID", car.fldCarID.ToString(), 0, 1, "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Mafasa(string containerId, int CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 381))
            {
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                result.ViewBag.CarFileId = CarFileId;
                return result;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult ReadMafasa(int CarfileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.Sp_MafasaSelect((int)car.fldCarID).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult printMafasa(string MafasaId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.MafasaId = MafasaId;
            return PartialView;
        }
        public ActionResult RptMafasa(string MafasaId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var peacokery_copy = p.Sp_MafasaSelect_Image(MafasaId.ToString()).FirstOrDefault();
            if (peacokery_copy != null)
            {
                return File(peacokery_copy.fldimage, "application/pdf");
            }
            return null;
        }
        public ActionResult Archive(int CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 382))
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.CarFileId = CarFileId;
                return PartialView;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
            
        }
        public ActionResult ReadArchive(int CarfileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.sp_DigitalArchiveSelect("fldCarID", car.fldCarID.ToString(), 0).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PicsOfArchive(int ArchiveId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ArchiveId = ArchiveId;
            return PartialView;
        }
        public ActionResult GetImages(int? ArchiveId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            //string path = "~/Areas/DataView_Basic/Content/images/touch-icons/";
            // string serverPath = Server.MapPath(path);
            //string[] files = System.IO.Directory.GetFiles(serverPath);

            List<object> data = new List<object>();
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pics = p.sp_DigitalArchive_DetailSelect("fldDigitalArchiveId", ArchiveId.ToString(), 0).ToList();
            if (pics != null)
                foreach (var pic in pics)
                {
                    data.Add(new
                    {
                        url = Url.Content("~/ListImageInTree/Image/" + pic.fldId),
                        ID = pic.fldId
                    });
                }
            return this.Store(data);
        }
        public ActionResult ShowPic(int id)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }
        public FileContentResult Show(int id)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_DigitalArchive_DetailSelect("fldId", id.ToString(), 0).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldPic != null)
                {
                    return File((byte[])pic.fldPic, "jpg");
                }
            }
            return null;


        }
        public ActionResult BlackList(string containerId, int CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 383))
            {
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                result.ViewBag.CarFileId = CarFileId;
                return result;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult ReadBlackList(int CarfileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.sp_ListeSiyahSelect("fldCarId", car.fldCarID.ToString(), 30).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NewBlackList(int id, int CarfileId)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;

            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            PartialView.ViewBag.CarID = car.fldCarID;
            return PartialView;
        }
        public ActionResult SaveBlackList(Models.sp_ListeSiyahSelect a)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            string Msg = "", MsgTitle = "";
            try
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                if (a.fldDesc == null)
                    a.fldDesc = "";
                if (a.fldId == 0)
                { //ذخیره
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 384))
                    {
                        m.sp_ListeSiyahInsert(a.fldCarId, a.fldType, a.fldMsg, Convert.ToInt32(Session["UserId"]), a.fldDesc);
                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                { //ویرایش
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 385))
                    {
                        m.sp_ListeSiyahUpdate(a.fldId, a.fldCarId, a.fldType, a.fldMsg, Convert.ToInt32(Session["UserId"]), a.fldDesc);
                        Msg = "ویرایش با موفقیت انجام شد.";
                        MsgTitle = "ویرایش موفق";
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = 0
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteBlackList(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            string Msg = "", MsgTitle = "";

            try
            {
                //حذف
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 386))
                {
                    Models.cartaxEntities m = new Models.cartaxEntities();
                    m.sp_ListeSiyahDelete(id, Convert.ToInt32(Session["UserId"]));
                    Msg = "حذف با موفقیت انجام شد.";
                    MsgTitle = "حذف موفق";
                }
                else
                {
                    return Json(new
                    {
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DetailsBlackList(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_ListeSiyahSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldId,
                    CarId = q.fldCarId,
                    fldType = q.fldType.ToString(),
                    fldMsg = q.fldMsg
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
        public ActionResult AccPicCar(int State, int Id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            //var car = p.sp_SelectCarDetils(Id).FirstOrDefault();
            var file = p.sp_CarFileSelect("fldid", Id.ToString(), 0, 1, "").FirstOrDefault();
            var fileId = file.fldSanadForoshFileId;
            if (State == 1)
                fileId = file.fldBargSabzFileId;
            else if (State == 2)
                fileId = file.fldCartFileId;
            else if (State == 3)
                fileId = file.fldCartBackFileId;
            p.sp_FilesUpdateAcc(fileId, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
            p.sp_CarFileAccept(Id, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
            return Json(new
            {
                Msg = "با موفقیت تایید شد.",
                MsgTitle = "عملیات موفق"
            }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult LoadLblAcc(int Id)
        //{
        //    Models.cartaxEntities p = new Models.cartaxEntities();
        //    var car = p.sp_SelectCarDetils(Id).FirstOrDefault();
        //    var file = p.sp_CarFileSelect("fldid", car.fldID.ToString(), 0, 1, "").FirstOrDefault();
        //    bool? fldIsAcceptSanad = null;
        //    var fldKarbarTaeedKonandeSanad = "";
        //    bool? fldIsAcceptCart = null;
        //    var fldKarbarTaeedKonandeCart = "";
        //    bool? fldIsAcceptBargSabz = null;
        //    var fldKarbarTaeedKonandeBargSabz = "";
        //    bool? fldIsAcceptCartBack = null;
        //    var fldKarbarTaeedKonandeCartBack = "";

        //    var f = p.Sp_FilesSelect(file.fldSanadForoshFileId).FirstOrDefault();
        //    if (f != null)
        //    {
        //        fldIsAcceptSanad = f.fldIsAccept;
        //        fldKarbarTaeedKonandeSanad = f.fldNameKarbar;
        //    }

        //    f = p.Sp_FilesSelect(file.fldBargSabzFileId).FirstOrDefault();
        //    if (f != null)
        //    {
        //        fldIsAcceptBargSabz = f.fldIsAccept;
        //        fldKarbarTaeedKonandeBargSabz = f.fldNameKarbar;
        //    }

        //    f = p.Sp_FilesSelect(file.fldCartFileId).FirstOrDefault();
        //    if (f != null)
        //    {
        //        fldIsAcceptCart = f.fldIsAccept;
        //        fldKarbarTaeedKonandeCart = f.fldNameKarbar;
        //    }

        //    f = p.Sp_FilesSelect(file.fldCartBackFileId).FirstOrDefault();
        //    if (f != null)
        //    {
        //        fldIsAcceptCartBack = f.fldIsAccept;
        //        fldKarbarTaeedKonandeCartBack = f.fldNameKarbar;
        //    }

        //    var HaveTaiid = false;
        //    if (fldKarbarTaeedKonandeSanad != "" || fldKarbarTaeedKonandeBargSabz != "" || fldKarbarTaeedKonandeCart != "" || fldKarbarTaeedKonandeCartBack != "")
        //        HaveTaiid = true;
        //    return Json(new
        //                {
        //                    HaveTaiid=HaveTaiid,
        //                    fldIsAcceptSanad = fldIsAcceptSanad,
        //                    fldKarbarTaeedKonandeSanad = fldKarbarTaeedKonandeSanad,
        //                    fldIsAcceptBargSabz = fldIsAcceptBargSabz,
        //                    fldKarbarTaeedKonandeBargSabz = fldKarbarTaeedKonandeBargSabz,
        //                    fldIsAcceptCart = fldIsAcceptCart,
        //                    fldKarbarTaeedKonandeCart = fldKarbarTaeedKonandeCart,
        //                    fldIsAcceptCartBack = fldIsAcceptCartBack,
        //                    fldKarbarTaeedKonandeCartBack = fldKarbarTaeedKonandeCartBack
        //                }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult AccMadrakSavabegh(int id, int CarfileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            byte Er = 0; string Msg = ""; string MsgTitle = "";
            try
            {
                var carEx = p.sp_CarExperienceSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
                p.sp_FilesUpdateAcc(carEx.fldFileId, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
                p.sp_CarExperienceAccept(id, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
               /* SignalrHub r = new SignalrHub();
                r.ReloadCarExperience();*/
                MsgTitle = "عملیات موفق";
                Msg = "با موفقیت تایید شد.";
            }
            catch (Exception x)
            {
                Msg = x.Message;
                if (x.InnerException != null)
                {
                    Msg = x.InnerException.Message;
                }
                MsgTitle = "خطا";
                Er = 1;
            }
            
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckTaiidSavabegh(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carEx = p.sp_CarExperienceSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
            /*var f = p.Sp_FilesSelect(carEx.fldFileId).FirstOrDefault();*/
            return Json(new
            {
                HaveTaiid = carEx.fldAccept
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AccPardakhtha(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carEx = p.sp_CollectionSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();

            p.sp_FilesUpdateAcc(carEx.fldFileId, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
            p.sp_CollectionAccept(id, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
            return Json(new
            {
                Msg = "با موفقیت تایید شد.",
                MsgTitle = "عملیات موفق"
            }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult CheckTaiidPardakhtha(int id)
        //{
        //    Models.cartaxEntities p = new Models.cartaxEntities();
        //    var carEx = p.sp_CollectionSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
        //    bool? HaveTaiid=true;
        //    if(carEx.fldFileId!=null)
        //        HaveTaiid = p.Sp_FilesSelect(carEx.fldFileId).FirstOrDefault().fldIsAccept;
        //    return Json(new
        //    {
        //        HaveTaiid = HaveTaiid
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult AccPicCar(int State, int Id)
        //{
        //    Models.cartaxEntities p = new Models.cartaxEntities();
        //    var car = p.sp_SelectCarDetils(Id).FirstOrDefault();
        //    var file = p.sp_CarFileSelect("fldid", car.fldID.ToString(), 0, 1, "").FirstOrDefault();
        //    var fileId = file.fldSanadForoshFileId;
        //    if (State == 1)
        //        fileId = file.fldBargSabzFileId;
        //    else if (State == 2)
        //        fileId = file.fldCartFileId;
        //    else if (State == 3)
        //        fileId = file.fldCartBackFileId;
        //    p.sp_FilesUpdateAcc(fileId, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
        //    return Json(new
        //    {
        //        Msg = "با موفقیت تایید شد.",
        //        MsgTitle = "عملیات موفق"
        //    }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult LoadLblAcc(int Id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            //var car = p.sp_SelectCarDetils(Id).FirstOrDefault();
            var file = p.sp_CarFileSelect("fldid", Id.ToString(), 0, 1, "").FirstOrDefault();
            bool? fldIsAcceptSanad = null;
            var fldKarbarTaeedKonandeSanad = "";
            bool? fldIsAcceptCart = null;
            var fldKarbarTaeedKonandeCart = "";
            bool? fldIsAcceptBargSabz = null;
            var fldKarbarTaeedKonandeBargSabz = "";
            bool? fldIsAcceptCartBack = null;
            var fldKarbarTaeedKonandeCartBack = "";

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
                HaveTaiid = true;
            return Json(new
            {
                HaveTaiid = HaveTaiid,
                fldIsAcceptSanad = fldIsAcceptSanad,
                fldKarbarTaeedKonandeSanad = fldKarbarTaeedKonandeSanad,
                fldIsAcceptBargSabz = fldIsAcceptBargSabz,
                fldKarbarTaeedKonandeBargSabz = fldKarbarTaeedKonandeBargSabz,
                fldIsAcceptCart = fldIsAcceptCart,
                fldKarbarTaeedKonandeCart = fldKarbarTaeedKonandeCart,
                fldIsAcceptCartBack = fldIsAcceptCartBack,
                fldKarbarTaeedKonandeCartBack = fldKarbarTaeedKonandeCartBack
            }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult AccMadrakSavabegh(int id, int CarfileId)
        //{
        //    Models.cartaxEntities p = new Models.cartaxEntities();
        //    var carEx = p.sp_CarExperienceSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();

        //    p.sp_FilesUpdateAcc(carEx.fldFileId, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
        //    return Json(new
        //    {
        //        Msg = "با موفقیت تایید شد.",
        //        MsgTitle = "عملیات موفق"
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult CheckTaiidSavabegh(int id)
        //{
        //    Models.cartaxEntities p = new Models.cartaxEntities();
        //    var carEx = p.sp_CarExperienceSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
        //    var f = p.Sp_FilesSelect(carEx.fldFileId).FirstOrDefault();
        //    return Json(new
        //    {
        //        HaveTaiid=f.fldIsAccept
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult AccPardakhtha(int id)
        //{
        //    Models.cartaxEntities p = new Models.cartaxEntities();
        //    var carEx = p.sp_CollectionSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();

        //    p.sp_FilesUpdateAcc(carEx.fldFileId, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
        //    return Json(new
        //    {
        //        Msg = "با موفقیت تایید شد.",
        //        MsgTitle = "عملیات موفق"
        //    }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult CheckTaiidPardakhtha(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carEx = p.sp_CollectionSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
            bool? HaveTaiid = true;
            if (carEx.fldFileId != null)
                HaveTaiid = p.Sp_FilesSelect(carEx.fldFileId).FirstOrDefault().fldIsAccept;
            return Json(new
            {
                HaveTaiid = HaveTaiid
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
