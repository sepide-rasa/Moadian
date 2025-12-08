using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers
{
    public class RegisterController : Controller
    {
        //
        // GET: /Register/

        public ActionResult Index()
        {
            var ImageSetting = ConfigurationManager.AppSettings["ImageSetting"].ToString();
            if (ImageSetting == "1")//تهران
            {
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            }
            else
            {
                return RedirectToAction("index", "Account_New", new { area = "NewVer" });
            }
            return View();
        }
        public FileContentResult generateCaptcha(string dc)
        {
            System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
            CaptchaImage img = new CaptchaImage(90, 40, family);
            string text = img.CreateRandomText(5);
            img.SetText(text);
            img.GenerateImage();
            MemoryStream stream = new MemoryStream();
            img.Image.Save(stream,
            System.Drawing.Imaging.ImageFormat.Png);
            Session["captchaRegister"] = text;
            return File(stream.ToArray(), "jpg");
        }

        public JsonResult GetCascadeLocal(int cboMnu)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Local = car.sp_LocalSelect("fldMunicipalityID", cboMnu.ToString(), 0, 1, "");
            return Json(Local.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeArea(int cboMnu)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var Local = car.sp_AreaSelect("fldMunicipalityID", cboMnu.ToString(), 0, 1, "");
            return Json(Local.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(Models.Sp_RegisterSelect Register, string Captcha)
        {
            try
            {
                if (Session["captchaRegister"].ToString() == Captcha)
                {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                    System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(Int64));
                long? fldLocalId = Register.fldLocalId;
                if (Register.fldLocalId == 0)
                    fldLocalId = null;

                long? fldAreaId = Register.fldAreaId;
                if (Register.fldAreaId == 0)
                    fldAreaId = null;

                var checkDaftar = Car.Sp_RegisterSelect("fldCodeDaftar", Register.fldCodeDaftar.ToString(), 30, 0, "").FirstOrDefault();
                if (checkDaftar != null)
                    return Json(new { data = "دفتر با کد مورد نظر قبلا در سیستم ثبت شده است.", state = 1 });
                else
                {
                    var checkMeli = Car.Sp_RegisterSelect("fldcodeMeli", Register.fldcodeMeli, 30, 0,"").FirstOrDefault();
                    if (checkMeli != null)
                        return Json(new { data = "کدملی مورد نظر قبلا در سیستم ثبت شده است.", state = 1 });
                    else
                    {
                        if (checkCodeMeli(Register.fldcodeMeli) == 1)
                        {
                            //Car.Sp_RegisterInsert(id, Register.fldCodeDaftar, Register.fldAddress,
                            //    Register.fldTel, Register.fldMunId, fldLocalId, fldAreaId, false,
                            //    Register.fldModirDaftar, Register.fldmodirFamily, Register.fldcodeMeli);

                            return Json(new { data = "ثبت نام با موفقیت انجام شد. کد رهگیری: " + id.Value, state = 0 });
                        }
                        else
                            return Json(new { data = "کدملی مورد نظر معتبر نمی باشد.", state = 1 });
                    }
                }
                }
                else
                    return Json(new { data = "کد امنیتی وارد شده نادرست است.", state = 1 });
            }
            catch (Exception x)
            {
                string x1 = x.Message;
                if (x.InnerException != null)
                    x1 += ";" + x.InnerException.Message;
                return Json(new { data = x1, state = 1 });
            }
        }
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
        public ActionResult PrintReport()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 251))
            //{
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "افراد ثبت شده");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            //}
            //else
            //{
            //    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
            //    return RedirectToAction("error", "Metro");
            //}
        }
        public ActionResult RegisterDetail()
        {
            return View();
        }
        public ActionResult Daftar(string Code)
        {
            Models.cartaxEntities c = new Models.cartaxEntities();
            var q = c.Sp_RegisterSelect("fldCodeDaftar", Code, 0, 1, "").FirstOrDefault();
            string name = "کد دفتر نا معتبر است.";
            if (q != null)
                name = q.fldModirDaftar + " " + q.fldmodirFamily;
            return Json(new { Name = name }, JsonRequestBehavior.AllowGet);
        }
    }
}
