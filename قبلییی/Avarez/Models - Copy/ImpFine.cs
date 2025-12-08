using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class ImpFine
    {
        public int fldID { get; set; }
        public string fldDesc { get; set; }
        public DateTime fldDate { get; set; }
        public int fldFinesRuleID { get; set; }
        public int fldTypeCountryDivisions { get; set; }
        public int fldCodeCountryDivisions { get; set; }
        public Boolean fldCountryDivisionsTreeApply { get; set; }
        public int fldTypeCar { get; set; }
        public int fldCodeCar { get; set; }
        public Boolean fldCarSeriesTreeApply { get; set; }
    }
}