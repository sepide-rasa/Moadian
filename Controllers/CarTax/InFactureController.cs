using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
using System.Collections;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Configuration;

namespace Avarez.Controllers.CarTax
{
    public class InFactureController : Controller
    {
        //
        // GET: /Facture/

        public ActionResult Index(int id)
        { 
            if (Session["UserState"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 246))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->صورتحساب");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                Session["fldCarID"] = id;
                Session["fldCarID1"] = id;
                Session["fldCarID2"] = id;
                Session["fldCarID3"] = id;
                Session["mablagh"] = null;
                Session["Fine"] = null;
                Session["ValueAddPrice"] = null;
                Session["Price"] = null;
                Session["Year"] = null;

                Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
                var Tree = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
               
                //var UpTree = m.sp_SelectUpTreeCountryDivisions(Convert.ToInt32(Tree.fldID), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                var PosIPId = 0;

                //foreach (var item in UpTree.OrderByDescending(l=>l.fldNodeType))
                //{
                    //var c = m.sp_GET_IDCountryDivisions(item.fldNodeType, item.fldSourceID).FirstOrDefault();
               var Pos = m.sp_PcPosInfoSelect("fldTreeId", Tree.CountryDivisionId.ToString(), 0).FirstOrDefault();
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
        public ActionResult GetPicStatus(int id)
        {            
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var car = m.sp_SelectCarDetils(id).FirstOrDefault();
            var file = m.sp_CarFileSelect("fldid", car.fldID.ToString(), 0, 1, "").FirstOrDefault();
            bool status = true; int carfile = 0; int carext = 0; int coll = 0;
           
                
            if (file.fldSanadForoshFileId == null && file.fldCartFileId == null && file.fldBargSabzFileId == null)
            {
                status = false;
                carfile = 1;
            }
            var carex = m.sp_CarExperienceSelect("fldCarFileID", car.fldID.ToString(), 0, 1, "").ToList();
            if (carex != null)
            {
                foreach (var item in carex)
                {
                    DateTime dt1 = MyLib.Shamsi.Shamsi2miladiDateTime(item.fldDate);
                    DateTime dt2 = DateTime.Parse("2017/02/19");

                    if (dt1.Date > dt2.Date)
                    if (item.fldFileId == null)
                    {
                        status = false;
                        carext = 1;
                    }
                }
            }
            var collection = m.sp_CollectionSelect("fldCarFileID", car.fldID.ToString(), 0, 1, "").Where(k => k.fldMunId != null).ToList();
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    if (item.fldFileId == null)
                    {
                        status = false;
                        coll = 1;
                    }
                }
            }
            return Json(new { status = status, carfile = carfile, carext = carext, coll = coll }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PcPosPay(string Mablagh, string PosIPId, int carid)
        {
            PAX.PCPOS.ActiveX objDOM = new PAX.PCPOS.ActiveX();
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            long peackokeryid = CheckExistFishForPos(carid, Convert.ToInt32(Mablagh));
            if (peackokeryid == 0)
            {
                FishReport(carid);
                peackokeryid = CheckExistFishForPos(carid, Convert.ToInt32(Mablagh));
            }
            if (peackokeryid != 0)
            {
                var fish = m.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                var car = m.sp_SelectCarDetils(carid).FirstOrDefault();
                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter _Cid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                var k = m.sp_PcPosTransactionInsert(_id, Convert.ToInt32(car.fldID), Convert.ToInt32(Session["showmoney"]), false, "", Convert.ToInt32(Session["UserId"]), "", fish.fldShGhabz, fish.fldShPardakht);
                var PosIp = m.sp_PcPosIPSelect("fldId", PosIPId, 0).FirstOrDefault();

                //var q = m.sp_PcPosParametrSelect("fldPosInfoId", PosIp.fldPcPosId.ToString(), 0).ToList();
                //var id = 0;
                //var id_url = 0;
                //foreach (var item in q)
                //{
                //    if (item.fldEnName == "PIN")
                //    {
                //        id = item.fldId;
                //    }
                //    else if (item.fldEnName == "BackUrl")
                //    {
                //        id_url = item.fldId;
                //    }
                //}
                //var q1 = m.sp_PcPosParam_DetailSelect("fldParamId", id.ToString(), 0).FirstOrDefault();

                objDOM.InitSocket(PosIp.fldIP);
                JavaScriptSerializer json = new JavaScriptSerializer();
                var r = objDOM.BillPayment(_id.Value.ToString(), fish.fldShGhabz, fish.fldShPardakht, true);
                Response res = json.Deserialize<Response>(r);

                if (res.Success && res.TransactionInfo.ResponseCode == "00")
                {
                    var t = m.sp_PcPosTransactionUpdate_Status(Convert.ToInt32(res.AdditionalData), res.TransactionInfo.Stan, "پرداخت در تاریخ " + MyLib.Shamsi.Miladi2ShamsiString(m.sp_GetDate().FirstOrDefault().CurrentDateTime) + " توسط دتگاه با کد ترمینال " + res.PosInformation.TerminalId + " و شماره سریال " + PosIp.fldSerialNum + "انجام شد.");
                    m.sp_CollectionInsert(_Cid, car.fldID, m.sp_GetDate().FirstOrDefault().CurrentDateTime, Convert.ToInt32(Session["showmoney"]), 9, (int)fish.fldID, null, "", Convert.ToInt32(Session["UserId"]),
                        "پرداخت از طریق شناسه قبض و شناسه پرداخت", "", "", null, "", null, null);
                    SmsSender sms = new SmsSender();
                    sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, Session["showmoney"].ToString(), "", "", "");
                    return Json(new { data = "پرداخت با موفقیت انجام و واریزی در سیستم ثبت گردید.", state = 0 });
                }
                else
                    return Json(new { data = "پرداخت انجام نشد." + res.ErrorCode, state = 1 });
            }
            else
                return Json(new { data = "پرداخت انجام نشد."+peackokeryid, state = 1 });
        }
     
        public ActionResult CheckBlackList(long carid)
        {
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_ListeSiyahSelect("fldCarId", carid.ToString(), 30).FirstOrDefault();
            var Type =0;
            string Msg = "";
            if (q != null)
            {
                Type = q.fldType;
                if (Type == 2)
                {
                    Msg = q.fldMsg;
                }
                else
                {
                    Msg = "";
                }
            }
            return Json(new { Msg = Msg}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckExistFish(long carid,int showmoney)
        {
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
            string MohlatDate = "";
            byte roundNumber = 0;
            var ServerDate = p.sp_GetDate().FirstOrDefault();
            if (SubSetting != null)
            {
                if (SubSetting.fldLastRespitePayment > 0)
                {
                    MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                }
                else if (SubSetting.fldLastRespitePayment == 0)
                {
                    string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                    int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                    int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                    int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                    if (Mounth <= 6)
                        MohlatDate = Year + "/" + Mounth + "/31";
                    else if (Mounth > 6 && Mounth < 12)
                        MohlatDate = Year + "/" + Mounth + "/30";
                    else if (MyLib.Shamsi.Iskabise(Year) == true)
                        MohlatDate = Year + "/" + Mounth + "/30";
                    else
                        MohlatDate = Year + "/" + Mounth + "/29";
                    //if
                }
                var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                roundNumber = Round.fldRound;
            }


            double Rounded = 10;
            switch (roundNumber)
            {
                case 3:
                    Rounded = 1000;
                    break;
                case 2:
                    Rounded = 100;
                    break;
            }


            showmoney = Convert.ToInt32(Math.Ceiling(showmoney / Rounded) * Rounded);//گرد به بالا  
            var q = p.sp_SelectExistPeacockery(carid, showmoney).FirstOrDefault();
            if (q != null)
                if (q.PeacockeryId != null)
                {
                    var t = p.sp_PeacockerySelect("fldId", q.PeacockeryId.ToString(), 1, 1, "").FirstOrDefault();
                    var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    if (bankid.fldID == t.fldAccountBankID)
                    {
                        return Json(new { PeacockeryId = q.PeacockeryId, state = 1 }, JsonRequestBehavior.AllowGet);
                    }else
                        return Json(new { PeacockeryId = 0, state = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { PeacockeryId = 0, state = 0 }, JsonRequestBehavior.AllowGet);
            return Json(new { PeacockeryId = 0, state = 0 }, JsonRequestBehavior.AllowGet);
        }
        public long CheckExistFishForPos(long carid, int showmoney)
        {
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
            string MohlatDate = "";
            byte roundNumber = 0;
            var ServerDate = p.sp_GetDate().FirstOrDefault();
            if (SubSetting != null)
            {
                if (SubSetting.fldLastRespitePayment > 0)
                {
                    MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                }
                else if (SubSetting.fldLastRespitePayment == 0)
                {
                    string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                    int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                    int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                    int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                    if (Mounth <= 6)
                        MohlatDate = Year + "/" + Mounth + "/31";
                    else if (Mounth > 6 && Mounth < 12)
                        MohlatDate = Year + "/" + Mounth + "/30";
                    else if (MyLib.Shamsi.Iskabise(Year) == true)
                        MohlatDate = Year + "/" + Mounth + "/30";
                    else
                        MohlatDate = Year + "/" + Mounth + "/29";
                    //if
                }
                var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                roundNumber = Round.fldRound;
            }


            double Rounded = 10;
            switch (roundNumber)
            {
                case 3:
                    Rounded = 1000;
                    break;
                case 2:
                    Rounded = 100;
                    break;
            }


            Session["showmoney"] = Convert.ToInt32(Math.Ceiling(showmoney / Rounded) * Rounded);//گرد به بالا  
            var q = p.sp_SelectExistPeacockery(carid, Convert.ToInt32(Session["showmoney"])).FirstOrDefault();
            if (q != null)
                if (q.PeacockeryId != null)
                {
                    var t = p.sp_PeacockerySelect("fldId", q.PeacockeryId.ToString(), 1, 1, "").FirstOrDefault();
                    var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    if (bankid.fldID == t.fldAccountBankID)
                    {
                        return (long)q.PeacockeryId;
                    }
                    else
                        return 0;
                }
                else
                    return 0;
            else
                return 0;
        }
        public ActionResult _FishReport(int id, int mablagh)//المثنی
        {
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            {
                Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
                var peacokery = p.sp_PeacockerySelect("fldid", id.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var peacokery_copy = p.Sp_Peacockery_CopySelect(id).FirstOrDefault();
                if (peacokery_copy != null)
                {
                    return File(peacokery_copy.fldCopy, "application/pdf");
                }
                var carfile=p.sp_CarFileSelect("fldid",peacokery.fldCarFileID.ToString(),0,Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var car = p.sp_SelectCarDetils(Convert.ToInt32(carfile.fldCarID)).FirstOrDefault();
                if (car != null)
                {
                    var ServerDate = p.sp_GetDate().FirstOrDefault();
                    int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime).Substring(0, 4));
                    string date = toYear + "/12/29";
                    if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                        date = toYear + "/12/30";
                    System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                    //var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                    //null, ServerDate.CurrentDateTime, _year, _Bed).ToList();
                    int sal=0, mah=0;
                    //ArrayList Years = new ArrayList();
                    //foreach (var item in bedehi)
                    //{
                    //    Years.Add(item.fldyear);
                    //}
                    ArrayList Years = (ArrayList)Session["Year"];
                    int[] AvarezSal = new int[Years.Count];
                    for (int i = 0; i < Years.Count; i++)
                    {
                        AvarezSal[i] = (int)Years[i];
                    }
                   
                    var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    //p.sp_selectAccountBank(Convert.ToInt32(Session["UserMnu"]), true).FirstOrDefault();

                    System.Data.Entity.Core.Objects.ObjectParameter _CountryID = new System.Data.Entity.Core.Objects.ObjectParameter("ID", typeof(long));

                    var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
                    string MohlatDate = "";
                    byte roundNumber = 0;
                    if (SubSetting != null)
                    {
                        if (SubSetting.fldLastRespitePayment > 0)
                        {
                            MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                        }
                        else if (SubSetting.fldLastRespitePayment == 0)
                        {
                            string Cdate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                            int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                            int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                            int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                            if (Years.Count > 1)
                            {
                                if (Mounth <= 6)
                                    MohlatDate = Year + "/" + Mounth + "/31";
                                else if (Mounth > 6 && Mounth < 12)
                                    MohlatDate = Year + "/" + Mounth + "/30";
                                else if (MyLib.Shamsi.Iskabise(Year) == true)
                                    MohlatDate = Year + "/" + Mounth + "/30";
                                else
                                    MohlatDate = Year + "/" + Mounth + "/29";
                            }
                            else if (Years.Count == 1)
                            {
                                if (MyLib.Shamsi.Iskabise(Year) == true)
                                    MohlatDate = Year + "/" + 12 + "/30";
                                else
                                    MohlatDate = Year + "/" + 12 + "/29";
                            }
                            //if
                        }    
                        var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                        roundNumber = Round.fldRound;
                    }


                    double Rounded = 10;
                    switch (roundNumber)
                    {
                        case 3:
                            Rounded = 1000;
                            break;
                        case 2:
                            Rounded = 100;
                            break;
                    }

                                    
                    mablagh = Convert.ToInt32(Math.Ceiling(mablagh / Rounded) * Rounded);//گرد به بالا
                    string ShGhabz = peacokery.fldShGhabz, ShPardakht = peacokery.fldShPardakht,
                        BarcodeText ="", ShParvande = "";
                    if (ShGhabz.Length > 0 && ShPardakht.Length > 0)
                        BarcodeText = ShGhabz.PadLeft(13, '0') + ShPardakht.PadLeft(13, '0');
                    //if (Convert.ToInt32(Session["CountryType"]) == 5)
                    //{
                    //    var mnu = p.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //    if (mnu.fldInformaticesCode == "")
                    //        mnu.fldInformaticesCode = "0";
                    //    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                    //    {
                    //        var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    //        if (Divisions != null)
                    //        {
                    //            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                    //            sal = ShParvande.Length - 2;
                    //            if (ShParvande.Length > 8)
                    //            {
                    //                string s = ShParvande.Substring(8).PadRight(2, '0');
                    //                ShParvande = ShParvande.Substring(0, 8);
                    //                mah = Convert.ToInt32(s);
                    //            }
                    //            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                    //                , Convert.ToInt32(mablagh), sal, mah);
                    //            ShGhabz = gh.ShGhabz;
                    //            ShPardakht = gh.ShPardakht;
                    //            BarcodeText = gh.BarcodeText;
                    //        }
                    //    }
                    //}
                    //else if (Convert.ToInt32(Session["CountryType"]) == 6)
                    //{
                    //    var local = p.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //    if (local.fldSourceInformatics == "")
                    //        local.fldSourceInformatics = "0";
                    //    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                    //    {
                    //        var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    //        if (Divisions != null)
                    //        {
                    //            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                    //            sal = ShParvande.Length - 2;
                    //            if (ShParvande.Length > 8)
                    //            {
                    //                string s = ShParvande.Substring(8).PadRight(2, '0');
                    //                ShParvande = ShParvande.Substring(0, 8);
                    //                mah = Convert.ToInt32(s);
                    //            }
                    //            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                    //                , Convert.ToInt32(mablagh), sal, mah);
                    //            ShGhabz = gh.ShGhabz;
                    //            ShPardakht = gh.ShPardakht;
                    //            BarcodeText = gh.BarcodeText;
                    //        }
                    //    }
                    //}
                    //else if (Convert.ToInt32(Session["CountryType"]) == 7)
                    //{
                    //    var area = p.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //    if (area != null)
                    //    {
                    //        if (area.fldLocalID != null)
                    //        {
                    //            var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                    //            if (local.fldSourceInformatics == "")
                    //                local.fldSourceInformatics = "0";
                    //            if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                    //            {
                    //                var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                    //                if (Divisions != null)
                    //                {
                    //                    var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                    //                    sal = ShParvande.Length - 2;
                    //                    if (ShParvande.Length > 8)
                    //                    {
                    //                        string s = ShParvande.Substring(8).PadRight(2, '0');
                    //                        ShParvande = ShParvande.Substring(0, 8);
                    //                        mah = Convert.ToInt32(s);
                    //                    }
                    //                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                    //                        , Convert.ToInt32(mablagh), sal, mah);
                    //                    ShGhabz = gh.ShGhabz;
                    //                    ShPardakht = gh.ShPardakht;
                    //                    BarcodeText = gh.BarcodeText;
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //            if (mnu.fldInformaticesCode == "")
                    //                mnu.fldInformaticesCode = "0";
                    //            if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                    //            {
                    //                var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                    //                if (Divisions != null)
                    //                {
                    //                    var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                    //                    sal = ShParvande.Length - 2;
                    //                    if (ShParvande.Length > 8)
                    //                    {
                    //                        string s = ShParvande.Substring(8).PadRight(2, '0');
                    //                        ShParvande = ShParvande.Substring(0, 8);
                    //                        mah = Convert.ToInt32(s);
                    //                    }
                    //                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                    //                        , Convert.ToInt32(mablagh), sal, mah);
                    //                    ShGhabz = gh.ShGhabz;
                    //                    ShPardakht = gh.ShPardakht;
                    //                    BarcodeText = gh.BarcodeText;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //else if (Convert.ToInt32(Session["CountryType"]) == 8)
                    //{
                    //    var office = p.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //    if (office != null)
                    //    {
                    //        if (office.fldAreaID != null)
                    //        {
                    //            var area = p.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //            if (area != null)
                    //            {
                    //                if (area.fldLocalID != null)
                    //                {
                    //                    var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                    //                    if (local.fldSourceInformatics == "")
                    //                        local.fldSourceInformatics = "0";
                    //                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                    //                    {
                    //                        var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                    //                        if (Divisions != null)
                    //                        {
                    //                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                    //                            sal = ShParvande.Length - 2;
                    //                            if (ShParvande.Length > 8)
                    //                            {
                    //                                string s = ShParvande.Substring(8).PadRight(2, '0');
                    //                                ShParvande = ShParvande.Substring(0, 8);
                    //                                mah = Convert.ToInt32(s);
                    //                            }
                    //                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                    //                                , Convert.ToInt32(mablagh), sal, mah);
                    //                            ShGhabz = gh.ShGhabz;
                    //                            ShPardakht = gh.ShPardakht;
                    //                            BarcodeText = gh.BarcodeText;
                    //                        }
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //                    if (mnu.fldInformaticesCode == "")
                    //                        mnu.fldInformaticesCode = "0";
                    //                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                    //                    {
                    //                        var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                    //                        if (Divisions != null)
                    //                        {
                    //                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                    //                            sal = ShParvande.Length - 2;
                    //                            if (ShParvande.Length > 8)
                    //                            {
                    //                                string s = ShParvande.Substring(8).PadRight(2, '0');
                    //                                ShParvande = ShParvande.Substring(0, 8);
                    //                                mah = Convert.ToInt32(s);
                    //                            }
                    //                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                    //                                , Convert.ToInt32(mablagh), sal, mah);
                    //                            ShGhabz = gh.ShGhabz;
                    //                            ShPardakht = gh.ShPardakht;
                    //                            BarcodeText = gh.BarcodeText;
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            var local = p.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //            if (local != null)
                    //            {
                    //                if (local.fldSourceInformatics == "")
                    //                    local.fldSourceInformatics = "0";
                    //                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                    //                {
                    //                    var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                    //                    if (Divisions != null)
                    //                    {
                    //                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                    //                        sal = ShParvande.Length - 2;
                    //                        if (ShParvande.Length > 8)
                    //                        {
                    //                            string s = ShParvande.Substring(8).PadRight(2, '0');
                    //                            ShParvande = ShParvande.Substring(0, 8);
                    //                            mah = Convert.ToInt32(s);
                    //                        }
                    //                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                    //                            , Convert.ToInt32(mablagh), sal, mah);
                    //                        ShGhabz = gh.ShGhabz;
                    //                        ShPardakht = gh.ShPardakht;
                    //                        BarcodeText = gh.BarcodeText;
                    //                    }
                    //                }
                    //            }
                    //            else
                    //            {
                    //                var mnu = p.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //                if (mnu.fldInformaticesCode == "")
                    //                    mnu.fldInformaticesCode = "0";
                    //                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                    //                {
                    //                    var Divisions = p.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                    //                    if (Divisions != null)
                    //                    {
                    //                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    //                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                    //                        sal = ShParvande.Length - 2;
                    //                        if (ShParvande.Length > 8)
                    //                        {
                    //                            string s = ShParvande.Substring(8).PadRight(2, '0');
                    //                            ShParvande = ShParvande.Substring(0, 8);
                    //                            mah = Convert.ToInt32(s);
                    //                        }
                    //                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                    //                            , Convert.ToInt32(mablagh), sal, mah);
                    //                        ShGhabz = gh.ShGhabz;
                    //                        ShPardakht = gh.ShPardakht;
                    //                        BarcodeText = gh.BarcodeText;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    
                    Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                    Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter sp_pic1 = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter();

                    sp_pic1.Fill(dt.sp_PictureSelect1, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                    DataSet.DataSet1.sp_jCalcCarFileDataTable joziat = new DataSet.DataSet1.sp_jCalcCarFileDataTable();
                    joziat = (DataSet.DataSet1.sp_jCalcCarFileDataTable)Session["Joziyat"];
                    foreach (var item in joziat)
                    {
                        dt.sp_jCalcCarFile.Addsp_jCalcCarFileRow((int)item.fldyear, (int)item.fldFirstPrice, (int)item.fldCurectPrice, (int)item.fldValueAdded, (int)item.fldFinalPrice,
                       (int)item.fldFine, (int)item.fldCountMonth, (int)item.fldDiscount, (int)item.fldDept, item.fldCalcDate, (int)item.OtherPrice, (int)item.fldValueAddDiscount,
                       (int)item.fldFineDiscount, (int)item.fldOtherDiscount);
                    }  

                    fish.Fill(dt.sp_PeacockerySelect, "fldId", id.ToString(), 1, 1, "");
                    var _fish = p.sp_PeacockerySelect("fldId", id.ToString(), 1, 1, "").FirstOrDefault();
                    int _Bedehkar = Convert.ToInt32(Session["Bed"]);
                    sp_pic.Fill(dt.sp_PictureSelect, "fldBankPic", _fish.fldBankId.ToString(), 1, 1, "");
                    var mnu1 = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    FastReport.Report Report = new FastReport.Report();
                    var UpReportSelect = p.sp_UpReportSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    var FishReport = p.sp_ReportsSelect("fldId", UpReportSelect.fldReportsID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    System.IO.MemoryStream Stream = new System.IO.MemoryStream(FishReport.fldReport);
                    Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                    var mnu = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    var State = p.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    Report.Load(Stream);
                    Report.RegisterData(dt, "CarTaxDataSet");
                    Report.SetParameterValue("MunicipalityName", mnu1.fldName);
                    Report.SetParameterValue("Barcode", BarcodeText);
                    Report.SetParameterValue("ShGhabz", ShGhabz);
                    Report.SetParameterValue("ShPardakht", ShPardakht);
                    Report.SetParameterValue("SalAvarez", "(" + arrang(AvarezSal) + ")");
                    Report.SetParameterValue("Mohlat", MohlatDate);
                    Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
                    var time = DateTime.Now;
                    Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                    Report.SetParameterValue("MunicipalityName", mnu.fldName);
                    Report.SetParameterValue("Parameter", _Bedehkar);
                    Report.SetParameterValue("StateName", State.fldName);
                    Report.SetParameterValue("AreaName", Session["area"].ToString());
                    Report.SetParameterValue("OfficeName", Session["office"].ToString());
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                    MemoryStream stream = new MemoryStream();
                    Report.Prepare();
                    Report.Export(pdf, stream);


                    return File(stream.ToArray(), "application/pdf");
                }
                return null;
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult FishReport(int id)
        {
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
            {
                Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
                var car = p.sp_SelectCarDetils(id).FirstOrDefault();
                if (car != null)
                {
                    var ServerDate=p.sp_GetDate().FirstOrDefault();
                    int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime).Substring(0, 4));
                    //string date = toYear + "/12/29";
                    //if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                    //    date = toYear + "/12/30";
                    System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                    //var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                    //null, ServerDate.CurrentDateTime, _year, _Bed).ToList();
                    int sal=0, mah=0;
                    double mablagh = 0;
                    int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0,
                        fldMainDiscount = 0, fldFineDiscount = 0, fldValueAddDiscount = 0, fldOtherDiscount = 0;
                    mablagh = Convert.ToInt32(Session["mablagh"]);
                    fldFine = Convert.ToInt32(Session["Fine"]);
                    fldValueAddPrice=Convert.ToInt32(Session["ValueAddPrice"]);
                    fldPrice = Convert.ToInt32(Session["Price"]);
                    fldOtherPrice=Convert.ToInt32( Session["OtherPrice"]);
                    fldMainDiscount =  Convert.ToInt32(Session["fldMainDiscount"]);
                    fldFineDiscount =  Convert.ToInt32(Session["fldFineDiscount"]);
                    fldValueAddDiscount =  Convert.ToInt32(Session["fldValueAddDiscount"]);
                    fldOtherDiscount =  Convert.ToInt32(Session["fldOtherDiscount"]);
                    //ArrayList Years = new ArrayList();
                    //foreach (var item in bedehi)
                    //{
                    //    fldFine += (int)item.fldFine;
                    //    fldValueAddPrice += (int)item.fldValueAdded;
                    //    fldPrice += (int)item.fldCurectPrice;
                    //    //fldOtherPrice += (int)item.fldOtherPrice;
                    //    mablagh += (int)item.fldDept;
                    //    Years.Add(item.fldyear);
                    //}
                    ArrayList Years = (ArrayList)Session["Year"];
                    int[] AvarezSal = new int[Years.Count];
                    for (int i = 0; i < Years.Count; i++)
                    {
                        AvarezSal[i] = (int)Years[i];
                    }
                    mablagh += Convert.ToInt32(_Bed.Value);
                    fldPrice += Convert.ToInt32(_Bed.Value);
                    if (mablagh < 10000)
                    {
                        Session["ER"] = "پرونده انتخابی بدهکار نیست.";
                        return RedirectToAction("error", "Metro");
                    }
                    
                    var bankid = p.sp_UpAccountBankSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    //p.sp_selectAccountBank(Convert.ToInt32(Session["UserMnu"]), true).FirstOrDefault();
                    
                    System.Data.Entity.Core.Objects.ObjectParameter _CountryID = new System.Data.Entity.Core.Objects.ObjectParameter("ID", typeof(long));

                    var SubSetting = p.sp_UpSubSettingSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    //p.sp_SubSettingSelect("fldCountryDivisionsID", _CountryID.Value.ToString(), 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
                    string MohlatDate = "";
                    int roundNumber = 0;
                    if (SubSetting != null)
                    {
                        if (SubSetting.fldLastRespitePayment > 0)
                        {
                            MohlatDate = MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime.AddDays(SubSetting.fldLastRespitePayment));
                        }
                        else if (SubSetting.fldLastRespitePayment == 0)
                        {
                            string Cdate=MyLib.Shamsi.Miladi2ShamsiString(ServerDate.CurrentDateTime);
                            int Year = Convert.ToInt32(Cdate.Substring(0, 4));
                            int Mounth = Convert.ToInt32(Cdate.Substring(5, 2));
                            int Day = Convert.ToInt32(Cdate.Substring(8, 2));
                            if (Years.Count > 1)
                            {
                                if (Mounth <= 6)
                                    MohlatDate = Year + "/" + Mounth + "/31";
                                else if (Mounth > 6 && Mounth < 12)
                                    MohlatDate = Year + "/" + Mounth + "/30";
                                else if (MyLib.Shamsi.Iskabise(Year) == true)
                                    MohlatDate = Year + "/" + Mounth + "/30";
                                else
                                    MohlatDate = Year + "/" + Mounth + "/29";
                            }
                            else if (Years.Count == 1)
                            {
                                if (MyLib.Shamsi.Iskabise(Year) == true)
                                    MohlatDate = Year + "/" + 12 + "/30";
                                else
                                    MohlatDate = Year + "/" + 12 + "/29";
                            }
                            //if
                        }
                        var Round = p.sp_RoundSelect("fldid", SubSetting.fldRoundID.ToString(), 1, 1, "").FirstOrDefault();
                        roundNumber = Round.fldRound;
                    }

                    double Rounded = 10;
                    switch (roundNumber)
                    {
                        case 3:
                            Rounded = 1000;
                            break;
                        case 2:
                            Rounded = 100;
                            break;
                    }


                    mablagh = Math.Ceiling(mablagh / Rounded) * Rounded;//گرد به بالا
                    
                    string ShGhabz = "", ShPardakht = "", BarcodeText = "", ShParvande = "";
                    
                    System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                    var datetime = p.sp_GetDate().FirstOrDefault().CurrentDateTime;
                    p.sp_PeacockeryInsert(car.fldID, datetime, bankid.fldID, "", Convert.ToInt32(fldPrice),
                        Convert.ToInt32(fldFine), fldValueAddPrice, fldOtherPrice, Convert.ToInt32(mablagh),
                        MyLib.Shamsi.Shamsi2miladiDateTime(car.fldStartDateInsurance), datetime, Convert.ToInt32(Session["UserId"]),
                        "", Session["UserPass"].ToString(), fldMainDiscount, fldValueAddDiscount, fldOtherDiscount, ShGhabz, ShPardakht, _id, fldFineDiscount);
                    if (Convert.ToInt32(Session["CountryType"]) == 5)
                    {
                        var mnu = p.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (mnu.fldInformaticesCode == "")
                            mnu.fldInformaticesCode = "0";
                        if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                        {
                            var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                            if (Divisions != null)
                            {
                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                sal = ShParvande.Length - 2;
                                if (ShParvande.Length > 8)
                                {
                                    string s = ShParvande.Substring(8).PadRight(2, '0');
                                    ShParvande = ShParvande.Substring(0, 8);
                                    mah = Convert.ToInt32(s);
                                }
                                ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                    , Convert.ToInt32(mablagh), sal, mah);
                                ShGhabz = gh.ShGhabz;
                                ShPardakht = gh.ShPardakht;
                                BarcodeText = gh.BarcodeText;
                            }
                        }
                    }
                    else if (Convert.ToInt32(Session["CountryType"]) == 6)
                    {
                        var local = p.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (local.fldSourceInformatics == "")
                            local.fldSourceInformatics = "0";
                        if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                        {
                            var Divisions = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                            if (Divisions != null)
                            {
                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                sal = ShParvande.Length - 2;
                                if (ShParvande.Length > 8)
                                {
                                    string s = ShParvande.Substring(8).PadRight(2, '0');
                                    ShParvande = ShParvande.Substring(0, 8);
                                    mah = Convert.ToInt32(s);
                                }
                                ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                    , Convert.ToInt32(mablagh), sal, mah);
                                ShGhabz = gh.ShGhabz;
                                ShPardakht = gh.ShPardakht;
                                BarcodeText = gh.BarcodeText;
                            }
                        }
                    }
                    else if (Convert.ToInt32(Session["CountryType"]) == 7)
                    {
                        var area = p.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (area != null)
                        {
                            if (area.fldLocalID != null)
                            {
                                var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                                if (local.fldSourceInformatics == "")
                                    local.fldSourceInformatics = "0";
                                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                            , Convert.ToInt32(mablagh), sal, mah);
                                        ShGhabz = gh.ShGhabz;
                                        ShPardakht = gh.ShPardakht;
                                        BarcodeText = gh.BarcodeText;
                                    }
                                }
                            }
                            else
                            {
                                var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (mnu.fldInformaticesCode == "")
                                    mnu.fldInformaticesCode = "0";
                                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                {
                                    var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                    if (Divisions != null)
                                    {
                                        var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                        sal = ShParvande.Length - 2;
                                        if (ShParvande.Length > 8)
                                        {
                                            string s = ShParvande.Substring(8).PadRight(2, '0');
                                            ShParvande = ShParvande.Substring(0, 8);
                                            mah = Convert.ToInt32(s);
                                        }
                                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                            , Convert.ToInt32(mablagh), sal, mah);
                                        ShGhabz = gh.ShGhabz;
                                        ShPardakht = gh.ShPardakht;
                                        BarcodeText = gh.BarcodeText;
                                    }
                                }
                            }
                        }
                    }
                    else if (Convert.ToInt32(Session["CountryType"]) == 8)
                    {
                        var office = p.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        if (office != null)
                        {
                            if (office.fldAreaID != null)
                            {
                                var area = p.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (area != null)
                                {
                                    if (area.fldLocalID != null)
                                    {
                                        var local = p.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                                        if (local.fldSourceInformatics == "")
                                            local.fldSourceInformatics = "0";
                                        if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                        {
                                            var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                            if (Divisions != null)
                                            {
                                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                                ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                                sal = ShParvande.Length - 2;
                                                if (ShParvande.Length > 8)
                                                {
                                                    string s = ShParvande.Substring(8).PadRight(2, '0');
                                                    ShParvande = ShParvande.Substring(0, 8);
                                                    mah = Convert.ToInt32(s);
                                                }
                                                ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                                    , Convert.ToInt32(mablagh), sal, mah);
                                                ShGhabz = gh.ShGhabz;
                                                ShPardakht = gh.ShPardakht;
                                                BarcodeText = gh.BarcodeText;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var mnu = p.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                        if (mnu.fldInformaticesCode == "")
                                            mnu.fldInformaticesCode = "0";
                                        if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                        {
                                            var Divisions = p.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                                            if (Divisions != null)
                                            {
                                                var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                                ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                                sal = ShParvande.Length - 2;
                                                if (ShParvande.Length > 8)
                                                {
                                                    string s = ShParvande.Substring(8).PadRight(2, '0');
                                                    ShParvande = ShParvande.Substring(0, 8);
                                                    mah = Convert.ToInt32(s);
                                                }
                                                ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                                    , Convert.ToInt32(mablagh), sal, mah);
                                                ShGhabz = gh.ShGhabz;
                                                ShPardakht = gh.ShPardakht;
                                                BarcodeText = gh.BarcodeText;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var local = p.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                if (local != null)
                                {
                                    if (local.fldSourceInformatics == "")
                                        local.fldSourceInformatics = "0";
                                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                                                , Convert.ToInt32(mablagh), sal, mah);
                                            ShGhabz = gh.ShGhabz;
                                            ShPardakht = gh.ShPardakht;
                                            BarcodeText = gh.BarcodeText;
                                        }
                                    }
                                }
                                else
                                {
                                    var mnu = p.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                    if (mnu.fldInformaticesCode == "")
                                        mnu.fldInformaticesCode = "0";
                                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                                    {
                                        var Divisions = p.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                                        if (Divisions != null)
                                        {
                                            var substring = p.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + _id.Value.ToString();
                                            sal = ShParvande.Length - 2;
                                            if (ShParvande.Length > 8)
                                            {
                                                string s = ShParvande.Substring(8).PadRight(2, '0');
                                                ShParvande = ShParvande.Substring(0, 8);
                                                mah = Convert.ToInt32(s);
                                            }
                                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                                                , Convert.ToInt32(mablagh), sal, mah);
                                            ShGhabz = gh.ShGhabz;
                                            ShPardakht = gh.ShPardakht;
                                            BarcodeText = gh.BarcodeText;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Avarez.DataSet.DataSet1TableAdapters.QueriesTableAdapter Queries = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                    Queries.PeacokeryUpdate(ShGhabz, ShPardakht, Convert.ToInt64(_id.Value));

                    Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();

                    Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter sp_pic1 = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter();
                    Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter();
                    DataSet.DataSet1.sp_jCalcCarFileDataTable joziat = new DataSet.DataSet1.sp_jCalcCarFileDataTable();
                    joziat = (DataSet.DataSet1.sp_jCalcCarFileDataTable)Session["Joziyat"];
                    foreach (var item in joziat)
                    {
                        dt.sp_jCalcCarFile.Addsp_jCalcCarFileRow((int)item.fldyear, (int)item.fldFirstPrice, (int)item.fldCurectPrice, (int)item.fldValueAdded, (int)item.fldFinalPrice,
                       (int)item.fldFine, (int)item.fldCountMonth, (int)item.fldDiscount, (int)item.fldDept, item.fldCalcDate, (int)item.OtherPrice, (int)item.fldValueAddDiscount,
                       (int)item.fldFineDiscount, (int)item.fldOtherDiscount);
                    }
                    SmsSender sendsms = new SmsSender();
                    sendsms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 4, car.fldID, "", "", "", "");

                    sp_pic1.Fill(dt.sp_PictureSelect1, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                    fish.Fill(dt.sp_PeacockerySelect, "fldId", _id.Value.ToString(), 1, 1, "");
                    var _fish = p.sp_PeacockerySelect("fldId", _id.Value.ToString(), 1, 1, "").FirstOrDefault();

                    sp_pic.Fill(dt.sp_PictureSelect, "fldBankPic", _fish.fldBankId.ToString(), 1, 1, "");
                    var mnu1 = p.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                    FastReport.Report Report = new FastReport.Report();
                    var UpReportSelect = p.sp_UpReportSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                    var FishReport = p.sp_ReportsSelect("fldId", UpReportSelect.fldReportsID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    System.IO.MemoryStream Stream = new System.IO.MemoryStream(FishReport.fldReport);
                    int _Bedehkar = Convert.ToInt32(Session["Bed"]);
                    var State = p.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                    Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                    Report.Load(Stream);
                    Report.RegisterData(dt, "CarTaxDataSet");
                    Report.SetParameterValue("MunicipalityName", mnu1.fldName);
                    Report.SetParameterValue("Barcode", BarcodeText);
                    Report.SetParameterValue("ShGhabz", ShGhabz);
                    Report.SetParameterValue("ShPardakht", ShPardakht);
                    Report.SetParameterValue("SalAvarez","(" + arrang(AvarezSal) + ")");
                    Report.SetParameterValue("Mohlat", MohlatDate);
                    var time = DateTime.Now;
                    Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                    Report.SetParameterValue("Parameter", _Bedehkar);
                    Report.SetParameterValue("StateName", State.fldName);
                    Report.SetParameterValue("AreaName", Session["area"].ToString());
                    Report.SetParameterValue("OfficeName", Session["office"].ToString());
                    Report.SetParameterValue("MyTablighat", MyTablighat.Matn);

                    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                    MemoryStream stream = new MemoryStream();
                    Report.Prepare();
                    Report.Export(pdf, stream);

                    p.Sp_Peacockery_CopyInsert(Convert.ToInt64(_id.Value), stream.ToArray());
                    return File(stream.ToArray(), "application/pdf");
                }
                return null;
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        
        private void sort(int[] a)
        {
            if (a.Length == 0) return;
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = i + 1; j < a.Length; j++)
                {
                    if (a[i] > a[j])
                    {
                        int temp = a[i];
                        a[i] = a[j];
                        a[j] = temp;
                    }
                }
            }
        }

        private string arrang(int[] a)
        {
            sort(a);
            int i;
            if (a.Length == 0) return "()";
            int start = a[0];
            string result = a[0].ToString();
            for (i = 1; i < a.Length - 1; i++)
            {

                if (a[i] - start > 1)
                {
                    result += "," + a[i].ToString();
                }
                else if (a[i + 1] - a[i] > 1)
                {
                    result += "-" + a[i].ToString();
                }
                start = a[i];
            }
            if (a.Length > 1)
            {
                if (a[i] - a[i - 1] > 1)
                {
                    result += "," + a[i].ToString();
                }
                else
                {
                    result += "-" + a[i].ToString();
                }
                start = a[i];
            }
            return result;
        }

        public ActionResult Receipt(int id,int Type)
        {
            Session["ResidId"] = id;
            Session["Type"] = Type;
            return View();
        }

        public ActionResult Savabegh([DataSourceRequest] DataSourceRequest request)
        {
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.sp_CarExperienceSelect("fldCarID", Session["fldCarID"].ToString(), 30, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            //Session.Remove("fldCarID");
            return Json(q);
        }

        public ActionResult Mafasa(int id)
        {
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 249))
            {
                Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
                var car = p.sp_SelectCarDetils(id).FirstOrDefault();
                if (car != null)
                {
                    var Cdate = p.sp_GetDate().FirstOrDefault();
                    int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Cdate.CurrentDateTime).Substring(0, 4));
                    string date = toYear + "/12/29";
                    if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                        date = toYear + "/12/30";
                    System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
                    if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
                    {
                        Transaction Tr = new Transaction();
                        var Div = p.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                        var TransactionInf = p.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                        var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["UserId"]));
                        string msg1 = "";
                        switch (Result)
                        {
                            case Transaction.TransactionResult.Fail:
                                msg1 = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.";
                                return Json(new
                                {
                                    msg = msg1
                                }, JsonRequestBehavior.AllowGet);

                            case Transaction.TransactionResult.NotSharj:
                                msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
                                return Json(new
                                {
                                    msg = msg1
                                }, JsonRequestBehavior.AllowGet);

                        }
                    }
                    var datetime = p.sp_GetDate().FirstOrDefault().CurrentDateTime;
                    var bedehi = p.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                        null, datetime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
                    double mablagh = 0;
                    foreach (var item in bedehi)
                    {
                        mablagh += (int)item.fldDept;
                    }
                    if (mablagh + Convert.ToInt32(_Bed.Value) <= 10000)
                    {
                        Session["CarFileId"] = car.fldID;
                        Session["Sal"] = toYear.ToString().Substring(0, 4);
                        Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                        Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                        Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.rpt_RecoupmentAccountTableAdapter();
                        Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter Receipt = new Avarez.DataSet.DataSet1TableAdapters.rpt_ReceiptTableAdapter();
                        sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                        Receipt.Fill(dt.rpt_Receipt, Convert.ToInt32(car.fldCarID), 2);
                        Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter exp = new Avarez.DataSet.DataSet1TableAdapters.sp_CarExperienceSelectTableAdapter();
                        fish.Fill(dt.rpt_RecoupmentAccount, Convert.ToInt32(Session["CarFileId"]));
                        exp.Fill(dt.sp_CarExperienceSelect, "fldCarFileID", Session["CarFileId"].ToString(), 0, Convert.ToInt32(Session["UserMnu"].ToString()), "");
                        Avarez.Models.ComplicationsCarDBEntities Car = new Avarez.Models.ComplicationsCarDBEntities();
                        Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                        var mnu = Car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                        var State = Car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();

                        System.Data.Entity.Core.Objects.ObjectParameter mafasaId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(string));
                        System.Data.Entity.Core.Objects.ObjectParameter LetterDate = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterDate", typeof(DateTime));
                        System.Data.Entity.Core.Objects.ObjectParameter LetterNum = new System.Data.Entity.Core.Objects.ObjectParameter("fldLetterNum", typeof(string));

                        p.Sp_MafasaInsert(mafasaId, car.fldCarID, Convert.ToInt32(Session["UserMnu"]), null, Convert.ToInt32(Session["UserId"]), LetterDate, LetterNum);
                        string barcode = WebConfigurationManager.AppSettings["SiteURL"] + "/QR_Mafasa/Get/" + mafasaId.Value;

                        FastReport.Report Report = new FastReport.Report();
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_Mafasa.frx");
                        Report.RegisterData(dt, "complicationsCarDBDataSet");
                        Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(LetterDate.Value)));
                        var time = Convert.ToDateTime(LetterDate.Value);
                        Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                        Report.SetParameterValue("Num", LetterNum.Value);
                        Report.SetParameterValue("barcode", barcode);
                        Report.SetParameterValue("MunicipalityName", mnu.fldName);
                        Report.SetParameterValue("StateName", State.fldName);
                        Report.SetParameterValue("AreaName", Session["area"].ToString());
                        Report.SetParameterValue("OfficeName", Session["office"].ToString());
                        Report.SetParameterValue("sal", Session["Sal"]);
                        Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                        Report.SetParameterValue("UserName", Car.sp_UserSelect("fldid", Session["UserId"].ToString(), 0, ""
                , Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldUserName);
                        FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                        MemoryStream stream = new MemoryStream();
                        Report.Prepare();
                        Report.Export(pdf, stream);
                        p.Sp_MafasaUpdate(mafasaId.Value.ToString(), stream.ToArray());
                        return File(stream.ToArray(), "application/pdf");
                    }
                    else
                        return Json("کاربر گرامی شما در حال انجام عملیات غیر قانونی می باشید و در صورت تکرار آی پی شما به پلیس فتا اعلام خواهد شد. باتشکر", JsonRequestBehavior.AllowGet);
                }
                else
                    return null;
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult OfficeRecipt(int id)
        {
            //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 249))
            //{
                Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
                var car = p.sp_SelectCarDetils(id).FirstOrDefault();
                if (car != null)
                {
                    
                    int? mablagh = p.sp_tblOfficeReciptNerkhSelect(6,car.fldCarClassID,5,Convert.ToInt32(Session["UserMnu"])).FirstOrDefault().fldPrice;

                    if (mablagh > 0)
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter residId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(string));
                        System.Data.Entity.Core.Objects.ObjectParameter LetterDate = new System.Data.Entity.Core.Objects.ObjectParameter("fldTarikh", typeof(DateTime));
                        System.Data.Entity.Core.Objects.ObjectParameter LetterNum = new System.Data.Entity.Core.Objects.ObjectParameter("fldShomare", typeof(string));
                        p.Sp_tblOfficeReciptInsert(residId, LetterDate, Convert.ToInt32(Session["UserId"]), car.fldCarID, mablagh, null, LetterNum);
                        
                        Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
                        Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
                        Avarez.DataSet.DataSet1TableAdapters.Sp_tblOfficeReciptSelectTableAdapter Receipt = new Avarez.DataSet.DataSet1TableAdapters.Sp_tblOfficeReciptSelectTableAdapter();
                        sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
                        Receipt.Fill(dt.Sp_tblOfficeReciptSelect, "fldid", residId.Value.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        Avarez.Models.ComplicationsCarDBEntities Car = new Avarez.Models.ComplicationsCarDBEntities();
                        Avarez.Models.MyTablighat MyTablighat = new Models.MyTablighat(Session["UserMnu"].ToString());
                        var mnu = Car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();
                        var State = Car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, 1, "").FirstOrDefault();                        
                        
                        string barcode = WebConfigurationManager.AppSettings["SiteURL"] + "/OfficeRecipt/Get/" + residId.Value;

                        FastReport.Report Report = new FastReport.Report();
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\rpt_OfficeRecipt.frx");
                        Report.RegisterData(dt, "dataSet1");
                        Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(LetterDate.Value)));
                        var time = Convert.ToDateTime(LetterDate.Value);
                        Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
                        Report.SetParameterValue("Num", LetterNum.Value);
                        Report.SetParameterValue("barcode", barcode);
                        Report.SetParameterValue("MunicipalityName", mnu.fldName);
                        Report.SetParameterValue("StateName", State.fldName);
                        Report.SetParameterValue("AreaName", Session["area"].ToString());
                        Report.SetParameterValue("OfficeName", Session["office"].ToString());
                        Report.SetParameterValue("sal", Session["Sal"]);
                        Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
                        Report.SetParameterValue("UserName", Car.sp_UserSelect("fldid", Session["UserId"].ToString(), 0, ""
                        , Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldUserName);
                        FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                        MemoryStream stream = new MemoryStream();
                        Report.Prepare();
                        Report.Export(pdf, stream);
                        p.Sp_tblOfficeReciptUpdate(residId.Value.ToString(), stream.ToArray());
                        return File(stream.ToArray(), "application/pdf");
                    }
                    else
                        return Json("کاربر گرامی شما در حال انجام عملیات غیر قانونی می باشید و در صورت تکرار آی پی شما به پلیس فتا اعلام خواهد شد. باتشکر", JsonRequestBehavior.AllowGet);
                }
                else
                    return null;
            //}
            //else
            //{
            //    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
            //    return RedirectToAction("error", "Metro");
            //}
        }
        public ActionResult Variziha([DataSourceRequest] DataSourceRequest request)
        {
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var q = m.rpt_Receipt(Convert.ToInt32(Session["fldCarID2"]), 2).OrderBy(h=>h.fldCollectionDate).ToList().ToDataSourceResult(request);
            //Session.Remove("fldCarID2");
            return Json(q);
        }

        public ActionResult GoToOnlinePay(decimal Amount, int CarId, int BankId)
        {
            //Amount = 1000;
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            string Tax = "";
            var car = p.sp_SelectCarDetils(CarId).FirstOrDefault();

            System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
            if (BankId != 17 && BankId != 15)
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]));

            if (BankId != 15)
            {
                Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
            }
            else
            {
                p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]));
                var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                Tax = q.fldTemporaryCode;
            }
            if (BankId == 17)
            {

                long peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
                if (peackokeryid == 0)
                {
                    FishReport(CarId);
                    peackokeryid = CheckExistFishForPos(CarId, Convert.ToInt32(Amount));
                }
                if (peackokeryid != 0)
                {
                    var fish = p.sp_PeacockerySelect("fldid", peackokeryid.ToString(), 1, 1, "").FirstOrDefault();
                    if (fish.fldShGhabz != "" && fish.fldShPardakht != "")
                    {
                        p.sp_OnlinePaymentsInsert(_id, BankId, car.fldID, "", false, "", Convert.ToInt32(Session["UserId"]), fish.fldShGhabz + "|" + fish.fldShPardakht, Session["UserPass"].ToString(), Amount, Convert.ToInt32(Session["UserMnu"]));
                        var q = p.sp_OnlinePaymentsSelect("fldId", _id.Value.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                        Tax = BankId.ToString() + _id.Value.ToString().PadLeft(8, '0');
                        Session["shGhabz"] = fish.fldShGhabz;
                        Session["shPardakht"] = fish.fldShPardakht;
                        Session["OnlinefishId"] = fish.fldID;
                    }
                    else
                        return null;
                }
            }
            p.sp_OnlineTemporaryCodePaymentsUpdate(Convert.ToInt32(_id.Value), Tax, Amount, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            if (Session["IsOfficeUser"] != null)
            {
                epishkhan.devpishkhannwsv1 epishkhan = new epishkhan.devpishkhannwsv1();
                var result = epishkhan.servicePay("atJ5+$J1RtFpj", Session["ver_code"].ToString(), Convert.ToInt32(Session["userkey"]), Convert.ToInt32(Session["ServiceId"]), 100);
                if (result > 0)
                    Session["serviceCallSerial"] = result;
                else
                {
                    switch (result)
                    {
                        case -3:
                            return Json("مهلت تراکنش به پایان رسیده است.", JsonRequestBehavior.AllowGet);                            
                        case -7:
                            return Json("مجوز استفاده از این سرویس را ندارید.", JsonRequestBehavior.AllowGet);
                        case -8:
                            return Json("کاربر شما غیر فعال شده است.", JsonRequestBehavior.AllowGet);
                        case -10:
                            return Json("اعتبار شما کافی نیست.", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            Session["Amount"] = Amount;
            Session["Tax"] = Tax;
            Session["ReturnUrl"] = "/Home/CarTax";
            string URL = "";
            if (BankId == 20)
            {
                URL = "CityBank";
            }
            else if (BankId == 1)
            {
                URL = "MeliBank";
            }
            else if (BankId == 2)
            {
                URL = "TejaratBank";
            }
            else if (BankId == 15)
            {
                URL = "Parsian";
            }
            else if (BankId == 17)
            {
                URL = "Saman";
            } 
            return RedirectToAction("Index", URL);
        }
        
        public ActionResult calc()
        {
            int State = 1;
            Models.ComplicationsCarDBEntities m = new Models.ComplicationsCarDBEntities();
            var DateTime = m.sp_GetDate().FirstOrDefault().CurrentDateTime;
            var car = m.sp_CarFileSelect("fldCarId", Session["fldCarID3"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            int toYear = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime).Substring(0, 4));
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";
            System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter _Bed = new System.Data.Entity.Core.Objects.ObjectParameter("Bed", typeof(int));
            if (WebConfigurationManager.AppSettings["InnerExeption"].ToString() == "false")
            {
                Transaction Tr = new Transaction();
                var Div = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                var TransactionInf = m.sp_TransactionInfSelect("fldDivId", Div.CountryDivisionId.ToString(), 0).FirstOrDefault();
                var Result = Tr.RunTransaction(TransactionInf.fldUserName, TransactionInf.fldPass, (int)TransactionInf.CountryType, TransactionInf.fldCountryDivisionsName, Convert.ToInt32(car.fldCarID), Convert.ToInt32(Session["UserId"]));
                string msg1 = "";

                switch (Result)
                {
                    case Transaction.TransactionResult.Fail:
                        msg1 = "تراکنش به یکی از دلایل عدم موجودی کافی یا اطلاعات نامعتبر با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1,
                            State = State
                        }, JsonRequestBehavior.AllowGet);

                    case Transaction.TransactionResult.NotSharj:
                        msg1 = "تراکنش به علت عدم موجودی کافی با موفقیت انجام نشد.";
                        return Json(new
                        {
                            msg = msg1,
                            State = State
                        }, JsonRequestBehavior.AllowGet);

                }
            }
            var bedehi = m.sp_jCalcCarFile(Convert.ToInt32(car.fldID), Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"]), null,
                null, DateTime, Convert.ToInt32(Session["UserId"]), _year, _Bed).ToList();
            //Session["year"] = _year.Value.ToString();
            //Session["bed"] = _Bed.Value.ToString();
            
           
            if (_year.Value.ToString() == "")
            {
                int? mablagh = 0;
                int fldFine = 0, fldValueAddPrice = 0, fldPrice = 0, fldOtherPrice = 0, fldMainDiscount = 0, fldFineDiscount = 0,
                    fldValueAddDiscount = 0, fldOtherDiscount = 0;
                ArrayList Years = new ArrayList();
                DataSet.DataSet1.sp_jCalcCarFileDataTable a = new DataSet.DataSet1.sp_jCalcCarFileDataTable();
                
                foreach (var item in bedehi)
                {
                    mablagh += item.fldDept;
                    fldFine += (int)item.fldFine;
                    fldValueAddPrice += (int)item.fldValueAdded;
                    fldPrice += (int)item.fldCurectPrice;
                    Years.Add(item.fldyear);
                    fldOtherPrice += (int)item.OtherPrice;
                    fldMainDiscount += (int)item.fldDiscount;
                    fldFineDiscount += (int)item.fldFineDiscount;
                    fldValueAddDiscount += (int)item.fldValueAddDiscount;
                    fldOtherDiscount += (int)item.fldOtherDiscount;
                    a.Addsp_jCalcCarFileRow((int)item.fldyear, (int)item.fldFirstPrice, (int)item.fldCurectPrice, (int)item.fldValueAdded, (int)item.fldFinalPrice,
                       (int)item.fldFine, (int)item.fldCountMonth, (int)item.fldDiscount, (int)item.fldDept, item.fldCalcDate, (int)item.OtherPrice, (int)item.fldValueAddDiscount,
                       (int)item.fldFineDiscount, (int)item.fldOtherDiscount);
                }
                   
                int sal = 0, mah = 0;
                mablagh += Convert.ToInt32(_Bed.Value);
                fldPrice += Convert.ToInt32(_Bed.Value);
                Session["mablagh"] = mablagh;
                Session["Fine"] = fldFine;
                Session["ValueAddPrice"] = fldValueAddPrice;
                Session["Price"] = fldPrice;
                Session["Year"] = Years;
                Session["Bed"] = Convert.ToInt32(_Bed.Value);
                Session["OtherPrice"] = fldOtherPrice;
                Session["fldMainDiscount"] = fldMainDiscount;
                Session["fldFineDiscount"] = fldFineDiscount;
                Session["fldValueAddDiscount"] = fldValueAddDiscount;
                Session["fldOtherDiscount"] = fldOtherDiscount;
                Session["Joziyat"] = a;
                if (mablagh < 1000)
                {
                    mablagh = 0;
                    //bedehi = null;
                }
                
                string shGhabz = "", ShParvande="",
                    shPardakht = "",
                    barcode = "";
                //if (Convert.ToInt32(Session["CountryType"]) == 5)
                //{
                //    var mnu = m.sp_MunicipalitySelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                //    {
                //        var Divisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                //        if (Divisions != null)
                //        {
                //            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                //            sal = ShParvande.Length - 2;
                //            if (ShParvande.Length > 8)
                //            {
                //                string s = ShParvande.Substring(8).PadRight(2, '0');
                //                ShParvande = ShParvande.Substring(0, 8);
                //                mah = Convert.ToInt32(s);
                //            }
                //            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                //                , Convert.ToInt32(mablagh), sal, mah);
                //            shGhabz = gh.ShGhabz;
                //            shPardakht = gh.ShPardakht;
                //            barcode = gh.BarcodeText;
                //        }
                //    }
                //}
                //else if (Convert.ToInt32(Session["CountryType"]) == 6)
                //{
                //    var local = m.sp_LocalSelect("fldId", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                //    {
                //        var Divisions = m.sp_GET_IDCountryDivisions(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
                //        if (Divisions != null)
                //        {
                //            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                //            sal = ShParvande.Length - 2;
                //            if (ShParvande.Length > 8)
                //            {
                //                string s = ShParvande.Substring(8).PadRight(2, '0');
                //                ShParvande = ShParvande.Substring(0, 8);
                //                mah = Convert.ToInt32(s);
                //            }
                //            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                //                 , Convert.ToInt32(mablagh), sal, mah);
                //            shGhabz = gh.ShGhabz;
                //            shPardakht = gh.ShPardakht;
                //            barcode = gh.BarcodeText;
                //        }
                //    }
                //}
                //else if (Convert.ToInt32(Session["CountryType"]) == 7)
                //{
                //    var area = m.sp_AreaSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //    if (area != null)
                //    {
                //        if (area.fldLocalID != null)
                //        {
                //            var local = m.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                //            if (local.fldSourceInformatics == "")
                //                local.fldSourceInformatics = "0";
                //            if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                //            {
                //                var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                //                if (Divisions != null)
                //                {
                //                    var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                //                    sal = ShParvande.Length - 2;
                //                    if (ShParvande.Length > 8)
                //                    {
                //                        string s = ShParvande.Substring(8).PadRight(2, '0');
                //                        ShParvande = ShParvande.Substring(0, 8);
                //                        mah = Convert.ToInt32(s);
                //                    }
                //                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                //                        , Convert.ToInt32(mablagh), sal, mah);
                //                    shGhabz = gh.ShGhabz;
                //                    shPardakht = gh.ShPardakht;
                //                    barcode = gh.BarcodeText;
                //                }
                //            }
                //        }
                //        else
                //        {
                //            var mnu = m.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //            if (mnu.fldInformaticesCode == "")
                //                mnu.fldInformaticesCode = "0";
                //            if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                //            {
                //                var Divisions = m.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                //                if (Divisions != null)
                //                {
                //                    var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //                    ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                //                    sal = ShParvande.Length - 2;
                //                    if (ShParvande.Length > 8)
                //                    {
                //                        string s = ShParvande.Substring(8).PadRight(2, '0');
                //                        ShParvande = ShParvande.Substring(0, 8);
                //                        mah = Convert.ToInt32(s);
                //                    }
                //                    ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                //                        , Convert.ToInt32(mablagh), sal, mah);
                //                    shGhabz = gh.ShGhabz;
                //                    shPardakht = gh.ShPardakht;
                //                    barcode = gh.BarcodeText;
                //                }
                //            }
                //        }
                //    }
                //}
                //else if (Convert.ToInt32(Session["CountryType"]) == 8)
                //{
                //    var office = m.sp_OfficesSelect("fldid", Session["CountryCode"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //    if (office != null)
                //    {
                //        if (office.fldAreaID != null)
                //        {
                //            var area = m.sp_AreaSelect("fldid", office.fldAreaID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //            if (area != null)
                //            {
                //                if (area.fldLocalID != null)
                //                {
                //                    var local = m.sp_LocalSelect("fldId", area.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

                //                    if (local.fldSourceInformatics == "")
                //                        local.fldSourceInformatics = "0";
                //                    if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                //                    {
                //                        var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                //                        if (Divisions != null)
                //                        {
                //                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                //                            sal = ShParvande.Length - 2;
                //                            if (ShParvande.Length > 8)
                //                            {
                //                                string s = ShParvande.Substring(8).PadRight(2, '0');
                //                                ShParvande = ShParvande.Substring(0, 8);
                //                                mah = Convert.ToInt32(s);
                //                            }
                //                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                //                                , Convert.ToInt32(mablagh), sal, mah);
                //                            shGhabz = gh.ShGhabz;
                //                            shPardakht = gh.ShPardakht;
                //                            barcode = gh.BarcodeText;
                //                        }
                //                    }
                //                }
                //                else
                //                {
                //                    var mnu = m.sp_MunicipalitySelect("fldId", area.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //                    if (mnu.fldInformaticesCode == "")
                //                        mnu.fldInformaticesCode = "0";
                //                    if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                //                    {
                //                        var Divisions = m.sp_GET_IDCountryDivisions(5, area.fldMunicipalityID).FirstOrDefault();
                //                        if (Divisions != null)
                //                        {
                //                            var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //                            ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                //                            sal = ShParvande.Length - 2;
                //                            if (ShParvande.Length > 8)
                //                            {
                //                                string s = ShParvande.Substring(8).PadRight(2, '0');
                //                                ShParvande = ShParvande.Substring(0, 8);
                //                                mah = Convert.ToInt32(s);
                //                            }
                //                            ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                //                                , Convert.ToInt32(mablagh), sal, mah);
                //                            shGhabz = gh.ShGhabz;
                //                            shPardakht = gh.ShPardakht;
                //                            barcode = gh.BarcodeText;
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //        else
                //        {
                //            var local = m.sp_LocalSelect("fldId", office.fldLocalID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //            if (local != null)
                //            {
                //                if (local.fldSourceInformatics == "")
                //                    local.fldSourceInformatics = "0";
                //                if (Convert.ToInt32(local.fldSourceInformatics) > 0)
                //                {
                //                    var Divisions = m.sp_GET_IDCountryDivisions(6, (int)local.fldID).FirstOrDefault();
                //                    if (Divisions != null)
                //                    {
                //                        var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                //                        sal = ShParvande.Length - 2;
                //                        if (ShParvande.Length > 8)
                //                        {
                //                            string s = ShParvande.Substring(8).PadRight(2, '0');
                //                            ShParvande = ShParvande.Substring(0, 8);
                //                            mah = Convert.ToInt32(s);
                //                        }
                //                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(local.fldSourceInformatics), Convert.ToInt32(local.fldServiceCode)
                //                            , Convert.ToInt32(mablagh), sal, mah);
                //                        shGhabz = gh.ShGhabz;
                //                        shPardakht = gh.ShPardakht;
                //                        barcode = gh.BarcodeText;
                //                    }
                //                }
                //            }
                //            else
                //            {
                //                var mnu = m.sp_MunicipalitySelect("fldId", office.fldMunicipalityID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //                if (mnu.fldInformaticesCode == "")
                //                    mnu.fldInformaticesCode = "0";
                //                if (Convert.ToInt32(mnu.fldInformaticesCode) > 0)
                //                {
                //                    var Divisions = m.sp_GET_IDCountryDivisions(5, office.fldMunicipalityID).FirstOrDefault();
                //                    if (Divisions != null)
                //                    {
                //                        var substring = m.sp_SubSettingSelect("fldCountryDivisionsID", Divisions.CountryDivisionId.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                //                        ShParvande = substring.fldStartCodeBillIdentity.ToString() + car.fldID.ToString();
                //                        sal = ShParvande.Length - 2;
                //                        if (ShParvande.Length > 8)
                //                        {
                //                            string s = ShParvande.Substring(8).PadRight(2, '0');
                //                            ShParvande = ShParvande.Substring(0, 8);
                //                            mah = Convert.ToInt32(s);
                //                        }
                //                        ghabz gh = new ghabz(Convert.ToInt32(ShParvande), Convert.ToInt32(mnu.fldInformaticesCode), Convert.ToInt32(mnu.fldServiceCode)
                //                            , Convert.ToInt32(mablagh), sal, mah);
                //                        shGhabz = gh.ShGhabz;
                //                        shPardakht = gh.ShPardakht;
                //                        barcode = gh.BarcodeText;
                //                    }
                //                }
                //            }
                //        }

                //    }
                //}
                
                return Json(new
                {
                    bedehi = bedehi,
                    mablagh = mablagh,
                    shGhabz = shGhabz,
                    shPardakht = shPardakht,
                    barcode = barcode,
                    msg = "",
                    State = State
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                State = 2;
                string s = "", msg = "", year =_year.Value.ToString();
                for (int i = 0; i < year.Length; i += 4)
                {
                    if (i < year.Length - 4)
                        s += year.Substring(i, 4) + " و ";
                    else
                        s += year.Substring(i, 4);
                }
                msg = "تعرفه سالهای " + s + " تعریف نشده است لطفا جهت اعلام به پشتیبان دکمه ارسال به پشتیبان را انتخاب و تا زمانی که نرخ توسط پشتیبان ثبت شود، منتظر بمانید، سپس دکمه دریافت از سرور را انتخاب کنید و پس از دریافت پیغام تایید، دکمه محاسبه مجدد را از قسمت صورت حساب انتخاب کنید.";
                return Json(new
                {
                    msg = msg,
                    Year = s.Replace(" و ",","),
                    State = State
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //public JsonResult Shpardakht(decimal mablagh, int CarId)
        //{
            

        //}
        public JsonResult Fill()
        {
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            var car = p.sp_SelectCarDetils(Convert.ToInt32(Session["fldCarID1"])).FirstOrDefault();            
           // Session.Remove("fldCarID1");
            return Json(new
            {
                plaq = car.fldPlaquNumber,
                CodeMeli = car.fldMelli_EconomicCode,
                classs = car.fldCarClassName,
                modell = car.fldCarModel,
                syst = car.fldCarSystemName,
                cabin = car.fldCarCabinName,
                account = car.fldCarAccountName,
                make = car.fldCarMakeName,
                Malek = car.fldOwnerName,
                motor = car.fldMotorNumber,
                shasi = car.fldShasiNumber,
                vin = car.fldVIN,
                color = car.fldColor,
                date = car.fldStartDateInsurance,
                datep = car.fldDatePlaque,
                year = car.fldModel,                
                carId = car.fldCarID//carid                
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertInSuporter(string Year, int? fldCarClassId, int? fldCarModelId, int? fldCarSystemId, int? fldCabinTypeId, int? fldCarAccountTypeId, int? fldCarMakeId)
        {
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            //var car = p.sp_SelectCarDetils(Convert.ToInt32(Session["fldCarID3"])).FirstOrDefault();
            var Div = p.sp_CountryDivName(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
            Supporter.SendToSuporter S = new Supporter.SendToSuporter();
            var Code = S.InsertInSupport(Year, fldCarClassId, fldCarModelId, fldCarSystemId, fldCabinTypeId, fldCarAccountTypeId, fldCarMakeId, Div.Name);

            return Json(new
            {
                msg = "درخواست با کد رهگیری "+Code+" به پشتیبان ارسال شد."           
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendToSupporter(string Year, int CarClassId)
        {
            Models.ComplicationsCarDBEntities p = new Models.ComplicationsCarDBEntities();
            int? fldCarClassId = 0;
            int? fldCarModelId = 0;
            int? fldCarSystemId = 0;
            int? fldCabinTypeId = 0;
            int? fldCarAccountTypeId = 0;
            int? fldCarMakeId = 0;

            if (CarClassId == 0)
            {
                var car = p.sp_SelectCarDetils(Convert.ToInt32(Session["fldCarID3"])).FirstOrDefault();
                fldCarClassId = car.fldCarClassID;
                fldCarModelId = car.fldCarModelID;
                fldCarSystemId = car.fldCarSystemID;
                fldCabinTypeId = car.fldCabinTypeID;
                fldCarAccountTypeId = car.fldCarAccountTypeID;
                fldCarMakeId = car.fldCarMakeID;
            }
            else
            {
                fldCarClassId = CarClassId;
                fldCarModelId = p.sp_CarClassSelect("fldId", CarClassId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarModelID;
                fldCarSystemId = p.sp_CarModelSelect("fldId", fldCarModelId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarSystemID;
                var k = p.sp_CarSystemSelect("fldId", fldCarSystemId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                fldCabinTypeId = k.fldCabinTypeID;
                fldCarAccountTypeId = k.fldCarAccountTypeID;
                fldCarMakeId = k.fldCarMakeID;
            }
            ViewBag.Year = Year;
            ViewBag.fldCarClassId = fldCarClassId;
            ViewBag.fldCarModelId = fldCarModelId;
            ViewBag.fldCarSystemId = fldCarSystemId;
            ViewBag.fldCabinTypeId = fldCabinTypeId;
            ViewBag.fldCarAccountTypeId = fldCarAccountTypeId;
            ViewBag.fldCarMakeId = fldCarMakeId;
            return PartialView();
        }
    }
}
