using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avarez.Controllers.Users;
using System.Web.Configuration;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class InSearch_FileController : Controller
    {
        //
        // GET: /NewVer/InSearch_File/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            return View();
        }

        public ActionResult CheckBlackList(long CarId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_ListeSiyahSelect("fldCarId", CarId.ToString(), 30).FirstOrDefault();
            var Type = 0;
            string Msg = "";
            if (q != null)
            {
                Type = q.fldType;
                if (Type == 1)
                {
                    Msg = q.fldMsg;
                }
                else
                {
                    Msg = "";
                }
            }
            return Json(new { Msg = Msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTrnStatus(int CarId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities m = new Models.cartaxEntities();
            var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
                var y = h.CheckAccountCharge(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName);

                if (y != null)
                {
                    if (y.Type == 1)
                        return Json("1", JsonRequestBehavior.AllowGet);
                    if (y.HaveCharge && y.Type == 2)//Type=2 --> کاربر تراکنشی
                    {

                        var Trans = m.sp_CalcTransactionSelect("fldCarId", CarId.ToString(), 0).FirstOrDefault();
                        if (Trans != null)
                        {
                            if (MyLib.Shamsi.DiffOfShamsiDate(Trans.fldTarikh, MyLib.Shamsi.Miladi2ShamsiString(m.sp_GetDate().FirstOrDefault().CurrentDateTime)) > 30)
                            {
                                return Json("0", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json("1", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json("0", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json("0", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
