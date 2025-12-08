using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class Pattern
    {
        public int fldID { get; set; }
        public string fldDesc { get; set; }
        public int fldTypeCountryDivisions { get; set; }
        public int fldCodeCountryDivisions { get; set; }
        public DateTime fldDate { get; set; }
        public string fldPattern { get; set; }
    }
}