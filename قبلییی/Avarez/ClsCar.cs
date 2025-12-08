using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez
{
    public class clsCar
    {
        public string CarType { get; set; }
        public short Model { get; set; }
        public string ChasiNum { get; set; }
        public string MotorNum { get; set; }
        public string VIN { get; set; }
        public List<Pay> Pay { get; set; }
        public List<Exprience> Exp { get; set; }
        public List<Mafasa> Mafasa { get; set; }
    }

    public class Pay
    {
        public string PayDate { get; set; }
        public int Price { get; set; }
        public string MunName { get; set; }
    }

    public class Exprience
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string MunName { get; set; }
        public byte[] Image { get; set; }
    }
    public class Mafasa
    {
        public string Date { get; set; }
        public string MunName { get; set; }
        public byte[] Image { get; set; }
        public long RefCode { get; set; }
    }
}