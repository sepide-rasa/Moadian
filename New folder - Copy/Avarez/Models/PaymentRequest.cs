using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class PaymentRequest
    {
        public PaymentRequest()
        {
            MultiplexingData = new MultiplexingData();
        }
        public string TerminalId { get; set; }
        public string MerchantId { get; set; }
        public long Amount { get; set; }
        public string OrderId { get; set; }
        public string AdditionalData { get; set; }
        public DateTime LocalDateTime { get; set; }
        public string ReturnUrl { get; set; }
        public string SignData { get; set; }
        public bool EnableMultiplexing { get; set; }
        public MultiplexingData MultiplexingData { get; set; }
        public string MerchantKey { get; set; }
        public string PurchasePage { get; set; }
    }
}