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
using System.Runtime.CompilerServices;
using System.Threading;
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
    public class SooratHesabController : Controller
    {
        //
        // GET: /Tax/SooratHesab/

        public ActionResult Index()
        {//باز شدن تب جدید
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            return new Ext.Net.MVC.PartialViewResult();


        }
        public ActionResult New(int State)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
         
            var result = new Ext.Net.MVC.PartialViewResult();
            return result;
        }
        public ActionResult NewForush1(int id)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            sp_GetDate date = new cartaxEntities().sp_GetDate().FirstOrDefault<sp_GetDate>();
                Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.TarikhShamsi = date.DateShamsi;
            result.ViewBag.saat = date.Time.ToString().Substring(0, 5);
           

                cartaxtest2Entities entities2 = new cartaxtest2Entities();
                prs_User_GharardadSelect select = entities2.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
                long serial = new Random().Next(0x3b9a_ca00);
                string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
                string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.taxId = str;
            result.ViewBag.inno = str2;
            result.ViewBag.id = id;
           
            return result;
        }
        public ActionResult NewForush2(int id)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            sp_GetDate date = new cartaxEntities().sp_GetDate().FirstOrDefault<sp_GetDate>();
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.TarikhShamsi = date.DateShamsi;
            result.ViewBag.saat = date.Time.ToString().Substring(0, 5);


            cartaxtest2Entities entities2 = new cartaxtest2Entities();
            prs_User_GharardadSelect select = entities2.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
            long serial = new Random().Next(0x3b9a_ca00);
            string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.taxId = str;
            result.ViewBag.inno = str2;
            result.ViewBag.id = id;

            return result;
        }
        public ActionResult NewForush3(int id)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            sp_GetDate date = new cartaxEntities().sp_GetDate().FirstOrDefault<sp_GetDate>();
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.TarikhShamsi = date.DateShamsi;
            result.ViewBag.saat = date.Time.ToString().Substring(0, 5);


            cartaxtest2Entities entities2 = new cartaxtest2Entities();
            prs_User_GharardadSelect select = entities2.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
            long serial = new Random().Next(0x3b9a_ca00);
            string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.taxId = str;
            result.ViewBag.inno = str2;
            result.ViewBag.id = id;

            return result;
        }
        public ActionResult NewForush4(int id)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            sp_GetDate date = new cartaxEntities().sp_GetDate().FirstOrDefault<sp_GetDate>();
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.TarikhShamsi = date.DateShamsi;
            result.ViewBag.saat = date.Time.ToString().Substring(0, 5);


            cartaxtest2Entities entities2 = new cartaxtest2Entities();
            prs_User_GharardadSelect select = entities2.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
            long serial = new Random().Next(0x3b9a_ca00);
            string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.taxId = str;
            result.ViewBag.inno = str2;
            result.ViewBag.id = id;

            return result;
        }
        public ActionResult NewForush5(int id)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            sp_GetDate date = new cartaxEntities().sp_GetDate().FirstOrDefault<sp_GetDate>();
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.TarikhShamsi = date.DateShamsi;
            result.ViewBag.saat = date.Time.ToString().Substring(0, 5);


            cartaxtest2Entities entities2 = new cartaxtest2Entities();
            prs_User_GharardadSelect select = entities2.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
            long serial = new Random().Next(0x3b9a_ca00);
            string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.taxId = str;
            result.ViewBag.inno = str2;
            result.ViewBag.id = id;

            return result;
        }
        public ActionResult NewForush6(int id)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            sp_GetDate date = new cartaxEntities().sp_GetDate().FirstOrDefault<sp_GetDate>();
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.TarikhShamsi = date.DateShamsi;
            result.ViewBag.saat = date.Time.ToString().Substring(0, 5);


            cartaxtest2Entities entities2 = new cartaxtest2Entities();
            prs_User_GharardadSelect select = entities2.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
            long serial = new Random().Next(0x3b9a_ca00);
            string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.taxId = str;
            result.ViewBag.inno = str2;
            result.ViewBag.id = id;

            return result;
        }
        public ActionResult NewForush7(int id)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            sp_GetDate date = new cartaxEntities().sp_GetDate().FirstOrDefault<sp_GetDate>();
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.TarikhShamsi = date.DateShamsi;
            result.ViewBag.saat = date.Time.ToString().Substring(0, 5);


            cartaxtest2Entities entities2 = new cartaxtest2Entities();
            prs_User_GharardadSelect select = entities2.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
            long serial = new Random().Next(0x3b9a_ca00);
            string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.taxId = str;
            result.ViewBag.inno = str2;
            result.ViewBag.id = id;

            return result;
        }
        public ActionResult GetUnits()
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var q = p.prs_tblMeasureUnitSelect("", "", 0).ToList().OrderBy(l => l.fldId).Select(l => new { fldId = l.fldCode, fldName = l.fldName });
            return this.Store(q);

        }
        public ActionResult Save(Areas.Tax.Models.prs_rptSooratHesab_Header Header, List<Areas.Tax.Models.prs_SelectDetailSooratHesab> Forush1Grid_DetailsArray, int fldForushandeId, int fldKharidarId)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            string Msg = "", MsgTitle = ""; var Er = 0; 
            try
            {


            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
       
            private static ITaxApi CreateTaxApi(string MemoryId, string ApiUrl, string PrivateKeyPath, string CertificatePath)
            {
                TaxProperties taxProperties = new TaxProperties(MemoryId);
                TaxApiFactory factory3 = new TaxApiFactory(ApiUrl, taxProperties);
                ISignatory signatory = new Pkcs8SignatoryFactory().Create(PrivateKeyPath, CertificatePath);
                return factory3.CreateApi(signatory, new EncryptorFactory().Create(factory3.CreatePublicApi(signatory)));
            }

        private static InvoiceDto CreateValidInvoice(string MemoryId, int HeaderId)
        {
            cartaxtest2Entities entities = new cartaxtest2Entities();
            prs_SelectHeaderSooratHesab hesab = entities.prs_SelectHeaderSooratHesab(new int?(HeaderId)).FirstOrDefault<prs_SelectHeaderSooratHesab>();
            List<prs_SelectDetailSooratHesab> list = entities.prs_SelectDetailSooratHesab(new int?(HeaderId)).ToList<prs_SelectDetailSooratHesab>();
            long num = new DateTimeOffset(hesab.fldIndatim).ToUnixTimeMilliseconds();
            long? nullable = null;
            if (hesab.fldIndati2m != null)
            {
                nullable = new long?(new DateTimeOffset(Convert.ToDateTime(hesab.fldIndati2m)).ToUnixTimeMilliseconds());
            }
            List<BodyItemDto> bodylist = new List<BodyItemDto>();
            foreach (var item in list)
            {
                BodyItemDto bd = new BodyItemDto
                {
                    sstid = item.fldsstid,
                    sstt = item.fldsstt,
                    mu = item.fldmu,

                    am = item.fldam,
                    fee = item.fldfee,
                    vra = item.fldvra,
                    prdis = item.fldprdis,
                    dis = item.flddis,
                    adis = item.fldadis,
                    vam = item.fldvam,
                    tsstam = item.fldtsstam,
                    bros = item.fldbros,
                    consfee = item.fldconsfee,
                    cop = item.fldcop,
                    odam = item.fldodam,
                    exr = item.fldexr,
                    ssrv = item.fldssrv,
                    tcpbs = item.fldtcpbs,
                    vop = item.fldvop,
                    spro = item.fldspro,
                    olam = item.fldolam,
                    bsrn = item.fldbsrn,
                    cfee = item.fldcfee,
                    cui = item.fldcui,
                    cut = item.fldcut,
                    nw = item.fldnw,
                    odr = item.fldodr,
                    odt = item.fldodt,
                    olr = item.fldolr,
                    olt = item.fldolt,
                    pspd = item.fldpspd,
                    sscv = item.fldsscv
                };
                bodylist.Add(bd);
            }


            long? indati2m = null;
            if (hesab.fldIndati2m != null)
                indati2m = new DateTimeOffset(Convert.ToDateTime(hesab.fldIndati2m)).ToUnixTimeMilliseconds();

            InvoiceDto invoice = new InvoiceDto()
            {
                Header = new HeaderDto()
                {
                    taxid = hesab.fldTaxId,
                    inno = hesab.fldInno,
                    indatim = new DateTimeOffset(hesab.fldIndatim).ToUnixTimeMilliseconds(),
                    indati2m = indati2m,
                    inty = Convert.ToInt32(hesab.fldInty),

                    inp = hesab.fldinp,
                    ins = hesab.fldins,
                    tins = hesab.fldBid,
                    tinb = hesab.fldkh_Bid,
                    tob = Convert.ToInt32(hesab.fldTob),
                    tprdis = (hesab.fldtprdis),
                    tdis = (hesab.fldtdis),
                    tadis = (hesab.fldtadis),
                    tvam = (hesab.fldtvam),
                    todam = (hesab.fldtodam),
                    tbill = (hesab.fldtbill),
                    setm = hesab.fldsetm,
                    irtaxid = hesab.fldIrtaxId,
                    bid = hesab.fldBid,
                    sbc = hesab.fldSbc,
                    bpc = hesab.fldBpc,
                    bbc = hesab.fldbbc,
                    ft = hesab.fldft,
                    bpn = hesab.fldbpn,
                    scln = hesab.fldscln,
                    scc = hesab.fldscc,
                    cdcn = hesab.fldcdcn,
                    cdcd = Convert.ToInt32(hesab.fldcdcd),
                    crn = hesab.fldcrn,
                    billid = hesab.fldbilid,
                    tonw = (hesab.fldtonw),
                    tocv = (hesab.fldtocv),
                    torv = (hesab.fldtorv),
                    insp = (hesab.fldinsp),
                    cap = (hesab.fldcap),
                    tvop = (hesab.fldtvop),
                    tax17 = (hesab.fldtax17),
                },
                Body = bodylist
            };
            return invoice;
        }

            public ActionResult Delete(int id)
            {
                ActionResult result;
                if (base.Session["TaxUserId"] == null)
                {
                    result = base.RedirectToAction("Login", "AccountTax", new { area = "Tax" });
                }
                else
                {
                    cartaxtest2Entities entities = new cartaxtest2Entities();
                    string str = "";
                    string str2 = "";
                    int num = 0;
                    try
                    {
                        str2 = "حذف موفق";
                        str = "حذف با موفقیت انجام شد.";
                        entities.prs_tblSooratHesab_HeaderDelete(new long?((long)id), new long?(Convert.ToInt64(base.Session["TaxUserId"])));
                    }
                    catch (Exception exception)
                    {
                        str = (exception.InnerException == null) ? exception.Message : exception.InnerException.Message;
                        str2 = "خطا";
                        num = 1;
                    }
                    result = base.Json(new
                    {
                        Msg = str,
                        MsgTitle = str2,
                        Er = num
                    }, JsonRequestBehavior.AllowGet);
                }
                return result;
            }

            public ActionResult Details(int Id)
            {
                prs_SelectHeaderSooratHesab hesab = new cartaxtest2Entities().prs_SelectHeaderSooratHesab(new int?(Id)).FirstOrDefault<prs_SelectHeaderSooratHesab>();
                return base.Json(new
                {
                    fldId = Id,
                    fldbbc = hesab.fldbbc,
                    fldBid = hesab.fldBid,
                    fldbilid = hesab.fldbilid,
                    fldBpc = hesab.fldBpc,
                    fldbpn = hesab.fldbpn,
                    fldcap = hesab.fldcap,
                    fldcdcd = hesab.fldcdcd,
                    fldcdcn = hesab.fldcdcn,
                    fldcrn = hesab.fldcrn,
                    fldft = hesab.fldft,
                    fldf_CodePosti = hesab.fldf_CodePosti,
                    fldf_name = hesab.fldf_name,
                    fldIndati2m = hesab.fldIndati2m,
                    fldIndati2m_Zaman = hesab.fldIndati2m_Zaman,
                    fldIndatim = hesab.fldIndatim,
                    fldIndatim_Zaman = hesab.fldIndatim_Zaman,
                    fldInno = hesab.fldInno,
                    fldinp = hesab.fldinp,
                    fldins = hesab.fldins.ToString(),
                    fldinsp = hesab.fldinsp,
                    fldInty = hesab.fldInty,
                    fldIrtaxId = hesab.fldIrtaxId,
                    fldkh_Bid = hesab.fldkh_Bid,
                    fldkh_Name = hesab.fldkh_Name,
                    fldKh_Tob = hesab.fldKh_Tob,
                    fldNamePettern = hesab.fldNamePettern,
                    fldSbc = hesab.fldSbc,
                    fldscc = hesab.fldscc,
                    fldscln = hesab.fldscln,
                    fldsetm = hesab.fldsetm,
                    fldSh_Indati2m = hesab.fldSh_Indati2m,
                    fldSh_Indatim = hesab.fldSh_Indatim,
                    fldtadis = hesab.fldtadis,
                    fldtax17 = hesab.fldtax17,
                    fldTaxId = hesab.fldTaxId,
                    fldtbill = hesab.fldtbill,
                    fldtdis = hesab.fldtdis,
                    fldTinb = hesab.fldTinb,
                    fldTins = hesab.fldTins,
                    fldTob = hesab.fldTob,
                    fldtyeShkashFoorosh = hesab.fldtyeShkashFoorosh,
                    fldTypeShakhKharid = hesab.fldTypeShakhKharid,
                    fldTypeSooratHesab = hesab.fldTypeSooratHesab
                }, JsonRequestBehavior.AllowGet);
            }

        private static string GenerateTaxId(long serial, DateTime now, string MemoryId)
        {
            TaxIdProvider taxIdProvider = new TaxIdProvider(new VerhoeffAlgorithm());
            return taxIdProvider.GenerateTaxId(MemoryId, serial, now);
        }
        public ActionResult GetCurrencyType()
            {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var q = p.prs_tblCurrencyTypeSelect("", "", 0).ToList().OrderBy(l => l.fldId).Select(l => new { fldId = l.fldNumericCode, fldName = l.fldCurrency });
            return this.Store(q);

            }

          
            


            private static string PrintInquiryResult(List<InquiryResultModel> inquiryResults, int HeaderId, long UserId,string SerializeObjectErsal)
            {
                cartaxtest2Entities entities = new cartaxtest2Entities();
                string fldMatn = "";
                byte num = 1;
            foreach (var result in inquiryResults)
            {
                    fldMatn = "Status = " + result.Status;
                    var errors = result.Data.Error;
                    if (errors != null)
                    {
                        fldMatn = fldMatn + "*** Errors:";
                    }
                foreach (var error in errors)
                {
                        num = 3;
                        string code = error.Code;
                        string message = error.Message;
                        string[] textArray1 = new string[] { fldMatn, "*** Code: ", code, ", Message: ", message };
                        fldMatn = string.Concat(textArray1);
                    }
                    List<InvoiceErrorModel> list2 = result.Data.Warning;
                    if (list2 != null)
                    {
                        fldMatn = fldMatn + "***  Warnings:";
                    }
                    foreach (InvoiceErrorModel model3 in list2)
                    {
                        num = 2;
                        string code = model3.Code;
                        string message = model3.Message;
                        string[] textArray2 = new string[] { fldMatn, "***  Code: ", code, ", Message: ", message };
                        fldMatn = string.Concat(textArray2);
                    }
                    entities.prs_tblSooratHesabStatusInsert(new long?((long)HeaderId), new byte?(num), fldMatn, result.ReferenceNumber, SerializeObjectErsal, result.Uid, new long?(UserId));
                }
                return (num.ToString() + ";" + fldMatn);
            }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.prs_tblSooratHesab_HeaderSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.prs_tblSooratHesab_HeaderSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {        
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldf_NationalCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldf_NationalCode";
                            break;
                        case "fldTypeSooratHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeSooratHesab";
                            break;
                        case "fldSubject":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSubject";
                            break;
                        case "fldkh_name":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldkh_name";
                            break;
                        case "fldIndatim":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldIndatim";
                            break;
                        case "fldf_Name":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldf_Name";
                            break;
                        case "fldkh_fldNationalCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldkh_fldNationalCode";
                            break;
                    }
                    if (data != null)
                        data1 = p.prs_tblSooratHesab_HeaderSelect(field, searchtext, 100).ToList();
                    else
                        data = p.prs_tblSooratHesab_HeaderSelect(field, searchtext, 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.prs_tblSooratHesab_HeaderSelect("", "", 100).ToList();
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

            List<Models.prs_tblSooratHesab_HeaderSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

       
            public ActionResult ReadDetails(Ext.Net.StoreRequestParameters parameters, int HeaderId)
            {
                List<prs_SelectDetailSooratHesab> data = null;
                data = new cartaxtest2Entities().prs_SelectDetailSooratHesab(new int?(HeaderId)).ToList<prs_SelectDetailSooratHesab>();
                return this.Store(data);
            }

            public ActionResult SamaneMoadian(int HeaderId)
            {
                string path = "";
                string str2 = "";
                try
                {
                    cartaxtest2Entities entities = new cartaxtest2Entities();
                    prs_User_GharardadSelect select = entities.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
                    prs_tblTarfGharardadSelect select2 = entities.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>();
                    path = base.Server.MapPath(@"~\Uploaded\privateKey" + select2.fldId.ToString() + ".pem");
                    str2 = base.Server.MapPath(@"~\Uploaded\certificate" + select2.fldId.ToString() + ".crt");
                    if (!System.IO.File.Exists(path))
                    {
                    System.IO.File.WriteAllBytes(path, select2.fldPrivateKey.ToArray<byte>());
                    }
                    if (!System.IO.File.Exists(str2))
                    {
                    System.IO.File.WriteAllBytes(str2, select2.fldSignatureCertificate.ToArray<byte>());
                    }
                    string fldUniqId = select2.fldUniqId;
                    string str5 = "ارسال با موفقیت انجام شد";
                    ITaxApi taxApi = CreateTaxApi(fldUniqId, "https://tp.tax.gov.ir/requestsmanager", path, str2);
                    InvoiceDto item = CreateValidInvoice(fldUniqId, HeaderId);
                    List<InvoiceDto> list1 = new List<InvoiceDto>();
                    list1.Add(item);
                    List<InvoiceDto> invoiceList = list1;

                List<InvoiceResponseModel> responseModels = taxApi.SendInvoices(invoiceList);
                Thread.Sleep(10_000);
                InquiryByReferenceNumberDto inquiryDto = new InquiryByReferenceNumberDto(responseModels.Select(r => r.ReferenceNumber).ToList());
                List<InquiryResultModel> inquiryResults = taxApi.InquiryByReferenceId(inquiryDto);

                string SerializeObjectErsal = Newtonsoft.Json.JsonConvert.SerializeObject(invoiceList);
                string mmsgg=PrintInquiryResult(inquiryResults, HeaderId,Convert.ToInt64(Session["TaxUserId"]), SerializeObjectErsal);
                var msgtitle = "ارسال موفق";
                var msg = "ارسال با موفقیت انجام شد.";

                if (mmsgg.Split(';')[0] != "1")
                    msg = mmsgg.Split(';')[1];

                if (mmsgg.Split(';')[0]=="2")
                    msgtitle = "هشدار";
                if (mmsgg.Split(';')[0] == "3")
                    msgtitle = "خطا";
               


                System.IO.File.Delete(path);
                System.IO.File.Delete(str2);

                return base.Json(new
                {
                    Msg = msg,
                    MsgTitle = msgtitle,
                    Er = mmsgg.Split(';')[0]
                }, JsonRequestBehavior.AllowGet);
            }
                catch (Exception exception)
                {
                    string str7 = "";
                    str7 = (exception.InnerException == null) ? exception.Message : exception.InnerException.Message;
                    return base.Json(new
                    {
                        Msg = "خطا",
                        MsgTitle = str7,
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            public ActionResult Savee(prs_SelectHeaderSooratHesab Header,List<prs_SelectDetailSooratHesab> Grid_DetailsArray,  int fldForushandeId, int fldKharidarId)
            {
                string str="ذخیره با موفقیت انجام شد.";
                string str2 = "عملیات موفق";
                int num=0;
                if (base.Session["TaxUserId"] != null)
                {
                    str = "";
                    str2 = "";
                    num = 0;
                    try
                    {
                        ParamValue value2;
                        long? fldtprdis;
                        decimal? fldtonw;
                        int? nullable18;
                        int? nullable36;
                        int? nullable37;
                        int? nullable38;
                        cartaxtest2Entities entities = new cartaxtest2Entities();
                        DateTime time = Shamsi.Shamsi2miladiDateTime(Header.fldSh_Indatim);
                        char[] separator = new char[] { ':' };
                        char[] chArray2 = new char[] { ':' };
                        TimeSpan span = new TimeSpan(Convert.ToInt32(Header.fldIndatim_Zaman.Split(separator)[0]), Convert.ToInt32(Header.fldIndatim_Zaman.Split(chArray2)[1]), 0);
                        time = time.Date + span;
                        DateTime? nullable = null;
                        if (Header.fldSh_Indati2m != null)
                        {
                            nullable = new DateTime?(Shamsi.Shamsi2miladiDateTime(Header.fldSh_Indati2m));
                            char[] chArray3 = new char[] { ':' };
                            char[] chArray4 = new char[] { ':' };
                            TimeSpan span2 = new TimeSpan(Convert.ToInt32(Header.fldIndati2m_Zaman.Split(chArray3)[0]), Convert.ToInt32(Header.fldIndati2m_Zaman.Split(chArray4)[1]), 0);
                            nullable = new DateTime?(Convert.ToDateTime(nullable).Date + span2);
                        }
                        decimal num3 = 0M;
                        decimal? nullable2 = new decimal?(num3);
                        num3 = 0M;
                        decimal? nullable3 = new decimal?(num3);
                        num3 = 0M;
                        decimal? nullable4 = new decimal?(num3);
                        num3 = 0M;
                        decimal? nullable5 = new decimal?(num3);
                        num3 = 0M;
                        decimal? nullable6 = new decimal?(num3);
                        num3 = 0M;
                        decimal? nullable7 = new decimal?(num3);
                        num3 = 0M;
                        decimal? nullable8 = new decimal?(num3);
                        num3 = 0M;
                        decimal? nullable9 = new decimal?(num3);
                        num3 = 0M;
                        decimal? nullable10 = new decimal?(num3);
                        num3 = 0M;
                        decimal? nullable11 = new decimal?(num3);
                        foreach (prs_SelectDetailSooratHesab hesab in Grid_DetailsArray)
                        {
                            decimal? nullable14;
                            decimal? nullable15;
                            if (hesab.fldvop != null)
                            {
                                decimal? nullable1;
                                decimal? nullable28;
                                fldtonw = nullable2;
                                fldtprdis = hesab.fldvop;
                                if (fldtprdis != null)
                                {
                                    nullable1 = new decimal?(fldtprdis.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable1 = nullable15;
                                }
                                nullable14 = nullable1;
                                if ((fldtonw != null) & (nullable14 != null))
                                {
                                    nullable28 = new decimal?(fldtonw.GetValueOrDefault() + nullable14.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable28 = nullable15;
                                }
                                nullable2 = nullable28;
                            }
                            if (hesab.fldsscv != null)
                            {
                                decimal? nullable19;
                                nullable14 = nullable3;
                                fldtonw = hesab.fldsscv;
                                if ((nullable14 != null) & (fldtonw != null))
                                {
                                    nullable19 = new decimal?(nullable14.GetValueOrDefault() + fldtonw.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable19 = nullable15;
                                }
                                nullable3 = nullable19;
                            }
                            if (hesab.fldssrv != null)
                            {
                                decimal? nullable20;
                                decimal? nullable29;
                                fldtonw = nullable4;
                                fldtprdis = hesab.fldssrv;
                                if (fldtprdis != null)
                                {
                                    nullable20 = new decimal?(fldtprdis.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable20 = nullable15;
                                }
                                nullable14 = nullable20;
                                if ((fldtonw != null) & (nullable14 != null))
                                {
                                    nullable29 = new decimal?(fldtonw.GetValueOrDefault() + nullable14.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable29 = nullable15;
                                }
                                nullable4 = nullable29;
                            }
                            if (hesab.fldnw != null)
                            {
                                decimal? nullable21;
                                nullable14 = nullable5;
                                fldtonw = hesab.fldnw;
                                if ((nullable14 != null) & (fldtonw != null))
                                {
                                    nullable21 = new decimal?(nullable14.GetValueOrDefault() + fldtonw.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable21 = nullable15;
                                }
                                nullable5 = nullable21;
                            }
                            if (hesab.fldtsstam != null)
                            {
                                decimal? nullable22;
                                decimal? nullable30;
                                fldtonw = nullable6;
                                fldtprdis = hesab.fldtsstam;
                                if (fldtprdis != null)
                                {
                                    nullable22 = new decimal?(fldtprdis.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable22 = nullable15;
                                }
                                nullable14 = nullable22;
                                if ((fldtonw != null) & (nullable14 != null))
                                {
                                    nullable30 = new decimal?(fldtonw.GetValueOrDefault() + nullable14.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable30 = nullable15;
                                }
                                nullable6 = nullable30;
                            }
                            if (hesab.fldvam != null)
                            {
                                decimal? nullable23;
                                decimal? nullable31;
                                nullable14 = nullable8;
                                fldtprdis = hesab.fldvam;
                                if (fldtprdis != null)
                                {
                                    nullable23 = new decimal?(fldtprdis.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable23 = nullable15;
                                }
                                fldtonw = nullable23;
                                if ((nullable14 != null) & (fldtonw != null))
                                {
                                    nullable31 = new decimal?(nullable14.GetValueOrDefault() + fldtonw.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable31 = nullable15;
                                }
                                nullable8 = nullable31;
                            }
                            if (hesab.fldodam != null)
                            {
                                decimal? nullable24;
                                decimal? nullable32;
                                fldtonw = nullable7;
                                fldtprdis = hesab.fldodam;
                                if (fldtprdis != null)
                                {
                                    nullable24 = new decimal?(fldtprdis.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable24 = nullable15;
                                }
                                nullable14 = nullable24;
                                if ((fldtonw != null) & (nullable14 != null))
                                {
                                    nullable32 = new decimal?(fldtonw.GetValueOrDefault() + nullable14.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable32 = nullable15;
                                }
                                nullable7 = nullable32;
                            }
                            if (hesab.fldadis != null)
                            {
                                decimal? nullable25;
                                decimal? nullable33;
                                nullable14 = nullable9;
                                fldtprdis = hesab.fldadis;
                                if (fldtprdis != null)
                                {
                                    nullable25 = new decimal?(fldtprdis.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable25 = nullable15;
                                }
                                fldtonw = nullable25;
                                if ((nullable14 != null) & (fldtonw != null))
                                {
                                    nullable33 = new decimal?(nullable14.GetValueOrDefault() + fldtonw.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable33 = nullable15;
                                }
                                nullable9 = nullable33;
                            }
                            if (hesab.flddis != null)
                            {
                                decimal? nullable26;
                                decimal? nullable34;
                                fldtonw = nullable10;
                                fldtprdis = hesab.flddis;
                                if (fldtprdis != null)
                                {
                                    nullable26 = new decimal?(fldtprdis.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable26 = nullable15;
                                }
                                nullable14 = nullable26;
                                if ((fldtonw != null) & (nullable14 != null))
                                {
                                    nullable34 = new decimal?(fldtonw.GetValueOrDefault() + nullable14.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable34 = nullable15;
                                }
                                nullable10 = nullable34;
                            }
                            if (hesab.fldprdis != null)
                            {
                                decimal? nullable27;
                                decimal? nullable35;
                                nullable14 = nullable11;
                                fldtprdis = hesab.fldprdis;
                                if (fldtprdis != null)
                                {
                                    nullable27 = new decimal?(fldtprdis.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable27 = nullable15;
                                }
                                fldtonw = nullable27;
                                if ((nullable14 != null) & (fldtonw != null))
                                {
                                    nullable35 = new decimal?(nullable14.GetValueOrDefault() + fldtonw.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable15 = null;
                                    nullable35 = nullable15;
                                }
                                nullable11 = nullable35;
                            }
                        }
                        List<ParamValue> source = new List<ParamValue>();
                        Header.fldtvop = new long?(Convert.ToInt64(nullable2));
                        Header.fldtocv = new decimal?(Convert.ToDecimal(nullable3));
                        Header.fldtorv = new long?(Convert.ToInt64(nullable4));
                        Header.fldtonw = new decimal?(Convert.ToDecimal(nullable5));
                        Header.fldtbill = new long?(Convert.ToInt64(nullable6));
                        Header.fldtodam = new long?(Convert.ToInt64(nullable7));
                        Header.fldtvam = new long?(Convert.ToInt64(nullable8));
                        Header.fldtadis = new long?(Convert.ToInt64(nullable9));
                        Header.fldtdis = new long?(Convert.ToInt64(nullable10));
                        Header.fldtprdis = new long?(Convert.ToInt64(nullable11));
                        byte? fldins = Header.fldins;
                        if (fldins != null)
                        {
                            nullable36 = new int?(fldins.GetValueOrDefault());
                        }
                        else
                        {
                            nullable18 = null;
                            nullable36 = nullable18;
                        }
                        int? fldcdcd = nullable36;
                        int num4 = 0;
                        if (!((fldcdcd.GetValueOrDefault() == num4) & (fldcdcd != null)) && (Header.fldins != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 1,
                                fldValue = Header.fldins.ToString()
                            };
                            source.Add(value2);
                        }
                        fldins = Header.fldft;
                        if (fldins != null)
                        {
                            nullable37 = new int?(fldins.GetValueOrDefault());
                        }
                        else
                        {
                            nullable18 = null;
                            nullable37 = nullable18;
                        }
                        fldcdcd = nullable37;
                        num4 = 0;
                        if (!((fldcdcd.GetValueOrDefault() == num4) & (fldcdcd != null)) && (Header.fldft != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 9,
                                fldValue = Header.fldft.ToString()
                            };
                            source.Add(value2);
                        }
                        if ((Header.fldbpn != "") && (Header.fldbpn != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 10,
                                fldValue = Header.fldbpn
                            };
                            source.Add(value2);
                        }
                        if ((Header.fldscln != "") && (Header.fldscln != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 11,
                                fldValue = Header.fldscln
                            };
                            source.Add(value2);
                        }
                        if ((Header.fldscc != "") && (Header.fldscc != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 12,
                                fldValue = Header.fldscc
                            };
                            source.Add(value2);
                        }
                        if ((Header.fldcdcn != "") && (Header.fldcdcn != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 13,
                                fldValue = Header.fldcdcn
                            };
                            source.Add(value2);
                        }
                        fldcdcd = Convert.ToInt32(Header.fldcdcd);
                        num4 = 0;
                        if (!((fldcdcd.GetValueOrDefault() == num4) & (fldcdcd != null)) && (Header.fldcdcd != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 14,
                                fldValue = Header.fldcdcd.ToString()
                            };
                            source.Add(value2);
                        }
                        if ((Header.fldcrn != "") && (Header.fldcrn != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 15,
                                fldValue = Header.fldcrn
                            };
                            source.Add(value2);
                        }
                        if ((Header.fldbilid != "") && (Header.fldbilid != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x10,
                                fldValue = Header.fldbilid
                            };
                            source.Add(value2);
                        }
                        fldtprdis = Header.fldtprdis;
                        long num5 = 0L;
                        if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (Header.fldtprdis != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x11,
                                fldValue = Header.fldtprdis.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtprdis = Header.fldtdis;
                        num5 = 0L;
                        if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (Header.fldtdis != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x12,
                                fldValue = Header.fldtdis.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtprdis = Header.fldtadis;
                        num5 = 0L;
                        if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (Header.fldtadis != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x13,
                                fldValue = Header.fldtadis.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtprdis = Header.fldtvam;
                        num5 = 0L;
                        if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (Header.fldtvam != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 20,
                                fldValue = Header.fldtvam.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtprdis = Header.fldtodam;
                        num5 = 0L;
                        if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (Header.fldtodam != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x15,
                                fldValue = Header.fldtodam.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtprdis = Header.fldtbill;
                        num5 = 0L;
                        if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (Header.fldtbill != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x16,
                                fldValue = Header.fldtbill.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtonw = Header.fldtonw;
                        num3 = 0M;
                        if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (Header.fldtonw != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x17,
                                fldValue = Header.fldtonw.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtprdis = Header.fldtorv;
                        num5 = 0L;
                        if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (Header.fldtorv != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x18,
                                fldValue = Header.fldtorv.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtonw = Header.fldtocv;
                        num3 = 0M;
                        if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (Header.fldtocv != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x19
                            };
                            fldtonw = Header.fldtocv;
                            value2.fldValue = fldtonw.ToString();
                            source.Add(value2);
                        }
                        fldins = Header.fldsetm;
                        if (fldins != null)
                        {
                            nullable38 = new int?(fldins.GetValueOrDefault());
                        }
                        else
                        {
                            nullable18 = null;
                            nullable38 = nullable18;
                        }
                        fldcdcd = nullable38;
                        num4 = 0;
                        if (!((fldcdcd.GetValueOrDefault() == num4) & (fldcdcd != null)) && (Header.fldsetm != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x1a,
                                fldValue = Header.fldsetm.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtprdis = Header.fldcap;
                        num5 = 0L;
                        if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (Header.fldcap != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x1b,
                                fldValue = Header.fldcap.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtprdis = Header.fldinsp;
                        num5 = 0L;
                        if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (Header.fldinsp != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x1c,
                                fldValue = Header.fldinsp.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtprdis = Header.fldtvop;
                        num5 = 0L;
                        if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (Header.fldtvop != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x1d,
                                fldValue = Header.fldtvop.ToString()
                            };
                            source.Add(value2);
                        }
                        fldtprdis = Header.fldtax17;
                        num5 = 0L;
                        if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (Header.fldtax17 != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 30
                            };
                            fldtprdis = Header.fldtax17;
                            value2.fldValue = fldtprdis.ToString();
                            source.Add(value2);
                        }
                        DataTable table1 = new DataTable();
                        table1.TableName = "movadi.tblSooratHesabHeader_Value";
                        DataTable table = table1;
                        using (ObjectReader reader = ObjectReader.Create<ParamValue>(source.ToList<ParamValue>(), Array.Empty<string>()))
                        {
                            table.Load(reader);
                        }
                        long num2 = entities.prs_tblSooratHesab_HeaderInsert(new ObjectParameter("fldid", typeof(long)), Header.fldTaxId, new DateTime?(time), nullable, new byte?(Header.fldInty), Header.fldinp, Header.fldInno, Header.fldIrtaxId, new long?((long)fldKharidarId), new long?((long)fldForushandeId),
                            Header.fldFunctionName, Header.fldbpn, Header.fldft, Header.fldscln, Header.fldscc, Header.fldcrn, table, new long?((long)Convert.ToInt32(base.Session["TaxUserId"])));
                        foreach (prs_SelectDetailSooratHesab hesab2 in Grid_DetailsArray)
                        {
                            ParamValue value3;
                            List<ParamValue> list2 = new List<ParamValue>();
                            if ((hesab2.fldsstid != "") && (hesab2.fldsstid != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x1f,
                                    fldValue = hesab2.fldsstid
                                };
                                list2.Add(value3);
                            }
                            if ((hesab2.fldsstt != "") && (hesab2.fldsstt != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x20,
                                    fldValue = hesab2.fldsstt
                                };
                                list2.Add(value3);
                            }
                            fldtonw = hesab2.fldam;
                            num3 = 0M;
                            if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (hesab2.fldam != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x21,
                                    fldValue = hesab2.fldam.ToString()
                                };
                                list2.Add(value3);
                            }
                            if ((hesab2.fldmu != "") && (hesab2.fldmu != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x22,
                                    fldValue = hesab2.fldmu
                                };
                                list2.Add(value3);
                            }
                            fldtonw = hesab2.fldnw;
                            num3 = 0M;
                            if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (hesab2.fldnw != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x23,
                                    fldValue = hesab2.fldnw.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtonw = hesab2.fldfee;
                            num3 = 0M;
                            if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (hesab2.fldfee != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x24,
                                    fldValue = hesab2.fldfee.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtonw = hesab2.fldcfee;
                            num3 = 0M;
                            if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (hesab2.fldcfee != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x25,
                                    fldValue = hesab2.fldcfee.ToString()
                                };
                                list2.Add(value3);
                            }
                            if ((hesab2.fldcut != "") && (hesab2.fldcut != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x26,
                                    fldValue = hesab2.fldcut
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldexr;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldexr != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x27,
                                    fldValue = hesab2.fldexr.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldssrv;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldssrv != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 40,
                                    fldValue = hesab2.fldssrv.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtonw = hesab2.fldsscv;
                            num3 = 0M;
                            if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (hesab2.fldsscv != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x29,
                                    fldValue = hesab2.fldsscv.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldprdis;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldprdis != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x2a,
                                    fldValue = hesab2.fldprdis.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.flddis;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.flddis != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x2b,
                                    fldValue = hesab2.flddis.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldadis;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldadis != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x2c,
                                    fldValue = hesab2.fldadis.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtonw = hesab2.fldvra;
                            num3 = 0M;
                            if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (hesab2.fldvra != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x2d,
                                    fldValue = hesab2.fldvra.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldvam;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldvam != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x2e,
                                    fldValue = hesab2.fldvam.ToString()
                                };
                                list2.Add(value3);
                            }
                            if ((hesab2.fldodt != "") && (hesab2.fldodt != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x2f,
                                    fldValue = hesab2.fldodt
                                };
                                list2.Add(value3);
                            }
                            fldtonw = hesab2.fldodr;
                            num3 = 0M;
                            if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (hesab2.fldodr != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x30,
                                    fldValue = hesab2.fldodr.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldodam;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldodam != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x31,
                                    fldValue = hesab2.fldodam.ToString()
                                };
                                list2.Add(value3);
                            }
                            if ((hesab2.fldolt != "") && (hesab2.fldolt != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 50,
                                    fldValue = hesab2.fldolt
                                };
                                list2.Add(value3);
                            }
                            fldtonw = hesab2.fldolr;
                            num3 = 0M;
                            if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (hesab2.fldolr != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x33,
                                    fldValue = hesab2.fldolr.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldolam;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldolam != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x34,
                                    fldValue = hesab2.fldolam.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldconsfee;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldconsfee != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x35,
                                    fldValue = hesab2.fldconsfee.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldspro;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldspro != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x36,
                                    fldValue = hesab2.fldspro.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldbros;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldbros != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x37,
                                    fldValue = hesab2.fldbros.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldtcpbs;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldtcpbs != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x38,
                                    fldValue = hesab2.fldtcpbs.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldcop;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldcop != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x39,
                                    fldValue = hesab2.fldcop.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldvop;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldvop != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x3a,
                                    fldValue = hesab2.fldvop.ToString()
                                };
                                list2.Add(value3);
                            }
                            if ((hesab2.fldbsrn != "") && (hesab2.fldbsrn != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x3b,
                                    fldValue = hesab2.fldbsrn
                                };
                                list2.Add(value3);
                            }
                            fldtprdis = hesab2.fldtsstam;
                            num5 = 0L;
                            if (!((fldtprdis.GetValueOrDefault() == num5) & (fldtprdis != null)) && (hesab2.fldtsstam != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 60,
                                    fldValue = hesab2.fldtsstam.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtonw = hesab2.fldpspd;
                            num3 = 0M;
                            if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (hesab2.fldpspd != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x3d,
                                    fldValue = hesab2.fldpspd.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtonw = hesab2.fldcui;
                            num3 = 0M;
                            if (!((fldtonw.GetValueOrDefault() == num3) & (fldtonw != null)) && (hesab2.fldcui != null))
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x3e,
                                    fldValue = hesab2.fldcui.ToString()
                                };
                                list2.Add(value3);
                            }
                            DataTable table3 = new DataTable();
                            table3.TableName = "movadi.tblSooratHesab_Detail";
                            DataTable table2 = table3;
                            using (ObjectReader reader2 = ObjectReader.Create<ParamValue>(list2, Array.Empty<string>()))
                            {
                                table2.Load(reader2);
                            }
                            entities.prs_tblSooratHesab_DetailInsert(new long?(num2), table2, new long?((long)Convert.ToInt32(base.Session["TaxUserId"])));
                        }
                    }
                    catch (Exception exception)
                    {
                        str = (exception.InnerException == null) ? exception.Message : exception.InnerException.Message;
                        str2 = "خطا";
                        num = 1;
                    }
                }
                else
                {
                    return base.RedirectToAction("Login", "AccountTax", new { area = "Tax" });
                }
                return base.Json(new
                {
                    Msg = str,
                    MsgTitle = str2,
                    Er = num
                }, JsonRequestBehavior.AllowGet);
            }

      
    }
}
