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
using JWT.Builder;
using JWT.Algorithms;


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
        private static ITaxApi CreateTaxApi(string MemoryId, string ApiUrl, string PrivateKeyPath, string CertificatePath)
        {
            try
            {
                ss = "12";
                TaxProperties taxProperties = new TaxProperties(MemoryId);
                ss = "13";
                TaxApiFactory factory3 = new TaxApiFactory(ApiUrl, taxProperties);
                ss = "14";
                ISignatory signatory = new Pkcs8SignatoryFactory().Create(PrivateKeyPath, CertificatePath);
                ss = "15";
                return factory3.CreateApi(signatory, new EncryptorFactory().Create(factory3.CreatePublicApi(signatory)));
            }
            catch (Exception x)
            {
                cartaxtest2Entities m = new cartaxtest2Entities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;

                string s = System.IO.File.ReadAllText(PrivateKeyPath);
                //System.IO.File.WriteAllText(PrivateKeyPath, "test");
                m.sp_ErrorProgramInsert(Eid, s + InnerException, 22, ss + "*" + x.Message + "_" + PrivateKeyPath + "_" + CertificatePath, DateTime.Now, "");
                return null;
            }
        }

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





        private static string PrintInquiryResult(List<InquiryResultModel> inquiryResults, long HeaderId, long UserId, string SerializeObjectErsal, long Id)
        {
            string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

            cartaxtest2Entities m = new cartaxtest2Entities();
            string fldMatn = "";
            byte num = 1;
            foreach (var result in inquiryResults)
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

                //path = "C:/privateKey.pem";
                //path2 = "C:/certificate.crt";

                prs_SelectHeaderSooratHesab hesab = entities.prs_SelectHeaderSooratHesab(new long?(HeaderId)).FirstOrDefault<prs_SelectHeaderSooratHesab>();

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

                var service = new MoadianService(certificateCrt, privateKeyPem);

                try
                {
                    // مرحله 1: دریافت Nonce
                    Console.WriteLine("دریافت Nonce...");
                    var nonce = await service.GetNonceAsync(30);

                    // مرحله 2: تولید JWT
                    Console.WriteLine("تولید JWT Token...");
                    var jwt = service.GenerateJwtToken(nonce, clientId);

                    // مرحله 3: دریافت اطلاعات سرور
                    Console.WriteLine("دریافت اطلاعات سرور...");
                    var serverInfo = await service.GetServerInfoAsync(jwt);
                    var serverKey = serverInfo.PublicKeys.FirstOrDefault(k => k.Purpose == 1);

                    // مرحله 4: ایجاد فاکتور نمونه
                    Console.WriteLine("ایجاد فاکتور...");
                    var invoice = new Invoice
                    {
                        Header = new InvoiceHeader
                        {
                            TaxId = service.GenerateTaxId(clientId, "123456", DateTime.Now),
                            Inno = "123456",
                            Indatim = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                            Inty = 1,
                            Inp = 1,
                            Ins = 1,
                            Tins = hesab.fldBid,
                            Tob = 2,
                            Bid = hesab.fldkh_Bid,
                            Tinb = hesab.fldkh_Bid,
                            Tbill = 115000,
                            Setm = 1
                        },
                        Body = new List<InvoiceBody>
                    {
                        new InvoiceBody
                        {
                            Sstid = "2330004115904",
                            Sstt = "کالای نمونه",
                            Mu = "164",
                            Am = 1,
                            Fee = 100000,
                            Vam = 9000,
                            Tsstam = 109000
                        }
                    },
                        Payments = new List<object>()
                    };

                    Console.WriteLine("امضای فاکتور...");
                    var signedInvoice = service.SignInvoice(invoice);

                    // مرحله 6: رمزگذاری فاکتور
                    Console.WriteLine("رمزگذاری فاکتور...");
                    var encryptedInvoice = service.EncryptSignedInvoice(signedInvoice, serverKey.Key, serverKey.Id);

                    // مرحله 7: آماده‌سازی بسته ارسال
                    var invoicePacket = new Avarez.Areas.Tax.Models.InvoicePacket
                    {
                        Header = new PacketHeader
                        {
                            RequestTraceId = Guid.NewGuid().ToString(),
                            FiscalId = clientId
                        },
                        Payload = encryptedInvoice
                    };

                    // مرحله 8: تولید JWT جدید برای ارسال
                    var newNonce = await service.GetNonceAsync(30);
                    var newJwt = service.GenerateJwtToken(newNonce, clientId);

                    // مرحله 9: ارسال فاکتور
                    Console.WriteLine("ارسال فاکتور...");
                    var result = await service.SendInvoiceAsync(new List<Avarez.Areas.Tax.Models.InvoicePacket> { invoicePacket }, newJwt);

                    Console.WriteLine($"فاکتور با موفقیت ارسال شد:");
                    Console.WriteLine($"شماره پیگیری: {result.Result.FirstOrDefault()?.ReferenceNumber}");
                    Console.WriteLine($"شناسه درخواست: {result.Result.FirstOrDefault()?.Uid}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"خطا در ارسال فاکتور: {ex.Message}");
                }
              

                

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

        public ActionResult SamaneMoadianAsli(long HeaderId)
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
                                    ITaxApi taxApi = CreateTaxApi(fldUniqId, "https://tp.tax.gov.ir/requestsmanager", path, path2);
                                ss = "2";
                                    InvoiceDto item = CreateValidInvoice(fldUniqId, HeaderId);
                                    List <InvoiceDto> list1 = new List<InvoiceDto>();
                                    list1.Add(item);
                                    List<InvoiceDto> invoiceList = list1;

                              

                ss = "3";
                try
                {

                    List<InvoiceResponseModel> responseModels = taxApi.SendInvoices(invoiceList);
                }
                catch (Exception x)
                {
                    string InnerException = "";
                    if (x.InnerException != null)
                        InnerException = x.InnerException.Message;
                  //  m.sp_ErrorProgramInsert(Eid, ss + InnerException, Convert.ToInt32(Session["TaxUserId"]), x.Message, DateTime.Now, "");

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
                        Msg = "خطایی با شماره: " ,
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }

            

                Thread.Sleep(10_000);
                ss = "4";
                //InquiryByReferenceNumberDto inquiryDto = new InquiryByReferenceNumberDto(responseModels.Select(r => r.ReferenceNumber).ToList());
                // List<InquiryResultModel> inquiryResults = taxApi.InquiryByReferenceId(inquiryDto);
                // string SerializeObjectErsal = Newtonsoft.Json.JsonConvert.SerializeObject(invoiceList);
                // ss = "5";
                // ss = inquiryResults[0].Status+"**"+ inquiryResults[0].ReferenceNumber + "**" + inquiryResults[0].Uid;

                // if (inquiryResults[0].Status == "IN_PROGRESS" || inquiryResults[0].Status == "NOT_FOUND")
                // {
                //     var uid = "";
                //     if (inquiryResults[0].Uid != null)
                //         uid = inquiryResults[0].Uid;
                //     entities.prs_tblSooratHesabStatusInsert(new long?((long)HeaderId), 4, inquiryResults[0].Status, inquiryResults[0].ReferenceNumber, SerializeObjectErsal, uid, Convert.ToInt64(Session["TaxUserId"]),IP);
                //     return base.Json(new
                //     {
                //         Msg = "عدم دریافت پاسخ از سامانه مودیان.("+ inquiryResults[0].ReferenceNumber + "**" + inquiryResults[0].Uid+ ")",
                //         MsgTitle = "خطا",
                //         Er = 1
                //     }, JsonRequestBehavior.AllowGet);
                // }

                // var mmsgg=PrintInquiryResult(inquiryResults, HeaderId,Convert.ToInt64(Session["TaxUserId"]), SerializeObjectErsal,0);

                var mmsgg = "";
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
        public ActionResult EstelamSamaneMoadian(long HeaderId)
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

                var st = entities.prs_tblSooratHesabStatusSelect("fldHeaderId", HeaderId.ToString(), 0).FirstOrDefault();
                var so = entities.prs_tblSooratHesab_HeaderSelect("fldId", HeaderId.ToString(),"","", 0).FirstOrDefault();

                if (st==null)
                {
                    return base.Json(new
                    {
                        Msg = "صورتحساب موردنظر ارسال نشده.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }



                ss = "e1";
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                prs_User_GharardadSelect select = entities.prs_User_GharardadSelect("fldID", base.Session["TaxUserId"].ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
                prs_tblTarfGharardadSelect select2 = entities.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>();
                path = AppDomain.CurrentDomain.BaseDirectory + "Uploaded\\privateKey" + select2.fldId.ToString() + ".pem";
                path2 = AppDomain.CurrentDomain.BaseDirectory + "Uploaded\\certificate" + select2.fldId.ToString() + ".crt";

                ss = "e2";
                //path = "C:/privateKey.pem";
                //path2 = "C:/certificate.crt";

                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.WriteAllBytes(path, select2.fldPrivateKey.ToArray<byte>());
                }
                if (!System.IO.File.Exists(path2))
                {
                    System.IO.File.WriteAllBytes(path2, select2.fldSignatureCertificate.ToArray<byte>());
                }
                string fldUniqId = select2.fldUniqId;



                ITaxApi taxApi = CreateTaxApi(fldUniqId, "https://tp.tax.gov.ir/requestsmanager", path, path2);

                ss = "e3";
                DateTime startDate = MyLib.Shamsi.Shamsi2miladiDateTime(MyLib.Shamsi.ShamsiAddDay(so.fldIndatim, -1));
                DateTime endDate = MyLib.Shamsi.Shamsi2miladiDateTime(MyLib.Shamsi.ShamsiAddDay(so.fldIndatim, 0));

                /*DateTime startDate = MyLib.Shamsi.Shamsi2miladiDateTime(MyLib.Shamsi.ShamsiAddDay(p.sp_GetDate().FirstOrDefault().DateShamsi, -2));
                DateTime endDate = MyLib.Shamsi.Shamsi2miladiDateTime(MyLib.Shamsi.ShamsiAddDay(p.sp_GetDate().FirstOrDefault().DateShamsi, 1));*/

                //DateTime startDate = DateTime.Now.AddDays(-1).ToLocalTime();
                //DateTime endDate = DateTime.Now.ToLocalTime();
                //Thread.Sleep(10_000);


                ss = "e4";
                List<String> referenceNumbers = new List<String>();
                referenceNumbers.Add(st.fldReferenceNumber);
                InquiryByReferenceNumberDto inquiryDto = new InquiryByReferenceNumberDto(referenceNumbers, startDate, endDate);
                List<InquiryResultModel> inquiryResults = taxApi.InquiryByReferenceId(inquiryDto);

               /*List<String> referenceNumbers = new List<String>();
                referenceNumbers.Add(st.fldUid);
                InquiryByUidDto inquiryDto = new InquiryByUidDto(referenceNumbers, fldUniqId, startDate, endDate);
                List<InquiryResultModel> inquiryResults = taxApi.InquiryByUid(inquiryDto);*/

                ss = "e5";
                string SerializeObjectErsal = st.fldSerializeObject;

                if (inquiryResults.Count == 0)
                {
                    return base.Json(new
                    {
                        Msg = "عدم دریافت پاسخ از سامانه مودیان.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                ss = inquiryResults[0].Status + "**" + inquiryResults[0].ReferenceNumber + "**" + inquiryResults[0].Uid;

                if (inquiryResults[0].Status == "IN_PROGRESS" || inquiryResults[0].Status == "NOT_FOUND")
                {
                    return base.Json(new
                    {
                        Msg = "عدم دریافت پاسخ از سامانه مودیان.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }

                ss = "e6";
                string mmsgg = PrintInquiryResult(inquiryResults, HeaderId, Convert.ToInt64(Session["TaxUserId"]), SerializeObjectErsal,so.fldId);

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
