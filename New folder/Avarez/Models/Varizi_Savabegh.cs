using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class Varizi_Savabegh
    {
        public IEnumerable<Models.rpt_Receipt> Varizi { get; set; }
        public IEnumerable<Models.sp_CarExperienceSelect> Savabegh { get; set; }
        public IEnumerable<Models.sp_ListeSiyahSelect> BlackList { get; set; }
        public IEnumerable<Models.sp_CarFileSelect> CarFileOwner { get; set; }
        public IEnumerable<Models.prs_newCarFileCalc> CarFileFish { get; set; }
        public IEnumerable<Models.sp_PeacockerySelect> Peacockery { get; set; }
    }
}