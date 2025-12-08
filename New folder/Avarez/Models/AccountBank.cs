using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class AccountBank
    {
        public int fldID { get; set; }
        public string fldDesc { get; set; }
        public int fldTypeCountryDivisions { get; set; }
        public int fldCodeCountryDivisions { get; set; }
        public int fldReportsID { get; set; }
        public DateTime fldDate { get; set; }
        public string fldAccountNumber { get; set; }
        public int fldBranchID { get; set; }
        public int fldTypeLogID { get; set; }
    }
}