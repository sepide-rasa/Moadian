using Avarez.Areas.Tax.Models;
using Avarez.Models;
using Ext.Net;
using Ext.Net.MVC;
using FastMember;
using Microsoft.CSharp.RuntimeBinder;
using MyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TaxCollectData.Library.Abstraction.Clients;
using TaxCollectData.Library.Abstraction.Cryptography;
using TaxCollectData.Library.Algorithms;
using TaxCollectData.Library.Dto;
using TaxCollectData.Library.Factories;
using TaxCollectData.Library.Models;
using TaxCollectData.Library.Properties;
using TaxCollectData.Library.Providers;

namespace Avarez.Areas.Tax.Controllers
{
    public class ErsalFromExcelController : Controller
    {
        //
        // GET: /Tax/ErsalFromExcel/

        public ActionResult Index()
        {//باز شدن تب جدید
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            return new Ext.Net.MVC.PartialViewResult();


        }
        public ActionResult Upload()
        {
            string Msg = "";
            try
            {
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    Session.Remove("FileName");
                    System.IO.File.Delete(physicalPath);
                }
                HttpPostedFileBase file = Request.Files[0];
                string e = Path.GetExtension(file.FileName);

                if (e.ToLower() == ".xls" || e.ToLower() == ".xlsx")
                {
                    string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                    file.SaveAs(savePath);
                    Session["FileName"] = file.FileName;
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
        public ActionResult ReloadRecords()
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            Avarez.Models.cartaxEntities p = new Avarez.Models.cartaxEntities();
            var TransactionInf = p.sp_TransactionInfSelect("fldTarfGharardadId", Session["TarafGharardadId"].ToString(), 0).FirstOrDefault();
            Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
            var divName = TransactionInf.fldCountryDivisionsName;
            if (divName == "سطح کشوری") divName = "جمهوری اسلامی ایران";
            var y = h.CheckAccountCharge(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, divName);

            cartaxtest2Entities m = new cartaxtest2Entities();
            var HeaderIds = ProcessXlsRecords(Session["savePath"].ToString());
            var ListRecords = m.prs_tblSooratHesab_HeaderSelect("HeaderIds", HeaderIds,"","", 0).ToList();
            //var headers = HeaderIds.Split(';');
            ErsalBeMoadian k = new ErsalBeMoadian();
            foreach (var item in ListRecords)
            {
                if (item.fldStatus == 0)
                {
                    if (y.HaveCharge && y.Type == 2)//Type=2 --> کاربر تراکنشی
                    {


                        k.SamaneMoadian(Convert.ToInt64(item.fldId), Convert.ToInt64(Session["TaxUserId"]));


                    }
                }
                
            }

            if (Session["savePath"] != null)
            {
                string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                Session.Remove("savePath");
                Session.Remove("FileName");
                System.IO.File.Delete(physicalPath);
            }
            
            return Json(ListRecords.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckErsal(string HeaderIds)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            var msg = "";
            cartaxtest2Entities m = new cartaxtest2Entities();
            
            var ListRecords = m.prs_tblSooratHesab_HeaderSelect("HeaderIds", HeaderIds, "", "", 0).ToList();
            //var headers = HeaderIds.Split(';');
            ErsalBeMoadian k = new ErsalBeMoadian();
            foreach (var item in ListRecords)
            {
               

                        msg=k.SamaneMoadian(Convert.ToInt64(item.fldId), Convert.ToInt64(Session["TaxUserId"]));

                

            }

            if (Session["savePath"] != null)
            {
                string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                Session.Remove("savePath");
                Session.Remove("FileName");
                System.IO.File.Delete(physicalPath);
            }

            return Json(ListRecords.ToList(), JsonRequestBehavior.AllowGet);
        }
        private string ProcessXlsRecords(string fileName)
        {
            //List<Models.ObjHeaderSooratHesab> Header = new List<Models.ObjHeaderSooratHesab>();
           // List<Models.ObjDetailSooratHesab> Detail = new List<Models.ObjDetailSooratHesab>();
            string HeaderIds = "";
            try
            {
                Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(fileName);
                for (int i = 1; i < wb.Worksheets[0].Cells.MaxDataRow + 1; i++)
                {
                    List<Models.ObjDetailSooratHesab> Detail = new List<Models.ObjDetailSooratHesab>();
                    ObjHeaderSooratHesab fish = new ObjHeaderSooratHesab();
                    ObjDetailSooratHesab fishD = new ObjDetailSooratHesab();
                    int count = 0;
                        string ShFish = "";
                    string indatim = "";
                    for (int j = wb.Worksheets[0].Cells.MinColumn; j < wb.Worksheets[0].Cells.MaxDataColumn + 1; j++)
                    {
                        var str = wb.Worksheets[0].Cells[i, j].StringValue;
                        switch (count)
                        {
                            case 0:
                                fish.FishId = str;
                                ShFish= str;
                                break;
                            case 1:
                                fish.Kh_Name = str;
                                break;
                            case 2:
                                fish.Kh_Family = str;
                                break;
                            case 3:
                                fish.bid = str;
                                break;
                            case 4:
                                fish.indatim = Convert.ToDateTime(str);
                                indatim = str;
                                break;
                            case 5:
                                if (str != "")
                                    fish.indati2m = Convert.ToDateTime(str);
                                break;
                            //case 6:
                            //    fish.irtaxid = str;
                            //    break;
                            case 6:
                                if (str != "")
                                    fish.ins = Convert.ToByte(str);
                                if (Convert.ToByte(str) > 1) {
                                    cartaxtest2Entities m = new cartaxtest2Entities();
                                   var h= m.prs_tblSooratHesab_HeaderSelect("ShomareFish", (Session["TarafGharardadId"]).ToString(), ShFish, indatim, 0).FirstOrDefault();//ins= 1 or 2 or 4
                                    if (h != null)
                                        fish.irtaxid = h.fldTaxId;
                                    else
                                        continue;
                                }
                                    break;
                            case 7:
                                fish.tinb = str;
                                break;
                            case 8:
                                if (str != "")
                                    fish.tob = Convert.ToByte(str);
                                break;
                            case 9:
                                if (str != "")
                                    fish.tprdis = Convert.ToInt64(str);
                                break;
                            case 10:
                                if (str != "")
                                    fish.tadis = Convert.ToInt64(str);
                                break;
                            case 11:
                                if (str != "")
                                    fish.tvam = Convert.ToInt64(str);
                                break;
                            case 12:
                                if (str != "")
                                    fish.todam = Convert.ToInt64(str);
                                break;
                            case 13:
                                if (str != "")
                                    fish.setm = Convert.ToByte(str); 
                                break;
                            case 14:
                                if (str != "")
                                    fish.cap = Convert.ToInt64(str);
                                break;
                            case 15:
                                if (str != "")
                                    fish.insp = Convert.ToInt64(str);
                                break;
                            /******/
                            case 16:
                                fishD.sstid = str;
                                break;
                            case 17:
                                fishD.sstt = str;
                                break;
                            case 18:
                                if (str != "")
                                    fishD.am = Convert.ToDecimal(str);
                                break;
                            case 19:
                                if (str != "")
                                    fishD.fee = Convert.ToDecimal(str);
                                break;
                            case 20:
                                if (str != "")
                                    fishD.dis = Convert.ToInt64(str);
                                break;
                            case 21:
                                if (str != "")
                                    fishD.vra = Convert.ToDecimal(str);
                                break;
                            case 22:
                                fishD.odt = str;
                                break;
                            case 23:
                                if (str != "")
                                    fishD.odr = Convert.ToDecimal(str);
                                break;
                            case 24:
                                fishD.olt = str;
                                break;
                            case 25:
                                if (str != "")
                                    fishD.olr = Convert.ToDecimal(str);
                                break;
                                /*													
*/
                        }
                        count++;
                    }
                    //Header.Add(fish);
                    Detail.Add(fishD);
                    ErsalBeMoadian k = new ErsalBeMoadian();
                    var ss=k.SaveData(fish, Detail, Convert.ToInt64(Session["TarafGharardadId"]), Convert.ToInt64(Session["TaxUserId"]), "Excel");
                    HeaderIds = HeaderIds + ss + ";";
                }

                return HeaderIds;
            }
            catch (Exception x)
            {
                return null;
            }
        }

       
    }
}
