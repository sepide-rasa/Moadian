using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class SepTxn
    {
        public string Action { get; set; }
        public long Amount { get; set; }
        public string Wage { get; set; }
        public string TerminalId { get; set; }
        public string ResNum { get; set; }
        public string RedirectURL { get; set; }
        //public string FinalBackUrl { get; set; }
        public long CellNumber { get; set; }
        public IBANKInfo[] SettlementIBANInfo { get; set; }
        public string TokenExpiryInMin { get; set; }
        public string HashedCardNumber { get; set; }
        //public string MerchantCustomNameIdentifier { get; set; }
    }
    public class IBANKInfo
    {
        public string IBAN { get; set; }
        public long Amount { get; set; }
        public string PurchaseId { get; set; }
    }
    public class IPGOutputModel
    {
        [JsonProperty("TransactionDetail")]
        public VerifyInfo VerifyInfo { get; set; }
        public int ResultCode { get; set; }
        public string ResultDescription { get; set; }
        public bool Success { get; set; }
    }
    public class IPGInputModel
    {
        public string RefNum { get; set; }
        public int TerminalNumber { get; set; }
        public int TxnRandomSessionKey { get; set; }
        public string CellNumber { get; set; }
        public bool IgnoreNationalcode { get; set; }
    }
    public class TokenInf
    {
        public string status { get; set; }
        public string errorCode { get; set; }
        public string errorDesc { get; set; }
        public string token { get; set; }
    }
    public class VerifyInfo
    {
        public string RRN { get; set; }
        public string RefNum { get; set; }
        public string ResNum { get; set; }
        public string MaskedPan { get; set; }
        public string HashedPan { get; set; }
        public int TerminalNumber { get; set; }
        public int OrginalAmount { get; set; }
        public int AffectiveAmount { get; set; }
        public string StraceDate { get; set; }
        public string StraceNo { get; set; }
    }
}