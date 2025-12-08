using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class MyTablighat
    {
        string desc = "";
        
        public MyTablighat(string munID)
        {
            Models.cartaxEntities c = new cartaxEntities();
            var q = c.sp_MunicipalitySelect("fldid", munID, 0, 1, "").FirstOrDefault();
            if (q != null)
                desc = q.fldDesc.ToString();

            Matn = desc + "\n" + "طراحی و توسعه: شرکت رسا سیستم البرز-آدرس وب سایت:www.rasa-system.com-تلفن: 02332336765 داخلی 104و103";
            //Matn = "استانداری تهران-دفتر فناوری اطلاعات و ارتباطات";
        }
        public string Matn = "";
    }
}