using Avarez.Controllers.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Models;
using System.Configuration;
using System.IO;
using System.IO.Compression;

namespace Avarez.Areas.NewVer.Controllers.Tools
{
    public class PardakhtFiles_NewController : Controller
    {
        //
        // GET: /NewVer/PardakhtFiles_New/

        public ActionResult Index()//آخرین تغییرات
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عملیات نهایی->ورود فایل پرداخت الکترونیک");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_GetDate().FirstOrDefault();
            PartialView.ViewBag.fldTarikhE = q.CurrentDateTime.ToString("yyyy-MM-dd");
            PartialView.ViewBag.fldTarikh_Sh = q.DateShamsi;
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters, string AzTarikh, string TaTarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_PardakhtFiles_DetailSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_PardakhtFiles_DetailSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldShenaseGhabz":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenaseGhabz";
                            break;
                        case "fldShomarePardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomarePardakht";
                            break;
                        case "fldTarikhPardakht":
                            searchtext = MyLib.Shamsi.Shamsi2miladiString(ConditionValue.Value.ToString());
                            searchtext = "%"+searchtext.Replace("/", "-").Replace("/", "-")+"%";
                            field = "fldTarikhPardakht";
                            break;
                        case "fldCodeRahgiry":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodeRahgiry";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                        case "fldGhabzNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldGhabzNumber";
                            break;
                        case "fldNahvePardakhtName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNahvePardakhtName";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_PardakhtFiles_DetailSelect(field, searchtext, Session["UserMnu"].ToString(),AzTarikh,TaTarikh, 0).ToList();
                    else
                        data = m.sp_PardakhtFiles_DetailSelect(field, searchtext, Session["UserMnu"].ToString(), AzTarikh, TaTarikh, 0).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                if (AzTarikh == "" || TaTarikh == "")
                {
                    data = m.sp_PardakhtFiles_DetailSelect("", "", Session["UserMnu"].ToString(), AzTarikh, TaTarikh, 100).ToList();
                }
                else
                {
                    data = m.sp_PardakhtFiles_DetailSelect("", "", Session["UserMnu"].ToString(), AzTarikh, TaTarikh, 0).ToList();
                }
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

            List<Avarez.Models.sp_PardakhtFiles_DetailSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult New(int Id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }

        public ActionResult PrintPardakhtFiles(string AzTarikh, string TaTarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.AzTarikh = AzTarikh;
            PartialView.ViewBag.TaTarikh = TaTarikh;
            return PartialView;
        }
        public ActionResult GeneratePDFPardakhtFiles(string AzTarikh, string TaTarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                dt.EnforceConstraints = false;
                Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                Avarez.DataSet.DataSet1TableAdapters.sp_PardakhtFiles_DetailSelectTableAdapter PardakhtFiles = new Avarez.DataSet.DataSet1TableAdapters.sp_PardakhtFiles_DetailSelectTableAdapter();
                sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                PardakhtFiles.Fill(dt.sp_PardakhtFiles_DetailSelect, "", "", Session["UserMnu"].ToString(), AzTarikh, TaTarikh, 0);

                Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
                var mnu = m.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var State = m.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_FTP.frx");
                Report.RegisterData(dt, "dataSet4");
                Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                var time = DateTime.Now;
                Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                Report.SetParameterValue("MunicipalityName", mnu.fldName);
                Report.SetParameterValue("StateName", State.fldName);
                Report.SetParameterValue("AreaName", Session["area"].ToString());
                Report.SetParameterValue("OfficeName", Session["office"].ToString());
                Report.SetParameterValue("StartDate", AzTarikh);
                Report.SetParameterValue("EndDate", TaTarikh);
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
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult UploadFileNew()
        {
            if (Session["savePathPF"] != null)
            {
                string physicalPath = System.IO.Path.Combine(Session["savePathPF"].ToString());
                Session.Remove("savePathPF");
                System.IO.File.Delete(physicalPath);
            }
            HttpPostedFileBase file = Request.Files[0];
            var e = System.IO.Path.GetExtension(Request.Files[0].FileName);
            if (file.ContentType == "application/zip")
            {
                if (file.ContentLength <= 5242880)
                {
                    //if (Session["savePath"] != null)
                    //{
                    //    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    //    Session.Remove("savePath");
                    //    Session.Remove("FileName");
                    //    System.IO.File.Delete(physicalPath);
                    //}                    
                    string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                    file.SaveAs(savePath);
                    Session["savePathPF"] = savePath;
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
                        Message = "حجم فایل انتخابی بیشتر از حدمجاز است."
                    });
                    DirectResult result = new DirectResult();
                    result.IsUpload = true;
                    return result;
                }
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "فرمت فایل انتخابی غیرمجاز است."
                });
                DirectResult result = new DirectResult();
                result.IsUpload = true;
                return result;
            }
        }

        public ActionResult checkDuplicateFile()
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var Duplicate = 0;
            if (Session["savePathPF"] == null)
            {
                return Json(new
                {
                    Msg = "لطفا فایل را انتخاب نمایید.",
                    MsgTitle = "خطا",
                    Er = 1,
                }, JsonRequestBehavior.AllowGet);
            }
            string FileName = System.IO.Path.GetFileName(Session["savePathPF"].ToString());
            var existfilee = p.sp_PardakhtFileSelect("fldFileName", FileName, 0).FirstOrDefault();
            if (existfilee != null)
            {
                Duplicate = 1;
            }
            return Json(new
            {
                Duplicate = Duplicate,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save()
        {
            string extractPathF = "";
            ZipArchive zipFile =null;
            try
            {
                List<string> FileNames = new List<string>();
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 314))
                {
                    if (Session["savePathPF"] == null)
                    {
                        return Json(new
                        {
                            Msg = "لطفا فایل را انتخاب نمایید.",
                            MsgTitle = "خطا",
                            Er = 1,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Models.cartaxEntities p = new Models.cartaxEntities();
                        var q = p.sp_GetAllShenaseGhabzInfo(Convert.ToInt32(Session["UserMnu"])).ToList();
                        System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));

                        zipFile = ZipFile.OpenRead(Session["savePathPF"].ToString());
                        extractPathF = Server.MapPath(@"~\Uploaded\ExtractFolder");
                        zipFile.ExtractToDirectory(extractPathF);
                        FileNames=Directory.GetFiles(extractPathF).ToList();

                        int y = 0;
                        foreach (var item in FileNames)
                        {
                            //var k = System.IO.File.ReadAllLines(Session["savePathPF"].ToString());
                            var k = System.IO.File.ReadAllLines(item);
                            string FileName = System.IO.Path.GetFileName(item);
                            /*var existfilee = p.sp_PardakhtFileSelect("fldFileName", FileName, 0).FirstOrDefault();
                            if (existfilee == null)
                            {*/
                            string type = System.IO.Path.GetFileName(item).Substring(0, 3);

                            var bank = p.sp_BankSelect("fldInfinitiveBank", type, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            
                            if (bank != null)
                            {
                                string date = System.IO.Path.GetFileName(item).Substring(3, 6);
                                p.sp_PardakhtFileInsert(_id, bank.fldID, FileName,
                                   MyLib.Shamsi.Shamsi2miladiDateTime("13" + date.Substring(0, 2) + "/" + date.Substring(2, 2) + "/" + date.Substring(4, 2)), Convert.ToInt32(Session["UserId"]), "");

                                for (int i = 1; i < k.Count(); i++)
                                {
                                    string shGhabz = k[i].Substring(14, 13).TrimStart('0');
                                    string shPardakht = k[i].Substring(27, 13).TrimStart('0');
                                    var isValid = q.Where(kk => kk.start == Convert.ToInt16(shGhabz.Substring(0, 2))).FirstOrDefault();
                                    if (isValid == null)
                                        continue;
                                    var divisionid = p.sp_GET_IDCountryDivisions(Convert.ToInt32(isValid.type), Convert.ToInt32(isValid.fldid)).FirstOrDefault();
                                    string date1 = k[i].Substring(8, 6);
                                    int type1 = Convert.ToInt32(k[i].Substring(6, 2));
                                    int count = Convert.ToInt32(shPardakht.Substring(shPardakht.Length - 5, 1));
                                    var fish = p.sp_GetFishWithShGhabz(shGhabz, shPardakht).FirstOrDefault();
                                    long? userid = Convert.ToInt64(Session["UserId"]);
                                    System.Data.Entity.Core.Objects.ObjectParameter c_id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                                    System.Data.Entity.Core.Objects.ObjectParameter p_id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                                    var SeettelType = p.sp_SettleTypeSelect("fldcode", type1.ToString(), 0, 1, "").FirstOrDefault();
                                    int price = Convert.ToInt32(shPardakht.Substring(0, shPardakht.Length - 5)) * 1000;

                                    if (shGhabz.Substring(0, 2) == isValid.start.ToString() &&
                                        shGhabz.Substring(shGhabz.Length - 2, 1) == isValid.fldServiceCode.ToString() &&
                                        shGhabz.Substring(shGhabz.Length - 5, 3) == isValid.fldInformaticesCode)
                                    {
                                        y++;
                                        p.sp_PardakhtFiles_DetailInsert(p_id, shGhabz, shPardakht,
                                                    MyLib.Shamsi.Shamsi2miladiDateTime("13" + date1.Substring(0, 2) + "/" + date1.Substring(2, 2) + "/" + date1.Substring(4, 2)),
                                                    k[i].Substring(k[i].Length - 6), SeettelType.fldID, Convert.ToInt32(_id.Value), Convert.ToInt32(divisionid.CountryDivisionId), Convert.ToInt32(Session["UserId"]), "");
                                        if (fish != null)
                                        {
                                            userid = fish.fldUserID;
                                            var collectionwithfish = p.sp_CollectionSelect("fldPeacockeryCode", fish.fldID.ToString(), 0,
                                                Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                            if (collectionwithfish == null)
                                            {
                                                p.sp_CollectionInsert(c_id, fish.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime("13" + date1.Substring(0, 2) + "/" + date1.Substring(2, 2) + "/" + date1.Substring(4, 2)),
                                                price, SeettelType.fldID, Convert.ToInt32(fish.fldID), null, "",
                                                userid, "پرداخت از طریق شناسه قبض و شناسه پرداخت", Session["UserPass"].ToString(),
                                                "", null, "", Convert.ToInt32(p_id.Value), null, true, 1, DateTime.Now);
                                                SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                                                if (Convert.ToInt32(Session["UserMnu"]) == 1)
                                                {
                                                    Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                                                    var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, fish.fldID.ToString(), MyLib.Shamsi.Shamsi2miladiDateTime("13" + date1.Substring(0, 2) + "/" + date1.Substring(2, 2) + "/" + date1.Substring(4, 2)), "");
                                                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                                           + k1 + "-" + fish.fldID.ToString() + "\n");
                                                }
                                                SmsSender sms = new SmsSender();
                                                sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, price, "", "", "");
                                            }
                                            else
                                            {
                                                p.sp_CollectionUpdate(collectionwithfish.fldID, fish.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime("13" + date1.Substring(0, 2) + "/" + date1.Substring(2, 2) + "/" + date1.Substring(4, 2)),
                                                price, SeettelType.fldID, Convert.ToInt32(fish.fldID), null, "",
                                                userid, "پرداخت از طریق شناسه قبض و شناسه پرداخت",
                                                Session["UserPass"].ToString(), "", null, "", Convert.ToInt32(p_id.Value), null);

                                                if (Convert.ToInt32(Session["UserMnu"]) == 1)
                                                {
                                                    Avarez.Hesabrayan.ServiseToRevenueSystems toRevenueSystems = new Avarez.Hesabrayan.ServiseToRevenueSystems();
                                                    toRevenueSystems.VitiationRecovrySendedFiche(3, 1, fish.fldID.ToString());
                                                    var k1 = toRevenueSystems.RecovrySendedFiche(3, 1, fish.fldID.ToString(), MyLib.Shamsi.Shamsi2miladiDateTime("13" + date1.Substring(0, 2) + "/" + date1.Substring(2, 2) + "/" + date1.Substring(4, 2)), "");
                                                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\b.txt", "ersal varizi: "
                                                           + k1 + "-" + fish.fldID.ToString() + "\n");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int carfile = Convert.ToInt32(shGhabz.Substring(0, shGhabz.Length - 5).Substring(2));
                                            if (count > 6 && count < 9)
                                                carfile = Convert.ToInt32(carfile.ToString() + shPardakht.Substring(shPardakht.Length - 4, 8 - count));
                                            var carfileId = p.sp_SingleCarfileSelect(carfile).FirstOrDefault();

                                            if (carfileId != null)
                                            {

                                                var collection = p.sp_SingleCollectionSelect(carfile, MyLib.Shamsi.Shamsi2miladiDateTime("13" + date1.Substring(0, 2) + "/" + date1.Substring(2, 2) + "/" + date1.Substring(4, 2)), price).FirstOrDefault();

                                                if (collection == null && fish == null)
                                                {
                                                    //var col = p.sp_CollectionSelect("fldPrice", price.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                                    //if (col == null)
                                                    //{
                                                    p.sp_CollectionInsert(c_id, carfileId.fldID, MyLib.Shamsi.Shamsi2miladiDateTime("13" + date1.Substring(0, 2) + "/" + date1.Substring(2, 2) + "/" + date1.Substring(4, 2)),
                                                        price, SeettelType.fldID, null, null, "", userid,
                                                        "پرداخت از طریق شناسه قبض و شناسه پرداخت", Session["UserPass"].ToString(),
                                                        "", null, "", Convert.ToInt32(p_id.Value), null, true, 1, DateTime.Now);
                                                    SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                                                    //}
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (System.IO.File.Exists(Session["savePathPF"].ToString()))
                        {
                            System.IO.Directory.Delete(extractPathF,true);
                            zipFile.Dispose();
                            System.IO.File.Delete(Session["savePathPF"].ToString());
                        }
                        if (Session["savePathPF"] != null)
                        {
                            Session.Remove("savePathPF");
                        }
                        return Json(new
                        {
                            Msg = "ذخیره با موفقیت انجام شد. تعداد رکورد قابل قبول: " + y,
                            MsgTitle = "ذخیره موفق",
                            Er = 0,
                        }, JsonRequestBehavior.AllowGet);
                    /*}
                    else
                    {
                        return Json(new
                        {
                            Msg = ".فایل انتخاب شده تکراری است",
                            MsgTitle = "خطا",
                            Er = 1,
                        }, JsonRequestBehavior.AllowGet);
                    }*/
                    }
                }
                else
                {
                    if (System.IO.File.Exists(Session["savePathPF"].ToString()))
                    {
                        System.IO.Directory.Delete(extractPathF, true);
                        zipFile.Dispose();
                        System.IO.File.Delete(Session["savePathPF"].ToString());
                    }
                    if (Session["savePathPF"] != null)
                    {
                        Session.Remove("savePathPF");
                    }
                    return Json(new
                    {
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        MsgTitle = "خطا",
                        Er = 1,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                if (System.IO.File.Exists(Session["savePathPF"].ToString()))
                {
                    System.IO.Directory.Delete(extractPathF, true);
                    zipFile.Dispose();
                    System.IO.File.Delete(Session["savePathPF"].ToString());
                }
                if (Session["savePathPF"] != null)
                {
                    Session.Remove("savePathPF");
                }
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    MsgTitle = "خطا",
                    Er = 1,
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult BankName()
        {
            string BankName = "";
            if (Session["savePathPF"] != null)
            {
                string type = System.IO.Path.GetFileName(Session["savePathPF"].ToString()).Substring(0, 3);
                Models.cartaxEntities p = new Models.cartaxEntities();
                var bank = p.sp_BankSelect("fldInfinitiveBank", type, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (bank != null)
                    BankName = bank.fldName;
            }
            return Json(BankName, JsonRequestBehavior.AllowGet);
        }
    }
}
