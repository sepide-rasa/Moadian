using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Areas.NewVer.Controllers
{
    public class Register_NewController : Controller
    {
        //
        // GET: /NewVer/Register_New/

        public ActionResult Index(string ImageSetting)//آخرین تغییرات
        {
            if (ImageSetting == "1")
            {
                return View("indexTehran");
            }
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ImageSetting = ImageSetting;
            return PartialView;
        }

        [AllowAnonymous]
        public FileContentResult generateCaptcha(string dc)
        {
            System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
            CaptchaImage img = new CaptchaImage(90, 40, family);
            string text = img.CreateRandomText(5);
            text = text.ToUpper();
            text = text.Replace("O", "P").Replace("0", "2").Replace("1", "3").Replace("I", "M");
            img.SetText(text);
            img.GenerateImage();
            MemoryStream stream = new MemoryStream();
            img.Image.Save(stream,
            System.Drawing.Imaging.ImageFormat.Png);
            Session["captchaRegister"] = text;
            return File(stream.ToArray(), "jpg");
        }
        public JsonResult GetCascadeState()
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            return Json(car.sp_StateSelect("", "", 0, 1, "").OrderBy(x => x.fldName).Select(c => new { fldID = c.fldID, fldName = c.fldName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeCounty(int cboState)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_RegionTree(1, cboState, 5).ToList().OrderBy(h => h.NodeName);
            return Json(County.Select(p1 => new { fldID = p1.SourceID, fldName = p1.NodeName }), JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Save(Models.Sp_RegisterSelect Register, string Captcha)
        //{
        //    try
        //    {
        //        if (Session["captchaRegister"].ToString().ToLower() == Captcha.ToLower())
        //        {
        //            Models.cartaxEntities Car = new Models.cartaxEntities();
        //            System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(Int64));
        //            long? fldLocalId = Register.fldLocalId;
        //            if (Register.fldLocalId == 0)
        //                fldLocalId = null;

        //            long? fldAreaId = Register.fldAreaId;
        //            if (Register.fldAreaId == 0)
        //                fldAreaId = null;
        //            if (Register.fldCodeEghtesadi == null)
        //                Register.fldCodeEghtesadi = "";
        //            if (Register.fldSh_Sabt == null)
        //                Register.fldSh_Sabt = "";
        //            var checkDaftar = Car.Sp_RegisterSelect("fldCodeDaftar", Register.fldCodeDaftar.ToString(), 30, 0, "").FirstOrDefault();
        //            if (checkDaftar != null)
        //                return Json(new { Msg = "دفتر با کد مورد نظر قبلا در سیستم ثبت شده است.", MsgTitle = "خطا", Er = 1 });
        //            else
        //            {
        //                var checkMeli = Car.Sp_RegisterSelect("fldcodeMeli", Register.fldcodeMeli, 30, 0, "").Where(l => l.fldTypePerson == Register.fldTypePerson).FirstOrDefault();
        //                if (checkMeli != null && checkMeli.fldTypePerson == true)
        //                    return Json(new { Msg = "کدملی مورد نظر قبلا در سیستم ثبت شده است.", MsgTitle = "خطا", Er = 1 });
        //                else if (checkMeli != null && checkMeli.fldTypePerson == false)
        //                    return Json(new { Msg = "شناسه ملی مورد نظر قبلا در سیستم ثبت شده است.", MsgTitle = "خطا", Er = 1 });
        //                else
        //                {
        //                    if (Register.fldTypePerson == true)
        //                    {
        //                        if (checkCodeMeli(Register.fldcodeMeli) == 1)
        //                        {
        //                            Car.Sp_RegisterInsert(id, Register.fldCodeDaftar, Register.fldAddress,
        //                                Register.fldTel, Register.fldMunId, fldLocalId, fldAreaId, false,
        //                                Register.fldModirDaftar, Register.fldmodirFamily, Register.fldcodeMeli, Register.fldCodeEghtesadi, Register.fldSh_Sabt, Register.fldTypePerson, Register.fldMobile);

        //                            return Json(new { Msg = "ثبت نام با موفقیت انجام شد. کد رهگیری: " + id.Value, MsgTile = "عملیات موفق", Er = 0 });
        //                        }
        //                        else
        //                            return Json(new { Msg = "کدملی مورد نظر معتبر نمی باشد.", MsgTile = "خطا", Er = 1 });
        //                    }
        //                    else
        //                    {
        //                        Car.Sp_RegisterInsert(id, Register.fldCodeDaftar, Register.fldAddress,
        //                               Register.fldTel, Register.fldMunId, fldLocalId, fldAreaId, false,
        //                               Register.fldModirDaftar, Register.fldmodirFamily, Register.fldcodeMeli, Register.fldCodeEghtesadi, Register.fldSh_Sabt, Register.fldTypePerson, Register.fldMobile);

        //                        return Json(new { Msg = "ثبت نام با موفقیت انجام شد. کد رهگیری: " + id.Value, MsgTile = "عملیات موفق", Er = 0 });
        //                    }
        //                }
        //            }
        //        }
        //        else
        //            return Json(new { Msg = "کد امنیتی وارد شده نادرست است.", MsgTile = "خطا", Er = 1 });
        //    }
        //    catch (Exception x)
        //    {
        //        string x1 = x.Message;
        //        if (x.InnerException != null)
        //            x1 += ";" + x.InnerException.Message;
        //        return Json(new { Msg = x1, MsgTile = "خطا", Er = 1 });
        //    }
        //}
        public ActionResult Save(Models.Sp_RegisterSelect Register, string Captcha)
        {
            try
            {
                if (Session["captchaRegister"].ToString().ToLower() == Captcha.ToLower())
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                    System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(Int64));
                    long? fldLocalId = Register.fldLocalId;
                    if (Register.fldLocalId == 0)
                        fldLocalId = null;

                    long? fldAreaId = Register.fldAreaId;
                    if (Register.fldAreaId == 0)
                        fldAreaId = null;
                    if (Register.fldCodeEghtesadi == null)
                        Register.fldCodeEghtesadi = "";
                    if (Register.fldSh_Sabt == null)
                        Register.fldSh_Sabt = "";
                    var checkDaftar = Car.Sp_RegisterSelect("fldCodeDaftar", Register.fldCodeDaftar.ToString(), 30, 0, "").FirstOrDefault();
                    if (checkDaftar != null)
                        return Json(new { Msg = "دفتر با کد مورد نظر قبلا در سیستم ثبت شده است.", MsgTitle = "خطا", Er = 1 });
                    else
                    {
                        var checkMeli = Car.Sp_RegisterSelect("fldcodeMeli", Register.fldcodeMeli, 30, 0, "").Where(l => l.fldTypePerson == Register.fldTypePerson).FirstOrDefault();
                        if (checkMeli != null && checkMeli.fldTypePerson == true)
                            return Json(new { Msg = "کدملی مورد نظر قبلا در سیستم ثبت شده است.", MsgTitle = "خطا", Er = 1 });
                        else if (checkMeli != null && checkMeli.fldTypePerson == false)
                            return Json(new { Msg = "شناسه ملی مورد نظر قبلا در سیستم ثبت شده است.", MsgTitle = "خطا", Er = 1 });
                        else
                        {
                            if (Register.fldTypePerson == true)
                            {
                                if (checkCodeMeli(Register.fldcodeMeli) == 1)
                                {
                                    Car.Sp_RegisterInsert(id, Register.fldCodeDaftar, Register.fldAddress,
                                        Register.fldTel, Register.fldMunId, fldLocalId, fldAreaId, false,
                                        Register.fldModirDaftar, Register.fldmodirFamily, Register.fldcodeMeli,
                                        Register.fldCodeEghtesadi, Register.fldSh_Sabt, Register.fldTypePerson, Register.fldMobile, Register.fldExpireDate);

                                    return Json(new { Msg = "ثبت نام با موفقیت انجام شد. کد رهگیری: " + id.Value, MsgTile = "عملیات موفق", Er = 0 });
                                }
                                else
                                    return Json(new { Msg = "کدملی مورد نظر معتبر نمی باشد.", MsgTile = "خطا", Er = 1 });
                            }
                            else
                            {
                                Car.Sp_RegisterInsert(id, Register.fldCodeDaftar, Register.fldAddress,
                                       Register.fldTel, Register.fldMunId, fldLocalId, fldAreaId, false,
                                       Register.fldModirDaftar, Register.fldmodirFamily, Register.fldcodeMeli,
                                       Register.fldCodeEghtesadi, Register.fldSh_Sabt, Register.fldTypePerson, Register.fldMobile, Register.fldExpireDate);

                                return Json(new { Msg = "ثبت نام با موفقیت انجام شد. کد رهگیری: " + id.Value, MsgTile = "عملیات موفق", Er = 0 });
                            }
                        }
                    }
                }
                else
                    return Json(new { Msg = "کد امنیتی وارد شده نادرست است.", MsgTile = "خطا", Er = 1 });
            }
            catch (Exception x)
            {
                string x1 = x.Message;
                if (x.InnerException != null)
                    x1 += ";" + x.InnerException.Message;
                return Json(new { Msg = x1, MsgTile = "خطا", Er = 1 });
            }
        }
        //public ActionResult Save(Models.Sp_RegisterSelect Register, string Captcha)
        //{
        //    try
        //    {
        //        if (Session["captchaRegister"].ToString().ToLower() == Captcha.ToLower())
        //        {
        //            Models.cartaxEntities Car = new Models.cartaxEntities();
        //            System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(Int64));
        //            long? fldLocalId = Register.fldLocalId;
        //            if (Register.fldLocalId == 0)
        //                fldLocalId = null;

        //            long? fldAreaId = Register.fldAreaId;
        //            if (Register.fldAreaId == 0)
        //                fldAreaId = null;

        //            var checkDaftar = Car.Sp_RegisterSelect("fldCodeDaftar", Register.fldCodeDaftar.ToString(), 30, 0, "").FirstOrDefault();
        //            if (checkDaftar != null)
        //                return Json(new { Msg = "دفتر با کد مورد نظر قبلا در سیستم ثبت شده است.", MsgTitle = "خطا", Er = 1 });
        //            else
        //            {
        //                var checkMeli = Car.Sp_RegisterSelect("fldcodeMeli", Register.fldcodeMeli, 30, 0, "").FirstOrDefault();
        //                if (checkMeli != null)
        //                    return Json(new { Msg = "کدملی مورد نظر قبلا در سیستم ثبت شده است.", MsgTitle = "خطا", Er = 1 });
        //                else
        //                {
        //                    if (checkCodeMeli(Register.fldcodeMeli) == 1)
        //                    {
        //                        //Car.Sp_RegisterInsert(id, Register.fldCodeDaftar, Register.fldAddress,
        //                        //    Register.fldTel, Register.fldMunId, fldLocalId, fldAreaId, false,
        //                        //    Register.fldModirDaftar, Register.fldmodirFamily, Register.fldcodeMeli);

        //                        return Json(new { Msg = "ثبت نام با موفقیت انجام شد. کد رهگیری: " + id.Value, MsgTile = "عملیات موفق", Er = 0 });
        //                    }
        //                    else
        //                        return Json(new { Msg = "کدملی مورد نظر معتبر نمی باشد.", MsgTile = "خطا", Er = 1 });
        //                }
        //            }
        //        }
        //        else
        //            return Json(new { Msg = "کد امنیتی وارد شده نادرست است.", MsgTile = "خطا", Er = 1 });
        //    }
        //    catch (Exception x)
        //    {
        //        string x1 = x.Message;
        //        if (x.InnerException != null)
        //            x1 += ";" + x.InnerException.Message;
        //        return Json(new { Msg = x1, MsgTile = "خطا", Er = 1 });
        //    }
        //}
        public int checkCodeMeli(string codec)
        {
            char[] chArray = codec.ToCharArray();
            int[] numArray = new int[chArray.Length];
            string x = codec;
            switch (x)
            {
                case "0000000000":
                case "1111111111":
                case "2222222222":
                case "3333333333":
                case "4444444444":
                case "5555555555":
                case "6666666666":
                case "7777777777":
                case "8888888888":
                case "9999999999":
                case "0123456789":
                case "9876543210":

                    return 0;
                    break;
            }
            for (int i = 0; i < chArray.Length; i++)
            {
                numArray[i] = (int)char.GetNumericValue(chArray[i]);
            }
            int num2 = numArray[9];

            int num3 = ((((((((numArray[0] * 10) + (numArray[1] * 9)) + (numArray[2] * 8)) + (numArray[3] * 7)) + (numArray[4] * 6)) + (numArray[5] * 5)) + (numArray[6] * 4)) + (numArray[7] * 3)) + (numArray[8] * 2);
            int num4 = num3 - ((num3 / 11) * 11);
            if ((((num4 == 0) && (num2 == num4)) || ((num4 == 1) && (num2 == 1))) || ((num4 > 1) && (num2 == Math.Abs((int)(num4 - 11)))))
            {
                return 1;
            }
            else
            {
                return 0;

            }
        }
    }
}
