using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class DegreeMun
    {
        public int fldID { get; set; }
        public byte  fldDegree { get; set; }
        public string fldDateDegree { get; set; }
        public int fldMunicipalityID { get; set; }
        public int fldUserID { get; set; }
        public string fldDesc { get; set; }
        public DateTime fldDate { get; set; }
    }
}