using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.CarTax
{
    public class BudgetByMonthController : Controller
    {
        //
        // GET: /BudgetByMonth/

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 354))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->مقایسه درآمد");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_BudgetByMonth_DetailSelect(Convert.ToInt32(value), Convert.ToInt32(Session["UserMnu"])).ToList();
            var p = m.sp_BudgetByMonthSelect("fldMun_Year", Session["UserMnu"].ToString(), value, 0).FirstOrDefault();
            int TotalBudget = 0;
            if (p != null)
            {
                TotalBudget = p.fldTotalBudget;
            }
            return Json(new { data = q, TotalBudget = TotalBudget }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(List<Models.BudgeByMonthVal> BudgeByMonthVal, Models.BudgeByMonth BudgeByMonth)
        {
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();

                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                var q = p.sp_BudgetByMonthSelect("fldMun_Year", Session["UserMnu"].ToString(),BudgeByMonth.fldYear.ToString(), 0).FirstOrDefault();
                if (q == null)
                {//ثبت رکورد جدید

                    p.sp_BudgetByMonthInsert(_id, BudgeByMonth.fldYear, BudgeByMonth.fldTotalBudget, Convert.ToInt32(Session["UserMnu"]), Convert.ToInt32(Session["UserId"]), "");

                    foreach (var item in BudgeByMonthVal)
                    {

                        if (item.fldId == 0)
                        {

                            p.sp_BudgetByMonth_DetailInsert(Convert.ToInt32(_id.Value), item.fldMonth_No, item.fldPercent, Convert.ToInt32(Session["UserId"]), "");
                        }

                    }

                }
                else
                {//ویرایش رکورد ارسالی
                    p.sp_BudgetByMonthUpdate(q.fldId, BudgeByMonth.fldYear, BudgeByMonth.fldTotalBudget, Convert.ToInt32(Session["UserMnu"]), Convert.ToInt32(Session["UserId"]), "");

                    foreach (var item in BudgeByMonthVal)
                    {
                        if (item.fldId == 0)
                        {
                            p.sp_BudgetByMonth_DetailInsert(q.fldId, item.fldMonth_No, item.fldPercent, Convert.ToInt32(Session["UserId"]), "");
                        }
                        else
                        {
                            p.sp_BudgetByMonth_DetailUpdate(item.fldId, q.fldId, item.fldMonth_No, item.fldPercent, Convert.ToInt32(Session["UserId"]), "");
                        }

                    }

                }
                return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
            }

            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }
        
    }
}
