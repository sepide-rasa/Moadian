using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez
{
    public class Transaction
    {
       public enum TransactionResult
        {
            Ok,
            Fail,
            NotSharj
        }
        public TransactionResult RunTransaction(string WebTUserName, string WebTPass, int countrydivtype, string LocationName, int CarId, int UserId)
        {
            Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
            var y = h.CheckAccountCharge(WebTUserName, WebTPass, countrydivtype, LocationName);

            if (y != null)
            {
                if (y.Type == 1)
                    return TransactionResult.Ok;
                if (y.HaveCharge && y.Type == 2)//Type=2 --> کاربر تراکنشی
                {
                    Models.cartaxEntities p = new Models.cartaxEntities();
                    var Trans = p.sp_CalcTransactionSelect("fldCarId", CarId.ToString(), 0).FirstOrDefault();
                    if (Trans != null)
                    {
                        if (MyLib.Shamsi.DiffOfShamsiDate(Trans.fldTarikh, MyLib.Shamsi.Miladi2ShamsiString(p.sp_GetDate().FirstOrDefault().CurrentDateTime)) > 30)
                        {
                            bool Tr = h.Transaction(WebTUserName, WebTPass, countrydivtype, LocationName);
                            if (Tr == true)
                            {
                                p.sp_CalcTransactionInsert(CarId, UserId, "");
                                return TransactionResult.Ok;
                            }
                            else
                            {
                                return TransactionResult.Fail;
                            }
                        }
                        else
                        {
                            return TransactionResult.Ok;
                        }
                    }
                    else
                    {
                        bool Tr = h.Transaction(WebTUserName, WebTPass, countrydivtype, LocationName);
                        if (Tr == true)
                        {
                            p.sp_CalcTransactionInsert(CarId, UserId, "");
                            return TransactionResult.Ok;
                        }
                        else
                        {
                            return TransactionResult.Fail;
                        }
                    }
                }
                else
                {
                    return TransactionResult.NotSharj;
                }
            }
            return TransactionResult.Fail;
        }
    }
}