using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class Facture_Guest
    {
        public IEnumerable<Models.prs_newCarFileCalc> jCalcCarFile { get; set; }
        public IEnumerable<Models.sp_CarExperienceSelect> CarExperience { get; set; }
        public IEnumerable<Models.rpt_Receipt> Receipt { get; set; }
    }
}