using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Areas.Tax.Models
{
    public class ObjDetailSooratHesab
    {
        public string sstid { get; set; }
        public string sstt { get; set; }
        public Nullable<decimal> am { get; set; }
        public Nullable<decimal> fee { get; set; }
        public Nullable<long> dis { get; set; }
        public Nullable<decimal> vra { get; set; }
        public string odt { get; set; }
        public Nullable<decimal> odr { get; set; }
        public string olt { get; set; }
        public Nullable<decimal> olr { get; set; }

                     
        public Nullable<long> prdis { get; set; }
        public Nullable<long> adis { get; set; }
        public Nullable<long> vam { get; set; }
        public Nullable<long> odam { get; set; }
        public Nullable<long> olam { get; set; }
        public Nullable<long> tsstam { get; set; }
        public Nullable<long> ssrv { get; set; }
        public Nullable<long> vop { get; set; }
        public Nullable<decimal> nw { get; set; }
        public Nullable<decimal> sscv { get; set; }
    }
}