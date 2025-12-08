using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class SubSetting
    {
        public int fldID { get; set; } 
        public string fldDesc { get; set; }
        public DateTime fldDate { get; set; }
        public bool fldCalcFromVariz { get; set; }
        public string fldAzAkharinTarikh { get; set; }
        public byte   fldStartCodeBillIdentity { get; set; }
        public byte  fldRoundID { get; set; }
        public Boolean fldPrintBill_Payment { get; set; }
        public Boolean fldExemptNewProduction { get; set; }

        public string  fldTitleUserReport { get; set; }
        public int fldLastRespitePayment { get; set; }
        public int fldTypeCountryDivisions { get; set; }
        public int fldCodeCountryDivisions { get; set; }

        public string fldImplementationDate { get; set; }
        public Boolean fldCountryDivisionsTreeApply { get; set; }
        public int fldTypeCar { get; set; }
        public int fldCodeCar { get; set; }
        public Boolean fldCarSeriesTreeApply { get; set; }
        public int? fldDefaultPelakSerial { get; set; }
        public int? fldDefaultPelakChar { get; set; }
        public byte fldDefaultSearch { get; set; }
        public Boolean fldHaveScan { get; set; }
        public bool fldMobileVerify { get; set; }
        public bool fldExpScan { get; set; }
    }
}