using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class SelectFullCar
    {
        public IEnumerable<Models.sp_SelectFullCarForSetMony> SetMony { get; set; }
        public IEnumerable<Models.sp_SelectFullCarForSetMonyFullNotNull> SetMonyFullNotNull { get; set; }
    }
}