using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avarez.Controllers.Users;
using System.Web.Configuration;

namespace Avarez.Controllers.CarTax
{
    public class InSearchFileController : Controller
    {
        //
        // GET: /SearchFile/

        public ActionResult Index(int State)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            var Per = 0;
            if (State == 1)
            {
                Per = 246;
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->صدور فیش");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
            }
            else if (State == 2)
                Per = 308;
            else if (State == 3)
            {
                Per = 315;
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->رسید های پرداختی");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
            }
            else if (State == 4)
            {
                Per = 351;
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->بایگانی دیجیتال");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
            }
            else if (State == 5)
            {
                Per = 302;
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->حذف فیش های تکراری خودرو خاص");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
            }
            else if (State == 6)
            {
                Per = 342;
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->لیست تراکنش های pcpos");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
            }

            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), Per))
            {
                ViewBag.State = State;
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }

        }
        public ActionResult GetTrnStatus(int CarId)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                Avarez.WebTransaction.TransactionWebService h = new Avarez.WebTransaction.TransactionWebService();
                var y = h.CheckAccountCharge(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName);

                if (y != null)
                {
                    if (y.Type == 1)
                        return Json("1", JsonRequestBehavior.AllowGet);
                    if (y.HaveCharge && y.Type == 2)//Type=2 --> کاربر تراکنشی
                    {

                        var Trans = m.sp_CalcTransactionSelect("fldCarId", CarId.ToString(), 0).FirstOrDefault();
                        if (Trans != null)
                        {
                            if (MyLib.Shamsi.DiffOfShamsiDate(Trans.fldTarikh, MyLib.Shamsi.Miladi2ShamsiString(m.sp_GetDate().FirstOrDefault().CurrentDateTime)) > 30)
                            {
                                return Json("0", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json("1", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json("0", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json("0", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ShowDetail(int id)
        {
            if (Session["UserState"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 246))
            {
                Session["fldCarID"] = id;
                Session["fldCarID1"] = id;
                Session["fldCarID2"] = id;
                Session["fldCarID3"] = id;
                Session["mablagh"] = null;
                Session["Fine"] = null;
                Session["ValueAddPrice"] = null;
                Session["Price"] = null;
                Session["Year"] = null;
                ViewBag.id = id;
                Models.cartaxEntities m = new Models.cartaxEntities();
                var Tree = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();

                //var UpTree = m.sp_SelectUpTreeCountryDivisions(Convert.ToInt32(Tree.fldID), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                var PosIPId = 0;

                //foreach (var item in UpTree.OrderByDescending(l=>l.fldNodeType))
                //{
                //var c = m.sp_GET_IDCountryDivisions(item.fldNodeType, item.fldSourceID).FirstOrDefault();
                var Pos = m.sp_PcPosInfoSelect("fldTreeId", "", Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), 0).FirstOrDefault();
                if (Pos != null)
                {
                    var UserPos = m.sp_PcPosUserSelect("fldIdUser", Session["UserId"].ToString(), 0).FirstOrDefault();
                    if (UserPos != null)
                    {
                        var PosIp = m.sp_PcPosIPSelect("fldId", UserPos.fldPosIPId.ToString(), 0).FirstOrDefault();
                        if (PosIp != null)
                        {
                            PosIPId = PosIp.fldId;
                            //break;
                        }
                    }
                }
                //}
                ViewBag.PosIPId = PosIPId;
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Reload(int field, string value1, string value2, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldVIN", "fldShasiAndMotorNumber", "fldMotor", "fldShasi", "fldOwnerName", "fldCodeMeli", "fldPelak" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value1);
            string searchtext2 = string.Format(searchType[searchtype], value2);
            Models.cartaxEntities m = new Models.cartaxEntities();
            if (field == 0)
                value2 = "";
            var q = m.sp_CarUserGuestSelect(_fiald[field], searchtext, searchtext2, 30).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckBlackList(long carid)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_ListeSiyahSelect("fldCarId", carid.ToString(), 30).FirstOrDefault();
            var Type = 0;
            string Msg = "";
            if (q != null)
            {
                Type = q.fldType;
                if (Type == 1)
                {
                    Msg = q.fldMsg;
                }
                else
                {
                    Msg = "";
                }
            }
            return Json(new { Msg = Msg }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckHaveArchive(long carid)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var carfiles = m.sp_CarFileSelect("fldCarId", carid.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            string Msg = "کاربر گرامی لطفا مدارک پرونده مورد نظر را در قسمت بایگانی دیجیتال ثبت بفرمایید.";
            foreach (var item1 in carfiles)
            {
                var q = m.sp_DigitalArchiveSelect("fldCarFileId", item1.fldID.ToString(), 30).FirstOrDefault();
                if (q != null)
                {
                    Msg = "";
                    return Json(new { Msg = Msg }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Msg = Msg }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DetailSubSetting()
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var subSett = Car.sp_SelectSubSetting(0, 0, Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), Car.sp_GetDate().FirstOrDefault().CurrentDateTime).FirstOrDefault();
                int? fldDefaultSearch = 0;
                if (subSett != null)
                {
                    fldDefaultSearch = subSett.fldDefaultSearch;
                }
                return Json(new
                {
                    fldDefaultSearch = fldDefaultSearch
                }, JsonRequestBehavior.AllowGet);
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
