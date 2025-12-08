//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Services;
//using Avarez.Areas.Tax.Models;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity.Core.Objects;
//using System.IO;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Threading;
//using System.Web.Mvc;
//using TaxCollectData.Library.Abstraction.Clients;
//using TaxCollectData.Library.Abstraction.Cryptography;
//using TaxCollectData.Library.Algorithms;
//using TaxCollectData.Library.Dto;
//using TaxCollectData.Library.Factories;
//using TaxCollectData.Library.Models;
//using TaxCollectData.Library.Properties;
//using TaxCollectData.Library.Providers;
//using FastMember;
//using MyLib;

//namespace Avarez.Areas.Tax.Controllers
//{
//    public class TestErsalController : Controller
//    {
//        //
//        // GET: /Tax/TestErsal/

//        private const string MemoryId = "A2EO6E";
//        private const string ApiUrl = "https://tp.tax.gov.ir/requestsmanager";
//        private const string PrivateKeyPath = "C:/privateKey.pem";
//        private const string CertificatePath = "C:/certificate.crt";
//        public ActionResult Index()
//        {
//            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

//            ITaxApi taxApi = CreateTaxApi();
//            InvoiceDto validInvoice = CreateValidInvoice();
//            //InvoiceDto invalidInvoice = CreateInvalidInvoice();
//            List<InvoiceDto> invoiceList = new List<InvoiceDto>() { validInvoice/*,invalidInvoice */};
//            List<InvoiceResponseModel> responseModels = taxApi.SendInvoices(invoiceList);
  
//            Thread.Sleep(10_000);
//            InquiryByReferenceNumberDto inquiryDto = new InquiryByReferenceNumberDto(responseModels.Select(r => r.ReferenceNumber).ToList());
//            List<InquiryResultModel> inquiryResults = taxApi.InquiryByReferenceId(inquiryDto);
//            //PrintInquiryResult(inquiryResults);



//            var kk=PrintInquiryResult(inquiryResults);

//            return View();
//        }
//        private static string PrintInquiryResult(List<InquiryResultModel> inquiryResults)
//        {
//            string matn = "";
//            foreach (var result in inquiryResults)
//            {
//                matn = "Status = " + result.Status;
//                matn = matn + "*** ReferenceId = " + result.ReferenceNumber;
//                var errors = result.Data.Error;
//                foreach (var error in errors)
//                {
//                    var code = error.Code; var message = error.Message;
//                    matn = matn + "*** Code: " + code + ", Message: " + message;
//                }
//                matn = matn + "***  Warnings:";
//                var warnings = result.Data.Warning;
//                foreach (var warning in warnings)
//                {
//                    var code = warning.Code; var message = warning.Message;
//                    matn = matn + "***  Code: " + code + ", Message: " + message;
//                }
//            }
//            return matn;
//        }
//        private static ITaxApi CreateTaxApi()
//        {
//            Pkcs8SignatoryFactory pkcs8SignatoryFactory = new Pkcs8SignatoryFactory();
//            EncryptorFactory encryptorFactory = new EncryptorFactory();
//            TaxProperties properties = new TaxProperties(MemoryId);
//            TaxApiFactory taxApiFactory = new TaxApiFactory(ApiUrl, properties);
//            ISignatory signatory = pkcs8SignatoryFactory.Create(PrivateKeyPath, CertificatePath);
//            ITaxPublicApi publicApi = taxApiFactory.CreatePublicApi(signatory);
//            IEncryptor encryptor = encryptorFactory.Create(publicApi);
//            return taxApiFactory.CreateApi(signatory, encryptor);
//        }
//        private static InvoiceDto CreateValidInvoice()
//        {
//            Random random = new Random();
//            long serial = random.Next(1_000_000_000);
//            DateTime now = DateTime.Now;
//            string taxId = GenerateTaxId(serial, now);
//            string inno = serial.ToString("X").PadLeft(10, '0');
//            long indatim = new DateTimeOffset(now).ToUnixTimeMilliseconds();
//            InvoiceDto invoice = new InvoiceDto()
//            {
//                Header = new HeaderDto()
//                {
//                    taxid = taxId,
//                    inno = inno,
//                    indatim = indatim,
//                    inty = 1,
//                    inp = 1,
//                    ins = 1,
//                    tins = "10862016649",
//                    tinb = "14006964802",
//                    tob = 2,
//                    tprdis = 1,
//                    tdis = 0,
//                    tadis = 1,
//                    tvam = 0,
//                    todam = 0,
//                    tbill = 1,
//                    setm = 1
//                }
//            ,
//                Body = new List<BodyItemDto>()
//                {
//                    new BodyItemDto()
//                    { sstid = "2720000114542", sstt = "تسست ", mu = "16112", am = 1, fee = 1, prdis = 1, dis = 0, adis = 1, vra = 10, vam = 0, tsstam = 1
//                    }
//                }
//            };
//            return invoice;
//        }
//        private static string GenerateTaxId(long serial, DateTime now)
//        {
//            TaxIdProvider taxIdProvider = new TaxIdProvider(new VerhoeffAlgorithm());
//            return taxIdProvider.GenerateTaxId(MemoryId, serial, now);
//        }
//    }
//}
