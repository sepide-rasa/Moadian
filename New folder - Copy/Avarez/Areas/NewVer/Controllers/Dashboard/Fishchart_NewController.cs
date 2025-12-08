using Avarez.Controllers.Users;
using Avarez.Models;
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.NewVer.Controllers.Dashboard
{
    public class Fishchart_NewController : Controller
    {
        //
        // GET: /NewVer/Fishchart_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "داشبورد->نمودار فیش های  ماه جاری");
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
        public StoreResult FishGetData(int? mah)
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
            return new StoreResult(ChartModel.GenerateFishData(Convert.ToInt32(Session["UserMnu"]), start, end));
        }
    }
}
