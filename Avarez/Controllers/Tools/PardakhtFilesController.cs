using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
using System.IO;
namespace Avarez.Controllers.Tools
{
    public class PardakhtFilesController : Controller
    {
        //
        // GET: /PardakhtFiles/

        public ActionResult Index()
        {
            if (Session["UserState"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 313))
            {
                Session["field"] = "";
                Session["Value"] = "";
                Session["Value2"] = "";
                Session["top"] = "30";
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        //public ActionResult Upload()
        //{
        //    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 313))
        //    {
        //        var file = Request.Files["Filedata"];
        //        string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
        //        file.SaveAs(savePath);
        //        Session["savePath"] = savePath;
        //        return Content(Url.Content(@"~\Uploaded\" + file.FileName));
        //    }
        //    else
        //        return null;
        //}
        public ActionResult UploadContent(HttpPostedFileBase UptContent)
        {
            if (UptContent != null)
            {
                if (UptContent.ContentLength <= 5242880)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(UptContent.FileName);
                    string savePath = Server.MapPath(@"~\Uploaded\" + fileName);

                    Session["savePath"] = savePath;
                    // The files are not actually saved in this demo
                    UptContent.SaveAs(savePath);
                }
                else
                {
                    Session["er"] = "حجم فایل بزرگتر از حد مجاز است. ";
                    return Content("");
                }
            }
            return Content("");
        }
        public ActionResult RemoveContent(string fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                string physicalPath = Server.MapPath(@"~\Uploaded\" + fileNames);
                if (System.IO.File.Exists(physicalPath))
                {
                    // The files are not actually removed in this demo
                    System.IO.File.Delete(physicalPath);
                }
                Session.Remove("savePath");
            }
            return Content("");
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var t = m.sp_GET_IDCountryDivisions(1, 1).FirstOrDefault();
            var q = m.sp_PardakhtFiles_DetailSelect(Session["field"].ToString(), Session["Value"].ToString(), Session["UserMnu"].ToString(), Session["Value2"].ToString(),"", Convert.ToInt32(Session["top"])).ToList().ToDataSourceResult(request);
            return Json(q);

        }

        public ActionResult GetFileInf()
        {
            string BankName = "";
            if (Session["savePath"] != null)
            {
                string type = System.IO.Path.GetFileName(Session["savePath"].ToString()).Substring(0, 3);
                Models.cartaxEntities p = new Models.cartaxEntities();
                var bank = p.sp_BankSelect("fldInfinitiveBank", type, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (bank != null)
                    BankName = bank.fldName;
            }
            return Json(BankName,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reload(string field, string value, string value2, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldShenaseGhabz", "fldShomarePardakht", "fldTarikhPardakht" , "fldCodeRahgiry","fldBankName","fldGhabzNumber","AzTarikh_TaTarikh",""};
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            //Models.cartaxEntities m = new Models.cartaxEntities();
            //var q = m.sp_PardakhtFiles_DetailSelect(_fiald[Convert.ToInt32(field)], searchtext, Session["UserMnu"].ToString(), top).ToList();
            //return Json(q, JsonRequestBehavior.AllowGet);
            Session["field"] = _fiald[Convert.ToInt32(field)];
            Session["Value"] = searchtext;
            Session["Value2"] = value2;
            Session["top"] = top;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save()
        {
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 314))
                {
                    Models.cartaxEntities p = new Models.cartaxEntities();
                    var q = p.sp_GetAllShenaseGhabzInfo(Convert.ToInt32(Session["UserMnu"])).ToList();
                    System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));

                    var k = System.IO.File.ReadAllLines(Session["savePath"].ToString());
                    string FileName=System.IO.Path.GetFileName(Session["savePath"].ToString());
                    string type = System.IO.Path.GetFileName(Session["savePath"].ToString()).Substring(0, 3);

                    var bank = p.sp_BankSelect("fldInfinitiveBank", type, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    int y = 0;
                    if (bank != null)
                    {
                        string date = System.IO.Path.GetFileName(Session["savePath"].ToString()).Substring(3, 6);
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
                                        SmsSender sms = new SmsSender();
                                        sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, price, "", "", "");
                                    }
                                    else
                                    {
                                        p.sp_CollectionUpdate(collectionwithfish.fldID, fish.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime("13" + date1.Substring(0, 2) + "/" + date1.Substring(2, 2) + "/" + date1.Substring(4, 2)),
                                        price, SeettelType.fldID, Convert.ToInt32(fish.fldID), null, "",
                                        userid, "پرداخت از طریق شناسه قبض و شناسه پرداخت",
                                        Session["UserPass"].ToString(), "", null, "", Convert.ToInt32(p_id.Value), null);
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
                                            //}
                                        }

                                    }
                                }
                                
                            }
                        }
                    }
                    if (System.IO.File.Exists(Session["savePath"].ToString()))
                        System.IO.File.Delete(Session["savePath"].ToString());
                    return Json(new { data = "ذخیره با موفقیت انجام شد. تعداد رکورد قابل قبول: " + y, state = 0 },JsonRequestBehavior.AllowGet);
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
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CheckName()
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                string FileName = System.IO.Path.GetFileName(Session["savePath"].ToString());
                bool IsNew=true;
                var k = Car.sp_PardakhtFileSelect("fldFileName", FileName, 0).FirstOrDefault();
                if (k != null)
                    IsNew = false;

                return Json(new
                {
                    IsNew = IsNew
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
        public ActionResult Rptprint(string field, string value, string value2, int top, int searchtype, string SearchFilter)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 298))
            //{
            string[] _fiald = new string[] { "fldShenaseGhabz", "fldShomarePardakht", "fldTarikhPardakht", "fldCodeRahgiry", "fldBankName", "fldGhabzNumber", "AzTarikh_TaTarikh","" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
           
            Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
            Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_PardakhtFileSelectTableAdapter Pardakht = new Avarez.DataSet.DataSet1TableAdapters.sp_PardakhtFileSelectTableAdapter();
            Avarez.DataSet.DataSet1TableAdapters.sp_PardakhtFiles_DetailSelectTableAdapter Pardakht_detail = new Avarez.DataSet.DataSet1TableAdapters.sp_PardakhtFiles_DetailSelectTableAdapter();
            sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            //Peacockery.Fill(dt.sp_RptPeacockery, "Paid", Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(SDate.ToString()), MyLib.Shamsi.Shamsi2miladiDateTime(EDate.ToString()));
            Pardakht_detail.Fill(dt.sp_PardakhtFiles_DetailSelect, _fiald[Convert.ToInt32(field)], searchtext,value2,"","", top);
            Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
            Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
            var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptPardakhtFile.frx");
            Report.RegisterData(dt, "CarTaxDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
            Report.SetParameterValue("MunicipalityName", mnu.fldName);
            Report.SetParameterValue("StateName", State.fldName);
            Report.SetParameterValue("AreaName", Session["area"].ToString());
            Report.SetParameterValue("OfficeName", Session["office"].ToString());
            Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
            Report.SetParameterValue("SearchFilter", SearchFilter);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
            //}
            //else
            //{
            //    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
            //    return RedirectToAction("error", "Metro");
            //}
        }
    }
}
