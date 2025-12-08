using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Avarez
{
    /// <summary>
    /// Summary description for SendToSuporter
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SendToSuporter : System.Web.Services.WebService
    {

        [WebMethod]
        public int InsertInSupport(string Year, int? fldCarClassId, int? fldCarModelId, int? fldCarSystemId, int? fldCabinTypeId, int? fldCarAccountTypeId, int? fldCarMakeId,string Desc)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
           // var y=Year.Split(',');
            //for (int i = 0; i < y.Count(); i++)
            //{
            p.sp_SupportRateInsert(id, fldCarClassId, fldCarModelId, fldCarSystemId, fldCabinTypeId, fldCarAccountTypeId, fldCarMakeId, Year, false, 1, Desc); 
            //}           
            return Convert.ToInt32(id.Value);
        }

        [WebMethod]
        public bool insertOffice(string fldCodeDaftar, string fldAddress, int fldMunId, long? fldLocalId, long? fldAreaId, string fldTel, string expire)
        {
            Models.cartaxEntities Car = new Models.cartaxEntities();
            Car.sp_OfficesInsert("کد" + fldCodeDaftar, fldAddress, 1, fldMunId,
                                    fldLocalId, fldAreaId, 1, "", fldTel, "", expire);
            return true;
        }
    }
}
