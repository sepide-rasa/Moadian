using TaxCollectData.Library.Abstraction.Clients;
using TaxCollectData.Library.Algorithms;
using TaxCollectData.Library.Dto;
using TaxCollectData.Library.Factories;
using TaxCollectData.Library.Models;
using TaxCollectData.Library.Properties;
using TaxCollectData.Library.Providers;
using Avarez.Areas.Tax.Models;
using Avarez.Models;
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using TaxCollectData.Library.Enums;
using System.Text.RegularExpressions;

namespace Avarez.Areas.Tax.Models
{
    public class InvoiceSender
    {
        private readonly ITaxApi _taxApi;
        private readonly ITaxPublicApi _publicApi;
        private readonly string _memoryId;

        // سازنده کلاس
        public InvoiceSender(string memoryId, string apiUrl, string privateKeyPath, string certificatePath)
        {
            _memoryId = memoryId;

            var pkcs8SignatoryFactory = new Pkcs8SignatoryFactory();
            var encryptorFactory = new EncryptorFactory();
            var properties = new TaxProperties(memoryId);
            var taxApiFactory = new TaxApiFactory(apiUrl, properties);

            var signatory = pkcs8SignatoryFactory.Create(privateKeyPath, certificatePath);
            _publicApi = taxApiFactory.CreatePublicApi(signatory);
            var encryptor = encryptorFactory.Create(_publicApi);

            _taxApi = taxApiFactory.CreateApi(signatory, encryptor);
        }
        public ITaxApi TaxApi => _taxApi;

        // دسترسی به PublicApi
        public ITaxPublicApi PublicApi => _publicApi;


        // تابع اصلی ارسال فاکتور
        public async System.Threading.Tasks.Task<List<InquiryResultModel>> SendInvoiceAsync(InvoiceDto invoice, int maxRetries = 5)
        {
            try
            {
                // ارسال فاکتور
                var invoiceList = new List<InvoiceDto> { invoice };
                var responseModels = _taxApi.SendInvoices(invoiceList);

                Console.WriteLine($"✅ فاکتور ارسال شد:");
                Console.WriteLine($"   شماره پیگیری: {responseModels[0].ReferenceNumber}");
                Console.WriteLine($"   UID: {responseModels[0].Uid}");
                Console.WriteLine($"   TaxId: {responseModels[0].TaxId}");

                // ذخیره اطلاعات برای استعلام
                var referenceNumber = responseModels[0].ReferenceNumber;
                var uid = responseModels[0].Uid;

                // تلاش مجدد برای استعلام (تا 5 بار)
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    Console.WriteLine($"\n🔍 تلاش {attempt} از {maxRetries} برای استعلام...");

                    // زمان انتظار بیشتر در تلاش‌های اول
                    int waitTime = attempt == 1 ? 15_000 : 10_000;
                    Console.WriteLine($"⏳ انتظار {waitTime / 1000} ثانیه...");
                    await System.Threading.Tasks.Task.Delay(waitTime);

                    // استعلام با بازه زمانی گسترده‌تر
                    var inquiryDto = new InquiryByReferenceNumberDto(
                        new List<string> { referenceNumber },
                        DateTime.Now.AddHours(-2), // 2 ساعت قبل
                        DateTime.Now.AddMinutes(5)  // 5 دقیقه بعد
                    );

                    var inquiryResults = _taxApi.InquiryByReferenceId(inquiryDto);

                    if (inquiryResults != null && inquiryResults.Count > 0)
                    {
                        var result = inquiryResults[0];
                        Console.WriteLine($"📊 وضعیت: {result.Status}");

                        // اگر هنوز در حال پردازش است
                        if (result.Status == "IN_PROGRESS")
                        {
                            Console.WriteLine("⏳ فاکتور هنوز در حال پردازش است...");
                            if (attempt < maxRetries)
                            {
                                continue; // تلاش مجدد
                            }
                        }
                        // اگر وضعیت قطعی شد
                        else if (result.Status == "SUCCESS" ||
                                 result.Status == "FAILED" ||
                                 result.Status == "TIMEOUT")
                        {
                            Console.WriteLine($"✅ پردازش تکمیل شد با وضعیت: {result.Status}");
                            return inquiryResults;
                        }
                        // اگر NOT_FOUND بود
                        else if (result.Status == "NOT_FOUND")
                        {
                            Console.WriteLine("⚠️ فاکتور پیدا نشد، تلاش مجدد...");
                            if (attempt < maxRetries)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("⚠️ پاسخ خالی از سرور");
                    }
                }

                // اگر همه تلاش‌ها ناموفق بود، یک استعلام با UID امتحان کن
                Console.WriteLine("\n🔍 تلاش استعلام با UID...");
                var inquiryByUid = InquiryByUid(new List<string> { uid });

                if (inquiryByUid != null && inquiryByUid.Count > 0)
                {
                    return inquiryByUid;
                }

                throw new Exception($"بعد از {maxRetries} تلاش، نتوانستیم وضعیت فاکتور را دریافت کنیم");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در ارسال فاکتور: {ex.Message}");
                throw;
            }
        }


        //// تابع ایجاد فاکتور نمونه
        //public InvoiceDto CreateSampleInvoice(string sellerEconomicCode, string buyerEconomicCode,
        //    List<InvoiceItem> items)
        //{
        //    var random = new Random();
        //    long serial = random.NextInt64(1_000_000_000);
        //    var now = DateTime.Now;

        //    string taxId = GenerateTaxId(serial, now);
        //    string inno = serial.ToString("X").PadLeft(10, '0');
        //    long indatim = new DateTimeOffset(now).ToUnixTimeMilliseconds();

        //    // محاسبه مجموع مبالغ
        //    decimal tprdis = items.Sum(i => i.Amount * i.UnitPrice);
        //    decimal tdis = items.Sum(i => i.Discount);
        //    decimal tadis = tprdis - tdis;
        //    decimal tvam = items.Sum(i => i.VATAmount);
        //    decimal tbill = tadis + tvam;

        //    var invoice = new InvoiceDto
        //    {
        //        Header = new HeaderDto
        //        {
        //            taxid = taxId,
        //            inno = inno,
        //            indatim = indatim,
        //            inty = 1,              // نوع صورتحساب: فروش
        //            inp = 1,               // الگوی صورتحساب
        //            ins = 1,               // موضوع صورتحساب
        //            tins = sellerEconomicCode,
        //            tinb = buyerEconomicCode,
        //            tprdis = tprdis,
        //            tdis = tdis,
        //            tadis = tadis,
        //            tvam = tvam,
        //            todam = 0,
        //            tbill = tbill,
        //            setm = 1               // روش تسویه: نقدی
        //        },
        //        Body = items.Select(item => new BodyItemDto
        //        {
        //            sstid = item.ProductCode,
        //            sstt = item.ProductName,
        //            mu = item.Unit,
        //            am = item.Amount,
        //            fee = item.UnitPrice,
        //            prdis = item.Amount * item.UnitPrice,
        //            dis = item.Discount,
        //            adis = (item.Amount * item.UnitPrice) - item.Discount,
        //            vra = item.VATRate,
        //            vam = item.VATAmount,
        //            tsstam = (item.Amount * item.UnitPrice) - item.Discount + item.VATAmount
        //        }).ToList()
        //    };

        //    return invoice;
        //}

        
        // تابع چاپ نتیجه استعلام
        public string PrintInquiryResult(List<InquiryResultModel> inquiryResults, long HeaderId, string SerializeObjectErsal, long UserId)
        {
            //string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

            cartaxtest2Entities m = new cartaxtest2Entities();
            string fldMatn = "";
            byte num = 1;
            foreach (var result in inquiryResults)
            {
                fldMatn = "Status = " + result.Status;
                //var errors = result.Data.Error;

                if (result.Data?.Error != null && result.Data.Error.Any())
                {
                    num = 3;
                }
                //List<ErrorModel> list2 = result.Data.Warning;
                if (result.Data?.Warning != null && result.Data.Warning.Any())
                {
                    if (!(result.Data?.Error != null && result.Data.Error.Any()))
                        num = 2;
                }

                Console.WriteLine("========================================");
                Console.WriteLine($"وضعیت: {result.Status}");
                Console.WriteLine($"شماره پیگیری: {result.ReferenceNumber}");
                Console.WriteLine($"UID: {result.Uid}");
                string uid = "";
                string referenceNumber = result.ReferenceNumber;
                if (result.Uid != null)
                    uid = result.Uid;
                var ss = m.prs_tblSooratHesabStatusInsert(new long?((long)HeaderId), new byte?(num), result.Status, result.ReferenceNumber, SerializeObjectErsal, uid, new long?(UserId), "1").FirstOrDefault().fldId;

                if (result.Data?.Error != null && result.Data.Error.Any())
                {
                    // Console.WriteLine("\nخطاها:");
                    foreach (var error in result.Data.Error)
                    {
                        //Console.WriteLine($"  کد: {error.Code}, پیغام: {error.Message}");
                        string code = error.Code;
                        string message = error.Message;
                        string[] textArray1 = new string[] { fldMatn, "*** Code: ", code, ", Message: ", message };
                        fldMatn = string.Concat(textArray1);

                        m.prs_tblSooratHesabStatus_DetailInsert(ss, 3, message, code, UserId, "1");
                    }

                }

                if (result.Data?.Warning != null && result.Data.Warning.Any())
                {
                    //Console.WriteLine("\nاخطارها:");
                    foreach (var warning in result.Data.Warning)
                    {
                        //Console.WriteLine($"  کد: {warning.Code}, پیغام: {warning.Message}");
                        string code = warning.Code;
                        string message = warning.Message;
                        string[] textArray2 = new string[] { fldMatn, "***  Code: ", code, ", Message: ", message };
                        fldMatn = string.Concat(textArray2);

                        m.prs_tblSooratHesabStatus_DetailInsert(ss, 2, message, code, UserId, "1");
                    }
                }

                Console.WriteLine($"\nموفقیت: {(result.Data?.Success == true ? "بله" : "خیر")}");
            }
            //foreach (var result in inquiryResults)
            //{
            //    fldMatn = "Status = " + result.Status;
            //    var errors = result.Data.Error;

            //    if (errors.Count() > 0)
            //    {
            //        num = 3;
            //    }
            //    List<ErrorModel> list2 = result.Data.Warning;
            //    if (list2.Count() > 0)
            //    {
            //        if (errors.Count() == 0)
            //            num = 2;
            //    }
            //    // long? ss = Id;
            //    //  if (Id == 0)
            //   // m.prs_tblSooratHesabStatusdDelete
            //         var ss = m.prs_tblSooratHesabStatusInsert(new long?((long)HeaderId), new byte?(num), result.Status, result.ReferenceNumber, SerializeObjectErsal, result.Uid, new long?(UserId), "1").FirstOrDefault().fldId;
            //   // else
            //      //  m.prs_tblSooratHesabStatusUpdate(Id, new long?((long)HeaderId), new byte?(num), result.Status, result.ReferenceNumber, SerializeObjectErsal, result.Uid, new long?(UserId), IP);
            //    foreach (var error in errors)
            //    {
            //        string code = error.Code;
            //        string message = error.Message;
            //        string[] textArray1 = new string[] { fldMatn, "*** Code: ", code, ", Message: ", message };
            //        fldMatn = string.Concat(textArray1);

            //        m.prs_tblSooratHesabStatus_DetailInsert(ss, 3, message, code, UserId, "1");
            //    }


            //    foreach (ErrorModel model3 in list2)
            //    {

            //        string code = model3.Code;
            //        string message = model3.Message;
            //        string[] textArray2 = new string[] { fldMatn, "***  Code: ", code, ", Message: ", message };
            //        fldMatn = string.Concat(textArray2);

            //        m.prs_tblSooratHesabStatus_DetailInsert(ss, 2, message, code, UserId, "1");
            //    }

            //}

            return (num.ToString() + ";" + fldMatn);
        }

        // تابع ایجاد TaxApi
        private ITaxApi CreateTaxApi(string memoryId, string apiUrl, string privateKeyPath, string certificatePath)
        {
            var pkcs8SignatoryFactory = new Pkcs8SignatoryFactory();
            var encryptorFactory = new EncryptorFactory();
            var properties = new TaxProperties(memoryId);
            var taxApiFactory = new TaxApiFactory(apiUrl, properties);

            var signatory = pkcs8SignatoryFactory.Create(privateKeyPath, certificatePath);
            var publicApi = taxApiFactory.CreatePublicApi(signatory);
            var encryptor = encryptorFactory.Create(publicApi);

            return taxApiFactory.CreateApi(signatory, encryptor);
        }

        // تولید شماره مالیاتی
        private string GenerateTaxId(long serial, DateTime dateTime)
        {
            var taxIdProvider = new TaxIdProvider(new VerhoeffAlgorithm());
            return taxIdProvider.GenerateTaxId(_memoryId, serial, dateTime);
        }

        // ============ توابع استعلام ============

        // استعلام با شماره پیگیری
        public List<InquiryResultModel> InquiryByReferenceNumber(List<string> referenceNumbers,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // بازه زمانی گسترده‌تر برای جلوگیری از NOT_FOUND
                var start = startDate ?? DateTime.Now.AddHours(-2);
                var end = endDate ?? DateTime.Now.AddMinutes(5);

                Console.WriteLine($"🔍 استعلام با شماره پیگیری:");
                Console.WriteLine($"   از تاریخ: {start:yyyy/MM/dd HH:mm:ss}");
                Console.WriteLine($"   تا تاریخ: {end:yyyy/MM/dd HH:mm:ss}");
                Console.WriteLine($"   شماره‌ها: {string.Join(", ", referenceNumbers)}");

                var inquiryDto = new InquiryByReferenceNumberDto(
                    referenceNumbers,
                    start,
                    end
                );

                var results = _taxApi.InquiryByReferenceId(inquiryDto);

                if (results == null || results.Count == 0)
                {
                    Console.WriteLine("⚠️ هیچ نتیجه‌ای یافت نشد");
                }
                else
                {
                    Console.WriteLine($"✅ {results.Count} نتیجه پیدا شد");
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در استعلام با شماره پیگیری: {ex.Message}");
                throw;
            }
        }

        // استعلام با UID
        public List<InquiryResultModel> InquiryByUid(List<string> uidList,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // بازه زمانی گسترده‌تر
                var start = startDate ?? DateTime.Now.AddHours(-2);
                var end = endDate ?? DateTime.Now.AddMinutes(5);

                Console.WriteLine($"🔍 استعلام با UID:");
                Console.WriteLine($"   از تاریخ: {start:yyyy/MM/dd HH:mm:ss}");
                Console.WriteLine($"   تا تاریخ: {end:yyyy/MM/dd HH:mm:ss}");
                Console.WriteLine($"   UID‌ها: {string.Join(", ", uidList)}");

                var inquiryDto = new InquiryByUidDto(
                    uidList,
                    _memoryId,
                    start,
                    end
                );

                var results = _taxApi.InquiryByUid(inquiryDto);

                if (results == null || results.Count == 0)
                {
                    Console.WriteLine("⚠️ هیچ نتیجه‌ای یافت نشد");
                }
                else
                {
                    Console.WriteLine($"✅ {results.Count} نتیجه پیدا شد");
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در استعلام با UID: {ex.Message}");
                throw;
            }
        }

        // استعلام بر اساس بازه زمانی (با فیلتر وضعیت و صفحه‌بندی)
        public List<InquiryResultModel> InquiryByTime(
            DateTime? startDate = null,
            DateTime? endDate = null,
            RequestStatus? status = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddDays(-1);
                var end = endDate ?? DateTime.Now;
                var pageable = new Pageable(pageNumber, pageSize);

                var inquiryDto = new InquiryByTimeRangeDto(
                    start,
                    end,
                    pageable,
                    status
                );

                var results = _taxApi.InquiryByTime(inquiryDto);
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در استعلام بر اساس زمان: {ex.Message}");
                throw;
            }
        }



        // استعلام اطلاعات مودی (با کد اقتصادی)
        public TaxpayerModel GetTaxpayerInfo(string economicCode)
        {
            try
            {
                var taxpayer = _taxApi.GetTaxpayer(economicCode);
                return taxpayer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در استعلام اطلاعات مودی: {ex.Message}");
                throw;
            }
        }

        // استعلام اطلاعات حافظه مالیاتی
        public FiscalFullInformationModel GetFiscalInfo(string memoryId)
        {
            try
            {
                var fiscalInfo = _taxApi.GetFiscalInformation(memoryId);
                return fiscalInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در استعلام اطلاعات حافظه: {ex.Message}");
                throw;
            }
        }

        // چاپ وضعیت فاکتور در کارپوشه
        public void PrintInvoiceStatus(List<InvoiceStatusInquiryResponseDto> statusList)
        {
            foreach (var status in statusList)
            {
                Console.WriteLine("========================================");
                Console.WriteLine($"شماره مالیاتی: {status.TaxId}");
                Console.WriteLine($"وضعیت فاکتور: {status.InvoiceStatus}");
                Console.WriteLine($"وضعیت ماده 6: {status.Article6Status}");

                if (!string.IsNullOrEmpty(status.Error))
                {
                    Console.WriteLine($"خطا: {status.Error}");
                }
            }
        }
    }
    public class TaxIdHelper
    {
        private const string TaxIdPattern = @"^[a-zA-Z0-9]{6}[a-fA-F0-9]{15}[0-9]{1}$";

        // بررسی صحت TaxId
        public static bool ValidateTaxId(string taxId)
        {
            if (string.IsNullOrEmpty(taxId))
            {
                Console.WriteLine("❌ TaxId خالی است");
                return false;
            }

            if (taxId.Length != 22)
            {
                Console.WriteLine($"❌ طول TaxId باید 22 کاراکتر باشد، ولی {taxId.Length} کاراکتر است");
                return false;
            }

            var regex = new Regex(TaxIdPattern);
            if (!regex.IsMatch(taxId))
            {
                Console.WriteLine($"❌ TaxId فرمت صحیح ندارد: {taxId}");
                AnalyzeTaxId(taxId);
                return false;
            }

            Console.WriteLine($"✅ TaxId معتبر است: {taxId}");
            return true;
        }

        // تحلیل دقیق TaxId
        public static void AnalyzeTaxId(string taxId)
        {
            if (string.IsNullOrEmpty(taxId))
            {
                Console.WriteLine("TaxId خالی است");
                return;
            }

            Console.WriteLine("\n🔍 تحلیل TaxId:");
            Console.WriteLine($"   طول: {taxId.Length} کاراکتر");
            Console.WriteLine($"   مقدار: {taxId}");

            if (taxId.Length >= 6)
            {
                string memoryPart = taxId.Substring(0, 6);
                Console.WriteLine($"   6 کاراکتر اول (Memory ID): {memoryPart}");

                bool memoryValid = Regex.IsMatch(memoryPart, @"^[a-zA-Z0-9]{6}$");
                Console.WriteLine($"      {(memoryValid ? "✅" : "❌")} باید فقط حروف و اعداد باشد");
            }

            if (taxId.Length >= 21)
            {
                string middlePart = taxId.Substring(6, 15);
                Console.WriteLine($"   15 کاراکتر وسط (Serial+Date): {middlePart}");

                bool middleValid = Regex.IsMatch(middlePart, @"^[a-fA-F0-9]{15}$");
                Console.WriteLine($"      {(middleValid ? "✅" : "❌")} باید فقط هگزادسیمال (0-9, a-f) باشد");

                if (!middleValid)
                {
                    // پیدا کردن کاراکترهای اشتباه
                    for (int i = 0; i < middlePart.Length; i++)
                    {
                        char c = middlePart[i];
                        if (!Regex.IsMatch(c.ToString(), @"[a-fA-F0-9]"))
                        {
                            Console.WriteLine($"      ⚠️ کاراکتر اشتباه در موقعیت {i + 6}: '{c}' (ASCII: {(int)c})");
                        }
                    }
                }
            }

            if (taxId.Length >= 22)
            {
                string checkDigit = taxId.Substring(21, 1);
                Console.WriteLine($"   کاراکتر آخر (Check Digit): {checkDigit}");

                bool checkValid = Regex.IsMatch(checkDigit, @"^[0-9]$");
                Console.WriteLine($"      {(checkValid ? "✅" : "❌")} باید فقط عدد (0-9) باشد");
            }
        }

        // تولید صحیح TaxId
        public static string GenerateValidTaxId(string memoryId, long serialNumber, DateTime dateTime)
        {
            Console.WriteLine("\n📝 تولید TaxId:");
            Console.WriteLine($"   Memory ID: {memoryId}");
            Console.WriteLine($"   Serial Number (Dec): {serialNumber}");
            Console.WriteLine($"   DateTime: {dateTime:yyyy/MM/dd HH:mm:ss}");

            // بررسی Memory ID
            if (string.IsNullOrEmpty(memoryId) || memoryId.Length != 6)
            {
                throw new ArgumentException("Memory ID باید دقیقاً 6 کاراکتر باشد");
            }

            if (!Regex.IsMatch(memoryId, @"^[a-zA-Z0-9]{6}$"))
            {
                throw new ArgumentException($"Memory ID فقط باید شامل حروف و اعداد باشد: {memoryId}");
            }

            // تبدیل سریال به هگزادسیمال با حروف کوچک
            string serialHex = serialNumber.ToString("x").PadLeft(10, '0');
            Console.WriteLine($"   Serial (Hex): {serialHex}");

            if (serialHex.Length > 10)
            {
                Console.WriteLine($"   ⚠️ هشدار: سریال بیش از 10 رقم هگزادسیمال دارد، برش داده می‌شود");
                serialHex = serialHex.Substring(serialHex.Length - 10, 10);
            }

            // تبدیل تاریخ به میلی‌ثانیه (5 رقم هگزادسیمال آخر)
            long timestamp = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
            string timestampHex = timestamp.ToString("x");

            // فقط 5 رقم آخر
            if (timestampHex.Length > 5)
            {
                timestampHex = timestampHex.Substring(timestampHex.Length - 5, 5);
            }
            else
            {
                timestampHex = timestampHex.PadLeft(5, '0');
            }

            Console.WriteLine($"   Timestamp (Hex): {timestampHex}");

            // ترکیب قسمت‌ها (با حروف کوچک)
            string combined = (serialHex + timestampHex).ToLower();
            Console.WriteLine($"   Combined (15 char): {combined}");

            // محاسبه چک‌دیجیت
            var taxIdProvider = new TaxIdProvider(new VerhoeffAlgorithm());
            string taxId = taxIdProvider.GenerateTaxId(memoryId, serialNumber, dateTime);

            Console.WriteLine($"   ✅ TaxId نهایی: {taxId}");

            // بررسی نهایی
            ValidateTaxId(taxId);

            return taxId;
        }

        // تست تولید TaxId
        public static void TestTaxIdGeneration()
        {
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("🧪 تست تولید TaxId");
            Console.WriteLine(new string('=', 70));

            var tests = new[]
            {
                ("A11216", 123456789L, DateTime.Now),
                ("ABC123", 987654321L, DateTime.Now),
                ("TEST01", 111111111L, new DateTime(2024, 1, 1, 12, 0, 0))
            };

            foreach (var (memoryId, serial, date) in tests)
            {
                Console.WriteLine();
                try
                {
                    var taxId = GenerateValidTaxId(memoryId, serial, date);
                    Console.WriteLine($"✅ موفق");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ خطا: {ex.Message}");
                }
            }
        }

        // اصلاح TaxId اشتباه
        public static string FixTaxId(string invalidTaxId, string memoryId)
        {
            Console.WriteLine($"\n🔧 تلاش برای اصلاح TaxId: {invalidTaxId}");

            if (string.IsNullOrEmpty(invalidTaxId) || invalidTaxId.Length < 21)
            {
                Console.WriteLine("❌ TaxId خیلی کوتاه است، نمی‌توان اصلاح کرد");
                return null;
            }

            try
            {
                // استخراج قسمت وسط (serial + timestamp)
                string middlePart = invalidTaxId.Substring(6, 15);

                // تبدیل به حروف کوچک (برای رفع مشکل حروف بزرگ)
                middlePart = middlePart.ToLower();

                // حذف کاراکترهای غیرمجاز
                middlePart = Regex.Replace(middlePart, @"[^a-f0-9]", "0");

                if (middlePart.Length != 15)
                {
                    middlePart = middlePart.PadRight(15, '0').Substring(0, 15);
                }

                // استخراج چک‌دیجیت یا تولید جدید
                string checkDigit = invalidTaxId.Length >= 22 ?
                    Regex.Replace(invalidTaxId.Substring(21, 1), @"[^0-9]", "0") : "0";

                string fixedTaxId = memoryId + middlePart + checkDigit;

                Console.WriteLine($"   TaxId اصلاح‌شده: {fixedTaxId}");

                if (ValidateTaxId(fixedTaxId))
                {
                    return fixedTaxId;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در اصلاح: {ex.Message}");
                return null;
            }
        }
    }
}