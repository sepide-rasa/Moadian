using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class ModelsofBankInf
    {
        public IEnumerable<Models.sp_SelectNameBankAndMunForBankInformation> BankInf { get; set; }
        public IEnumerable<Models.sp_BankParameterSelect> BankInfDetails { get; set; }
    }
}