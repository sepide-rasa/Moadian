using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class Car
    {
        public int fldID { get; set; }
        public int fldUserID { get; set; }
        public string fldDesc { get; set; }
        public DateTime fldDate { get; set; }
        public string fldMotorNumber { get; set; }
        public string fldShasiNumber{ get; set; }
        public string fldVIN{ get; set; }
        public int fldCarModelID{ get; set; }
        public int fldCarClassID{ get; set; }
        public int fldCarColorID{ get; set; }
        public short fldModel{ get; set; }
        public string fldStartDateInsurance { get; set; }

    }
}