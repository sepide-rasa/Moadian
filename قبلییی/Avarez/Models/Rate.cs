using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class Rate
    {
        public int fldId { get; set; }
        public int fldTypeCar { set; get; }
        public int fldCodeCar { set; get; }
        public int fldTypeCountryDivisions { set; get; }
        public int fldCodeCountryDivisions { set; get; }
        public short fldYear { set; get; }
        public byte fldFromCylinder { set; get; }
        public byte fldToCylinder { set; get; }
        public byte fldFromWheel { set; get; }
        public byte fldToWheel { set; get; }
        public short fldFromModel { set; get; }
        public short fldToModel { set; get; }
        public int fldFromContentMotor { set; get; }
        public int fldToContentMotor { set; get; }
        public int fldPrice { set; get; }
        public long fldUserID { set; get; }
        public string fldDesc { set; get; }
        public string fldUserPass { set; get; }
    }
}