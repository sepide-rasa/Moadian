using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class ShorTerm
    {
        public int fldID { get; set; }
        public string fldEnglishName { get; set; }
        public string fldPersianName { get; set; }
        public string fldSymbol { get; set; }
        public string fldImage { get; set; }
        public int fldUserID { get; set; }
        public string fldDesc { get; set; }
        public DateTime fldDate { get; set; }
    }
}