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
        public string SendToMoadian(string UserRasa, String PassRasa, ObjHeaderSooratHesab Header, List<ObjDetailSooratHesab> Grid_DetailsArray)
        {
            Avarez.Areas.Tax.Models.cartaxtest2Entities p = new Avarez.Areas.Tax.Models.cartaxtest2Entities();
            var qq = p.prs_User_GharardadSelect("cheakPass", UserRasa, 1, CodeDecode.GenerateHash(PassRasa), 1, "").FirstOrDefault();
            long UserIdTax = 0;
            long ShakhsIdTax = 0;
            string Msg = "";
            long HeaderId = 0;
            if (qq != null)
            {
                if (qq.fldTarfGharardadId != null)
                {
                    UserIdTax = qq.fldID;
                    ShakhsIdTax = qq.fldShakhsId;
                    HeaderId = SaveData(Header, Grid_DetailsArray, ShakhsIdTax, UserIdTax, "WebService");
                    if (HeaderId > 0)
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
        public string SendToMoadianJustSave(string UserRasa, String PassRasa, ObjHeaderSooratHesab Header, List<ObjDetailSooratHesab> Grid_DetailsArray)
        {
            Avarez.Areas.Tax.Models.cartaxtest2Entities p = new Avarez.Areas.Tax.Models.cartaxtest2Entities();
            var qq = p.prs_User_GharardadSelect("cheakPass", UserRasa, 1, CodeDecode.GenerateHash(PassRasa), 1, "").FirstOrDefault();
            long UserIdTax = 0;
            long ShakhsIdTax = 0;
            string Msg = "";
            long HeaderId = 0;
            if (qq != null)
            {
                if (qq.fldTarfGharardadId != null)
                {
                    UserIdTax = qq.fldID;
                    ShakhsIdTax = qq.fldShakhsId;
                    HeaderId = SaveData(Header, Grid_DetailsArray, ShakhsIdTax, UserIdTax, "WebService");
                    if (HeaderId > 0)
                        Msg = "فاکتور شماره ."+ HeaderId + "در سامانه مودیان ذخیره شد";
                    //  SamaneMoadian(HeaderId, UserIdTax);
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
            if (HeaderId > 0)
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

                cartaxtest2Entities entities = new cartaxtest2Entities();
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

                prs_User_GharardadSelect select = entities.prs_User_GharardadSelect("fldID", UserIdTax.ToString(), 0, "", 1, "").FirstOrDefault<prs_User_GharardadSelect>();
                Random random = new Random();
                long serial = random.Next(1_000_000_000);
                string TaxId = GenerateTaxId(serial, Header.indatim, entities.prs_tblTarfGharardadSelect("fldId", select.fldTarfGharardadId.ToString(), 0).FirstOrDefault<prs_tblTarfGharardadSelect>().fldUniqId);
                string Inno = serial.ToString("X").PadLeft(10, '0');

                DateTime time = Header.indatim;

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
                Header.todam = todam;
                Header.tvam = tvam;
                Header.tadis = tadis;
                Header.tdis = tdis;
                Header.tprdis = tprdis;

                byte? fldins = 0;
                if ((Header.ins != null))
                    fldins = Header.ins;

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
                if ((Header.cdcn != "") && (Header.cdcn != null))
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 13,
                        fldValue = Header.cdcn
                    };
                    source.Add(value2);
                }

                if (Header.cdcd != null)
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 14,
                        fldValue = Header.cdcd.ToString()
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
                if ((Header.bilid != "") && (Header.bilid != null))
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 0x10,
                        fldValue = Header.bilid
                    };
                    source.Add(value2);
                }
                long? fldtprdis = 0;
                if ((Header.tprdis != null))
                    fldtprdis = Header.tprdis;

                value2 = new ParamValue
                {
                    fldParamertId = 0x11,
                    fldValue = fldtprdis.ToString()
                };
                source.Add(value2);

                long? fldtdis = 0;
                if ((Header.tdis != null))
                    fldtdis = Header.tdis;

                value2 = new ParamValue
                {
                    fldParamertId = 0x12,
                    fldValue = fldtdis.ToString()
                };

                long? fldtadis = 0;
                if ((Header.tadis != null))
                    fldtadis = Header.tadis;

                value2 = new ParamValue
                {
                    fldParamertId = 0x13,
                    fldValue = fldtadis.ToString()
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

                if (Header.tonw != null && Header.tonw != 0)
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 0x17,
                        fldValue = Header.tonw.ToString()
                    };
                    source.Add(value2);
                }

                if (Header.torv != null && Header.torv != 0)
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 0x18,
                        fldValue = Header.torv.ToString()
                    };
                    source.Add(value2);
                }

                if (Header.tocv != null && Header.tocv != 0)
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 0x19,
                        fldValue = Header.tocv.ToString()
                    };
                    source.Add(value2);
                }

                byte? fldsetm = 0;
                if ((Header.setm != null))
                    fldsetm = Header.setm;

                value2 = new ParamValue
                {
                    fldParamertId = 26,
                    fldValue = fldsetm.ToString()
                };
                source.Add(value2);

                if (Header.cap != null && Header.cap != 0)
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 0x1b,
                        fldValue = Header.cap.ToString()
                    };
                    source.Add(value2);
                }

                if (Header.insp != null && Header.insp != 0)
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 0x1c,
                        fldValue = Header.insp.ToString()
                    };
                    source.Add(value2);
                }
                if (Header.tvop != null)
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 0x1d,
                        fldValue = Header.tvop.ToString()
                    };
                    source.Add(value2);
                }

                if (Header.tax17 != null)
                {
                    value2 = new ParamValue
                    {
                        fldParamertId = 30,
                        fldValue = Header.tax17.ToString()
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
                if (FuncName == "WebService")
                    HeaderId = entities.prs_tblSooratHesabFromWebServiceInsert(new ObjectParameter("fldid", typeof(long)), ShakhsIdTax, Header.Kh_Name, Header.Kh_Family, Header.bid, Header.tob,
                            Header.tinb, TaxId, Header.indatim, Header.indati2m, 1, 1, Inno, Header.irtaxid,
                            FuncName, table, UserIdTax, "");
                if (FuncName == "Excel")
                    HeaderId = entities.prs_InsertSooratHesabForExcel(new ObjectParameter("fldid", typeof(long)), ShakhsIdTax, Header.Kh_Name, Header.Kh_Family, Header.bid, Header.tob,
                            Header.tinb, TaxId, Header.indatim, Header.indati2m, 1, 1, Inno, Header.irtaxid,
                            FuncName, Header.FishId, table, UserIdTax, "", IP);

                if (HeaderId != 0)
                {
                    saveDetail(HeaderId, Grid_DetailsArray, 1, UserIdTax);

                    var AllDetail = entities.prs_SelectDetailSooratHesab(HeaderId).ToList();

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
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, tdis.ToString(), 18, UserIdTax, IP);
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, tbill.ToString(), 22, UserIdTax, IP);
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, tonw.ToString(), 23, UserIdTax, IP);
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, torv.ToString(), 24, UserIdTax, IP);
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, tocv.ToString(), 25, UserIdTax, IP);
                        entities.prs_tblSooratHesabHeader_ValueUpdate(HeaderId, tvop.ToString(), 29, UserIdTax, IP);
                    }
                }
            }
            catch (Exception x)
            {
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                entities.sp_ErrorProgramInsert(Eid, InnerException, UserIdTax, x.Message, DateTime.Now, "");
                return -1;

                //return base.Json(new
                //{
                //    MsgTitle = "خطا",
                //    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                //    Er = 1
                //}, JsonRequestBehavior.AllowGet);
            }

            return HeaderId;
        }
        public void saveDetail(long HeaderId, List<ObjDetailSooratHesab> Grid_DetailsArray, int state,long UserIdTax)
        {
                cartaxtest2Entities entities = new cartaxtest2Entities();
            try
            {

                foreach (ObjDetailSooratHesab hesab2 in Grid_DetailsArray)
                {
                    ParamValue value3;
                    List<ParamValue> list2 = new List<ParamValue>();
                    if ((hesab2.sstid != "") && (hesab2.sstid != null))
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x1f,
                            fldValue = hesab2.sstid
                        };
                        list2.Add(value3);
                    }
                    if ((hesab2.sstt != "") && (hesab2.sstt != null))
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x20,
                            fldValue = hesab2.sstt
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.am != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x21,
                            fldValue = hesab2.am.ToString()
                        };
                        list2.Add(value3);
                    }
                    if ((hesab2.mu != "") && (hesab2.mu != null))
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x22,
                            fldValue = hesab2.mu
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.nw != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x23,
                            fldValue = hesab2.nw.ToString()
                        };
                        list2.Add(value3);
                    }

                    if (hesab2.fee != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x24,
                            fldValue = hesab2.fee.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.cfee != null && hesab2.cfee != 0)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x25,
                            fldValue = hesab2.cfee.ToString()
                        };
                        list2.Add(value3);
                    }
                    if ((hesab2.cut != "0") && (hesab2.cut != "") && (hesab2.cut != null))
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x26,
                            fldValue = hesab2.cut
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.exr != null && hesab2.exr != 0)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x27,
                            fldValue = hesab2.exr.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.ssrv != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 40,
                            fldValue = hesab2.ssrv.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.sscv != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x29,
                            fldValue = hesab2.sscv.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.prdis != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x2a,
                            fldValue = hesab2.prdis.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.dis != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x2b,
                            fldValue = hesab2.dis.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.adis != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x2c,
                            fldValue = hesab2.adis.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.vra != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x2d,
                            fldValue = hesab2.vra.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.vam != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x2e,
                            fldValue = hesab2.vam.ToString()
                        };
                        list2.Add(value3);
                    }
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
                    }
                    if (hesab2.odam != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x31,
                            fldValue = hesab2.odam.ToString()
                        };
                        list2.Add(value3);
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
                    }
                    if (hesab2.olam != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x34,
                            fldValue = hesab2.olam.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.consfee != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x35,
                            fldValue = hesab2.consfee.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.spro != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x36,
                            fldValue = hesab2.spro.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.bros != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x37,
                            fldValue = hesab2.bros.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.tcpbs != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x38,
                            fldValue = hesab2.tcpbs.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.cop != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x39,
                            fldValue = hesab2.cop.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.vop != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x3a,
                            fldValue = hesab2.vop.ToString()
                        };
                        list2.Add(value3);
                    }
                    if ((hesab2.bsrn != "") && (hesab2.bsrn != null))
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x3b,
                            fldValue = hesab2.bsrn
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.tsstam != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 60,
                            fldValue = hesab2.tsstam.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.pspd != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x3d,
                            fldValue = hesab2.pspd.ToString()
                        };
                        list2.Add(value3);
                    }
                    if (hesab2.cui != null)
                    {
                        value3 = new ParamValue
                        {
                            fldParamertId = 0x3e,
                            fldValue = hesab2.cui.ToString()
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
                    entities.prs_tblSooratHesab_DetailInsert(new long?(HeaderId), table2, UserIdTax, IP);
                }
            }
            catch (Exception x)
            {

                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                entities.sp_ErrorProgramInsert(Eid, InnerException, UserIdTax, x.Message, DateTime.Now, "");
            }
            
        }
        public async System.Threading.Tasks.Task<string> SamaneMoadian(long HeaderId, long UserIdTax)
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
 

                //await System.Threading.Tasks.Task.Run(() =>
                //{
                // ct.ThrowIfCancellationRequested();
                InvoiceDto item = CreateValidInvoice(fldUniqId, HeaderId);
                List<InvoiceDto> list1 = new List<InvoiceDto>();
                list1.Add(item);
                List<InvoiceDto> invoiceList = list1;
                string invoiceJson = Newtonsoft.Json.JsonConvert.SerializeObject(invoiceList);


                


                // تنظیمات اولیه
                string memoryId = select2.fldUniqId;
                const string apiUrl = "https://tp.tax.gov.ir/requestsmanager";
                string privateKeyPath = path;// "C:/Keys/privateKey.pem";
                string certificatePath = str2;// "C:/Keys/certificate.crt";
              


                // ایجاد نمونه ارسال کننده
                var sender = new InvoiceSender(memoryId, apiUrl, privateKeyPath, certificatePath);

                var mmsgg = "";
                // ========== ارسال و استعلام هوشمند ==========
                var result = await SendAndInquireWithRetry(sender, item, memoryId);

                if (result != null)
                {
                    Console.WriteLine("\n✅ موفق:");
                    mmsgg = sender.PrintInquiryResult(new List<InquiryResultModel> { result }, HeaderId, invoiceJson, Convert.ToInt64(Session["TaxUserId"]));
                }
                else
                {
                    Console.WriteLine("\n❌ ناموفق: فاکتور پیدا نشد");
                }
               

                var msgtitle = "ارسال موفق";
                var msg = "ارسال با موفقیت انجام شد.";

                if (mmsgg.Split(';')[0] != "1")
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
                m.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["TaxUserId"]), x.Message, DateTime.Now, "");

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
        static async System.Threading.Tasks.Task<InquiryResultModel> SendAndInquireWithRetry(
            InvoiceSender sender,
            InvoiceDto invoice,
            string memoryId,
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
                            memoryId,
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
            string memoryIdd,
            InvoiceSender sender)
        {
            try
            {
                var memoryId = memoryIdd;// "A11216"; // از تنظیمات بگیر

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
                    // pspd = item.fldpspd,
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
                    ins = Convert.ToInt32(hesab.fldins),
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
                var ss = m.prs_tblSooratHesabStatusInsert(new long?((long)HeaderId), new byte?(num), result.Status, result.ReferenceNumber, SerializeObjectErsal, result.Uid, new long?(UserId), IP).FirstOrDefault();


                foreach (var error in errors)
                {
                    string code = error.Code;
                    string message = error.Message;
                    string[] textArray1 = new string[] { fldMatn, "*** Code: ", code, ", Message: ", message };
                    fldMatn = string.Concat(textArray1);

                    m.prs_tblSooratHesabStatus_DetailInsert(ss.fldId, 3, message, code, UserId, IP);
                }


                foreach (ErrorModel model3 in list2)
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
