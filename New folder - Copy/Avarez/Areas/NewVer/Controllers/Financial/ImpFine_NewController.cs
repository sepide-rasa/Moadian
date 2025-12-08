using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Controllers.Users;
using System.Collections;

namespace Avarez.Areas.NewVer.Controllers.Financial
{
    public class ImpFine_NewController : Controller
    {
        //
        // GET: /NewVer/ImpFine_New/
        public ActionResult Index(string containerId)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات مالی->اعمال قانون جریمه");
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

        public ActionResult New(int Id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }


        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
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



        public ActionResult CountryPosition(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var nodes = car.sp_SelectUpTreeCountryDivisions(id, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            ArrayList ar = new ArrayList();

            foreach (var item in nodes)
            {
                ar.Add(item.fldNodeName);
            }
            string nodeNames = "";
            for (int i = 0; i < ar.Count; i++)
            {
                if (i < ar.Count - 1)
                    nodeNames += ar[i].ToString() + "-->";
                else
                    nodeNames += ar[i].ToString();
            }

            return Json(new { Position = nodeNames });
        }


        public ActionResult NodeLoadTreeCar(string nod)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var p = new Models.cartaxEntities();
            string url = Url.Content("~/Content/images/c");
            if (nod == "0" || nod == null)
            {
                var q = p.sp_TableTreeCarSelect("fldId", "1", 0, 0, 0).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldNodeName;
                    asyncNode.NodeID = item.fldID.ToString();
                    asyncNode.DataPath = item.fldNodeType.ToString();
                    asyncNode.Cls = item.fldSourceID.ToString();

                    var child = p.sp_TableTreeCarSelect("fldPId", item.fldID.ToString(), 0, 0, 0).ToList();
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
            var child = p.sp_TableTreeCarSelect("fldPId", nod, 0, 0, 0).OrderBy(l => l.fldNodeName).ToList();

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

        public ActionResult CarPosition(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            var nodes = car.sp_SelectUpTreeCarSeries(id, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            ArrayList ar = new ArrayList();

            foreach (var item in nodes)
            {
                ar.Add(item.fldNodeName);
            }
            string nodeNames = "";
            for (int i = 0; i < ar.Count; i++)
            {
                if (i < ar.Count - 1)
                    nodeNames += ar[i].ToString() + "-->";
                else
                    nodeNames += ar[i].ToString();
            }

            return Json(new { Position = nodeNames });
        }
        public ActionResult GetCascadeRound()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_FinesRuleSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Select(c => new { fldID = c.fldID, fldName = c.fldName }).OrderBy(x => x.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(Models.ImpFine ImpFine)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (ImpFine.fldDesc == null)
                    ImpFine.fldDesc = "";
                if (ImpFine.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 136))
                    {
                        Car.sp_ImplementationFinesRuleInsert(ImpFine.fldFinesRuleID, ImpFine.fldTypeCar, ImpFine.fldCodeCar,
                            ImpFine.fldTypeCountryDivisions, ImpFine.fldCodeCountryDivisions, Convert.ToInt32(Session["UserId"]),
                            ImpFine.fldDesc, ImpFine.fldCarSeriesTreeApply, ImpFine.fldCountryDivisionsTreeApply,
                             Session["UserPass"].ToString());

                        return Json(new
                        {
                            MsgTitle = "ذخیره موفق",
                            Msg = "ذخیره با موفقیت انجام شد.",
                            Er = 0
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        });
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 138))
                    {
                        Car.sp_ImplementationFinesRuleUpdate(ImpFine.fldID, ImpFine.fldFinesRuleID, ImpFine.fldTypeCar, ImpFine.fldCodeCar,
                            ImpFine.fldTypeCountryDivisions, ImpFine.fldCodeCountryDivisions, Convert.ToInt32(Session["UserId"]),
                            ImpFine.fldDesc, ImpFine.fldCarSeriesTreeApply, ImpFine.fldCountryDivisionsTreeApply,
                             Session["UserPass"].ToString());
                        return Json(new
                        {
                            MsgTitle = "ویرایش موفق",
                            Msg = "ویرایش با موفقیت انجام شد.",
                            Er = 0
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        });
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
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }

        

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 137))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                        Car.sp_ImplementationFinesRuleDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        return Json(new
                        {
                            MsgTitle = "حذف موفق",
                            Msg = "حذف با موفقیت انجام شد.",
                            Er = 0
                        });
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    });
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities m = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                m.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
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

        public ActionResult LoadPathCar(string Path)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New");
            List<string> a = Path.Split('/').Skip(1).Skip(1).ToList();
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var node = new Ext.Net.Node();
            var node2 = new Ext.Net.Node();

            string url = Url.Content("~/Content/images/c");
            for (var i = 0; i < a.Count - 1; i++)
            {
                var p = new Models.cartaxEntities();
                var child = p.sp_TableTreeCarSelect("fldPID", a[i].ToString(), 0, 0, 0).OrderBy(l => l.fldNodeName).ToList();
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

        public ActionResult Details(int Id)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                long? sid = 0;
                int? carsid = 0;
                string Path = "/";
                string CarPath = "/";
                int? type = 0;
                int? typeCar = 0;
                int LastNodeId = 0;
                int LastNodeIdCar=0;
                /*var countryId = Car.sp_TableTreeSelect("fldSourceID", ImpFine.CountryCode.ToString(), 0, 0, 0).Where(h => h.fldNodeType == ImpFine.CountryType).FirstOrDefault();
                var CarId = Car.sp_TableTreeCarSelect("fldSourceID", ImpFine.CarCode.ToString(), 0, 0, 0).Where(h => h.fldNodeType == ImpFine.CarType).FirstOrDefault();*/


                Models.cartaxEntities Car = new Models.cartaxEntities();
                var ImpFine = Car.sp_ImplementationFinesRuleSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var a = Car.sp_CountryDivisionsSelect("fldId", ImpFine.fldCountryDivisionID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var b = Car.sp_CarSeriesSelect("Id", ImpFine.fldCarSeriesID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
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
                if (b != null)
                {
                    if (b.fldCabinTypeID != null)
                    {
                        carsid = b.fldCabinTypeID;
                        typeCar = 3;
                    }
                    else if (b.fldCarAccountTypeID != null)
                    {
                        carsid = b.fldCarAccountTypeID;
                        typeCar = 2;
                    }
                    else if (b.fldCarClassID != null)
                    {
                        carsid = b.fldCarClassID;
                        typeCar = 6;
                    }
                    else if (b.fldCarMakeID != null)
                    {
                        carsid = b.fldCarMakeID;
                        typeCar = 1;
                    }
                    else if (b.fldCarModelID != null)
                    {
                        carsid = b.fldCarModelID;
                        typeCar = 5;
                    }
                    else if (b.fldCarSystemID != null)
                    {
                        carsid = b.fldCarSystemID;
                        typeCar = 4;
                    }
                    LastNodeIdCar = Car.sp_TableTreeCarSelect("fldSourceID", carsid.ToString(), 0, 0, 0).Where(l => l.fldNodeType == typeCar).FirstOrDefault().fldID;
                    var nodesCar = Car.sp_SelectUpTreeCarSeries(LastNodeIdCar, 1, "").ToList();
                    foreach (var item in nodesCar)
                    {
                        CarPath = CarPath + item.fldID + "/";
                    }
                    CarPath = CarPath.Substring(0, CarPath.Length - 1);
                    if (carsid == 0)
                    {
                        CarPath = "/1";
                    }
                }

                return Json(new
                {
                    fldId = ImpFine.fldID,
                    fldFinesRuleID = ImpFine.fldFinesRuleID.ToString(),
                    fldImplementationDate = ImpFine.fldImplementationDate,
                    fldCountryDivisionsTreeApply = ImpFine.fldCountryDivisionsTreeApplyBit,
                    fldCarSeriesTreeApply = ImpFine.fldCarSeriesTreeApplyBit,
                    fldDesc = ImpFine.fldDesc,
                    countryid=LastNodeId,
                    CountryType = ImpFine.CountryType,
                    countryCode = ImpFine.CountryCode,
                    Carid=LastNodeIdCar,
                    carType = ImpFine.CarType,
                    carCode = ImpFine.CarCode,
                    Path = Path,
                    CarPath = CarPath,
                    /*carid = CarId.fldID,
                    countryId = countryId.fldID,*/
                    Er = 0
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
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_ImplementationFinesRuleSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_ImplementationFinesRuleSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldFineRuleName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFineRuleName";
                            break;
                        case "fldCarSeriesName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCarSeriesName";
                            break;
                        case "fldCountryDivisionsTreeApply":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCountryDivisionsTreeApply";
                            break;
                        case "fldCarSeriesTreeApply":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCarSeriesTreeApply";
                            break;
                        case "fldImplementationDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldImplementationDate";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_ImplementationFinesRuleSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_ImplementationFinesRuleSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_ImplementationFinesRuleSelect("", "", 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Avarez.Models.sp_ImplementationFinesRuleSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
