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
using TaxCollectData.Library.Algorithms;
using TaxCollectData.Library.Dto;
using TaxCollectData.Library.Factories;
using TaxCollectData.Library.Models;
using TaxCollectData.Library.Properties;
using TaxCollectData.Library.Providers;
using FastReport;

using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Jose; // Jose.JWT library
//using JWT.Builder;
//using JWT.Algorithms;

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Operators;

namespace Avarez.Areas.Tax.Controllers
{
    public class SooratHesabController : Controller
    {
        //
        // GET: /Tax/SooratHesab/

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

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
        public ActionResult ShowstatusWin(string HeaderId)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Models.cartaxtest2Entities m = new Models.cartaxtest2Entities();
            var k = m.prs_tblSooratHesabStatusSelect("fldHeaderId", HeaderId, 0).FirstOrDefault();
            PartialView.ViewBag.Message = k.fldMatn;
            PartialView.ViewBag.HeaderId = k.fldid;
            return PartialView;
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
            // string str = GenerateTaxId(serial, date.CurrentDateTime, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            var khodesh = entities2.prs_tblShakhsHaghighi_HoghoghiSelect("fldId", Session["TarafGharardadId"].ToString(), Convert.ToInt64(Session["TaxUserId"]), 0).FirstOrDefault();
            result.ViewBag.ForushandeId = khodesh.fldId;
            result.ViewBag.NameF = khodesh.fldName + " " + khodesh.fldFamily;
            result.ViewBag.ShEghtesadiF = khodesh.fldCodeEghtesadi;
            result.ViewBag.ShenaseF = khodesh.fldNationalCode;
            result.ViewBag.ShobeF = khodesh.fldCodeShobe;
            result.ViewBag.PostiF = khodesh.fldCodePosti;

            result.ViewBag.serial = serial;
            result.ViewBag.taxId = "";// str;
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
            //  string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.serial = serial;
            result.ViewBag.taxId = "";//str;
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
            //  string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.serial = serial;
            result.ViewBag.taxId = "";// str;
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
            //string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.serial = serial;
            result.ViewBag.taxId = "";// str;
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
            // string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.serial = serial;
            result.ViewBag.taxId = "";// str;
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
            //string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.serial = serial;
            result.ViewBag.taxId = "";// str;
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
            //string str = GenerateTaxId(serial, DateTime.Now, entities2.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
            string str2 = serial.ToString("X").PadLeft(10, '0');

            result.ViewBag.serial = serial;
            result.ViewBag.taxId = "";// str;
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

        static string ss = "0";
     

        private static InvoiceDto CreateValidInvoice(string MemoryId, long HeaderId)
        {
            cartaxtest2Entities entities = new cartaxtest2Entities();
            prs_SelectHeaderSooratHesab hesab = entities.prs_SelectHeaderSooratHesab(new long?(HeaderId)).FirstOrDefault<prs_SelectHeaderSooratHesab>();
            List<prs_SelectDetailSooratHesab> list = entities.prs_SelectDetailSooratHesab(new long?(HeaderId)).ToList<prs_SelectDetailSooratHesab>();
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
                   // pspd = item.fldpspd,
                    sscv = item.fldsscv
                };
                bodylist.Add(bd);
            }


            long? indati2m = null;
            if (hesab.fldIndati2m != null)
                indati2m = new DateTimeOffset(Convert.ToDateTime(hesab.fldIndati2m)).ToUnixTimeMilliseconds();

            int? cdcd = null;
            if (hesab.fldcdcd != null)
                cdcd = Convert.ToInt32(hesab.fldcdcd);


            InvoiceDto invoice = null;
            if (hesab.fldKh_Tob == 2)
            {
                invoice = new InvoiceDto()
                {
                    Header = new HeaderDto()
                    {
                        taxid = hesab.fldTaxId,
                        inno = hesab.fldInno,
                        indatim = new DateTimeOffset(hesab.fldIndatim).ToUnixTimeMilliseconds(),
                        indati2m = indati2m,
                        inty = Convert.ToInt32(hesab.fldInty),

                        inp = hesab.fldinp,
                        ins = Convert.ToInt32(hesab.fldins),
                        tins = hesab.fldBid,
                        tinb = hesab.fldkh_Bid,
                        tob = Convert.ToInt32(hesab.fldKh_Tob),
                        tprdis = (hesab.fldtprdis),
                        tdis = (hesab.fldtdis),
                        tadis = (hesab.fldtadis),
                        tvam = (hesab.fldtvam),
                        todam = (hesab.fldtodam),
                        tbill = (hesab.fldtbill),
                        setm = hesab.fldsetm,
                        irtaxid = hesab.fldIrtaxId,
                        bid = hesab.fldkh_Bid,
                        sbc = hesab.fldSbc,
                        bpc = hesab.fldBpc,
                        bbc = hesab.fldbbc,
                        ft = hesab.fldft,
                        bpn = hesab.fldbpn,
                        scln = hesab.fldscln,
                        scc = hesab.fldscc,
                        cdcn = hesab.fldcdcn,
                        cdcd = cdcd,
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
            }
            else
            {
                invoice = new InvoiceDto()
                {
                    Header = new HeaderDto()
                    {
                        taxid = hesab.fldTaxId,
                        inno = hesab.fldInno,
                        indatim = new DateTimeOffset(hesab.fldIndatim).ToUnixTimeMilliseconds(),
                        indati2m = indati2m,
                        inty = Convert.ToInt32(hesab.fldInty),

                        inp = hesab.fldinp,
                        ins = Convert.ToInt32(hesab.fldins),
                        tins = hesab.fldBid,
                        //tinb = hesab.fldkh_Bid,
                        tob = Convert.ToInt32(hesab.fldKh_Tob),
                        tprdis = (hesab.fldtprdis),
                        tdis = (hesab.fldtdis),
                        tadis = (hesab.fldtadis),
                        tvam = (hesab.fldtvam),
                        todam = (hesab.fldtodam),
                        tbill = (hesab.fldtbill),
                        setm = hesab.fldsetm,
                        irtaxid = hesab.fldIrtaxId,
                        bid = hesab.fldkh_Bid,
                        sbc = hesab.fldSbc,
                        bpc = hesab.fldBpc,
                        bbc = hesab.fldbbc,
                        ft = hesab.fldft,
                        bpn = hesab.fldbpn,
                        scln = hesab.fldscln,
                        scc = hesab.fldscc,
                        cdcn = hesab.fldcdcn,
                        cdcd = cdcd,
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
            }
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
                    entities.prs_tblSooratHesab_HeaderDelete(new long?((long)id), new long?(Convert.ToInt64(base.Session["TaxUserId"])), IP);
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
            var Indati2m_Zaman = "";
            if (hesab.fldIndati2m_Zaman != null) Indati2m_Zaman = hesab.fldIndati2m_Zaman.Substring(0, 5);
            var Indatim_Zaman = "";
            if (hesab.fldIndatim_Zaman != null) Indatim_Zaman = hesab.fldIndatim_Zaman.Substring(0, 5);
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
                fldIndati2m_Zaman = Indati2m_Zaman,
                fldIndatim = hesab.fldIndatim,
                fldIndatim_Zaman = Indatim_Zaman,
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
                fldsetm = hesab.fldsetm.ToString(),
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
                fldTypeSooratHesab = hesab.fldTypeSooratHesab,
                fldForushandeId = hesab.fldForooshandeId,
                fldKharidarId = hesab.fldKharidarId,
                fldShomareFish = hesab.fldShomareFish
            }, JsonRequestBehavior.AllowGet);
        }

        private static string GenerateTaxId(long serial, DateTime now, string MemoryId)
        {
            var helper = new TaxIdHelper();
            string taxId = TaxIdHelper.GenerateValidTaxId(MemoryId, serial, now/*DateTime.Now*/);
            return taxId;
        }
        public ActionResult GetCurrencyType()
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var q = p.prs_tblCurrencyTypeSelect("", "", 0).ToList().OrderBy(l => l.fldId).Select(l => new { fldId = l.fldNumericCode, fldName = l.fldCurrency });
            return this.Store(q);

        }





        private static string PrintInquiryResult(List<InquiryResultModel> inquiryResults, long HeaderId, long UserId, string SerializeObjectErsal, long Id)
        {
            string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

            cartaxtest2Entities m = new cartaxtest2Entities();
            string fldMatn = "";
            byte num = 1;
           /* foreach (var result in inquiryResults)
            {
                fldMatn = "Status = " + result.Status;
                var errors = result.Data.Error;

                if (errors.Count() > 0)
                {
                    num = 3;
                }
                List<ErrorModel> list2 = result.Data.Warning;
                if (list2.Count() > 0)
                {
                    if (errors.Count() == 0)
                        num = 2;
                }
                long? ss = Id;
                if (Id == 0)
                    ss = m.prs_tblSooratHesabStatusInsert(new long?((long)HeaderId), new byte?(num), result.Status, result.ReferenceNumber, SerializeObjectErsal, result.Uid, new long?(UserId), IP).FirstOrDefault().fldId;
                else
                    m.prs_tblSooratHesabStatusUpdate(Id, new long?((long)HeaderId), new byte?(num), result.Status, result.ReferenceNumber, SerializeObjectErsal, result.Uid, new long?(UserId), IP);
                foreach (var error in errors)
                {
                    string code = error.Code;
                    string message = error.Message;
                    string[] textArray1 = new string[] { fldMatn, "*** Code: ", code, ", Message: ", message };
                    fldMatn = string.Concat(textArray1);

                    m.prs_tblSooratHesabStatus_DetailInsert(ss, 3, message, code, UserId, IP);
                }


                foreach (ErrorModel model3 in list2)
                {

                    string code = model3.Code;
                    string message = model3.Message;
                    string[] textArray2 = new string[] { fldMatn, "***  Code: ", code, ", Message: ", message };
                    fldMatn = string.Concat(textArray2);

                    m.prs_tblSooratHesabStatus_DetailInsert(ss, 2, message, code, UserId, IP);
                }

            }*/
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
                        case "fldShomareFish":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareFish";
                            break;
                        case "SumSooratHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "SumSooratHesab";
                            break;
                    }
                    if (data != null)
                        data1 = p.prs_tblSooratHesab_HeaderSelect(field, searchtext, Session["TarafGharardadId"].ToString(), "", 100).ToList();
                    else
                        data = p.prs_tblSooratHesab_HeaderSelect(field, searchtext, Session["TarafGharardadId"].ToString(), "", 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.prs_tblSooratHesab_HeaderSelect("fldForooshandeId", Session["TarafGharardadId"].ToString(), "", "", 100).ToList();
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
        private const string BaseUrl = "https://tp.tax.gov.ir/requestsmanager/";
        public static async System.Threading.Tasks.Task<string> GetNonceAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "*/*");

                    var res = await client.GetAsync("api/v2/nonce?timeToLive=20");

                    // بررسی جزئیات خطا
                    if (!res.IsSuccessStatusCode)
                    {
                        string errorBody = await res.Content.ReadAsStringAsync();
                        throw new Exception($"GetNonce failed: {res.StatusCode} - {errorBody}");
                    }

                    string body = await res.Content.ReadAsStringAsync();
                    var jo = JObject.Parse(body);
                    return jo["nonce"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در GetNonce: {ex.Message}");
                throw;
            }
        }

       

        // ============================
        // 2) بارگذاری private key (PEM) با BouncyCastle -> RSA
        // ============================
        private static RSA LoadPrivateRsaFromPem(string privateKeyPemPath)
        {
            using (var sr = System.IO.File.OpenText(privateKeyPemPath)){
            var pemReader = new PemReader(sr);
        var pemObject = pemReader.ReadObject();

        RsaPrivateCrtKeyParameters rsaParamsBc;
            if (pemObject is AsymmetricCipherKeyPair kp)
                rsaParamsBc = (RsaPrivateCrtKeyParameters) kp.Private;
            else if (pemObject is RsaPrivateCrtKeyParameters pk)
                rsaParamsBc = pk;
            else
                throw new Exception("فرمت کلید خصوصی پشتیبانی نمی‌شود.");
    
            return DotNetUtilities.ToRSA(rsaParamsBc);
            }
        }

        // ============================
        // 3) خواندن certificate.crt -> Base64 (x5c)
        // ============================
        private static string LoadCertificateBase64(string certificatePath)
        {
            // crt ممکن است PEM یا DER باشد. Try to detect PEM header:

            string text = System.IO.File.ReadAllText(certificatePath).Trim();

            if (text.Contains("-----BEGIN CERTIFICATE REQUEST-----"))
            {
                string base64 = text
                    .Replace("-----BEGIN CERTIFICATE REQUEST-----", "")
                    .Replace("-----END CERTIFICATE REQUEST-----", "")
                    .Replace("\r", "").Replace("\n", "").Replace(" ", "")
                    .Trim();
                return base64;
            }
            else
            {
                // assume binary DER
                /*byte[] raw = System.IO.File.ReadAllBytes(certificatePath);
                return Convert.ToBase64String(raw);*/
                return text;
            }
        }

        // ============================
        // 4) ساخت JWT احراز (payload: { nonce, clientId }) و امضا (JWS)
        //    نتیجه: authJwt (string) که برای گرفتن access token استفاده می‌شود.
        // ============================
        public static string CreateAuthJwt(string clientId, string privateKeyPemPath, string certificatePath)
        {
            var rsa = LoadPrivateRsaFromPem(privateKeyPemPath);
            var certBase64 = LoadCertificateBase64(certificatePath);

            string sigT = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");

            var headers = new Dictionary<string, object>
        {
            {"x5c", new[] { certBase64 } },
            {"sigT", sigT},
            {"typ", "jose"},
            {"crit", new[] { "sigT" }},
            {"cty", "text/plain"}
        };

            var payload = new Dictionary<string, object>
        {
            { "nonce", "" },    // مقدار nonce را بعدا جایگزین می‌کنیم یا از GetNonce استفاده کن قبل فراخوانی
            { "clientId", clientId }
        };

            // NOTE: For auth jwt we will set nonce externally; typical flow: call GetNonceAsync() then set payload["nonce"]=nonce
            // Use JWT.Encode to make JWS
            string token = Jose.JWT.Encode(payload, rsa, JwsAlgorithm.RS256, extraHeaders: headers);
            return token;
        }

        // convenient overload that accepts nonce
        public static string CreateAuthJwtWithNonce(string nonce, string clientId, string privateKeyPemPath, string certificatePath)
        {
            var rsa = LoadPrivateRsaFromPem(privateKeyPemPath);
            var certBase64 = LoadCertificateBase64(certificatePath);

            string sigT = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");

            // header طبق مستند مودیان
            var headers = new Dictionary<string, object>
    {
        {"alg", "RS256"},
        {"x5c", new string[] { certBase64 }}, // باید آرایه string باشد
        {"sigT", sigT},
        {"crit", new string[] { "sigT" }}
    };

            var payload = new Dictionary<string, object>
    {
        { "nonce", nonce },
        { "clientId", clientId }
    };

            Console.WriteLine($"JWT Header: alg=RS256, x5c length={certBase64.Length}, sigT={sigT}");

            string token = Jose.JWT.Encode(payload, rsa, JwsAlgorithm.RS256, extraHeaders: headers);
            return token;
        }

        // ============================
        // 5) ارسال JWT برای گرفتن Access Token
        // ============================
        public static async System.Threading.Tasks.Task<string> AuthenticateAsync(string authJwt, string clientId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authJwt);
                client.DefaultRequestHeaders.Add("uid", clientId);

                // طبق مستندات باید GET باشد
                var res = await client.GetAsync("api/v2/authenticate");
                string text = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    throw new Exception($"Authenticate failed: {res.StatusCode} - {text}");

                var jo = JObject.Parse(text);
                string token = jo.Value<string>("accessToken") ?? jo.Value<string>("token") ?? jo.Value<string>("jwt") ?? jo.Value<string>("data");

                if (string.IsNullOrEmpty(token))
                    throw new Exception("Cannot find access token in authenticate response: " + text);

                return token;
            }
        }

        // ============================
        // 6) گرفتن server-information (برای دریافت public key و id)
        // ============================
        public static async System.Threading.Tasks.Task<(string publicKeyBase64, string keyId)> GetServerInformationAsync(string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var res = await client.GetAsync("api/v2/server-information");
                res.EnsureSuccessStatusCode();
                string text = await res.Content.ReadAsStringAsync();
                var jo = JObject.Parse(text);

                // structure per PDF: { "publicKeys": [ { "key": "...", "id":"...", ... } ] } or { "encryptionKey": "...", "encryptionKeyId": "..." }
                if (jo["publicKeys"] != null && jo["publicKeys"].Type == JTokenType.Array)
                {
                    var first = jo["publicKeys"].First;
                    string key = first.Value<string>("key");
                    string id = first.Value<string>("id");
                    return (key, id);
                }
                if (jo["encryptionKey"] != null)
                {
                    return (jo.Value<string>("encryptionKey"), jo.Value<string>("encryptionKeyId"));
                }

            throw new Exception("server-information returned unexpected structure: " + text);
            }
        }

        // ============================
        // 7) امضای فاکتور -> JWS (signedJson)
        // ============================
        public static string SignInvoice(string invoiceJson, string privateKeyPemPath, string certificatePath)
        {
            var rsa = LoadPrivateRsaFromPem(privateKeyPemPath);
            var certBase64 = LoadCertificateBase64(certificatePath);
            string sigT = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");

            var headers = new Dictionary<string, object>
        {
            {"x5c", new[] { certBase64 } },
            {"sigT", sigT },
            {"typ", "jose" },
            {"crit", new[] { "sigT" } },
            {"cty", "text/plain" }
        };

            string jws = Jose.JWT.Encode(invoiceJson, rsa, JwsAlgorithm.RS256, extraHeaders: headers);
            return jws;
        }

        // ============================
        // 8) رمزنگاری JWS -> JWE با کلید عمومی سرور
        // ============================
        public static string EncryptInvoice(string signedJws, string serverPublicKeyBase64, string serverPublicKeyId)
        {
            // serverPublicKeyBase64 should be base64 (DER) of the public key or X509; jose-jwt expects RSA or byte[] key depending overload
            // We'll parse as X.509 / SubjectPublicKeyInfo (DER)
            byte[] decoded = Convert.FromBase64String(serverPublicKeyBase64);
            var asymmetric = PublicKeyFactory.CreateKey(decoded); // BouncyCastle
            var rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)asymmetric);
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(rsaParams);

                var jweHeaders = new Dictionary<string, object> { { "kid", serverPublicKeyId } };
                // jose-jwt: pass payload as string or byte[]; here pass string and specify algorithm/encryption
                string jwe = Jose.JWT.Encode(signedJws, rsa, JweAlgorithm.RSA_OAEP_256, JweEncryption.A256GCM, extraHeaders: jweHeaders);
                return jwe;
            }
        }
        // ============================
        // 9) ارسال فاکتور (POST /api/v2/invoice)
        // ============================
        public static async System.Threading.Tasks.Task<string> SendInvoiceAsync(string encryptedInvoiceJwe, string fiscalId, string accessToken)
        {
            var packet = new[]
            {
            new {
                payload = encryptedInvoiceJwe,
                header = new {
                    requestTraceId = Guid.NewGuid().ToString(),
                    fiscalId = fiscalId
                }
            }
        };

            string json = JsonConvert.SerializeObject(packet);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync("api/v2/invoice", content);
                string responseBody = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    throw new Exception($"SendInvoice failed: {res.StatusCode} - {responseBody}");

                return responseBody;
            }
        }
        public static async System.Threading.Tasks.Task<(string publicKeyBase64, string keyId)> GetServerInformationWithJwt(string authJwt)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authJwt);

                    var res = await client.GetAsync("api/v2/server-information");

                    if (!res.IsSuccessStatusCode)
                    {
                        string errorBody = await res.Content.ReadAsStringAsync();
                        throw new Exception($"GetServerInfo failed: {res.StatusCode} - {errorBody}");
                    }

                    string text = await res.Content.ReadAsStringAsync();
                    var jo = JObject.Parse(text);

                    if (jo["publicKeys"] != null && jo["publicKeys"].Type == JTokenType.Array)
                    {
                        var first = jo["publicKeys"].First;
                        string key = first.Value<string>("key");
                        string id = first.Value<string>("id");
                        return (key, id);
                    }

                    throw new Exception("server-information returned unexpected structure: " + text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در GetServerInfo: {ex.Message}");
                throw;
            }
        }
        private static void ValidateCertificate(string certificatePath)
        {
            try
            {
                // بررسی با X509Certificate2
                var cert = new X509Certificate2(certificatePath);
                Console.WriteLine($"Certificate loaded successfully:");
                Console.WriteLine($"Subject: {cert.Subject}");
                Console.WriteLine($"Valid from: {cert.NotBefore} to: {cert.NotAfter}");
                Console.WriteLine($"Thumbprint: {cert.Thumbprint}");

                // تست base64 encoding
                var base64 = Convert.ToBase64String(cert.RawData);
                Console.WriteLine($"Certificate base64 length: {base64.Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Certificate validation failed: {ex.Message}");
            }
        }
        public static async System.Threading.Tasks.Task<string> SendInvoiceWithJwt(string encryptedInvoiceJwe, string fiscalId, string jwt)
        {
            var packet = new[]
            {
        new {
            payload = encryptedInvoiceJwe,
            header = new {
                requestTraceId = Guid.NewGuid().ToString(),
                fiscalId = fiscalId
            }
        }
    };

            string json = JsonConvert.SerializeObject(packet);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwt);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync("api/v2/invoice", content);
                string responseBody = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    throw new Exception($"SendInvoice failed: {res.StatusCode} - {responseBody}");

                return responseBody;
            }
        }
        private static void ValidateKeyPairMatch(string certificatePath, string privateKeyPath)
        {
            try
            {
                // بارگذاری گواهی
                var cert = new X509Certificate2(certificatePath);
                var certPublicKey = cert.GetRSAPublicKey();

                // بارگذاری کلید خصوصی
                var rsa = LoadPrivateRsaFromPem(privateKeyPath);

                // تست امضا و تایید
                string testData = "test-signature-validation";
                byte[] testBytes = Encoding.UTF8.GetBytes(testData);

                // امضا با کلید خصوصی
                byte[] signature = rsa.SignData(testBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                // تایید امضا با کلید عمومی گواهی
                bool isValid = certPublicKey.VerifyData(testBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                Console.WriteLine($"Certificate-PrivateKey match: {isValid}");

                if (!isValid)
                {
                    Console.WriteLine("❌ کلید خصوصی با گواهی مطابقت ندارد!");
                    Console.WriteLine($"Certificate thumbprint: {cert.Thumbprint}");
                    Console.WriteLine($"Certificate subject: {cert.Subject}");
                }
                else
                {
                    Console.WriteLine("✅ کلید خصوصی با گواهی مطابقت دارد");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در اعتبارسنجی: {ex.Message}");
            }
        }
        private static void DebugCertificateInfo(string certificatePath)
        {
            try
            {
                // خواندن فایل به صورت متن
                string certText = System.IO.File.ReadAllText(certificatePath);
                Console.WriteLine("Certificate content preview:");
                Console.WriteLine(certText.Substring(0, Math.Min(200, certText.Length)));
                Console.WriteLine("...");

                // بررسی فرمت
                if (certText.Contains("-----BEGIN CERTIFICATE-----"))
                {
                    Console.WriteLine("✅ Certificate is in PEM format");
                }
                else
                {
                    Console.WriteLine("⚠️ Certificate might be in DER format or corrupted");
                }

                // بارگذاری و نمایش اطلاعات
                var cert = new X509Certificate2(certificatePath);
                Console.WriteLine($"Subject: {cert.Subject}");
                Console.WriteLine($"Issuer: {cert.Issuer}");
                Console.WriteLine($"Serial Number: {cert.SerialNumber}");
                Console.WriteLine($"Valid from: {cert.NotBefore}");
                Console.WriteLine($"Valid to: {cert.NotAfter}");
                Console.WriteLine($"Is currently valid: {DateTime.Now >= cert.NotBefore && DateTime.Now <= cert.NotAfter}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در خواندن گواهی: {ex.Message}");
            }
        }
        public static string CreateSelfSignedCertWithBouncyCastle(RSA rsa, string subject)
        {
            // تبدیل RSA به BouncyCastle format
            var rsaParams = rsa.ExportParameters(true);
            var bcRsa = new RsaPrivateCrtKeyParameters(
                new BigInteger(1, rsaParams.Modulus),
                new BigInteger(1, rsaParams.Exponent),
                new BigInteger(1, rsaParams.D),
                new BigInteger(1, rsaParams.P),
                new BigInteger(1, rsaParams.Q),
                new BigInteger(1, rsaParams.DP),
                new BigInteger(1, rsaParams.DQ),
                new BigInteger(1, rsaParams.InverseQ));

            var certGen = new X509V3CertificateGenerator();
            var certName = new X509Name(subject);

            certGen.SetSerialNumber(BigInteger.ValueOf(1));
            certGen.SetIssuerDN(certName);
            certGen.SetSubjectDN(certName);
            certGen.SetNotBefore(DateTime.UtcNow.AddHours(-1));
            certGen.SetNotAfter(DateTime.UtcNow.AddYears(1));

            // کلید عمومی از کلید خصوصی
            AsymmetricKeyParameter bcPublic = new RsaKeyParameters(
                false, // false یعنی public
                ((RsaPrivateCrtKeyParameters)bcRsa).Modulus,
                ((RsaPrivateCrtKeyParameters)bcRsa).PublicExponent);

            certGen.SetPublicKey(bcPublic);

            // امضا با کلید خصوصی
            var signatureFactory = new Asn1SignatureFactory("SHA256WITHRSA", bcRsa);
            var cert = certGen.Generate(signatureFactory);
            //var cert = certGen.Generate(bcRsa);
            return Convert.ToBase64String(cert.GetEncoded());
        }

        public async System.Threading.Tasks.Task<ActionResult> SamaneMoadianTest(long HeaderId)
            {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            string path = "";
                string path2 = "";
           
                try
                {

                cartaxtest2Entities entities = new cartaxtest2Entities();
                Avarez.Models.cartaxEntities p = new Avarez.Models.cartaxEntities();

                var user = entities.prs_User_GharardadSelect("fldId", Session["TaxUserId"].ToString(), 1, "", 1, "").FirstOrDefault();

                var TransactionInf = entities.prs_TransactionInfSelect("fldTarfGharardadId", user.fldTarfGharardadId.ToString(), 0).FirstOrDefault();
                Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
                var divName = "جمهوری اسلامی ایران";
                bool Tr = h.Transaction(TransactionInf.fldUserName, TransactionInf.fldPass, 0/*(int)TransactionInf.CountryType*/, divName);
                if (Tr != true)
                {
                    return base.Json(new
                    {
                        Msg = "اعتبار شما پایان یافته است",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }


                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                    prs_User_GharardadSelect select = entities.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
                    prs_tblTarfGharardadSelect select2 = entities.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>();
                path = AppDomain.CurrentDomain.BaseDirectory + "Uploaded\\privateKey" + select2.fldId.ToString() + ".pem";
                path2 = AppDomain.CurrentDomain.BaseDirectory + "Uploaded\\certificate" + select2.fldId.ToString() + ".crt";

                //path = "C:/privateKey.pem";
                //path2 = "C:/certificate.crt";



                // شناسه یکتای مالیاتی (UID فروشنده)
                string uid = select2.fldUniqId;//A3A3TZ

                // مسیر Private Key
                string privateKeyPem = @"C:\Keys\privateKey.pem";
                string certificateCrt = @"C:\keys\certificate.crt";


                string clientId = select2.fldUniqId;   // شناسه یکتا
                string fiscalId = select2.fldUniqId;   // شناسه حافظه/فروشنده (ممکن است همان clientId باشد)

                InvoiceDto item = CreateValidInvoice(clientId, HeaderId);
                List<InvoiceDto> list1 = new List<InvoiceDto>();
                list1.Add(item);
                List<InvoiceDto> invoiceList = list1;
                string invoiceJson = Newtonsoft.Json.JsonConvert.SerializeObject(invoiceList);
                // string invoiceJson = System.IO.File.ReadAllText(@"C:\invoices\sampleInvoice.json"); // یا build JSON

                ValidateCertificate(certificateCrt);
               /* Console.WriteLine("=== بررسی گواهی و کلید خصوصی ===");
                DebugCertificateInfo(certificateCrt);
                ValidateKeyPairMatch(certificateCrt, privateKeyPem);

                var rsa = LoadPrivateRsaFromPem(privateKeyPem);
                string subject = "CN=Shahrdari Mojen[Stamp], SERIALNUMBER=14001976776, OU=شهرداري مجن, O=Governmental, L=شاهرود,  C=IR";
                    string pemCert = CreateSelfSignedCertWithBouncyCastle(rsa, subject);
*/

                // 1) nonce اول برای server-information
                string nonce1 = await GetNonceAsync();
                Console.WriteLine("nonce1: " + nonce1);

                // 2) JWT اول برای server-information
                string authJwt1 = CreateAuthJwtWithNonce(nonce1, clientId, privateKeyPem, certificateCrt);
                Console.WriteLine("authJwt1: " + authJwt1);

                // 3) گرفتن server information
                var serverInfo = await GetServerInformationWithJwt(authJwt1);
                Console.WriteLine("server key id: " + serverInfo.keyId + " key len: " + (serverInfo.publicKeyBase64?.Length ?? 0));

                // 4) امضای فاکتور -> JWS
                string signedJws = SignInvoice(invoiceJson, privateKeyPem, certificateCrt);
                Console.WriteLine("signedJWS length: " + signedJws.Length);

                // 5) رمزنگاری JWE
                string jwe = EncryptInvoice(signedJws, serverInfo.publicKeyBase64, serverInfo.keyId);
                Console.WriteLine("JWE length: " + jwe.Length);

                // 6) nonce دوم برای ارسال فاکتور
                string nonce2 = await GetNonceAsync();
                Console.WriteLine("nonce2: " + nonce2);

                // 7) JWT دوم برای ارسال فاکتور
                string authJwt2 = CreateAuthJwtWithNonce(nonce2, clientId, privateKeyPem, certificateCrt);
                Console.WriteLine("authJwt2: " + authJwt2);

                // 8) ارسال فاکتور با JWT جدید
                string sendResponse = await SendInvoiceWithJwt(jwe, fiscalId, authJwt2);
                Console.WriteLine("Send response: " + sendResponse);

                //********
                /*
                                if (!System.IO.File.Exists(path)) 
                                    {
                                    System.IO.File.WriteAllBytes(path, select2.fldPrivateKey.ToArray<byte>());
                                    }
                                    if (!System.IO.File.Exists(path2))
                                    {
                                    System.IO.File.WriteAllBytes(path2, select2.fldSignatureCertificate.ToArray<byte>());
                                    }
                                    string fldUniqId = select2.fldUniqId;


                                ss = "1";
                                    ITaxApi taxApi = CreateTaxApi(fldUniqId, "https://tp.tax.gov.ir/requestsmanager", path, path2);
                                ss = "2";
                                    InvoiceDto item = CreateValidInvoice(fldUniqId, HeaderId);
                                    List <InvoiceDto> list1 = new List<InvoiceDto>();
                                    list1.Add(item);
                                    List<InvoiceDto> invoiceList = list1;

                              

                ss = "3";


                List<InvoiceResponseModel> responseModels = taxApi.SendInvoices(invoiceList);
            

                Thread.Sleep(10_000);
                ss = "4";
               InquiryByReferenceNumberDto inquiryDto = new InquiryByReferenceNumberDto(responseModels.Select(r => r.ReferenceNumber).ToList());
                List<InquiryResultModel> inquiryResults = taxApi.InquiryByReferenceId(inquiryDto);
                string SerializeObjectErsal = Newtonsoft.Json.JsonConvert.SerializeObject(invoiceList);
                ss = "5";
                ss = inquiryResults[0].Status+"**"+ inquiryResults[0].ReferenceNumber + "**" + inquiryResults[0].Uid;

                if (inquiryResults[0].Status == "IN_PROGRESS" || inquiryResults[0].Status == "NOT_FOUND")
                {
                    var uid = "";
                    if (inquiryResults[0].Uid != null)
                        uid = inquiryResults[0].Uid;
                    entities.prs_tblSooratHesabStatusInsert(new long?((long)HeaderId), 4, inquiryResults[0].Status, inquiryResults[0].ReferenceNumber, SerializeObjectErsal, uid, Convert.ToInt64(Session["TaxUserId"]),IP);
                    return base.Json(new
                    {
                        Msg = "عدم دریافت پاسخ از سامانه مودیان.("+ inquiryResults[0].ReferenceNumber + "**" + inquiryResults[0].Uid+ ")",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }

                 mmsgg=PrintInquiryResult(inquiryResults, HeaderId,Convert.ToInt64(Session["TaxUserId"]), SerializeObjectErsal,0);
                */
                string mmsgg = "";
                var msgtitle = "ارسال موفق";
                var msg = "ارسال با موفقیت انجام شد.";

                if (mmsgg.Split(';')[0] != "1")
                    msg = mmsgg.Split(';')[1];
                //else
                //{
                //    Avarez.Models.cartaxEntities p = new Avarez.Models.cartaxEntities();
                //    var TransactionInf = entities.prs_TransactionInfSelect("fldTarfGharardadId", Session["TarafGharardadId"].ToString(), 0).FirstOrDefault();
                //    Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
                //    var divName = "جمهوری اسلامی ایران";
                //    bool Tr = h.Transaction(TransactionInf.fldUserName, TransactionInf.fldPass, 0/*(int)TransactionInf.CountryType*/, divName);
                //    if (Tr != true)
                //    {
                //        msg = "ارسال با موفقیت انجام شد." + ")اعتبار شما پایان یافته است)";
                //    }
                //}

                if (mmsgg.Split(';')[0] == "2")
                {
                    msgtitle = "هشدار";
                    msg = "ارسال همراه با هشدار انجام شد.";
                }
                if (mmsgg.Split(';')[0] == "3")
                {
                    msgtitle = "خطا";
                    msg = "خطا در ارسال.";
                }
               


                //System.IO.File.Delete(path);
                //System.IO.File.Delete(str2);

                return base.Json(new
                {
                    Msg = msg,
                    MsgTitle = msgtitle,
                    Er = mmsgg.Split(';')[0]
                }, JsonRequestBehavior.AllowGet);
            }
                catch (Exception x)
                {
                //string str7 = "";
                //str7 = (exception.InnerException == null) ? exception.Message : exception.InnerException.Message;
                //return str7;
                cartaxtest2Entities m = new cartaxtest2Entities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                m.sp_ErrorProgramInsert(Eid, ss+InnerException, Convert.ToInt32(Session["TaxUserId"]), x.Message, DateTime.Now, "");

                //X.Msg.Show(new MessageBoxConfig
                //{
                //    Buttons = MessageBox.Button.OK,
                //    Icon = MessageBox.Icon.ERROR,
                //    Title = "خطا",
                //    Message = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید."
                //});
                //X.Mask.Hide();
                //DirectResult result = new DirectResult();
              
                return base.Json(new
                    {
                        MsgTitle = "خطا",
                         Msg ="خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        
        public async System.Threading.Tasks.Task<ActionResult> SamaneMoadian(long HeaderId)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            string path = "";
            string path2 = "";

            try
            {

                cartaxtest2Entities entities = new cartaxtest2Entities();
                Avarez.Models.cartaxEntities p = new Avarez.Models.cartaxEntities();

                var user = entities.prs_User_GharardadSelect("fldId", Session["TaxUserId"].ToString(), 1, "", 1, "").FirstOrDefault();

                var TransactionInf = entities.prs_TransactionInfSelect("fldTarfGharardadId", user.fldTarfGharardadId.ToString(), 0).FirstOrDefault();
                Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
                var divName = "جمهوری اسلامی ایران";
                bool Tr = h.Transaction(TransactionInf.fldUserName, TransactionInf.fldPass, 0/*(int)TransactionInf.CountryType*/, divName);
                if (Tr != true)
                {
                    return base.Json(new
                    {
                        Msg = "اعتبار شما پایان یافته است",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }


                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                prs_User_GharardadSelect select = entities.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
                prs_tblTarfGharardadSelect select2 = entities.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>();
                path = AppDomain.CurrentDomain.BaseDirectory + "Uploaded\\privateKey" + select2.fldId.ToString() + ".pem";
                path2 = AppDomain.CurrentDomain.BaseDirectory + "Uploaded\\certificate" + select2.fldId.ToString() + ".crt";

              


                if (!System.IO.File.Exists(path)) 
                                    {
                                    System.IO.File.WriteAllBytes(path, select2.fldPrivateKey.ToArray<byte>());
                                    }
                                    if (!System.IO.File.Exists(path2))
                                    {
                                    System.IO.File.WriteAllBytes(path2, select2.fldSignatureCertificate.ToArray<byte>());
                                    }
                                    string fldUniqId = select2.fldUniqId;

               
            

                ss = "1";
                                  //  ITaxApi taxApi = CreateTaxApi(fldUniqId, "https://tp.tax.gov.ir/requestsmanager", path, path2);
                                ss = "2";
                                    InvoiceDto item = CreateValidInvoice(fldUniqId, HeaderId);
                                    List <InvoiceDto> list1 = new List<InvoiceDto>();
                                    list1.Add(item);
                                    List<InvoiceDto> invoiceList = list1;
                string invoiceJson = Newtonsoft.Json.JsonConvert.SerializeObject(invoiceList);


                ss = "3";


                // تنظیمات اولیه
                 string memoryId = select2.fldUniqId;
                const string apiUrl = "https://tp.tax.gov.ir/requestsmanager";
                string privateKeyPath = path;// "C:/Keys/privateKey.pem";
                string certificatePath = path2;// "C:/Keys/certificate.crt";
                //if (memoryId == "A3HHG7")
                //{
                //    privateKeyPath = "C:/Keys/Private.pem";
                //    certificatePath = "C:/Keys/ShahrdariMojen.crt";
                //}


                // ایجاد نمونه ارسال کننده
                var sender = new InvoiceSender(memoryId, apiUrl, privateKeyPath, certificatePath);

                // ========== ارسال و استعلام هوشمند ==========
                var result = await SendAndInquireWithRetry(sender, item);

                if (result != null)
                {
                    Console.WriteLine("\n✅ موفق:");
                    sender.PrintInquiryResult(new List<InquiryResultModel> { result }, HeaderId, invoiceJson, Convert.ToInt64(Session["TaxUserId"]));
                }
                else
                {
                    Console.WriteLine("\n❌ ناموفق: فاکتور پیدا نشد");
                }
/*
                // ارسال فاکتور و دریافت نتیجه
                var results = await sender.SendInvoiceAsync(item);

                // چاپ نتیجه
                sender.PrintInquiryResult(results, HeaderId, invoiceJson, Convert.ToInt64(Session["TaxUserId"]));
                string uid = "";
                string referenceNumber = results[0].ReferenceNumber;
                if (results[0].Uid != null)
                    uid = results[0].Uid;
                entities.prs_tblSooratHesabStatusInsert(HeaderId, 4, "", referenceNumber, invoiceJson, uid, Convert.ToInt64(Session["TaxUserId"]), "1");
             

                Console.WriteLine("\n\n=== استعلام با شماره پیگیری ===");
                var inquiryByRef = sender.InquiryByReferenceNumber(
                    new List<string> { referenceNumber }
                );
                sender.PrintInquiryResult(inquiryByRef, HeaderId, invoiceJson,Convert.ToInt64(Session["TaxUserId"]));

                //var st = entities.prs_tblSooratHesabStatusSelect("fldHeaderId", HeaderId.ToString(), 0).FirstOrDefault();
                //await InquireInvoiceStatus(st.fldUid, st.fldReferenceNumber, CLIENT_ID);

                Console.WriteLine("=== ارسال فاکتور با موفقیت انجام شد ===");
                */
                /*   List<InvoiceResponseModel> responseModels = taxApi.SendInvoices(invoiceList);


                   Thread.Sleep(10_000);
                   ss = "4";
                  InquiryByReferenceNumberDto inquiryDto = new InquiryByReferenceNumberDto(responseModels.Select(r => r.ReferenceNumber).ToList());
                   List<InquiryResultModel> inquiryResults = taxApi.InquiryByReferenceId(inquiryDto);
                   string SerializeObjectErsal = Newtonsoft.Json.JsonConvert.SerializeObject(invoiceList);
                   ss = "5";
                   ss = inquiryResults[0].Status+"**"+ inquiryResults[0].ReferenceNumber + "**" + inquiryResults[0].Uid;

                   if (inquiryResults[0].Status == "IN_PROGRESS" || inquiryResults[0].Status == "NOT_FOUND")
                   {
                       var uid = "";
                       if (inquiryResults[0].Uid != null)
                           uid = inquiryResults[0].Uid;
                       entities.prs_tblSooratHesabStatusInsert(new long?((long)HeaderId), 4, inquiryResults[0].Status, inquiryResults[0].ReferenceNumber, SerializeObjectErsal, uid, Convert.ToInt64(Session["TaxUserId"]),IP);
                       return base.Json(new
                       {
                           Msg = "عدم دریافت پاسخ از سامانه مودیان.("+ inquiryResults[0].ReferenceNumber + "**" + inquiryResults[0].Uid+ ")",
                           MsgTitle = "خطا",
                           Er = 1
                       }, JsonRequestBehavior.AllowGet);
                   }

                   var mmsgg=PrintInquiryResult(inquiryResults, HeaderId,Convert.ToInt64(Session["TaxUserId"]), SerializeObjectErsal,0);
                   */
                var mmsgg = "";
                var msgtitle = "ارسال موفق";
                var msg = "ارسال با موفقیت انجام شد.";

                /*if (mmsgg.Split(';')[0] != "1")
                    msg = mmsgg.Split(';')[1];
               

                if (mmsgg.Split(';')[0] == "2")
                {
                    msgtitle = "هشدار";
                    msg = "ارسال همراه با هشدار انجام شد.";
                }
                if (mmsgg.Split(';')[0] == "3")
                {
                    msgtitle = "خطا";
                    msg = "خطا در ارسال.";
                }*/



                //System.IO.File.Delete(path);
                //System.IO.File.Delete(str2);

                return base.Json(new
                {
                    Msg = msg,
                    MsgTitle = msgtitle,
                    Er = mmsgg//.Split(';')[0]
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                //string str7 = "";
                //str7 = (exception.InnerException == null) ? exception.Message : exception.InnerException.Message;
                //return str7;
                cartaxtest2Entities m = new cartaxtest2Entities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                m.sp_ErrorProgramInsert(Eid, ss + InnerException, Convert.ToInt32(Session["TaxUserId"]), x.Message, DateTime.Now, "");

                //X.Msg.Show(new MessageBoxConfig
                //{
                //    Buttons = MessageBox.Button.OK,
                //    Icon = MessageBox.Icon.ERROR,
                //    Title = "خطا",
                //    Message = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید."
                //});
                //X.Mask.Hide();
                //DirectResult result = new DirectResult();

                return base.Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }

        static async System.Threading.Tasks.Task<InquiryResultModel> SendAndInquireWithRetry(
            InvoiceSender sender,
            InvoiceDto invoice,
            int maxAttempts = 6)
        {
            Console.WriteLine("📤 ارسال فاکتور...");

            // ارسال فاکتور
            var invoiceList = new List<InvoiceDto> { invoice };
            var taxApi = sender.TaxApi;
            var responseModels = taxApi.SendInvoices(invoiceList);

            if (responseModels == null || responseModels.Count == 0)
            {
                Console.WriteLine("❌ خطا در ارسال فاکتور");
                return null;
            }

            var response = responseModels[0];
            Console.WriteLine($"✅ ارسال شد:");
            Console.WriteLine($"   Reference: {response.ReferenceNumber}");
            Console.WriteLine($"   UID: {response.Uid}");
            Console.WriteLine($"   TaxId: {response.TaxId}");

            // زمان‌بندی تلاش‌های استعلام (به ثانیه)
            int[] waitTimes = { 15, 10, 10, 15, 20, 30 };

            for (int attempt = 0; attempt < Math.Min(maxAttempts, waitTimes.Length); attempt++)
            {
                Console.WriteLine($"\n⏳ انتظار {waitTimes[attempt]} ثانیه... (تلاش {attempt + 1}/{maxAttempts})");
                await System.Threading.Tasks.Task.Delay(waitTimes[attempt] * 1000);

                // استعلام با بازه زمانی گسترده
                var result = InquireInvoice(
                    taxApi,
                    response.ReferenceNumber,
                    response.Uid,
                    sender
                );

                if (result == null)
                {
                    Console.WriteLine("⚠️ خطا در استعلام");
                    continue;
                }

                Console.WriteLine($"📊 وضعیت: {result.Status}");

                // بررسی وضعیت
                switch (result.Status)
                {
                    case "SUCCESS":
                        Console.WriteLine("✅ فاکتور با موفقیت پردازش شد!");
                        return result;

                    case "FAILED":
                        Console.WriteLine("❌ فاکتور رد شد");
                        return result;

                    case "TIMEOUT":
                        Console.WriteLine("⏱️ تایم‌اوت در پردازش");
                        return result;

                    case "IN_PROGRESS":
                        Console.WriteLine("⏳ هنوز در حال پردازش...");
                        if (attempt < maxAttempts - 1)
                            continue;
                        break;

                    case "NOT_FOUND":
                        Console.WriteLine("⚠️ NOT_FOUND - تلاش با روش دیگر...");

                        // تلاش با UID
                        var uidResult = InquireByUid(
                            taxApi,
                            response.Uid,
                            sender
                        );

                        if (uidResult != null && uidResult.Status != "NOT_FOUND")
                        {
                            Console.WriteLine($"✅ با UID پیدا شد: {uidResult.Status}");
                            return uidResult;
                        }

                        if (attempt < maxAttempts - 1)
                            continue;
                        break;
                }
            }

            // تلاش آخر: جستجو در فاکتورهای اخیر
            Console.WriteLine("\n🔍 جستجو در فاکتورهای اخیر...");
            var recentResult = SearchInRecentInvoices(taxApi, response.TaxId);

            if (recentResult != null)
            {
                Console.WriteLine("✅ فاکتور در لیست اخیر پیدا شد!");
                return recentResult;
            }

            Console.WriteLine($"❌ بعد از {maxAttempts} تلاش، فاکتور پیدا نشد");
            return null;
        }

        // استعلام با شماره پیگیری
        static InquiryResultModel InquireInvoice(
            ITaxApi taxApi,
            string referenceNumber,
            string uid,
            InvoiceSender sender)
        {
            try
            {
                // بازه زمانی گسترده: 3 ساعت قبل تا 10 دقیقه بعد
                var inquiryDto = new InquiryByReferenceNumberDto(
                    new List<string> { referenceNumber },
                    DateTime.Now.AddHours(-3),
                    DateTime.Now.AddMinutes(10)
                );

                var results = taxApi.InquiryByReferenceId(inquiryDto);

                if (results != null && results.Count > 0)
                {
                    return results[0];
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در استعلام: {ex.Message}");
                return null;
            }
        }

        // استعلام با UID
        static InquiryResultModel InquireByUid(
            ITaxApi taxApi,
            string uid,
            InvoiceSender sender)
        {
            try
            {
                var memoryId = "A11216"; // از تنظیمات بگیر

                var inquiryDto = new InquiryByUidDto(
                    new List<string> { uid },
                    memoryId,
                    DateTime.Now.AddHours(-3),
                    DateTime.Now.AddMinutes(10)
                );

                var results = taxApi.InquiryByUid(inquiryDto);

                if (results != null && results.Count > 0)
                {
                    return results[0];
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در استعلام با UID: {ex.Message}");
                return null;
            }
        }

        // جستجو در فاکتورهای اخیر
        static InquiryResultModel SearchInRecentInvoices(
            ITaxApi taxApi,
            string targetTaxId)
        {
            try
            {
                var inquiryDto = new InquiryByTimeRangeDto(
                    DateTime.Now.AddHours(-2),
                    DateTime.Now.AddMinutes(10),
                    new Pageable(1, 100),
                    null // همه وضعیت‌ها
                );

                var results = taxApi.InquiryByTime(inquiryDto);

                if (results == null || results.Count == 0)
                {
                    Console.WriteLine("   هیچ فاکتوری در 2 ساعت اخیر نیست");
                    return null;
                }

                Console.WriteLine($"   {results.Count} فاکتور پیدا شد، در حال جستجو...");

                // جستجو بر اساس نزدیک‌ترین زمان (آخرین فاکتور)
                var latest = results
                    .Where(r => r.Status != "NOT_FOUND")
                    .OrderByDescending(r => r.ReferenceNumber)
                    .FirstOrDefault();

                return latest;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در جستجو: {ex.Message}");
                return null;
            }
        }

        public async System.Threading.Tasks.Task<ActionResult> EstelamSamaneMoadian(long HeaderId)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            cartaxtest2Entities entities = new cartaxtest2Entities();
            var st = entities.prs_tblSooratHesabStatusSelect("fldHeaderId", HeaderId.ToString(), 0).FirstOrDefault();
            var select = entities.prs_User_GharardadSelect("fldID", st.fldUserId.ToString(), 0, "", 1, "").FirstOrDefault();
            var select2 = entities.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault();
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Uploaded\\privateKey" + select2.fldId.ToString() + ".pem";
                string path2 = AppDomain.CurrentDomain.BaseDirectory + "Uploaded\\certificate" + select2.fldId.ToString() + ".crt";




                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.WriteAllBytes(path, select2.fldPrivateKey.ToArray<byte>());
                }
                if (!System.IO.File.Exists(path2))
                {
                    System.IO.File.WriteAllBytes(path2, select2.fldSignatureCertificate.ToArray<byte>());
                }

                string memoryId = select2.fldUniqId;
                const string apiUrl = "https://tp.tax.gov.ir/requestsmanager";
                string privateKeyPath = path;// "C:/Keys/privateKey.pem";
                string certificatePath = path2;// "C:/Keys/certificate.crt";
                //if (memoryId == "A3HHG7")
                //{
                //    privateKeyPath = "C:/Keys/Private.pem";
                //    certificatePath = "C:/Keys/ShahrdariMojen.crt";
                //}


                // ایجاد نمونه ارسال کننده
                var sender = new InvoiceSender(memoryId, apiUrl, privateKeyPath, certificatePath);

                var inquiryByRef = sender.InquiryByReferenceNumber(
                    new List<string> { st.fldReferenceNumber }
                );
                sender.PrintInquiryResult(inquiryByRef, HeaderId, st.fldSerializeObject, Convert.ToInt64(Session["TaxUserId"]));


                //await InitializeTaxApiService(CLIENT_ID, PRIVATE_KEY_PEM_PATH, BASE_URL);

                //await TestConnection();
                //// مرحله 2: دریافت اطلاعات سرور
                //await GetServerInformation();

                //// مرحله 3: دریافت توکن دسترسی
                //await RequestAccessToken();
                //// کمی صبر کنیم تا فاکتور پردازش شود
                //// await System.Threading.Tasks.Task.Delay(5000);

                //// استعلام با UID
                //var uidAndFiscalId = new UidAndFiscalId(st.fldUid, select2.fldUniqId);
                //var inquiryResult = TaxApiService.Instance.TaxApis.InquiryByUidAndFiscalId(new List<UidAndFiscalId>() { uidAndFiscalId });

                //if (inquiryResult != null && inquiryResult.Count > 0)
                //{

                //    var result = inquiryResult[0];
                //    Console.WriteLine($"وضعیت فاکتور: {result.Status}");
                //    Console.WriteLine($"UID: {result.Uid}");
                //    Console.WriteLine($"شماره پیگیری: {result.ReferenceNumber}");

                //    //entities.prs_tblSooratHesabStatusUpdate_Matn( HeaderId, 2, result.Status, Convert.ToInt32(Session["TaxUserId"]), IP);
                //    /* if (result.Data != null)
                //     {
                //         Console.WriteLine($"موفقیت: {result.Data.Success}");
                //         if (result.Data.Error != null && result.Data.Error.Count > 0)
                //         {
                //             Console.WriteLine("خطاهای موجود:");
                //             foreach (var error in result.Data.Error)
                //             {
                //                 Console.WriteLine($"  - کد: {error.Code}, پیام: {error.Message}");
                //             }
                //         }
                //         if (result.Data.Warning != null && result.Data.Warning.Count > 0)
                //         {
                //             Console.WriteLine("هشدارهای موجود:");
                //             foreach (var warning in result.Data.Warning)
                //             {
                //                 Console.WriteLine($"  - کد: {warning.Code}, پیام: {warning.Message}");
                //             }
                //         }
                //     }*/

                //    return base.Json(new
                //    {
                //        Msg = $"وضعیت فاکتور: {result.Status}",
                //        MsgTitle = "عملیات موفق",
                //        Er = 0
                //    }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                return base.Json(new
                {
                    Msg = "⚠️ اطلاعات وضعیت فاکتور دریافت نشد",
                    MsgTitle = "خطا",
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
                //   // Console.WriteLine("⚠️ اطلاعات وضعیت فاکتور دریافت نشد");
                //}
            }
            catch (Exception ex)
            {
                return base.Json(new
                {
                    Msg = $"⚠️ خطا در استعلام وضعیت: {ex.Message}",
                    MsgTitle = "خطا",
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
                //Console.WriteLine($"⚠️ خطا در استعلام وضعیت: {ex.Message}");
            }
           
        }

        public ActionResult Savee(long HeaderId, prs_SelectHeaderSooratHesab Header,List<prs_SelectDetailSooratHesab> Grid_DetailsArray,  int fldForushandeId, int fldKharidarId,long serial)
            {
                string str="ذخیره با موفقیت انجام شد.";
                string str2 = "عملیات موفق";
                int num=0;
                if (base.Session["TaxUserId"] != null)
                {
                str = "ذخیره با موفقیت انجام شد.";
                str2 = "عملیات موفق";
                num = 0;
                    try
                    {
                    
                        ParamValue value2;
                        cartaxtest2Entities entities = new cartaxtest2Entities();
                        var  tt = entities.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault();

                        DateTime Indatim = Shamsi.Shamsi2miladiDateTime(Header.fldSh_Indatim);
                        char[] separator = new char[] { ':' };
                        char[] chArray2 = new char[] { ':' };
                        TimeSpan Indatimspan = new TimeSpan(Convert.ToInt32(Header.fldIndatim_Zaman.Split(separator)[0]), Convert.ToInt32(Header.fldIndatim_Zaman.Split(chArray2)[1]), 0);
                        Indatim = Indatim.Date + Indatimspan;
                        DateTime? Indati2m = null;
                        if (Header.fldSh_Indati2m != null)
                        {
                            Indati2m = new DateTime?(Shamsi.Shamsi2miladiDateTime(Header.fldSh_Indati2m));
                            char[] chArray3 = new char[] { ':' };
                            char[] chArray4 = new char[] { ':' };
                            TimeSpan Indati2mspan2 = new TimeSpan(Convert.ToInt32(Header.fldIndati2m_Zaman.Split(chArray3)[0]), Convert.ToInt32(Header.fldIndati2m_Zaman.Split(chArray4)[1]), 0);
                            Indati2m = new DateTime?(Convert.ToDateTime(Indati2m).Date + Indati2mspan2);
                        }
                        Header.fldTaxId= GenerateTaxId(serial, Indatim, entities.prs_tblTarfGharardadSelect("fldId", tt.fldTarfGharardadId.ToString(), 0).FirstOrDefault().fldUniqId);
                        
                        long? tprdis = 0;
                        long? tdis = 0;
                        long? tadis = 0;
                        long? tvam = 0;
                        long? todam = 0;
                        long? tbill = 0;
                        decimal? tonw = 0;
                        long? torv = 0;
                        decimal? tocv = 0;
                        long? tvop = 0;
                        foreach (var hesab in Grid_DetailsArray)
                        {
                            if (hesab.fldprdis != null)
                            {
                                tprdis = tprdis+hesab.fldprdis;
                            }
                            if (hesab.flddis != null)
                            {
                                tdis = tdis + hesab.flddis;
                            }
                            if (hesab.fldadis != null)
                            {
                                tadis = tadis + hesab.fldadis;
                            }
                            if (hesab.fldvam != null)
                            {
                                tvam = tvam + hesab.fldvam;
                            }
                            if (hesab.fldodam != null)
                            {
                                todam = todam + hesab.fldodam;
                            }
                            if (hesab.fldtsstam != null)
                            {
                                tbill = tbill + hesab.fldtsstam;
                            }
                            if (hesab.fldnw != null)
                            {
                                tonw = tonw + hesab.fldnw;
                            }
                            if (hesab.fldssrv != null)
                            {
                                torv = torv + hesab.fldssrv;
                            }
                            if (hesab.fldsscv != null)
                            {
                                tocv = tocv + hesab.fldsscv;
                            }
                            if (hesab.fldvop != null)
                            {
                                tvop = tvop + hesab.fldvop;
                            }
                        }
                        decimal num3 = 0M;
                        List<ParamValue> source = new List<ParamValue>();
                        Header.fldtvop = tvop;
                        Header.fldtocv = tocv;
                        Header.fldtorv = torv;
                        Header.fldtonw = tonw;
                        Header.fldtbill = tbill;
                        Header.fldtodam = todam;
                        Header.fldtvam = tvam;
                        Header.fldtadis = tadis;
                        Header.fldtdis = tdis;
                        Header.fldtprdis = tprdis;

                        byte? fldins = 0;
                        if ((Header.fldins != null))
                            fldins = Header.fldins;

                        value2 = new ParamValue
                        {
                            fldParamertId = 1,
                            fldValue = fldins.ToString()
                        };
                        source.Add(value2);

                        //byte? fldft = 0;
                        //if (Header.fldft != null)
                        //{
                        //    fldft = Header.fldft;
                        //    value2 = new ParamValue
                        //    {
                        //        fldParamertId = 9,
                        //        fldValue = Header.fldft.ToString()
                        //    };
                        //    source.Add(value2);
                        //}
                        //if ((Header.fldbpn != "") && (Header.fldbpn != null))
                        //{
                        //    value2 = new ParamValue
                        //    {
                        //        fldParamertId = 10,
                        //        fldValue = Header.fldbpn
                        //    };
                        //    source.Add(value2);
                        //}
                        //if ((Header.fldscln != "") && (Header.fldscln != null))
                        //{
                        //    value2 = new ParamValue
                        //    {
                        //        fldParamertId = 11,
                        //        fldValue = Header.fldscln
                        //    };
                        //    source.Add(value2);
                        //}
                        //if ((Header.fldscc != "") && (Header.fldscc != null))
                        //{
                        //    value2 = new ParamValue
                        //    {
                        //        fldParamertId = 12,
                        //        fldValue = Header.fldscc
                        //    };
                        //    source.Add(value2);
                        //}
                        if ((Header.fldcdcn != "") && (Header.fldcdcn != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 13,
                                fldValue = Header.fldcdcn
                            };
                            source.Add(value2);
                        }
                     
                        if (Header.fldcdcd != null)
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 14,
                                fldValue = Header.fldcdcd.ToString()
                            };
                            source.Add(value2);
                        }
                        //if ((Header.fldcrn != "") && (Header.fldcrn != null))
                        //{
                        //    value2 = new ParamValue
                        //    {
                        //        fldParamertId = 15,
                        //        fldValue = Header.fldcrn
                        //    };
                        //    source.Add(value2);
                        //}
                        if ((Header.fldbilid != "") && (Header.fldbilid != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x10,
                                fldValue = Header.fldbilid
                            };
                            source.Add(value2);
                        }
                        long? fldtprdis = 0;
                        if ((Header.fldtprdis != null))
                            fldtprdis = Header.fldtprdis;

                        value2 = new ParamValue
                        {
                            fldParamertId = 0x11,
                            fldValue = fldtprdis.ToString()
                        };
                        source.Add(value2);

                        long? fldtdis = 0;
                        if ((Header.fldtdis != null))
                            fldtdis = Header.fldtdis;

                        value2 = new ParamValue
                        {
                            fldParamertId = 0x12,
                            fldValue = fldtdis.ToString()
                        };

                        long? fldtadis = 0;
                        if ((Header.fldtadis != null))
                            fldtadis = Header.fldtadis;

                        value2 = new ParamValue
                        {
                            fldParamertId = 0x13,
                            fldValue = fldtadis.ToString()
                        };
                        source.Add(value2);

                        long? fldtvam = 0;
                        if ((Header.fldtvam != null))
                            fldtvam = Header.fldtvam;

                        value2 = new ParamValue
                        {
                            fldParamertId = 20,
                            fldValue = fldtvam.ToString()
                        };
                        source.Add(value2);

                        long? fldtodam = 0;
                        if ((Header.fldtodam != null))
                            fldtodam = Header.fldtodam;
                        value2 = new ParamValue
                        {
                            fldParamertId = 0x15,
                            fldValue = fldtodam.ToString()
                        };
                        source.Add(value2);

                        long? fldtbill = fldtvam + fldtodam + fldtadis;
                        if ((fldtbill != null))
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x16, //tbill
                                fldValue = fldtbill.ToString()
                            };
                            source.Add(value2);
                        }

                        if (Header.fldtonw != null && Header.fldtonw != 0)
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x17,
                                fldValue = Header.fldtonw.ToString()
                            };
                            source.Add(value2);
                        }
                       
                        if (Header.fldtorv != null && Header.fldtorv !=0)
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x18,
                                fldValue = Header.fldtorv.ToString()
                            };
                            source.Add(value2);
                        }

                        if (Header.fldtocv != null && Header.fldtocv != 0)
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x19,
                                fldValue = Header.fldtocv.ToString()
                            };
                            source.Add(value2);
                        }

                        byte? fldsetm = 0;
                        if ((Header.fldsetm != null))
                            fldsetm = Header.fldsetm;

                        value2 = new ParamValue
                        {
                            fldParamertId = 26,
                            fldValue = fldsetm.ToString()
                        };
                    source.Add(value2);

                    if (Header.fldcap != null && Header.fldcap !=0)
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x1b,
                                fldValue = Header.fldcap.ToString()
                            };
                            source.Add(value2);
                        }
             
                        if (Header.fldinsp != null && Header.fldinsp !=0)
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x1c,
                                fldValue = Header.fldinsp.ToString()
                            };
                            source.Add(value2);
                        }
                        if (Header.fldtvop != null)
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 0x1d,
                                fldValue = Header.fldtvop.ToString()
                            };
                            source.Add(value2);
                        }
                       
                        if (Header.fldtax17 != null)
                        {
                            value2 = new ParamValue
                            {
                                fldParamertId = 30,
                                fldValue = Header.fldtax17.ToString()
                            };
                            source.Add(value2);
                        }
                      
                    System.Data.DataTable dt = new System.Data.DataTable { TableName = "movadi.tblSooratHesabHeader_Value" };
                    using (var reader = FastMember.ObjectReader.Create(source))
                    {
                        dt.Load(reader);
                    }
                    if (HeaderId == 0)
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter SId = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(Int64));
                        long HeaderIdd = entities.prs_tblSooratHesab_HeaderInsert(SId, Header.fldTaxId, Indatim, Indati2m, Convert.ToByte(Header.fldInty), Header.fldinp, Header.fldInno, Header.fldIrtaxId, Convert.ToInt64(fldKharidarId), Convert.ToInt64(fldForushandeId),
                            Header.fldFunctionName, Header.fldbpn, Header.fldft, Header.fldscln, Header.fldscc, Header.fldcrn,dt, Header.fldShomareFish, Convert.ToInt64(Session["TaxUserId"]),IP);

                        saveDetail(HeaderIdd, Grid_DetailsArray,1);
                        HeaderId = HeaderIdd;
                    }
                    else
                    {
                        entities.prs_tblSooratHesab_HeaderUpdate(HeaderId, Header.fldTaxId, Indatim, Indati2m, Convert.ToByte(Header.fldInty), Header.fldinp, Header.fldInno, Header.fldIrtaxId, Convert.ToInt64(fldKharidarId), Convert.ToInt64(fldForushandeId),
                             Header.fldFunctionName, Header.fldbpn, Header.fldft, Header.fldscln, Header.fldscc, Header.fldcrn, dt, Header.fldShomareFish, Convert.ToInt64(Session["TaxUserId"]),IP);

                        saveDetail(HeaderId, Grid_DetailsArray,2);

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
                    Er = num,
                    HeaderId= HeaderId
                }, JsonRequestBehavior.AllowGet);
            }

      public void saveDetail(long HeaderId,List<prs_SelectDetailSooratHesab> Grid_DetailsArray,int state)
        {
            cartaxtest2Entities entities = new cartaxtest2Entities(); 

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
                if (hesab2.fldam != null)
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
                if (hesab2.fldnw != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x23,
                        fldValue = hesab2.fldnw.ToString()
                    };
                    list2.Add(value3);
                }

                if (hesab2.fldfee != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x24,
                        fldValue = hesab2.fldfee.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldcfee != null && hesab2.fldcfee != 0)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x25,
                        fldValue = hesab2.fldcfee.ToString()
                    };
                    list2.Add(value3);
                }
                if ((hesab2.fldcut != "0") && (hesab2.fldcut != "") && (hesab2.fldcut != null))
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x26,
                        fldValue = hesab2.fldcut
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldexr != null && hesab2.fldexr != 0)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x27,
                        fldValue = hesab2.fldexr.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldssrv != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 40,
                        fldValue = hesab2.fldssrv.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldsscv != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x29,
                        fldValue = hesab2.fldsscv.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldprdis != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x2a,
                        fldValue = hesab2.fldprdis.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.flddis != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x2b,
                        fldValue = hesab2.flddis.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldadis != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x2c,
                        fldValue = hesab2.fldadis.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldvra != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x2d,
                        fldValue = hesab2.fldvra.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldvam != null)
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
                if (hesab2.fldodr != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x30,
                        fldValue = hesab2.fldodr.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldodam != null)
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
                if (hesab2.fldolr != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x33,
                        fldValue = hesab2.fldolr.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldolam != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x34,
                        fldValue = hesab2.fldolam.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldconsfee != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x35,
                        fldValue = hesab2.fldconsfee.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldspro != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x36,
                        fldValue = hesab2.fldspro.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldbros != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x37,
                        fldValue = hesab2.fldbros.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldtcpbs != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x38,
                        fldValue = hesab2.fldtcpbs.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldcop != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x39,
                        fldValue = hesab2.fldcop.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldvop != null)
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
                if (hesab2.fldtsstam != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 60,
                        fldValue = hesab2.fldtsstam.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldpspd != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x3d,
                        fldValue = hesab2.fldpspd.ToString()
                    };
                    list2.Add(value3);
                }
                if (hesab2.fldcui != null)
                {
                    value3 = new ParamValue
                    {
                        fldParamertId = 0x3e,
                        fldValue = hesab2.fldcui.ToString()
                    };
                    list2.Add(value3);
                }
                

                System.Data.DataTable dt = new System.Data.DataTable { TableName = "movadi.tblSooratHesab_Detail" };
                using (var reader = FastMember.ObjectReader.Create(list2))
                {
                    dt.Load(reader);
                }

                if (state==1)
                entities.prs_tblSooratHesab_DetailInsert(HeaderId, dt, new long?((long)Convert.ToInt32(base.Session["TaxUserId"])),IP);
                else
                    entities.prs_tblSooratHesab_DetailUpdate(HeaderId, dt, new long?((long)Convert.ToInt32(base.Session["TaxUserId"])),IP);
            }
        }
        public ActionResult PrintBill(int Id)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;

            return PartialView;
        }
        public ActionResult GenerateRptBill(int Id)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            try
            {
                Models.cartaxtest2Entities m = new Models.cartaxtest2Entities();

                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Tax\bill.frx";
                rep.Load(path); 
                rep.SetParameterValue("fldidHeaderId", Id);
                rep.SetParameterValue("fldIdSooratHesab", Id);
                rep.SetParameterValue("connectionStr", System.Configuration.ConfigurationManager.ConnectionStrings["ComplicationsCarDBConnectionString"].ConnectionString);

                if (rep.Report.Prepare())
                {
                    // Set PDF export props
                    FastReport.Export.Pdf.PDFExport pdfExport = new FastReport.Export.Pdf.PDFExport();
                    pdfExport.ShowProgress = false;
                    pdfExport.Compressed = true;
                    pdfExport.AllowPrint = true;
                    pdfExport.EmbeddingFonts = true;


                    MemoryStream strm = new MemoryStream();
                    rep.Report.Export(pdfExport, strm);
                    rep.Dispose();
                    pdfExport.Dispose();
                    strm.Position = 0;

                    // return stream in browser
                    return File(strm.ToArray(), "application/pdf");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult ReadDetailStatus(StoreRequestParameters parameters,string HeaderId)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.prs_tblSooratHesabStatus_DetailSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.prs_tblSooratHesabStatus_DetailSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                    }
                    if (data != null)
                        data1 = p.prs_tblSooratHesabStatus_DetailSelect(field, searchtext, 100).ToList();
                    else
                        data = p.prs_tblSooratHesabStatus_DetailSelect(field, searchtext,  100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.prs_tblSooratHesabStatus_DetailSelect("fldSooratHesabStatusId", HeaderId,  100).ToList();
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

            List<Models.prs_tblSooratHesabStatus_DetailSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
