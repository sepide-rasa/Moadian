//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using TaxCollectData.Library.Dto;
//using TaxCollectData.Library.Models;
//using TaxCollectData.Library.Abstraction.Clients;

//namespace Avarez.Areas.Tax.Models
//{
//    public class InquiryDebugger
//    {
//        private readonly ITaxApi _taxApi;
//        private readonly ITaxPublicApi _publicApi;
//        private readonly string _memoryId;

//        public InquiryDebugger(ITaxApi taxApi, ITaxPublicApi publicApi, string memoryId)
//        {
//            _taxApi = taxApi;
//            _publicApi = publicApi;
//            _memoryId = memoryId;
//        }

//        // سازنده جایگزین بدون publicApi (بدون چک زمان سرور)
//        public InquiryDebugger(ITaxApi taxApi, string memoryId)
//        {
//            _taxApi = taxApi;
//            _publicApi = null;
//            _memoryId = memoryId;
//        }

//        // دیباگ کامل مشکل NOT_FOUND
//        public async Task<InquiryResultModel> DebugInvoiceInquiry(
//            string referenceNumber,
//            string uid,
//            string taxId)
//        {
//            Console.WriteLine("\n" + new string('=', 70));
//            Console.WriteLine("🔧 شروع دیباگ مشکل NOT_FOUND");
//            Console.WriteLine(new string('=', 70));

//            Console.WriteLine($"\n📋 اطلاعات فاکتور:");
//            Console.WriteLine($"   Reference Number: {referenceNumber}");
//            Console.WriteLine($"   UID: {uid}");
//            Console.WriteLine($"   Tax ID: {taxId}");
//            Console.WriteLine($"   Memory ID: {_memoryId}");

//            // 1. بررسی زمان سرور
//            Console.WriteLine("\n1️⃣ بررسی زمان سرور...");
//            await CheckServerTime();

//            // 2. استعلام با بازه‌های زمانی مختلف
//            Console.WriteLine("\n2️⃣ استعلام با بازه‌های زمانی مختلف...");
//            var result = await TryDifferentTimeRanges(referenceNumber, uid);

//            if (result != null && result.Status != "NOT_FOUND")
//            {
//                Console.WriteLine($"\n✅ فاکتور پیدا شد با وضعیت: {result.Status}");
//                return result;
//            }

//            // 3. استعلام با متد‌های مختلف
//            Console.WriteLine("\n3️⃣ استعلام با متد‌های مختلف...");
//            result = await TryDifferentMethods(referenceNumber, uid);

//            if (result != null && result.Status != "NOT_FOUND")
//            {
//                Console.WriteLine($"\n✅ فاکتور پیدا شد با وضعیت: {result.Status}");
//                return result;
//            }

//            // 4. جستجو در لیست فاکتورهای اخیر
//            Console.WriteLine("\n4️⃣ جستجو در لیست فاکتورهای اخیر...");
//            result = await SearchInRecentInvoices(taxId);

//            if (result != null)
//            {
//                Console.WriteLine($"\n✅ فاکتور در لیست اخیر پیدا شد!");
//                return result;
//            }

//            Console.WriteLine("\n" + new string('=', 70));
//            Console.WriteLine("❌ متأسفانه فاکتور پیدا نشد");
//            Console.WriteLine("💡 احتمالات:");
//            Console.WriteLine("   1. فاکتور هنوز در حال پردازش است (بیشتر صبر کنید)");
//            Console.WriteLine("   2. فاکتور به دلیل خطای اعتبارسنجی رد شده");
//            Console.WriteLine("   3. مشکلی در ارسال اولیه رخ داده");
//            Console.WriteLine(new string('=', 70));

//            return null;
//        }

//        // بررسی زمان سرور
//        private async Task CheckServerTime()
//        {
//            try
//            {
//                var serverInfo = _taxApi.GetServerInformation();
//                var serverTime = DateTimeOffset.FromUnixTimeMilliseconds(serverInfo.ServerTime);
//                var localTime = DateTimeOffset.Now;
//                var diff = (localTime - serverTime).TotalMinutes;

//                Console.WriteLine($"   زمان سرور: {serverTime:yyyy/MM/dd HH:mm:ss}");
//                Console.WriteLine($"   زمان سیستم: {localTime:yyyy/MM/dd HH:mm:ss}");
//                Console.WriteLine($"   اختلاف: {Math.Abs(diff):F1} دقیقه");

//                if (Math.Abs(diff) > 5)
//                {
//                    Console.WriteLine($"   ⚠️ اختلاف زمانی زیاد است! این می‌تواند باعث NOT_FOUND شود");
//                }
//                else
//                {
//                    Console.WriteLine($"   ✅ زمان سرور و سیستم همگام هستند");
//                }

//                await Task.Delay(1000);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"   ❌ خطا در دریافت زمان سرور: {ex.Message}");
//            }
//        }

//        // تست با بازه‌های زمانی مختلف
//        private async Task<InquiryResultModel> TryDifferentTimeRanges(string referenceNumber, string uid)
//        {
//            var timeRanges = new[]
//            {
//                ("15 دقیقه گذشته", DateTime.Now.AddMinutes(-15), DateTime.Now.AddMinutes(5)),
//                ("30 دقیقه گذشته", DateTime.Now.AddMinutes(-30), DateTime.Now.AddMinutes(5)),
//                ("1 ساعت گذشته", DateTime.Now.AddHours(-1), DateTime.Now.AddMinutes(5)),
//                ("2 ساعت گذشته", DateTime.Now.AddHours(-2), DateTime.Now.AddMinutes(5)),
//                ("6 ساعت گذشته", DateTime.Now.AddHours(-6), DateTime.Now.AddMinutes(5)),
//                ("24 ساعت گذشته", DateTime.Now.AddDays(-1), DateTime.Now.AddMinutes(5))
//            };

//            foreach (var (label, start, end) in timeRanges)
//            {
//                Console.WriteLine($"\n   📅 تست {label}:");
//                Console.WriteLine($"      از: {start:yyyy/MM/dd HH:mm:ss}");
//                Console.WriteLine($"      تا: {end:yyyy/MM/dd HH:mm:ss}");

//                try
//                {
//                    var inquiryDto = new InquiryByReferenceNumberDto(
//                        new List<string> { referenceNumber },
//                        start,
//                        end
//                    );

//                    var results = _taxApi.InquiryByReferenceId(inquiryDto);

//                    if (results != null && results.Count > 0)
//                    {
//                        var result = results[0];
//                        Console.WriteLine($"      ✅ یافت شد! وضعیت: {result.Status}");

//                        if (result.Status != "NOT_FOUND")
//                        {
//                            return result;
//                        }
//                    }
//                    else
//                    {
//                        Console.WriteLine($"      ❌ یافت نشد");
//                    }

//                    await Task.Delay(2000); // 2 ثانیه تأخیر بین درخواست‌ها
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"      ❌ خطا: {ex.Message}");
//                }
//            }

//            return null;
//        }

//        // تست با متدهای مختلف استعلام
//        private async Task<InquiryResultModel> TryDifferentMethods(string referenceNumber, string uid)
//        {
//            // روش 1: با شماره پیگیری
//            Console.WriteLine("\n   🔍 روش 1: استعلام با شماره پیگیری");
//            try
//            {
//                var inquiryDto = new InquiryByReferenceNumberDto(
//                    new List<string> { referenceNumber },
//                    DateTime.Now.AddHours(-3),
//                    DateTime.Now.AddMinutes(10)
//                );

//                var results = _taxApi.InquiryByReferenceId(inquiryDto);

//                if (results != null && results.Count > 0 && results[0].Status != "NOT_FOUND")
//                {
//                    Console.WriteLine($"      ✅ پیدا شد: {results[0].Status}");
//                    return results[0];
//                }
//                else
//                {
//                    Console.WriteLine($"      ❌ NOT_FOUND");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"      ❌ خطا: {ex.Message}");
//            }

//            await Task.Delay(3000);

//            // روش 2: با UID
//            Console.WriteLine("\n   🔍 روش 2: استعلام با UID");
//            try
//            {
//                var inquiryDto = new InquiryByUidDto(
//                    new List<string> { uid },
//                    _memoryId,
//                    DateTime.Now.AddHours(-3),
//                    DateTime.Now.AddMinutes(10)
//                );

//                var results = _taxApi.InquiryByUid(inquiryDto);

//                if (results != null && results.Count > 0 && results[0].Status != "NOT_FOUND")
//                {
//                    Console.WriteLine($"      ✅ پیدا شد: {results[0].Status}");
//                    return results[0];
//                }
//                else
//                {
//                    Console.WriteLine($"      ❌ NOT_FOUND");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"      ❌ خطا: {ex.Message}");
//            }

//            return null;
//        }

//        // جستجو در فاکتورهای اخیر
//        private async Task<InquiryResultModel> SearchInRecentInvoices(string taxId)
//        {
//            try
//            {
//                Console.WriteLine("   🔍 جستجو در فاکتورهای 24 ساعت اخیر...");

//                var inquiryDto = new InquiryByTimeRangeDto(
//                    DateTime.Now.AddDays(-1),
//                    DateTime.Now.AddMinutes(5),
//                    new Pageable(1, 100),
//                    null // همه وضعیت‌ها
//                );

//                var results = _taxApi.InquiryByTime(inquiryDto);

//                if (results == null || results.Count == 0)
//                {
//                    Console.WriteLine("      ❌ هیچ فاکتوری پیدا نشد");
//                    return null;
//                }

//                Console.WriteLine($"      📋 {results.Count} فاکتور پیدا شد، در حال جستجو...");

//                // جستجو بر اساس TaxId
//                foreach (var result in results)
//                {
//                    // فرض می‌کنیم TaxId در Data موجود باشد
//                    if (result.Data != null)
//                    {
//                        Console.WriteLine($"         - {result.ReferenceNumber}: {result.Status}");
//                    }
//                }

//                // اگر TaxId پیدا شد
//                var found = results.FirstOrDefault(r =>
//                    r.Data != null &&
//                    r.Status != "NOT_FOUND"
//                );

//                if (found != null)
//                {
//                    Console.WriteLine($"      ✅ فاکتور پیدا شد!");
//                    return found;
//                }

//                Console.WriteLine("      ❌ فاکتور مورد نظر در لیست نیست");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"      ❌ خطا: {ex.Message}");
//            }

//            return null;
//        }

//        // تست ارسال و استعلام با زمان‌بندی دقیق
//        public async Task<InquiryResultModel> TestWithOptimalTiming(InvoiceDto invoice)
//        {
//            Console.WriteLine("\n🧪 تست ارسال با زمان‌بندی بهینه\n");

//            // ارسال
//            Console.WriteLine("📤 ارسال فاکتور...");
//            var invoiceList = new List<InvoiceDto> { invoice };
//            var responseModels = _taxApi.SendInvoices(invoiceList);

//            var refNum = responseModels[0].ReferenceNumber;
//            var uid = responseModels[0].Uid;
//            var taxId = responseModels[0].TaxId;

//            Console.WriteLine($"✅ ارسال شد:");
//            Console.WriteLine($"   Reference: {refNum}");
//            Console.WriteLine($"   UID: {uid}");

//            // استعلام با تلاش‌های متعدد
//            int[] waitTimes = { 15, 10, 10, 15, 20 }; // ثانیه

//            for (int i = 0; i < waitTimes.Length; i++)
//            {
//                Console.WriteLine($"\n⏳ انتظار {waitTimes[i]} ثانیه... (تلاش {i + 1}/{waitTimes.Length})");
//                await Task.Delay(waitTimes[i] * 1000);

//                var inquiryDto = new InquiryByReferenceNumberDto(
//                    new List<string> { refNum },
//                    DateTime.Now.AddHours(-1),
//                    DateTime.Now.AddMinutes(5)
//                );

//                var results = _taxApi.InquiryByReferenceId(inquiryDto);

//                if (results != null && results.Count > 0)
//                {
//                    var status = results[0].Status;
//                    Console.WriteLine($"📊 وضعیت: {status}");

//                    if (status != "NOT_FOUND" && status != "IN_PROGRESS")
//                    {
//                        Console.WriteLine($"✅ پردازش تکمیل شد!");
//                        return results[0];
//                    }
//                    else if (status == "IN_PROGRESS")
//                    {
//                        Console.WriteLine($"⏳ هنوز در حال پردازش...");
//                    }
//                    else
//                    {
//                        Console.WriteLine($"⚠️ NOT_FOUND در تلاش {i + 1}");
//                    }
//                }
//            }

//            Console.WriteLine("\n❌ پس از همه تلاش‌ها فاکتور پیدا نشد");
//            return null;
//        }
//    }
//}