using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;

namespace Avarez.Areas.NewVer.Controllers.Config
{
    public class PcPosInfo_NewController : Controller
    {
        //
        // GET: /NewVer/PcPosInfo_New/

        public ActionResult Index(string containerId)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "پیکربندی سیستم->تعیین بانک");
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

        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult New(int Id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }
        public ActionResult GetBank()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_BankSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(Models.sp_PcPosInfoSelect Pos)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Pos.fldDesc == null)
                    Pos.fldDesc = "";
                if (Pos.fldId == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 328))
                    {
                        Car.sp_PcPosInfoInsert(Pos.fldBankId, Pos.CountryDivisionsType, Pos.CountryDivisionscode, Convert.ToInt32(Session["UserId"]), Pos.fldDesc);

                        return Json(new { Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = "ذخیره موفق", Er = 0 });
                    }
                    else
                    {
                        return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Er = 1 });
                        //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        //return RedirectToAction("error", "Metro");
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 329))
                    {
                        Car.sp_PcPosInfoUpdate(Pos.fldId, Pos.fldBankId, Pos.CountryDivisionsType, Pos.CountryDivisionscode, Convert.ToInt32(Session["UserId"]), Pos.fldDesc);
                        return Json(new { Msg = "ویرایش با موفقیت انجام شد.", MsgTitle = ",ویرایش موفق", Er = 0 });
                    }
                    else
                    {
                        return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Er = 1 });
                        //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        //return RedirectToAction("error", "Metro");
                    }
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 330))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    Car.sp_PcPosParam_Detail_InfoIdDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
                    foreach (var item in Car.sp_PcPosParametrSelect("fldPosInfoId", id, 0).ToList())
                        Car.sp_PcPosParametrDelete(item.fldId, Convert.ToInt32(Session["UserId"]));
                    foreach (var _item in Car.sp_PcPosUserSelect("fldPcPosInfoId", id, 0).ToList())
                        Car.sp_PcPosUserDelete(_item.fldId, Convert.ToInt32(Session["UserId"]));

                    Car.sp_PcPosInfoDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
                    return Json(new { Msg = "حذف با موفقیت انجام شد.",MsgTitle="حذف موفق", Er = 0 });

                }
                else
                {
                    return Json(new { Msg = "شما مجاز به دسترسی نمی باشید.", MsgTitle = "خطا", Er = 1 });
                    //Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    //return RedirectToAction("error", "Metro");
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }
        public ActionResult LoadPath(string Path)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            List<string> a = Path.Split('/').Skip(1).Skip(1).ToList();
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var node = new Ext.Net.Node();
            var node2 = new Ext.Net.Node();

            string url = Url.Content("~/Content/images/");
            int m = 0;
            for (var i = 0; i < a.Count - 1; i++)
            {
                var p = new Models.cartaxEntities();
                var child = p.sp_TableTreeSelect("fldPId", a[i].ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();
                if (i == 0)
                {
                    for (int j = 0; j < child.Count; j++)
                    {
                        Node childNode = new Node();
                        if (child[j].fldID == Convert.ToInt32(a[i + 1])) { node = childNode; }
                        childNode.Text = child[j].fldNodeName;
                        childNode.NodeID = child[j].fldID.ToString();
                        childNode.IconFile = url + child[j].fldNodeType + ".png";
                        childNode.DataPath = child[j].fldNodeType.ToString();
                        childNode.Cls = child[j].fldSourceID.ToString();
                        nodes.Add(childNode);
                    }
                }
                else
                {
                    for (int j = 0; j < child.Count; j++)
                    {
                        Node childNode = new Node();
                        if (child[j].fldID == Convert.ToInt32(a[i + 1])) { node2 = childNode; }
                        childNode.Text = child[j].fldNodeName;
                        childNode.NodeID = child[j].fldID.ToString();
                        childNode.IconFile = url + child[j].fldNodeType + ".png";
                        childNode.DataPath = child[j].fldNodeType.ToString();
                        childNode.Cls = child[j].fldSourceID.ToString();
                        node.Children.Add(childNode);
                    };
                    node = node2;
                }
            }
            return this.Direct(nodes);
        }
        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                long? sid = 0;
                string Path = "/";
                int? type = 0;
                int LastNodeId = 0;
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var Pos = Car.sp_PcPosInfoSelect("fldId", id.ToString(),0, 0,1).FirstOrDefault();
                //var countryId = Car.sp_TableTreeSelect("fldSourceID", Pos.CountryDivisionscode.ToString(), 0, 0, 0).Where(h => h.fldNodeType == Pos.CountryDivisionsType).FirstOrDefault();
                var a = Car.sp_CountryDivisionsSelect("fldId", Pos.fldTreeId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                if (a != null)
                {
                    if (a.fldAreaID != null)
                    {
                        sid = a.fldAreaID;
                        type = 7;
                    }
                    else if (a.fldCityID != null)
                    {
                        sid = a.fldCityID;
                        type = 4;
                    }
                    else if (a.fldCountyID != null)
                    {
                        sid = a.fldCountyID;
                        type = 2;
                    }
                    else if (a.fldLocalID != null)
                    {
                        sid = a.fldLocalID;
                        type = 6;
                    }
                    else if (a.fldMunicipalityID != null)
                    {
                        sid = a.fldMunicipalityID;
                        type = 5;
                    }
                    else if (a.fldOfficesID != null)
                    {
                        sid = a.fldOfficesID;
                        type = 8;
                    }
                    else if (a.fldStateID != null)
                    {
                        sid = a.fldStateID;
                        type = 1;
                    }
                    else if (a.fldZoneID != null)
                    {
                        sid = a.fldZoneID;
                        type = 3;
                    }
                    LastNodeId = Car.sp_TableTreeSelect("fldSourceID", sid.ToString(), 0, 0, 0).Where(l => l.fldNodeType == type).FirstOrDefault().fldID;
                    var nodes = Car.sp_SelectUpTreeCountryDivisions(LastNodeId, 1, "").ToList();
                    foreach (var item in nodes)
                    {
                        Path = Path + item.fldID + "/";
                    }
                    Path = Path.Substring(0, Path.Length - 1);
                    if (sid == 0)
                    {
                        Path = "/1";
                    }
                }
                return Json(new
                {
                    Er = 0,
                    fldId = Pos.fldId,
                    fldBankId = Pos.fldBankId.ToString(),
                    CoutryDivisionId=LastNodeId,
                    CoutryDivisionCode = Pos.CountryDivisionscode,
                    CoutryDivisionType = Pos.CountryDivisionsType,
                    Path = Path,
                    //countryId = countryId.fldID
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle = "خطا", Er = 1 });
            }
        }
        public ActionResult NodeLoadTreeCountry(string nod)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            string url = Url.Content("~/Content/images/");
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var p = new Models.cartaxEntities();
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            if (nod == "0" || nod == null)
            {
                var q = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldNodeName;
                    asyncNode.NodeID = item.fldID.ToString();
                    asyncNode.DataPath = item.fldNodeType.ToString();
                    asyncNode.Cls = item.fldSourceID.ToString();
                    var child = p.sp_TableTreeSelect("fldPId", item.fldID.ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldNodeName;
                        childNode.NodeID = ch.fldID.ToString();
                        childNode.IconFile = url + ch.fldNodeType.ToString() + ".png";
                        childNode.DataPath = ch.fldNodeType.ToString();
                        childNode.Cls = ch.fldSourceID.ToString();
                        asyncNode.Children.Add(childNode);
                    }
                    nodes.Add(asyncNode);
                }
            }
            else
            {
            var child = p.sp_TableTreeSelect("fldPId", nod, 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();

            foreach (var ch in child)
            {
                Node childNode = new Node();
                childNode.Text = ch.fldNodeName;
                childNode.NodeID = ch.fldID.ToString();
                childNode.IconFile = url + ch.fldNodeType.ToString() + ".png";
                childNode.DataPath = ch.fldNodeType.ToString();
                childNode.Cls = ch.fldSourceID.ToString();
                nodes.Add(childNode);
            }
            }
            return this.Direct(nodes);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_PcPosInfoSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_PcPosInfoSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldCountryDivisionName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCountryDivisionName";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_PcPosInfoSelect(field, searchtext,0,0, 100).ToList();
                    else
                        data = m.sp_PcPosInfoSelect(field, searchtext,0,0, 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_PcPosInfoSelect("", "",0,0, 100).ToList();
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

            List<Avarez.Models.sp_PcPosInfoSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
