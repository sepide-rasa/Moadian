using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;

namespace Avarez.Areas.NewVer.Controllers.Dashboard
{
    public class compare_newController : Controller
    {
        //
        // GET: /NewVer/compare_new/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "داشبورد->مقایسه میزان وصولی");
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

        public ActionResult GetData()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities p = new Models.cartaxEntities();
            int year = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4));
            DateTime start = MyLib.Shamsi.Shamsi2miladiDateTime((year - 1) + "/01/01");
            var q = p.sp_RptCollectionIn2Year(Convert.ToInt32(Session["UserMnu"]), start, DateTime.Now).ToList();
            long[] oldyear = new long[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            long[] newYear = new long[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            if (q.Count > 0)
            {
                var lastyear = q.Max(l => l.fldsh_year).Value;

                for (int i = 0; i < q.Count; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if (q[i].fldsh_year == lastyear && q[i].fldsh_m == j + 1)
                            newYear[j] = (long)q[i].price;
                        else if (q[i].fldsh_year == lastyear - 1 && q[i].fldsh_m == j + 1)
                            oldyear[j] = (long)q[i].price;
                    }
                }
            }


            
            return Json(new { oldyear = oldyear, newYear = newYear, year = year }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getCars(int? mah)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            string date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
            DateTime start;
            if (mah == null)
                mah = Convert.ToInt32(date.Substring(5, 2));
            start = MyLib.Shamsi.Shamsi2miladiDateTime(date.Substring(0, 4) +
                "/" + mah.ToString().PadLeft(2, '0') + "/01");
            DateTime end; int roz = 31;
            if (mah > 6 && mah < 12)
                roz = 30;
            else if (mah == 12)
            {
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(date.Substring(0, 4))))
                    roz = 30;
                else
                    roz = 29;
            }

            end = MyLib.Shamsi.Shamsi2miladiDateTime(date.Substring(0, 4) +
                "/" + mah.ToString().PadLeft(2, '0') + "/" + roz.ToString().PadLeft(2, '0'));
            var q = p.sp_RptChart(start, end, Convert.ToInt32(Session["UserMnu"])).ToList();
            
            List<Avarez.Models.sp_RptChart> sum = new List<Avarez.Models.sp_RptChart>();
            foreach (var item in q)
            {
                var t = sum.Where(k => k.CarModelTip == item.CarModelTip).FirstOrDefault();
                if (t != null)
                    t.fldPrice += item.fldPrice;
                else
                    sum.Add(item);
            }

            string[,] price = new string[sum.Count,2];
            for (int i = 0; i < sum.Count; i++)
            {
                price[i, 0] = sum[i].fldPrice.ToString();
                price[i, 1] = sum[i].CarModelTip;
            }


            return Json(new { data = price }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUsers(int? mah)
        {
            string date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
            DateTime start;
            if (mah == null)
                mah = Convert.ToInt32(date.Substring(5, 2));
            start = MyLib.Shamsi.Shamsi2miladiDateTime(date.Substring(0, 4) +
                "/" + mah.ToString().PadLeft(2, '0') + "/01");
            DateTime end; int roz = 31;
            if (mah > 6 && mah < 12)
                roz = 30;
            else if (mah == 12)
            {
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(date.Substring(0, 4))))
                    roz = 30;
                else
                    roz = 29;
            }

            end = MyLib.Shamsi.Shamsi2miladiDateTime(date.Substring(0, 4) +
                "/" + mah.ToString().PadLeft(2, '0') + "/" + roz.ToString().PadLeft(2, '0'));
            Models.cartaxEntities p = new Models.cartaxEntities();
            var q = p.sp_RptMonthlyUser_CountWithDate(start,
                end, Convert.ToInt32(Session["UserMnu"])).ToList();
            string[,] user = new string[q.Count, 2];
            for (int i = 0; i < q.Count; i++)
            {
                user[i, 0] = q[i].UserName;
                user[i, 1] = q[i].fldCount.ToString();
            }
            return Json(new { data = user }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getpayType(int? mah)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            string date = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
            DateTime start;
            if (mah == null)
                mah = Convert.ToInt32(date.Substring(5, 2));
            start = MyLib.Shamsi.Shamsi2miladiDateTime(date.Substring(0, 4) +
                "/" + mah.ToString().PadLeft(2, '0') + "/01");
            DateTime end; int roz = 31;
            if (mah > 6 && mah < 12)
                roz = 30;
            else if (mah == 12)
            {
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(date.Substring(0, 4))))
                    roz = 30;
                else
                    roz = 29;
            }

            end = MyLib.Shamsi.Shamsi2miladiDateTime(date.Substring(0, 4) +
                "/" + mah.ToString().PadLeft(2, '0') + "/" + roz.ToString().PadLeft(2, '0'));
            var q = p.sp_RptChart(start, end, Convert.ToInt32(Session["UserMnu"])).ToList();

            List<Avarez.Models.sp_RptChart> sum = new List<Avarez.Models.sp_RptChart>();
            foreach (var item in q)
            {
                var t = sum.Where(k => k.SettleType == item.SettleType).FirstOrDefault();
                if (t != null)
                    t.fldPrice += item.fldPrice;
                else
                    sum.Add(item);
            }

            string[,] price = new string[sum.Count, 2];
            for (int i = 0; i < sum.Count; i++)
            {
                price[i, 0] = sum[i].fldPrice.ToString();
                price[i, 1] = sum[i].SettleType;
            }


            return Json(new { data = price }, JsonRequestBehavior.AllowGet);
        }
    }
}
