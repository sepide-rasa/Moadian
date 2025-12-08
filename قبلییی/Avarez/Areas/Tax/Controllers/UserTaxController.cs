using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Areas.Tax.Models;
using System.IO;

namespace Avarez.Areas.Tax.Controllers
{
    public class UserTaxController : Controller
    {
        //
        // GET: /Tax/UserTax/

        public ActionResult Index()
        {//باز شدن تب جدید
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            return new Ext.Net.MVC.PartialViewResult();


        }
        public ActionResult GetUserType()
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var q = p.prs_tblTypeShakhsSelect("", "", 0).ToList().OrderBy(l => l.fldId).Select(l => new { ID = l.fldId, fldName = l.fldNameTypeShakhs });
            return this.Store(q);

        }
        public ActionResult New(int id)
        {//باز شدن پنجره
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }
        public ActionResult newTransaction(int Userid)
        {//باز شدن پنجره
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var q = p.prs_User_GharardadSelect("fldId", Userid.ToString(), 0, "", 1, "").FirstOrDefault();
            PartialView.ViewBag.TarfGharardadId = q.fldTarfGharardadId;
            var Id = 0;
            var t=p.prs_TransactionInfSelect("fldTarfGharardadId", q.fldTarfGharardadId.ToString(), 0).FirstOrDefault();
            if (t != null)
                Id = t.fldId;

            string Mojodi = "فاقد اعتبار";
            try
            {
                Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();

                if (t != null)
                {
                    var divName = "جمهوری اسلامی ایران";
                    var y = h.CheckAccountCharge(t.fldUserName, t.fldPass, 0, divName);
                    if (y != null)
                    {
                        Mojodi = y.Mojodi;
                    }
                }

            }
            catch (Exception)
            {
                Mojodi = "قطع ارتباط";
            }

            PartialView.ViewBag.Id = Id;
            PartialView.ViewBag.Mojodi = Mojodi;

            return PartialView;
        }
        
        public ActionResult Help()
        {//باز شدن پنجره
            //if (Session["UserId"] == null)
            //    return RedirectToAction("Logon", "Account");
            //else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }

        }
        public ActionResult Details(int Id)
        {
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var q = p.prs_User_GharardadSelect("fldId", Id.ToString(), 0,"",1,"").FirstOrDefault();
            var vaviat = 0;
            if (q.fldStatus == true)
                vaviat = 1;
            return Json(new
            {
                fldId = q.fldID,
                fldName = q.fldName + " " + q.fldFamily,
                fldMelliCode = q.fldMelliCode,
                fldStatus = vaviat.ToString(),
                fldTarfGharardadId = q.fldTarfGharardadId,
                fldUserName = q.fldUserName,
                fldUserType = q.fldUserType.ToString(),
                fldDesc = q.fldDesc

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.prs_User_GharardadSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.prs_User_GharardadSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamily";
                            break;
                        case "fldUserName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldUserName";
                            break;
                        case "fldStatusName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStatusName";
                            break;
                    }
                    if (data != null)
                        data1 = p.prs_User_GharardadSelect(field, searchtext, 100,"",1,"").ToList();
                    else
                        data = p.prs_User_GharardadSelect(field, searchtext, 100, "", 1, "").ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.prs_User_GharardadSelect("", "", 100, "", 1, "").ToList();
            }

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            return !oValue.ToString().Contains(value.ToString());
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Models.prs_User_GharardadSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Save(Models.prs_User_GharardadSelect shakhs)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                Models.cartaxtest2Entities m = new Models.cartaxtest2Entities();


                if (shakhs.fldDesc == null)
                    shakhs.fldDesc = "";

                var q = m.prs_User_GharardadSelect("fldTarfGharardadId", shakhs.fldTarfGharardadId.ToString(), 0,"",1,"").FirstOrDefault();
                if (shakhs.fldID == 0)
                { //ذخیره
                    if (q != null)
                    {
                        Er = 1;
                        Msg = "کاربری برای این شخص قبلا ثبت شده است.";
                        MsgTitle = "خطا";
                    }
                    else
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter UId = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                        m.prs_User_GharardadInsert(UId, shakhs.fldTarfGharardadId, shakhs.fldStatus, CodeDecode.GenerateHash(shakhs.fldUserName), shakhs.fldUserName, Convert.ToInt64(Session["TaxUserId"]), shakhs.fldDesc,null);

                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";
                    }

                }
                else
                { //ویرایش
                    if (q != null && q.fldID != shakhs.fldID)
                    {
                        Er = 1;
                        Msg = "کاربری برای این شخص قبلا ثبت شده است.";
                        MsgTitle = "خطا";
                    }
                    else
                    {
                        m.prs_User_GharardadUpdate(shakhs.fldID, shakhs.fldTarfGharardadId, shakhs.fldStatus,  shakhs.fldUserName, Convert.ToInt64(Session["TaxUserId"]), shakhs.fldDesc, null);



                        Msg = "ویرایش با موفقیت انجام شد.";
                        MsgTitle = "ویرایش موفق";
                    }
                }

            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;
                Er = 1;
                MsgTitle = "خطا";
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailsTrans(int Id)
        {
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var q = p.prs_TransactionInfSelect("fldId", Id.ToString(), 0).FirstOrDefault();
          
            return Json(new
            {
                fldId = q.fldId,
                fldUserName = q.fldUserName,
                fldPass = CodeDecode.stringDecode(q.fldPass)

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveTrans(Models.prs_TransactionInfSelect shakhs)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                Models.cartaxtest2Entities m = new Models.cartaxtest2Entities();


                if (shakhs.fldId == 0)
                { //ذخیره
                   
                        m.prs_TransactionInfInsert(shakhs.fldTarfGharardadId, shakhs.fldUserName, CodeDecode.stringcode(shakhs.fldPass), Convert.ToInt64(Session["TaxUserId"]), "",false);

                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";
                    

                }
                else
                { //ویرایش
                        m.prs_TransactionInfUpdate(shakhs.fldId, shakhs.fldUserName,  shakhs.fldTarfGharardadId,CodeDecode.stringcode(shakhs.fldPass), Convert.ToInt64(Session["TaxUserId"]), "", false);



                        Msg = "ویرایش با موفقیت انجام شد.";
                        MsgTitle = "ویرایش موفق";
                    
                }

            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;
                Er = 1;
                MsgTitle = "خطا";
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
