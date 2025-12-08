using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class ChCarFilePelaquSearch_Controller : Controller
    {
        //
        // GET: /NewVer/ChCarFilePelaquSearch_/

        public ActionResult Index(int CarId, int CarPlaqId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 300))
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->تعویض مالک");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                PartialView.ViewBag.CarId = CarId;
                PartialView.ViewBag.CarPlaqId = CarPlaqId;
                return PartialView;
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "شما مجاز به دسترسی نمی باشید."
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }

        public ActionResult Read(StoreRequestParameters parameters, int CarPlaqId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities p = new Models.cartaxEntities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.sp_CarPlaqueSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.sp_CarPlaqueSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldOwnerName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOwnerName";
                            break;
                        case "fldOwnerMelli_EconomicCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOwnerMelli_EconomicCode";
                            break;
                        case "fldPlaqueNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPlaqueNumber";
                            break;
                        case "fldPlaqueCityName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPlaqueCityName";
                            break;
                        case "fldPlaqueSerial":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPlaqueSerial";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = p.sp_CarPlaqueSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(l => l.fldID != CarPlaqId).ToList();
                    else
                        data = p.sp_CarPlaqueSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(l => l.fldID != CarPlaqId).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.sp_CarPlaqueSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(l => l.fldID != CarPlaqId).ToList();
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

            List<Models.sp_CarPlaqueSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
