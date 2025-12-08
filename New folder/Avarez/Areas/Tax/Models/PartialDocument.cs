using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Data.SqlClient;

namespace Avarez.Areas.Tax.Models
{
    public partial class cartaxtest2Entities
    {
       
        public virtual Int64  prs_tblSooratHesab_HeaderInsert(ObjectParameter fldid, string fldTaxId, Nullable<System.DateTime> fldIndatim, Nullable<System.DateTime> fldIndati2m, Nullable<byte> fldInty, Nullable<byte> fldInp, string fldInno, string fldIrtaxId, Nullable<long> fldKharidarId, Nullable<long> fldForooshandeId, System.Data.DataTable Value, Nullable<long> fldUserId)
        {

            var fldidParameter = new System.Data.SqlClient.SqlParameter();
            fldidParameter.SqlDbType = System.Data.SqlDbType.BigInt;
            fldidParameter.ParameterName = "@fldid";
            fldidParameter.Direction = System.Data.ParameterDirection.Output;

            var fldTaxIdParameter = fldTaxId != null ?
                new SqlParameter("fldTaxId", fldTaxId) :
                new SqlParameter("fldTaxId", typeof(string));

            var fldIndatimParameter = fldIndatim.HasValue ?
                new SqlParameter("fldIndatim", fldIndatim) :
                new SqlParameter("fldIndatim", typeof(System.DateTime));

            var fldIndati2mParameter = fldIndati2m.HasValue ?
                new SqlParameter("fldIndati2m", fldIndati2m) :
                new SqlParameter("fldIndati2m", DBNull.Value);

            var fldIntyParameter = fldInty.HasValue ?
                new SqlParameter("fldInty", fldInty) :
                new SqlParameter("fldInty", typeof(byte));

            var fldInpParameter = fldInp.HasValue ?
                new SqlParameter("fldInp", fldInp) :
                new SqlParameter("fldInp", typeof(byte));

            var fldInnoParameter = fldInno != null ?
                new SqlParameter("fldInno", fldInno) :
                new SqlParameter("fldInno", DBNull.Value);

            var fldIrtaxIdParameter = fldIrtaxId != null ?
                new SqlParameter("fldIrtaxId", fldIrtaxId) :
                new SqlParameter("fldIrtaxId", DBNull.Value);

            var fldKharidarIdParameter = fldKharidarId.HasValue ?
                new SqlParameter("fldKharidarId", fldKharidarId) :
                new SqlParameter("fldKharidarId", typeof(long));

            var fldForooshandeIdParameter = fldForooshandeId.HasValue ?
                new SqlParameter("fldForooshandeId", fldForooshandeId) :
                new SqlParameter("fldForooshandeId", typeof(long));
            
            var ValueParameter = Value.Rows.Count > 0 ?
                new System.Data.SqlClient.SqlParameter("Value", Value) :
                new System.Data.SqlClient.SqlParameter("Value", typeof(System.Data.DataTable));
            ValueParameter.TypeName = "Movadi.ParametrValue";
            ValueParameter.SqlDbType = System.Data.SqlDbType.Structured;

            var fldUserIdParameter = fldUserId.HasValue ?
                new SqlParameter("fldUserId", fldUserId) :
                new SqlParameter("fldUserId", typeof(long));

            ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("movadi.prs_tblSooratHesab_HeaderInsert @fldid OUT,@fldTaxId,@fldIndatim,@fldIndati2m,@fldInty,@fldInp,@fldInno,@fldIrtaxId,@fldKharidarId,@fldForooshandeId,@Value,@fldUserId",
               fldidParameter, fldTaxIdParameter, fldIndatimParameter, fldIndati2mParameter, fldIntyParameter, fldInpParameter, fldInnoParameter, fldIrtaxIdParameter, fldKharidarIdParameter, fldForooshandeIdParameter, ValueParameter, fldUserIdParameter);
            return Convert.ToInt64(fldidParameter.Value);

        }
        public virtual int prs_tblSooratHesab_DetailInsert(Nullable<long> fldHeaderId, System.Data.DataTable Value, Nullable<long> fldUserID)
        {
            var fldHeaderIdParameter = fldHeaderId.HasValue ?
                new SqlParameter("fldHeaderId", fldHeaderId) :
                new SqlParameter("fldHeaderId", typeof(long));

            var fldUserIDParameter = fldUserID.HasValue ?
                new SqlParameter("fldUserID", fldUserID) :
                new SqlParameter("fldUserID", typeof(long));

            var ValueParameter = Value.Rows.Count > 0 ?
                new System.Data.SqlClient.SqlParameter("Value", Value) :
                new System.Data.SqlClient.SqlParameter("Value", typeof(System.Data.DataTable));
            ValueParameter.TypeName = "movadi.ParametrValue";
            ValueParameter.SqlDbType = System.Data.SqlDbType.Structured;

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("movadi.prs_tblSooratHesab_DetailInsert @fldHeaderId,@Value,@fldUserID"
                , fldHeaderIdParameter, ValueParameter, fldUserIDParameter);
        }
    }
}