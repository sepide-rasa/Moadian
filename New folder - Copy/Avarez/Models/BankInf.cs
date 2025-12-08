using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class BankInf
    {
        public int fldID { get; set; }
        public int fldMunID { get; set; }
        public string fldValue { get; set; }
        public int fldParametrID { get; set; }
        public int fldUserID { get; set; }
        public string fldDesc { get; set; }
        public DateTime fldDate { get; set; }
        public int fldBankId { get; set; }
        public int? fldType { get; set; }
        public int? fldDivisionID { get; set; }
    }
}