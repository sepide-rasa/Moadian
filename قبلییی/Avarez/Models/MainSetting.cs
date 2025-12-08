using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class MainSetting
    {
        public int fldID { get; set; }
        public string fldDesc { get; set; }
        public DateTime fldDate { get; set; }

        public Boolean fldLateFine { get; set; }
        public Boolean fldTax { get; set; }
        public int fldTypeCountryDivisions { get; set; }
        public int fldCodeCountryDivisions { get; set; }
        public string  fldImplementationDate { get; set; }
        public Boolean fldCountryDivisionsTreeApply { get; set; }
        public int fldTypeCar { get; set; }
        public int fldCodeCar { get; set; }
        public Boolean fldCarSeriesTreeApply { get; set; }
        public Boolean fldFineType { get; set; }
        public Boolean fldFirstInsurance { get; set; }
    }
}