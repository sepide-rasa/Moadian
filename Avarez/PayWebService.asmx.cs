using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace Avarez
{
    /// <summary>
    /// Summary description for PayWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PayWebService : System.Web.Services.WebService
    {
        [WebMethod]
        public bool PayVrify(string ShGhabz,string ShPardakht,string ShamsiDate,string CodePeigiry,string UserName,string Password,int MunId)
        {
            if (UserName == "kashan" && Password == "#Sh1394Kashan#")
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.sp_GetAllShenaseGhabzInfo(MunId).ToList();
                string shGhabz = ShGhabz.TrimStart('0');
                string shPardakht = ShPardakht.TrimStart('0');
                var isValid = q.Where(kk => kk.start == Convert.ToInt16(shGhabz.Substring(0, 2))).FirstOrDefault();
                if (isValid == null)
                    return false;
                var divisionid = p.sp_GET_IDCountryDivisions(Convert.ToInt32(isValid.type), Convert.ToInt32(isValid.fldid)).FirstOrDefault();
                string date1 = ShamsiDate;
                int type1 = 3;//اینترانت
                int count = Convert.ToInt32(shPardakht.Substring(shPardakht.Length - 5, 1));
                int carfile = Convert.ToInt32(shGhabz.Substring(0, shGhabz.Length - 5).Substring(2));
                if (count > 6 && count < 9)
                    carfile = Convert.ToInt32(carfile.ToString() + shPardakht.Substring(shPardakht.Length - 4, 8 - count));
                var SeettelType = p.sp_SettleTypeSelect("fldid", type1.ToString(), 0, 1, "").FirstOrDefault();
                int price = Convert.ToInt32(shPardakht.Substring(0, shPardakht.Length - 5)) * 1000;
                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                p.sp_PardakhtFileInsert(_id, 20, "اینترنتی",
                               MyLib.Shamsi.Shamsi2miladiDateTime(ShamsiDate), 20, "");

                if (shGhabz.Substring(0, 2) == isValid.start.ToString() &&
                    shGhabz.Substring(shGhabz.Length - 2, 1) == isValid.fldServiceCode.ToString() &&
                    shGhabz.Substring(shGhabz.Length - 5, 3) == isValid.fldInformaticesCode)
                {
                    var carfileId = p.sp_SingleCarfileSelect(carfile).FirstOrDefault();

                    if (carfileId != null)
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter c_id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                        System.Data.Entity.Core.Objects.ObjectParameter p_id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));

                        p.sp_PardakhtFiles_DetailInsert(p_id, shGhabz, shPardakht,
                            MyLib.Shamsi.Shamsi2miladiDateTime(ShamsiDate),
                            CodePeigiry, SeettelType.fldID, Convert.ToInt32(_id.Value), Convert.ToInt32(divisionid.CountryDivisionId), 20, "");
                        var collection = p.sp_SingleCollectionSelect(carfile, MyLib.Shamsi.Shamsi2miladiDateTime(ShamsiDate), price).FirstOrDefault();
                        if (collection == null)
                        {
                            p.sp_CollectionInsert(c_id, carfileId.fldID, MyLib.Shamsi.Shamsi2miladiDateTime(ShamsiDate),
                                price, SeettelType.fldID, null, null, "", 20,
                                "پرداخت از طریق شناسه قبض و شناسه پرداخت(اینترنتی)", "", "", null,
                                "", Convert.ToInt32(p_id.Value), null, false, null, null);
                            SmsSender sms = new SmsSender();
                            sms.SendMessage(MunId, 3, carfileId.fldID, price, "", "", "");
                        }
                    }
                }
                return true;
            }
            else 
                return false;
        }
    }
}
