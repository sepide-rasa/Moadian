using Avarez.Controllers.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Collections;

namespace Avarez.Areas.NewVer.Controllers.Financial
{
    public class ComplicationRate_NewController : Controller
    {
        //
        // GET: /NewVer/ComplicationRate_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 116))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "اطلاعات مالی->ورود نرخ عوارض سالیانه");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                ViewData.Model = new Avarez.Models.SelectFullCar();
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    ViewData = this.ViewData,
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };

                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return result;
                //}
                //else
                //{
                //    return null;
                //}
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

        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public JsonResult GetFromYear(int value)
        {
            List<SelectListItem> sal = new List<SelectListItem>();

            for (int i = value; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4))+1; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString();
                item.Value = i.ToString();
                sal.Add(item);
            }

            return Json(sal.OrderByDescending(k => k.Text).Select(p1 => new { fldID = p1.Value, fldName = p1.Text }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Grid_Save(List<Models.Rate> ArrayL)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 117))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    foreach (var item in ArrayL)
                    {
                        if (item.fldDesc == null)
                            item.fldDesc = "";
                        Car.sp_ComplicationsRateInsert_Update(item.fldTypeCar, item.fldCodeCar, item.fldTypeCountryDivisions, item.fldCodeCountryDivisions
                            , item.fldYear, item.fldPrice,
                            Convert.ToInt32(Session["UserId"]), item.fldDesc, Session["UserPass"].ToString());
                    }
                    return Json(new { Msg = "ذخیره با موفقیت انجام شد.",MsgTitle="ذخیره موفق", Er = 0 });
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
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
        public ActionResult NodeLoadTreeStructure(string nod)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account_New", new { area = "NewVer" });
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            //پروسیجر تشخیص کاربر در تقسیمات کشوری برای
            //لود درخت همان ناحیه ای کاربر در سطح آن تعریف شده 
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();
            NodeCollection nodes = new Ext.Net.NodeCollection();
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
                    asyncNode.IconFile = url + item.fldNodeType + ".png";

                    var child = p.sp_TableTreeSelect("fldPId", item.fldID.ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldNodeName;
                        childNode.NodeID = ch.fldID.ToString();
                        childNode.IconFile = url + ch.fldNodeType + ".png";
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
                    childNode.IconFile = url + ch.fldNodeType + ".png";
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
                return RedirectToAction("Logon", "Account_New", new { area = "NewVer" });
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
        public ActionResult nodeLoadcarLocation(string nod)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            string url = Url.Content("~/Content/images/c");
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var p = new Models.cartaxEntities();
            if (nod == "0" || nod == null)
            {
                var q = p.sp_TableTreeCarSelect("fldId", "1", 0, 0, 0).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldNodeName;
                    asyncNode.NodeID = item.fldID.ToString();
                    asyncNode.DataPath = item.fldNodeType.ToString();
                    asyncNode.IconFile = url + item.fldNodeType + ".png";
                    asyncNode.Cls = item.fldSourceID.ToString();

                    var child = p.sp_TableTreeCarSelect("fldPId", item.fldID.ToString(), 0, 0, 0).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldNodeName;
                        childNode.NodeID = ch.fldID.ToString();
                        childNode.IconFile = url + ch.fldNodeType + ".png";
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
                    childNode.IconFile = url + ch.fldNodeType + ".png";
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
            string nodeNames = ""; bool error = false;
            foreach (var item in nodes)
            {
                ar.Add(item.fldNodeName);
            }

            for (int i = 0; i < ar.Count; i++)
            {

                if (i < ar.Count - 1)
                    nodeNames += ar[i].ToString() + "-->";
                else
                    nodeNames += ar[i].ToString();
            }
            if (Convert.ToInt32(Session["UserId"]) != 1 && (nodeNames.ToString().Contains("سواری") || nodeNames.ToString().Contains("آمبولانس") || nodeNames.ToString().Contains("وانت دوکابین")))
            {
                nodeNames = "شما مجاز به انتخاب این گزینه نمی باشید.";
                error = true;
                return Json(new { Position = nodeNames, error = error });
            }
            return Json(new { Position = nodeNames });
        }
        public ActionResult Reload(string type, string value, bool check, string year, string CountryCode, string CountryType)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var data = m.sp_SelectFullCarForSetMony(Convert.ToInt32(type),Convert.ToInt32(value), check, year, Convert.ToInt32(CountryType), Convert.ToInt32(CountryCode)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NotNullReload(int type, int value, string year, int CountryCode, int CountryType)
        {//جستجو            
            Models.cartaxEntities m = new Models.cartaxEntities();
            var data = m.sp_SelectFullCarForSetMonyFullNotNull(type, value, year, CountryType, CountryCode, Convert.ToInt32(Session["UserId"])).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(Models.Rate rate)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 118))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (rate.fldDesc == null)
                        rate.fldDesc = "";
                    if (rate.fldId == 0)
                    {
                        Car.sp_ComplicationsRateInsert(rate.fldTypeCar, rate.fldCodeCar, rate.fldTypeCountryDivisions,
                            rate.fldCodeCountryDivisions, rate.fldYear, rate.fldFromCylinder, rate.fldToCylinder,
                            rate.fldFromWheel, rate.fldToWheel, rate.fldFromModel, rate.fldToModel, rate.fldFromContentMotor,
                            rate.fldToContentMotor, rate.fldPrice, Convert.ToInt32(Session["UserId"]), rate.fldDesc, Session["UserPass"].ToString());
                        return Json(new { Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = "ذخیره موفق", Er = 0 });
                    }
                    else
                    {
                        Car.sp_ComplicationsRateUpdate(rate.fldId, rate.fldTypeCar, rate.fldCodeCar, rate.fldTypeCountryDivisions,
                            rate.fldCodeCountryDivisions, rate.fldYear, rate.fldFromCylinder, rate.fldToCylinder,
                            rate.fldFromWheel, rate.fldToWheel, rate.fldFromModel, rate.fldToModel, rate.fldFromContentMotor,
                            rate.fldToContentMotor, rate.fldPrice, Convert.ToInt32(Session["UserId"]), rate.fldDesc, Session["UserPass"].ToString());
                        return Json(new { Msg = "ویرایش با موفقیت انجام شد.", MsgTitle = "ویرایش موفق", Er = 0 });
                    }
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
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
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",MsgTitle="خطا", Er = 1 });
            }
        }
        public ActionResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_ComplicationsRateSelect("fldId", id.ToString(), 1, 1, "").FirstOrDefault();
                var p = Car.sp_CarSeriesSelect("fldId", q.fldCarSeriesID.ToString(), 1, 1, "").FirstOrDefault();
                var c = Car.sp_CountryDivisionsSelect("fldId", q.fldCarSeriesID.ToString(), 1, 1, "").FirstOrDefault();
                return Json(new
                {
                    Er = 0,
                    fldId = q.fldID,
                    fldCabinTypeID = q.fldCabinTypeID,
                    fldCarAccountTypeID = q.fldCarAccountTypeID,
                    fldCarAccountTypeName = q.fldCarAccountTypeName,
                    fldCarCabinTypeName = q.fldCarCabinTypeName,
                    fldCarClassID = q.fldCarClassID,
                    fldCarClassName = q.fldCarClassName,
                    fldCarMakeID = q.fldCarMakeID,
                    fldCarMakeName = q.fldCarMakeName,
                    fldCarModelID = q.fldCarModelID,
                    fldCarModelName = q.fldCarModelName,
                    fldCarSeriesID = q.fldCarSeriesID,
                    fldCarSeriesName = q.fldCarSeriesName,
                    fldCarSystemID = q.fldCarSystemID,
                    fldCarSystemName = q.fldCarSystemName,
                    fldCountryDivisions = q.fldCountryDivisions,
                    fldCountryDivisionsName = q.fldCountryDivisionsName,
                    fldFromContentMotor = q.fldFromContentMotor,
                    fldFromCylinder = q.fldFromCylinder,
                    fldFromModel = q.fldFromModel,
                    fldFromWheel = q.fldFromWheel,
                    fldPrice = q.fldPrice,
                    fldToContentMotor = q.fldToContentMotor,
                    fldToCylinder = q.fldToCylinder,
                    fldToModel = q.fldToModel,
                    fldToWheel = q.fldToWheel,
                    fldUserName = q.fldUserName,
                    fldYear = q.fldYear
                    //,
                    //CarId = q.fldCarId,
                    //fldType = q.fldType,
                    //fldMsg = q.fldMsg
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
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 244))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();

                    //var q = Car.sp_CarExperienceSelect("fldId", id.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //if (q.fldUserID == 1 && Convert.ToInt32(Session["UserId"]) != 1)
                    //{
                    //    return Json(new { data = "شما مجوز ویرایش را ندارید.", state = 1 });
                    //}
                    Car.sp_ComplicationsRateDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                    return Json(new { Msg = "حذف با موفقیت انجام شد.",MsgTitle="حذف موفق", Er = 0 });

                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException.Message != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", MsgTitle="خطا",Er = 1 });
            }
        }
        public ActionResult GetYear()
        {
            Models.cartaxEntities Car = new Models.cartaxEntities();
           var Sal= Car.sp_GetDate().FirstOrDefault().DateShamsi;
           Sal.Substring(0, 4);
           return Json(new { Sal=Sal.Substring(0, 4)}, JsonRequestBehavior.AllowGet);
        }
    }
}
