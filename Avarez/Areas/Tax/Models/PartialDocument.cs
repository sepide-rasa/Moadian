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
       
        public virtual Int64  prs_tblSooratHesab_HeaderInsert(ObjectParameter fldid, string fldTaxId, Nullable<System.DateTime> fldIndatim, Nullable<System.DateTime> fldIndati2m, Nullable<byte> fldInty, Nullable<byte> fldInp, string fldInno, string fldIrtaxId, Nullable<long> fldKharidarId, Nullable<long> fldForooshandeId,
            string fldFunctionName, string fldkh_Pas_No, Nullable<byte> fldkh_TypeFly, string fldF_Sh_Parvane, string fldF_GomrokCode, string fldF_UniqId,
            System.Data.DataTable Parametrss,string fldShomareFish, Nullable<long> fldUserId, string fldIP)
        {

            var fldidParameter = new System.Data.SqlClient.SqlParameter();
            fldidParameter.SqlDbType = System.Data.SqlDbType.BigInt;
            fldidParameter.ParameterName = "@fldid";
            fldidParameter.Direction = System.Data.ParameterDirection.Output;

            var fldTaxIdParameter = fldTaxId != null ?
                new System.Data.SqlClient.SqlParameter("fldTaxId", fldTaxId) :
                new System.Data.SqlClient.SqlParameter("fldTaxId", typeof(string));

            var fldIndatimParameter = fldIndatim.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldIndatim", fldIndatim) :
                new System.Data.SqlClient.SqlParameter("fldIndatim", typeof(System.DateTime));
            
            var fldIndati2mParameter = fldIndati2m.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldIndati2m", fldIndati2m) :
                new System.Data.SqlClient.SqlParameter("fldIndati2m", DBNull.Value);

            var fldIntyParameter = fldInty.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldInty", fldInty) :
                new System.Data.SqlClient.SqlParameter("fldInty", typeof(byte));

            var fldInpParameter = fldInp.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldInp", fldInp) :
                new System.Data.SqlClient.SqlParameter("fldInp", typeof(byte));

            var fldInnoParameter = fldInno != null ?
                new System.Data.SqlClient.SqlParameter("fldInno", fldInno) :
                new System.Data.SqlClient.SqlParameter("fldInno", DBNull.Value);

            var fldIrtaxIdParameter = fldIrtaxId != null ?
                new System.Data.SqlClient.SqlParameter("fldIrtaxId", fldIrtaxId) :
                new System.Data.SqlClient.SqlParameter("fldIrtaxId", DBNull.Value);

            var fldKharidarIdParameter = fldKharidarId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldKharidarId", fldKharidarId) :
                new System.Data.SqlClient.SqlParameter("fldKharidarId", typeof(long));

            var fldForooshandeIdParameter = fldForooshandeId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldForooshandeId", fldForooshandeId) :
                new System.Data.SqlClient.SqlParameter("fldForooshandeId", typeof(long));

            var fldFunctionNameParameter = fldFunctionName != null ?
                new System.Data.SqlClient.SqlParameter("fldFunctionName", fldFunctionName) :
                new System.Data.SqlClient.SqlParameter("fldFunctionName", typeof(string));

            var fldkh_Pas_NoParameter = fldkh_Pas_No != null ?
                   new System.Data.SqlClient.SqlParameter("fldkh_Pas_No", fldkh_Pas_No) :
                   new System.Data.SqlClient.SqlParameter("fldkh_Pas_No", DBNull.Value);

            var fldkh_TypeFlyParameter = fldkh_TypeFly.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldkh_TypeFly", fldkh_TypeFly) :
                new System.Data.SqlClient.SqlParameter("fldkh_TypeFly", DBNull.Value);

            var fldF_Sh_ParvaneParameter = fldF_Sh_Parvane != null ?
                new System.Data.SqlClient.SqlParameter("fldF_Sh_Parvane", fldF_Sh_Parvane) :
                new System.Data.SqlClient.SqlParameter("fldF_Sh_Parvane", DBNull.Value);

            var fldF_GomrokCodeParameter = fldF_GomrokCode != null ?
                new System.Data.SqlClient.SqlParameter("fldF_GomrokCode", fldF_GomrokCode) :
                new System.Data.SqlClient.SqlParameter("fldF_GomrokCode", DBNull.Value);

            var fldF_UniqIdParameter = fldF_UniqId != null ?
                new System.Data.SqlClient.SqlParameter("fldF_UniqId", fldF_UniqId) :
                new System.Data.SqlClient.SqlParameter("fldF_UniqId", DBNull.Value);

            var ParametrsParameter = Parametrss.Rows.Count > 0 ?
                new System.Data.SqlClient.SqlParameter("Parametrs", Parametrss) :
                new System.Data.SqlClient.SqlParameter("Parametrs", typeof(System.Data.DataTable));
            ParametrsParameter.TypeName = "movadi.ParametrValue";
            ParametrsParameter.SqlDbType = System.Data.SqlDbType.Structured;


            var fldUserIdParameter = fldUserId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldUserId", fldUserId) :
                new System.Data.SqlClient.SqlParameter("fldUserId", typeof(long));

            var fldShomareFishParameter = fldShomareFish != null ?
                new System.Data.SqlClient.SqlParameter("fldShomareFish", fldShomareFish) :
                new System.Data.SqlClient.SqlParameter("fldShomareFish", DBNull.Value);

            var fldIPParameter = fldIP != null ?
                new System.Data.SqlClient.SqlParameter("fldIP", fldIP) :
                new System.Data.SqlClient.SqlParameter("fldIP", DBNull.Value);

            ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("Movadi.prs_tblSooratHesab_HeaderInsert @fldid OUT,@fldTaxId,@fldIndatim,@fldIndati2m,@fldInty,@fldInp,@fldInno,@fldIrtaxId,@fldKharidarId,@fldForooshandeId,@fldFunctionName,@fldkh_Pas_No,@fldkh_TypeFly,@fldF_Sh_Parvane,@fldF_GomrokCode,@fldF_UniqId,@Parametrs,@fldShomareFish,@fldUserId,@fldIP",
               fldidParameter, fldTaxIdParameter, fldIndatimParameter, fldIndati2mParameter, fldIntyParameter, fldInpParameter, fldInnoParameter, fldIrtaxIdParameter, fldKharidarIdParameter, fldForooshandeIdParameter, fldFunctionNameParameter, fldkh_Pas_NoParameter, fldkh_TypeFlyParameter, fldF_Sh_ParvaneParameter, fldF_GomrokCodeParameter, fldF_UniqIdParameter, ParametrsParameter, fldShomareFishParameter,  fldUserIdParameter, fldIPParameter);
            return Convert.ToInt64(fldidParameter.Value);
        }
        public virtual int prs_tblSooratHesab_HeaderUpdate(Nullable<long> fldid, string fldTaxId, Nullable<System.DateTime> fldIndatim, Nullable<System.DateTime> fldIndati2m, Nullable<byte> fldInty, 
            Nullable<byte> fldInp, string fldInno, string fldIrtaxId, Nullable<long> fldKharidarId, Nullable<long> fldForooshandeId, string fldFunctionName, string fldkh_Pas_No,
            Nullable<byte> fldkh_TypeFly, string fldF_Sh_Parvane, string fldF_GomrokCode, string fldF_UniqId, System.Data.DataTable Parametrss, string fldShomareFish, Nullable<long> fldUserId, string fldIP)
        {
            var fldidParameter = fldid.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldid", fldid) :
                new System.Data.SqlClient.SqlParameter("fldid", typeof(long));

            var fldTaxIdParameter = fldTaxId != null ?
                new System.Data.SqlClient.SqlParameter("fldTaxId", fldTaxId) :
                new System.Data.SqlClient.SqlParameter("fldTaxId", typeof(string));

            var fldIndatimParameter = fldIndatim.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldIndatim", fldIndatim) :
                new System.Data.SqlClient.SqlParameter("fldIndatim", typeof(System.DateTime));

            var fldIndati2mParameter = fldIndati2m.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldIndati2m", fldIndati2m) :
                new System.Data.SqlClient.SqlParameter("fldIndati2m", DBNull.Value);

            var fldIntyParameter = fldInty.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldInty", fldInty) :
                new System.Data.SqlClient.SqlParameter("fldInty", typeof(byte));

            var fldInpParameter = fldInp.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldInp", fldInp) :
                new System.Data.SqlClient.SqlParameter("fldInp", typeof(byte));

            var fldInnoParameter = fldInno != null ?
                new System.Data.SqlClient.SqlParameter("fldInno", fldInno) :
                new System.Data.SqlClient.SqlParameter("fldInno", DBNull.Value);

            var fldIrtaxIdParameter = fldIrtaxId != null ?
                new System.Data.SqlClient.SqlParameter("fldIrtaxId", fldIrtaxId) :
                new System.Data.SqlClient.SqlParameter("fldIrtaxId", DBNull.Value);

            var fldKharidarIdParameter = fldKharidarId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldKharidarId", fldKharidarId) :
                new System.Data.SqlClient.SqlParameter("fldKharidarId", typeof(long));

            var fldForooshandeIdParameter = fldForooshandeId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldForooshandeId", fldForooshandeId) :
                new System.Data.SqlClient.SqlParameter("fldForooshandeId", typeof(long));

            var fldFunctionNameParameter = fldFunctionName != null ?
                new System.Data.SqlClient.SqlParameter("fldFunctionName", fldFunctionName) :
                new System.Data.SqlClient.SqlParameter("fldFunctionName", typeof(string));

            var fldkh_Pas_NoParameter = fldkh_Pas_No != null ?
                   new System.Data.SqlClient.SqlParameter("fldkh_Pas_No", fldkh_Pas_No) :
                   new System.Data.SqlClient.SqlParameter("fldkh_Pas_No", DBNull.Value);

            var fldkh_TypeFlyParameter = fldkh_TypeFly.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldkh_TypeFly", fldkh_TypeFly) :
                new System.Data.SqlClient.SqlParameter("fldkh_TypeFly", DBNull.Value);

            var fldF_Sh_ParvaneParameter = fldF_Sh_Parvane != null ?
                new System.Data.SqlClient.SqlParameter("fldF_Sh_Parvane", fldF_Sh_Parvane) :
                new System.Data.SqlClient.SqlParameter("fldF_Sh_Parvane", DBNull.Value);

            var fldF_GomrokCodeParameter = fldF_GomrokCode != null ?
                new System.Data.SqlClient.SqlParameter("fldF_GomrokCode", fldF_GomrokCode) :
                new System.Data.SqlClient.SqlParameter("fldF_GomrokCode", DBNull.Value);

            var fldF_UniqIdParameter = fldF_UniqId != null ?
                new System.Data.SqlClient.SqlParameter("fldF_UniqId", fldF_UniqId) :
                new System.Data.SqlClient.SqlParameter("fldF_UniqId", DBNull.Value);

            var ParametrsParameter = Parametrss.Rows.Count > 0 ?
                new System.Data.SqlClient.SqlParameter("Parametrs", Parametrss) :
                new System.Data.SqlClient.SqlParameter("Parametrs", typeof(System.Data.DataTable));
            ParametrsParameter.TypeName = "movadi.ParametrValue";
            ParametrsParameter.SqlDbType = System.Data.SqlDbType.Structured;


            var fldShomareFishParameter = fldShomareFish != null ?
                new System.Data.SqlClient.SqlParameter("fldShomareFish", fldShomareFish) :
                new System.Data.SqlClient.SqlParameter("fldShomareFish", DBNull.Value);

            var fldUserIdParameter = fldUserId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldUserId", fldUserId) :
                new System.Data.SqlClient.SqlParameter("fldUserId", typeof(long));

            var fldIPParameter = fldIP != null ?
                new System.Data.SqlClient.SqlParameter("fldIP", fldIP) :
                new System.Data.SqlClient.SqlParameter("fldIP", DBNull.Value);



            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("Movadi.prs_tblSooratHesab_HeaderUpdate @fldid,@fldTaxId,@fldIndatim,@fldIndati2m,@fldInty,@fldInp,@fldInno,@fldIrtaxId,@fldKharidarId,@fldForooshandeId,@fldFunctionName,@fldkh_Pas_No,@fldkh_TypeFly,@fldF_Sh_Parvane,@fldF_GomrokCode,@fldF_UniqId,@Parametrs,@fldShomareFish,@fldUserId,@fldIP",
                   fldidParameter, fldTaxIdParameter, fldIndatimParameter, fldIndati2mParameter, fldIntyParameter, fldInpParameter, fldInnoParameter, fldIrtaxIdParameter, fldKharidarIdParameter, fldForooshandeIdParameter, fldFunctionNameParameter, fldkh_Pas_NoParameter, fldkh_TypeFlyParameter, fldF_Sh_ParvaneParameter, fldF_GomrokCodeParameter, fldF_UniqIdParameter, ParametrsParameter, fldShomareFishParameter, fldUserIdParameter, fldIPParameter);

        }
        public virtual int prs_tblSooratHesab_DetailInsert(Nullable<long> fldHeaderId, System.Data.DataTable Value, Nullable<long> fldUserID,string fldIP)
        {
            var fldHeaderIdParameter = fldHeaderId.HasValue ?
                new SqlParameter("fldHeaderId", fldHeaderId) :
                new SqlParameter("fldHeaderId", typeof(long));

            var fldUserIDParameter = fldUserID.HasValue ?
                new SqlParameter("fldUserID", fldUserID) :
                new SqlParameter("fldUserID", typeof(long));

            var fldIPParameter = fldIP != null ?
                new System.Data.SqlClient.SqlParameter("fldIP", fldIP) :
                new System.Data.SqlClient.SqlParameter("fldIP", DBNull.Value);

            var ValueParameter = Value.Rows.Count > 0 ?
                new System.Data.SqlClient.SqlParameter("Value", Value) :
                new System.Data.SqlClient.SqlParameter("Value", typeof(System.Data.DataTable));
            ValueParameter.TypeName = "movadi.ParametrValue";
            ValueParameter.SqlDbType = System.Data.SqlDbType.Structured;

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("movadi.prs_tblSooratHesab_DetailInsert @fldHeaderId,@Value,@fldUserID,@fldIP"
                , fldHeaderIdParameter, ValueParameter, fldUserIDParameter, fldIPParameter);
        }
        public virtual int prs_tblSooratHesab_DetailUpdate(Nullable<long> fldHeaderId, System.Data.DataTable Value, Nullable<long> fldUserID,string fldIP)
        {
            var fldHeaderIdParameter = fldHeaderId.HasValue ?
                new SqlParameter("fldHeaderId", fldHeaderId) :
                new SqlParameter("fldHeaderId", typeof(long));

            var fldUserIDParameter = fldUserID.HasValue ?
                new SqlParameter("fldUserID", fldUserID) :
                new SqlParameter("fldUserID", typeof(long));

            var fldIPParameter = fldIP != null ?
                new System.Data.SqlClient.SqlParameter("fldIP", fldIP) :
                new System.Data.SqlClient.SqlParameter("fldIP", DBNull.Value);

            var ValueParameter = Value.Rows.Count > 0 ?
                new System.Data.SqlClient.SqlParameter("Value", Value) :
                new System.Data.SqlClient.SqlParameter("Value", typeof(System.Data.DataTable));
            ValueParameter.TypeName = "movadi.ParametrValue";
            ValueParameter.SqlDbType = System.Data.SqlDbType.Structured;

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("movadi.prs_tblSooratHesab_DetailUpdate @fldHeaderId,@Value,@fldUserID,@fldIP"
                , fldHeaderIdParameter, ValueParameter, fldUserIDParameter, fldIPParameter);
        }
        public virtual long prs_tblSooratHesabFromWebServiceInsert(ObjectParameter fldid, Nullable<long> fldForooshandeId, string fldkh_Name, string fldkh_Family, string fldBid, Nullable<byte> fldkh_TypeShakhsId, string fldtinb,  string fldTaxId, Nullable<System.DateTime> fldIndatim, Nullable<System.DateTime> fldIndati2m, Nullable<byte> fldInty, Nullable<byte> fldInp, string fldInno, string fldIrtaxId, string fldFunctionName, System.Data.DataTable ValueHeader, Nullable<long> fldUserId, string fldDesc)
        {
            var fldidParameter = new System.Data.SqlClient.SqlParameter();
            fldidParameter.SqlDbType = System.Data.SqlDbType.BigInt;
            fldidParameter.ParameterName = "@HeaderId";
            fldidParameter.Direction = System.Data.ParameterDirection.Output;

            var fldForooshandeIdParameter = fldForooshandeId.HasValue ?
                new SqlParameter("fldForooshandeId", fldForooshandeId) :
                new SqlParameter("fldForooshandeId", typeof(long));

            var fldkh_NameParameter = fldkh_Name != null ?
                new SqlParameter("fldkh_Name", fldkh_Name) :
                new SqlParameter("fldkh_Name", typeof(string));

            var fldkh_FamilyParameter = fldkh_Family != null ?
                new SqlParameter("fldkh_Family", fldkh_Family) :
                new SqlParameter("fldkh_Family", typeof(string));

            var fldBidParameter = fldBid != null ?
                new SqlParameter("fldBid", fldBid) :
                new SqlParameter("fldBid", typeof(string));

            var fldkh_TypeShakhsIdParameter = fldkh_TypeShakhsId.HasValue ?
                new SqlParameter("fldkh_TypeShakhsId", fldkh_TypeShakhsId) :
                new SqlParameter("fldkh_TypeShakhsId", typeof(byte));

            var fldtinbParameter = fldtinb != null ?
                new SqlParameter("fldtinb", fldtinb) :
                new SqlParameter("fldtinb", typeof(string));


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

            var fldFunctionNameParameter = fldFunctionName != null ?
                new SqlParameter("fldFunctionName", fldFunctionName) :
                new SqlParameter("fldFunctionName", typeof(string));
            
            var ValueHeaderParameter = ValueHeader.Rows.Count > 0 ?
                new System.Data.SqlClient.SqlParameter("ValueHeader", ValueHeader) :
                new System.Data.SqlClient.SqlParameter("ValueHeader", typeof(System.Data.DataTable));
            ValueHeaderParameter.TypeName = "movadi.ParametrValue";
            ValueHeaderParameter.SqlDbType = System.Data.SqlDbType.Structured;

            var fldUserIdParameter = fldUserId.HasValue ?
                new SqlParameter("fldUserId", fldUserId) :
                new SqlParameter("fldUserId", typeof(long));

            var fldDescParameter = fldDesc != null ?
                new SqlParameter("fldDesc", fldDesc) :
                new SqlParameter("fldDesc", typeof(string));

            ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("Movadi.prs_tblSooratHesabFromWebServiceInsert @HeaderId OUT,@fldForooshandeId,@fldkh_Name,@fldkh_Family,@fldBid,@fldkh_TypeShakhsId@fldtinb,@fldTaxId,@fldIndatim,@fldIndati2m,@fldInty,@fldInp,@fldInno,@fldIrtaxId@fldFunctionName,@ValueHeader,@fldUserId,@fldDesc",
               fldidParameter, fldForooshandeIdParameter, fldkh_NameParameter, fldkh_FamilyParameter, fldBidParameter, fldkh_TypeShakhsIdParameter, fldtinbParameter, fldTaxIdParameter, fldIndatimParameter, fldIndati2mParameter, fldIntyParameter, fldInpParameter, fldInnoParameter, fldIrtaxIdParameter, fldFunctionNameParameter, ValueHeaderParameter, fldUserIdParameter, fldDescParameter);
            return Convert.ToInt64(fldidParameter.Value);
        }
        public virtual long prs_InsertSooratHesabForExcel(ObjectParameter headerId, Nullable<long> fldForooshandeId, string fldkh_Name, string fldkh_Family, string fldBid, Nullable<byte> fldkh_TypeShakhsId, string fldtinb, string fldTaxId, Nullable<System.DateTime> fldIndatim, Nullable<System.DateTime> fldIndati2m, Nullable<byte> fldInty, Nullable<byte> fldInp, string fldInno, string fldIrtaxId, string fldFunctionName, string fldShomareFish, System.Data.DataTable ValueHeader, Nullable<long> fldUserId, string fldDesc,string fldIP)
        {
            var HeaderIdParameter = new System.Data.SqlClient.SqlParameter();
            HeaderIdParameter.SqlDbType = System.Data.SqlDbType.BigInt;
            HeaderIdParameter.ParameterName = "@HeaderId";
            HeaderIdParameter.Direction = System.Data.ParameterDirection.Output;

            var fldForooshandeIdParameter = fldForooshandeId.HasValue ?
                new SqlParameter("fldForooshandeId", fldForooshandeId) :
                new SqlParameter("fldForooshandeId", typeof(long));

            var fldkh_NameParameter = fldkh_Name != null ?
                new SqlParameter("fldkh_Name", fldkh_Name) :
                new SqlParameter("fldkh_Name", typeof(string));

            var fldkh_FamilyParameter = fldkh_Family != null ?
                new SqlParameter("fldkh_Family", fldkh_Family) :
                new SqlParameter("fldkh_Family", typeof(string));

            var fldBidParameter = fldBid != null ?
                new SqlParameter("fldBid", fldBid) :
                new SqlParameter("fldBid", typeof(string));

            var fldkh_TypeShakhsIdParameter = fldkh_TypeShakhsId.HasValue ?
                new SqlParameter("fldkh_TypeShakhsId", fldkh_TypeShakhsId) :
                new SqlParameter("fldkh_TypeShakhsId", typeof(byte));

            var fldtinbParameter = fldtinb != null ?
                new SqlParameter("fldtinb", fldtinb) :
                new SqlParameter("fldtinb", typeof(string));

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

            var fldFunctionNameParameter = fldFunctionName != null ?
                new SqlParameter("fldFunctionName", fldFunctionName) :
                new SqlParameter("fldFunctionName", typeof(string));

            var fldShomareFishParameter = fldShomareFish != null ?
                new SqlParameter("fldShomareFish", fldShomareFish) :
                new SqlParameter("fldShomareFish", typeof(string));


            var ValueHeaderParameter = ValueHeader.Rows.Count > 0 ?
                new System.Data.SqlClient.SqlParameter("ValueHeader", ValueHeader) :
                new System.Data.SqlClient.SqlParameter("ValueHeader", typeof(System.Data.DataTable));
            ValueHeaderParameter.TypeName = "movadi.ParametrValue";
            ValueHeaderParameter.SqlDbType = System.Data.SqlDbType.Structured;

            var fldUserIdParameter = fldUserId.HasValue ?
                new SqlParameter("fldUserId", fldUserId) :
                new SqlParameter("fldUserId", typeof(long));

            var fldDescParameter = fldDesc != null ?
                new SqlParameter("fldDesc", fldDesc) :
                new SqlParameter("fldDesc", typeof(string));

            var fldIPParameter = fldIP != null ?
                new System.Data.SqlClient.SqlParameter("fldIP", fldIP) :
                new System.Data.SqlClient.SqlParameter("fldIP", DBNull.Value);

            ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("Movadi.prs_InsertSooratHesabForExcel @HeaderId OUT,@fldForooshandeId,@fldkh_Name,@fldkh_Family,@fldBid,@fldkh_TypeShakhsId,@fldtinb,@fldTaxId,@fldIndatim,@fldIndati2m,@fldInty,@fldInp,@fldInno,@fldIrtaxId,@fldFunctionName,@fldShomareFish,@ValueHeader,@fldUserId,@fldDesc,@fldIP",
          HeaderIdParameter, fldForooshandeIdParameter, fldkh_NameParameter, fldkh_FamilyParameter, fldBidParameter, fldkh_TypeShakhsIdParameter, fldtinbParameter, fldTaxIdParameter, fldIndatimParameter, fldIndati2mParameter, fldIntyParameter, fldInpParameter, fldInnoParameter, fldIrtaxIdParameter, fldFunctionNameParameter, fldShomareFishParameter, ValueHeaderParameter, fldUserIdParameter, fldDescParameter, fldIPParameter);
            return Convert.ToInt64(HeaderIdParameter.Value);

        }
    }
}