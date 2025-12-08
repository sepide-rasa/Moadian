using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Avarez.Controllers.Users;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class SabtVariziController : Controller
    {
        //
        // GET: /NewVer/SabtVarizi/

        public ActionResult Index(string containerId, string CarId, string CarFileId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 290))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->ثبت واریزی");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo
                };
                result.ViewBag.CarId = CarId;
                result.ViewBag.CarFileId = CarFileId;
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return result;
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

        public ActionResult HelpSabtVarizi()
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }
        }

        public ActionResult GetSettleType()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Settle = car.sp_SettleTypeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(Settle.Select(p1 => new { SettleTypeID = p1.fldID, SettleTypeName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Upload()
        {
            string Msg = "";
            try
            {
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    System.IO.File.Delete(physicalPath);
                }
                var extension=Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg")
                {
                    if (Request.Files[0].ContentLength <= 5242880)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                        file.SaveAs(savePath);
                        Session["savePath"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "حجم فایل انتخابی می بایست کمتر از 5 مگابایت باشد."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }

        }

        public ActionResult GetPrice(string Id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            int Money=0;
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_PeacockerySelect("fldId", Id, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            if (q != null)
            {
                Money = q.fldShowMoney;
            }

            return Json(Money, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters, string CarId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities p = new Models.cartaxEntities();
            List<Models.sp_CollectionSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.sp_CollectionSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldCollectionDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCollectionDate";
                            break;
                        case "fldSettleTypeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSettleTypeName";
                            break;
                        case "fldPrice":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPrice";
                            break;
                        case "fldPeacockeryCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPeacockeryCode";
                            break;
                        case "fldSerialBarChasb":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSerialBarChasb";
                            break;
                    }
                    if (data != null)
                        data1 =p.sp_CollectionSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(l=>l.fldCarId==Convert.ToInt32(CarId)).ToList();
                    else
                        data = p.sp_CollectionSelect(field, searchtext, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).Where(l => l.fldCarId == Convert.ToInt32(CarId)).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.sp_CollectionSelect("fldCarID", CarId, 100, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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

            List<Models.sp_CollectionSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Save(Models.sp_CollectionSelect Collection)
        {
            string MsgTitle = ""; string Msg = ""; byte Er = 0;
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Collection.fldDesc == null)
                    Collection.fldDesc = "";
                if (Collection.fldSerialBarChasb == null)
                    Collection.fldSerialBarChasb = "";
                if (Collection.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 291))
                    {
                        /*if (Collection.fldPeacockeryCode != null)*/
                        if (Collection.fldMunId == 0)//پرداخت عادی
                        {
                            var fish = Car.sp_PeacockerySelect("fldId", Collection.fldPeacockeryCode.ToString(), 1,
                                Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            if (fish != null)
                            {
                                var carIdfish = Car.sp_CarFileSelect("fldId", fish.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarID;
                                var carIdcollection = Car.sp_CarFileSelect("fldId", Collection.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarID;
                                if (carIdfish == carIdcollection)
                                /*if (fish.fldCarFileID == Collection.fldCarFileID)*/
                                {
                                    var q = Car.sp_CollectionSelect("fldPeacockeryCode", Collection.fldPeacockeryCode.ToString(), 1,
                                        Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                    if (q == null)
                                    {
                                        System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                                        Car.sp_CollectionInsert(_id, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                            Collection.fldPrice, Collection.fldSettleTypeID, (int)Collection.fldPeacockeryCode, null,
                                             Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]),
                                             Collection.fldDesc, Session["UserPass"].ToString(), "", null, "", null, null, true, 1, DateTime.Now);
                                        SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));

                                        /*Car.SaveChanges();*/
                                        SmsSender sms = new SmsSender();
                                        sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, Collection.fldPrice, "", "", "");
                                        return Json(new { MsgTitle="ذخیره موفق",Msg = "ذخیره با موفقیت انجام شد.",Er=Er});
                                    }
                                    else
                                    {
                                        return Json(new { MsgTitle="خطا",Msg = "فیش فعلی قبلا در سیستم ثبت گردیده است.", Er = 1 });
                                    }
                                }
                                else
                                    return Json(new { MsgTitle="خطا",Msg = "فیش فعلی مربوط به این پرونده نمی باشد.", Er = 1 });
                            }
                            else
                                return Json(new { MsgTitle = "خطا", Msg = "فیش فعلی در سیستم وجود ندارد.", Er = 1 });
                        }
                        else
                        {//پرداخت علی الحساب
                            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 318))
                            {
                                if (Session["savePath"] != null)
                                {
                                    System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                                    string savePath = Session["savePath"].ToString();

                                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                    var Ex = Path.GetExtension(savePath);
                                    byte[] _File = stream.ToArray();

                                    Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), null, null,null);
                                    System.IO.File.Delete(savePath);
                                    Session.Remove("savePath");
                                    int? fileid = null;
                                    if (a.Value != null)
                                        fileid = Convert.ToInt32(a.Value);
                                    if (Collection.fldMunId == 0 || Collection.fldMunId == null)
                                        return Json(new {MsgTitle="خطا", Msg = "لطفا شهرداری را مشخص کنید.", Er = 1 });
                                    if (Collection.fldSerialFish == "" || Collection.fldSerialFish == "0" || Collection.fldSerialFish == null)
                                        return Json(new { MsgTitle = "خطا", Msg = "لطفا شماره سریال فیش را مشخص کنید.", Er = 1 });

                                    System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", sizeof(int));
                                    Car.sp_CollectionInsert(_id, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                        Collection.fldPrice, Collection.fldSettleTypeID, null, null,
                                         Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]),
                                         Collection.fldDesc, Session["UserPass"].ToString(), "",
                                         Collection.fldMunId, Collection.fldSerialFish, null, fileid, false, null, null);
                                    if (Collection.fldMunId == Convert.ToInt32(Session["UserMnu"]))
                                        SendToSamie.Send(Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserMnu"]));
                                    return Json(new { MsgTitle="ذخیره موفق",Msg = "ذخیره با موفقیت انجام شد.", Er = Er });
                                }
                                else
                                    return Json(new {MsgTitle="خطا" ,Msg = "لطفا فایل مدرک را آپلود کنید.", Er = 1 });
                            }
                            else
                            {
                                MsgTitle = "خطا";
                                Msg = "شما مجاز به دسترسی نمی باشید.";
                                Er = 1;
                            }
                        }
                    }
                    else
                    {
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به دسترسی نمی باشید.";
                        Er = 1;
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 293))
                    {
                        if (Collection.fldPeacockeryCode != null)
                        {
                            var fish = Car.sp_PeacockerySelect("fldId", Collection.fldPeacockeryCode.ToString(), 1,
                               Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            if (fish != null)
                            {
                                var carIdfish = Car.sp_CarFileSelect("fldId", fish.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarID;
                                var carIdcollection = Car.sp_CarFileSelect("fldId", Collection.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarID;
                                if (carIdfish == carIdcollection)
                                /*if (fish.fldCarFileID == Collection.fldCarFileID)*/
                                {
                                    Car.sp_CollectionUpdate(Collection.fldID, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                        Collection.fldPrice, Collection.fldSettleTypeID, (int)Collection.fldPeacockeryCode, null,
                                        Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]), Collection.fldDesc,
                                        Session["UserPass"].ToString(), "", null, "", null, null);
                                    return Json(new {MsgTitle="ویرایش موفق", Msg = "ویرایش با موفقیت انجام شد.", Er = Er });
                                }
                                else
                                    return Json(new { MsgTitle="خطا",Msg = "فیش فعلی مربوط به این پرونده نمی باشد.", Er = 1 });
                            }
                            else
                                return Json(new { MsgTitle="خطا",Msg = "فیش فعلی در سیستم وجود ندارد.", Er = 1 });
                        }
                        else
                        {
                            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 319))
                            {
                                int? fileid = null;
                                if (Session["savePath"] != null)
                                {
                                    System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                                    string savePath = Session["savePath"].ToString();

                                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                    var Ex = Path.GetExtension(savePath);
                                    byte[] _File = stream.ToArray();

                                    Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), null, null, null);
                                    System.IO.File.Delete(savePath);

                                    if (a.Value != null)
                                        fileid = Convert.ToInt32(a.Value);
                                }
                                else if (Collection.fldFileId != null)
                                    fileid = Collection.fldFileId;
                                else
                                    return Json(new { MsgTitle="خطا",Msg = "لطفا فایل مدرک را آپلود کنید.", Er = 1 });

                                if (Collection.fldMunId == 0 || Collection.fldMunId == null)
                                    return Json(new { MsgTitle = "خطا", Msg = "لطفا شهرداری را مشخص کنید.", Er = 1 });
                                if (Collection.fldSerialFish == "" || Collection.fldSerialFish == "0" || Collection.fldSerialFish == null)
                                    return Json(new { MsgTitle = "خطا", Msg = "لطفا شماره سریال فیش را مشخص کنید.", Er = 1 });
                                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", sizeof(int));
                                Car.sp_CollectionUpdate(Collection.fldID, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                    Collection.fldPrice, Collection.fldSettleTypeID, null, null,
                                     Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"])
                                     , Collection.fldDesc, Session["UserPass"].ToString(), "",
                                     Collection.fldMunId, Collection.fldSerialFish, null, fileid);

                                if (Collection.fldFileId != null && Session["savePath"] != null)
                                    Car.Sp_FilesDelete(Collection.fldFileId);
                                Session.Remove("savePath");
                                return Json(new { MsgTitle = "ویرایش موفق", Msg = "ویرایش با موفقیت انجام شد.", Er = Er });
                            }
                            else
                            {
                                MsgTitle = "خطا";
                                Msg = "شما مجاز به دسترسی نمی باشید.";
                                Er = 1;
                            }
                        }
                    }
                    else
                    {
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به دسترسی نمی باشید.";
                        Er = 1;
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
                return Json(new { MsgTitle="خطا",Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int Id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_CollectionSelect("fldId", Id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var p = Car.sp_MunicipalitySelect("fldId", q.fldMunId.ToString(), 1, 1, "").FirstOrDefault();
                var payType = "2";
                if (q.fldMunId == null)
                    payType = "1";
                int munid = 0;
                string munName = "";
                if (p != null)
                {
                    munid = p.fldID;
                    munName = p.fldName;
                }
                return Json(new
                {
                    fldId = q.fldID,
                    fldDesc = q.fldDesc,
                    fldPeacockeryCode = q.fldPeacockeryCode,
                    fldPrice = q.fldPrice,
                    fldSettleTypeID = q.fldSettleTypeID.ToString(),
                    fldCollectionDate = q.fldCollectionDate,
                    fldSerialBarChasb = q.fldSerialBarChasb,
                    fldPayType = payType,
                    fldSerialFish = q.fldSerialFish,
                    fldName = munName,
                    fldMunId = munid,
                    fldFileId = q.fldFileId,
                    Er=0
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
                return Json(new { MsgTitle="خطا",Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }

        public ActionResult Delete(string Id)
        {//حذف یک رکورد
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 292))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(Id) != 0)
                    {
                        var q = Car.sp_CollectionSelect("fldId", Id, 1, Convert.ToInt32(Session["UserId"]), "").FirstOrDefault();
                        if (q.fldPeacockeryCode == null)
                        {
                            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 320))
                            {
                                Car.sp_CollectionDelete(Convert.ToInt32(Id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                                return Json(new { MsgTitle="حذف موفق",Msg = "حذف با موفقیت انجام شد.", Er = 0 });
                            }
                            else
                            {
                                return Json(new { MsgTitle = "خطا", Msg = "شما مجاز به دسترسی نمی باشید.", Er = 1 });
                            }
                        }
                        else
                        {
                            Car.sp_CollectionDelete(Convert.ToInt32(Id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                            return Json(new { MsgTitle="حذف موفق",Msg = "حذف با موفقیت انجام شد.", Er = 0 });
                        }
                    }
                    else
                    {
                        return Json(new { MsgTitle="خطا",Msg = "رکوردی برای حذف انتخاب نشده است.", Er = 1 });
                    }
                }
                else
                {
                    return Json(new { MsgTitle = "خطا", Msg = "شما مجاز به دسترسی نمی باشید.", Er = 1 });
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new { MsgTitle="خطا",Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }
        public ActionResult CheckTaiidPardakhtha(int id)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var carCollection = p.sp_CollectionSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
            bool? HaveTaiid = true;
            //if (carEx.fldFileId != null)
            HaveTaiid = carCollection.fldAccept;
            return Json(new
            {
                HaveTaiid = HaveTaiid,
                userId = Session["UserId"].ToString()
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
