using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class Discount
    {
        public int fldID { get; set; }
        public string fldDesc { get; set; }
        public DateTime fldDate { get; set; }

        public string fldName { get; set; }
        public string fldStartDate{ get; set; }
        public string fldDateOf { get; set; }
        public string fldEndDate { get; set; }
        public Boolean  fldComplicationPrice { get; set; }
        public Boolean fldFinePrice { get; set; }
        public Boolean fldValueAddedPrice { get; set; }
        public Boolean fldOtherPrice { get; set; }
        public int fldPercentDiscount { get; set; }


        public int fldTypeCountryDivisions { get; set; }
        public int fldCodeCountryDivisions { get; set; }
        public string fldImplementationDate { get; set; }
        public Boolean fldCountryDivisionsTreeApply { get; set; }
        public int fldTypeCar { get; set; }
        public int fldCodeCar { get; set; }
        public Boolean fldCarSeriesTreeApply { get; set; }

        public Boolean fldEffectiveUser{ get; set; }
        public Boolean fldEffectiveOffice{ get; set; }
        public Boolean fldEffectiveMunicipality { get; set; }
    }
}