using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;
using System.IO;

namespace Avarez.Areas.NewVer.Controllers.Financial
{
    public class BudjetByMonth_NewController : Controller
    {
        //
        // GET: /NewVer/BudjetByMonth_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات مالی->مقایسه درآمد");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }

        public JsonResult GetYear()
        {
            List<SelectListItem> sal = new List<SelectListItem>();
            var NowDate=MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4);
            for (int i = 1390; i <=1399; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString();
                item.Value = i.ToString();
                sal.Add(item);
            }
            return Json(sal.OrderByDescending(k => k.Text).Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNowDate()
        {
            var NowDate = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4);
            return Json(new{NowDate=NowDate,}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetData(string Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                var q = m.sp_BudgetByMonth_DetailSelect(Convert.ToInt32(Year), Convert.ToInt32(Session["UserMnu"])).ToList();
                var p = m.sp_BudgetByMonthSelect("fldMun_Year",Session["UserMnu"].ToString(), Year, 0).FirstOrDefault();
                int TotalBudget = 0;
                if (p != null)
                {
                    TotalBudget = p.fldTotalBudget;
                }
                return Json(new { data = q, TotalBudget = TotalBudget,Er=0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }

        public ActionResult Save(List<Models.BudgeByMonthVal> BudgeByMonthVal, string fldYear, string fldTotalBudget)
        {
            try
            {
                Models.cartaxEntities p = new Models.cartaxEntities();

                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                var q = p.sp_BudgetByMonthSelect("fldMun_Year", Session["UserMnu"].ToString(), fldYear.ToString(), 0).FirstOrDefault();
                if (q == null)
                {//ثبت رکورد جدید
                    p.sp_BudgetByMonthInsert(_id, Convert.ToInt32(fldYear),Convert.ToInt32(fldTotalBudget), Convert.ToInt32(Session["UserMnu"]), Convert.ToInt32(Session["UserId"]), "");

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
                    p.sp_BudgetByMonthUpdate(q.fldId, Convert.ToInt32(fldYear), Convert.ToInt32(fldTotalBudget), Convert.ToInt32(Session["UserMnu"]), Convert.ToInt32(Session["UserId"]), "");

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
                return Json(new { MsgTitle="ذخیره موفق",Msg = "ذخیره با موفقیت انجام شد.", Er = 0 });
            }

            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle="خطا",Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }
    }
}
