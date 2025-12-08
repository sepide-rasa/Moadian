using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class SmsSetting
    {
        public int fldID { get; set; }
        public string fldUserName { get; set; }
        public string fldPassword { get; set; }
        public string fldLineNumber { get; set; }
        public string fldDesc { get; set; }
        public int fldCountryType { get; set; }
        public int fldCountryCode { get; set; }
    }
}