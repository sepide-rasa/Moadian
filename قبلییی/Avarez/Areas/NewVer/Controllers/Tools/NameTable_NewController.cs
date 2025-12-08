using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using System.IO;
using Kendo.Mvc.Extensions;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data.Entity;
using Avarez.Controllers.Users;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Configuration;

namespace Avarez.Areas.NewVer.Controllers.Tools
{
    public class NameTable_NewController : Controller
    {
        //
        // GET: /NewVer/NameTable_New/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "ابزارهای سیستم->تاریخچه کاربران");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.ImageSetting = ImageSetting;
            return result;
        }

        public ActionResult getLogs(string AzTarikh, string TaTarikh)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.AzTarikh = AzTarikh;
            PartialView.ViewBag.TaTarikh = TaTarikh;
            return PartialView;
        }

        public ActionResult getLogsforSpecifyrow(int id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.id = id;
            return PartialView;
        }
        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult ReadLogs(StoreRequestParameters parameters, string TaTarikh, string AzTarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_CarFile_LogSelect2> data = null;
            data = m.sp_CarFile_LogSelect2("","",AzTarikh, TaTarikh, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Avarez.Models.sp_CarFile_LogSelect2> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult ReadLogsRow(StoreRequestParameters parameters, int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_CarFile_LogSelect2> data = null;
            data = m.sp_CarFile_LogSelect2("fldId", id.ToString(),"","", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Avarez.Models.sp_CarFile_LogSelect2> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_NameTablesSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_NameTablesSelect> data1 = null;
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

                    }
                    if (data != null)

                        data1 = m.sp_NameTablesSelect(field, searchtext, 100).ToList();
                    else
                        data = m.sp_NameTablesSelect(field, searchtext, 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_NameTablesSelect("", "", 30).ToList();
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

            List<Avarez.Models.sp_NameTablesSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult FileExport(int id, string start, string end)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            Models.cartaxEntities car = new Models.cartaxEntities();
            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            if (id == 1)
            {
                gv.DataSource = car.sp_State_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                 
            }
            else if (id == 2)
            {
                gv.DataSource = car.sp_County_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 3)
            {
                gv.DataSource = car.sp_Zone_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 4)
            {
                //gv.DataSource = car.sp_City_LogSelect(start,
                //    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 5)
            {
                gv.DataSource = car.sp_Municipality_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 6)
            {
                gv.DataSource = car.sp_Local_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 7)
            {
                gv.DataSource = car.sp_Area_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 8)
            {
                gv.DataSource = car.sp_OfficesType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 9)
            {
                gv.DataSource = car.sp_Offices_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 10)
            {
                gv.DataSource = car.sp_DegreeMunicipality_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 11)
            {
                gv.DataSource = car.sp_PlaqueCity_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 12)
            {
                gv.DataSource = car.sp_PlaqueSerial_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 13)
            {
                gv.DataSource = car.sp_StatusPlaque_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 14)
            {
                gv.DataSource = car.sp_ColorCar_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 15)
            {
                gv.DataSource = car.sp_Bank_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }

            else if (id == 16)
            {
                gv.DataSource = car.sp_BankType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();

            }
            else if (id == 17)
            {
                gv.DataSource = car.sp_BankBranch_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 18)
            {
                gv.DataSource = car.sp_AccountBank_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 19)
            {
                gv.DataSource = car.sp_User_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 20)
            {
                gv.DataSource = car.sp_CarMake_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 21)
            {
                gv.DataSource = car.sp_CarAccountType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 22)
            {
                gv.DataSource = car.sp_CabinType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 23)
            {
                gv.DataSource = car.sp_CarSystem_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 24)
            {
                gv.DataSource = car.sp_CarPatternModel_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 25)
            {
                gv.DataSource = car.sp_CarModel_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 26)
            {
                gv.DataSource = car.sp_CarClass_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 27)
            {
                gv.DataSource = car.sp_FuelType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 28)
            {
                gv.DataSource = car.sp_CharacterPersianPlaque_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 29)
            {
                gv.DataSource = car.sp_Cost_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 30)
            {
                gv.DataSource = car.sp_AmountCost_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }

            else if (id == 31)
            {
                gv.DataSource = car.sp_Contact_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();

            }
            else if (id == 32)
            {
                gv.DataSource = car.sp_AccostType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 33)
            {
                gv.DataSource = car.sp_MainSetting_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 34)
            {
                gv.DataSource = car.sp_SubSetting_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 35)
            {
                gv.DataSource = car.sp_SendLetter_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 36)
            {
                gv.DataSource = car.sp_PlaqueType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 37)
            {
                gv.DataSource = car.sp_Organization_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 38)
            {
                gv.DataSource = car.sp_CarPlaque_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 39)
            {
                gv.DataSource = car.sp_Owner_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 40)
            {
                gv.DataSource = car.sp_Car_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 41)
            {
                gv.DataSource = car.sp_CarFile_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 42)
            {
                gv.DataSource = car.sp_Post_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 43)
            {
                gv.DataSource = car.sp_SignerEmployee_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 44)
            {
                gv.DataSource = car.sp_CarExperience_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 45)
            {
                gv.DataSource = car.sp_Peacockery_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 46)
            {
                gv.DataSource = car.sp_Collection_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 47)
            {
                gv.DataSource = car.sp_OftenInvolved_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 48)
            {
                gv.DataSource = car.sp_ComplicationsRate_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 49)
            {
                //gv.DataSource = car.sp_FinesRule_LogSelect(start,
                //    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 50)
            {
                gv.DataSource = car.sp_ImplementationFinesRule_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 51)
            {
                gv.DataSource = car.sp_SettleType_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 52)
            {
                gv.DataSource = car.sp_Discount_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 53)
            {
                gv.DataSource = car.sp_News_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 54)
            {
                gv.DataSource = car.sp_OnlinePayments_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 55)
            {
                gv.DataSource = car.sp_BankInformation_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 56)
            {
                gv.DataSource = car.sp_Reports_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 57)
            {
                gv.DataSource = car.sp_SystemReport_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 58)
            {
                gv.DataSource = car.sp_ApplicationPart_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 59)
            {
                gv.DataSource = car.sp_ShortTermCountry_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 60)
            {
                gv.DataSource = car.sp_UserGroup_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 61)
            {
                gv.DataSource = car.sp_User_Group_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 62)
            {
                gv.DataSource = car.sp_Permission_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 63)
            {
                gv.DataSource = car.sp_Round_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            else if (id == 64)
            {
                gv.DataSource = car.sp_Friends_LogSelect(start,
                    end, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            }
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=پیش نمایش.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View();
        }
    }
}
