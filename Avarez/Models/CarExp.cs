using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class CarExp
    {
        public int fldID { get; set; }
        public long fldCarFileID { get; set; }
        public string fldStartDate { get; set; }
        public string fldEndDate { get; set; }
        public int fldMunicipalityID { get; set; }
        public string fldLetterNumber { get; set; }
        public long fldUserID { get; set; }
        public string fldDesc { get; set; }
        public string fldUserPass { get; set; }
        public int fldFromYear { get; set; }
        public int fldToYear { get; set; }
        public int? fldFileId { get; set; }
    }
}