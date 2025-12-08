using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class FinesRules
    {
        public  int fldID { get; set; }
        public  string  fldName { get; set; }
        public  string  fldImplementationDate { get; set; }
        public int fldPercentFine { get; set; }
        public  int fldUserID { get; set; }
        public  string  fldDesc { get; set; }
        public  DateTime  fldDate { get; set; }

    }
}