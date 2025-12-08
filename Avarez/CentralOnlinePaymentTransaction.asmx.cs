using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;

namespace Avarez
{
    /// <summary>
    /// Summary description for CentralOnlinePaymentTransaction
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CentralOnlinePaymentTransaction : System.Web.Services.WebService
    {
        [WebMethod]
        public string GetTransactionId(int Mablagh,string Type)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            System.Data.Entity.Core.Objects.ObjectParameter fldCodeTarakonesh = new System.Data.Entity.Core.Objects.ObjectParameter("fldCodeTarakonesh", typeof(string));
            p.sp_WebTransactionInsert(Mablagh, fldCodeTarakonesh, Type);
            return (fldCodeTarakonesh.Value).ToString();
        }

        [WebMethod]
        public int GetFactorId(int Mablagh, string Type,int Year)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            System.Data.Entity.Core.Objects.ObjectParameter order = new System.Data.Entity.Core.Objects.ObjectParameter("fldOrder", typeof(string));
            p.Sp_tblWebServiceFactorInsert(Mablagh, Type, (short)Year, order);
            return Convert.ToInt32(order.Value);
        }

        [WebMethod]
        public int GetSamanOrderId(int Mablagh, string MunName)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            System.Data.Entity.Core.Objects.ObjectParameter order = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(string));
            p.sp_tblCentralSamanOrdersInsert(order,Mablagh, MunName);
            return Convert.ToInt32(order.Value);
        }

        [WebMethod]
        public bool UpdateSamanRefNum(int Id, string RefNum)
        {
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                p.sp_tblCentralSamanOrdersUpdate_RefNum(Id, RefNum);
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        [WebMethod]
        public bool ExistRefNum(string RefNum)
        {
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.sp_tblCentralSamanOrdersSelect("fldRefNum", RefNum, 1).Any();
                return q;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
