using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class Parvande
    {
        public IEnumerable<Models.sp_OwnerSelect> Owner { get; set; }
        public IEnumerable<Models.sp_CarPlaqueSelect> CarPlaque { get; set; }
        public IEnumerable<Models.sp_CarFileSelect> CarFile { get; set; }
    }
}