using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Avarez.Controllers.Users;
using System.IO;
namespace Avarez.Controllers.CarTax
{
    public class SaveFishController : Controller
    {
        //
        // GET: /SaveFish/ثبت واریزی

        public ActionResult Index(int id)
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 290))
            {
                Session.Remove("savePath");
                Models.cartaxEntities p = new Models.cartaxEntities();
                var q = p.sp_SelectCarDetils(id).FirstOrDefault();
                ViewBag.CarId = q.fldCarID;
                Session["carId"] = q.fldCarID;
                ViewBag.CarfileId = q.fldID;
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->ثبت واریزی");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public FileContentResult showFile(int id)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();

            var image = p.Sp_FilesSelect(id).FirstOrDefault();
            if (image != null)
            {
                if (image.fldImage != null)
                {
                    return File((byte[])image.fldImage, "jpg");
                }
            }
            return null;
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_CollectionSelect("fldCarID", Session["carId"].ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList().ToDataSourceResult(request);
            Session.Remove("carId");
            return Json(q);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldCarID" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_CollectionSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFishPrice(string id)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var q = car.sp_PeacockerySelect("fldid", id, 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
            
            return Json(q.fldShowMoney,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSettleType()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Settle = car.sp_SettleTypeSelect("", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(Settle.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 292))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    if (Convert.ToInt32(id) != 0)
                    {
                        var q = Car.sp_CollectionSelect("fldId",id,1,Convert.ToInt32(Session["UserId"]),"").FirstOrDefault();
                        if (q.fldPeacockeryCode == null)
                        {
                            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 320))
                            {
                                Car.sp_CollectionDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                                return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
                            }
                            else
                            {
                                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                                return RedirectToAction("error", "Metro");
                            }
                        }
                        else
                        {
                            Car.sp_CollectionDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                            return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
                        }
                    }
                    else
                    {
                        return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 });
                    }
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
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
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public ActionResult Save(Models.sp_CollectionSelect Collection)
        {
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                if (Collection.fldDesc == null)
                    Collection.fldDesc = "";
                if (Collection.fldSerialBarChasb == null)
                    Collection.fldSerialBarChasb = "";
                if (Collection.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 291))
                    {
                        if (Collection.fldPeacockeryCode != null)
                        {
                            var fish = Car.sp_PeacockerySelect("fldId", Collection.fldPeacockeryCode.ToString(), 1,
                                Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            if (fish != null)
                            {
                                var carIdfish = Car.sp_CarFileSelect("fldId", fish.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarID;
                                var carIdcollection = Car.sp_CarFileSelect("fldId", Collection.fldCarFileID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault().fldCarID;
                                /*if (fish.fldCarFileID == Collection.fldCarFileID)*/
                                if (carIdfish == carIdcollection)
                                {
                                    var q = Car.sp_CollectionSelect("fldPeacockeryCode", Collection.fldPeacockeryCode.ToString(), 1,
                                        Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                                    if (q == null)
                                    {
                                        System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", sizeof(int));
                                        Car.sp_CollectionInsert(_id, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                            Collection.fldPrice, Collection.fldSettleTypeID, (int)Collection.fldPeacockeryCode, null,
                                             Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]), Collection.fldDesc,
                                             Session["UserPass"].ToString(), "", null, "", null, null, true, 1, DateTime.Now);
                                        Car.SaveChanges();
                                        SmsSender sms = new SmsSender();
                                        sms.SendMessage(Convert.ToInt32(Session["UserMnu"]), 3, fish.fldCarFileID, Collection.fldPrice, "", "", "");
                                        return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                                    }
                                    else
                                    {
                                        return Json(new { data = "فیش فعلی قبلا در سیستم ثبت گردیده است.", state = 1 });
                                    }
                                }
                                else
                                    return Json(new { data = "فیش فعلی مربوط به این پرونده نمی باشد.", state = 1 });
                            }
                            else
                                return Json(new { data = "فیش فعلی در سیستم وجود ندارد.", state = 1 });
                        }
                        else
                        {
                            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 318))
                            {
                                if (Session["savePath"] != null)
                                {
                                    System.Data.Entity.Core.Objects.ObjectParameter a = new System.Data.Entity.Core.Objects.ObjectParameter("fldid", typeof(int));
                                    string savePath = Session["savePath"].ToString();

                                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                                    var Ex = Path.GetExtension(savePath);
                                    if (Ex == ".tiff" || Ex == ".tif")
                                    {
                                        using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                        {
                                            image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                        }
                                    }
                                    byte[] _File = stream.ToArray();

                                    Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), true, null, Car.sp_GetDate().FirstOrDefault().CurrentDateTime);
                                    System.IO.File.Delete(savePath);
                                    Session.Remove("savePath");
                                    int? fileid = null;
                                    if (a.Value != null)
                                        fileid = Convert.ToInt32(a.Value);
                                    if (Collection.fldMunId == 0 || Collection.fldMunId == null)
                                        return Json(new { data = "لطفا شهرداری را مشخص کنید.", state = 1 });
                                    if (Collection.fldSerialFish == "" || Collection.fldSerialFish == "0" || Collection.fldSerialFish == null)
                                        return Json(new { data = "لطفا شماره سریال فیش را مشخص کنید.", state = 1 });

                                    System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", sizeof(int));
                                    Car.sp_CollectionInsert(_id, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                        Collection.fldPrice, Collection.fldSettleTypeID, null, null,
                                         Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"]),
                                         Collection.fldDesc, Session["UserPass"].ToString(), "",
                                         Collection.fldMunId, Collection.fldSerialFish, null, fileid, false, null, null);
                                    Car.SaveChanges();
                                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                                }
                                else
                                    return Json(new { data = "لطفا فایل مدرک را آپلود کنید.", state = 1 });
                            }
                            else
                            {
                                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                                return RedirectToAction("error", "Metro");
                            }
                        }
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
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
                                    return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                                }
                                else
                                    return Json(new { data = "فیش فعلی مربوط به این پرونده نمی باشد.", state = 1 });
                            }
                            else
                                return Json(new { data = "فیش فعلی در سیستم وجود ندارد.", state = 1 });
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
                                    if (Ex == ".tiff" || Ex == ".tif")
                                    {
                                        using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(savePath))
                                        {
                                            image.Save(stream, new Aspose.Imaging.ImageOptions.JpegOptions());
                                        }
                                    }
                                    byte[] _File = stream.ToArray();

                                    Car.Sp_FilesInsert(a, _File, Convert.ToInt32(Session["UserId"]), true, null, Car.sp_GetDate().FirstOrDefault().CurrentDateTime);
                                    System.IO.File.Delete(savePath);

                                   

                                    if (a.Value != null)
                                        fileid = Convert.ToInt32(a.Value);
                                }
                                else if (Collection.fldFileId != null)
                                    fileid = Collection.fldFileId;
                                else
                                    return Json(new { data = "لطفا فایل مدرک را آپلود کنید.", state = 1 });

                                if (Collection.fldMunId == 0 || Collection.fldMunId == null)
                                    return Json(new { data = "لطفا شهرداری را مشخص کنید.", state = 1 });
                                if (Collection.fldSerialFish == "" || Collection.fldSerialFish == "0" || Collection.fldSerialFish == null)
                                    return Json(new { data = "لطفا شماره سریال فیش را مشخص کنید.", state = 1 });
                                System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", sizeof(int));
                                Car.sp_CollectionUpdate(Collection.fldID, Collection.fldCarFileID, MyLib.Shamsi.Shamsi2miladiDateTime(Collection.fldCollectionDate),
                                    Collection.fldPrice, Collection.fldSettleTypeID, null, null,
                                     Collection.fldSerialBarChasb, Convert.ToInt32(Session["UserId"])
                                     , Collection.fldDesc, Session["UserPass"].ToString(), "",
                                     Collection.fldMunId, Collection.fldSerialFish, null, fileid);
                                Car.SaveChanges();
                                if (Collection.fldFileId != null && Session["savePath"] != null)
                                    Car.Sp_FilesDelete(Collection.fldFileId);
                                Session.Remove("savePath");
                                return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                            }
                            else
                            {
                                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                                return RedirectToAction("error", "Metro");
                            }
                        }
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }
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
                return Json(new { data = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", state = 1 });
            }
        }

        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var q = Car.sp_CollectionSelect("fldId", id.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var p = Car.sp_MunicipalitySelect("fldId", q.fldMunId.ToString(), 1, 1, "").FirstOrDefault();
                var payType = "1";
                if (q.fldPeacockeryCode == null)
                    payType = "2";
                int munid=0;
                string munName="";
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
                    fldSettleTypeID = q.fldSettleTypeID,
                    fldCollectionDate = q.fldCollectionDate,
                    fldSerialBarChasb = q.fldSerialBarChasb,
                    fldPayType = payType,
                    fldSerialFish = q.fldSerialFish,
                    fldName = munName,
                    fldMunId = munid,
                    fldFileId=q.fldFileId
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
