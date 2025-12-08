using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Areas.Tax.Models
{
    public class ObjHeaderSooratHesab
    {
        public string FishId { get; set; }
        public string Kh_Name { get; set; }
        public string Kh_Family { get; set; }
        public string bid { get; set; }
        public System.DateTime indatim { get; set; }
        public Nullable<System.DateTime> indati2m { get; set; }
        public string irtaxid { get; set; }
        public Nullable<byte> ins { get; set; }
        public string tinb { get; set; }
        public Nullable<byte> tob { get; set; }
        public Nullable<long> tprdis { get; set; }
        public Nullable<long> tadis { get; set; }
        public Nullable<long> tvam { get; set; }
        public Nullable<long> todam { get; set; }
        public Nullable<byte> setm { get; set; }
        public Nullable<long> cap { get; set; }
        public Nullable<long> insp { get; set; }

        public Nullable<long> tdis { get; set; }
        public Nullable<long> tbill { get; set; }
        public Nullable<decimal> tonw { get; set; }
        public Nullable<long> torv { get; set; }
        public Nullable<decimal> tocv { get; set; }
        public Nullable<long> tvop { get; set; }

    }
}