using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class clsEstelam
    {
        public IEnumerable<Pay> Pay { get; set; }
        public IEnumerable<Exprience> Exp { get; set; }
        public IEnumerable<Mafasa> Mafasa { get; set; }
    }
}