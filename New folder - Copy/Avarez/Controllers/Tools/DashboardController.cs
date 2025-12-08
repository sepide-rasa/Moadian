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

namespace Avarez.Controllers.Tools
{
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات پایه->داشبورد");
            return View();
        }
        public ActionResult Filtering()
        {
            return PartialView();
        }
        public ActionResult FishChart()
        {
            return PartialView();
        }
        public StoreResult FishGetData(int? mah)
        {
            string date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
            DateTime start;
            if (mah == null)
                mah = Convert.ToInt32(date.Substring(5, 2));
            start = MyLib.Shamsi.Shamsi2miladiDateTime(date.Substring(0, 4) +
                "/" + mah.ToString().PadLeft(2, '0') + "/01");
            DateTime end; int roz = 31;
            if (mah > 6 && mah < 12)
                roz = 30;
            else if (mah == 12)
            {
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(date.Substring(0, 4))))
                    roz = 30;
                else
                    roz = 29;
            }

            end = MyLib.Shamsi.Shamsi2miladiDateTime(date.Substring(0, 4) +
                "/" + mah.ToString().PadLeft(2, '0') + "/" + roz.ToString().PadLeft(2, '0'));
            return new StoreResult(ChartModel.GenerateFishData(Convert.ToInt32(Session["UserMnu"]), start, end));
        }

        public ActionResult PieChart()
        {
            return PartialView();
        }
        public StoreResult PieGetData(int? mah)
        {
            string date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
            DateTime start;
            if (mah == null)
                mah = Convert.ToInt32(date.Substring(5, 2));
            start = MyLib.Shamsi.Shamsi2miladiDateTime(date.Substring(0, 4) +
                "/" + mah.ToString().PadLeft(2, '0') + "/01");
            DateTime end; int roz = 31;
            if (mah > 6 && mah < 12)
                roz = 30;
            else if (mah == 12)
            {
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(date.Substring(0, 4))))
                    roz = 30;
                else
                    roz = 29;
            }

            end = MyLib.Shamsi.Shamsi2miladiDateTime(date.Substring(0, 4) +
                "/" + mah.ToString().PadLeft(2, '0') + "/" + roz.ToString().PadLeft(2, '0'));
            return new StoreResult(ChartModel.GeneratePieData(Convert.ToInt32(Session["UserMnu"]), start, end));
        }

        public ActionResult Read(string start, string end)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();

            var data = p.sp_KolParvandeHa(MyLib.Shamsi.Shamsi2miladiDateTime(start),
                MyLib.Shamsi.Shamsi2miladiDateTime(end)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Fishes(string containerId, int CarFileId)
        {
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
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.sp_PeacockerySelect("fldCarID", car.fldCarID.ToString(), 0, 1, "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult printFish(string containerId, int FishId)
        {
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.FishId = FishId;
            return result;
        }
        public ActionResult RptFish(int FishId)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var peacokery_copy = p.Sp_Peacockery_CopySelect(FishId).FirstOrDefault();
            if (peacokery_copy != null)
            {
                return File(peacokery_copy.fldCopy, "application/pdf");
            }
            return null;
        }
        public ActionResult PicCar(int CarID)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.CarID = CarID;
            return PartialView;
        }
        public ActionResult DetailsPicCar(int CarID)
        {
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
        public ActionResult ReadPardakhta(int CarfileId)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.sp_CollectionSelect("fldCarID", car.fldCarID.ToString(), 0, 1, "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult printResid(string containerId, int CollId)
        {
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.CollId = CollId;
            return result;
        }
        public ActionResult RptResid(int CollId)
        {
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
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.sp_CarExperienceSelect("fldCarID", car.fldCarID.ToString(), 0, 1, "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Mafasa(string containerId, int CarFileId)
        {
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
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.Sp_MafasaSelect((int)car.fldCarID).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult printMafasa(string containerId, string MafasaId)
        {
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.MafasaId = MafasaId;
            return result;
        }
        public ActionResult RptMafasa(string MafasaId)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var peacokery_copy = p.Sp_MafasaSelect_Image(MafasaId.ToString()).FirstOrDefault();
            if (peacokery_copy != null)
            {
                return File(peacokery_copy.fldimage, "application/pdf");
            }
            return null;
        }
        public ActionResult Archive(string containerId, int CarFileId)
        {
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 382))
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
        public ActionResult ReadArchive(int CarfileId)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.sp_DigitalArchiveSelect("fldCarID", car.fldCarID.ToString(), 0).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PicsOfArchive(string containerId, int ArchiveId)
        {
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.ArchiveId = ArchiveId;
            return result;
        }
        public ActionResult GetImages(int? ArchiveId)
        {
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
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            var data = p.sp_ListeSiyahSelect("fldCarId", car.fldCarID.ToString(), 30).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NewBlackList(int id, int CarfileId)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;

            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_CarFileSelect("fldid", CarfileId.ToString(), 0, 1, "").FirstOrDefault();
            PartialView.ViewBag.CarID = car.fldCarID;
            return PartialView;
        }
        public ActionResult SaveBlackList(Models.sp_ListeSiyahSelect a)
        {
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
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_SelectCarDetils(Id).FirstOrDefault();
            var file = p.sp_CarFileSelect("fldid", car.fldID.ToString(), 0, 1, "").FirstOrDefault();
            var fileId = file.fldSanadForoshFileId;
            if (State == 1)
                fileId = file.fldBargSabzFileId;
            else if (State == 2)
                fileId = file.fldCartFileId;
            else if (State == 3)
                fileId = file.fldCartBackFileId;
            p.sp_FilesUpdateAcc(fileId, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
            p.sp_CarFileAccept(car.fldID, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
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
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carEx = p.sp_CarExperienceSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
           
            p.sp_FilesUpdateAcc(carEx.fldFileId, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
            p.sp_CarExperienceAccept(id, true, Convert.ToInt32(Session["UserId"]), p.sp_GetDate().FirstOrDefault().CurrentDateTime);
            return Json(new
            {
                Msg = "با موفقیت تایید شد.",
                MsgTitle = "عملیات موفق"
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckTaiidSavabegh(int id)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carEx = p.sp_CarExperienceSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
            var f = p.Sp_FilesSelect(carEx.fldFileId).FirstOrDefault();
            return Json(new
            {
                HaveTaiid=f.fldIsAccept
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AccPardakhtha(int id)
        {
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
            Models.cartaxEntities p = new Models.cartaxEntities();
            var car = p.sp_SelectCarDetils(Id).FirstOrDefault();
            var file = p.sp_CarFileSelect("fldid", car.fldID.ToString(), 0, 1, "").FirstOrDefault();
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
                            HaveTaiid=HaveTaiid,
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
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carEx = p.sp_CollectionSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
            bool? HaveTaiid=true;
            if(carEx.fldFileId!=null)
                HaveTaiid = p.Sp_FilesSelect(carEx.fldFileId).FirstOrDefault().fldIsAccept;
            return Json(new
            {
                HaveTaiid = HaveTaiid
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
