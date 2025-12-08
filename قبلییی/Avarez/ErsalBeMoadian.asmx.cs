using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Avarez.Areas.Tax.Models;
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
using FastMember;
using System.Threading.Tasks;
using MyLib;
using Ext.Net;
using Ext.Net.MVC;

namespace Avarez
{
    /// <summary>
    /// Summary description for ErsalBeMoadian
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ErsalBeMoadian : System.Web.Services.WebService
    {
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        [WebMethod]
        public string SendToMoadian(string UserRasa,String PassRasa, ObjHeaderSooratHesab Header, List<ObjDetailSooratHesab> Grid_DetailsArray)
        {
            Avarez.Areas.Tax.Models.cartaxtest2Entities p = new Avarez.Areas.Tax.Models.cartaxtest2Entities();
            var qq = p.prs_User_GharardadSelect("cheakPass", UserRasa, 1, CodeDecode.GenerateHash(PassRasa), 1, "").FirstOrDefault();
            long UserIdTax = 0;
            long ShakhsIdTax = 0;
                string Msg="";
            long HeaderId = 0;
            if (qq != null)
            {
                if (qq.fldTarfGharardadId != null)
                {
                    UserIdTax = qq.fldID;
                    ShakhsIdTax = qq.fldShakhsId;
                    HeaderId=SaveData(Header, Grid_DetailsArray, ShakhsIdTax, UserIdTax, "WebService");
                    if (HeaderId != 0)
                        SamaneMoadian(HeaderId, UserIdTax);
                    else
                        Msg = "در ذخیره اطلاعات خطایی رخ داده است.";
                }
                else
                {
                    Msg = "شما مجاز به ورود به سامانه مودیان نمی باشید.";
                }
            }
            return Msg;
        }
        [WebMethod]
        public string SendToMoadianWithId(long UserIdTax, long ShakhsIdTax, ObjHeaderSooratHesab Header, List<ObjDetailSooratHesab> Grid_DetailsArray)
        {
            Avarez.Areas.Tax.Models.cartaxtest2Entities p = new Avarez.Areas.Tax.Models.cartaxtest2Entities();
          
            string Msg = "";
            long HeaderId = 0;
         
                    HeaderId = SaveData(Header, Grid_DetailsArray, ShakhsIdTax, UserIdTax, "WebService");
            if (HeaderId != 0)
                SamaneMoadian(HeaderId, UserIdTax);
            else
                Msg = "در ذخیره اطلاعات خطایی رخ داده است.";

            return Msg;
        }
        private static string GenerateTaxId(long serial, DateTime now, string MemoryId)
        {
            TaxIdProvider taxIdProvider = new TaxIdProvider(new VerhoeffAlgorithm());
            return taxIdProvider.GenerateTaxId(MemoryId, serial, now);
        }
        public long SaveData(ObjHeaderSooratHesab Header, List<ObjDetailSooratHesab> Grid_DetailsArray, long ShakhsIdTax, long UserIdTax, string FuncName)
        {
            string str = "ذخیره با موفقیت انجام شد.";
            string str2 = "عملیات موفق";
            int num = 0;
            long HeaderId = 0;

            str = "";
            str2 = "";
            num = 0;
            try
            {
                ParamValue value2;
          
                decimal? fldtonw;
                int? nullable18;
                int? nullable36;
                int? nullable37;
                int? nullable38;
                cartaxtest2Entities entities = new cartaxtest2Entities();

                prs_User_GharardadSelect select = entities.prs_User_GharardadSelect("fldID", UserIdTax.ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
                Random random = new Random();
                long serial = random.Next(1_000_000_000);
                string TaxId = GenerateTaxId(serial, Header.indatim, entities.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
                string Inno = serial.ToString("X").PadLeft(10, '0');

                DateTime time = Header.indatim;

                long? tprdis = 0;
                long? tadis = 0;
                long? tvam = 0;
                long? todam = 0;
                long? tdis = 0;
                long? tbill = 0;
                decimal? tonw = 0;
                long? torv = 0;
                decimal? tocv = 0;
                long? tvop = 0;
                foreach (var hesab in Grid_DetailsArray)
                {
                    long? fldvam = Convert.ToInt64(hesab.vra * (Convert.ToInt64(hesab.am * hesab.fee) - hesab.dis) / 100);
                    long? fldodam = Convert.ToInt64(hesab.odr * (Convert.ToInt64(hesab.am * hesab.fee) - hesab.dis) / 100);
                    long? fldadis = Convert.ToInt64(hesab.am * hesab.fee) - hesab.dis;
                    long? fldprdis = Convert.ToInt64(hesab.am * hesab.fee);
                    long? fldolam = Convert.ToInt64(hesab.olr * (Convert.ToInt64(hesab.am * hesab.fee) - hesab.dis) / 100);
                    long? fldtsstam = fldadis + fldodam + fldolam + fldvam;

                    hesab.vam = fldvam;
                    hesab.odam = fldodam;
                    hesab.adis = fldadis;
                    hesab.prdis = fldprdis;
                    hesab.olam = fldolam;
                    hesab.tsstam = fldtsstam;

                    if (hesab.prdis != null)
                    {
                        tprdis = tprdis + hesab.prdis;
                    }
                    if (hesab.dis != null)
                    {
                        tdis = tdis + hesab.dis;
                    }
                    if (hesab.adis != null)
                    {
                        tadis = tadis + hesab.adis;
                    }
                    if (hesab.vam != null)
                    {
                        tvam = tvam + hesab.vam;
                    }
                    if (hesab.odam != null)
                    {
                        todam = todam + hesab.odam;
                    }
                    if (hesab.tsstam != null)
                    {
                        tbill = tbill + hesab.tsstam;
                    }
                    if (hesab.nw != null)
                    {
                        tonw = tonw + hesab.nw;
                    }
                    if (hesab.ssrv != null)
                    {
                        torv = torv + hesab.ssrv;
                    }
                    if (hesab.sscv != null)
                    {
                        tocv = tocv + hesab.sscv;
                    }
                    if (hesab.vop != null)
                    {
                        tvop = tvop + hesab.vop;
                    }
                }
                List<ParamValue> source = new List<ParamValue>();
                Header.tvop = tvop;
                Header.tocv = tocv;
                Header.torv = torv;
                Header.tonw = tonw;
                Header.tbill = tbill;
                Header.tdis = tdis;
                //Header.fldtodam = todam;
                //Header.fldtvam = tvam;
                //Header.fldtadis = tadis;
                //Header.fldtprdis = tprdis;
                if (Header.setm == 1 && (Header.cap==null|| Header.cap==0))
                    Header.cap = tbill;
                if (Header.setm == 3) {
                    if((Header.cap == null || Header.cap == 0)&& !(Header.insp == null || Header.insp == 0))
                    Header.cap = tbill - Header.insp;
                    if ((Header.insp == null || Header.insp == 0)&& (Header.cap == null || Header.cap == 0))
                        Header.insp = tbill - Header.cap;
                }
                if (Header.setm == 2 && (Header.insp == null || Header.insp == 0))
                    Header.insp = tbill;

                byte? fldins = 0;
                if ((Header.ins != null))
                    fldins = Header.ins;
                   
                        value2 = new ParamValue
                        {
                            fldParamertId = 1,
                            fldValue = fldins.ToString()
                        };
                        source.Add(value2);



                long? fldtprdis = 0;
                if ((Header.tprdis != null))
                    fldtprdis = Header.tprdis;
                    
                        value2 = new ParamValue
                        {
                            fldParamertId = 0x11,
                            fldValue = Header.tprdis.ToString()
                        };
                        source.Add(value2);

                long? fldtadis = 0;
                if ((Header.tadis != null))
                    fldtadis = Header.tadis;
                    
                        value2 = new ParamValue
                        {
                            fldParamertId = 0x13,
                            fldValue = fldtadis.ToString()
                        };
                        source.Add(value2);

                long? fldtdis = 0;
                if ((Header.tadis != null))
                    fldtdis = Header.tdis;

                value2 = new ParamValue
                {
                    fldParamertId = 0x12,
                    fldValue = fldtdis.ToString()
                };
                source.Add(value2);

                long? fldtvam = 0;
                if ((Header.tvam != null))
                    fldtvam = Header.tvam;
                    
                        value2 = new ParamValue
                        {
                            fldParamertId = 20,
                            fldValue = fldtvam.ToString()
                        };
                        source.Add(value2);

                long? fldtodam = 0;
                if ((Header.todam != null))
                    fldtodam = Header.todam;
                        value2 = new ParamValue
                        {
                            fldParamertId = 0x15,
                            fldValue = Header.todam.ToString()
                        };
                        source.Add(value2);
                    
                long? fldtbill =fldtvam + fldtodam + fldtadis;
               
                if ((fldtbill != null))
                {
                    value2 = new ParamValue
                        {
                            fldParamertId = 0x16, //tbill
                            fldValue = fldtbill.ToString()
                        };
                        source.Add(value2);
                }

                byte? fldsetm = 0;
                if ((Header.setm != null))
                    fldsetm = Header.setm;
                    
                        value2 = new ParamValue
                        {
                            fldParamertId = 0x1a,
                            fldValue = fldsetm.ToString()
                        };
                        source.Add(value2);

                if ((Header.cap != null && Header.cap !=0))
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 27,
                        fldValue = Header.cap.ToString()
                    };
                    source.Add(value2);
                }

                if ((Header.insp != null && Header.insp !=0))
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 28,
                        fldValue = Header.insp.ToString()
                    };
                    source.Add(value2);
                }


                DataTable table1 = new DataTable();
                    table1.TableName = "movadi.tblSooratHesabHeader_Value";
                    DataTable table = table1;
                    using (ObjectReader reader = ObjectReader.Create<ParamValue>(source.ToList<ParamValue>(), Array.Empty<string>()))
                    {
                        table.Load(reader);
                    }
                    if(FuncName== "WebService")
                HeaderId = entities.prs_tblSooratHesabFromWebServiceInsert(new ObjectParameter("fldid", typeof(long)),ShakhsIdTax, Header.Kh_Name, Header.Kh_Family,Header.bid,Header.tob,
                        Header.tinb,TaxId, Header.indatim, Header.indati2m, 1, 1, Inno, Header.irtaxid,
                        FuncName, table,UserIdTax,"");
                if (FuncName == "Excel")
                    HeaderId = entities.prs_InsertSooratHesabForExcel(new ObjectParameter("fldid", typeof(long)), ShakhsIdTax, Header.Kh_Name, Header.Kh_Family, Header.bid, Header.tob,
                            Header.tinb, TaxId, Header.indatim, Header.indati2m, 1, 1, Inno, Header.irtaxid,
                            FuncName,Header.FishId, table, UserIdTax, "",IP);

                if (HeaderId != 0)
                {
                    foreach (var hesab2 in Grid_DetailsArray)
                    {
                        ParamValue value3;
                        List<ParamValue> list2 = new List<ParamValue>();

                        string fldsstid = "";
                        if (hesab2.sstid != null)
                            fldsstid = hesab2.sstid;
                        value3 = new ParamValue
                        {
                            fldParamertId = 31,
                            fldValue = fldsstid.ToString()
                        };
                        list2.Add(value3);

                        string fldsstt = "";
                        if (hesab2.sstt != null)
                            fldsstt = hesab2.sstt;
                        value3 = new ParamValue
                        {
                            fldParamertId = 32,
                            fldValue = fldsstt.ToString()
                        };
                        list2.Add(value3);

                        decimal? fldam = 0;
                        if (hesab2.am != null)
                            fldam = hesab2.am;
                        value3 = new ParamValue
                            {
                                fldParamertId = 0x21,
                                fldValue = fldam.ToString()
                            };
                            list2.Add(value3);
                        
                        decimal? fldfee = 0;
                        if (hesab2.fee != null)
                            fldfee = hesab2.fee;
                                value3 = new ParamValue
                            {
                                fldParamertId = 0x24,
                                fldValue = fldfee.ToString()
                            };
                            list2.Add(value3);
                        
                        long? fldprdis = Convert.ToInt64(fldam * fldfee);
                        fldtprdis = fldprdis;
                        if (fldprdis != null)
                        {
                            value3 = new ParamValue
                            {
                                fldParamertId = 0x2a,//prdis
                                fldValue = fldprdis.ToString()
                            };
                            list2.Add(value3);
                        }
                        long? flddis = 0;
                        if (hesab2.dis != null)
                            flddis=hesab2.dis;
                        
                            value3 = new ParamValue
                            {
                                fldParamertId = 0x2b,
                                fldValue = flddis.ToString()
                            };
                            list2.Add(value3);
                        
                        long? fldadis = Convert.ToInt64(fldam * fldfee) - flddis;
                        if (fldadis != null)
                        {
                            value3 = new ParamValue
                            {
                                fldParamertId = 0x2c,//adis
                                fldValue = fldadis.ToString()
                            };
                            list2.Add(value3);
                        }
                        decimal fldvra = 0;
                        long? fldvam = 0;
                        if (hesab2.vra != null)
                        {
                            fldvra = Convert.ToDecimal(hesab2.vra);

                            value3 = new ParamValue
                            {
                                fldParamertId = 0x2d,
                                fldValue = fldvra.ToString()
                            };
                            list2.Add(value3);

                             fldvam = Convert.ToInt64(Math.Floor(Convert.ToDecimal(fldvra * (Convert.ToInt64(Convert.ToDecimal(fldam) * Convert.ToDecimal(fldfee)) - flddis) / 100)));
                            value3 = new ParamValue
                            {
                                fldParamertId = 0x2e,//vam
                                fldValue = fldvam.ToString()
                            };
                            list2.Add(value3);

                        }
                        long? fldtsstam = fldvam + fldadis;

                        if ((hesab2.odt != "") && (hesab2.odt != null))
                        {
                            value3 = new ParamValue
                            {
                                fldParamertId = 0x2f,
                                fldValue = hesab2.odt
                            };
                            list2.Add(value3);
                        }
                        if (hesab2.odr != null)
                        {
                            value3 = new ParamValue
                            {
                                fldParamertId = 0x30,
                                fldValue = hesab2.odr.ToString()
                            };
                            list2.Add(value3);

                            long? fldodam = Convert.ToInt64(hesab2.odr * (Convert.ToInt64(hesab2.am * hesab2.fee) - hesab2.dis) / 100);
                          
                            if (fldodam != null)
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x31,//odam
                                    fldValue = fldodam.ToString()
                                };
                                list2.Add(value3);
                            }

                            fldtsstam = fldtsstam + fldodam;
                        }
                        if ((hesab2.olt != "") && (hesab2.olt != null))
                        {
                            value3 = new ParamValue
                            {
                                fldParamertId = 50,
                                fldValue = hesab2.olt
                            };
                            list2.Add(value3);
                        }
                        if (hesab2.olr != null)
                        {
                            value3 = new ParamValue
                            {
                                fldParamertId = 0x33,
                                fldValue = hesab2.olr.ToString()
                            };
                            list2.Add(value3);

                            long? fldolam = Convert.ToInt64(hesab2.olr * (Convert.ToInt64(hesab2.am * hesab2.fee) - hesab2.dis) / 100);
                          
                            if (fldolam != null)
                            {
                                value3 = new ParamValue
                                {
                                    fldParamertId = 0x34,
                                    fldValue = fldolam.ToString()
                                };
                                list2.Add(value3);
                            }
                            fldtsstam = fldtsstam + fldolam;
                        }

                        if (fldtsstam != null)
                        {
                            value3 = new ParamValue
                            {
                                fldParamertId = 60,
                                fldValue = fldtsstam.ToString()
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
                        entities.prs_tblSooratHesab_DetailInsert(new long?(HeaderId), table2, UserIdTax,IP);
                    }

                    var AllDetail=entities.prs_SelectDetailSooratHesab(HeaderId).ToList();

                    if (AllDetail.Count > 1)
                    {
                        tdis = 0;
                        tbill = 0;
                        tonw = 0;
                        torv = 0;
                        tocv = 0;
                        tvop = 0;
                        foreach (var hesab in AllDetail)
                        {
                            if (hesab.flddis != null)
                            {
                                tdis = tdis + hesab.flddis;
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
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, tdis.ToString(), 18, UserIdTax,IP);
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, tbill.ToString(), 22, UserIdTax, IP);
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, tonw.ToString(), 23, UserIdTax, IP);
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, torv.ToString(), 24, UserIdTax, IP);
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, tocv.ToString(), 25, UserIdTax, IP);
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, tvop.ToString(), 29, UserIdTax, IP);
                    }
                }
                }
                catch (Exception exception)
                {
                    str = (exception.InnerException == null) ? exception.Message : exception.InnerException.Message;
                    str2 = "خطا";
                    num = 1;
                }
           
            return HeaderId;
        }
        public/*async Task<*/string/*>*/ SamaneMoadian(long HeaderId, long UserIdTax)
        {
            string path = "";
            string str2 = "";
            try
            {
                cartaxtest2Entities entities = new cartaxtest2Entities();

                var user = entities.prs_User_GharardadSelect("fldId", Session["TaxUserId"].ToString(), 1, "", 1, "").FirstOrDefault();
                var TransactionInf = entities.prs_TransactionInfSelect("fldTarfGharardadId", user.fldTarfGharardadId.ToString(), 0).FirstOrDefault();
                Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
                var divName = "جمهوری اسلامی ایران";
                bool Tr = h.Transaction(TransactionInf.fldUserName, TransactionInf.fldPass, 0/*(int)TransactionInf.CountryType*/, divName);
                if (Tr != true)
                {
                    return "اعتبار شما پایان یافته است.";
                }

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

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

                var msgtitle = "ارسال موفق";
                var msg = "ارسال با موفقیت انجام شد.";

                //await System.Threading.Tasks.Task.Run(() =>
                //{
                   // ct.ThrowIfCancellationRequested();
                    InvoiceDto item = CreateValidInvoice(fldUniqId, HeaderId);
                    List<InvoiceDto> list1 = new List<InvoiceDto>();
                    list1.Add(item);
                    List<InvoiceDto> invoiceList = list1;

                    List<InvoiceResponseModel> responseModels = taxApi.SendInvoices(invoiceList);
                    Thread.Sleep(10_000);
                    InquiryByReferenceNumberDto inquiryDto = new InquiryByReferenceNumberDto(responseModels.Select(r => r.ReferenceNumber).ToList());
                    List<InquiryResultModel> inquiryResults = taxApi.InquiryByReferenceId(inquiryDto);
                    string SerializeObjectErsal = Newtonsoft.Json.JsonConvert.SerializeObject(invoiceList);

                if (inquiryResults[0].Status == "IN_PROGRESS" || inquiryResults[0].Status == "NOT_FOUND")
                    {

                    var uid = "";
                    if (inquiryResults[0].Uid != null)
                        uid = inquiryResults[0].Uid;
                    entities.prs_tblSooratHesabStatusInsert(HeaderId, 4, inquiryResults[0].Status, inquiryResults[0].ReferenceNumber, SerializeObjectErsal, uid, Convert.ToInt64(Session["TaxUserId"]), IP);
                    msg ="عدم دریافت پاسخ از سامانه مودیان.(" + inquiryResults[0].ReferenceNumber + "**" + inquiryResults[0].Uid + ")";

                    }
                    else
                    {
                        string mmsgg = PrintInquiryResult(inquiryResults, HeaderId, Convert.ToInt64(Session["TaxUserId"]), SerializeObjectErsal);


                        if (mmsgg.Split(';')[0] != "1")
                            msg = mmsgg.Split(';')[1];
                        //else
                        //{
                        //    var TransactionInf = entities.prs_TransactionInfSelect("fldTarfGharardadId", Session["TarafGharardadId"].ToString(), 0).FirstOrDefault();
                        //     Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
                        //    var divName  = "جمهوری اسلامی ایران";
                        //    bool Tr = h.Transaction(TransactionInf.fldUserName, TransactionInf.fldPass, 0/*(int)TransactionInf.CountryType*/, divName);
                        //    if (Tr != true)
                        //    {
                        //        msg = "ارسال با موفقیت انجام شد."+")اعتبار شما پایان یافته است)";
                        //    }
                        //}

                        if (mmsgg.Split(';')[0] == "2")
                        {
                            msgtitle = "هشدار";
                            msg = "هشدار در هنگام ارسال.";
                        }
                        if (mmsgg.Split(';')[0] == "3")
                        {
                            msgtitle = "خطا";
                            msg = "خطا در ارسال.";
                        }
                    }
                //});
                //System.IO.File.Delete(path);
                //System.IO.File.Delete(str2);


                return msg;
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
                m.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["TaxUserId"]), x.Message, DateTime.Now,"");

                //X.Msg.Show(new MessageBoxConfig
                //{
                //    Buttons = MessageBox.Button.OK,
                //    Icon = MessageBox.Icon.ERROR,
                //    Title = "خطا",
                //    Message = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید."
                //});
                //X.Mask.Hide();
                //DirectResult result = new DirectResult();
                return "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.";
            }
        }
        private static ITaxApi CreateTaxApi(string MemoryId, string ApiUrl, string PrivateKeyPath, string CertificatePath)
        {
            TaxProperties taxProperties = new TaxProperties(MemoryId);
            TaxApiFactory factory3 = new TaxApiFactory(ApiUrl, taxProperties);
            ISignatory signatory = new Pkcs8SignatoryFactory().Create(PrivateKeyPath, CertificatePath);
            return factory3.CreateApi(signatory, new EncryptorFactory().Create(factory3.CreatePublicApi(signatory)));
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
                    pspd = item.fldpspd,
                    sscv = item.fldsscv
                };

                bodylist.Add(bd);
            }


            long? indati2m = null;
            if (hesab.fldIndati2m != null)
                indati2m = new DateTimeOffset(Convert.ToDateTime(hesab.fldIndati2m)).ToUnixTimeMilliseconds();
            
            string Tinbb = null;
            if (hesab.fldKh_Tob == 2)
                Tinbb = hesab.fldkh_Bid;

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
                    ins = hesab.fldins ,
                    tins = hesab.fldBid,
                    tinb = Tinbb,
                    tob = Convert.ToInt32(hesab.fldKh_Tob),
                    tprdis = (hesab.fldtprdis),
                    tdis = (hesab.fldtdis),
                    tadis = (hesab.fldtadis),
                    tvam = (hesab.fldtvam),
                    todam = (hesab.fldtodam),
                    tbill = (hesab.fldtbill),
                    setm = hesab.fldsetm,
                    irtaxid = hesab.fldIrtaxId == "" ? null : hesab.fldIrtaxId,
                    bid = hesab.fldBid,
                    sbc = hesab.fldSbc == "" ? null : hesab.fldSbc,
                    bpc = hesab.fldBpc == "" ? null : hesab.fldBpc,
                    bbc = hesab.fldbbc == "" ? null : hesab.fldbbc,
                    ft = hesab.fldft,
                    bpn = hesab.fldbpn,
                    scln = hesab.fldscln,
                    scc = hesab.fldscc,
                    cdcn = hesab.fldcdcn,
                    cdcd = null,//Convert.ToInt32(hesab.fldcdcd),
                    crn = hesab.fldcrn,
                    billid = hesab.fldbilid,
                    tonw = (hesab.fldtonw),
                    tocv = (hesab.fldtocv),
                    torv = (hesab.fldtorv),
                    insp = (hesab.fldinsp),
                    cap = (hesab.fldcap),
                    tvop = (hesab.fldtvop),
                    tax17 = (hesab.fldtax17)
                },
                Body = bodylist
            };
            return invoice;
        }
        private static string PrintInquiryResult(List<InquiryResultModel> inquiryResults, long HeaderId, long UserId, string SerializeObjectErsal)
        {
            string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

            cartaxtest2Entities m = new cartaxtest2Entities();
            string fldMatn = "";
            byte num = 1;
            foreach (var result in inquiryResults)
            {
                fldMatn = "Status = " + result.Status;
                var errors = result.Data.Error;

                if (errors.Count() >0)
                {
                    num = 3; 
                }
                List<InvoiceErrorModel> list2 = result.Data.Warning;
                if (list2.Count() > 0)
                {
                    if (errors.Count()== 0)
                        num = 2;
                }
                var ss = m.prs_tblSooratHesabStatusInsert(new long?((long)HeaderId), new byte?(num), result.Status, result.ReferenceNumber, SerializeObjectErsal, result.Uid, new long?(UserId), IP).FirstOrDefault();


                foreach (var error in errors)
                {
                    string code = error.Code;
                    string message = error.Message;
                    string[] textArray1 = new string[] { fldMatn, "*** Code: ", code, ", Message: ", message };
                    fldMatn = string.Concat(textArray1);

                    m.prs_tblSooratHesabStatus_DetailInsert(ss.fldId, 3, message, code, UserId, IP);
                }

                
                foreach (InvoiceErrorModel model3 in list2)
                {
                    string code = model3.Code;
                    string message = model3.Message;
                    string[] textArray2 = new string[] { fldMatn, "***  Code: ", code, ", Message: ", message };
                    fldMatn = string.Concat(textArray2);

                    m.prs_tblSooratHesabStatus_DetailInsert(ss.fldId, 2, message, code, UserId, IP);
                }

            }
            return (num.ToString() + ";" + fldMatn);
        }
    }
}
