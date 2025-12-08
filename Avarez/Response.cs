using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez
{
    public class Response
    {
        public Boolean Success;
        public byte ErrorCode = 0; public byte WarningCode = 0;
        public pcPosTransaction TransactionInfo = null;
        public PosInfo PosInformation = null;
        public string AdditionalData = "";
    }
    public class pcPosTransaction
    {
        public String PAN = "";// شماره کارت بانکی
        public String SVC = "";// شماره شناسایی در سمت بانک
        public String Stan = "";// شماره تراکنش در ترمینال
        public String RRN = "";//شماره ارجاع تراکنش
        public String Amount = "";//مبلغ تراکنش
        public String DateTime = "";//زمان انجام تراکنش
        public String ResponseCode = "";// وضعیت تراکنش
        public string EntryType { get; set; }// روش ورود داده
    }
    public class PosInfo
    {
        public String TerminalId = "";// شماره ترمینال
        public String SerialNumber = "";
        public String MerchantId = "";// شماره دارنده ترمینال
        public String MerchantName { get; set; }// نام و مشخصات دارنده ترمینال
    }
}