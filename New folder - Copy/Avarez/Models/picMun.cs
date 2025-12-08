using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class picMun
    {
        public int fldID { get; set; }
        public string  fldName { get; set; }
        public Boolean fldShowStatus { get; set; }
        public int fldMunicipalityID { get; set; }
        public int fldUserID { get; set; }
        public string  fldDesc{ get; set; }
        public string fldImage { get; set; }
        public DateTime fldDate { get; set; }
    }
}